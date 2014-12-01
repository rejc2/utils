using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using System.Drawing;
using System.Threading;

namespace Rejc2.Utils.Web
{
	public abstract class WebsiteDownloadBase
	{
		static readonly object s_Lock = new object();
		static readonly Dictionary<Type, DateTime> s_WaitUntil = new Dictionary<Type, DateTime>();
		static readonly Random s_Random = new Random();

		protected void WaitAWhile()
		{
			DateTime waitUntil;

			lock (s_Lock)
			{
				if (!s_WaitUntil.TryGetValue(GetType(), out waitUntil))
					waitUntil = DateTime.MinValue;

				var waitTime = GetWaitTime();
				if (waitTime > TimeSpan.Zero)
				{
					s_WaitUntil[GetType()] = DateTime.UtcNow + waitTime;
				}
				else
				{
					s_WaitUntil[GetType()] = DateTime.MinValue;
				}
			}

			while (true)
			{
				var now = DateTime.UtcNow;
				if (now >= waitUntil)
				{
					return;
				}

				Thread.Sleep(Math.Max(1, (int)(waitUntil - now).TotalMilliseconds));
			}
		}

		protected virtual TimeSpan GetWaitTime()
		{
			return TimeSpan.Zero;
		}

		protected static TimeSpan GetRandomSeconds(double fewest, double most)
		{
			lock (s_Lock)
			{
				var seconds = fewest + s_Random.NextDouble() * (most - fewest);
				return TimeSpan.FromSeconds(seconds);
			}
		}

		private readonly string m_basePath;

		protected WebsiteDownloadBase(string basePath)
		{
			m_basePath = basePath ?? ".";
		}

		public abstract Uri WebsiteUrl { get; }
		protected abstract string SavedFilesPath { get; }

		private Uri previousUri;

		protected MemoryStream Download(Uri requestUrl, out Uri responseUrl)
		{
			string filePath;
			string fileNotFoundPath;
			if (SavedFilesPath != null)
			{
				filePath = Path.Combine(Path.Combine(m_basePath, SavedFilesPath),
									requestUrl.PathAndQuery.TrimStart('/').Replace('?', '£').Replace('/', '\\').Replace(':', '^'));
				fileNotFoundPath = filePath + ".NotFound";
			}
			else
			{
				filePath = null;
				fileNotFoundPath = null;
			}

			if (filePath != null)
			{
				if (File.Exists(fileNotFoundPath))
				{
					responseUrl = null;
					return null; ;
				}
				else if (File.Exists(filePath))
				{
					responseUrl = requestUrl;

					var memStream = new MemoryStream();
					byte[] buffer = new byte[4096];
					using (var stream = File.OpenRead(filePath))
					{
						while (true)
						{
							int count = stream.Read(buffer, 0, buffer.Length);
							if (count == 0)
								break;
							memStream.Write(buffer, 0, count);
						}
					}

					memStream.Position = 0;
					return memStream;
				}
			}

			var request = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
			request.UserAgent = "Mozilla/5.0 (MSIE 9.0; Windows NT 6.1; Trident/5.0)";
			//request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; rv:10.0.1) Gecko/20100101 Firefox/10.0.1";
			request.Referer = previousUri != null ? previousUri.ToString() : "";

			WaitAWhile();
			try
			{
				using (var response = request.GetResponse())
				{
					responseUrl = response.ResponseUri;
					if (response.ContentType.IndexOf("html", StringComparison.InvariantCultureIgnoreCase) >= 0)
						previousUri = responseUrl;

					if (filePath != null)
						Directory.CreateDirectory(Path.GetDirectoryName(filePath));

					var memStream = new MemoryStream();
					byte[] buffer = new byte[4096];
					using (var stream = response.GetResponseStream())
					using (var writeStream = filePath != null ? new FileStream(filePath, FileMode.Create, FileAccess.Write) : null)
					{
						while (true)
						{
							int count = stream.Read(buffer, 0, buffer.Length);
							if (count == 0)
								break;
							memStream.Write(buffer, 0, count);
							if (writeStream != null)
								writeStream.Write(buffer, 0, count);
						}
					}

					memStream.Position = 0;
					return memStream;

				}
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError &&
					ex.Response is HttpWebResponse &&
					((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
				{
					File.WriteAllBytes(fileNotFoundPath, ArrayUtils<byte>.EmptyArray);
					responseUrl = null;
					return null;
				}

				throw;
			}
		}
		

		public HtmlDocument DownloadHtml(Uri url, out Uri responseUrl)
		{
			return HtmlUtils.MemoryStreamToHtml(Download(url, out responseUrl));
		}

		protected Image DownloadGifImage(Uri url, out Uri responseUrl)
		{
			var memstr = Download(url, out responseUrl);

			var decoder = new GifComponents.GifDecoder(memstr);
			decoder.Decode();
			foreach (var frame in decoder.Frames)
			{
				return frame.TheImage;
			}

			return null;
		}
	}
}

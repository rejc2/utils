using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAgilityPack;

namespace Rejc2.Utils.Web
{
	public class WebsiteDownload : WebsiteDownloadBase
	{
		public override Uri WebsiteUrl
		{
			get { return default(Uri); }
		}

		protected override string SavedFilesPath
		{
			get { return null; }
		}

		public MemoryStream Download(Uri requestUrl)
		{
			Uri temp;
			return Download(requestUrl, out temp);
		}

		public HtmlDocument DownloadHtml(Uri requestUrl)
		{
			Uri temp;
			return DownloadHtml(requestUrl, out temp);
		}

		public WebsiteDownload(string basePath)
			: base(basePath)
		{
		}
	}
}

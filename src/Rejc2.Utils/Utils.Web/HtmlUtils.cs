using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.IO;

namespace Rejc2.Utils.Web
{
	public static class HtmlUtils
	{
		public static HtmlDocument MemoryStreamToHtml(MemoryStream memStream)
		{
			HtmlDocument doc = new HtmlDocument();
			var encoding = doc.DetectEncoding(memStream) ?? Encoding.Default;
			memStream.Position = 0;
			string str = encoding.GetString(memStream.ToArray());
			memStream.Position = 0;
			doc.Load(memStream, encoding);

			return doc;
		}

		public static void ProcessHtmlText(HtmlNode node, Func<string, string> process)
		{
			if (node is HtmlTextNode)
			{
				var textNode = (HtmlTextNode)node;
				string newText = process(textNode.Text);
				textNode.Text = newText;
			}
			else
			{
				foreach (var subNode in node.ChildNodes)
				{
					ProcessHtmlText(subNode, process);
				}
			}
		}
	}
}

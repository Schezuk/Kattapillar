using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace VShawnEpub.FileDown
{
	public class UtilTool
	{
		/// <summary>
		/// 获取文件大小
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static long GetUrlSize(string url)
		{
			WebRequest req = HttpWebRequest.Create(url);
			req.Timeout = 1000;
			req.Method = "HEAD";
			WebResponse resp = req.GetResponse();
			return long.Parse(resp.Headers.Get("Content-Length"));
		}

		/// <summary>
		/// 格式化空间大小 形如：[2.63M]
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static string FormatSize(long size)
		{
			string strReturn = "";
			double tempSize = Math.Abs(size);
			if (tempSize < 1024)
			{
				strReturn += tempSize.ToString() + "B";
			}
			else if (tempSize < 1024 * 1024)
			{
				tempSize = tempSize / 1024;
				strReturn += tempSize.ToString("0.##") + "K";
			}
			else if (tempSize < 1024 * 1024 * 1024)
			{
				tempSize = tempSize / 1024 / 1024;
				strReturn += tempSize.ToString("0.##") + "M";
			}
			else
			{
				tempSize = tempSize / 1024 / 1024 / 1024;
				strReturn += tempSize.ToString("0.##") + "G";
			}

			if (size < 0)
			{
				strReturn = "-" + strReturn;
			}

			return strReturn;
		}
	}
}

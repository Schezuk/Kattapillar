using System;
using System.Collections.Generic;
using System.Text;

namespace VShawnEpub.FileDown
{
	public class FileDownDao
	{
		/// <summary>
		/// 文件下载列表
		/// </summary>
		public static IList<FileDown> FileDownList = new List<FileDown>();

		#region 添加文件
		/// <summary>
		/// 添加文件
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static FileDown AddDownFile(string url)
		{
			url = url.Replace("\\", "/");
			int lastIndex = url.LastIndexOf("/");
			if (lastIndex == -1)
			{
				return null;
			}
            string fileName = url.Substring(lastIndex + 1);
            string filePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + fileName;
			return AddDownFile(url, filePath);
		}

		/// <summary>
		/// 添加文件
		/// </summary>
		/// <param name="url"></param>
		/// <param name="disk"></param>
		/// <param name="isCover"></param>
		/// <returns></returns>
		public static FileDown AddDownFile(string url, string disk)
		{
			url = url.Replace("\\", "/");
			disk = disk.Replace("/", "\\");

			foreach (FileDown r in FileDownList)
			{
				if (r.Url.Equals(url, StringComparison.CurrentCultureIgnoreCase))
				{
					return null;
				}
			}

			FileDown file = new FileDown();
			file.BeginTime = DateTime.MinValue;
			file.EndTime = DateTime.MinValue;
			file.ErrorMsg = "";
			file.FileName = url.Substring(url.LastIndexOf("/"));
			file.FileSize = 0;
			file.Id = Guid.NewGuid().ToString();
			file.SavePath = disk;
			file.Status = FileDown.FileDownStatus.Wait;
			file.Url = url;
			file.UseTime = new TimeSpan();

			FileDownList.Add(file);

			return file;
		}
		#endregion
	}
}

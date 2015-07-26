using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace VShawnEpub.FileDown
{
	public class FileDown
	{
		private string id;
		/// <summary>
		/// ID唯一标识
		/// </summary>
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		private FileDownStatus status;
		/// <summary>
		/// 状态
		/// </summary>
		public FileDownStatus Status
		{
			get { return status; }
			set { status = value; }
		}

		private string url;
		/// <summary>
		/// URL地址
		/// </summary>
		public string Url
		{
			get { return url; }
			set { url = value; }
		}

		private string savePath;
		/// <summary>
		/// 保存路径
		/// </summary>
		public string SavePath
		{
			get { return savePath; }
			set { savePath = value; }
		}

		private long fileSize;
		/// <summary>
		/// 文件大小
		/// </summary>
		public long FileSize
		{
			get { return fileSize; }
			set { fileSize = value; }
		}

		private DateTime beginTime;
		/// <summary>
		/// 开始时间
		/// </summary>
		public DateTime BeginTime
		{
			get { return beginTime; }
			set { beginTime = value; }
		}

		private DateTime endTime;
		/// <summary>
		/// 结束时间
		/// </summary>
		public DateTime EndTime
		{
			get { return endTime; }
			set { endTime = value; }
		}

		private TimeSpan useTime;
		/// <summary>
		/// 用时
		/// </summary>
		public TimeSpan UseTime
		{
			get { return useTime; }
			set { useTime = value; }
		}

		private string fileName;
		/// <summary>
		/// 文件名
		/// </summary>
		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		private string errorMsg;
		/// <summary>
		/// 错误消息
		/// </summary>
		public string ErrorMsg
		{
			get { return errorMsg; }
			set { errorMsg = value; }
		}

		private int errorCount = 0;
		/// <summary>
		/// 错误次数
		/// </summary>
		public int ErrorCount
		{
			get { return errorCount; }
			set { errorCount = value; }
		}

		/// <summary>
		/// 获取备注
		/// </summary>
		/// <returns></returns>
		public string GetMemo()
		{
			string str = "";
			if (!string.IsNullOrEmpty(errorMsg))
			{
				str += "[" + errorCount.ToString() + "] 错误：" + errorMsg;
			}

			return str;
		}
		
		/// <summary>
		/// 下载状态
		/// </summary>
		public enum FileDownStatus
		{ 
			/// <summary>
			/// 等待下载
			/// </summary>
			Wait,

			/// <summary>
			/// 下载中
			/// </summary>
			Doing,

			/// <summary>
			/// 下载失败
			/// </summary>
			Error,

			/// <summary>
			/// 下载完成
			/// </summary>
			Finish,
            /// <summary>
            /// 已存在
            /// </summary>
            Exixted
		}

		/// <summary>
		/// 获取下载状态名称
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		public static string GetFileDownStatusName(FileDownStatus status)
		{
			string strReturn = "";
			switch (status)
			{
				case FileDownStatus.Wait:
					strReturn = "等待下载";
					break;
				case FileDownStatus.Doing:
					strReturn = "下载中";
					break;
				case FileDownStatus.Error:
					strReturn = "下载失败";
					break;
                case FileDownStatus.Finish:
                    strReturn = "下载完成";
                    break;
                case FileDownStatus.Exixted:
                    strReturn = "文件已经存在";
                    break;
				default:
					break;
			}

			return strReturn;
		}
	}
}

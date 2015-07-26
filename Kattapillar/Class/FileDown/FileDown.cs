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
		/// IDΨһ��ʶ
		/// </summary>
		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		private FileDownStatus status;
		/// <summary>
		/// ״̬
		/// </summary>
		public FileDownStatus Status
		{
			get { return status; }
			set { status = value; }
		}

		private string url;
		/// <summary>
		/// URL��ַ
		/// </summary>
		public string Url
		{
			get { return url; }
			set { url = value; }
		}

		private string savePath;
		/// <summary>
		/// ����·��
		/// </summary>
		public string SavePath
		{
			get { return savePath; }
			set { savePath = value; }
		}

		private long fileSize;
		/// <summary>
		/// �ļ���С
		/// </summary>
		public long FileSize
		{
			get { return fileSize; }
			set { fileSize = value; }
		}

		private DateTime beginTime;
		/// <summary>
		/// ��ʼʱ��
		/// </summary>
		public DateTime BeginTime
		{
			get { return beginTime; }
			set { beginTime = value; }
		}

		private DateTime endTime;
		/// <summary>
		/// ����ʱ��
		/// </summary>
		public DateTime EndTime
		{
			get { return endTime; }
			set { endTime = value; }
		}

		private TimeSpan useTime;
		/// <summary>
		/// ��ʱ
		/// </summary>
		public TimeSpan UseTime
		{
			get { return useTime; }
			set { useTime = value; }
		}

		private string fileName;
		/// <summary>
		/// �ļ���
		/// </summary>
		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		private string errorMsg;
		/// <summary>
		/// ������Ϣ
		/// </summary>
		public string ErrorMsg
		{
			get { return errorMsg; }
			set { errorMsg = value; }
		}

		private int errorCount = 0;
		/// <summary>
		/// �������
		/// </summary>
		public int ErrorCount
		{
			get { return errorCount; }
			set { errorCount = value; }
		}

		/// <summary>
		/// ��ȡ��ע
		/// </summary>
		/// <returns></returns>
		public string GetMemo()
		{
			string str = "";
			if (!string.IsNullOrEmpty(errorMsg))
			{
				str += "[" + errorCount.ToString() + "] ����" + errorMsg;
			}

			return str;
		}
		
		/// <summary>
		/// ����״̬
		/// </summary>
		public enum FileDownStatus
		{ 
			/// <summary>
			/// �ȴ�����
			/// </summary>
			Wait,

			/// <summary>
			/// ������
			/// </summary>
			Doing,

			/// <summary>
			/// ����ʧ��
			/// </summary>
			Error,

			/// <summary>
			/// �������
			/// </summary>
			Finish,
            /// <summary>
            /// �Ѵ���
            /// </summary>
            Exixted
		}

		/// <summary>
		/// ��ȡ����״̬����
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		public static string GetFileDownStatusName(FileDownStatus status)
		{
			string strReturn = "";
			switch (status)
			{
				case FileDownStatus.Wait:
					strReturn = "�ȴ�����";
					break;
				case FileDownStatus.Doing:
					strReturn = "������";
					break;
				case FileDownStatus.Error:
					strReturn = "����ʧ��";
					break;
                case FileDownStatus.Finish:
                    strReturn = "�������";
                    break;
                case FileDownStatus.Exixted:
                    strReturn = "�ļ��Ѿ�����";
                    break;
				default:
					break;
			}

			return strReturn;
		}
	}
}

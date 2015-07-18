using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VShawnEpub.Model
{
    public class EpubModel
    {
        /// <summary>
        /// 书名
        /// </summary>
        public string Title;
        public string OriginalTitle;
        /// <summary>
        /// 卷标题
        /// </summary>
        public string SubTitle;
        public string Author;
        public string Illustration;
        public List<Capater> Capaters;
        public Image CoverImg;
        public EpubModel()
        {
            Capaters = new List<Capater>();
            CoverImg = new Image();
        }
        public void OutPutEpub(string floderPath)
        {
            if (!Directory.Exists(floderPath))
                Directory.CreateDirectory(floderPath);
            using (StreamWriter sw = new StreamWriter(floderPath + this.Title + ".txt", false, Encoding.UTF8))
            {
                for (int i = 0; i < Capaters.Count; i++)
                {
                    if (i != 0)
                        sw.Write("\r\n\r\n\r\n\r\n");
                    sw.Write(Capaters[i].Txt);
                }
                sw.Close();
            }
        }
        /// <summary>
        /// 输入Txt文件
        /// </summary>
        /// <param name="floderPath"></param>
        public void OutPutTxt(string floderPath)
        {
            if (!Directory.Exists(floderPath))
                Directory.CreateDirectory(floderPath);
            using (StreamWriter sw = new StreamWriter(floderPath + this.Title + ".txt", false, Encoding.UTF8))
            {
                for (int i = 0; i < Capaters.Count; i++)
                {
                    if (i != 0)
                        sw.Write("\r\n\r\n\r\n\r\n");
                    sw.Write(Capaters[i].Txt);
                }
                sw.Close();
            }
        }
    }
}

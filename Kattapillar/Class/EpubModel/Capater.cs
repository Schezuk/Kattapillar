using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VShawnEpub.Model
{
    public class Capater
    {
        /// <summary>
        /// 章节索引，从0开始
        /// </summary>
        public int Index;
        /// <summary>
        /// 章节标题
        /// </summary>
        public string Title;
        public string Url;
        public string Html;
        private string txt;
        public string Txt
        {
            set
            {
                txtLines = new List<string>(value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                if (Index != 0)
                {
                    List<string> tmp = new List<string>(txtLines);
                    for (int i = 0; i < tmp.Count; i++)
                    {
                        tmp[i] = "    " + tmp[i].Trim();
                    }
                    txt = string.Join("\r\n", tmp.ToArray());
                }
                else
                    txt = value;
            }
            get
            {
                return txt;
            }
        }
        private List<string> txtLines;
        /// <summary>
        /// 分行存储Txt
        /// </summary>
        public List<string> TxtLines
        {
            get { return txtLines; }
        }
    }
}

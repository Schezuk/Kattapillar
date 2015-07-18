using System;
using System.Collections.Generic;
using System.Text;

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
                txt = value;
                txtLines = new List<string>(txt.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
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

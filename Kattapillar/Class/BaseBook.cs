using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;

namespace VShawnEpub
{
    public abstract class BaseBook
    {
        public string Title;

        private string author;
        public string Author
        {
            get { return author; }
            set { if (author == "") author = value; }
        }
        public List<string> Htmls;
        public List<string> Txts;
        public string MainURL;
        public string Host;
        public string OutPutDir;
        public BaseBook(string title, string mainURL,string outPutDir)
        {
            Txts = new List<string>();
            Htmls = new List<string>();
            Host = "";
            Title = title;
            MainURL = mainURL;
            OutPutDir = outPutDir;
        }
        public abstract void Add(string url);
        /// <summary>
        /// 所有书本完成事件
        /// </summary>
        public abstract event EventHandler EvenAllCompleted;

        /// <summary>
        /// 根据网址获取网页html
        /// </summary>
        public void OutPutTxt(string floderPath)
        {
            using (StreamWriter sw = new StreamWriter(floderPath + this.Title + ".txt", false, Encoding.UTF8))
            {
                for (int i = 0; i < Txts.Count; i++)
                {
                    if(i != 0)
                        sw.Write("\r\n\r\n\r\n\r\n");
                    sw.Write(Txts[i]);
                }
                sw.Close();
            }
        }  
        protected string getRegEx(string str, string pattern,string get = "", RegexOptions o = RegexOptions.IgnoreCase)
        {
            Match m = Regex.Match(str, pattern, o);
            ThrowMissMatch(m);
            if (get != "")
            {
                return m.Result(get);
            }
            return m.ToString();
        }
        protected MatchCollection getRegExs(string str, string pattern, RegexOptions o = RegexOptions.IgnoreCase)
        {
            MatchCollection rs = Regex.Matches(str, pattern, o);
            ThrowMissMatch(rs);
            return rs;
        }
        private static void ThrowMissMatch(Match m)
        {
            if (!m.Success)
            {
                MessageBox.Show("Miss Mach! 可能是由于网页未加载完毕或网站源代码已经改变所导致的，请重试几次或联系程序开发者修改程序。", "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
        }
        private static void ThrowMissMatch(MatchCollection m)
        {
            if (m.Count == 0)
            {
                MessageBox.Show("Miss Mach! 可能是由于网页未加载完毕或网站源代码已经改变所导致的，请重试几次或联系程序开发者修改程序。", "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception();
            }
        }
        /// <summary>
        /// 删除Html 将img标签替换为一行url
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        protected string NoHTML(string Htmlstring)
        {
            //图片统一格式到Txt
            Htmlstring = Regex.Replace(Htmlstring, @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", "\r\n[IMG]$1\r\n", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"<br\s*/>", "\r", RegexOptions.IgnoreCase);
            //删除脚本
            //Htmlstring = Regex.Replace(Htmlstring, @"<.*>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            //Htmlstring.Replace("<", "");
            //Htmlstring.Replace(">", "");
            //Htmlstring = Regex.Replace(Htmlstring, @"([\r])[\s]+", "", RegexOptions.IgnoreCase);
            //Htmlstring.Replace("\r", "");

            return Htmlstring;
        }
    }
}

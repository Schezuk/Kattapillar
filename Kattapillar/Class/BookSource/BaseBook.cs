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
        /// <summary>
        /// 小说名称
        /// </summary>
        public string Title;
        /// <summary>
        /// 小说作者
        /// </summary>
        public string Author;
        /// <summary>
        /// 原始Html
        /// </summary>
        public List<string> Htmls;
        /// <summary>
        /// 处理后的Txt
        /// </summary>
        public List<string> Txts;
        /// <summary>
        /// 书本主页
        /// </summary>
        public string MainURL;
        public string Host;
        /// <summary>
        /// 最终输出路径
        /// </summary>
        public string OutPutDir;

        public BookStatus Status = BookStatus.NotInit;
        //浏览器
        /// <summary>
        /// 浏览器序列，用于预加载网页
        /// </summary>
        public List<WebBrowser> Browsers;
        /// <summary>
        /// 浏览器创建序列指针
        /// </summary>
        public int BroswersLodedIndex = 0;
        /// <summary>
        /// 浏览器使用序列指针
        /// </summary>
        public int BroswersUsingIndex = 0;
        public BaseBook(string title, string mainURL,string outPutDir)
        {
            Txts = new List<string>();
            Htmls = new List<string>();
            Browsers = new List<WebBrowser>();
            BroswersLodedIndex = 0;
            BroswersUsingIndex = 0;

            Initialize(title, mainURL, outPutDir);
        }
        public BaseBook()
        {
            Txts = new List<string>();
            Htmls = new List<string>();
            Browsers = new List<WebBrowser>();
            BroswersLodedIndex = 0;
        }

        /// <summary>
        /// 手动初始化类
        /// </summary>
        /// <param name="title"></param>
        /// <param name="mainURL"></param>
        /// <param name="outPutDir"></param>
        public void Initialize(string title, string mainURL, string outPutDir)
        {
            Txts = new List<string>();
            Htmls = new List<string>();
            BroswersUsingIndex = 0;
            Host = "";
            Title = title;
            MainURL = mainURL;
            OutPutDir = outPutDir;
            Status = BookStatus.Inited;
        }
        /// <summary>
        /// 根据网址获取网页html
        /// </summary>
        public abstract void Add(string url);
        /// <summary>
        /// 所有书本完成事件，其响应为输出Txt
        /// </summary>
        public abstract event EventHandler EvenAllCompleted;
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








        /// <summary>
        /// 获得一个正则表达式的匹配
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="get">$1 $2...表示获得第几个()匹配</param>
        /// <param name="o">匹配模式</param>
        /// <returns></returns>
        protected static string getRegEx(string str, string pattern,string get = "", RegexOptions o = RegexOptions.IgnoreCase)
        {
            Match m = Regex.Match(str, pattern, o);
            ThrowMissMatch(m);
            if (get != "")
            {
                return m.Result(get);
            }
            return m.ToString();
        }
        /// <summary>
        /// 获得多个正则表达式的匹配
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        protected static MatchCollection getRegExs(string str, string pattern, RegexOptions o = RegexOptions.IgnoreCase)
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

        public enum BookStatus
        {
            NotInit,
            Inited,
            WaitingBroswer,
            Running,
            Completed
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace VShawnEpub.Wenku
{
    public class LKWenKuBook:WenKuBook
    {
        public override event EventHandler EvenAllCompleted;
        public LKWenKuBook(string title, string mainURL, string outPutDir) : base(title, mainURL, outPutDir)
        {
            ChaptersURLs = new List<string>();
        }
        public override void Add(string html)
        {
            
        }
    }
}

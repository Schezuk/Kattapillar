using System;
using System.Collections.Generic;
using System.Text;

namespace VShawnEpub
{
    public abstract class WenKuBook:BaseBook
    {
        public List<string> ChaptersURLs;
        public WenKuBook(string title, string mainURL, string outPutDir) : base(title, mainURL, outPutDir)
        {
            ChaptersURLs = new List<string>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSHawnEpub.Model.LKDiscuz
{
    public class LKDiscuzModel
    {
        private string booktitle;
        private List<Floor> everyFloors;


        public string Master;
        public string Txt = "";
        public List<string> htmls;
        public LKDiscuzModel(string title)
        {
            booktitle = title;
            everyFloors = new List<Floor>();
            htmls = new List<string>();
        }
        public string Title
        {
            get { return booktitle; }
            set { booktitle = value; }
        }
        public List<Floor> EveryFloors
        {
            get { return EveryFloors;}
        }
        public void AddFloor(Floor f)
        {
            everyFloors.Add(f);
            Txt += f.Txt;
        }
    }
    public class Floor
    {
        public Floor(string _author, string _html,string _txt, int _floorIndex = 0, int _pageIndex = 1)
        {
            author = _author;
            html = _html;
            txt = _txt;
            floorIndex = _floorIndex;
            pageIndex = _pageIndex;
        }
        private string author;
        public string Author
        {
            get { return author; }
            set { author = value; }
        }
        private int floorIndex;
        public int FloorIndex
        {
            get { return floorIndex; }
            set { floorIndex = value; }
        }
        private int pageIndex;
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }
        private string txt;
        public string Txt
        {
            get { return txt; }
            set { txt = value; }
        }
        private string html;
        public string Html
        {
            get { return html; }
            set 
            { 
                html = value;
            }
        }
    }
}

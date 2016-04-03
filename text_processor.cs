using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsefulExtensions
{
    public class TextProcessor
    {
        private List<string> text = new List<string>();
        public Dictionary<int,string> Bookmarks = new Dictionary<int, string>();
        public string Header { get; set; }
        public string Footer { get; set; }
        public string Offset { get; set; }
        public bool LineNumbers { get; set; }

        public TextProcessor(string fileName = "")
        {
            Clear();
            if (fileName != string.Empty)
            {
                Read(fileName);
            }
        }


        public void Clear()
        {
            text.Clear();
            Bookmarks.Clear();
            Header = string.Empty;
            Footer = string.Empty;
            Offset = string.Empty;
            LineNumbers = false;
        }


        public int Read(string fileName)
        {
            Clear();
            text = File.ReadAllLines(fileName).ToList();
            return text.Count;
        }

        public int Save(string fileName, bool append = true)
        {
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                foreach (string line in text)
                {
                    writer.WriteLine(line);
                }
            }
            return 0;
        }

        public int SaveFormatted(string fileName, bool append = true)
        {
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                if (Header != string.Empty)
                {
                    writer.WriteLine(Header);
                }
                foreach (string line in text)
                {
                    writer.WriteLine(line);
                }
                if (Footer != string.Empty)
                {
                    writer.WriteLine(Footer);
                }
            }
            return 0;
        }

        public int Count
        {
            get
            {
                return text.Count;
            }
        }


    }
}

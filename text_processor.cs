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
            if (File.Exists(fileName))
            {
                text = File.ReadAllLines(fileName).ToList();
            }
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

        public int SearchForward(string pattern, int startIdx=0)
        {
            int idx = startIdx;
            while (idx < text.Count)
            {
                string cur = text[idx];
                if (Regex.IsMatch(cur, pattern))
                {
                    return idx;
                }
                idx++;
            }
            return -1;
        }

        public string this[int idx]
        {
            get
            {
                return text[idx];
            }
            set
            {
                text[idx] = value;
            }
        }


        public string[] FindAndParse(string pattern, string delimiter)
        {
            int lineIdx = SearchForward(pattern);
            if (lineIdx > 0)
            {
                string di = text[lineIdx];
                string[] elements = Regex.Split(di, delimiter);
                return elements;
            } else
            {
                return null;
            }

        }


        public void Add(string newLine)
        {
            text.Add(newLine);
        }



        public int SearchBackward(string pattern, int startIdx = 0)
        {
            int idx = text.Count-1;
            if (startIdx > 0)
            {
                idx = startIdx;
            }
            while (idx >= 0)
            {
                string cur = text[idx];
                if (Regex.IsMatch(cur, pattern))
                {
                    return idx;
                }
                idx--;
            }
            return -1;
        }

        public List<int> FindAll(string pattern, int startIdx = -1, int stopIdx = -1)
        {
            if (startIdx<0)
            {
                startIdx = 0;
            }
            if (stopIdx<0)
            {
                stopIdx = text.Count;
            }
            List<int> retVal = new List<int>();
            int idx = startIdx;
            while (idx > startIdx)
            {
                string cur = text[idx];
                if (Regex.IsMatch(cur, pattern))
                {
                    retVal.Add(idx);
                }
                idx++;
            }
            return retVal;
        }

        public int ReplaceAll(string searchPattern, string replacePattern, int startIdx = -1, int stopIdx = -1)
        {
            if (startIdx < 0)
            {
                startIdx = 0;
            }
            if (stopIdx < 0)
            {
                stopIdx = text.Count;
            }
            int replacementCount = 0;
            if (text.Count > 0)
            {
                for (int idx = startIdx; idx < stopIdx; idx++)
                {
                    string cur = text[idx];
                    if (Regex.IsMatch(cur, searchPattern))
                    {
                        text[idx] = Regex.Replace(text[idx], searchPattern, replacePattern);
                        replacementCount++;
                    }
                }
            }
            return replacementCount;
        }

        public TextProcessor Clone(string searchPattern = "", int startIdx = -1, int stopIdx = -1)
        {
            if (startIdx < 0)
            {
                startIdx = 0;
            }
            if (stopIdx < 0)
            {
                stopIdx = text.Count;
            }
            TextProcessor newTP = new TextProcessor();
            if (text.Count > 0)
            {
                for (int idx = startIdx; idx< stopIdx; idx++)
                {
                    if (searchPattern == "")
                    {
                        newTP.Add(text[idx]);
                    } else if (Regex.IsMatch(text[idx], searchPattern))
                    {
                        newTP.Add(text[idx]);
                    }
                }
            }
            return newTP;
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string row in text)
            {
                sb.AppendLine(row);
            }
            return sb.ToString();
        }

        public string ToSingleString(string lineSeparator)
        {
            StringBuilder sb = new StringBuilder();
            for (int idx = 0; idx < text.Count; idx++)
            {
                if (idx == (text.Count - 1))
                {
                    sb.Append(text[idx]); // Last line, no separator
                }
                else
                {
                    sb.Append(text[idx] + lineSeparator);
                }
                
            }
            return sb.ToString();
        }


    }
}

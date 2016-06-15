/*-----------------------------------------------------------------------------------------------------------

             _   _ ____  _____ _____ _   _ _       _______  _______ _____ _   _ ____ ___ ___  _   _ ____
            | | | / ___|| ____|  ___| | | | |     | ____\ \/ /_   _| ____| \ | / ___|_ _/ _ \| \ | / ___|
            | | | \___ \|  _| | |_  | | | | |     |  _|  \  /  | | |  _| |  \| \___ \| | | | |  \| \___ \
            | |_| |___) | |___|  _| | |_| | |___  | |___ /  \  | | | |___| |\  |___) | | |_| | |\  |___) |
             \___/|____/|_____|_|    \___/|_____| |_____/_/\_\ |_| |_____|_| \_|____/___\___/|_| \_|____/

    Useful extensions is a public project to extend functionality of standard C# classes with most frequently
    used functions. 

    Project home
    https://github.com/DmitryObukhov/UsefulExtensions

-----------------------------------------------------------------------------------------------------------*/
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace UsefulExtensions
{

    public static class Strings
    {
        public const string whitespace = " \t\n\r\v\f";
        public const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        public const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string letters = lowercase + uppercase;
        public const string ascii_lowercase = lowercase;
        public const string ascii_uppercase = uppercase;
        public const string ascii_letters = ascii_lowercase + ascii_uppercase;
        public const string digits = "0123456789";
        public const string hexdigits = digits + "abcdef" + "ABCDEF";
        public const string octdigits = "01234567";
        public const string punctuation = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
        public const string printable = digits + letters + punctuation + whitespace;
    }

    public static class StringGeneralExtensions
    {

        /// <summary>
        ///  This function prepares string to be used in SQL request string:
        /// <list type="bullet">
        /// <item><description>escape special characters</description></item>
        /// <item><description>add ' to be used in SQL request</description></item>
        /// </list>
        /// </summary>
        /// <param name="strVariable">string variable</param>
        /// <example>table.Select("Column = "+"xyz".SQL());</example>
        public static string SQL(this string strVariable)
        {
            StringBuilder sb = new StringBuilder(strVariable.Length);
            for (int i = 0; i < strVariable.Length; i++)
            {
                char c = strVariable[i];
                switch (c)
                {
                    case ']':
                    case '[':
                    case '%':
                    case '*':
                        sb.Append("[" + c + "]");
                        break;
                    case '\'':
                        sb.Append("''");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return "'" + sb.ToString() + "'";
        }


        /// <summary>
        ///  This function prepares string to be used in SQL request string:
        /// <list type="bullet">
        /// <item><description>escape special characters</description></item>
        /// <item><description>add ' to be used in SQL request</description></item>
        /// </list>
        /// </summary>
        /// <param name="strVariable">string variable</param>
        /// <example>table.Select("Column = "+"xyz".SQL());</example>
        public static string QT(this string strVariable)
        {
            return "\"" + strVariable + "\"";
        }

        public static string RemoveLineEndings(this string value, string replacement = " ")
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", replacement).Replace("\n", replacement).Replace("\r", replacement).Replace(lineSeparator, replacement).Replace(paragraphSeparator, replacement);
        }


        public static string NoSpace(this string strVariable)
        {
            return Regex.Replace(strVariable, @"\W+", "_");
        }

        public static string FixLen(this string strVariable, int strLen)
        {
            string retVal = strVariable.ToString();
            if (retVal.Length > strLen)
            {
                retVal = retVal.Substring(0, strLen);
            }
            else if (retVal.Length < strLen)
            {
                retVal = retVal + new String(' ', (strLen - retVal.Length));
            }
            return retVal;
        }

        public static string SS(this DateTime dateVal)
        {
            return "#" + dateVal.ToShortDateString() + "#";
        }


        public static string MD5str(this string strVariable)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(strVariable));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string Random(this string strVariable, int length, string chars, int seed)
        {
            Random random;
            if (seed != 0) {
                random = new Random(seed);
            } else
            {
                random = new Random();
            }

            string retVal = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return retVal;
        }

        public static string Random(this string strVariable, int length)
        {
            return strVariable.Random(length, strVariable, 0);
        }

        public static string TimeStamp(this string strVariable, string separator="")
        {
            DateTime now = DateTime.Now;
            string timestamp = "";
            timestamp = timestamp + now.Year.ToString("d04")+ separator;
            timestamp = timestamp + now.Month.ToString("d02") + separator;
            timestamp = timestamp + now.Day.ToString("d02") + separator;
            timestamp = timestamp + now.Hour.ToString("d02") + separator;
            timestamp = timestamp + now.Minute.ToString("d02") + separator;
            timestamp = timestamp + now.Second.ToString("d02") + separator;
            timestamp = timestamp + now.Millisecond.ToString("d04");
            return timestamp;
        }


    }


    public class StringCounter
    {
        private Dictionary<string, int> values;
        public StringCounter()
        {
            values = new Dictionary<string, int>();
        }
        public StringCounter(string[] mandatoryValues)
        {
            values = new Dictionary<string, int>();
            foreach (string x in mandatoryValues)
            {
                values.Add(x, 0);
            }
        }
        public int AddCount(string x, int n)
        {
            if (values.Keys.Contains(x))
            {
                values[x] += n;
            }
            else
            {
                values.Add(x, n);
            }
            return values[x];
        }

        public int AddCount(string x)
        {
            return AddCount(x, 1);
        }

        public int GetCount(string x)
        {
            if (!values.Keys.Contains(x))
            {
                return 0;
            }
            return values[x];
        }

        public Dictionary<string, int> GetAllCounters(string x)
        {
            return values;
        }

    }

}

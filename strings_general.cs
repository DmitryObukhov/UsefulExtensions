using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace UsefulExtensions
{
    public static class StringGeneralExtensions
    {

        // Prepare string to be used in SQL request string
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



    }

}

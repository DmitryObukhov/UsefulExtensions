using System;
using System.Data;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace UsefulExtensions
{
    public static class TableStatisticalExtensions
    {

        public static Dictionary<string, int> CountColumnValues(this System.Data.DataTable source, string column)
        {
            Dictionary<string, int> retVal = new Dictionary<string, int>();
            foreach (DataRow row in source.Rows)
            {
                string key = row[column].ToString();
                if (retVal.Keys.Contains(key))
                {
                    retVal[key]++;
                }
                else
                {
                    retVal.Add(key, 1);
                }
            }
            return retVal;
        }


        public static Dictionary<string, int> CountColumnValues(this System.Data.DataTable source, string column, params string[] expectedValues)
        {
            Dictionary<string, int> retVal = CountColumnValues(source, column);
            foreach (string templateStr in expectedValues)
            {
                if (!retVal.Keys.Contains(templateStr))
                {
                    retVal.Add(templateStr, 0);
                }
            }
            return retVal;
        }

        public static int CountRows(this System.Data.DataTable source, string selector)
        {
            DataRow[] selection = source.Select(selector);
            return selection.Count();
        }


    }

}

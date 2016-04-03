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
    public static class TableGeneralExtensions
    {

        public static string ToCSV(this System.Data.DataTable table)
        {
            var result = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append(table.Columns[i].ColumnName);
                result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
            }

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(row[i].ToString());
                    result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
                }
            }
            return result.ToString();
        }

        public static string WriteCSV(this System.Data.DataTable table, string fileName)
        {
            string result = table.ToCSV();
            System.IO.File.WriteAllText(fileName, result.ToString());
            return result;
        }




        public static System.Data.DataTable CloneIntToDouble(this System.Data.DataTable source)
        {
            System.Data.DataTable retVal = new System.Data.DataTable();
            foreach (DataColumn dc in source.Columns)
            {
                if (dc.DataType == typeof(int))
                {
                    retVal.Columns.Add(dc.ColumnName, typeof(double));
                }
                else
                {
                    retVal.Columns.Add(dc.ColumnName, dc.DataType);
                }
            }
            foreach (DataRow dr in source.Rows)
            {
                retVal.ImportRow(dr);
            }
            return retVal;
        }

        public static System.Data.DataTable CloneAllToString(this System.Data.DataTable source)
        {
            System.Data.DataTable retVal = new System.Data.DataTable();
            foreach (DataColumn dc in source.Columns)
            {
                retVal.Columns.Add(dc.ColumnName, typeof(string));
            }
            foreach (DataRow dr in source.Rows)
            {
                retVal.ImportRow(dr);
            }
            return retVal;
        }

    }

}

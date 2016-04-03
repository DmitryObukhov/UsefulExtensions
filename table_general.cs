using System;
using System.Data;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;


namespace UsefulExtensions
{
    public static class TableGeneralExtensions
    {

        public static void FromClass<T>(this DataTable source, T classInstance)
        {
            source.Clear();
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                source.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
        }

        public static void FromObject<T>(this System.Data.DataTable source, T objectInstance)
        {
            source.Clear();
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                source.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            DataRow row = source.NewRow();
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                row[info.Name] = info.GetValue(objectInstance, null);
            }
            source.Rows.Add(row);
        }


        public static void FromList<T>(this DataTable source, List<T> list)
        {
            source.Clear();
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                source.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (T t in list)
            {
                DataRow row = source.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }
                source.Rows.Add(row);
            }
        }






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

        public static bool IsNumericColumn(this System.Data.DataTable table, string colname)
        {
            if (!table.Columns.Contains(colname))
            {
                return false;
            }
            DataColumn col = table.Columns[colname];
            if (col == null)
                return false;
            // Make this const
            var numericTypes = new[] {  typeof(Byte), typeof(Decimal), typeof(Double),
                                        typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
                                        typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64)};
            return numericTypes.Contains(col.DataType);
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

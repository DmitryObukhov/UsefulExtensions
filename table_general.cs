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
    public static class TypeInfo
    {
        public static Type[] NumericTypes = new Type[] {
                            typeof(Byte),
                            typeof(Decimal),
                            typeof(Double),
                            typeof(Int16),
                            typeof(Int32),
                            typeof(Int64),
                            typeof(SByte),
                            typeof(Single),
                            typeof(UInt16),
                            typeof(UInt32),
                            typeof(UInt64)};

        public static Type[] IntegerTypes = new Type[] {
                            typeof(Byte),
                            typeof(Int16),
                            typeof(Int32),
                            typeof(Int64),
                            typeof(SByte),
                            typeof(UInt16),
                            typeof(UInt32),
                            typeof(UInt64)};

        public static Type[] SignedTypes = new Type[] {
                            typeof(Int16),
                            typeof(Int32),
                            typeof(Int64),
                            typeof(SByte)};

    }

    public static class DateTimeGeneralExtensions
    {
        public static DateTime GetInvalid(this DateTime dt)
        {
            return new DateTime(1111, 1, 1);
        }
        public static void SetInvalid(this DateTime dt)
        {
            dt = dt.GetInvalid();
        }

        public static bool IsInvalid(this DateTime dt)
        {
            if (dt.Year==1111 && dt.Month==1 && dt.Day == 1)
            {
                return true;
            }
            return false;
        }

    }


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

        public static void FromObject<T>(this DataTable source, T objectInstance)
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






        public static string ToCSV(this System.Data.DataTable table, string delimiter="\t")
        {
            var result = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append("\"" + table.Columns[i].ColumnName + "\"");
                result.Append(i == table.Columns.Count - 1 ? "\n" : delimiter);
            }

            for (int rIdx=0; rIdx < table.Rows.Count; rIdx++)
            {
                for (int cIdx = 0; cIdx < table.Columns.Count; cIdx++)
                {
                    if (table.Columns[cIdx].DataType == typeof(string))
                    {
                        result.Append("\""+ table.Rows[rIdx][cIdx].ToString()+"\"");
                    }
                    //else if (table.Columns[cIdx].DataType == typeof(DateTime))
                    //{
                    //    string res = "";
                    //    try {
                    //        DateTime dt = Convert.ToDateTime(table.Rows[rIdx][cIdx]);
                    //        res = "\"" + dt.ToLongDateString() + "\"";
                    //    }
                    //    catch
                    //    {
                    //        res = "";
                    //    }
                    //    result.Append(res);
                    //}
                    else
                    {
                        result.Append(table.Rows[rIdx][cIdx].ToString());
                    }
                    if (rIdx< (table.Rows.Count - 1))
                    {
                        result.Append(cIdx == table.Columns.Count - 1 ? "\n" : delimiter);
                    }
                    else
                    {
                        result.Append(cIdx == table.Columns.Count - 1 ? "" : delimiter);
                    }
                }
            }
            return result.ToString();
        }

        public static string WriteCSV(this DataTable table, string fileName)
        {
            string result = table.ToCSV(",");
            System.IO.File.WriteAllText(fileName, result.ToString());
            return result;
        }

        public static bool IsNumericColumn(this DataTable table, string colname)
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

        public static bool IsNumeric(this DataColumn col)
        {
            if (col == null)
                return false;
            // Make this const
            var numericTypes = new[] {  typeof(Byte), typeof(Decimal), typeof(Double),
                                        typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
                                        typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64)};
            return numericTypes.Contains(col.DataType);
        }


        public static DataTable CloneIntToDouble(this DataTable source)
        {
            DataTable retVal = new DataTable();
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

        public static System.Data.DataTable CloneAllToString(this DataTable source)
        {
            DataTable retVal = new DataTable();
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



        public static DateTime GetEarliest(this System.Data.DataTable table, string dateTimeColumnName, int idx)
        {
            List<string> sStrings = table.AsEnumerable().Select(x => x[dateTimeColumnName].ToString()).ToList();
            sStrings.SortAndClean();
            List<DateTime> s = sStrings.ConvertAll(x => Convert.ToDateTime(x));
            s.Sort();
            if (s.Count > idx)
            {
                return s[idx];
            }
            else if (s.Count > 0)
            {
                return s[0];

            }
            return DateTime.MaxValue;
        }

        public static DateTime GetEarliest(this System.Data.DataTable table, string dateTimeColumnName)
        {
            return GetEarliest(table, dateTimeColumnName, 0);
        }


        public static DateTime GetLatest(this System.Data.DataTable table, string dateTimeColumnName, int idx)
        {
            List<string> sStrings = table.AsEnumerable().Select(x => x[dateTimeColumnName].ToString()).ToList();
            sStrings.SortAndClean();
            List<DateTime> s = sStrings.ConvertAll(x => Convert.ToDateTime(x));
            s.Sort();
            if (s.Count > idx)
            {
                return s[s.Count - 1 - idx];
            }
            else if (s.Count > 0)
            {
                return s[s.Count - 1];

            }
            return DateTime.MaxValue;
        }


        public static DateTime GetLatest(this System.Data.DataTable table, string dateTimeColumnName)
        {
            return GetLatest(table, dateTimeColumnName, 0);
        }


        public static System.Data.DataTable CloneSelectedRows(this System.Data.DataTable source, string selector, string sort)
        {
            DataRow[] selection = source.Select(selector, sort);
            System.Data.DataTable retVal = selection.CopyToDataTable();
            return retVal;
        }

        public static System.Data.DataTable CloneSelectedRows(this System.Data.DataTable source, string selector)
        {
            DataRow[] selection;
            try
            {
                selection = source.Select(selector);
            }
            catch
            {
                return null;
            }
            if (selection.Count() > 0)
            {
                System.Data.DataTable retVal = selection.CopyToDataTable();
                return retVal;
            } else
            {
                return null;
            }
        }

        public static int CountRows(this System.Data.DataTable source, string selector)
        {
            DataRow[] selection = source.Select(selector);
            return selection.Count();
        }

        public static Dictionary<string, int> CountValues(this System.Data.DataTable source, string column)
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


        public static Dictionary<string, int> CountValues(this System.Data.DataTable source, string column, params string[] expectedValues)
        {
            Dictionary<string, int> retVal = CountValues(source, column);
            foreach (string templateStr in expectedValues)
            {
                if (!retVal.Keys.Contains(templateStr))
                {
                    retVal.Add(templateStr, 0);
                }
            }
            return retVal;
        }


        public static List<string> ListOfDistinctValues(this System.Data.DataTable table, string columnName)
        {
            List<string> returnList;
            if (true)
            {
                DataView filter = new DataView(table);
                System.Data.DataTable ListOfValues = filter.ToTable(true, columnName);
                returnList = ListOfValues.AsEnumerable().Select(x => x[0].ToString()).ToList();
            }
            else
            {
                List<object> rawList = new List<object>();
                returnList = new List<string>();
                for (int rIdx = 0; rIdx < table.Rows.Count; rIdx++)
                {
                    if (!rawList.Contains(table.Rows[rIdx][columnName]))
                    {
                        rawList.Add(table.Rows[rIdx][columnName]);
                        returnList.Add(table.Rows[rIdx][columnName].ToString());
                    }
                }
            }
            return returnList;
        }


        public static int FindFirst(this System.Data.DataTable table, int columnIdx, object cellValue)
        {
            for (int idx = 0; idx < table.Rows.Count; idx++)
            {
                if (table.Rows[idx][columnIdx].Equals(cellValue))
                {
                    return idx;
                }
            }
            return -1;
        }

        public static int FindFirst(this System.Data.DataTable table, string columnIdx, object cellValue)
        {
            for (int idx = 0; idx < table.Rows.Count; idx++)
            {
                if (table.Rows[idx][columnIdx].Equals(cellValue))
                {
                    return idx;
                }
            }
            return -1;
        }


    }



}

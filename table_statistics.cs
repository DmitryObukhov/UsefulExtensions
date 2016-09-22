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
            if (source != null)
            {
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


        public static void CollapseRows(this DataTable source, int destIndex, int[] srcIndexes)
        {
            //Debug.Assert(srcIndexes.Length > 1, "Emty Index");
            //Debug.Assert(!(srcIndexes.Contains(destIndex)), "Repeated Index");
            foreach (DataColumn dc in source.Columns)
            {
                if (dc.IsNumeric())
                {
                    Double sum = 0.0;
                    foreach (int rIdx in srcIndexes)
                    {
                        sum += Convert.ToDouble(source.Rows[rIdx][dc.Ordinal]);
                    }
                    source.Rows[destIndex][dc.Ordinal] = Convert.ChangeType(sum, dc.DataType);
                }
                else
                {
                    // Take value from the last row in the range
                    source.Rows[destIndex][dc.Ordinal] = source.Rows[srcIndexes[srcIndexes.Count() - 1]][dc.Ordinal];
                }
            }
            source.AcceptChanges();
            for (int idx = 0; idx < srcIndexes.Count(); idx++)
            {
                source.Rows[srcIndexes[idx]].Delete();
            }
            source.AcceptChanges();
        }



        public static void CollapseRange(this DataTable source, int destIndex, int rangeStart, int rangeEnd)
        {
            int[] srcIndexes = new int[rangeEnd - rangeStart + 1];
            srcIndexes.FillIncrement(rangeStart);
            CollapseRows(source, destIndex, srcIndexes);
        }

        public static DataTable ParetoByValue(this DataTable source, string sqlSelector, string namesColumn, string valuesColumn = "")
        {
            DataTable rawData = source.CloneSelectedRows(sqlSelector);
            Dictionary<string, int> countedValues = rawData.CountValues(namesColumn);

            DataTable paretoTable = new DataTable();

            paretoTable.Columns.Add("Name", typeof(string));
            paretoTable.Columns.Add("Cumulative", typeof(double));
            paretoTable.Columns.Add("Absolute", typeof(double));
            paretoTable.Columns.Add("Relative", typeof(double));

            List<string> names = rawData.ListOfDistinctValues(namesColumn);

            foreach (string name in names)
            {
                DataRow dr = paretoTable.NewRow();
                dr["Name"] = name;

                dr["Cumulative"] = Double.NaN; // Uninitiated value
                dr["Relative"] = Double.NaN;

                DataTable recordsOfTheName = rawData.CloneSelectedRows(namesColumn + " = " + name.SQL());
                object sumObject;
                sumObject = recordsOfTheName.Compute("Sum("+ valuesColumn + ")", "");
                dr["Absolute"] = Convert.ToDouble(sumObject);

                paretoTable.Rows.Add(dr);
            }

            double total = Convert.ToDouble(paretoTable.Compute("Sum(Absolute)", ""));
            paretoTable = paretoTable.CloneSelectedRows("", "Absolute desc");

            double cumulative = 0.0;
            for (int idx = 0; idx< paretoTable.Rows.Count; idx++)
            {
                double curAbs = Convert.ToDouble(paretoTable.Rows[idx]["Absolute"]);
                double curRel = (curAbs / total) * 100.0;
                cumulative += curRel;
                paretoTable.Rows[idx]["Relative"] = curRel;
                paretoTable.Rows[idx]["Cumulative"] = cumulative;
            }
            paretoTable.AcceptChanges();
            return paretoTable;
        }

        public static DataTable ParetoByCount(this DataTable source, string sqlSelector, string namesColumn)
        {
            System.Data.DataTable rawData = source.CloneSelectedRows(sqlSelector);
            Dictionary<string, int> countedValues = rawData.CountValues(namesColumn);

            System.Data.DataTable paretoTable = new System.Data.DataTable();

            paretoTable.Columns.Add("Name", typeof(string));
            paretoTable.Columns.Add("Cumulative", typeof(double));
            paretoTable.Columns.Add("Absolute", typeof(double));
            paretoTable.Columns.Add("Relative", typeof(double));

            List<string> names = countedValues.Keys.ToList();
            double total = (double)countedValues.Values.Sum();

            foreach (string name in names)
            {
                DataRow dr = paretoTable.NewRow();
                double currentValue = (double)countedValues[name];
                double currentPercent = 100.0 * (currentValue / total);
                dr["Name"] = name;

                dr["Cumulative"] = Double.NaN; // Uninitiated value
                dr["Absolute"] = currentValue;
                dr["Relative"] = currentPercent;

                paretoTable.Rows.Add(dr);
            }

            paretoTable = paretoTable.CloneSelectedRows("", "Absolute desc");

            double cumulative = 0.0;
            foreach (DataRow dr in paretoTable.Rows)
            {
                cumulative = cumulative + (double)dr["Relative"];
                dr["Cumulative"] = cumulative;
            }
            paretoTable.AcceptChanges();
            return paretoTable;
        }
        public static DataTable TopRowsByOrder(this DataTable source, string selector, string sortExpression, int topNrows)
        {
            DataRow[] selection = source.Select(selector, sortExpression);
            DataTable retVal = source.Clone();
            for (int idx=0; idx<topNrows; idx++)
            {
                retVal.ImportRow(selection[idx]);
            }
            return retVal;
        }

        public static DataTable TopRowsByValueAsc(this DataTable source, string selector, string sortExpression, string column, double limit)
        {
            DataRow[] selection = source.Select(selector, sortExpression);
            DataTable retVal = source.Clone();
            int idx = 0;
            bool runFlag = true;
            while (runFlag)
            {
                retVal.ImportRow(selection[idx]);
                idx++;
                if (idx >= selection.Count())
                {
                    runFlag = false;
                } else if (Convert.ToDouble(selection[idx][column]) > limit)
                {
                    runFlag = false;
                }
            }
            return retVal;
        }

        public static DataTable TopRowsByValueDesc(this DataTable source, string selector, string sortExpression, string column, double limit)
        {
            DataRow[] selection = source.Select(selector, sortExpression);
            DataTable retVal = source.Clone();
            int idx = 0;
            bool runFlag = true;
            while (runFlag)
            {
                retVal.ImportRow(selection[idx]);
                idx++;
                if (idx >= selection.Count())
                {
                    runFlag = false;
                }
                else if (Convert.ToDouble(selection[idx][column]) < limit)
                {
                    runFlag = false;
                }
            }
            return retVal;
        }


        public static void ScaleValues(this System.Data.DataTable source, double multiplier)
        {
            for (int rIdx = 0; rIdx < source.Rows.Count; rIdx++)
            {
                for (int cIdx = 0; cIdx < source.Columns.Count; cIdx++)
                {
                    if (source.Columns[cIdx].DataType == typeof(double))
                    {
                        source.Rows[rIdx][cIdx] = (double)source.Rows[rIdx][cIdx] * multiplier;
                    }
                }
            }
        }


    }





}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BD.Inventory.Common
{
    public class CommonOperation
    {
        #region DataTable转实体

        /// <summary>
        /// 将DataTable转为实体类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ThisTable"></param>
        /// <returns></returns>
        public static T DataTableToModel<T>(DataTable ThisTable)
        {
            List<T> list = ConvertDataTableToModelList<T>(ThisTable);
            if (list != null && list.Count() > 0)
            {
                return list[0];
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 将DataTatable转为实体对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ThisTable"></param>
        /// <returns></returns>
        public static List<T> ConvertDataTableToModelList<T>(DataTable ThisTable)
        {
            //使用typeof运算符来获取T对象，只需提供类型名作为操作数，会返回Type对象的引用
            var ModelType = typeof(T);
            if (ThisTable == null || ThisTable.Rows.Count == 0)
            {
                return null;
            }
            var ModelList = new List<T>();

            var DataRowCollection = ThisTable.Rows;
            var DataColumnCollection = ThisTable.Columns;
            //两层循环来遍历这个二维数组
            //第一层循环 循环行数
            foreach (DataRow DR in DataRowCollection)
            {
                var Model = ModelType.Assembly.CreateInstance(ModelType.FullName);
                // 第二层循环，循环每行中的属性
                foreach (var p in Model.GetType().GetProperties())
                {
                    if (!ThisTable.Columns.Contains(p.Name))
                    {
                        continue;
                    }
                    if (DR[DataColumnCollection[p.Name]] is DBNull)
                    {
                        p.SetValue(Model, null, null);
                    }
                    else
                    {
                        switch (p.PropertyType.Name)
                        {
                            case "Int16":
                                p.SetValue(Model, NvlReturnInt(DR[DataColumnCollection[p.Name]]), null);
                                break;

                            case "Int32":
                                p.SetValue(Model, NvlReturnInt(DR[DataColumnCollection[p.Name]]), null);
                                break;

                            case "Int64":
                                p.SetValue(Model, NvlReturnLong(DR[DataColumnCollection[p.Name]]), null);
                                break;

                            case "Long":
                                p.SetValue(Model, NvlReturnLong(DR[DataColumnCollection[p.Name]]), null);
                                break;

                            case "String":
                                p.SetValue(Model, NvlReturnString(DR[DataColumnCollection[p.Name]]), null);
                                break;

                            case "Double":
                                p.SetValue(Model, NvlReturnDouble(DR[DataColumnCollection[p.Name]]), null);
                                break;

                            case "Decimal":
                                p.SetValue(Model, NvlReturnDecimal(DR[DataColumnCollection[p.Name]]), null);
                                break;

                            case "DateTime":
                                p.SetValue(Model, NvlReturnDateTime(DR[DataColumnCollection[p.Name]]), null);
                                break;
                            case "Nullable`1":
                                Type genericType = p.PropertyType.GetGenericArguments()[0];
                                if (genericType == typeof(int))
                                {
                                    int? value = NvlReturnInt(DR[DataColumnCollection[p.Name]]) as int?;
                                    p.SetValue(Model, value, null);
                                }
                                if (genericType == typeof(double))
                                {
                                    double? value = NvlReturnDouble(DR[DataColumnCollection[p.Name]]) as double?;
                                    p.SetValue(Model, value, null);
                                }
                                if (genericType == typeof(DateTime))
                                {
                                    DateTime? value = NvlReturnDateTime(DR[DataColumnCollection[p.Name]]) as DateTime?;
                                    p.SetValue(Model, value, null);
                                }
                                break;

                            default: break;
                        }
                    }
                }
                ModelList.Add((T)Model);
            }
            return ModelList;
        }

        #endregion DataTable转实体

        #region DataTable转JArray/ DataRow转JObject

        /// <summary>
        /// DataTable转JArray
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static JArray DataTable2JArray(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return null;
            var ja = new JArray();
            foreach (DataRow dr in dt.Rows)
            {
                ja.Add(DataRow2JObject(dr));
            }
            return ja;
        }

        /// <summary>
        /// DataRow转JObject
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static JObject DataRow2JObject(DataRow dr)
        {
            if (dr == null)
                return null;
            var jo = new JObject();
            foreach (DataColumn column in dr.Table.Columns)
            {
                switch (column.DataType.Name)
                {
                    case "Int16":
                        jo.Add(column.ColumnName, NvlReturnInt(dr[column]));
                        break;

                    case "Int32":
                        jo.Add(column.ColumnName, NvlReturnInt(dr[column]));
                        break;

                    case "Int64":
                        //jo.Add(column.ColumnName, NvlReturnLong(dr[column]));
                        jo.Add(column.ColumnName, NvlReturnString(dr[column]));
                        break;

                    case "Long":
                        jo.Add(column.ColumnName, NvlReturnLong(dr[column]));
                        break;

                    case "String":
                        jo.Add(column.ColumnName, NvlReturnString(dr[column]));
                        break;

                    case "Double":
                        jo.Add(column.ColumnName, NvlReturnDouble(dr[column]));
                        break;

                    case "Decimal":
                        jo.Add(column.ColumnName, NvlReturnDecimal(dr[column]));
                        break;

                    case "DateTime":
                        //jo.Add(column.ColumnName, NvlReturnDateTime(dr[column]));
                        jo.Add(column.ColumnName, NvlReturnDateTimeStr(dr[column]));
                        break;

                    default: break;
                }
            }
            return jo;
        }

        #endregion DataTable转JArray/ DataRow转JObject

        #region object类型转换

        /// <summary>
        /// object 转 int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int NvlReturnInt(object obj)
        {
            try
            {
                if (obj != null) return obj.ToString().Length == 0 ? 0 : Convert.ToInt32(obj);
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// object 转 long
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long NvlReturnLong(object obj)
        {
            try
            {
                if (obj != null)
                    return obj.ToString().Length == 0 ? 0 : Convert.ToInt64(obj);
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// object 转 string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string NvlReturnString(object strObject, bool blTrim = false, bool blUpper = false)
        {
            try
            {
                if (strObject == null) return string.Empty;
                var returnString = strObject.ToString();
                if (blTrim) returnString = returnString.Trim();
                if (blUpper) returnString = returnString.ToUpper();
                return returnString;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// object 转 double
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double NvlReturnDouble(object obj)
        {
            try
            {
                if (obj != null) return obj.ToString().Length == 0 ? 0 : Convert.ToDouble(obj);
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// object 转 decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal NvlReturnDecimal(object obj)
        {
            try
            {
                if (obj != null) return obj.ToString().Length == 0 ? 0 : Convert.ToDecimal(obj);
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// object 转 DateTime
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime NvlReturnDateTime(object obj)
        {
            try
            {
                if (obj != null) return Convert.ToDateTime(obj);
                return DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// object 转 日期字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string NvlReturnDateTimeStr(object obj)
        {
            try
            {
                if (obj != null) return Convert.ToDateTime(obj).ToString("yyyy-MM-dd HH:mm:ss");
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion object类型转换
    }
}
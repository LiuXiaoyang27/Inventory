﻿using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BD.Inventory.Common
{
    public class NPOIHelper
    {

        #region 从DataTable导出到excel文件中，支持xls和xlsx格式
        #region 导出为xls文件内部方法
        /// <summary>
        /// 从datatable 中导出到excel
        /// </summary>
        /// <param name="strFileName">excel文件名</param>
        /// <param name="dtSource">datatabe源数据</param>
        /// <param name="strHeaderText">表名</param>
        /// <param name="sheetnum">sheet的编号</param>
        /// <returns></returns>
        static MemoryStream ExportDT(string strFileName, DataTable dtSource, string strHeaderText, Dictionary<string, string> dir, int sheetnum)
        {
            //创建工作簿和sheet
            IWorkbook workbook = new HSSFWorkbook();
            using (Stream writefile = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.Read))
            {
                if (writefile.Length > 0 && sheetnum > 0)
                {
                    workbook = WorkbookFactory.Create(writefile);
                }
            }

            ISheet sheet = null;
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            ICellStyle dateStyle1 = workbook.CreateCellStyle();
            IDataFormat format1 = workbook.CreateDataFormat();
            dateStyle1.DataFormat = format1.GetFormat("yyyy-mm-dd HH:mm:ss");
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 0)
                {
                    string sheetName = strHeaderText + (sheetnum == 0 ? "" : sheetnum.ToString());
                    if (workbook.GetSheetIndex(sheetName) >= 0)
                    {
                        workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName));
                    }
                    sheet = workbook.CreateSheet(sheetName);
                    #region 表头及样式
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.IsBold = true;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;

                        rowIndex = 1;
                    }
                    #endregion

                    #region 列头及样式

                    if (rowIndex == 1)
                    {
                        IRow headerRow = sheet.CreateRow(1);//第二行设置列名
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.IsBold = true;
                        headStyle.SetFont(font);
                        //写入列标题
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(dir[column.ColumnName]);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256 * 2);
                        }
                        rowIndex = 2;
                    }
                    #endregion
                }
                #endregion

                #region 填充内容

                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            double result;
                            if (isNumeric(drValue, out result))
                            {
                                //数字字符串
                                double.TryParse(drValue, out result);
                                newCell.SetCellValue(result);
                                break;
                            }
                            else
                            {
                                newCell.SetCellValue(drValue);
                                break;
                            }

                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle; //格式化显示
                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue(drValue.ToString());
                            break;
                    }

                }
                #endregion
                rowIndex++;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }

        }
        #endregion

        #region 导出为xlsx文件内部方法
        /// <summary>
        /// 从datatable 中导出到excel
        /// </summary>
        /// <param name="dtSource">datatable数据源</param>
        /// <param name="strHeaderText">表名</param>
        /// <param name="fs">文件流</param>
        /// <param name="readfs">内存流</param>
        /// <param name="sheetnum">sheet索引</param>
        static void ExportDTI(DataTable dtSource, string strHeaderText, FileStream fs, MemoryStream readfs, Dictionary<string, string> dir, int sheetnum)
        {

            IWorkbook workbook = new XSSFWorkbook();
            if (readfs.Length > 0 && sheetnum > 0)
            {
                workbook = WorkbookFactory.Create(readfs);
            }
            ISheet sheet = null;
            ICellStyle dateStyle = workbook.CreateCellStyle();
            // 设置边框为实线
            dateStyle.BorderTop = BorderStyle.Thin;
            dateStyle.BorderBottom = BorderStyle.Thin;
            dateStyle.BorderLeft = BorderStyle.Thin;
            dateStyle.BorderRight = BorderStyle.Thin;
            // 设置水平对齐为居中（5代表水平居中）
            dateStyle.Alignment = HorizontalAlignment.Center;
            // 设置垂直对齐为居中（2代表垂直居中）
            dateStyle.VerticalAlignment = VerticalAlignment.Center;
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            // lxy
            ICellStyle dateStyle1 = workbook.CreateCellStyle();
            // 设置边框为实线
            dateStyle1.BorderTop = BorderStyle.Thin;
            dateStyle1.BorderBottom = BorderStyle.Thin;
            dateStyle1.BorderLeft = BorderStyle.Thin;
            dateStyle1.BorderRight = BorderStyle.Thin;
            // 设置水平对齐为居中（5代表水平居中）
            dateStyle1.Alignment = HorizontalAlignment.Center;
            // 设置垂直对齐为居中（2代表垂直居中）
            dateStyle1.VerticalAlignment = VerticalAlignment.Center;
            IDataFormat format1 = workbook.CreateDataFormat();
            dateStyle1.DataFormat = format1.GetFormat("yyyy-mm-dd HH:mm:ss");

            // lxy
            ICellStyle Style2 = workbook.CreateCellStyle();
            // 设置边框为实线
            Style2.BorderTop = BorderStyle.Thin;
            Style2.BorderBottom = BorderStyle.Thin;
            Style2.BorderLeft = BorderStyle.Thin;
            Style2.BorderRight = BorderStyle.Thin;
            // 设置水平对齐为居中（5代表水平居中）
            Style2.Alignment = HorizontalAlignment.Center;
            // 设置垂直对齐为居中（2代表垂直居中）
            Style2.VerticalAlignment = VerticalAlignment.Center;

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式

                if (rowIndex == 0)
                {
                    #region 表头及样式
                    {
                        string sheetName = strHeaderText + (sheetnum == 0 ? "" : sheetnum.ToString());
                        if (workbook.GetSheetIndex(sheetName) >= 0)
                        {
                            workbook.RemoveSheetAt(workbook.GetSheetIndex(sheetName));
                        }
                        sheet = workbook.CreateSheet(sheetName);
                        sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                        IRow headerRow = sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        ICellStyle headStyle = workbook.CreateCellStyle();
                        // 设置边框为实线
                        headStyle.BorderTop = BorderStyle.Thin;
                        headStyle.BorderBottom = BorderStyle.Thin;
                        headStyle.BorderLeft = BorderStyle.Thin;
                        headStyle.BorderRight = BorderStyle.Thin;

                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        //font.Boldweight = 700;
                        font.IsBold = true;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                    }
                    #endregion

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(1);
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        // 设置背景颜色
                        headStyle.FillForegroundColor = IndexedColors.PaleBlue.Index;
                        headStyle.FillPattern = FillPattern.SolidForeground;
                        // 设置边框为实线
                        headStyle.BorderTop = BorderStyle.Thin;
                        headStyle.BorderBottom = BorderStyle.Thin;
                        headStyle.BorderLeft = BorderStyle.Thin;
                        headStyle.BorderRight = BorderStyle.Thin;
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.IsBold = true;
                        headStyle.SetFont(font);


                        foreach (DataColumn column in dtSource.Columns)
                        {
                            if (dir.ContainsKey(column.ColumnName))
                            {
                                headerRow.CreateCell(column.Ordinal).SetCellValue(dir[column.ColumnName]);
                            }
                            else
                            {
                                // 如果字典中没有对应的列名，可以选择使用列名本身作为默认值
                                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            }

                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            // 设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256 * 2);

                        }
                    }

                    #endregion

                    rowIndex = 2;
                }
                #endregion

                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    string strColumn = column.ToString();
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            newCell.SetCellValue(drValue);
                            newCell.CellStyle = Style2;
                            break;
                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            newCell.CellStyle = dateStyle1; //格式化显示

                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            newCell.CellStyle = Style2;
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            if (strColumn.Equals("has_check"))
                            {
                                int.TryParse(drValue, out int intV);
                                string hascheck;
                                if(intV == 0)
                                {
                                    hascheck = "未盘";
                                    ICellStyle Style = workbook.CreateCellStyle();
                                    // 设置边框为实线
                                    Style.BorderTop = BorderStyle.Thin;
                                    Style.BorderBottom = BorderStyle.Thin;
                                    Style.BorderLeft = BorderStyle.Thin;
                                    Style.BorderRight = BorderStyle.Thin;
                                    // 设置水平对齐为居中（5代表水平居中）
                                    Style.Alignment = HorizontalAlignment.Center;
                                    // 设置垂直对齐为居中（2代表垂直居中）
                                    Style.VerticalAlignment = VerticalAlignment.Center;
                                    // 设置填充模式为实心填充
                                    Style.FillPattern = FillPattern.SolidForeground;                                    
                                    // 设置填充颜色为红色
                                    Style.FillForegroundColor = IndexedColors.Red.Index;
                                    newCell.SetCellValue(hascheck);
                                    newCell.CellStyle = Style;
                                }
                                else
                                {
                                    hascheck = "已盘";
                                    ICellStyle Style = workbook.CreateCellStyle();
                                    // 设置边框为实线
                                    Style.BorderTop = BorderStyle.Thin;
                                    Style.BorderBottom = BorderStyle.Thin;
                                    Style.BorderLeft = BorderStyle.Thin;
                                    Style.BorderRight = BorderStyle.Thin;
                                    // 设置水平对齐为居中（5代表水平居中）
                                    Style.Alignment = HorizontalAlignment.Center;
                                    // 设置垂直对齐为居中（2代表垂直居中）
                                    Style.VerticalAlignment = VerticalAlignment.Center;
                                    // 设置填充模式为实心填充
                                    Style.FillPattern = FillPattern.SolidForeground;
                                    // 设置填充颜色为绿色
                                    Style.FillForegroundColor = IndexedColors.BrightGreen.Index;
                                    newCell.SetCellValue(hascheck);
                                    newCell.CellStyle = Style;
                                }
                            }
                            else
                            {
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                newCell.CellStyle = Style2;
                            }
                            
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            newCell.CellStyle = Style2;
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            newCell.CellStyle = Style2;
                            break;
                        default:
                            newCell.SetCellValue(drValue.ToString());
                            newCell.CellStyle = Style2;
                            break;
                    }
                }
                #endregion
                rowIndex++;
            }
            workbook.Write(fs);
            fs.Close();
        }
        #endregion

        #region 导出excel表格
        /// <summary>
        ///  DataTable导出到Excel文件，xls文件
        /// </summary>
        /// <param name="dtSource">数据源</param>
        /// <param name="strHeaderText">表名</param>
        /// <param name="strFileName">excel文件名</param>
        /// <param name="dir">datatable和excel列名对应字典</param>
        /// <param name="sheetRow">每个sheet存放的行数</param>
        public static void ExportDTtoExcel(DataTable dtSource, string strHeaderText, string strFileName, Dictionary<string, string> dir, bool isNew, int sheetRow = 50000)
        {
            int currentSheetCount = GetSheetNumber(strFileName);//现有的页数sheetnum
            if (sheetRow <= 0)
            {
                sheetRow = dtSource.Rows.Count;
            }
            string[] temp = strFileName.Split('.');
            string fileExtens = temp[temp.Length - 1];
            int sheetCount = (int)Math.Ceiling((double)dtSource.Rows.Count / sheetRow);//sheet数目
            if (temp[temp.Length - 1] == "xls" && dtSource.Columns.Count < 256 && sheetRow < 65536)
            {
                if (isNew)
                {
                    currentSheetCount = 0;
                }
                for (int i = currentSheetCount; i < currentSheetCount + sheetCount; i++)
                {
                    DataTable pageDataTable = dtSource.Clone();
                    int hasRowCount = dtSource.Rows.Count - sheetRow * (i - currentSheetCount) < sheetRow ? dtSource.Rows.Count - sheetRow * (i - currentSheetCount) : sheetRow;
                    for (int j = 0; j < hasRowCount; j++)
                    {
                        pageDataTable.ImportRow(dtSource.Rows[(i - currentSheetCount) * sheetRow + j]);
                    }

                    using (MemoryStream ms = ExportDT(strFileName, pageDataTable, strHeaderText, dir, i))
                    {
                        using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                        {

                            byte[] data = ms.ToArray();
                            fs.Write(data, 0, data.Length);
                            fs.Flush();
                        }
                    }
                }
            }
            else
            {
                if (temp[temp.Length - 1] == "xls")
                    strFileName = strFileName + "x";
                if (isNew)
                {
                    currentSheetCount = 0;
                }
                for (int i = currentSheetCount; i < currentSheetCount + sheetCount; i++)
                {
                    DataTable pageDataTable = dtSource.Clone();
                    int hasRowCount = dtSource.Rows.Count - sheetRow * (i - currentSheetCount) < sheetRow ? dtSource.Rows.Count - sheetRow * (i - currentSheetCount) : sheetRow;
                    for (int j = 0; j < hasRowCount; j++)
                    {
                        pageDataTable.ImportRow(dtSource.Rows[(i - currentSheetCount) * sheetRow + j]);
                    }
                    FileStream readfs = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.Read);
                    MemoryStream readfsm = new MemoryStream();
                    readfs.CopyTo(readfsm);
                    readfs.Close();
                    using (FileStream writefs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                    {

                        ExportDTI(pageDataTable, strHeaderText, writefs, readfsm, dir, i);
                    }
                    readfsm.Close();
                }
            }
        }
        #endregion
        #endregion

        #region 从excel文件中将数据导出到datatable/datatable
        /// <summary>
        /// 将制定sheet中的数据导出到datatable中
        /// </summary>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <param name="dir">excel列名和DataTable列名的对应字典</param>
        /// <returns></returns>
        static DataTable ImportDt(ISheet sheet, int HeaderRowIndex, Dictionary<string, string> dir)
        {
            DataTable table = new DataTable();
            IRow headerRow;
            int cellCount;
            try
            {
                //没有标头或者不需要表头用excel列的序号（1,2,3..）作为DataTable的列名
                if (HeaderRowIndex < 0)
                {
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        DataColumn column = new DataColumn(Convert.ToString(i));
                        table.Columns.Add(column);
                    }
                }
                //有表头，使用表头做为DataTable的列名
                else
                {
                    headerRow = sheet.GetRow(HeaderRowIndex);
                    cellCount = headerRow.LastCellNum;
                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        //如果excel某一列列名不存在：以该列的序号作为Datatable的列名，如果DataTable中包含了这个序列为名的列，那么列名为重复列名+序号
                        if (headerRow.GetCell(i) == null)
                        {
                            if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                            {
                                DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                                table.Columns.Add(column);
                            }
                            else
                            {
                                DataColumn column = new DataColumn(Convert.ToString(i));
                                table.Columns.Add(column);
                            }

                        }
                        //excel中的某一列列名不为空，但是重复了：对应的Datatable列名为“重复列名+序号”
                        else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                        {
                            DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        //正常情况，列名存在且不重复：用excel中的列名作为datatable中对应的列名
                        {
                            string colName = dir.Where(s => s.Value == headerRow.GetCell(i).ToString()).First().Key;
                            DataColumn column = new DataColumn(colName);
                            table.Columns.Add(column);
                        }
                    }
                }
                int rowCount = sheet.LastRowNum;
                for (int i = HeaderRowIndex + 1; i <= sheet.LastRowNum; i++)//excel行遍历
                {
                    try
                    {
                        IRow row;
                        if (sheet.GetRow(i) == null)//如果excel有空行，则添加缺失的行
                        {
                            row = sheet.CreateRow(i);
                        }
                        else
                        {
                            row = sheet.GetRow(i);
                        }

                        DataRow dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j <= cellCount; j++)//excel列遍历
                        {
                            try
                            {
                                if (row.GetCell(j) != null)
                                {
                                    switch (row.GetCell(j).CellType)
                                    {
                                        case CellType.String://字符串
                                            string str = row.GetCell(j).StringCellValue;
                                            if (str != null && str.Length > 0)
                                            {
                                                dataRow[j] = str.ToString();
                                            }
                                            else
                                            {
                                                dataRow[j] = default(string);
                                            }
                                            break;
                                        case CellType.Numeric://数字
                                            if (DateUtil.IsCellDateFormatted(row.GetCell(j)))//时间戳数字
                                            {
                                                dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                            }
                                            else
                                            {
                                                dataRow[j] = Convert.ToDouble(row.GetCell(j).NumericCellValue);
                                            }
                                            break;
                                        case CellType.Boolean:
                                            dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                            break;
                                        case CellType.Error:
                                            dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                            break;
                                        case CellType.Formula://公式
                                            switch (row.GetCell(j).CachedFormulaResultType)
                                            {
                                                case CellType.String:
                                                    string strFORMULA = row.GetCell(j).StringCellValue;
                                                    if (strFORMULA != null && strFORMULA.Length > 0)
                                                    {
                                                        dataRow[j] = strFORMULA.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = null;
                                                    }
                                                    break;
                                                case CellType.Numeric:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                    break;
                                                case CellType.Boolean:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                                    break;
                                                case CellType.Error:
                                                    dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                                    break;
                                                default:
                                                    dataRow[j] = "";
                                                    break;
                                            }
                                            break;
                                        default:
                                            dataRow[j] = "";
                                            break;
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                throw exception;
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return table;
        }

        /// <summary>
        /// 读取Excel文件特定名字sheet的内容到DataTable
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <param name="dir">excel列名和DataTable列名的对应字典</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, Dictionary<string, string> dir, string SheetName, int HeaderRowIndex = 1)
        {
            DataTable table = new DataTable();
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (file.Length > 0)
                {
                    IWorkbook wb = WorkbookFactory.Create(file);
                    ISheet isheet = wb.GetSheet(SheetName);
                    table = ImportDt(isheet, HeaderRowIndex, dir);
                    isheet = null;
                }
            }
            return table;
        }

        /// <summary>
        /// 读取Excel文件某一索引sheet的内容到DataTable
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <param name="dir">excel列名和DataTable列名的对应字典</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, Dictionary<string, string> dir, int HeaderRowIndex = 1, int SheetIndex = 0)
        {
            DataTable table = new DataTable();
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (file.Length > 0)
                {
                    IWorkbook wb = WorkbookFactory.Create(file);
                    ISheet isheet = wb.GetSheetAt(SheetIndex);
                    table = ImportDt(isheet, HeaderRowIndex, dir);
                    isheet = null;
                }
            }
            return table;

        }
        #endregion



        /// <summary>
        /// 获取excel文件的sheet数目
        /// </summary>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static int GetSheetNumber(string outputFile)
        {
            int number = 0;
            using (FileStream readfile = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.Read))
            {
                if (readfile.Length > 0)
                {
                    IWorkbook wb = WorkbookFactory.Create(readfile);
                    number = wb.NumberOfSheets;
                }
            }
            return number;
        }

        /// <summary>
        /// 判断内容是否是数字
        /// </summary>
        /// <param name="message"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool isNumeric(string message, out double result)
        {
            Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 验证导入的Excel是否有数据
        /// </summary>
        /// <param name="excelFileStream"></param>
        /// <returns></returns>
        public static bool HasData(Stream excelFileStream)
        {
            using (excelFileStream)
            {
                IWorkbook workBook = new HSSFWorkbook(excelFileStream);
                if (workBook.NumberOfSheets > 0)
                {
                    ISheet sheet = workBook.GetSheetAt(0);
                    return sheet.PhysicalNumberOfRows > 0;
                }
            }
            return false;
        }
    }
}

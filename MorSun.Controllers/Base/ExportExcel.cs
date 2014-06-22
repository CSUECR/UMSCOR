using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HOHO18.Common;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using FastReflectionLib;
using System.Reflection;
using System.Web;
using System.Data;

namespace MorSun.Controllers
{
    public class ExportExcel
    {
        /// <summary>
        /// 以NOPI的方式获取EXCEL的数据,返回JSON格式数据
        /// </summary>
        /// <returns></returns>
        public static string GetPriceListByADO(string fileName, List<ColumnKeyValue> keyValues, int excelIndex = 0, int headIndex = 0, bool isDelFile = true)
        {
            List<string> res = new List<string>();
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            HSSFWorkbook xls = new HSSFWorkbook(fileStream);
            //获取完成数据后，删除此文件
            if (isDelFile)
            {
                System.IO.File.Delete(fileName);
            }
            var sheet = xls.GetSheetAt(excelIndex);
            var headRow = sheet.GetRow(headIndex);
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                StringBuilder item = new StringBuilder(); ;
                Row row = sheet.GetRow(i);
                if (row.RowNum == headIndex) { continue; }

                for (int j = 0; j < row.LastCellNum; j++)
                {
                    Cell cell = row.GetCell(j);
                    if (cell == null) { continue; }//如果单元格为NULL值，过滤此单元格
                    cell.SetCellType(CellType.STRING);
                    var val = RepSpelChar(cell.StringCellValue);
                    //获取表头
                    var headCell = headRow.GetCell(j);
                    headCell.SetCellType(CellType.STRING);
                    var excelColumn = headCell.StringCellValue;
                    var dataColumn = keyValues.Single(u => u.ExcelColumn == excelColumn).DataColumn;

                    if (!string.IsNullOrEmpty(item.ToString())) item.Append(",");
                    item.AppendFormat("\"{0}\":\"{1}\"", dataColumn, val);
                }
                res.Add("{" + item.ToString() + "}");
            }
            return "[" + res.Join(",") + "]";
        }

        /// <summary>
        /// 导出EXCEL数据,只能导出T的属性值，如果是更多层级的级的属性无法导出。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="keyValues"></param>
        /// <param name="fileName"></param>
        public static void LeadingOut<T>(List<T> list, List<ColumnKeyValue> keyValues, string fileName)
        {
            var template = HttpContext.Current.Server.MapPath(templateUrl);
            FileStream fileStream = new FileStream(template, FileMode.Open, FileAccess.Read);
            HSSFWorkbook xls = new HSSFWorkbook(fileStream);
            var sheet = xls.GetSheetAt(0);

            //设置表头
            var rowIndex = 0;
            var rowHead = sheet.CreateRow(rowIndex++);
            for (int colIndex = 0; colIndex < keyValues.Count; colIndex++)
            {
                var keyValue=keyValues[colIndex];
                var colHead = rowHead.CreateCell(colIndex, CellType.STRING);
                colHead.SetCellValue(keyValue.ExcelColumn);
            }

            //循环输出列
            foreach (var t in list)
	        {
                var row = sheet.CreateRow(rowIndex++);
                for (int colIndex = 0; colIndex < keyValues.Count; colIndex++)
                {
                    var keyValue = keyValues[colIndex];
                    string val = "";
                    try
                    {
                        val = t.GetType().GetProperties().Single(u => u.Name == keyValue.DataColumn).FastGetValue(t).ToString();
                    }
                    catch { }
                    Cell cell = row.CreateCell(colIndex, CellType.STRING);
                    cell.SetCellValue(val);
                }
	        }

            MemoryStream file = new MemoryStream();
            xls.Write(file);
            ResponseExcel(file,fileName);
        }


        public static void LeadingOut(DataTable dt,string fileName)
        {
            var template = HttpContext.Current.Server.MapPath(templateUrl);
            FileStream fileStream = new FileStream(template, FileMode.Open, FileAccess.Read);
            HSSFWorkbook xls = new HSSFWorkbook(fileStream);
            var sheet = xls.GetSheetAt(0);

            //设置表头
            var rowIndex = 0;
            var rowHead = sheet.CreateRow(rowIndex++);
            for (int colIndex = 0; colIndex< dt.Columns.Count; colIndex++)
            {
                var keyValue=dt.Columns[colIndex];
                var colHead = rowHead.CreateCell(colIndex, CellType.STRING);
                colHead.SetCellValue(keyValue.ColumnName);
            }

            //循环输出列
            foreach (DataRow t in dt.Rows)
	        {
                var row = sheet.CreateRow(rowIndex++);
                for (int colIndex = 0; colIndex < t.ItemArray.Length; colIndex++)
                {
                    var val = t[colIndex];
                    Cell cell = row.CreateCell(colIndex, CellType.STRING);
                    cell.SetCellValue(val.ToString());
                }
	        }

            MemoryStream file = new MemoryStream();
            xls.Write(file);
            ResponseExcel(file,fileName);
        }

        #region 私有方法
        private static void ResponseExcel(MemoryStream file, string fileName)
        {
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            if (!HttpContext.Current.Request.Browser.IsBrowser("Firefox"))
            {
                HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", System.Web.HttpUtility.UrlEncode(fileName)));
            }
            else
            {
                HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            }
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }
        private static string RepSpelChar(string obj)
        { 
            obj=obj.Replace("\\","\\\\")
                .Replace("\"","\\\"");
            return obj;
        }
        #endregion

        #region Field
        private static string templateUrl = "/Content/ExcelTemplets/emptyExcel.xls";
        #endregion
    }
    /// <summary>
    /// EXCEL和DATABASE的键值组合类
    /// </summary>
    public class ColumnKeyValue
    {
        public ColumnKeyValue(string excelColumn, string dataColumn)
        {
            this.DataColumn = dataColumn;
            this.ExcelColumn = excelColumn;
        }
        /// <summary>
        /// 数据库中对应的字段
        /// </summary>
        public string DataColumn { get; set; }
        /// <summary>
        /// EXCEL中对应的表头
        /// </summary>
        public string ExcelColumn { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace HOHO18.Common.Base
{
    /// <summary>
    /// 类名称：操Excle数据
    /// 制作人：HOHO18
    /// 制作日期：2010年9月15日
    /// </summary>
  public class ExcleHelper
    {
      public ExcleHelper()
      { }
      public static string strCon = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = d:\\1.xls;Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1\";";
      OleDbConnection myConn = null;

      /// <summary>
      ///  读取Excle数据到Dataset中
      /// </summary>
      /// <param name="sheetName">表名</param>
      /// <returns></returns>
      public DataSet ExcleToDataSet(string sheetName)
      {
          DataSet myDataSet = null;
          string sqlText = " SELECT * FROM ["+sheetName+"$] ";
          try
          {
              //创建一个数据链接
              myConn = new OleDbConnection(strCon);
              myConn.Open();
              //打开数据链接，得到一个数据集
              OleDbDataAdapter myCommand = new OleDbDataAdapter(sqlText, myConn);
              //DataSet对象
              myDataSet = new DataSet();
              //得到自己的DataSet对象
              myCommand.Fill(myDataSet, sheetName);

              return myDataSet;
          }
          catch (Exception e)
          {
              throw e;
          }
          finally 
          {
              //关闭此数据链接
              myConn.Close();
          }  
        }

      /// <summary>
      /// 读取Excle数据到Dataset中
      /// </summary>
     /// <param name="sheetName">表名称</param>
     /// <param name="startRecord">开始行</param>
     /// <param name="maxRecords">结束行</param>
     /// <returns></returns>
      public DataSet ExcleToDataSet(string sheetName,int startRecord, int maxRecords)
      {
          DataSet myDataSet = null;
          string sqlText = " SELECT * FROM [" + sheetName + "$] ";
          try
          {
              //创建一个数据链接
              myConn = new OleDbConnection(strCon);
              myConn.Open();
              //打开数据链接，得到一个数据集
              OleDbDataAdapter myCommand = new OleDbDataAdapter(sqlText, myConn);
              //DataSet对象
              myDataSet = new DataSet();
              //得到自己的DataSet对象
              myCommand.Fill(myDataSet, startRecord, maxRecords,sheetName);
              //myDataSet.Tables[0].
              return myDataSet;
          }
          catch (Exception e)
          {
              throw e;
          }
          finally
          {
              //关闭此数据链接
              myConn.Close();
          }
      }

      /// <summary>
      ///  读取Excle数据到Dataset中
      /// </summary>
      /// <param name="dataSource">数据源</param>
      /// <param name="sheetName">表名</param>
      /// <returns></returns>
      public DataSet ExcleToDataSet(string dataSource, string sheetName)
      {
          DataSet myDataSet = null;
          strCon = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = "+dataSource+";Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1\";";
          string sqlText = " SELECT * FROM [" + sheetName + "$] ";
          try
          {
              //创建一个数据链接
              myConn = new OleDbConnection(strCon);
              myConn.Open();
              //打开数据链接，得到一个数据集
              OleDbDataAdapter myCommand = new OleDbDataAdapter(sqlText, myConn);
              //DataSet对象
              myDataSet = new DataSet();
              //得到自己的DataSet对象
              myCommand.Fill(myDataSet, sheetName);

              return myDataSet;
          }
          catch (Exception e)
          {
              throw e;
          }
          finally
          {
              //关闭此数据链接
              myConn.Close();
          }
      }

      /// <summary>
      /// DataTable添加自定义列名
      /// </summary>
      /// <param name="ds">DataSet</param>
      /// <returns>DataTable</returns>
      public DataTable DataSetAddColumnName(DataSet ds)
      {
          var dsTable = ds.Tables[0];

           if (dsTable != null && dsTable.Rows.Count > 0)
           {
               for (int j = 0; j < dsTable.Columns.Count; j++)
               {
                   if (dsTable.Rows[0][j].ToString() != "")
                   {
                       dsTable.Columns[j].ColumnName = dsTable.Rows[0][j].ToString();
                   }
               }
           }
           dsTable.Rows.RemoveAt(0);
           return dsTable;
      }

      /// <summary>
      ///  读取Excle数据到Dataset中
      /// </summary>
      /// <param name="dataSource">数据源</param>
      /// <param name="sheetName">表名</param>
      /// <param name="startRecord">开始行</param>
      /// <param name="maxRecords">结束行</param>
      /// <returns></returns>
      public DataSet ExcleToDataSet(string dataSource, string sheetName, int startRecord, int maxRecords)
      {
          DataSet myDataSet = null;
          strCon = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = " + dataSource + ";Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1\";";
          string sqlText = " SELECT * FROM [" + sheetName + "$] ";
          try
          {
              //创建一个数据链接
              myConn = new OleDbConnection(strCon);
              myConn.Open();
              //打开数据链接，得到一个数据集
              OleDbDataAdapter myCommand = new OleDbDataAdapter(sqlText, myConn);
              //DataSet对象
              myDataSet = new DataSet();
              //得到自己的DataSet对象
              myCommand.Fill(myDataSet, startRecord, maxRecords, sheetName);

              return myDataSet;
          }
          catch (Exception e)
          {
              throw e;
          }
          finally
          {
              //关闭此数据链接
              myConn.Close();
          }
      }

      //导出Excel文件
      public void DataSetToExcle(DataSet ds, string fileName)
      {
          RenderDataTableToExcel(ds.Tables[0], fileName);
      }
      #region Kill Special Excel Process
      [DllImport("user32.dll", SetLastError = true)]
      static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
      #endregion

      public static Stream RenderDataTableToExcel(DataTable SourceTable)
      {
          HSSFWorkbook workbook = new HSSFWorkbook();
          MemoryStream ms = new MemoryStream();
          Sheet sheet = workbook.CreateSheet();
          Row headerRow = sheet.CreateRow(0);

          // handling header.
          foreach (DataColumn column in SourceTable.Columns)
              headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);

          // handling value.
          int rowIndex = 1;

          foreach (DataRow row in SourceTable.Rows)
          {
              Row dataRow = sheet.CreateRow(rowIndex);

              foreach (DataColumn column in SourceTable.Columns)
              {
                  dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
              }

              rowIndex++;
          }

          workbook.Write(ms);
          ms.Flush();
          ms.Position = 0;

          sheet = null;
          headerRow = null;
          workbook = null;

          return ms;
      }

      public static void RenderDataTableToExcel(DataTable SourceTable, string FileName)
      {
          MemoryStream ms = RenderDataTableToExcel(SourceTable) as MemoryStream;
          FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
          byte[] data = ms.ToArray();

          fs.Write(data, 0, data.Length);
          fs.Flush();
          fs.Close();

          data = null;
          ms = null;
          fs = null;
      }
    }
}

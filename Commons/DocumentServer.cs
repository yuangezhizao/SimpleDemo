using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;

namespace Commons
{
    public class DocumentServer
    {
        public static string ReadAllFilesByPath(string path)
        {
            if (!Directory.Exists(path))
                return "";
            DirectoryInfo dti = new DirectoryInfo(path);
            FileInfo[] allFile = dti.GetFiles();
            StringBuilder result = new StringBuilder();
            foreach (FileInfo fi in allFile)
            {
                var test = fi.OpenText();
                result.Append(fi.Name + "\t");
                result.Append(test.ReadToEnd() + "\r\n");
                test.Close();
                test.Dispose();
            }
            return result.ToString();
        }

        public static string ReadFileInfo(string fileName, string eccoding = "utf8")
        {

            if (!File.Exists(fileName))
                return "";
            Encoding cdg = eccoding == "utf8" ? Encoding.UTF8 : Encoding.Default;
            try
            {
                using (StreamReader sr = new StreamReader(fileName, cdg))
                {
                    string result = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
                return "";
            }
        }

        public static List<string> GetAllFileNameByPath(string path)
        {
            if (!Directory.Exists(path))
                return new List<string>();
            DirectoryInfo dti = new DirectoryInfo(path);
            FileInfo[] allFile = dti.GetFiles();

            List < string > result = new List<string>();
            foreach (var fi in allFile)
            {
                var test = fi.OpenText();
                result.Add(fi.Name);
                test.Close();
                test.Dispose();
            }
            return result;
        }
        public static string GetExcelToJson(string fileFullPath)
        {
            //string  strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + FileFullPath +";Extended Properties='Excel 8.0; HDR=NO; IMEX=1'"; //此连接只能操作Excel2007之前(.xls)文件
            var strConn = $"Provider=Microsoft.Ace.OleDb.12.0;data source = {fileFullPath};Extended Properties = 'Excel 12.0; HDR=NO; IMEX=1' ";//此连接可以操作.xls与.xlsx文件
            try
            {
                var conn = new OleDbConnection(strConn);
                conn.Open();
                DataSet ds = new DataSet();
                DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                    new object[] {null, null, null, "Table"});
                if (dtSheetName == null)
                {
                    conn.Close();
                    return "";
                }
                string[] strTableNames = new string[dtSheetName.Rows.Count];
                for (int k = 0; k < dtSheetName.Rows.Count; k++)
                {
                    strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
                }
                var odda = new OleDbDataAdapter($"SELECT * FROM[{strTableNames[0]}]", conn);
                odda.Fill(ds);
                conn.Close();
                return DataTableJson(ds.Tables[0]);
            }
            catch (Exception ex)
            {
                LogServer.WriteLog(ex);
                return "";
            }

        }
        private static string DataTableJson(DataTable dt)
        {
            var jsonBuilder = new StringBuilder();
            //jsonBuilder.Append("{\"");
            //jsonBuilder.Append(dt.TableName);
            //jsonBuilder.Append("\":[");
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    string tempvalue = dt.Rows[i][j].ToString();
                    tempvalue = tempvalue.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace(":", "").Replace("\"", "");
                    jsonBuilder.Append(tempvalue);
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            //jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

    }
}

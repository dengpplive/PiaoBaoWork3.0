using System;
using System.Data;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace PbProject.WebCommon.Utility.Encoding
{
    public static class JsonHelper
    {
        public static string _strErrMsg;
        public static string ObjToJson(this IJson source)
        {
            return ObjToJson(source, source.GetType());
        }

        public static string TbObjToJson(this IJson[] source)
        {
            return TbObjToJson(source, source.GetType());
        }

        public static string ObjToJson<T>(T obj)
        {
            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    json.WriteObject(stream, obj);
                    string szJson = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                    return szJson;
                }
            }
            catch (Exception ex) { _strErrMsg = ex.Message; return ""; }
            finally { }

        }

        public static string ObjToJson(this IJson source, Type type)
        {
            DataContractJsonSerializer serilializer = null;
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                serilializer = new DataContractJsonSerializer(type);
                stream = new MemoryStream();
                serilializer.WriteObject(stream, source);
                stream.Flush();
                stream.Position = 0;
                reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                _strErrMsg = ex.Message;
                return "";
            }
            finally
            {
                if (reader != null)
                    try { reader.Close(); }
                    catch { }
                    finally { reader = null; }
                if (stream != null)
                    try { stream.Close(); }
                    catch { }
                    finally { stream = null; }
                serilializer = null;
            }
        }

        public static T JsonToObj<T>(this string str)
        {
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(str)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }

        public static string TbObjToJson(this IJson[] source, Type type)
        {
            DataContractJsonSerializer serilializer = new DataContractJsonSerializer(type);
            System.Text.ASCIIEncoding converter = new System.Text.ASCIIEncoding();

            using (Stream stream = new MemoryStream())
            {
                serilializer.WriteObject(stream, source);
                stream.Flush();
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        /// <summary> 
        /// DataTable转成Json 
        /// </summary> 
        /// <param name="jsonName"></param> 
        /// <param name="dt"></param> 
        /// <returns></returns> 
        public static string DataTableToJson(DataTable dt,string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
                jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + StringFormat(dt.Rows[i][j].ToString(), type));
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
        /// <summary> 
        /// Datatable转换为Json 
        /// </summary> 
        /// <param name="table">Datatable对象</param> 
        /// <returns>Json字符串</returns> 
        public static string DataTableToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();

            if (dt.Rows.Count == 0)
            {
                jsonString.Append("[{}]");
                return jsonString.ToString();
            }

            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }


        /// <summary> 
        /// DataSet转换为Json 
        /// </summary> 
        /// <param name="dataSet">DataSet对象</param> 
        /// <returns>Json字符串</returns> 
        public static string DataSetToJson(DataSet dataSet)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                jsonString += "\"" + table.TableName + "\":" + DataTableToJson(table) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }

        /// <summary> 
        /// 过滤特殊字符 
        /// </summary> 
        /// <param name="s"></param> 
        /// <returns></returns> 
        private static string String2Json(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];

                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }
        /// <summary> 
        /// 格式化字符型、日期型、布尔型 
        /// </summary> 
        /// <param name="str"></param> 
        /// <param name="type"></param> 
        /// <returns></returns> 
        private static string StringFormat(string str, Type type)
        {
            if (type == typeof(string))
            {
                str = String2Json(str);
                str = "\"" + str + "\"";
            }
            else if (type == typeof(DateTime))
            {
                str = "\"" + Convert.ToDateTime(str).ToShortDateString() + "\"";
            }
            else if (type == typeof(bool))
            {
                str = str.ToLower();
            }

            if (str.Length == 0)
                str = "\"\"";

            return str;
        }
    

     


        // 通过JSON序列化深表Copy对象
        public static T DeepClone<T>(this IJson Source) where T : IJson
        {
            string jsonString = Source.ObjToJson();
            return jsonString.JsonToObj<T>();
        }

        public static T DeepClone<T>(T obj)
        {
            string jsonString = ObjToJson<T>(obj);
            return JsonToObj<T>(jsonString);
        }
    }
}

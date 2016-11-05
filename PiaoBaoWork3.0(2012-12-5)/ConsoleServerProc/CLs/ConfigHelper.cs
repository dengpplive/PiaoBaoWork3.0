using System;
using System.IO;
using System.Xml.Serialization;
namespace ConsoleServerProc
{
    /// <summary>
    /// 配置文件序列化帮助类
    /// </summary>
    public static class ConfigHelper
    {
        public static string CurrFilePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        /// <summary>
        ///  将对象序列化为XML 即： OBJECT -> XML   
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool SaveXml(string filePath, string fileName, object obj)
        {
            bool IsSuc = false;
            try
            {
                //获取对象的类型
                System.Type type = obj.GetType();
                //文件路径处理
                string path = filePath + fileName;
                string dir = System.IO.Path.GetDirectoryName(path);
                string filename = System.IO.Path.GetFileName(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                path = System.IO.Path.GetFullPath(path);
                //文件流和编码处理
                System.IO.FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                System.IO.StreamWriter writer = new System.IO.StreamWriter(fs, System.Text.Encoding.Default);
                //序列化
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(type);
                xs.Serialize(writer, obj);
                //清空缓冲区
                writer.Flush();
                fs.Flush();
                //关闭流
                writer.Close();
                fs.Close();
                IsSuc = true;
            }
            catch (Exception e)
            {
                //LogWrite.RecordLog("SaveXml:" + e.Message + "\r\n", "Err", true, "txt", new string[] { "", "HH" });
            }
            finally
            {
            }
            return IsSuc;
        }
        /// <summary>
        /// 将XML反序列化为对象 即：XML -> OBJECT
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Object LoadXml(string filePath, System.Type type)
        {
            try
            {
                //对象
                Object obj = null;
                //文件路径判断
                filePath = System.IO.Path.GetFullPath(CurrFilePath + filePath);
                if (!System.IO.File.Exists(filePath))
                    return obj;
                //流处理 反序列化
                using (System.IO.FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(fs, System.Text.Encoding.Default))
                    {
                        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(type);
                        obj = xs.Deserialize(reader);
                        reader.Close();
                    }
                    fs.Close();
                }
                return obj;
            }
            catch (Exception e)
            {
                //LogWrite.RecordLog("LoadXml:" + e.Message + "\r\n", "Err", true, "txt", new string[] { "", "HH" });
                return null;
            }
        }
    }
}

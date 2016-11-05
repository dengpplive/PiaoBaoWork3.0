using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Web;
namespace ConsoleServerProc
{
    /// <summary>
    /// 加密解密帮助类 支持中文
    /// </summary>
    public class CryptionDataHelper
    {
        /// <summary>
        /// 8个字节长度的的密钥 如:abcdefgh
        /// </summary>
        private string EncryptionString;

        /// <summary>
        /// 初始化IV向量 8字节长度 默认值如下
        /// </summary>
        private static Byte[] EncryptionIV = Encoding.Default.GetBytes("^%#@()&A");
        /// <summary>
        /// Key
        /// </summary>
        /// <param name="EncryptionString"></param>
        public CryptionDataHelper(string EncryptionString)
        {
            this.EncryptionString = EncryptionString;
        }

        /// <summary>
        /// 对字节数组加密
        /// </summary>
        /// <param name="SourceData"></param>
        /// <returns></returns>
        public byte[] EncryptionByteData(byte[] SourceData)
        {
            byte[] returnData = null;
            try
            {
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
                byte[] byteKey = Encoding.Default.GetBytes(EncryptionString);
                desProvider.Key = byteKey;
                desProvider.IV = EncryptionIV;
                MemoryStream ms = new MemoryStream();
                ICryptoTransform encrypto = desProvider.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(SourceData, 0, SourceData.Length);
                cs.FlushFinalBlock();
                returnData = ms.ToArray();
            }
            catch (Exception ex)
            {
                //LogWrite.RecordLog("EncryptionByteData:" + ex.Message + "\r\n", "ErrCrypt", true, "txt", new string[] { "", "HH" });
                throw ex;
            }

            return returnData;

        }
        /// <summary>
        /// 对字节数组解密
        /// </summary>
        /// <param name="SourceData"></param>
        /// <returns></returns>
        public byte[] DecryptionByteData(byte[] SourceData)
        {
            byte[] returnData = null;
            try
            {
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
                byte[] byteKey = Encoding.Default.GetBytes(EncryptionString);
                desProvider.Key = byteKey;
                desProvider.IV = EncryptionIV;
                MemoryStream ms = new MemoryStream();
                ICryptoTransform encrypto = desProvider.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(SourceData, 0, SourceData.Length);
                cs.FlushFinalBlock();
                returnData = ms.ToArray();
            }
            catch (Exception ex)
            {
                //LogWrite.RecordLog("DecryptionByteData:" + ex.Message + "\r\n", "ErrCrypt", true, "txt", new string[] { "", "HH" });
                throw ex;
            }
            return returnData;

        }


        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="SourceData"></param>
        /// <returns></returns>
        public string EncryptionStringData(string SourceData)
        {
            try
            {
                byte[] SourData = Encoding.Default.GetBytes(SourceData);
                byte[] retData = EncryptionByteData(SourData);
                return Convert.ToBase64String(retData, 0, retData.Length);
            }
            catch (Exception ex)
            {
                //LogWrite.RecordLog("EncryptionStringData:" + ex.Message + "\r\n", "ErrCrypt", true, "txt", new string[] { "", "HH" });
                throw ex;
            }
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="SourceData"></param>
        /// <returns></returns>
        public string DecryptionStringdata(string SourceData)
        {
            try
            {
                byte[] SourData = Convert.FromBase64String(SourceData);
                byte[] retData = DecryptionByteData(SourData);
                return Encoding.Default.GetString(retData, 0, retData.Length);
            }
            catch (Exception ex)
            {
                //LogWrite.RecordLog("DecryptionStringdata:" + ex.Message + "\r\n", "ErrCrypt", true, "txt", new string[] { "", "HH" });
                return "";
            }
        }
    }
}
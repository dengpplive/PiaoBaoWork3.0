namespace PbProject.WebCommon.Utility
{
    /*
     * 用法
     *  CryptionDataHelper Cd = new CryptionDataHelper("8位长度秘钥");
        string NewData= Cd.EncryptionStringData("需要加密的数据")
        string OldData= Cd.EncryptionStringData("需要解密的数据")
     * 
     * */

    using System;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;
    using System.Web;
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
        private static Byte[] EncryptionIV = System.Text.Encoding.Default.GetBytes("^%#@()&A");
        /// <summary>
        /// Key
        /// </summary>
        /// <param name="EncryptionString"></param>
        public CryptionDataHelper(string EncryptionString)
        {
            this.EncryptionString = EncryptionString;

        }
        /// <summary>
        /// Key
        /// </summary>
        public CryptionDataHelper()
        {

        }

        /// <summary>
        /// 对字节数组加密
        /// </summary>
        /// <param name="SourceData"></param>
        /// <returns></returns>
        public byte[] EncryptionByteData(byte[] SourceData)
        {
            byte[] returnData = null;

            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
            byte[] byteKey = System.Text.Encoding.Default.GetBytes(EncryptionString);
            desProvider.Key = byteKey;
            desProvider.IV = EncryptionIV;
            MemoryStream ms = new MemoryStream();
            ICryptoTransform encrypto = desProvider.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(SourceData, 0, SourceData.Length);
            cs.FlushFinalBlock();
            returnData = ms.ToArray();
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

            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
            byte[] byteKey = System.Text.Encoding.Default.GetBytes(EncryptionString);
            desProvider.Key = byteKey;
            desProvider.IV = EncryptionIV;
            MemoryStream ms = new MemoryStream();
            ICryptoTransform encrypto = desProvider.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(SourceData, 0, SourceData.Length);
            cs.FlushFinalBlock();
            returnData = ms.ToArray();

            return returnData;
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="SourceData"></param>
        /// <returns></returns>
        public string EncryptionStringData(string SourceData)
        {
            byte[] SourData = System.Text.Encoding.Default.GetBytes(SourceData);
            byte[] retData = EncryptionByteData(SourData);
            return Convert.ToBase64String(retData, 0, retData.Length);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="SourceData"></param>
        /// <returns></returns>
        public string DecryptionStringdata(string SourceData)
        {
            byte[] SourData = Convert.FromBase64String(SourceData);
            byte[] retData = DecryptionByteData(SourData);
            return System.Text.Encoding.Default.GetString(retData, 0, retData.Length);
        }

        /// <summary>MD5加密算法</summary>
        /// <param name="s">被加密字符串</param>
        /// <returns></returns>
        public string Md5(string s)
        {
            string md5 = "";
            md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "md5");
            md5 = "YYxhjPO)768_=" + md5 + "78HtPloaQQdvc" + "票宝";
            md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(md5, "md5");
            return md5;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace PbProject.ConsoleServerProc.Utils
{
    public class StringUtils
    {
        /// <summary>
        /// 返回大写的32位MD5值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetMd5(string data)
        {
            StringBuilder res = new StringBuilder();
            byte[] md5Bytes = MD5.Create().ComputeHash(Encoding.Default.GetBytes(data));
            for (int i = 0; i < md5Bytes.Length; i++)
            {
                res.Append(md5Bytes[i].ToString("X2"));
            }
            return res.ToString();
        }
    }
}

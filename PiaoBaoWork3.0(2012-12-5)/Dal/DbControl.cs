using System;
using System.Configuration;

namespace PbProject.Dal
{
    /// <summary>
    /// 与数据库交互行为定义
    /// </summary>
    public enum DbHandleBehaviorDefine
    {
        /// <summary>
        /// 插入
        /// </summary>
        WRITE = 0,
        /// <summary>
        /// 更新
        /// </summary>
        UPDATE = 1,
        /// <summary>
        /// 删除
        /// </summary>
        DELETE = 2,
        /// <summary>
        /// 查询
        /// </summary>
        SELECT = 3
    }

    public class DbControl
    {
        /// <summary>
        /// 获取基础数据库连接串
        /// </summary>
        /// <param name="behavior">行为</param>
        /// <param name="systemID">系统ID</param>
        /// <returns></returns>
        public static string GetDbConnStringBasic(DbHandleBehaviorDefine behavior, int systemID)
        {
            string result = "";
            int idx = systemID % 1;
            switch (behavior)
            {
                case DbHandleBehaviorDefine.WRITE:
                case DbHandleBehaviorDefine.UPDATE:
                case DbHandleBehaviorDefine.DELETE:
                    result = ConfigurationManager.ConnectionStrings["BaseConStrWrite"].ToString();
                    break;
                case DbHandleBehaviorDefine.SELECT:
                    switch (idx)
                    {
                        case 0:
                            result = ConfigurationManager.ConnectionStrings["BaseConStrRead"].ToString();
                            break;
                    }
                    break;
            }
            DbEncrOrDecr dbenorcr = new DbEncrOrDecr();
            //解密
            result = dbenorcr.Deciphering(result);
            return result;
        }

        /// <summary>
        /// 获取业务数据库连接串
        /// </summary>
        /// <param name="behavior">行为</param>
        /// <param name="systemID">系统ID</param>
        /// <returns></returns>
        public static string GetDbConnStringBusiness(DbHandleBehaviorDefine behavior, int systemID)
        {
            string result = "";
            int idx = systemID % 1;
            switch (behavior)
            {
                case DbHandleBehaviorDefine.WRITE:
                case DbHandleBehaviorDefine.UPDATE:
                case DbHandleBehaviorDefine.DELETE:
                    result = ConfigurationManager.ConnectionStrings["ConStrWrite"].ToString();
                    break;
                case DbHandleBehaviorDefine.SELECT:
                    switch (idx)
                    {
                        case 0:
                            result = ConfigurationManager.ConnectionStrings["ConStrRead"].ToString();
                            break;
                    }
                    break;
            }
            DbEncrOrDecr dbenorcr = new DbEncrOrDecr();
            //解密
            result = dbenorcr.Deciphering(result);
            return result;
        }
    }

    /// <summary>
    /// 数据库连接字符串加密解密
    /// </summary>
    public class DbEncrOrDecr
    {
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="ConfigString">需要加密的字符串</param>
        public string Encrypting(string ConfigString)
        {
            string mstrEncrypting = "";
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(ConfigString);
            mstrEncrypting = Convert.ToBase64String(data);
            return mstrEncrypting;
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="EncryptConfigString">加密字符串</param>
        public string Deciphering(string EncryptConfigString)
        {
            string mstrDeciphering = "";
            byte[] data = Convert.FromBase64String(EncryptConfigString);
            mstrDeciphering = System.Text.ASCIIEncoding.ASCII.GetString(data);
            return mstrDeciphering;
        }
    }
}

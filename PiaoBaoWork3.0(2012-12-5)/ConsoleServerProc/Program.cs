using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PbProject.ConsoleServerProc;
using PbProject.Logic;

namespace ConsoleServerProc
{
    public static class Program
    {
        public static string LoginUser = string.Empty;
        public static string LoginPwd = string.Empty;
        public static string CpyNo = string.Empty;
        public static bool IsAutologin = false;
        public static bool IsSavePwd = false;
        private static SessionContent m_UserModel = null;
        /// <summary>
        /// 登录用户所有信息
        /// </summary>
        public static SessionContent UserModel
        {
            get
            {
                return m_UserModel;
            }
            set
            {
                m_UserModel = value;
            }
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new PbMain());//PbMain LoginUI
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace PbProject.WebCommon.Script
{
    public class ScriptUtils
    {
        /// <summary>
        /// 弹出错误提示框(需注册)
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowError(string msg)
        {
            Page page = PageUtils.GetCurrentPage();
            string key = Guid.NewGuid().ToString();
            page.ClientScript.RegisterStartupScript(page.GetType(), key, "$(document).ready(function () { g_showError('" + msg + "'); });", true);
        }

        /// <summary>
        /// 弹出信息提示框(需注册)
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowInfo(string msg)
        {
            Page page = PageUtils.GetCurrentPage();
            string key = Guid.NewGuid().ToString();
            page.ClientScript.RegisterStartupScript(page.GetType(), key, "$(document).ready(function () { g_showInfo('" + msg + "'); });", true);
        }

        /// <summary>
        /// 弹出警告提示框(需注册)
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowWarn(string msg)
        {
            Page page = PageUtils.GetCurrentPage();
            string key = Guid.NewGuid().ToString();
            page.ClientScript.RegisterStartupScript(page.GetType(), key, "$(document).ready(function () { g_showWarn('" + msg + "'); });", true);
        }
    }
}

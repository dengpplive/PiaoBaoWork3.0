using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Web.UI;

namespace PbProject.WebCommon.Script
{
    public class PageUtils
    {
        /// <summary>
        /// 将指定路径的的js文件按层次加入到Header里
        /// </summary>
        /// <param name="url">js资源路径</param>
        /// <param name="index">加入Header的索引位置</param>
        public static void AddJavaScriptReferenceToHeader(string url, int index)
        {
            Page page = GetCurrentPage();
            HtmlGenericControl js = new HtmlGenericControl("script");
            js.Attributes["type"] = "text/javascript";
            js.Attributes["src"] = url;
            page.Header.Controls.AddAt(index, js);
        }

        /// <summary>
        /// 将制定路径的css文件按层次加入到Header里
        /// </summary>
        /// <param name="url">css资源路径</param>
        /// <param name="index">加入Header的索引位置</param>
        public static void AddStyleSheetToHeader(string url, int index)
        {
            Page page = GetCurrentPage();
            HtmlLink css = new HtmlLink();
            css.Href = url;
            css.Attributes["rel"] = "stylesheet";
            css.Attributes["type"] = "text/css";
            page.Header.Controls.AddAt(index, css);
        }

        /// <summary>
        /// 将指定的meta按层次添加到Header
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <param name="index"></param>
        public static void AddMetaToHeader(string name, string content, int index)
        {
            Page page = GetCurrentPage();
            HtmlMeta metadescription = new HtmlMeta();
            metadescription.Name = name;
            metadescription.Content = content;
            page.Header.Controls.AddAt(index, metadescription);
        }

        /// <summary>
        /// 注册jQuery
        /// </summary>
        public static void RegisterjQuery()
        {
            Page page = GetCurrentPage();
            string jquery = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.js.jquery-1.8.1.min.js");
            AddJavaScriptReferenceToHeader(jquery, 0);
        }

        /// <summary>
        /// 注册内部style.css文件
        /// </summary>
        public static void RegisterMainCss()
        {
            Page page = GetCurrentPage();
            string style = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.css.style.css");
            AddStyleSheetToHeader(style, 1);
        }

        /// <summary>
        /// 注册弹出提示框相关资源
        /// </summary>
        public static void RegisterMsgBox()
        {
            Page page = GetCurrentPage();
            string style = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.css.jquery.msgbox.css");
            string msgbox = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.js.jquery.msgbox.min.js");
            string msgboxinit = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.js.msgboxinit.js");
            AddStyleSheetToHeader(style, 2);
            AddJavaScriptReferenceToHeader(msgbox, 3);
            AddJavaScriptReferenceToHeader(msgboxinit, 4);
        }

        /// <summary>
        /// 注册验证框架相关资源
        /// </summary>
        public static void RegisterValidate()
        {
            Page page = GetCurrentPage();
            string validate = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.js.jquery.validate.min.js");
            string metadata = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.js.jquery.metadata.js");
            string messages_cn = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.js.messages_cn.js");
            string validateinit = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.js.validateinit.js");

            AddJavaScriptReferenceToHeader(validate, 2);
            AddJavaScriptReferenceToHeader(metadata, 3);
            AddJavaScriptReferenceToHeader(messages_cn, 4);
            AddJavaScriptReferenceToHeader(validateinit, 5);
        }

        /// <summary>
        /// 注册Hint功能相关资源
        /// </summary>
        public static void RegisterHintInfo()
        {
            Page page = GetCurrentPage();
            string cursorfocus = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.js.cursorfocus.js");
            AddJavaScriptReferenceToHeader(cursorfocus, 2);
        }

        public static void RegisterDialog()
        {
            Page page = GetCurrentPage();
            string dialog = page.ClientScript.GetWebResourceUrl(typeof(WebResource), "PbProject.WebCommon.Script.js.jquery.dialog.js");
            AddJavaScriptReferenceToHeader(dialog, 2);
        }

        //获取当前Page对象
        internal static Page GetCurrentPage()
        {
            if (HttpContext.Current == null)
                throw new Exception("当前上下文不存在");
            Page page = HttpContext.Current.Handler as Page;
            return page;
        }
    }
}

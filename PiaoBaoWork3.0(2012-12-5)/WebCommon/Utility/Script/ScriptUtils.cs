using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PbProject.WebCommon.Utility.Script
{
    public class ScriptUtils
    {
        public static void  ShowMsg(string title, string content)
        {
            WriteMsgCssAndScript();
            HttpContext.Current.Response.Write(@"
                <script type='text/javascript'>
                    window.onload = function () {
                        sAlert('" + title + "', '" + content + "');"

                + "}</script>");
        }

        private static void WriteMsgCssAndScript()
        {
            HttpContext.Current.Response.Write(@"<style type='text/css'>
                .msgDiv{word-wrap:break-word;}
            </style>");
            HttpContext.Current.Response.Write(@"
                <script type='text/javascript' language='javascript'>
        function sAlert(strTitle, strContent) {
            var msgw, msgh, bordercolor;
            msgw = 200; //提示窗口的宽度 
            msgh = 100; //提示窗口的高度 
            titleheight = 25 //提示窗口标题高度 
            bordercolor = '#336699'; //提示窗口的边框颜色 
            titlecolor = '#99CCFF'; //提示窗口的标题颜色 

            var sWidth, sHeight;
            sWidth = document.body.offsetWidth;
            sHeight = screen.height;
            var bgObj = document.createElement('div');
            bgObj.setAttribute('id', 'bgDiv');
            bgObj.style.position = 'absolute';
            bgObj.style.top = '0';
            bgObj.style.background = '#777';
            bgObj.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(style=3,opacity=25,finishOpacity=75';
            bgObj.style.opacity = '0.6';
            bgObj.style.left = '0';
            bgObj.style.width = sWidth + 'px';
            bgObj.style.height = sHeight + 'px';
            bgObj.style.zIndex = '10000';
            document.body.appendChild(bgObj);

            var msgObj = document.createElement('div')
            msgObj.setAttribute('id', 'msgDiv');
            msgObj.setAttribute('align', 'center');
            msgObj.style.background = 'white';
            msgObj.style.border = '1px solid ' + bordercolor;
            msgObj.style.position = 'absolute';
            msgObj.style.left = (sWidth - msgw) / 2 + 'px';  //'50%';
            msgObj.style.top = (document.body.offsetHeight/2 - msgh) + 'px';  //'50%';
            msgObj.style.font = '12px/1.6em Verdana, Geneva, Arial, Helvetica, sans-serif';
            //msgObj.style.marginLeft = '-225px';
            //msgObj.style.marginTop = -75 + document.documentElement.scrollTop + 'px';
            msgObj.style.width = msgw + 'px';
            msgObj.style.height = msgh + 'px';
            msgObj.style.textAlign = 'center';
            msgObj.style.lineHeight = '25px';
            msgObj.style.zIndex = '10001';
            msgObj.className = 'msgDiv'

            var title = document.createElement('h4');
            title.setAttribute('id', 'msgTitle');
            title.setAttribute('align', 'right');
            title.style.margin = '0';
            title.style.padding = '3px';
            title.style.background = bordercolor;
            title.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(startX=20, startY=20, finishX=100, finishY=100,style=1,opacity=75,finishOpacity=100);';
            title.style.opacity = '0.75';
            title.style.border = '1px solid ' + bordercolor;
            title.style.height = '18px';
            title.style.font = '12px Verdana, Geneva, Arial, Helvetica, sans-serif';
            title.style.color = 'white';
            title.style.cursor = 'pointer';
            title.title = '点击关闭';
            title.innerHTML = " + "\"<table border='0' width='100%'><tr><td align='left'><b>" + "\"" + " + strTitle + " + "\"" + "</b></td><td align='right'>关闭</td></tr></table></div>" + "\"" + @";
            title.onclick = function () {
                document.body.removeChild(bgObj);
                document.getElementById('msgDiv').removeChild(title);
                document.body.removeChild(msgObj);
            }
            document.body.appendChild(msgObj);
            document.getElementById('msgDiv').appendChild(title);
            var txt = document.createElement('p');
            txt.style.margin = '1em 0';
            txt.setAttribute('id', 'msgTxt');
            txt.innerHTML = strContent;
            document.getElementById('msgDiv').appendChild(txt);
        } 
    </script>
            ");
        }
    }
}

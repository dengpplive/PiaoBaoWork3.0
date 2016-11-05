using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
public partial class UserContrl_TimeCtrl : System.Web.UI.UserControl
{
    protected override void OnInit(EventArgs e)
    {
        if (!IsPostBack)
        {
            BindTime();
            CreateScript();
        }
        base.OnInit(e);
    }
    /// <summary>
    /// 初始值
    /// </summary>
    public void BindTime()
    {
        ddlHour0.Items.Clear();
        ddlHour1.Items.Clear();
        ddlMinute0.Items.Clear();
        ddlMinute1.Items.Clear();
        ddlSec0.Items.Clear();
        ddlSec1.Items.Clear();
        string Text = "", Val = "";
        for (int i = 0; i < 24; i++)
        {
            Text = i.ToString().PadLeft(2, '0');
            ListItem li = new ListItem(Text, Text);
            ddlHour0.Items.Add(li);
            ddlHour1.Items.Add(li);
        }
        for (int i = 0; i < 60; i++)
        {
            Text = i.ToString().PadLeft(2, '0');
            ListItem li = new ListItem(Text, Text);
            ddlMinute0.Items.Add(li);
            ddlMinute1.Items.Add(li);

            ddlSec0.Items.Add(li);
            ddlSec1.Items.Add(li);
        }
    }
    /// <summary>
    /// 设置控件隐藏显示 flase隐藏 true显示
    /// </summary>
    /// <param name="Hour0"></param>
    /// <param name="Minute0"></param>
    /// <param name="Sec0"></param>
    /// <param name="Hour1"></param>
    /// <param name="Minute1"></param>
    /// <param name="Sec1"></param>
    public void SetTimeShow(bool Hour0, bool Minute0, bool Sec0, bool Hour1, bool Minute1, bool Sec1)
    {
        ddlHour0.Visible = Hour0;
        ddlHour1.Visible = Hour1;

        ddlMinute0.Visible = Minute0;
        ddlMinute1.Visible = Minute1;

        ddlSec0.Visible = Sec0;
        ddlSec1.Visible = Sec1;

        if (!Sec1)
        {
            span_minute1.Visible = false;
        }
        if (!Sec0)
        {
            span_minute0.Visible = false;
        }
        if (!Minute1)
        {
            span_hour1.Visible = false;
        }
        if (!Minute0)
        {
            span_hour0.Visible = false;
        }
    }

    public void CreateScript()
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("<script language='javascript' type='text/javascript'>");
        sbScript.Append(" function GetValue_" + this.ID + "() {\r\n");       
        sbScript.Append(" var selHour0=document.getElementById('" + ddlHour0.ClientID + "');\r\n");
        sbScript.Append(" var selHour1=document.getElementById('" + ddlHour1.ClientID + "');\r\n");
        sbScript.Append(" var selMinute0=document.getElementById('" + ddlMinute0.ClientID + "');\r\n");
        sbScript.Append(" var selMinute1=document.getElementById('" + ddlMinute1.ClientID + "');\r\n");
        sbScript.Append(" var selSec0=document.getElementById('" + ddlSec0.ClientID + "');\r\n");
        sbScript.Append(" var selSec1=document.getElementById('" + ddlSec1.ClientID + "');\r\n");
        sbScript.Append(" var spanChar=document.getElementById('" + span_Char.ClientID + "');\r\n");
        sbScript.Append(" var spchar=spanChar!=null?spanChar.innerText:'-';\r\n");
        sbScript.Append(" var val_Hour0=selHour0!=null?selHour0.options[selHour0.selectedIndex].value:'00';\r\n");
        sbScript.Append(" var val_Hour1=selHour1!=null?selHour1.options[selHour1.selectedIndex].value:'00';\r\n");
        sbScript.Append(" var val_Minute0=selMinute0!=null?selMinute0.options[selMinute0.selectedIndex].value:'00';\r\n");
        sbScript.Append(" var val_Minute1=selMinute1!=null?selMinute1.options[selMinute1.selectedIndex].value:'00';\r\n");
        sbScript.Append(" var val_Sec0=selSec0!=null?selSec0.options[selSec0.selectedIndex].value:'00';\r\n");
        sbScript.Append(" var val_Sec1=selSec1!=null?selSec1.options[selSec1.selectedIndex].value:'00';\r\n");
        sbScript.Append(" var redata=val_Hour0+':'+val_Minute0+':'+val_Sec0+spchar+val_Hour1+':'+val_Minute1+':'+val_Sec1;\r\n");
        sbScript.Append(" return redata;}\r\n</script>");
        literScript.Text = sbScript.ToString();
    }

    /// <summary>
    /// 获取和设置时间控件的值  00:00:00-00:00:00
    /// </summary>
    public string Value
    {
        get
        {
            string Hour0 = "00";
            string Minute0 = "00";
            string Sec0 = "00";
            string Hour1 = "00";
            string Minute1 = "00";
            string Sec1 = "00";
            if (ddlHour0.Visible && ddlHour0.SelectedValue != "")
            {
                Hour0 = ddlHour0.SelectedValue.PadLeft(2, '0');
            }
            if (ddlHour1.Visible && ddlHour1.SelectedValue != "")
            {
                Hour1 = ddlHour1.SelectedValue.PadLeft(2, '0');
            }
            if (ddlMinute0.Visible && ddlMinute0.SelectedValue != "")
            {
                Minute0 = ddlMinute0.SelectedValue.PadLeft(2, '0');
            }
            if (ddlMinute1.Visible && ddlMinute1.SelectedValue != "")
            {
                Minute1 = ddlMinute1.SelectedValue.PadLeft(2, '0');
            }
            if (ddlSec0.Visible && ddlSec0.SelectedValue != "")
            {
                Sec0 = ddlSec0.SelectedValue.PadLeft(2, '0');
            }
            if (ddlSec1.Visible && ddlSec1.SelectedValue != "")
            {
                Sec1 = ddlSec1.SelectedValue.PadLeft(2, '0');
            }
            return string.Format("{0}:{1}:{2}{3}{4}:{5}:{6}", Hour0, Minute0, Sec0, span_Char.InnerText.Trim(), Hour1, Minute1, Sec1);
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                string[] strArr = value.Split(new string[] { span_Char.InnerText.Trim() }, StringSplitOptions.None);
                if (strArr != null && strArr.Length == 2)
                {
                    string[] strTime0 = strArr[0].Split(':');
                    string[] strTime1 = strArr[1].Split(':');
                    if (strTime0.Length == 3 && strTime1.Length == 3)
                    {
                        ddlHour0.SelectedValue = strTime0[0].PadLeft(2, '0');
                        ddlMinute0.SelectedValue = strTime0[1].PadLeft(2, '0');
                        ddlSec0.SelectedValue = strTime0[2].PadLeft(2, '0');

                        ddlHour1.SelectedValue = strTime1[0].PadLeft(2, '0');
                        ddlMinute1.SelectedValue = strTime1[1].PadLeft(2, '0');
                        ddlSec1.SelectedValue = strTime1[2].PadLeft(2, '0');
                    }
                }
            }
        }
    }

}
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using PbProject.Dal.ControlBase;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using System.Text;
using System.Collections;
public partial class UserControl_SelectAirCode : System.Web.UI.UserControl
{
    BaseData<Bd_Air_Carrier> Manage = new BaseData<Bd_Air_Carrier>();

    protected override void OnInit(EventArgs e)
    {
        txt_AirCo.ID = txt_AirCo.ID + "_" + this.ID;
        ddl_AirCode.ID = ddl_AirCode.ID + "_" + this.ID;
        scriptLiter.Text = CreateScript();
        //txt_AirCo.Attributes.Add("onpropertychange", "setAirCode_" + this.ID + "(this.value)");
        txt_AirCo.Attributes.Add("onkeyup", "setAirCode_" + this.ID + "(this.value)");
        ddl_AirCode.Attributes.Add("onchange", "ddlSetText_" + this.ID + "(this)");
        if (!IsPostBack)
        {
            DataBind();
        }
        base.OnInit(e);
    }

    public string CreateScript()
    {
        StringBuilder sbscript = new StringBuilder();
        sbscript.Append("<script type=\"text/javascript\">\r\n");
        sbscript.Append("function setAirCode_" + this.ID + "(TxtValue) {\r\n");
        sbscript.Append("var DropId='" + ddl_AirCode.ClientID + "';\r\n");
        sbscript.Append("var fnName='" + ChangeFunctionName + "';\r\n");
        sbscript.Append("var a='',b='',c='',d='',e='',f=document.getElementById(DropId);\r\n");
        sbscript.Append("if(f!=null&&f.options.length>0) {\r\n");
        sbscript.Append("if(TxtValue=='') {\r\n");
        sbscript.Append("f.options[0].selected=true;\r\n");
        sbscript.Append("return;\r\n");
        sbscript.Append("}\r\n");
        sbscript.Append("for(var i=0;i<f.options.length;i++) {\r\n");
        sbscript.Append("if(TxtValue.length<=f.options[i].text.length||f.options[i].value.length) {\r\n");
        sbscript.Append("a=TxtValue.toUpperCase();\r\n");
        sbscript.Append("b=f.options[i].text.substring(0,TxtValue.length).toUpperCase();\r\n");
        sbscript.Append("c=f.options[i].value.substring(0,TxtValue.length).toUpperCase();\r\n");
        sbscript.Append("d=f.options[i].text;\r\n");
        sbscript.Append("d=d.substring(d.lastIndexOf('-')+1,d.lastIndexOf('-')+1+a.length);\r\n");
        sbscript.Append("e=f.options[i].value;\r\n");
        sbscript.Append("if(a==c||a==b||a==e||a==d) {\r\n");
        sbscript.Append("f.options[i].selected=true;\r\n");
        //sbscript.Append("debugger\r\n");
        sbscript.Append(" document.getElementById('" + hf_AirCodeText.ClientID + "').value=f.options[i].text;\r\n");
        sbscript.Append("document.getElementById('" + this.ID + "_Hid_SelectIndex').value=f.selectedIndex;\r\n");
        sbscript.Append("if(fnName!=''&&typeof (fnName)=='string'" + (IsCompareLength ? ("&&TxtValue.length==" + this.InputMaxLength) : "") + ") {\r\n");
        sbscript.Append(" eval('" + ChangeFunctionName + "(\"'+f.options[i].text+'\",\"'+f.options[i].value+'\",f)');\r\n");
        sbscript.Append(" f.flag='1';\r\n");
        sbscript.Append("}\r\n");
        sbscript.Append("break;\r\n");
        sbscript.Append("}\r\n");
        sbscript.Append("}\r\n");
        sbscript.Append("}\r\n");
        sbscript.Append("}\r\n");
        sbscript.Append("}    \r\n");

        sbscript.Append("   function ddlSetText_" + this.ID + "(SelObj) {\r\n");
        sbscript.Append(" var txtID='" + txt_AirCo.ClientID + "';\r\n");
        sbscript.Append(" var fnName='" + ChangeFunctionName + "';\r\n");
        sbscript.Append(" var SelText=SelObj.options[SelObj.selectedIndex].text;\r\n");
        sbscript.Append(" var SelVal=SelObj.options[SelObj.selectedIndex].value;\r\n");
        sbscript.Append(" document.getElementById('" + hf_AirCodeText.ClientID + "').value=SelText;\r\n");
        sbscript.Append("document.getElementById('" + this.ID + "_Hid_SelectIndex').value=SelObj.selectedIndex;\r\n");
        sbscript.Append("if(SelText!=undefined&&SelText.replace(' ','')!='" + this.DefaultOptionText + "') {\r\n");
        sbscript.Append("    var ss=SelText.split('-');\r\n");
        sbscript.Append("    document.getElementById(txtID).value=ss[0];\r\n");
        sbscript.Append("} else {\r\n");
        sbscript.Append("     document.getElementById(txtID).value='';\r\n");
        sbscript.Append(" }\r\n");
        sbscript.Append(" if(fnName!=''&&typeof (fnName)=='string'&&(SelObj.flag=='0'||SelObj.flag==null)) {\r\n");
        sbscript.Append("     eval(''+fnName+'(\"'+SelText+'\",\"'+SelVal+'\",SelObj);');\r\n");
        sbscript.Append(" }\r\n");
        sbscript.Append(" SelObj.flag='0';\r\n");
        sbscript.Append(" }\r\n");

        sbscript.Append("window.onload=function(){ var index=document.getElementById('" + this.ID + "_Hid_SelectIndex').value;\r\n");
        sbscript.Append("var DropId='" + this.ID + "_" + ddl_AirCode.ID + "';\r\n");
        sbscript.Append("var ddl=document.getElementById(DropId);\r\n");
        sbscript.Append("if(ddl!=null&&index>=0&&index<ddl.options.length){ \r\n");
        sbscript.Append("ddl.options[index].selected=true;}  \r\n");
        sbscript.Append("}\r\n");
        sbscript.Append("</script>\r\n");
        return sbscript.ToString();
    }

    /// <summary>
    /// 绑定数据
    /// </summary>
    public void DataBind()
    {
        try
        {
            if (IsDShowName)
            {
                airSpan.Visible = true;
            }
            else
            {
                airSpan.Visible = false;
            }
            if (this.DataSource == null && this.DataTableSource == null)
            {
                Bd_Air_Carrier[] allAir = Manage.GetArray("");
                ddl_AirCode.Items.Clear();
                if (IsShowAll)
                {
                    ddl_AirCode.Items.Add(new ListItem(this.DefaultOptionText, this.DefaultOptionValue));
                    txt_AirCo.Text = this.DefaultOptionValue;
                }
                foreach (Bd_Air_Carrier item in allAir)
                {
                    ListItem items = new ListItem();
                    items.Text = item.Code + "-" + item.ShortName;
                    items.Value = item.Code;
                    ddl_AirCode.Items.Add(items);
                }
                //最大输入字符数
                txt_AirCo.MaxLength = this.InputMaxLength;
                //设置下拉列表宽度
                ddl_AirCode.Width = this.ddlWidth;
                //文本框宽度
                txt_AirCo.Width = this.TxtWidth;
            }
            else
            {
                ddl_AirCode.Items.Clear();
                Type t = null;
                string[] strArr = null;
                List<string> TextList = null;
                List<string> ValList = null;
                if (this.DataSource != null)
                {
                    foreach (Object item in this.DataSource)
                    {
                        t = item.GetType();
                        if (t.IsClass)
                        {
                            //绑定Class类型的List
                            strArr = this.DataFiledText.Split(new string[] { this.SplitChar }, StringSplitOptions.None);
                            TextList = new List<string>();
                            foreach (string strPName in strArr)
                            {
                                TextList.Add(t.GetProperty(strPName).GetValue(item, null).ToString());
                            }
                            strArr = this.DataFiledValue.Split(new string[] { this.SplitChar }, StringSplitOptions.None);
                            ValList = new List<string>();
                            foreach (string strPName in strArr)
                            {
                                ValList.Add(t.GetProperty(strPName).GetValue(item, null).ToString());
                            }
                            ListItem items = new ListItem();
                            items.Text = string.Join(this.SplitChar, TextList.ToArray());
                            items.Value = string.Join(this.SplitChar, ValList.ToArray());
                            ddl_AirCode.Items.Add(items);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //数据源为DataTable
                    if (this.DataTableSource != null)
                    {
                        foreach (DataRow dr in this.DataTableSource.Rows)
                        {
                            //显示列数据
                            strArr = this.DataFiledText.Split(new string[] { this.SplitChar }, StringSplitOptions.None);
                            TextList = new List<string>();
                            foreach (string strPName in strArr)
                            {
                                TextList.Add(dr[strPName] != DBNull.Value ? dr[strPName].ToString() : "");
                            }
                            //绑定值数据
                            strArr = this.DataFiledValue.Split(new string[] { this.SplitChar }, StringSplitOptions.None);
                            ValList = new List<string>();
                            foreach (string strPName in strArr)
                            {
                                ValList.Add(dr[strPName] != DBNull.Value ? dr[strPName].ToString() : "");
                            }
                            ListItem items = new ListItem();
                            items.Text = string.Join(this.SplitChar, TextList.ToArray());
                            items.Value = string.Join(this.SplitChar, ValList.ToArray());
                            ddl_AirCode.Items.Add(items);
                        }
                    }
                }
                //绑定值类型的List
                if (t != null && t.IsValueType)
                {
                    ddl_AirCode.DataSource = this.DataSource;
                    ddl_AirCode.DataBind();
                }
                if (IsShowAll)
                {
                    ddl_AirCode.Items.Insert(0, new ListItem(this.DefaultOptionText, this.DefaultOptionValue));
                    txt_AirCo.Text = this.DefaultOptionValue;
                }
                //最大输入字符数
                txt_AirCo.MaxLength = this.InputMaxLength;
                //设置下拉列表宽度
                ddl_AirCode.Width = this.ddlWidth;
                //文本框宽度
                txt_AirCo.Width = this.TxtWidth;
                t = null;
                strArr = null;
                TextList = null;
                ValList = null;
            }
            scriptLiter.Text = CreateScript();
        }
        catch
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "alert('航空公司控件加载错误!');", true);
        }
    }


    #region 控件属性

    //private IList _DataSource = null;
    public IList DataSource
    {
        get
        {
            return (ViewState["_DataSource"] as IList);
        }
        set
        {
            ViewState["_DataSource"] = value;
        }
    }

    //private DataTable _DataTableSource = null;
    /// <summary>
    /// 数据源为DataTable
    /// </summary>
    public DataTable DataTableSource
    {
        get
        {
            return (ViewState["_DataTableSource"] as DataTable);
        }
        set
        {
            ViewState["_DataTableSource"] = value;
        }
    }

    //public string _DataFiledText = string.Empty;
    /// <summary>
    /// 绑定字段显示名
    /// </summary>
    public string DataFiledText
    {
        get
        {
            return (string)ViewState["_DataFiledText"];
        }
        set
        {
            ViewState["_DataFiledText"] = value;
        }
    }
    //public string _DataFiledValue = string.Empty;
    /// <summary>
    /// 绑定字段值
    /// </summary>
    public string DataFiledValue
    {
        get
        {
            return (string)ViewState["_DataFiledValue"];
        }
        set
        {
            ViewState["_DataFiledValue"] = value;
        }
    }

    // private string _SplitChar = "-";
    /// <summary>
    /// 获取和设置绑定多个字段连接符号  默认"-"
    /// </summary>
    public string SplitChar
    {
        get
        {
            return ViewState["_SplitChar"] == null ? "-" : (string)ViewState["_SplitChar"];
        }
        set
        {
            ViewState["_SplitChar"] = value;
        }
    }
    /// <summary>
    /// 获取和设置下拉列表的值
    /// </summary>
    /// <returns></returns>
    public string Value
    {
        get
        {
            string str = Request.Form[ddl_AirCode.UniqueID];
            if (string.IsNullOrEmpty(str))
            {
                int Count = ddl_AirCode.Items.Count;
                for (int i = 0; i < Count; i++)
                {
                    ListItem item = ddl_AirCode.Items[i];
                    if (item.Text != this.DefaultOptionText && (item.Selected || Hid_SelectIndex.Value == i.ToString()))
                    {
                        str = item.Value;
                        break;
                    }
                }
            }
            return str == null ? "" : str;
        }
        set
        {
            DataBind();
            ddl_AirCode.Text = value;
            int Count = ddl_AirCode.Items.Count;
            for (int i = 0; i < Count; i++)
            {
                ListItem item = ddl_AirCode.Items[i];
                if (item.Value.ToUpper() == value.ToUpper())
                {
                    //清空选择项
                    ddl_AirCode.ClearSelection();
                    if (item.Value == DefaultOptionValue)
                    {
                        txt_AirCo.Text = "";
                    }
                    else
                    {
                        txt_AirCo.Text = item.Value.ToUpper();
                    }
                    item.Selected = true;
                    Hid_SelectIndex.Value = i.ToString();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 获取和设置下拉列表的文本
    /// </summary>
    /// <returns></returns>
    public string Text
    {
        get
        {
            string str = Request.Form[hf_AirCodeText.UniqueID];
            if (string.IsNullOrEmpty(str))
            {
                int Count = ddl_AirCode.Items.Count;
                for (int i = 0; i < Count; i++)
                {
                    ListItem item = ddl_AirCode.Items[i];
                    if (item.Text != this.DefaultOptionText && (item.Selected || Hid_SelectIndex.Value == i.ToString()))
                    {
                        str = item.Text;
                        break;
                    }
                }
            }
            return str == null ? "" : str;
        }
        set
        {
            DataBind();
            int Count = ddl_AirCode.Items.Count;
            for (int i = 0; i < Count; i++)
            {
                ListItem item = ddl_AirCode.Items[i];
                if (item.Text.ToUpper() == value.ToUpper())
                {
                    //清空选择项
                    ddl_AirCode.ClearSelection();
                    if (item.Text == DefaultOptionText)
                    {
                        txt_AirCo.Text = "";
                    }
                    else
                    {
                        txt_AirCo.Text = item.Text.ToUpper();
                    }
                    item.Selected = true;
                    Hid_SelectIndex.Value = i.ToString();
                    break;
                }
            }
            hf_AirCodeText.Value = value;
        }
    }

    //private bool _IsShowName = true;
    /// <summary>
    /// 是否显示控件提示名称
    /// </summary>
    public bool IsDShowName
    {
        get
        {
            return ViewState["_IsShowName"] == null ? true : (bool)ViewState["_IsShowName"];
        }
        set
        {
            ViewState["_IsShowName"] = value;
        }
    }


    // private bool _isShowAll = true;
    /// <summary>
    /// 是否添加所有航空公司显示
    /// </summary>
    public bool IsShowAll
    {
        get { return ViewState["_isShowAll"] == null ? true : (bool)ViewState["_isShowAll"]; }
        set { ViewState["_isShowAll"] = value; }
    }


    // private string _ChangeFunctionName = string.Empty;
    /// <summary>
    /// 数据发生变化执行js函数 函数名称不能是页面上已有的函数名称 三个参数 Fn(选择的文本,选择的值,列表对象)
    /// </summary>
    public string ChangeFunctionName
    {
        get
        {
            return (string)ViewState["_ChangeFunctionName"];
        }
        set
        {
            ViewState["_ChangeFunctionName"] = value;
        }
    }

    // private string _CaptionName = string.Empty;
    /// <summary>
    /// 控件描述名称 默认值：承运人: 
    /// </summary>
    public string CaptionName
    {
        get
        {
            return (string)ViewState["_CaptionName"];
        }
        set
        {
            airSpan.InnerText = value;
            ViewState["_CaptionName"] = value;
        }
    }

    // private string _DefaultOptionText = "所有航空公司";
    /// <summary>
    /// option默认第一项Item文本 默认值：所有航空公司
    /// </summary>
    public string DefaultOptionText
    {
        get
        {
            return ViewState["_DefaultOptionText"] == null ? "所有航空公司" : (string)ViewState["_DefaultOptionText"];
        }
        set
        {
            ViewState["_DefaultOptionText"] = value;
        }
    }

    //private string _DefaultOptionValue = "0";
    /// <summary>
    /// option默认第一项Item文本 默认值：所有航空公司
    /// </summary>
    public string DefaultOptionValue
    {
        get
        {
            return ViewState["_DefaultOptionValue"] == null ? "0" : (string)ViewState["_DefaultOptionValue"];
        }
        set
        {
            ViewState["_DefaultOptionValue"] = value;
        }
    }
    //设置文本框最大输入字符数 默认2
    public int InputMaxLength
    {
        get
        {
            return txt_AirCo.MaxLength;
        }
        set
        {
            txt_AirCo.MaxLength = value;
        }
    }

    // private bool _IsCompareLength = true;
    //是否比较输入字符长度
    public bool IsCompareLength
    {
        get
        {
            return ViewState["_IsCompareLength"] == null ? true : (bool)ViewState["_IsCompareLength"];
        }
        set
        {
            ViewState["_IsCompareLength"] = value;
        }
    }


    //private int m_ddlWidth = 118;
    /// <summary>
    /// 设置下拉列表的宽度
    /// </summary>
    public int ddlWidth
    {
        get
        {
            return ViewState["m_ddlWidth"] == null ? 118 : (int)ViewState["m_ddlWidth"];
        }
        set
        {
            ViewState["m_ddlWidth"] = value;
        }
    }


    // private int m_TxtWidth = 30;
    /// <summary>
    /// 设置下文本框的宽度
    /// </summary>
    public int TxtWidth
    {
        get
        {
            return ViewState["m_TxtWidth"] == null ? 30 : (int)ViewState["m_TxtWidth"];
        }
        set
        {
            ViewState["m_TxtWidth"] = value;
        }
    }


    #endregion
}
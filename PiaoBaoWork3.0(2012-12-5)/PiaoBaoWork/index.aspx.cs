using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using DataBase.Data;
using PbProject.Model;
using PbProject.WebCommon.Utility;
public partial class index : BasePage
{
    #region 属性
    /// <summary>
    /// 当前分页第几页
    /// </summary>
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            Curr = 1;
            Hid_IsPager.Value = "0";
            AspNetPager1.PageSize = int.Parse(hid_PageSize.Value);
            BindNotice();
        }

        if (mUser.LoginName.ToUpper().StartsWith("HQ") && (mUser.Phone == "13888888888"))
        {
            //AccountInfo.Text = ("<H2>请您尽快维护个人资料信息，以便于更好的为您服务！</H2>");
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "Wopen()", true);
        }
    }
    /// <summary>
    /// 显示部分字符
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Len"></param>
    /// <returns></returns>
    public string SubChar(object Str, int Len, string replaceSchar)
    {
        string reStr = "";
        if (Str == null) return reStr;
        reStr = CommonManage.StripHTML(CommonManage.ReplaceCharToXML(Str.ToString()));
        if (!string.IsNullOrEmpty(reStr))
        {
            if (reStr.Length > Len)
            {
                reStr = reStr.Substring(0, Len) + " " + replaceSchar;
            }
        }
        else
        {
            reStr = "";
        }
        return reStr;
    }

    /// <summary>
    /// 绑定公告
    /// </summary>
    public void BindNotice()
    {
        StringBuilder sbSQLwhere = new StringBuilder();
        sbSQLwhere.AppendFormat(" (ReleaseCpyNo=left('{0}',12) or ReleaseCpyNo=left('{0}',6)) and CallboardType=1  ", mCompany.UninCode);
        //获取已审核 有效公告 内部或者全部公告
        if (mCompany.RoleType == 1)//平台管理员
        {
            //sbSQLwhere.Append(" and (IsInternal=1 or IsInternal=3) ");
        }
        else if (mCompany.RoleType == 2 || mCompany.RoleType == 3)//供应商和落地运营商
        {
            sbSQLwhere.Append(" and (IsInternal=1 or IsInternal=3) ");
        }
        else if (mCompany.RoleType > 3)//采购 分销
        {
            sbSQLwhere.Append(" and (IsInternal=2 or IsInternal=3) ");
        }
        sbSQLwhere.AppendFormat(" and StartDate <= '{0}' and ExpirationDate >= '{1}'", DateTime.Now.ToString(), DateTime.Now.ToString());
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        outParams.Add("1", "out");
        List<Bd_Base_Notice> notice = baseDataManage.CallMethod("Bd_Base_Notice", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", sbSQLwhere.ToString(), " Emergency " }) as List<Bd_Base_Notice>;
        TotalCount = outParams.GetValue<int>("1");
        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;

        //查找紧急公告
        Bd_Base_Notice Notice = notice.Find(delegate(Bd_Base_Notice _notice)
        {
            return _notice.Emergency == 1;
        });
        if (Notice != null)
        {
            Hid_Emergency.Value = Notice.id.ToString();
        }
        else
        {
            Hid_Emergency.Value = "";
        }
        if (notice.Count > 0)
        {
            divNotice.Attributes.Add("class", "show");
        }
        else
        {
            divNotice.Attributes.Add("class", "hide");
        }
        //绑定
        Repeater.DataSource = notice;
        Repeater.DataBind();
        if (AspNetPager1.PageCount > 1)
        {
            Pagefooter.Attributes.Add("class", "show");
        }
        else
        {
            Pagefooter.Attributes.Add("class", "hide");
        }
    }

    /// <summary>
    /// 分页事件
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        Hid_IsPager.Value = "1";
        BindNotice();
    }

    /// <summary>
    /// 页面显示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string ShowText(int type, params object[] obj)
    {
        string result = "";
        if (type == 0)
        {
            //显示下载附件
            if (obj != null && obj.Length == 3)
            {
                string FileName = obj[0].ToString().Split('|')[0];
                byte[] b = obj[1] as byte[];
                if (b != null && b.Length > 10)
                {
                    result = FileName + "【<a  href='Manager/Sys/DownLoadFile.aspx?did=" + obj[2].ToString() + "&currentuserid=" + this.currentuserid.Value.ToString() + "'>下载附件</a>】";
                }
            }
        }
        else if (type == 1)//是否紧急公告
        {
            if (obj != null && obj.Length == 1)
            {
                if (obj[0] != null && obj[0].ToString() == "1")
                {
                    result = "<font class=\"red\">紧急公告</font>";
                }
                else
                {
                    result = "<font>普通公告</font>";
                }
            }
        }
        else if (type == 2)//紧急公告标题加红
        {
            if (obj != null && obj.Length == 3)
            {
                if (obj[0].ToString() == "1")
                {
                    result = "<a target=\"_blank\" href=\"Manager/Sys/LookBulletin.aspx?id=" + obj[2].ToString() + "&currentuserid=" + this.currentuserid.Value.ToString() + "\"><font class=\"red\">" + obj[1].ToString() + "</font></a>";
                }
                else
                {
                    result = "<a target=\"_blank\" href=\"Manager/Sys/LookBulletin.aspx?id=" + obj[2].ToString() + "&currentuserid=" + this.currentuserid.Value.ToString() + "\">" + obj[1].ToString() + "</a>";
                }
            }
        }
        else if (type == 3)//公告内容特殊字符处理
        {
            if (obj != null && obj.Length == 1)
            {
                result = CommonManage.StripHTML(CommonManage.ReplaceCharToXML(obj[0].ToString()));
            }
        }
        return result;
    }
}
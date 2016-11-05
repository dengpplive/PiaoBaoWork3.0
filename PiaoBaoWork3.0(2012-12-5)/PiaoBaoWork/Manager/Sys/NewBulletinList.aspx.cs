using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using DataBase.Data;
using PbProject.Model;
public partial class Sys_BulletinList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            PageDataBind();
        }
    }
    private void PageDataBind()
    {
        StringBuilder sbSQLwhere = new StringBuilder();
        sbSQLwhere.AppendFormat(" (ReleaseCpyNo=left('{0}',12) or ReleaseCpyNo=left('{0}',6)) and CallboardType=1  ", mCompany.UninCode);
        //获取已审核 有效公告 内部或者全部公告
        if (mCompany.RoleType == 1)//平台管理员
        {
           // sbSQLwhere.Append(" and (IsInternal=1 or IsInternal=3) ");
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
        List<Bd_Base_Notice> notice = baseDataManage.CallMethod("Bd_Base_Notice", "GetBasePager1", outParams, new object[] { TotalCount, 10, 1, "*", sbSQLwhere.ToString(), " Emergency " }) as List<Bd_Base_Notice>;
        Repeater.DataSource = notice;
        Repeater.DataBind();
    }

    /// <summary>
    /// 页面绑定数据显示
    /// </summary>
    /// <returns></returns>
    public string ShowText(int type, params Object[] obj)
    {
        string result = "";
        if (type == 0)//紧急公告颜色
        {
            if (obj != null && obj.Length == 3)
            {
                if (obj[0].ToString() == "1")//紧急公告
                {
                    result = "<a href=\"LookBulletin.aspx?id=" + obj[2].ToString() + "&currentuserid="+this.currentuserid.Value.ToString()+"\"><font class=\"red\">" + obj[1].ToString() + "</font></a>";
                }
                else
                {
                    result = "<a href=\"LookBulletin.aspx?id=" + obj[2].ToString() + "&currentuserid=" + this.currentuserid.Value.ToString() + "\">" + obj[1].ToString() + "</a>";
                }
            }
        }
        else if (type == 1)//显示文件名
        {
            if (obj != null && obj.Length == 2)
            {
                if (obj[0] != null && obj[0].ToString() != "")
                {
                    result = "<a href='DownLoadFile.aspx?did=" + obj[1].ToString() + "&currentuserid=" + this.currentuserid.Value.ToString() + "'>" + obj[0].ToString().Split('|')[0] + "</a>";
                }
            }
        }
        return result;
    }
}
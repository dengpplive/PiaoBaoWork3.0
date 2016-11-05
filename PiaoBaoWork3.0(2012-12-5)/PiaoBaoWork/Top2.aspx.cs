using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PbProject.Model;
using DataBase.Data;
/// <summary>
/// 显示数据
/// </summary>
public partial class Top2 : BasePage
{
    /// <summary>
    /// 登录用户名
    /// </summary>
    public string userName = "";

    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsPostBack)
            {
                userName = mUser.UserName;
                GetScrollNotice();
            }
        }
        catch (Exception)
        {
        }
    }
    /// <summary>
    /// 获取滚动公告
    /// </summary>
    public void GetScrollNotice()
    {
        StringBuilder sbSQLwhere = new StringBuilder();
        //获取已审核 有效公告 内部或者全部公告 且滚动
        sbSQLwhere.AppendFormat(" (ReleaseCpyNo=left('{0}',12) or ReleaseCpyNo=left('{0}',6)) and CallboardType=1 and Rollflag=2  ", mCompany.UninCode);
        if (mCompany.RoleType == 1)//平台管理员
        {
            sbSQLwhere.Append(" and (IsInternal=1 or IsInternal=3) ");
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
        if (notice.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < notice.Count; i++)
            {
                sb.Append("<font color='red'>·</font><a target='_blank' href='Manager/Sys/LookBulletin.aspx?id=" + notice[i].id + "&currentuserid="+this.mUser.id+"'>" + notice[i].Title + "</a>&nbsp;&nbsp;&nbsp;&nbsp;");
            }
            divGG.InnerHtml = sb.ToString();
        }
        else
        {
            divGG.InnerHtml = "";
        }
    }
}
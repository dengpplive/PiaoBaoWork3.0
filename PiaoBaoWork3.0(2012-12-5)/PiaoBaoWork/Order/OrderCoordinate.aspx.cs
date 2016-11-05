using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;
using PbProject.WebCommon.Utility;

public partial class Order_OrderCoordinate : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (!string.IsNullOrEmpty(Request["orderid"]))
            {
                DataBind(Server.UrlDecode(Request["orderid"].ToString()));
            }
        }
    }
    /// <summary>
    /// 绑定数据
    /// </summary>
    /// <param name="orderid"></param>
    private void DataBind(string orderid)
    {
        string sqlAirOrderWhere = " OrderId='" + orderid + "'";
        if (mCompany.RoleType == 1)
            sqlAirOrderWhere += " and WatchType in(0,1,2,3,4,5)";
        else
            sqlAirOrderWhere += " and WatchType in(2,3,4,5)";
        sqlAirOrderWhere += " order by OperTime ";

        List<Log_Tb_AirOrder> AirOrderList = baseDataManage.CallMethod("Log_Tb_AirOrder", "GetList", null, new Object[] { sqlAirOrderWhere }) as List<Log_Tb_AirOrder>;

        if (AirOrderList != null && AirOrderList.Count > 0)
        {
            RepOrderLog.DataSource = AirOrderList;
            RepOrderLog.DataBind();
        }
    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btsave_Click(object sender, EventArgs e)
    {
        try
        {
            IHashObject parameter = new HashObject();
            parameter.Add("OrderId",Request["orderid"].ToString());
            parameter.Add("OperType", "协调");
            parameter.Add("OperTime", DateTime.Now);
            parameter.Add("OperLoginName", mUser.LoginName);
            parameter.Add("OperUserName", mUser.UserName);
            parameter.Add("CpyNo", mUser.CpyNo);
            parameter.Add("CpyType", mCompany.RoleType);
            parameter.Add("CpyName", mCompany.UninAllName);
            parameter.Add("OperContent", CommonManage.TrimSQL(ddltype.SelectedValue + "|" + txtRemark.Text.Trim()).TrimEnd('|'));
            parameter.Add("WatchType", mCompany.RoleType == 1 ? "1" : "2");//判断登录用户是否是平台（只有平台和运营可以进此页面）
           
            if ((bool)baseDataManage.CallMethod("Log_Tb_AirOrder", "Insert", null, new Object[] { parameter }) == true)
            {
                DataBind(Request["orderid"].ToString());
                Log_Operation logoper = new Log_Operation();
                logoper.ModuleName = "协调";
                logoper.LoginName = mUser.LoginName;
                logoper.UserName = mUser.UserName;
                logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
                logoper.CpyNo = mCompany.UninCode;
                logoper.OperateType = ddltype.SelectedValue;
                logoper.OptContent = CommonManage.TrimSQL(ddltype.SelectedValue + "|" + txtRemark.Text.Trim()).TrimEnd('|');
                new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);//添加日志
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('失败');", true);
            }
           
        }
        catch (Exception)
        {
            
            throw;
        }
       
        
    }
}

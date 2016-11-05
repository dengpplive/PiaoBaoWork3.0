using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;

public partial class Policy_StrategyGroupDetailEdit : BasePage
{
    public Tb_Ticket_TakeOffDetail groupdetail = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnBack.PostBackUrl = string.Format("StrategyGroupDetailList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            BindGroup();
            if (Request["id"] != null)
            {
                ViewState["Id"] = Request["id"];
                GetGroupDetailinfo();
            }
            else
            {
                txtStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtEndTime.Text = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            }
            lbsave.Attributes.Add("onclick", "return showAllErr();");
        }
    }
    /// <summary>
    /// 绑定分组
    /// </summary>
    protected void BindGroup()
    {
        List<Tb_Ticket_StrategyGroup> GroupList = baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "GetList", null, new object[] { "CpyNo='"+mCompany.UninCode+"'" }) as List<Tb_Ticket_StrategyGroup>;
        this.ddlGroup.DataSource = GroupList;
        ddlGroup.DataTextField = "GroupName";
        ddlGroup.DataValueField = "id";
        ddlGroup.DataBind();

        List<Bd_Base_Dictionary> list = baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new Object[] { "ParentID=24" }) as List<Bd_Base_Dictionary>;
        this.ddljk.DataSource = list;
        ddljk.DataTextField = "ChildName";
        ddljk.DataValueField = "ChildID";
        this.ddljk.DataBind();

    }
    /// <summary>
    /// 获取扣点组详情
    /// </summary>
    protected void GetGroupDetailinfo()
    {
        groupdetail = baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "GetById", null, new object[] { ViewState["Id"].ToString() }) as Tb_Ticket_TakeOffDetail;
        ddlGroup.SelectedValue = groupdetail.GroupId.ToString();
        ddlbasetype.SelectedValue = groupdetail.BaseType.ToString();
        txtCarryCode.Text = groupdetail.CarryCode;
        txtFromCityCode.Text = groupdetail.FromCityCode;
        rblSelectType.SelectedValue = groupdetail.SelectType.ToString();
        txtToCityCode.Text = groupdetail.ToCityCode;
        txtPoint.Text = groupdetail.Point.ToString();
        txtMoney.Text = groupdetail.Money.ToString();
        txtPointScope1.Text = groupdetail.PointScope.Split('|')[0].ToString();
        txtPointScope2.Text = groupdetail.PointScope.Split('|')[1].ToString();
        txtStartTime.Text = groupdetail.TimeScope.Split('|')[0].ToString();
        txtEndTime.Text = groupdetail.TimeScope.Split('|')[1].ToString();

        ddljk.SelectedValue = groupdetail.PolicySource.ToString();
        showjk.Visible = ddlbasetype.SelectedValue == "2" ? true : false;
    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbsave_Click(object sender, EventArgs e)
    {
        string msg = "";
        IHashObject parameter = new HashObject();
        try
        {
            parameter.Add("GroupId", ddlGroup.SelectedValue);
            parameter.Add("BaseType", ddlbasetype.SelectedValue);
            parameter.Add("PolicySource", ddljk.SelectedValue);
            parameter.Add("CarryCode", txtCarryCode.Text.ToUpper().Trim());
            parameter.Add("FromCityCode", txtFromCityCode.Text.ToUpper().Trim());
            parameter.Add("ToCityCode", txtToCityCode.Text.ToUpper().Trim());
            parameter.Add("TimeScope", txtStartTime.Text.Trim() + "|" + txtEndTime.Text.Trim());
            parameter.Add("PointScope", txtPointScope1.Text.Trim() + "|" + txtPointScope2.Text.Trim());
            parameter.Add("SelectType", rblSelectType.SelectedValue);
            parameter.Add("Point", txtPoint.Text.Trim());
            parameter.Add("Money", txtMoney.Text.Trim());
            if (ViewState["Id"] != null)
            {
                #region 更新
                parameter.Add("id", ViewState["Id"].ToString());
                if ((bool)baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "Update", null, new object[] { parameter }) == true)
                {
                    msg = "更新成功";
                }
                else
                {
                    msg = "更新失败";
                }
                #endregion
            }
            else
            {
                #region 添加
                parameter.Add("CpyNo", mCompany.UninCode);
                parameter.Add("CpyName", mCompany.UninAllName);
                parameter.Add("CpyType", mCompany.RoleType);
                parameter.Add("OperTime", DateTime.Now);
                parameter.Add("OperLoginName", mUser.LoginName);
                parameter.Add("OperUserName", mUser.UserName);
                msg = (bool)baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "Insert", null, new Object[] { parameter }) == true ? "添加成功" : "添加失败";
                #endregion
            }
        }
        catch (Exception)
        {

            throw;
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
    protected void ddlbasetype_SelectedIndexChanged(object sender, EventArgs e)
    {
        showjk.Visible = ddlbasetype.SelectedValue == "2" ? true : false;
        ddljk.SelectedValue = "";
    }
}
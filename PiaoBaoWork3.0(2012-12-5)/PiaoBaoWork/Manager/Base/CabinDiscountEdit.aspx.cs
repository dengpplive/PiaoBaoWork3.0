using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using PbProject.Dal.ControlBase;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using PbProject.Logic.ControlBase;
/// <summary>
/// 仓位管理
/// </summary>
public partial class Sys_CabinDiscountEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            if (Request.QueryString["Id"] != null)
            {
                ViewState["Id"] = Request.QueryString["Id"];
                CabnInDiscountInfoBind();
            }
            else
            {
                txtBeginTime.Value = System.DateTime.Now.ToString("yyyy-MM-dd");
                txtEndTime.Value = System.DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            }
            LinkButton1.Attributes.Add("onclick", "return showAllErr();");
        }
    }
    private void CabnInDiscountInfoBind()
    {
        //获取实体
        Bd_Air_CabinDiscount cd = baseDataManage.CallMethod("Bd_Air_CabinDiscount", "GetById", null, new object[] { ViewState["Id"].ToString() }) as Bd_Air_CabinDiscount;
        txtCabin.Text = cd.Cabin;

        txtFormCity.Text = cd.FromCity;
        txtFromCityCode.Text = cd.FromCityCode;
        txtToCity.Text = cd.ToCity;
        txtToCityCode.Text = cd.ToCityCode;
        txtBeginTime.Value = cd.BeginTime.ToString().Split(' ')[0];
        txtEndTime.Value = cd.EndTime.ToString().Split(' ')[0];
        txtAirPortCode.Text = cd.AirCode;
        txtAirPortName.Text = cd.AirName;
        txtDiscountRate.Text = cd.CabinPrice.ToString();
        if (cd.IsGN == 0)
        {
            ddlIsGN.SelectedIndex = 0;
        }
        if (cd.IsGN == 1)
        {
            ddlIsGN.SelectedIndex = 1;
        }
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Bd_Air_CabinDiscount cd = null;
        if (Request.QueryString["Id"] != null && Request.QueryString["Id"].ToString() != "")
        {
            //获取实体
            cd = baseDataManage.CallMethod("Bd_Air_CabinDiscount", "GetById", null, new object[] { ViewState["Id"].ToString() }) as Bd_Air_CabinDiscount;
        }
        else
        {
            cd = new Bd_Air_CabinDiscount();
        }
        if (Convert.ToDateTime(txtBeginTime.Value).CompareTo(Convert.ToDateTime(txtEndTime.Value)) >= 0)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('起始时间不能大于或等于终止时间！');", true);
            return;
        }
        decimal num = 0m;
        if (!decimal.TryParse(txtDiscountRate.Text.ToString(), out num))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('舱位价格只能为数字！');", true);
            return;
        }
        cd.Cabin = txtCabin.Text.ToUpper().Trim().Replace("'", "");
        cd.FromCity = txtFormCity.Text.Trim().Replace("'", "");
        cd.FromCityCode = txtFromCityCode.Text.ToUpper().Trim().Replace("'", "");
        cd.ToCity = txtToCity.Text.Trim().Replace("'", "");
        cd.ToCityCode = txtToCityCode.Text.ToUpper().Trim().Replace("'", "");
        cd.BeginTime = DateTime.Parse(txtBeginTime.Value.Replace("'", ""));
        cd.EndTime = DateTime.Parse(txtEndTime.Value.Replace("'", ""));
        cd.IsGN = int.Parse(ddlIsGN.SelectedValue.Replace("'", ""));
        cd.AddDate = DateTime.Now;
        cd.AirCode = txtAirPortCode.Text.Trim().Replace("'", "");
        cd.AirName = txtAirPortName.Text.Trim().Replace("'", "");
        cd.CabinPrice = decimal.Parse(txtDiscountRate.Text.ToString());
        if (Request.QueryString["Id"] != null && Request.QueryString["Id"].ToString() != "")
        {
            //修改
            bool UpdateSuc = (bool)baseDataManage.CallMethod("Bd_Air_CabinDiscount", "Update", null, new object[] { cd });
            if (UpdateSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('保存成功！');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('保存失败！');", true);
            }
        }
        else
        {
            //添加
            bool InsertSuc = (bool)baseDataManage.CallMethod("Bd_Air_CabinDiscount", "Insert", null, new object[] { cd });
            if (InsertSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('添加成功！');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('添加失败！');", true);
            }
        }
    }
    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("CabinDiscountList.aspx?currentuserid=" + this.currentuserid.Value.ToString());

    }
}
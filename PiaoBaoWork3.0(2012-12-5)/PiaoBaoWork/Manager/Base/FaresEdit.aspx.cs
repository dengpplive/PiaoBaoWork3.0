using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Dal.ControlBase;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using PbProject.Logic.ControlBase;
public partial class Sys_FaresEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["Id"] != null)
            {
                ViewState["Id"] = Request.QueryString["Id"];
                FaresInfoBind();
            }
            LinkButton1.Attributes.Add("onclick", "return showAllErr();");
            txtEffectTime.Text = System.DateTime.Now.ToString("yyyy-MM-dd");
            txtInvalidTime.Text = System.DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
        }
    }
    private void FaresInfoBind()
    {
        //Bd_Air_Fares fares = Manage.GetById(ViewState["Id"].ToString());
        Bd_Air_Fares fares = baseDataManage.CallMethod("Bd_Air_Fares", "GetById", null, new object[] { ViewState["Id"].ToString() }) as Bd_Air_Fares;
        txtFromCityName.Text = fares.FromCityName;
        txtFromCityCode.Text = fares.FromCityCode;
        txtToCityName.Text = fares.ToCityName;
        txtToCityCode.Text = fares.ToCityCode;
        txtFareFee.Text = fares.FareFee.ToString();
        txtMileage.Text = fares.Mileage.ToString();
        txtCarryCode.Text = fares.CarryCode;
        txtEffectTime.Text = fares.EffectTime.ToString().Split(' ')[0];
        txtInvalidTime.Text = fares.InvalidTime.ToString().Split(' ')[0];
        if (fares.IsDomestic == 1)
        {
            ddlIsDomestic.SelectedIndex = 0;
        }
        else
        {
            ddlIsDomestic.SelectedIndex = 1;
        }
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Bd_Air_Fares fares = null;
        if (Request.QueryString["Id"] != null && Request.QueryString["Id"].ToString() != "")
        {
            fares = baseDataManage.CallMethod("Bd_Air_Fares", "GetById", null, new object[] { ViewState["Id"].ToString() }) as Bd_Air_Fares;
        }
        else
        {
            fares = new Bd_Air_Fares();
        }
        fares.FromCityName = txtFromCityName.Text.Trim().Replace("'", "");
        fares.FromCityCode = txtFromCityCode.Text.Trim().Replace("'", "");
        fares.ToCityName = txtToCityName.Text.Trim().Replace("'", "");
        fares.ToCityCode = txtToCityCode.Text.Trim().Replace("'", "");
        fares.FareFee = decimal.Parse(txtFareFee.Text.Trim());
        fares.Mileage = int.Parse(txtMileage.Text.Trim());
        fares.CarryCode = txtCarryCode.Text.Trim().Replace("'", "");
        fares.EffectTime = DateTime.Parse(txtEffectTime.Text.Trim());
        fares.InvalidTime = DateTime.Parse(txtInvalidTime.Text.Trim());
        if (ddlIsDomestic.SelectedIndex == 0)
        {
            fares.IsDomestic = 1;
        }
        else
        {
            fares.IsDomestic = 2;
        }
        if (Request.QueryString["Id"] != null && Request.QueryString["Id"].ToString() != "")
        {
            //if (Manage.Update(fares))
            bool UpdateSuc = (bool)baseDataManage.CallMethod("Bd_Air_Fares", "Update", null, new object[] { fares });
            if (UpdateSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid ().ToString(), "showdialog3('保存成功！');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('保存失败！');", true);
            }
        }
        else
        {
            bool InsertSuc = (bool)baseDataManage.CallMethod("Bd_Air_Fares", "Insert", null, new object[] { fares });
            //if (Manage.Insert(fares))
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
}
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
public partial class Sys_CityEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            if (Request.QueryString["Id"] != null)
            {
                ViewState["Id"] = Request.QueryString["Id"];
                CityInfoBind();
            }
            LinkButton1.Attributes.Add("onclick", "return showAllErr();");
        }

    }
    private void CityInfoBind()
    {
        Bd_Air_AirPort city = baseDataManage.CallMethod("Bd_Air_AirPort", "GetById", null, new object[] { ViewState["Id"].ToString() }) as Bd_Air_AirPort;
        txtCity.Text = city.CityName;
        txtQuanPing.Text = city.CityQuanPin;
        txtJianPin.Text = city.CityJianPin;
        txtCityCode.Text = city.CityCodeWord;
        txtCountry.Text = city.Country;
        txtContinents.Text = city.Continents;
        txtAirPortName.Text = city.AirPortName;
        if (city.IsDomestic == 1)
        {
            ddlType.SelectedIndex = 0;
        }
        else
        {
            ddlType.SelectedIndex = 1;
        }
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        try
        {
            Bd_Air_AirPort city = null;
            if (Request.QueryString["Id"] != null && Request.QueryString["Id"].ToString() != "")
            {
                //获取实体          
                city = baseDataManage.CallMethod("Bd_Air_AirPort", "GetById", null, new object[] { ViewState["Id"].ToString() }) as Bd_Air_AirPort;
            }
            else
            {
                city = new Bd_Air_AirPort();
            }
            Regex regex = new Regex("^[a-zA-Z]+$");
            Regex regex1 = new Regex("^[\u4E00-\u9FA5]+$");
            bool IsOk = true;
            bool IsOk1 = true;
            if (!regex.IsMatch(txtQuanPing.Text))
            {
                IsOk = false;
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('城市全拼必须为字母！');", true);

            }
            if (!regex1.IsMatch(txtCity.Text))
            {
                IsOk1 = false;
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('城市中文必须为汉字！');", true);

            }
            if (IsOk && IsOk1)
            {
                city.CityName = txtCity.Text.Trim().Replace("'", "");
                city.CityQuanPin = txtQuanPing.Text.Trim().Replace("'", "").ToUpper();
                city.CityJianPin = txtJianPin.Text.Trim().Replace("'", "").ToUpper();
                city.CityCodeWord = txtCityCode.Text.Trim().Replace("'", "").ToUpper();
                city.Continents = txtContinents.Text.Trim().Replace("'", "");
                city.Country = txtCountry.Text.Trim().Replace("'", "");
                city.IsDomestic = int.Parse(ddlType.SelectedValue);
                if (Request.QueryString["Id"] != null)
                {
                    //修改
                    bool UpdateSuc = (bool)baseDataManage.CallMethod("Bd_Air_AirPort", "Update", null, new object[] { city });
                    if (UpdateSuc)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog3('保存成功！');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('保存失败,此三字码已存在！');", true);
                    }
                }
                else
                {
                    string SqlWhere = string.Format(" CityName='{0}' and CityCodeWord='{1}' ", city.CityName, city.CityCodeWord);
                    bool IsExist = (bool)baseDataManage.CallMethod("Bd_Air_AirPort", "IsExist", null, new object[] { SqlWhere });
                    if (!IsExist)
                    {
                        //添加
                        bool InsertSuc = (bool)baseDataManage.CallMethod("Bd_Air_AirPort", "Insert", null, new object[] { city });
                        if (InsertSuc)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog3('添加成功！');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('添加失败,此三字码已存在！');", true);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('添加失败,此三字码已存在！');", true);
                    }
                }
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('三字码已存在！');", true);
        }

    }
}
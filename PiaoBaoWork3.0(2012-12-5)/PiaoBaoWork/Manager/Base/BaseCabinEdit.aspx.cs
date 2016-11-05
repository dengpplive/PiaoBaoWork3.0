using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;
using PbProject.WebCommon.Utility;

public partial class Sys_BaseCabinEdit : BasePage
{
    public Bd_Air_BaseCabin mbc = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnBack.PostBackUrl = string.Format("BaseCabinList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            if (Request.QueryString["Id"] != null)
            {
                ViewState["Id"] = Request.QueryString["Id"];
                GetBaseCabinById();
            }
            else
            {
                txtBeginTime.Value = System.DateTime.Now.ToString("yyyy-MM-dd");
                txtEndTime.Value = System.DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            }
            lbsave.Attributes.Add("onclick", "return showAllErr();");
        }
    }
    /// <summary>
    /// 获取要修改的舱位
    /// </summary>
    protected void GetBaseCabinById()
    {
        mbc = baseDataManage.CallMethod("Bd_Air_BaseCabin", "GetById", null, new object[] { ViewState["Id"].ToString() }) as Bd_Air_BaseCabin;
        if (mbc != null)
        {
            txtCabin.Text = mbc.Cabin;
            txtCabinName.Text = mbc.CabinName;
            txtAirCode.Text = mbc.AirCode;
            txtAirSPortName.Text = mbc.AirShortName;
            txtBeginTime.Value = mbc.StartTime.ToString("yyyy-MM-dd");
            txtEndTime.Value = mbc.EndTime.ToString("yyyy-MM-dd");
            txtDiscountRate.Text = mbc.Rebate.ToString();
        }
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
            if (mCompany.RoleType != 1)
            {
                msg = "此用户无权限操作";
            }
            else
            {
                if (Convert.ToDateTime(txtBeginTime.Value) > Convert.ToDateTime(txtEndTime.Value))
                {
                    msg = "生效日期不能大于失效日期";
                }
                else
                {
                    parameter.Add("Cabin", CommonManage.TrimSQL(txtCabin.Text.Trim()));
                    parameter.Add("CabinName", CommonManage.TrimSQL(txtCabinName.Text.Trim()));
                    parameter.Add("AirCode", CommonManage.TrimSQL(txtAirCode.Text.Trim()));
                    parameter.Add("AirShortName", CommonManage.TrimSQL(txtAirSPortName.Text.Trim()));
                    parameter.Add("StartTime", Convert.ToDateTime(txtBeginTime.Value));
                    parameter.Add("EndTime", Convert.ToDateTime(CommonManage.TrimSQL(txtEndTime.Value.Trim())));
                    parameter.Add("Rebate", decimal.Parse(txtDiscountRate.Text));
                    if (Request.QueryString["Id"] != null && Request.QueryString["Id"].ToString().Length != 0)
                    {
                        #region 修改
                        parameter.Add("id", Guid.Parse(Request["Id"]));
                        msg = (bool)baseDataManage.CallMethod("Bd_Air_BaseCabin", "Update", null, new object[] { parameter }) == true ? "更新成功" : "更新失败";
                        #endregion
                    }
                    else
                    {
                        #region 添加
                        parameter.Add("AddTime", Convert.ToDateTime(DateTime.Now));
                        msg = (bool)baseDataManage.CallMethod("Bd_Air_BaseCabin", "Insert", null, new Object[] { parameter }) == true ? "添加成功" : "添加失败";
                        #endregion
                    }
                }

            }

        }
        catch (Exception)
        {
            msg = "操作异常";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
}
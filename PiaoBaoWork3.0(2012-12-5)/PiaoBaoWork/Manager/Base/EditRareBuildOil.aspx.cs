using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;

/// <summary>
/// 添加或者修改
/// </summary>
public partial class Manager_Base_EditRareBuildOil : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Hidd_uid.Value = Request["currentuserid"].ToString();
            if (Request["id"] != null && Request["id"].ToString() != "")
            {
                Id = Request["id"].ToString();
                Bind();
            }
        }
    }

    protected string Id
    {
        get { return (string)ViewState["id"]; }
        set { ViewState["id"] = value; }
    }
    /// <summary>
    /// 添加或者编辑
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string strFromCode = txtFromCityCode.Text.Trim();
        string strToCode = txtToCityCode.Text.Trim();
        string strPersonType = ddlPasType.SelectedValue;
        string strRQFare = txtRQFare.Text.Trim();
        string txtTax = txtTAX.Text.Trim();
        string msg = "";
        decimal m_RQFare = 0m, m_TAX = 0m;
        try
        {
            if (strFromCode.Trim().Length != 3)
            {
                msg = "出发城市三字码输入错误";
            }
            else if (strToCode.Trim().Length != 3)
            {
                msg = "到达城市三字码输入错误";
            }
            if (!decimal.TryParse(strRQFare, out m_RQFare))
            {
                msg = "输入机建费格式错误,必须为数字";
            }
            if (!decimal.TryParse(txtTax, out m_TAX))
            {
                msg = "输入燃油费格式错误,必须为数字";
            }
            if (msg == "")
            {
                Tb_Air_BuildOilInfo tb_air_buildoilinfo = new Tb_Air_BuildOilInfo();
                if (Request["id"] != null && Request["id"].ToString() != "")
                {
                    tb_air_buildoilinfo = this.baseDataManage.CallMethod("Tb_Air_BuildOilInfo", "GetById", null, new object[] { Id }) as Tb_Air_BuildOilInfo;
                }
                else
                {
                    string sqlWhere = string.Format(" FromCityCode='{0}' and ToCityCode='{1}' and PersonType={2}  ", strFromCode, strToCode, strPersonType);
                    bool IsExist = (bool)this.baseDataManage.CallMethod("Tb_Air_BuildOilInfo", "IsExist", null, new object[] { sqlWhere });
                    if (IsExist)
                    {
                        msg = "该航线已存在，请修改！";
                    }
                    else
                    {
                        tb_air_buildoilinfo = new Tb_Air_BuildOilInfo();
                    }
                }
                tb_air_buildoilinfo.FromCityCode = strFromCode;
                tb_air_buildoilinfo.ToCityCode = strToCode;
                tb_air_buildoilinfo.PersonType = int.Parse(ddlPasType.SelectedValue);
                tb_air_buildoilinfo.OilPrice = m_RQFare;
                tb_air_buildoilinfo.BuildPrice = m_TAX;
                if (msg == "")
                {
                    if (Request["id"] != null && Request["id"].ToString() != "")
                    {
                        //修改
                        msg = (bool)baseDataManage.CallMethod("Tb_Air_BuildOilInfo", "Update", null, new object[] { tb_air_buildoilinfo }) == true ? "更新成功" : "更新失败";
                    }
                    else
                    {
                        //添加
                        msg = (bool)baseDataManage.CallMethod("Tb_Air_BuildOilInfo", "Insert", null, new Object[] { tb_air_buildoilinfo }) == true ? "添加成功" : "添加失败";
                    }
                }
            }
        }
        catch (Exception)
        {
            msg = "操作异常！";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('" + msg + "');", true);
    }
    /// <summary>
    /// 绑定数据
    /// </summary>
    public void Bind()
    {
        Tb_Air_BuildOilInfo tb_air_buildoilinfo = this.baseDataManage.CallMethod("Tb_Air_BuildOilInfo", "GetById", null, new object[] { Id }) as Tb_Air_BuildOilInfo;
        if (tb_air_buildoilinfo != null)
        {
            txtFromCityCode.Text = tb_air_buildoilinfo.FromCityCode;
            txtToCityCode.Text = tb_air_buildoilinfo.ToCityCode;
            ddlPasType.SelectedValue = tb_air_buildoilinfo.PersonType.ToString();
            txtTAX.Text = tb_air_buildoilinfo.BuildPrice.ToString();
            txtRQFare.Text = tb_air_buildoilinfo.OilPrice.ToString();
        }
    }
    /// <summary>
    /// 返回
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnGoBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("RareBuildOilList.aspx?currentuserid=" + Hidd_uid.Value);
    }
}
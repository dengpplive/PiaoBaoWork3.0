using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;
using System.Text.RegularExpressions;

public partial class Manager_Base_SpecialCabinEdit : BasePage
{
    public Tb_SpecialCabin mspbc = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.btnBack.PostBackUrl = string.Format("SpecialCabinList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            if (Request.QueryString["Id"] != null)
            {
                ViewState["Id"] = Request.QueryString["Id"];
                GetBaseCabinById();
            }
            else
            {
                this.txtAirCode.ReadOnly = false;
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
        mspbc = baseDataManage.CallMethod("Tb_SpecialCabin", "GetById", null, new object[] { ViewState["Id"].ToString() }) as Tb_SpecialCabin;
        if (mspbc != null)
        {
            txtCabin.Text = mspbc.SpCabin;
            txtAirCode.Text = mspbc.SpAirCode;
            txtAirSPortName.Text = mspbc.SpAirShortName;
            txtBeginTime.Value = mspbc.SpStartTime.ToString("yyyy-MM-dd");
            txtEndTime.Value = mspbc.SpEndTime.ToString("yyyy-MM-dd");
          
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

            //判断用户类型
            if (mCompany.RoleType != 2 && mCompany.RoleType != 3)
            {
                msg = "此用户无权限操作";
            }
            else
            {
                string cabinPattern = "^[a-zA-Z]|([a-zA-Z]1)$";
                string[] SpCab = txtCabin.Text.Trim().ToUpper().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in SpCab)
                {
                    if (!Regex.IsMatch(item, cabinPattern))
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('仓位格式错误！');", true);
                        return;
                    }
                }
                if (Convert.ToDateTime(txtBeginTime.Value) > Convert.ToDateTime(txtEndTime.Value))
                {
                    msg = "生效日期不能大于失效日期";
                }
                else
                {
                    parameter.Add("SpCabin", txtCabin.Text.Trim());
                    parameter.Add("SpAirCode", txtAirCode.Text.Trim().ToUpper());
                    parameter.Add("SpAirShortName", txtAirSPortName.Text);
                    parameter.Add("SpStartTime", Convert.ToDateTime(txtBeginTime.Value));
                    parameter.Add("SpEndTime", Convert.ToDateTime(txtEndTime.Value));
                    parameter.Add("CpyNo", mCompany.UninCode);
                    if (Request.QueryString["Id"] != null && Request.QueryString["Id"].ToString().Length != 0)
                    {
                        #region 修改
                        parameter.Add("id", Guid.Parse(Request["Id"]));
                        msg = (bool)baseDataManage.CallMethod("Tb_SpecialCabin", "Update", null, new object[] { parameter }) == true ? "更新成功" : "更新失败";
                        #endregion
                    }
                    else
                    {
                        #region 添加
                        parameter.Add("SpAddTime", Convert.ToDateTime(DateTime.Now));
                        List<Tb_SpecialCabin> list = baseDataManage.CallMethod("Tb_SpecialCabin", "GetList", null, new Object[] { "CpyNo='"+mCompany.UninCode+"' and SpAirCode='" + txtAirCode.Text.Trim().ToUpper() + "'" }) as List<Tb_SpecialCabin>;
                        if (list!=null&&list.Count>0)
                        {
                            msg = "该承运人已存在";
                        }
                        else
                        {
                            msg = (bool)baseDataManage.CallMethod("Tb_SpecialCabin", "Insert", null, new Object[] { parameter }) == true ? "添加成功" : "添加失败";
                        }
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
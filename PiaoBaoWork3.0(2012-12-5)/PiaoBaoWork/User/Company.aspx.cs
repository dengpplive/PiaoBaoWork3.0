using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.IO;
using Img = System.Drawing.Image;
using System.Drawing;
using System.Collections;
using System.Data.SqlClient;
using DataBase.Data;
using PbProject.Model;
using PbProject.WebCommon.Utility;
using PbProject.Logic;

/// <summary>
/// 公司信息维护
/// </summary>
public partial class Company : BasePage
{
    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            trtime.Visible = (mCompany.RoleType == 2 || mCompany.RoleType == 3) ? true : false;
            string cpyno = "";
            if (!string.IsNullOrEmpty(Request["cpyno"]))//平台可设置
            {
                cpyno = Request["cpyno"].ToString();
            }
            else
            {
                if (mUser.IsAdmin != 0)
                {
                    LinkButton1.Visible = false;
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('需管理员权限才能修改！');", true);
                }
                else
                {
                    cpyno = mCompany.UninCode;
                }

            }
            ViewState["cpyno"] = cpyno;
            //订单提醒
            tr_HeadPrompt.Visible = mCompany.RoleType < 3 ? true : false;
            tr_Prompt.Visible = mCompany.RoleType < 3 ? true : false;
            bind(cpyno);
            txtUnitName.Enabled = false;
        }
    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        try
        {
            string msg = "";

            List<User_Company> listcpywebsite = null;
            if (!string.IsNullOrEmpty(txtWebSite.Text.Trim()))
            {
                listcpywebsite = new PbProject.Logic.User.User_CompanyBLL().GetListBySqlWhere("WebSite like '%" + CommonManage.TrimSQL(txtWebSite.Text) + "%' and UninCode <> '" + ViewState["cpyno"].ToString() + "'");
            }
            if (listcpywebsite != null && listcpywebsite.Count > 0)
            {
                msg = "该商家所填网址已存在";
            }
            else
            {
                string worktime = ddlworkHtime.SelectedValue + ":" + ddlworkMtime.SelectedValue + "-" + ddlafterworkHtime.SelectedValue + ":" + ddlafterworkMtime.SelectedValue;
                string BusinessTime = ddlBusinessHstartTime.SelectedValue + ":" + ddlBusinessMstartTime.SelectedValue + "-" + ddlBusinessHendTime.SelectedValue + ":" + ddlBusinessMendTime.SelectedValue;
                string isdl = cksetdlfx.Checked == true ? "1" : "0";
                string isshowdl = "";
                if (isdl == "0")//若不是独立分销显示自己信息也为0
                {
                    isshowdl = "0";
                }
                else
                {
                    isshowdl = ckshowdlinfo.Checked == true ? "1" : "0";
                }
                string parametersql = "";
                parametersql += GetParameterUpSql(CommonManage.TrimSQL(Hid_KefuValue.Value.Trim('@')), ViewState["cpyno"].ToString(), PbProject.Model.definitionParam.paramsName.cssQQ);
                parametersql += GetParameterUpSql(isdl, ViewState["cpyno"].ToString(), PbProject.Model.definitionParam.paramsName.isDuLiFenXiao);
                parametersql += GetParameterUpSql(isshowdl, ViewState["cpyno"].ToString(), PbProject.Model.definitionParam.paramsName.isShowDuLiInfo);
                IHashObject paramter = new HashObject();
                paramter.Add("User_Employees", " ");
                paramter.Add("User_Permissions", " ");
                paramter.Add("User_Company", "update User_Company set ContactUser='" + CommonManage.TrimSQL(txtLXR.Text.Trim()) + "',ContactTel='" + CommonManage.TrimSQL(txtLXTel.Text.Trim()) + "',Tel='" + CommonManage.TrimSQL(txtBanGongTel.Text.Trim()) + "',Fax='" + CommonManage.TrimSQL(txtFax.Text.Trim()) + "',Provice='" + Request.Form["province"] + "',City='" + Request.Form["city"] + "',UninAddress='" + CommonManage.TrimSQL(txtUnitAddr.Text.Trim()) + "',Email='" + CommonManage.TrimSQL(txtEmail.Text.Trim()) + "',WebSite='" + CommonManage.TrimSQL(txtWebSite.Text.Trim()) + "',WorkTime='" + worktime + "',BusinessTime='" + BusinessTime + "',IsEmpPrompt='" + (cbkEmpPrompt.Checked ? 1 : 0) + "',IsPrompt='" + (cbkPrompt.Checked ? 1 : 0) + "',PromptTime=" + ddlPromptTime.SelectedValue + " where UninCode='" + ViewState["cpyno"].ToString() + "'");
                paramter.Add("Bd_Base_Parameters", parametersql);
                msg = (new PbProject.Logic.User.User_CompanyBLL().uporinAccount(paramter, 1) > 0) ? "修改成功!" : "修改失败!";

                #region 更新应用程序池
                string UId = mUser.id.ToString();
                SessionContent sessionContent = HttpContext.Current.Application[UId] as SessionContent;
                if (mCompany != null && sessionContent != null)
                {
                    if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
                    {
                        string strwhere = "1=1 and unincode='" + sessionContent.parentCpyno + "'";
                        List<User_Company> Sup_Company = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { strwhere }) as List<User_Company>;
                        if (Sup_Company != null && Sup_Company.Count > 0)
                        {
                            Application[sessionContent.parentCpyno + "Company"] = Sup_Company[0];
                            sessionContent.COMPANY = Sup_Company[0];
                        }
                        strwhere = "1=1 and cpyno='" + sessionContent.parentCpyno + "'";
                        List<Bd_Base_Parameters> SupParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { strwhere }) as List<Bd_Base_Parameters>;
                        if (SupParameters != null)
                        {
                            Application[sessionContent.parentCpyno + "Parameters"] = SupParameters;
                            sessionContent.BASEPARAMETERS = SupParameters;
                        }
                    }
                    else if (mCompany.RoleType == 4 || mCompany.RoleType == 5)
                    {
                        string strwhere = "1=1 and unincode='" + ViewState["cpyno"].ToString() + "'";
                        List<User_Company> Curr_Company = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { strwhere }) as List<User_Company>;
                        if (Curr_Company != null && Curr_Company.Count > 0)
                        {
                            sessionContent.COMPANY = Curr_Company[0];
                        }
                        strwhere = "1=1 and cpyno='" + ViewState["cpyno"].ToString() + "'";
                        List<Bd_Base_Parameters> Curr_Parameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { strwhere }) as List<Bd_Base_Parameters>;
                        if (Curr_Parameters != null)
                        {
                            sessionContent.BASEPARAMETERS = Curr_Parameters;
                        }
                    }
                    //重新保存会到IIS应用程序池中
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[UId] = sessionContent;
                    HttpContext.Current.Application.UnLock();
                }
                #endregion


                bind(ViewState["cpyno"].ToString());
            }
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "！');", true);

        }
        catch (Exception)
        {

            throw;
        }

    }
    /// <summary>
    /// 绑定公司，参数信息
    /// </summary>
    public void bind(string cpyno)
    {
        try
        {
            User_Company mcpy = (baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + cpyno + "'" }) as List<User_Company>)[0];
            List<Bd_Base_Parameters> listParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + cpyno + "'" }) as List<Bd_Base_Parameters>;
            PbProject.Model.definitionParam.BaseSwitch pmdb = PbProject.WebCommon.Utility.BaseParams.getParams(listParameters);
            if (mcpy != null)
            {

                txtUnitName.Text = mcpy.UninAllName;
                txtLXR.Text = mcpy.ContactUser;
                txtLXTel.Text = mcpy.ContactTel;
                txtFax.Text = mcpy.Fax;
                txtUnitAddr.Text = mcpy.UninAddress;
                txtEmail.Text = mcpy.Email;
                txtBanGongTel.Text = mcpy.Tel;
                txtWebSite.Text = mcpy.WebSite;
                //订单提醒设置
                ddlPromptTime.SelectedValue = mcpy.PromptTime.ToString();
                cbkPrompt.Checked = mcpy.IsPrompt == 1 ? true : false;
                cbkEmpPrompt.Checked = mcpy.IsEmpPrompt == 1 ? true : false;

                List<User_Company> listcpy = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + cpyno + "'" }) as List<User_Company>;
                if (listcpy != null && listcpy.Count > 0)
                {
                    ViewState["cpyid"] = listcpy[0].id;
                    if (listcpy[0].WorkTime != null && listcpy[0].WorkTime != "")
                    {
                        ddlworkHtime.SelectedValue = listcpy[0].WorkTime.Split('-')[0].Split(':')[0];
                        ddlworkMtime.SelectedValue = listcpy[0].WorkTime.Split('-')[0].Split(':')[1];
                        ddlafterworkHtime.SelectedValue = listcpy[0].WorkTime.Split('-')[1].Split(':')[0];
                        ddlafterworkMtime.SelectedValue = listcpy[0].WorkTime.Split('-')[1].Split(':')[1];
                    }
                    if (listcpy[0].BusinessTime != null && listcpy[0].BusinessTime != "")
                    {
                        ddlBusinessHstartTime.SelectedValue = listcpy[0].BusinessTime.Split('-')[0].Split(':')[0];
                        ddlBusinessMstartTime.SelectedValue = listcpy[0].BusinessTime.Split('-')[0].Split(':')[1];
                        ddlBusinessHendTime.SelectedValue = listcpy[0].BusinessTime.Split('-')[1].Split(':')[0];
                        ddlBusinessMendTime.SelectedValue = listcpy[0].BusinessTime.Split('-')[1].Split(':')[1];
                    }
                }
            }

            cksetdlfx.Checked = pmdb.IsDuLiFenXiao == "1" ? true : false;
            if (!string.IsNullOrEmpty(Request["cpyno"]))//平台进入
            {
                trdlfx.Visible = true;
                cksetdlfx.Visible = true;

            }
            else
            {
                if (pmdb.IsDuLiFenXiao == "1")//独立分销(是否显示自己独立信息)
                {
                    trdlfx.Visible = true;
                    ckshowdlinfo.Visible = true;
                    ckshowdlinfo.Checked = pmdb.IsShowDuLiInfo == "1" ? true : false;
                }
                if (!string.IsNullOrEmpty(mcpy.WebSite))//网址不为空时不能修改
                {
                    txtWebSite.Enabled = false;
                }
            }
            Hid_KefuValue.Value = pmdb.cssQQ;
            string s = "initxiugai('" + mcpy.Provice + "','" + mcpy.City + "');";
            ClientScript.RegisterStartupScript(this.GetType(), System.DateTime.Now.Ticks.ToString(), s, true);
        }
        catch (Exception)
        {

            throw;
        }

    }
    /// <summary>
    /// 获取修改参数sql语句
    /// </summary>
    /// <param name="setvalue">参数值</param>
    /// <param name="cpyno">公司编号</param>
    /// <param name="setname">参数名</param>
    /// <returns></returns>
    protected string GetParameterUpSql(string setvalue, string cpyno, string setname)
    {
        string sql = "update Bd_Base_Parameters set SetValue =" +
                 " '" + setvalue + "'" +
                 " where " +
                 " CpyNo = " + " '" + cpyno + "' and " +
                 " SetName = " + " '" + setname + "' ";
        return sql;
    }
}
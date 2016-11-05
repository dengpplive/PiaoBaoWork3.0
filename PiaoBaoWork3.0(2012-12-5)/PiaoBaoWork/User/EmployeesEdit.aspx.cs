using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;

/// <summary>
/// 个人信息维护
/// </summary>
public partial class User_EmployeesEdit : BasePage
{
    public TextBox txtA = new TextBox();
    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                InfoBind();
                if (mCompany.RoleType == 1 && mUser.IsAdmin == 1)//平台员工不能自己设定由管理员设定
                {
                    divipset.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
        }
    }
    /// <summary>
    /// InfoBind()
    /// </summary>
    private void InfoBind()
    {
        try
        {
            string sqlParams = " RestrictLoginAccount = '" + mUser.LoginName + "'";
            List<User_Login_RestrictIp> listuloginip = baseDataManage.CallMethod("User_Login_RestrictIp", "GetList", null, new object[] { sqlParams }) as List<User_Login_RestrictIp>;
            User_Employees ue = new PbProject.Logic.User.User_EmployeesBLL().GetById(mUser.id.ToString());
            if (ue != null)
            {
                txtLoginName.Text = ue.LoginName;
                txtWorkNum.Text = ue.WorkNum;
                txtUserName.Text = ue.UserName;
                rblType.SelectedValue = string.IsNullOrEmpty(ue.Sex) || ue.Sex == "0" ? "0" : "1";
                txtQQ.Text = ue.QQ;
                txtMSN.Text = ue.MSN;
                txtEmail.Text = ue.Email;
                txtTel.Text = ue.Tel;
                txtPhone.Text = ue.Phone;
                txtPostalCode.Text = ue.PostalCode;
                txtAdress.Text = ue.Address;
                UserPowerControl1.ImportantMarkStr = ue.UserPower;
                if (listuloginip != null && listuloginip.Count > 0)
                {
                    if (listuloginip[0].RestrictLoginIP != "")
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            ((System.Web.UI.HtmlControls.HtmlTableRow)this.FindControl("tr" + i)).Style.Value = "display: none";
                        }
                        hidtxtCount.Value = listuloginip[0].RestrictLoginIP.Split('|').Length.ToString();
                        for (int i = 0; i < listuloginip[0].RestrictLoginIP.Split('|').Length; i++)
                        {
                            //不是最后一个
                            if (i < listuloginip[0].RestrictLoginIP.Split('|').Length - 1)
                            {
                                if (i != 4)
                                    ((System.Web.UI.HtmlControls.HtmlGenericControl)this.FindControl("sAdd" + i)).Style.Value = "display: none";
                                if (i != 0)
                                    ((System.Web.UI.HtmlControls.HtmlGenericControl)this.FindControl("sDelete" + i)).Style.Value = "display: none";
                            }
                            //最后一个
                            else
                            {
                                if (i != 4)
                                    ((System.Web.UI.HtmlControls.HtmlGenericControl)this.FindControl("sAdd" + i)).Style.Value = "display: block";
                                if (i != 0)
                                    ((System.Web.UI.HtmlControls.HtmlGenericControl)this.FindControl("sDelete" + i)).Style.Value = "display: block";
                            }
                            ((System.Web.UI.HtmlControls.HtmlTableRow)this.FindControl("tr" + i)).Style.Value = "display: block";
                            txtA = ((TextBox)this.FindControl("txtA" + i));
                            txtA.Text = listuloginip[0].RestrictLoginIP.Split('|')[i].ToString();
                        }
                    }
                }
            }

            else
            {

            }
        }
        catch (Exception ex)
        {


        }
    }
    /// <summary>
    /// lbtnOk_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnOk_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            string Sex = rblType.SelectedValue;
            string QQ = txtQQ.Text;
            string MSN = txtMSN.Text;
            string Email = txtEmail.Text;
            string Tel = txtTel.Text;
            string Phone = txtPhone.Text;
            string PostalCode = txtPostalCode.Text;
            string Address = txtAdress.Text;

            //HashObject parameter = new HashObject();
            //parameter.Add("id", mUser.id);
            //parameter.Add("Sex", Sex);
            //parameter.Add("QQ", QQ);
            //parameter.Add("MSN", MSN);
            //parameter.Add("Email", Email);
            //parameter.Add("Tel", Tel);
            //parameter.Add("Phone", Phone);
            //parameter.Add("PostalCode", PostalCode);
            //parameter.Add("Address", Address);
            //parameter.Add("UserPower", UserPowerControl1.ImportantMarkStr);
            //#region 修改日志
            //#endregion
            //bool retulst = new PbProject.Logic.User.User_EmployeesBLL().UpdateById(parameter);

            List<string> sqllist = new List<string>();
            string sql1 = string.Format("update User_Employees set Sex='{0}',QQ='{1}',MSN='{2}',Email='{3}',Tel='{4}',Phone='{5}',PostalCode='{6}',Address='{7}',UserPower='{8}' where id='{9}'", Sex, QQ, MSN, Email, Tel, Phone, PostalCode, Address, UserPowerControl1.ImportantMarkStr, mUser.id);
            sqllist.Add(sql1);
            string sqlParams = " RestrictLoginAccount = '" + mUser.LoginName + "'";
            List<User_Login_RestrictIp> listuloginip = baseDataManage.CallMethod("User_Login_RestrictIp", "GetList", null, new object[] { sqlParams }) as List<User_Login_RestrictIp>;
            if ((mCompany.RoleType == 1 && mUser.IsAdmin == 0) || mCompany.RoleType != 1)
            {
                string sql2 = " ", ip = "";
                for (int i = 0; i < int.Parse(hidtxtCount.Value); i++)
                {
                    txtA = ((TextBox)this.FindControl("txtA" + i));
                    ip += txtA.Text.Trim() + "|";
                }
                ip = ip.TrimEnd('|');
                if (listuloginip != null && listuloginip.Count > 0)//有ip数据
                {
                    sql2 = string.Format("update User_Login_RestrictIp set RestrictLoginIP='{0}',OperTime='{1}' where RestrictLoginAccount='{2}'", ip, DateTime.Now.ToString(), mUser.LoginName);
                }
                else
                {
                    if (!string.IsNullOrEmpty(ip))
                    {
                        sql2 = string.Format("insert into User_Login_RestrictIp(CpyNo,RestrictLoginAccount,RestrictLoginIP,OperTime) values ('{0}','{1}','{2}','{3}')", mCompany.UninCode, mUser.LoginName, ip, DateTime.Now.ToString());
                    }
                }
                sqllist.Add(sql2);
            }
            string errormsg = "修改失败";
            bool retulst = baseDataManage.ExecuteSqlTran(sqllist, out errormsg);
            if (retulst)
            {
                //同步数据,不用退出再登录才生效,仅是对个人权限,其他数据根据实际情况更新 YYY 2013_03_24
                mUser.UserPower = UserPowerControl1.ImportantMarkStr;
                msg = "更新完毕！";
                InfoBind();
            }
            else
            {
                msg = "更新失败！";
            }
        }
        catch (Exception ex)
        {
            msg = "操作异常！";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialogmsg('" + msg + "');", true);

    }
}

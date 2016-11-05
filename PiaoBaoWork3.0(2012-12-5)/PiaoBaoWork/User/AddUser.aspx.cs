using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Activities.Statements;
using System.Data;
using System.Text;
using DataBase.Data;
using PbProject.Logic.User;

/// <summary>
/// 添加员工
/// </summary>
public partial class Air_Person_AddUser : BasePage
{
    public TextBox txtA = new TextBox();
    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lbtnAddUserPermissions.PostBackUrl = string.Format("~/User/UserPermissionsEdit.aspx?Url=AddUser.aspx&currentuserid={0}", this.mUser.id.ToString());
            BindUserPermissions();
            if (mCompany.RoleType == 1 && mUser.IsAdmin == 0)//只有平台管理员可设置
            {
                divipset.Visible = true;
            }
            if (Request["id"] != null & Request["id"] != "")
            {
                this.txtUser.Enabled = false;
                this.txtPass.Enabled = false;
                BindUserInfo(Request["id"]);
            }
            Bindzjinfo();
        }
    }
    /// <summary>
    /// 绑定证件类型
    /// </summary>
    protected void Bindzjinfo()
    {
        List<Bd_Base_Dictionary> list = baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new Object[] { "ParentID=7" }) as List<Bd_Base_Dictionary>;
        this.ddlCertificateType.DataSource = list;
        ddlCertificateType.DataTextField = "ChildName";
        ddlCertificateType.DataValueField = "ChildID";
        this.ddlCertificateType.DataBind();

    }

    /// <summary>
    /// 绑定部门权限
    /// </summary>
    protected void BindUserPermissions()
    {
        List<User_Permissions> bPaseList = new User_PermissionsBLL().GetListByCpyNo(mUser.CpyNo);

        ddlUserPermissions.DataSource = bPaseList;
        ddlUserPermissions.DataTextField = "DeptName";
        ddlUserPermissions.DataValueField = "id";

        ddlUserPermissions.DataBind();
    }

    /// <summary>
    /// 获取要修改的员工信息
    /// </summary>
    /// <param name="id"></param>
    protected void BindUserInfo(string id)
    {
        User_Employees muser = new PbProject.Logic.User.User_EmployeesBLL().GetById(id);
        txtUser.Text = muser.LoginName;
        txtPass.Text = muser.LoginPassWord;
        txtPass.Attributes.Add("value", "888888");//密码
        txtName.Text = muser.UserName;
        txtNameEasy.Text = muser.NameEasy;
        txtGong.Text = muser.WorkNum;
        ddlCertificateType.SelectedValue = muser.CertificateType;
        txtCertificateNum.Text = muser.CertificateNum;
        txtTel.Text = muser.Tel;
        txtPhone.Text = muser.Phone;
        txtEmail.Text = muser.Email;
        txtAddr.Text = muser.Address;
        rblState.SelectedValue = muser.State.ToString();
        txtovertime.Value = muser.OverDueTime.ToString("yyyy-MM-dd");
        txtQQ.Text = muser.QQ;
        txtMSN.Text = muser.MSN;
        txtBZ.Text = muser.Remark;
        rblSex.SelectedValue = muser.Sex;
        txtYB.Text = muser.PostalCode;
        //rbIsPrompt.SelectedValue = muser.IsPrompt.ToString();
        ddlUserPermissions.SelectedValue = muser.DeptId;
        string sqlParams = " RestrictLoginAccount = '" + txtUser.Text.Trim() + "'";
        List<User_Login_RestrictIp> listuloginip = baseDataManage.CallMethod("User_Login_RestrictIp", "GetList", null, new object[] { sqlParams }) as List<User_Login_RestrictIp>;
        if (mCompany.RoleType == 1 && mUser.IsAdmin == 0)
        {
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
    }

    /// <summary>
    /// 保存信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbsave_Click(object sender, EventArgs e)
    {
        string msg = "";
        DateTime timenow = Convert.ToDateTime(DateTime.Now.ToString());
        try
        {
            string pwdMd5 = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(txtPass.Text.Trim());
            //IHashObject parameter = new HashObject();
            //parameter.Add("CpyNo", mCompany.UninCode);
            //parameter.Add("UserName", txtName.Text);
            //parameter.Add("NameEasy", txtNameEasy.Text);
            //parameter.Add("WorkNum", txtGong.Text);
            //parameter.Add("CertificateType", ddlCertificateType.SelectedValue);
            //parameter.Add("CertificateNum", txtCertificateNum.Text);
            //parameter.Add("Tel", txtTel.Text);
            //parameter.Add("Phone", txtPhone.Text);
            //parameter.Add("Email", txtEmail.Text);
            //parameter.Add("Address", txtAddr.Text);
            //parameter.Add("State", rblState.SelectedValue);
            //parameter.Add("QQ", txtQQ.Text);
            //parameter.Add("MSN", txtMSN.Text);
            //parameter.Add("Remark", txtBZ.Text);
            //parameter.Add("Sex", rblSex.SelectedValue);
            //parameter.Add("PostalCode", txtYB.Text);
            //parameter.Add("IsAdmin", 1);
            //parameter.Add("DeptId", ddlUserPermissions.SelectedValue);
            //parameter.Add("OverDueTime", txtovertime.Value);
            List<string> sqllist = new List<string>();
            string sql2 = " ", ip = "", sql1="";
            string errormsg = "操作失败";
            if (mCompany.RoleType == 1 && mUser.IsAdmin == 0)
            {
                for (int i = 0; i < int.Parse(hidtxtCount.Value); i++)
                {
                    txtA = ((TextBox)this.FindControl("txtA" + i));
                    ip += txtA.Text.Trim() + "|";
                }
                ip = ip.TrimEnd('|');
            }
            if (Request["id"] != null)
            {
                //parameter.Add("id", Request["id"].ToString());
                //msg = new PbProject.Logic.User.User_EmployeesBLL().UpdateById(parameter) ? "更新成功" : "更新失败";


               //修改员工信息
                sql1 = string.Format("update User_Employees set Sex='{0}',QQ='{1}',MSN='{2}',Email='{3}',Tel='{4}',Phone='{5}',PostalCode='{6}',Address='{7}',UserName='{8}',NameEasy='{9}',WorkNum='{10}',CertificateType='{11}',CertificateNum='{12}',State={13},Remark='{14}',DeptId='{15}',OverDueTime='{16}' where id='{17}'",
                rblSex.SelectedValue, txtQQ.Text, txtMSN.Text, txtEmail.Text, txtTel.Text, txtPhone.Text, txtYB.Text, txtAddr.Text, txtName.Text, txtNameEasy.Text, txtGong.Text, ddlCertificateType.SelectedValue, txtCertificateNum.Text, rblState.SelectedValue, txtBZ.Text, ddlUserPermissions.SelectedValue, txtovertime.Value, Request["id"].ToString());
                sqllist.Add(sql1);

                if (mCompany.RoleType == 1 && mUser.IsAdmin == 0)
                {
                    string sqlParams = " RestrictLoginAccount = '" + txtUser.Text.Trim() + "'";
                    List<User_Login_RestrictIp> listuloginip = baseDataManage.CallMethod("User_Login_RestrictIp", "GetList", null, new object[] { sqlParams }) as List<User_Login_RestrictIp>;
                    if (listuloginip != null && listuloginip.Count > 0)//有ip数据
                    {
                        sql2 = string.Format("update User_Login_RestrictIp set RestrictLoginIP='{0}',OperTime='{1}' where RestrictLoginAccount='{2}'", ip, DateTime.Now.ToString(), txtUser.Text.Trim());
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ip))
                        {
                            sql2 = string.Format("insert into User_Login_RestrictIp(CpyNo,RestrictLoginAccount,RestrictLoginIP,OperTime) values ('{0}','{1}','{2}','{3}')", mCompany.UninCode, txtUser.Text.Trim(), ip, DateTime.Now.ToString());
                        }
                    }
                    sqllist.Add(sql2);
                }
                msg = baseDataManage.ExecuteSqlTran(sqllist, out errormsg) == true ? "更新成功" : "更新失败";
                BindUserInfo(Request["id"].ToString());
            }
            else
            {
                //parameter.Add("LoginPassWord", pwdMd5);
                //parameter.Add("CreateTime", timenow);
                //parameter.Add("StartTime", timenow);
                //parameter.Add("LoginName", txtUser.Text.Trim());
                List<User_Employees> listuser = new PbProject.Logic.User.User_EmployeesBLL().GetListByLoginName(txtUser.Text);
                if (listuser != null && listuser.Count > 0)
                {
                    msg = "改账户已存在请重新输入";
                }
                else
                {
                    //msg = new PbProject.Logic.User.User_EmployeesBLL().Insert(parameter) == true ? "添加成功" : "添加失败";
                    sql1 =string.Format("insert into User_Employees (Sex,QQ,MSN,Email,Tel,Phone,PostalCode,Address,UserName,NameEasy,WorkNum,CertificateType,CertificateNum,State,Remark,DeptId,OverDueTime,LoginPassWord,CreateTime,StartTime,LoginName,IsAdmin,CpyNo)" +
                        "values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13},'{14}','{15}','{16}','{17}','{18}','{19}','{20}',{21},'{22}')",
                        rblSex.SelectedValue, txtQQ.Text, txtMSN.Text, txtEmail.Text, txtTel.Text, txtPhone.Text, txtYB.Text, txtAddr.Text, txtName.Text, txtNameEasy.Text, txtGong.Text, ddlCertificateType.SelectedValue, txtCertificateNum.Text, rblState.SelectedValue, txtBZ.Text, ddlUserPermissions.SelectedValue, txtovertime.Value, pwdMd5, timenow, timenow, txtUser.Text.Trim(),1,mCompany.UninCode);
                    sqllist.Add(sql1);

                    if (mCompany.RoleType == 1 && mUser.IsAdmin == 0)
                    {
                        if (!string.IsNullOrEmpty(ip) && ip != "...")
                        {
                            sql2 = string.Format("insert into User_Login_RestrictIp(CpyNo,RestrictLoginAccount,RestrictLoginIP,OperTime) values ('{0}','{1}','{2}','{3}')", mCompany.UninCode, txtUser.Text.Trim(), ip, DateTime.Now.ToString());
                            sqllist.Add(sql2);
                        }
                    }
                }
                msg = baseDataManage.ExecuteSqlTran(sqllist, out errormsg) == true ? "添加成功" : "添加失败";
            }
            
        }
        catch (Exception)
        {
            msg = "操作失败";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
   
   
}
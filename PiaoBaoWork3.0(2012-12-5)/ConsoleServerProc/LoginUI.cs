using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PbProject.Model;
using PbProject.Logic;
using PbProject.Dal.Mapping;
using PbProject.Logic.ControlBase;
using PbProject.ConsoleServerProc;
namespace ConsoleServerProc
{
    public partial class LoginUI : Form
    {
        public LoginUI()
        {
            InitializeComponent();
        }
        CryptionDataHelper encry = new CryptionDataHelper("12584587");
        public delegate void TItem();
        public void ShowText(string text, int flag, DataSet pam)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new TItem(delegate()
                    {
                        ShowText(text, flag, pam);
                    }));
                }
                else
                {
                    if (flag == 0)//错误信息
                    {
                        MessageBox.Show(text, "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else if (flag == 1)//登录成功信息
                    {
                        SaveUser();
                        this.Visible = false;
                        GetLoginUserModel(pam);
                        if (Program.UserModel.COMPANY.RoleType != 2)
                        {
                            MessageBox.Show("该账号没有权限不够！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            OpenTicket OT = new OpenTicket();
                            OT.Show();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                btnOK.Enabled = true;
            }
        }
        /// <summary>
        /// 登录用户信息转化
        /// </summary>
        /// <param name="LoginIn"></param>
        /// <returns></returns>
        public SessionContent GetLoginUserModel(DataSet LoginIn)
        {
            //当前登录用户信息
            User_Employees m_User = null;
            //当前登录公司信息
            User_Company mCompany = null;
            //供应商和落地运营商公司信息
            User_Company mSupCompany = null;
            //当前登录用户参数信息
            List<Bd_Base_Parameters> baseParametersList = null;
            //落地运营商和供应商公司参数信息
            List<Bd_Base_Parameters> SupParameters = null;
            //配置信息
            ConfigParam configparam = null;
            //保存用户信息
            SessionContent sessionContent = new SessionContent();
            if (LoginIn.Tables.Count == 5)
            {
                m_User = MappingHelper<User_Employees>.FillModel(LoginIn.Tables[0].Rows[0]);
                mCompany = MappingHelper<User_Company>.FillModel(LoginIn.Tables[1].Rows[0]);
                baseParametersList = MappingHelper<Bd_Base_Parameters>.FillModelList(LoginIn.Tables[2]);

                mSupCompany = MappingHelper<User_Company>.FillModel(LoginIn.Tables[3].Rows[0]);
                SupParameters = MappingHelper<Bd_Base_Parameters>.FillModelList(LoginIn.Tables[4]);

                configparam = Bd_Base_ParametersBLL.GetConfigParam(SupParameters);
            }
            else if (LoginIn.Tables.Count == 3)
            {
                //管理员                            
                m_User = MappingHelper<User_Employees>.FillModel(LoginIn.Tables[0].Rows[0]);
                mCompany = MappingHelper<User_Company>.FillModel(LoginIn.Tables[1].Rows[0]);
                baseParametersList = MappingHelper<Bd_Base_Parameters>.FillModelList(LoginIn.Tables[2]);
            }
            sessionContent.USER = m_User;// 用户信息
            sessionContent.COMPANY = mCompany;// 公司信息
            sessionContent.SUPCOMPANY = mSupCompany;//供应商和落地运营商公司信息
            sessionContent.BASEPARAMETERS = baseParametersList;//公司参数信息
            sessionContent.SupBASEPARAMETERS = SupParameters;//落地运营商和供应商公司参数信息
            sessionContent.CONFIGPARAM = configparam;//配置信息   
            //设置到全局变量中
            Program.UserModel = sessionContent;
            return sessionContent;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            btnOK.Enabled = false;
            string LoginName = txtAccount.Text.Trim();
            string LoginPwd = txtPwd.Text.Trim();
            Program.LoginUser = LoginName;
            Program.LoginPwd = LoginPwd;
            string LoginType = "";
            int IsLoginSuc = 0;
            Thread th = new Thread(delegate()
            {
                try
                {
                    DataSet ds = WebLoginManage.Login(LoginName, LoginPwd, LoginType, out IsLoginSuc);
                    //登录成功
                    if (IsLoginSuc == 1)
                    {
                        //错误提示
                        ShowText("登录成功", 1, ds);
                    }
                    else if (IsLoginSuc == 2)
                    {
                        //错误提示
                        ShowText(ds.Tables[0].Rows[0][0].ToString().Split('|')[1], 0, null);
                    }
                    else if (IsLoginSuc == 3)
                    {
                        //错误提示
                        ShowText("登录异常", 0, null);
                    }
                }
                catch (Exception)
                {
                    ShowText("登录异常", 0, null);
                }
            });
            th.Start();
        }
        //关闭
        private void btnCancel_Click(object sender, EventArgs e)
        {
            frmClose();
        }

        private void frmClose()
        {
            try
            {
                this.Dispose();
                System.Environment.Exit(0);
            }
            catch (Exception)
            {
            }
        }

        private void LoginUI_Load(object sender, EventArgs e)
        {
            UserInfo User = ConfigHelper.LoadXml("User.xml", typeof(UserInfo)) as UserInfo;
            if (User != null)
            {
                UpdateDataToControl(User);
            }
        }
        /// <summary>
        /// 加载用户信息
        /// </summary>
        /// <param name="User"></param>
        public void UpdateDataToControl(UserInfo User)
        {
            Program.LoginUser = User.LoginUser;
            Program.LoginPwd = encry.DecryptionStringdata(User.LoginPwd);
            ckBox.Checked = User.IsSavePwd;
            if (ckBox.Checked)
            {
                txtAccount.Text = Program.LoginUser;
                txtPwd.Text = Program.LoginPwd;
            }
            Program.IsSavePwd = ckBox.Checked;
        }
        //保存用户信息
        public bool SaveUser()
        {
            UserInfo user = new UserInfo();
            user.LoginPwd = encry.EncryptionStringData(Program.LoginPwd);
            user.LoginUser = Program.LoginUser;
            user.IsSavePwd = Program.IsSavePwd;
            bool isOk = ConfigHelper.SaveXml(ConfigHelper.CurrFilePath, "User.xml", user);
            return isOk;
        }
        //记住密码
        private void ckBox_Click(object sender, EventArgs e)
        {
            Program.IsSavePwd = ckBox.Checked;
        }
        //关闭事件
        private void LoginUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            //SaveUser();
            frmClose();
        }
    }
}

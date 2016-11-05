using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Activities.Statements;
using DataBase.Data;
using System.Data;
using PbProject.Logic.ControlBase;
using PbProject.WebCommon.Utility;
using PbProject.Logic;
/// <summary>
/// 
/// </summary>
public partial class Account_OpenAccount : BasePage
{

    BaseDataManage Manage = new BaseDataManage();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuser.Value = this.mUser.id.ToString();
            rbroletypeBind();

            if (Request["id"] != null)
            {
                BindAccountInfo(Request["id"].ToString());

                this.txtZH.Enabled = false;//账户不能修改
                this.txtPass.Enabled = false;//密码不能修改
                txtUnitName.Enabled = false;//公司名不能修改
                txtPass.Visible = false;//隐藏密码框
                txtshowpass.Visible = true;//显示密码显示框

                if (mCompany.RoleType == 2 || mCompany.RoleType == 4)
                {
                    rbroletype.Enabled = true;
                }
                else
                {
                    rbroletype.Enabled = false;//编辑角色类型不能选择
                }


            }
            else
            {
                bindUserEmployees("");//所属业务员绑定
                //订单提醒设置
                if (mCompany.RoleType == 1)
                {
                    tr_Prompt.Visible = true;
                }
                else
                {
                    tr_Prompt.Visible = false;
                }
            }
            if (rbroletype.Items.Count == 0)
            {
                //提示错误
            }
            //关闭保存按钮
            if ((mCompany.RoleType == 2 || mCompany.RoleType == 3) && mUser.IsAdmin == 1 && (this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|106|")))
            {
                this.lbtnOK.Visible = false;
            }
        }
    }

    /// <summary>
    /// 判断公司类型可以开通的商户
    /// </summary>
    private void rbroletypeBind()
    {
        int type = mCompany.RoleType;

        switch (type)
        {
            case 1:
                rbroletype.Items.Add(new ListItem("落地运营商", "2"));
                rbroletype.Items.Add(new ListItem("供应商", "3"));
                this.divQuanXian.Style["display"] = "block";//平台开户显示重要标识权限
                this.trtime.Visible = true;
                this.hidroletype.Value = "1";
                break;
            case 2:
                rbroletype.Items.Add(new ListItem("分销商", "4"));
                rbroletype.Items.Add(new ListItem("采购商", "5"));
                this.divFenXiaoQuanXian.Style["display"] = "block";//落地运营商开户显示参数信息
                break;
            case 4:
                if (mCompany.UninCode.Length == 18)
                {
                    string strGongYingKongZhiFenXiao = PbProject.WebCommon.Utility.BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
                    //分销开分销
                    if (Request["id"] != null)
                    {
                        rbroletype.Items.Add(new ListItem("分销商", "4"));//二级
                    }
                    else
                    {
                        if (strGongYingKongZhiFenXiao.Contains("|8|"))
                        {
                            rbroletype.Items.Add(new ListItem("分销商", "4"));//二级
                        }
                        else
                        {
                            lbtnOK.Visible = false;
                            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('无分销可开分销权限,请联系上级设置!');", true);
                        }
                    }
                    rbroletype.Items.Add(new ListItem("采购商", "5"));
                }
                else
                {
                    rbroletype.Items.Add(new ListItem("采购商", "5"));//三级
                }
                break;
            default:
                break;
        }

        if (rbroletype.Items.Count != 0)
        {
            if (type == 2)
            {
                rbroletype.Items[1].Selected = true;
            }
            else
            {
                rbroletype.Items[0].Selected = true;
            }
            string roletype = rbroletype.Items[0].Value;
            if (roletype != "2" && roletype != "4")
            {
                this.txtAccountCount.Text = "0";
                this.txtAccountCount.Enabled = false;
            }
            else
            {
                this.txtAccountCount.Enabled = true;
            }
        }
    }
    /// <summary>
    /// 获取修改数据
    /// </summary>
    /// <param name="id"></param>
    protected void BindAccountInfo(string id)
    {
        try
        {
            DataTable dt = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetViewInfoByStrWhere("V_AccountInfo", "IsAdmin=0 and id='" + id + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                #region 加载数据
                this.hidroletype.Value = dt.Rows[0]["RoleType"].ToString();
                rbroletype.SelectedValue = dt.Rows[0]["RoleType"].ToString();
                txtZH.Text = dt.Rows[0]["LoginName"].ToString();
                txtPass.Attributes.Add("value", "888888");//密码
                txtshowpass.Attributes.Add("value", "******");//密码显示框
                txtShiXiaoDate.Text = Convert.ToDateTime(dt.Rows[0]["OverDueTime"]).ToString("yyyy-MM-dd");
                txtName.Text = dt.Rows[0]["UserName"].ToString();
                txtNameEasy.Text = dt.Rows[0]["NameEasy"].ToString();
                rblState.SelectedValue = dt.Rows[0]["State"].ToString();
                txtUnitName.Text = dt.Rows[0]["UninAllName"].ToString();
                txtBanGongTel.Text = dt.Rows[0]["Tel"].ToString();
                txtLXR.Text = dt.Rows[0]["ContactUser"].ToString();
                txtLXTel.Text = dt.Rows[0]["ContactTel"].ToString();
                txtEmail.Text = dt.Rows[0]["Email"].ToString();
                txtFax.Text = dt.Rows[0]["Fax"].ToString();
                txtWebSite.Text = dt.Rows[0]["WebSite"].ToString();
                txtAccountCount.Text = dt.Rows[0]["AccountCount"].ToString();
                hidaccount.Value = dt.Rows[0]["AccountCount"].ToString();
                txtUnitAddr.Text = dt.Rows[0]["UninAddress"].ToString();
                rblcpystate.SelectedValue = dt.Rows[0]["AccountState"].ToString();

                cbkPrompt.Checked = dt.Rows[0]["IsPrompt"].ToString() == "1" ? true : false;//订单提醒开关
                cbkEmpPrompt.Checked = dt.Rows[0]["IsEmpPrompt"].ToString() == "1" ? true : false;//员工订单提醒开关
                ddlPromptTime.SelectedValue = dt.Rows[0]["PromptTime"].ToString();//订单提醒时间间隔 默认15秒
                //订单提醒设置
                if (dt.Rows[0]["RoleType"].ToString() == "2")
                {
                    tr_Prompt.Visible = true;
                }
                else
                {
                    tr_Prompt.Visible = false;
                }
                //抢票绑定
                //抢票扫描时间设置
                Hid_RobSetting.Value = dt.Rows[0]["RobSetting"] != DBNull.Value ? dt.Rows[0]["RobSetting"].ToString() : "";
                //时间范围
                txtRobMinuteInner.Text = dt.Rows[0]["RobInnerTime"] == DBNull.Value ? "60" : dt.Rows[0]["RobInnerTime"].ToString();


                if (dt.Rows[0]["WorkTime"].ToString().Length > 0)
                {
                    ddlworkHtime.SelectedValue = dt.Rows[0]["WorkTime"].ToString().Split('-')[0].Split(':')[0];
                    ddlworkMtime.SelectedValue = dt.Rows[0]["WorkTime"].ToString().Split('-')[0].Split(':')[1];
                    ddlafterworkHtime.SelectedValue = dt.Rows[0]["WorkTime"].ToString().Split('-')[1].Split(':')[0];
                    ddlafterworkMtime.SelectedValue = dt.Rows[0]["WorkTime"].ToString().Split('-')[1].Split(':')[1];
                }
                if (dt.Rows[0]["BusinessTime"].ToString().Length > 0)
                {
                    ddlBusinessHstartTime.SelectedValue = dt.Rows[0]["BusinessTime"].ToString().Split('-')[0].Split(':')[0];
                    ddlBusinessMstartTime.SelectedValue = dt.Rows[0]["BusinessTime"].ToString().Split('-')[0].Split(':')[1];
                    ddlBusinessHendTime.SelectedValue = dt.Rows[0]["BusinessTime"].ToString().Split('-')[1].Split(':')[0];
                    ddlBusinessMendTime.SelectedValue = dt.Rows[0]["BusinessTime"].ToString().Split('-')[1].Split(':')[1];
                }
                pc.Value = dt.Rows[0]["Provice"].ToString() + "|" + dt.Rows[0]["City"].ToString();
                ViewState["CpyNo"] = dt.Rows[0]["UninCode"].ToString();
                //获得账号参数集合
                List<Bd_Base_Parameters> listParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + dt.Rows[0]["UninCode"].ToString() + "'" }) as List<Bd_Base_Parameters>;
                PbProject.Model.definitionParam.BaseSwitch pmdb = PbProject.WebCommon.Utility.BaseParams.getParams(listParameters);

                if (dt.Rows[0]["RoleType"].ToString() == "2" || dt.Rows[0]["RoleType"].ToString() == "3")
                {

                    //网银类型
                    rblPayType.SelectedValue = (string.IsNullOrEmpty(pmdb.WangYinLeiXing)) ? "1" : pmdb.WangYinLeiXing;
                    //采购佣金取舍
                    rblsetCommission.SelectedValue = (string.IsNullOrEmpty(pmdb.setCommission)) ? "2" : pmdb.setCommission;
                    //黑屏参数
                    string[] strs4 = pmdb.HeiPingCanShu.Split('|');
                    txtBlankScreenIp.Text = strs4[0];
                    txtBlankScreenPort.Text = strs4[1];
                    txtWhiteIP.Text = strs4[2];

                    txtWhiteScreenPort.Text = strs4[3];
                    txtoffice.Text = strs4[4];
                    txtBlankUser.Text = strs4[5];

                    txtBlankPwd.Text = strs4[6];
                    txtECBlankPort.Text = strs4[7];
                    txtTicketCompany.Text = strs4[8];
                    txtHangxiehao.Text = strs4[9];
                    if (strs4.Length > 10)
                    {
                        //PidKeyNo
                        txtPidKeyNo.Text = strs4[10];
                    }

                    //大配置参数
                    string[] strs5 = pmdb.DaPeiZhiCanShu.Split('|');
                    txtBigConfigIP.Text = strs5[0];
                    txtBigConfigPort.Text = strs5[1];
                    txtBigOffice.Text = strs5[2];
                    txtBigPwd.Text = strs5[3];
                    //接口账号
                    string[] strs6 = pmdb.JieKouZhangHao.Split('|');
                    txtJKact517.Text = strs6[0].Split('^')[0];
                    txtJKpwd517.Text = strs6[0].Split('^')[1];
                    txtJKkey517.Text = strs6[0].Split('^')[2];
                    txtyckack517.Text = strs6[0].Split('^')[3];
                    txtyckpwd517.Text = strs6[0].Split('^')[4];

                    txtJKact51book.Text = strs6[1].Split('^')[0];
                    txtJKpwd51book.Text = strs6[1].Split('^')[1];
                    txtJKkey51book.Text = strs6[1].Split('^')[2];
                    txtNoticeURL51book.Text = strs6[1].Split('^')[3];

                    txtJKactBT.Text = strs6[2].Split('^')[0];
                    txtJKpwdBT.Text = strs6[2].Split('^')[1];
                    txtJKkeyBT.Text = strs6[2].Split('^')[2];

                    txtJKactPM.Text = strs6[3].Split('^')[0];
                    txtJKpwdPM.Text = strs6[3].Split('^')[1];
                    txtJKkeyPM.Text = strs6[3].Split('^')[2];

                    txtJKactJR.Text = strs6[4].Split('^')[0];
                    txtJKpwdJR.Text = strs6[4].Split('^')[1];


                    txtJKact8000yi.Text = strs6[5].Split('^')[0];
                    txtJKpwd8000yi.Text = strs6[5].Split('^')[1];
                    txtJKDKZFB8000yi.Text = strs6[5].Split('^')[2];

                    txtyixing.Text = strs6[6].Split('^')[0];
                    txtyixinggy.Text = strs6[6].Split('^')[1];

                    if (strs6[6].Split('^').Length == 2)
                    {
                        txtyixingpwd.Text = "";
                    }
                    else
                    {
                        txtyixingpwd.Text = strs6[6].Split('^')[2];
                    }

                    //控制系统权限
                    Importanter2.ImportantMarkStr = pmdb.KongZhiXiTong;
                #endregion
                }
                if (dt.Rows[0]["RoleType"].ToString() == "2" || dt.Rows[0]["RoleType"].ToString() == "3" || dt.Rows[0]["RoleType"].ToString() == "4")
                {
                    //网银账号

                    string[] strs1 = pmdb.WangYinZhangHao.Split('|');
                    txtZfbPay.Text = strs1[0].Split('^')[0];
                    txtZfbPayCZ.Text = strs1[0].Split('^')[1];
                    txtcollectionRateAlipay.Text = strs1[0].Split('^')[2];//支付宝
                    txtcollectiongxRateAlipay.Text = strs1[0].Split('^')[3];

                    txtQKPay.Text = strs1[1].Split('^')[0];
                    txtQKPayCZ.Text = strs1[1].Split('^')[1];
                    txtcollectionRate99Bill.Text = strs1[1].Split('^')[2];//块钱
                    txtcollectiongxRate99Bill.Text = strs1[1].Split('^')[3];

                    txtHfPay.Text = strs1[2].Split('^')[0];
                    txtHfPayCZ.Text = strs1[2].Split('^')[1];
                    txtcollectionRateChinaPNR.Text = strs1[2].Split('^')[2];//汇付
                    txtcollectiongxRateChinaPNR.Text = strs1[2].Split('^')[3];

                    txtCftPay.Text = strs1[3].Split('^')[0];
                    txtCftPayCZ.Text = strs1[3].Split('^')[1];
                    txtcollectionRateTenpay.Text = strs1[3].Split('^')[2];//财付通
                    txtcollectiongxRateTenpay.Text = strs1[3].Split('^')[3];
                    //供应控制分销权限
                    Importanter1.ImportantMarkStr = pmdb.GongYingKongZhiFenXiao;
                }
                //业务员数据加载
                bindUserEmployees(pmdb.SuoShuYeWuYuan);
            }
        }
        catch (Exception)
        {

            throw;
        }

    }
    /// <summary>
    /// lbtnOK_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnOK_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {

            IHashObject paramter = new HashObject();
            int roletype = int.Parse(rbroletype.SelectedValue);
            Guid DeptId = Guid.NewGuid();
            string timenow = DateTime.Now.ToString();
            string worktime = ddlworkHtime.SelectedValue + ":" + ddlworkMtime.SelectedValue + "-" + ddlafterworkHtime.SelectedValue + ":" + ddlafterworkMtime.SelectedValue;
            string BusinessTime = ddlBusinessHstartTime.SelectedValue + ":" + ddlBusinessMstartTime.SelectedValue + "-" + ddlBusinessHendTime.SelectedValue + ":" + ddlBusinessMendTime.SelectedValue;
            string Bd_Base_Parameters_insertSQL = " ";
            string setvalues = "";

            //订单提醒时间
            int _PromptTime = 0;
            string PromptTime = int.TryParse(ddlPromptTime.SelectedValue.Trim(), out _PromptTime) ? _PromptTime.ToString() : "15";
            //抢票持续时间
            int _RobInnerTime = 60;
            _RobInnerTime = int.TryParse(txtRobMinuteInner.Text.Trim(), out _RobInnerTime) ? _RobInnerTime : 60;
            //抢票设置
            string _RobSetting = Hid_RobSetting.Value.Trim();

            //日志
            Log_Operation logoper = new Log_Operation();
            logoper.ModuleName = "下级管理";
            logoper.LoginName = mUser.LoginName;
            logoper.UserName = mUser.UserName;
            logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
            logoper.CpyNo = mCompany.UninCode;
            if (Request["id"] != null && Request["userid"] != null)
            {
                #region 修改
                User_Company mcpy = baseDataManage.CallMethod("User_Company", "GetById", null, new object[] { Request["id"].ToString() }) as User_Company;
                List<User_Company> listcpywebsite = null;
                if (!string.IsNullOrEmpty(txtWebSite.Text.Trim()))
                {
                    listcpywebsite = new PbProject.Logic.User.User_CompanyBLL().GetListBySqlWhere("WebSite like '%" + CommonManage.TrimSQL(txtWebSite.Text) + "%' and id <> '" + Request["id"] + "'");
                }
                if (listcpywebsite != null && listcpywebsite.Count > 0)
                {
                    msg = "该商家所填网址已存在";
                }
                else
                {
                    paramter.Add("User_Company", "update User_Company set UninAllName='" + txtUnitName.Text.Trim() + "',ContactUser='" + txtLXR.Text.Trim() + "',ContactTel='" + txtLXTel.Text.Trim() + "',Tel='" + txtBanGongTel.Text.Trim() + "',Fax='" + txtFax.Text.Trim() + "',Provice='" + Request.Form["province"] + "',City='" + Request.Form["city"] + "',UninAddress='" + txtUnitAddr.Text.Trim() + "',Email='" + txtEmail.Text.Trim() + "',WebSite='" + txtWebSite.Text.Trim() + "',AccountCount=" + txtAccountCount.Text.Trim() + ",AccountState=" + rblcpystate.SelectedValue + ",InvalidationDate='" + txtShiXiaoDate.Text + "',RoleType=" + roletype + ",WorkTime='" + worktime + "',BusinessTime='" + BusinessTime + "',IsPrompt=" + (cbkPrompt.Checked ? 1 : 0) + ",IsEmpPrompt=" + (cbkEmpPrompt.Checked ? 1 : 0) + ", PromptTime=" + PromptTime + ",RobInnerTime=" + _RobInnerTime + ",RobSetting='" + _RobSetting + "' where id='" + Request["id"] + "'");
                    paramter.Add("User_Employees", "update User_Employees set UserName='" + txtName.Text.Trim() + "',NameEasy='" + txtNameEasy.Text.Trim() + "',Tel='" + txtBanGongTel.Text.Trim() + "',Phone='" + txtLXTel.Text.Trim() + "',OverDueTime='" + txtShiXiaoDate.Text.Trim() + "',State=" + rblState.SelectedValue + " where id='" + Request["userid"] + "'");
                    string User_Permissions_UpSql = " ";
                    if (roletype == 2 || roletype == 3)//供应商或运营商 需要 添加标识参数
                    {
                        #region 修改标识参数

                        //网银类型
                        Bd_Base_Parameters_insertSQL += GetParameterUpSql(rblPayType.SelectedValue, ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.wangYinLeiXing);

                        //采购佣金取舍
                        Bd_Base_Parameters_insertSQL += GetParameterUpSql(rblsetCommission.SelectedValue, ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.setCommission);

                        //黑屏参数集合
                        setvalues = txtBlankScreenIp.Text + "|" + txtBlankScreenPort.Text + "|" + txtWhiteIP.Text + "|" + txtWhiteScreenPort.Text + "|" + txtoffice.Text + "|" + txtBlankUser.Text + "|" + txtBlankPwd.Text + "|" + txtECBlankPort.Text + "|" + txtTicketCompany.Text + "|" + txtHangxiehao.Text + "|" + txtPidKeyNo.Text;
                        Bd_Base_Parameters_insertSQL += GetParameterUpSql(setvalues, ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.heiPingCanShu);

                        //大配置参数集合
                        setvalues = txtBigConfigIP.Text + "|" + txtBigConfigPort.Text + "|" + txtBigOffice.Text + "|" + txtBigPwd.Text;
                        Bd_Base_Parameters_insertSQL += GetParameterUpSql(setvalues, ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.daPeiZhiCanShu);

                        //接口账号集合
                        setvalues = txtJKact517.Text + "^" + txtJKpwd517.Text + "^" + txtJKkey517.Text + "^" + txtyckack517.Text + "^" + txtyckpwd517.Text + "|" +
                           txtJKact51book.Text + "^" + txtJKpwd51book.Text + "^" + txtJKkey51book.Text + "^" + txtNoticeURL51book.Text + "|" +
                           txtJKactBT.Text + "^" + txtJKpwdBT.Text + "^" + txtJKkeyBT.Text + "|" +
                           txtJKactPM.Text + "^" + txtJKpwdPM.Text + "^" + txtJKkeyPM.Text + "|" +
                           txtJKactJR.Text + "^" + txtJKpwdJR.Text + "|" +
                           txtJKact8000yi.Text + "^" + txtJKpwd8000yi.Text + "^" + txtJKDKZFB8000yi.Text + "|" +
                           txtyixing.Text + "^" + txtyixinggy.Text + "^" + txtyixingpwd.Text;
                        Bd_Base_Parameters_insertSQL += GetParameterUpSql(setvalues, ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.jieKouZhangHao);

                        //供应控制分销集合
                        Bd_Base_Parameters_insertSQL += GetParameterUpSql(Importanter1.ImportantMarkStr, ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.gongYingKongZhiFenXiao);

                        //控制系统集合
                        Bd_Base_Parameters_insertSQL += GetParameterUpSql(Importanter2.ImportantMarkStr, ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.kongZhiXiTong);
                        #endregion
                    }
                    if (roletype == 4 || roletype == 5)
                    {
                        if (roletype == 4 && mcpy.RoleType == 5)//采购转分销
                        {
                            //网银账号集合

                            setvalues = txtZfbPay.Text + "^" + txtZfbPayCZ.Text + "^^|" + txtQKPay.Text + "^" + txtQKPayCZ.Text + "^^|" + txtHfPay.Text + "^" + txtHfPayCZ.Text + "^^|" + txtCftPay.Text + "^" + txtCftPayCZ.Text + "^^";
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.wangYinZhangHao, setvalues, "网银账号集合(0支付宝收款^支付宝充值收款^本地费率^共享费率|1快钱收款^快钱充值收款^本地费率^共享费率|2汇付收款^汇付充值收款^本地费率^共享费率|3财付通收款^财付通充值收款^本地费率^共享费率)", "网银账号集合");

                            //QQ
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.cssQQ, "", "QQ", "QQ");

                            //是否为独立分销
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.isDuLiFenXiao, "0", "是否为独立分销(0否1是)", "是否为独立分销(0否1是)");

                            //是否显示分销信息
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.isShowDuLiInfo, "0", "是否显示分销信息(0否1是)", "是否显示分销信息(0否1是)");

                            //修改页面权限
                            User_Permissions_UpSql += GetPermissUpSql(roletype, ViewState["CpyNo"].ToString());
                        }
                        else if (roletype == 5 && mcpy.RoleType == 4)//分销转采购
                        {
                            //删除采购不该有的权限
                            Bd_Base_Parameters_insertSQL += "delete from Bd_Base_Parameters " +
                            "where CpyNo= '" + ViewState["CpyNo"].ToString() + "' and SetName not in ('" + PbProject.Model.definitionParam.paramsName.gongYingKongZhiFenXiao + "','" + PbProject.Model.definitionParam.paramsName.suoShuYeWuYuan + "')";

                            //修改页面权限
                            User_Permissions_UpSql += GetPermissUpSql(roletype, ViewState["CpyNo"].ToString());
                        }

                        //供应控制分销参数(运营修改一级分销时就修改此分销下级所有)
                        Bd_Base_Parameters_insertSQL += GetParameterUpSql1(Importanter3.ImportantMarkStr, ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.gongYingKongZhiFenXiao);
                    }
                    if ((roletype == 2 || roletype == 3 || roletype == 4) && mcpy.RoleType != 5)//要修改的用户不能为（采购转分销）
                    {
                        //网银账号集合
                        if (roletype == 4)
                        {
                            setvalues = txtZfbPay.Text + "^" + txtZfbPayCZ.Text + "^^|" + txtQKPay.Text + "^" + txtQKPayCZ.Text + "^^|" + txtHfPay.Text + "^" + txtHfPayCZ.Text + "^^|" + txtCftPay.Text + "^" + txtCftPayCZ.Text + "^^";
                        }
                        else
                        {
                            setvalues = txtZfbPay.Text + "^" + txtZfbPayCZ.Text + "^" + txtcollectionRateAlipay.Text + "^" + txtcollectiongxRateAlipay.Text + "|" + txtQKPay.Text + "^" + txtQKPayCZ.Text + "^" + txtcollectionRate99Bill.Text + "^" + txtcollectiongxRate99Bill.Text + "|" + txtHfPay.Text + "^" + txtHfPayCZ.Text + "^" + txtcollectionRateChinaPNR.Text + "^" + txtcollectiongxRateChinaPNR.Text + "|" + txtCftPay.Text + "^" + txtCftPayCZ.Text + "^" + txtcollectionRateTenpay.Text + "^" + txtcollectiongxRateTenpay.Text;
                        }
                        Bd_Base_Parameters_insertSQL += GetParameterUpSql(setvalues, ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.wangYinZhangHao);
                    }

                    //所属业务员
                    Bd_Base_Parameters_insertSQL += GetParameterUpSql(SaleValue.Value, ViewState["CpyNo"].ToString(), PbProject.Model.definitionParam.paramsName.suoShuYeWuYuan);

                    paramter.Add("Bd_Base_Parameters", Bd_Base_Parameters_insertSQL);
                    paramter.Add("User_Permissions", User_Permissions_UpSql);

                    DataTable dt = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetViewInfoByStrWhere("V_AccountInfo", "IsAdmin=0 and id='" + Request["id"].ToString() + "'");
                    List<Bd_Base_Parameters> listParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + dt.Rows[0]["UninCode"].ToString() + "'" }) as List<Bd_Base_Parameters>;
                    PbProject.Model.definitionParam.BaseSwitch pmdb = PbProject.WebCommon.Utility.BaseParams.getParams(listParameters);
                    logoper.OperateType = "修改下级";
                    logoper.OptContent =
                    "原公司名称:" + dt.Rows[0]["UninAllName"].ToString() + ",新公司名称:" + txtUnitName.Text.Trim() + "<br>" +
                    "原联系人:" + dt.Rows[0]["ContactUser"].ToString() + ",新联系人:" + txtLXR.Text.Trim() + "<br>" +
                    "原手机号码:" + dt.Rows[0]["ContactTel"].ToString() + ",新手机号码:" + txtLXTel.Text.Trim() + "<br>" +
                        "原公司电话:" + dt.Rows[0]["Tel"] + ",新公司电话:" + txtBanGongTel.Text.Trim() + "<br>" +
                            "原电子邮箱:" + dt.Rows[0]["Email"] + ",新电子邮箱:" + dt.Rows[0]["Email"].ToString() + "<br>" +
                                "原传真号码:" + dt.Rows[0]["Fax"] + ",新传真号码:" + txtFax.Text.Trim() + "<br>" +
                                    "原省市:" + dt.Rows[0]["Provice"].ToString() + "|" + dt.Rows[0]["City"].ToString() + ",新省市:" + Request.Form["province"] + "|" + Request.Form["city"] + "<br>" +
                                        "原网址:" + dt.Rows[0]["WebSite"].ToString() + ",新网址:" + txtWebSite.Text.Trim() + "<br>" +
                                            "原可开下级账号数:" + dt.Rows[0]["AccountCount"] + ",新可开下级账号数:" + txtAccountCount.Text.Trim() + "<br>" +
                                                "原单位地址:" + dt.Rows[0]["UninAddress"].ToString() + ",新单位地址:" + txtUnitAddr.Text.Trim() + "<br>" +
                                                    "原角色类型:" + dt.Rows[0]["RoleType"].ToString() + ",新角色类型:" + rbroletype.SelectedValue + "<br>" +
                                                        "原公司状态:" + dt.Rows[0]["AccountState"].ToString() + ",新公司状态:" + rblcpystate.SelectedValue + "<br>" +
                                                            "原失效日期:" + dt.Rows[0]["OverDueTime"] + ",新失效日期:" + txtShiXiaoDate.Text + "<br>" +
                                                                "原姓名:" + dt.Rows[0]["UserName"].ToString() + ",新姓名:" + txtName.Text + "<br>" +
                                                                    "原帐号状态:" + dt.Rows[0]["State"].ToString() + ",新帐号状态:" + rblState.SelectedValue + "<br>" +
                                                                    "原订单提醒:" + dt.Rows[0]["IsPrompt"].ToString() + "|" + dt.Rows[0]["IsEmpPrompt"].ToString() + ",新订单提醒:" + (cbkPrompt.Checked ? "1" : "0") + "|" + (cbkEmpPrompt.Checked ? "1" : "0") + "<br>" +
                                                                            "原订单提醒时间间隔:" + dt.Rows[0]["PromptTime"].ToString() + ",新订单提醒时间间隔:" + ddlPromptTime.SelectedValue + "<br>" +
                                                                                "原上下班时间:" + dt.Rows[0]["WorkTime"].ToString() + ",新上下班时间:" + worktime + "<br>" +
                                                                                    "原业务处理时间:" + dt.Rows[0]["BusinessTime"].ToString() + ",新业务处理时间:" + BusinessTime + "<br>";
                    if (dt.Rows[0]["RoleType"].ToString() == "2" || dt.Rows[0]["RoleType"].ToString() == "3")
                    {
                        string jkzh = txtJKact517.Text + "^" + txtJKpwd517.Text + "^" + txtJKkey517.Text + "^" + txtyckack517.Text + "^" + txtyckpwd517.Text + "|" +
                        txtJKact51book.Text + "^" + txtJKpwd51book.Text + "^" + txtJKkey51book.Text + "^" + txtNoticeURL51book.Text + "|" +
                        txtJKactBT.Text + "^" + txtJKpwdBT.Text + "^" + txtJKkeyBT.Text + "|" +
                        txtJKactPM.Text + "^" + txtJKpwdPM.Text + "^" + txtJKkeyPM.Text + "|" +
                        txtJKactJR.Text + "^" + txtJKpwdJR.Text + "|" +
                        txtJKact8000yi.Text + "^" + txtJKpwd8000yi.Text + "^" + txtJKDKZFB8000yi.Text + "|" + txtyixing.Text + "^" + txtyixinggy.Text;
                        logoper.OptContent +=
                            "原网银类型:" + pmdb.WangYinLeiXing + ",新网银类型:" + rblPayType.SelectedValue + "<br>" +
                            "原采购佣金取舍:" + pmdb.setCommission + ",新采购佣金取舍:" + rblsetCommission.SelectedValue + "<br>" +
                            "原重要标志设置（控制系统权限）:" + pmdb.KongZhiXiTong + ",新重要标志设置（控制系统权限）:" + Importanter2.ImportantMarkStr + "<br>" +
                            "原黑屏参数设置:" + pmdb.HeiPingCanShu + ",新黑屏参数设置:" + txtBlankScreenIp.Text + "|" + txtBlankScreenPort.Text + "|" + txtWhiteIP.Text + "|" + txtWhiteScreenPort.Text + "|" + txtoffice.Text + "|" + txtBlankUser.Text + "|" + txtBlankPwd.Text + "|" + txtECBlankPort.Text + "|" + txtTicketCompany.Text + "|" + txtHangxiehao.Text + "|" + txtPidKeyNo.Text + "<br>" +
                            "原大配置参数设置:" + pmdb.DaPeiZhiCanShu + ",新大配置参数设置:" + txtBigConfigIP.Text + "|" + txtBigConfigPort.Text + "|" + txtBigOffice.Text + "|" + txtBigPwd.Text + "<br>" +
                            "原接口账号设置 :" + pmdb.JieKouZhangHao + ",新接口账号设置 :" + jkzh + "<br>";

                    }
                    if (dt.Rows[0]["RoleType"].ToString() == "2" || dt.Rows[0]["RoleType"].ToString() == "3" || dt.Rows[0]["RoleType"].ToString() == "4")
                    {
                        logoper.OptContent +=
                        "原网银账号及收款费率:" + pmdb.WangYinZhangHao + ",新网银账号及收款费率:" + txtZfbPay.Text + "^" + txtZfbPayCZ.Text + "^" + txtcollectionRateAlipay.Text + "^" + txtcollectiongxRateAlipay.Text + "|" + txtQKPay.Text + "^" + txtQKPayCZ.Text + "^" + txtcollectionRate99Bill.Text + "^" + txtcollectiongxRate99Bill.Text + "|" + txtHfPay.Text + "^" + txtHfPayCZ.Text + "^" + txtcollectionRateChinaPNR.Text + "^" + txtcollectiongxRateChinaPNR.Text + "|" + txtCftPay.Text + "^" + txtCftPayCZ.Text + "^" + txtcollectionRateTenpay.Text + "^" + txtcollectiongxRateTenpay.Text + "<br>" +
                        "原重要标志设置（供应控制分销权限):" + pmdb.GongYingKongZhiFenXiao + ",新重要标志设置（供应控制分销权限）:" + Importanter1.ImportantMarkStr + "<br>";

                    }
                    logoper.OptContent +=
                        "原抢票参数设置 :" + dt.Rows[0]["RobSetting"].ToString() + "|" + dt.Rows[0]["RobInnerTime"].ToString() + ",新抢票参数设置 :" + _RobSetting + "|" + _RobInnerTime + "<br>" +
                        "原所属业务员 :" + pmdb.SuoShuYeWuYuan + ",新所属业务员 :" + SaleValue.Value + "<br>";
                    if (new PbProject.Logic.User.User_CompanyBLL().uporinAccount(paramter, 1) > 0)
                    {
                        msg = "修改成功";
                        new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);//修改日志
                    }
                    else
                    {
                        msg = "修改失败!";
                    }

                }
                #endregion


                #region 更新IIS中应用程序数据 时时生效
                UpdateApp();
                #endregion
            }
            else
            {
                #region 添加
                //获取下级公司编号
                string unincode = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetUninCode(mCompany.UninCode);
                //添加账户
                List<User_Company> listCpyAccount = new PbProject.Logic.User.User_CompanyBLL().GetListBySqlWhere("UninCode like'" + mCompany.UninCode + "%' and len(UninCode)=" + (mCompany.UninCode.Length + 6));
                List<User_Company> listcpy = new PbProject.Logic.User.User_CompanyBLL().GetListByCpyName(txtUnitName.Text);
                List<User_Employees> listuser = new PbProject.Logic.User.User_EmployeesBLL().GetListByLoginName(txtZH.Text);
                List<User_Company> listcpywebsite = new PbProject.Logic.User.User_CompanyBLL().GetListBySqlWhere("WebSite like '%" + CommonManage.TrimSQL(txtWebSite.Text) + "%'");
                if (listcpy != null && listcpy.Count > 0)
                {
                    msg = "商家已存在请重新输入";
                }
                else if (listuser != null && listuser.Count > 0)
                {
                    msg = "该账户已存在请重新输入";
                }
                else if (mCompany.AccountCount <= listCpyAccount.Count)
                {
                    msg = "该商家可开下级账户数不足";
                }
                else if (!string.IsNullOrEmpty(txtWebSite.Text.Trim()) && listcpywebsite.Count > 0)
                {
                    msg = "该商家所填网址已存在";
                }
                else
                {
                    if (unincode != null)
                    {

                        string sql = "select replace((select replace((select [PageIndex] from [Bd_Base_Page] A where RoleType=" + roletype + " for xml auto),'<A PageIndex=\"','')),'\"/>',',')";
                        //公司信息
                        paramter.Add("User_Company", "insert into User_Company(UninCode,UninAllName,ContactUser,ContactTel,Tel,Fax,CreateTime,Provice,City,UninAddress,Email,WebSite,AccountCount,AccountState,TakeEffectDate,InvalidationDate,RoleType,WorkTime,BusinessTime,IsPrompt,IsEmpPrompt,PromptTime,RobInnerTime,RobSetting)" +
                        " values('" + unincode + "','" + txtUnitName.Text.Trim() + "','" + txtLXR.Text.Trim() + "','" + txtLXTel.Text.Trim() + "','" + txtBanGongTel.Text.Trim() + "','" + txtFax.Text.Trim().ToString() + "','" + Convert.ToDateTime(timenow) + "','" + Request.Form["province"] + "','" + Request.Form["city"] + "','" + txtUnitAddr.Text.Trim() + "','" + txtEmail.Text.Trim() + "','" + txtWebSite.Text.Trim() + "'," + txtAccountCount.Text.Trim() + "," + Convert.ToInt32(rblcpystate.SelectedValue.ToString()) + ",'" + Convert.ToDateTime(timenow) + "','" + Convert.ToDateTime(txtShiXiaoDate.Text.ToString()) + "'," + roletype + ",'" + worktime + "','" + BusinessTime + "'," + (cbkPrompt.Checked ? "1" : "0") + "," + (cbkEmpPrompt.Checked ? "1" : "0") + "," + PromptTime + "," + _RobInnerTime + ",'" + _RobSetting + "')");
                        //用户信息
                        paramter.Add("User_Employees", "insert into User_Employees(DeptId,CpyNo,LoginName,LoginPassWord,UserName,NameEasy,Tel,Phone,CreateTime,StartTime,OverDueTime,IsAdmin)" +
                            " values('" + DeptId + "','" + unincode + "','" + txtZH.Text.Trim() + "','" + PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(txtPass.Text.Trim()) + "','" + txtName.Text.Trim() + "','" + txtNameEasy.Text.Trim() + "','" + txtBanGongTel.Text.Trim() + "','" + txtLXTel.Text.Trim() + "','" + Convert.ToDateTime(timenow) + "','" + Convert.ToDateTime(timenow) + "','" + Convert.ToDateTime(txtShiXiaoDate.Text.ToString()) + "',0)");
                        //页面权限信息
                        paramter.Add("User_Permissions", "insert into User_Permissions(id,CpyNo,DeptName,DeptIndex,ParentIndex,[Permissions],Remark,A1)" +
                            " values('" + DeptId + "','" + unincode + "','" + txtUnitName.Text.Trim() + "',1,1,(" + sql + "),'',0)");

                        if (roletype == 2 || roletype == 3)//供应商或运营商 需要 添加标识参数
                        {
                            #region 添加标识参数

                            //网银类型
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.wangYinLeiXing, rblPayType.SelectedValue, "网银类型(0不使用网银,5支付宝网银,6快钱网银,7汇付网银,8财付通网银)", "网银类型");

                            //采购佣金取舍
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.setCommission, rblsetCommission.SelectedValue, "采购佣金取舍(0:保留元,1:保留角,2:保留分)", "采购佣金取舍(0:保留元,1:保留角,2:保留分)");

                            //黑屏参数集合
                            setvalues = txtBlankScreenIp.Text + "|" + txtBlankScreenPort.Text + "|" + txtWhiteIP.Text + "|" + txtWhiteScreenPort.Text + "|" + txtoffice.Text + "|" + txtBlankUser.Text + "|" + txtBlankPwd.Text + "|" + txtECBlankPort.Text + "|" + txtTicketCompany.Text + "|" + txtHangxiehao.Text + "|" + txtPidKeyNo.Text;
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.heiPingCanShu, setvalues, "黑屏参数(0网页黑屏IP|1网页黑屏端口|2白屏IP|3白屏交互端口|4Office号|5网页黑屏帐号|6网页黑屏密码|7EC网页黑屏监听端口|8开票单位名称|9航协号|PidKeyNo)", "黑屏参数");

                            //大配置参数集合
                            setvalues = txtBigConfigIP.Text + "|" + txtBigConfigPort.Text + "|" + txtBigOffice.Text + "|" + txtBigPwd.Text;
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.daPeiZhiCanShu, setvalues, "大配置参数(0大配置IP|1大配置端口|2大配置Office|3大配置名称与密码)", "大配置参数");

                            //接口账号集合
                            setvalues = txtJKact517.Text + "^" + txtJKpwd517.Text + "^" + txtJKkey517.Text + "^" + txtyckack517.Text + "^" + txtyckpwd517.Text + "|" +
                            txtJKact51book.Text + "^" + txtJKpwd51book.Text + "^" + txtJKkey51book.Text + "^" + txtNoticeURL51book.Text + "|" +
                            txtJKactBT.Text + "^" + txtJKpwdBT.Text + "^" + txtJKkeyBT.Text + "|" +
                            txtJKactPM.Text + "^" + txtJKpwdPM.Text + "^" + txtJKkeyPM.Text + "|" +
                            txtJKactJR.Text + "^" + txtJKpwdJR.Text + "|" +
                            txtJKact8000yi.Text + "^" + txtJKpwd8000yi.Text + "^" + txtJKDKZFB8000yi.Text + "|" +
                            txtyixing.Text + "^" + txtyixinggy.Text + "^" + txtyixingpwd.Text;

                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.jieKouZhangHao, setvalues, "接口账号(0 517账号^517密码^517密钥^517预存款账户^517预存款密码|1 51book账号^51book密码^51book密钥^51book通知URL地址|2 百拓账号^百拓密码^百拓密钥|3 票盟账号^票盟密码^票盟密钥|4 今日账号^今日密码|5 8000y账号^8000y密码^8000y代扣支付宝|6 易行总账号^易行供应账号)", "接口账号");

                            //供应控制分销集合
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.gongYingKongZhiFenXiao, Importanter1.ImportantMarkStr, "供应控制分销", "供应控制分销");

                            //控制系统集合
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.kongZhiXiTong, Importanter2.ImportantMarkStr, "控制系统", "控制系统");

                            //log图片路径
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.cssURL, unincode, "机票样式路径", "机票样式路径");

                            //自动出票航空公司帐号密码
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.autoAccount, "^^^://^^^://^^^://^^^://^^^://^^^://^^^://^^^://^^^://^^^://^^^://", "自动出票航空公司帐号密码(总长度只有500，现在暂时够用)格式：CA:xxx//xxx^^^CZ:xxx//xxx^^^MU:xxx//xxx", "自动出票航空公司帐号密码");

                            //自动出票支付帐号参数
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.autoPayAccount, "^^|^||", "格式：自动出票方式(1，支付宝本票通；2，汇付天下出票窗)^自动出票重调次数^帐号|是否签约(1，已签约；2，未签)^帐号|密码|支付方式(1，信用账户；2，付款账户)", "自动出票支付帐号参数");

                            //出票时间
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.chuPiaoShiJian, "|||||||||", "0-B2B|1-BSP|2-517|3-百拓|4-8000|5-今日|6-票盟|7-51book|8-共享|9-易行", "出票时间");

                            #endregion
                        }
                        if (roletype == 2 || roletype == 3 || roletype == 4)
                        {
                            //网银账号集合
                            if (roletype == 4)
                            {
                                setvalues = txtZfbPay.Text + "^" + txtZfbPayCZ.Text + "^^|" + txtQKPay.Text + "^" + txtQKPayCZ.Text + "^^|" + txtHfPay.Text + "^" + txtHfPayCZ.Text + "^^|" + txtCftPay.Text + "^" + txtCftPayCZ.Text + "^^";
                            }
                            else
                            {
                                setvalues = txtZfbPay.Text + "^" + txtZfbPayCZ.Text + "^" + txtcollectionRateAlipay.Text + "^" + txtcollectiongxRateAlipay.Text + "|" + txtQKPay.Text + "^" + txtQKPayCZ.Text + "^" + txtcollectionRate99Bill.Text + "^" + txtcollectiongxRate99Bill.Text + "|" + txtHfPay.Text + "^" + txtHfPayCZ.Text + "^" + txtcollectionRateChinaPNR.Text + "^" + txtcollectiongxRateChinaPNR.Text + "|" + txtCftPay.Text + "^" + txtCftPayCZ.Text + "^" + txtcollectionRateTenpay.Text + "^" + txtcollectiongxRateTenpay.Text;
                            }
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.wangYinZhangHao, setvalues, "网银账号集合(0支付宝收款^支付宝充值收款^本地费率^共享费率|1快钱收款^快钱充值收款^本地费率^共享费率|2汇付收款^汇付充值收款^本地费率^共享费率|3财付通收款^财付通充值收款^本地费率^共享费率)", "网银账号集合");
                        }
                        if (roletype == 4 || roletype == 5)
                        {
                            //供应控制分销参数
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.gongYingKongZhiFenXiao, Importanter3.ImportantMarkStr, "供应控制分销", "供应控制分销");
                        }
                        if (roletype == 2 || roletype == 4)
                        {
                            if (roletype == 2)
                            {
                                //运营权限
                                Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.yunYingQuanXian, "", "运营权限", "运营权限");
                            }
                            else
                            {
                                //是否为独立分销
                                Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.isDuLiFenXiao, "0", "是否为独立分销(0否1是)", "是否为独立分销(0否1是)");
                                //是否显示分销信息
                                Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.isShowDuLiInfo, "0", "是否显示分销信息(0否1是)", "是否显示分销信息(0否1是)");
                            }
                            //QQ
                            Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.cssQQ, "", "QQ", "QQ");
                        }

                        //所属业务员
                        Bd_Base_Parameters_insertSQL += GetParameterinsertSql(unincode, PbProject.Model.definitionParam.paramsName.suoShuYeWuYuan, SaleValue.Value, "所属业务员", "所属业务员");

                        paramter.Add("Bd_Base_Parameters", Bd_Base_Parameters_insertSQL);
                        logoper.OperateType = "添加下级";
                        logoper.OptContent = "商家信息:UninAllName='" + txtUnitName.Text.Trim() + "',ContactUser='" + txtLXR.Text.Trim() + "',ContactTel='" + txtLXTel.Text.Trim() + "',Tel='" + txtBanGongTel.Text.Trim() + "',Fax='" + txtFax.Text.Trim() + "',Provice='" + Request.Form["province"] + "',City='" + Request.Form["city"] + "',UninAddress='" + txtUnitAddr.Text.Trim() + "',Email='" + txtEmail.Text.Trim() + "',WebSite='" + txtWebSite.Text.Trim() + "',AccountCount=" + txtAccountCount.Text.Trim() + ",AccountState=" + rblcpystate.SelectedValue + ",InvalidationDate='" + txtShiXiaoDate.Text + "',RoleType=" + roletype + ",WorkTime='" + worktime + "',BusinessTime=" + BusinessTime +
                        "/////员工信息:UserName='" + txtName.Text.Trim() + "',NameEasy='" + txtNameEasy.Text.Trim() + "',Tel='" + txtBanGongTel.Text.Trim() + "',Phone='" + txtLXTel.Text.Trim() + "',OverDueTime='" + txtShiXiaoDate.Text.Trim() + "',State=" + rblState.SelectedValue;
                        if (new PbProject.Logic.User.User_CompanyBLL().uporinAccount(paramter, 1) > 0)
                        {
                            msg = "开户成功";
                            new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);//添加下级日志
                        }
                        else
                        {
                            msg = "添加失败";
                        }


                    }
                }
                #endregion
            }
            SalesManA.Items.Clear();
            string values = this.SaleValue.Value;
            bindUserEmployees(values);
        }
        catch (Exception)
        {
            msg = "操作失败！";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);

    }
    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="strVale"></param>
    public void bindUserEmployees(string strVale)
    {
        strVale = "/" + strVale.ToUpper() + "/";
        //PiaoBao.BLLLogic.Person.Pmanager p = PiaoBao.BLLLogic.Factory_System.CreatePmanager();
        //IList<User_Employees> GetUser_Employees = p.GetUser_Employees("  UserType=1 and Cpyid=" + mCompany.Id, 1, 1000, "*", " id desc ");

        string sqlParams = " CpyNo = '" + mCompany.UninCode + "'";
        List<User_Employees> GetUser_Employees = Manage.CallMethod("User_Employees", "GetList", null, new object[] { sqlParams }) as List<User_Employees>;



        if (GetUser_Employees != null && GetUser_Employees.Count > 0)
        {
            for (int i = 0; i < GetUser_Employees.Count; i++)
            {

                if (strVale.Contains("/" + GetUser_Employees[i].id.ToString().ToUpper() + "/"))
                {
                    this.SalesManA.Items.Add(new ListItem(GetUser_Employees[i].UserName.ToString(), GetUser_Employees[i].id.ToString()));
                    SaleValue.Value = GetUser_Employees[i].id.ToString().ToUpper() + "/";
                }
                else
                {
                    this.SalesManB.Items.Add(new ListItem(GetUser_Employees[i].UserName.ToString(), GetUser_Employees[i].id.ToString().ToUpper()));
                }
            }
        }
        else
        {
            //没有数据
        }
    }
    protected void lk8000yiZFBSigning_Click(object sender, EventArgs e)
    {
        try
        {
            w_8000YService.W8000YService WSvc8000Y = new w_8000YService.W8000YService();

            DataTable dt = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetViewInfoByStrWhere("V_AccountInfo", "IsAdmin=0 and id='" + Request["id"].ToString() + "'");

            List<PbProject.Model.Bd_Base_Parameters> mBP = new PbProject.Logic.ControlBase.BaseDataManage().
                CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + dt.Rows[0]["UninCode"].ToString() + "'" }) as List<PbProject.Model.Bd_Base_Parameters>;

            PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
            string Accout8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[0];

            string Password8000yi = BS.JieKouZhangHao.Split('|')[5].Split('^')[1];
            if (txtJKDKZFB8000yi.Text != "")
            {
                string url = WSvc8000Y.PaySignOn(Accout8000yi, Password8000yi, txtJKDKZFB8000yi.Text);
                lk8000yiZFBSigning.Visible = false;
                Response.Write("<script> window.open('" + url + "'); </script>");
            }
            else
            {
                lk8000yiZFBSigning.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('请输入账号!');", true);
            }

        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('签约失败:" + ex.ToString() + "');", true);
        }
    }

    protected void YiXingSigning_Click(object sender, EventArgs e)
    {
        try
        {
            w_YeeXingService.YeeXingSerivce WSvcYeeXing = new w_YeeXingService.YeeXingSerivce();
            DataSet ds = WSvcYeeXing.UserSign(txtyixing.Text, txtyixinggy.Text, PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(txtyixingpwd.Text));
            if (ds.Tables[0].Rows[0][0].ToString().ToUpper() == "T")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('签约成功!');", true);
            }
            else if (ds.Tables[0].Rows[0][0].ToString().ToUpper() == "F")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + ds.Tables[0].Rows[0][1].ToString() + "');", true);
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('签约失败:" + ex.ToString() + "');", true);
        }
    }


    /// <summary>
    /// 获取添加参数sql语句
    /// </summary>
    /// <param name="cpyno">公司编号</param>
    /// <param name="setname">参数名</param>
    /// <param name="setvalue">参数值</param>
    /// <param name="descriptiong">描述</param>
    /// <param name="remark">备注</param>
    /// <returns></returns>
    protected string GetParameterinsertSql(string cpyno, string setname, string setvalue, string descriptiong, string remark)
    {
        string sql = "insert into Bd_Base_Parameters (" +
                          " id, " +
                          " CpyNo, " +
                          " SetName, " +
                          " SetValue, " +
                          " SetDescription, " +
                          " StartDate, " +
                          " EndDate, " +
                          " Remark) " +
                          " values (" +
                          " newid()," +
                          " '" + cpyno + "'," +
                          " '" + setname + "', " +
                          "  '" + setvalue + "'," +
                          " '" + descriptiong + "', " +
                          " '2012-12-05 00:00:00.000' ," +
                          " '2112-12-05 00:00:00.000' ," +
                          " '" + remark + "' " +
                          " ) ";
        return sql;
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
    protected string GetParameterUpSql1(string setvalue, string cpyno, string setname)
    {
        string sql = "update Bd_Base_Parameters set SetValue =" +
                 " '" + setvalue + "'" +
                 " where " +
                 " CpyNo like " + " '" + cpyno + "%' and " +
                 " SetName = " + " '" + setname + "' ";
        return sql;
    }
    /// <summary>
    /// 获取页面权限修改sql语句
    /// </summary>
    /// <param name="roletype"></param>
    /// <param name="cpyno"></param>
    /// <returns></returns>
    protected string GetPermissUpSql(int roletype, string cpyno)
    {
        string sql = "select replace((select replace((select [PageIndex] from [Bd_Base_Page] A where RoleType=" + roletype + " for xml auto),'<A PageIndex=\"','')),'\"/>',',')";
        return "update User_Permissions set [Permissions]=(" + sql + ") where CpyNo='" + cpyno + "'";
    }


    /// <summary>
    /// 保存时读取数据库数据更新到IIS应用程序中的数据
    /// </summary>
    /// <returns></returns>
    public bool UpdateApp()
    {
        bool IsUpdateSuc = false;
        try
        {   //公司id 和用户userid
            if (Request["id"] != null && Request["userid"] != null && mCompany != null)
            {
                string UId = Request["userid"].ToString();
                SessionContent sessionContent = HttpContext.Current.Application[UId] as SessionContent;
                //登录后就会存在 没有登录就为空
                if (sessionContent != null)
                {

                    //公司和公司参数
                    List<User_Company> SupCompanyList = baseDataManage.CallMethod("User_Company", "GetList", null, new object[] { " id='" + Request["id"].ToString() + "' " }) as List<User_Company>;
                    if (SupCompanyList != null && SupCompanyList.Count > 0)
                    {
                        sessionContent.COMPANY = SupCompanyList[0];
                        //参数
                        List<Bd_Base_Parameters> listParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { " cpyno='" + SupCompanyList[0].UninCode + "'" }) as List<Bd_Base_Parameters>;
                        if (listParameters != null)
                        {
                            sessionContent.BASEPARAMETERS = listParameters;
                        }
                        if (mCompany.RoleType == 1)
                        {
                            //公司
                            HttpContext.Current.Application.Lock();
                            Application[SupCompanyList[0].UninCode + "Company"] = SupCompanyList[0];
                            HttpContext.Current.Application.UnLock();
                            HttpContext.Current.Application.Lock();
                            Application[SupCompanyList[0].UninCode + "Parameters"] = listParameters;
                            HttpContext.Current.Application.UnLock();
                        }
                    }

                    //用户账号
                    List<User_Employees> EmpList = baseDataManage.CallMethod("User_Employees", "GetList", null, new object[] { "id='" + UId + "'" }) as List<User_Employees>;
                    if (EmpList != null && EmpList.Count > 0)
                    {
                        sessionContent.USER = EmpList[0];
                    }
                    //重新保存会到IIS应用程序池中
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[UId] = sessionContent;
                    HttpContext.Current.Application.UnLock();
                    IsUpdateSuc = true;
                }
            }
        }
        catch (Exception)
        {
            IsUpdateSuc = false;
        }
        return IsUpdateSuc;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using DataBase.Data;
using PbProject.Model;

public partial class User_AutoTicketSetting : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            string sqlParams = " CpyNo = '" + mCompany.UninCode + "'";
            List<Bd_Base_Parameters> Bd_Base_ParametersList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { sqlParams }) as List<Bd_Base_Parameters>;
            if (Bd_Base_ParametersList == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('未获取到参数,请重新尝试或联系管理员');", true);
                return;
            }
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(Bd_Base_ParametersList);
            //if (!BS.KongZhiXiTong.Contains("|22|"))//自动出票开关关闭自动出票参数设置隐藏
            //{

            //    AutoCpSet.Style.Add("display", "none");
            //}
            //else
            //{
            CarrierBind(BS);
            AutoCpSet.Style.Add("display", "");
            //}


            if (BS.AutoPayAccount != "")
            {
                txtAutoCPAlipay.Text = BS.AutoPayAccount.Split('^')[2].Split('|')[0];//文本框赋值

                if (txtAutoCPAlipay.Text != "")
                {
                    txtAutoCPAlipay.ReadOnly = true;
                }

                if (BS.AutoPayAccount.Split('^')[3] != "无")
                {
                    txtChinanprAccount.Text = BS.AutoPayAccount.Split('^')[3].Split('|')[0];
                }
                if (BS.AutoPayAccount.Split('^')[3] != "无")
                {
                    txtChinapnrPwd.Text = BS.AutoPayAccount.Split('^')[3].Split('|')[1];
                    if (BS.AutoPayAccount.Split('^')[3].Split('|')[2] == "1")//信用支付
                    {
                        rbXinyong.Checked = true;
                    }
                    else
                    {
                        rbFukuan.Checked = true;
                    }
                }
                rbXinyong.Enabled = false;
                rbFukuan.Enabled = false;

                if (txtChinanprAccount.Text != "")
                {
                    txtChinanprAccount.ReadOnly = true;
                    txtChinapnrPwd.ReadOnly = true;
                }
            }
            txtfailcount.Text = BS.AutoPayAccount.Split('^')[1].ToString();
            if (BS.AutoPayAccount.Split('^')[0] == "1") //支付宝
            {
                rbAlipay.Checked = true;
            }
            else
            {
                rbChinapnr.Checked = true;
            }
        }
    }

    /// <summary>
    /// CarrierBind
    /// </summary>
    public void CarrierBind(PbProject.Model.definitionParam.BaseSwitch BS)
    {
        try
        {
            string[] CarrList = BS.AutoAccount.Split(new string[] { "^^^" }, StringSplitOptions.RemoveEmptyEntries);// Regex.Split(BS.AutoAccount, "^^^", RegexOptions.IgnoreCase);
            for (int i = 0; i < CarrList.Length; i++)
            {
                if (CarrList[i] != "")
                {
                    if (CarrList[i].Split(':')[0].ToString() == "CA")
                    {
                        txtCAaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtCApwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "MU")
                    {
                        txtMUaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtMUpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "HU")
                    {
                        txtHUaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtHUpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "ZH")
                    {
                        txtZHaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtZHpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "KN")
                    {
                        txtKNaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtKNpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "SC")
                    {
                        txtSCaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtSCpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "3U")
                    {
                        txt3Uaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txt3Upwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "CZ")
                    {
                        txtCZaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtCZpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "MF")
                    {
                        txtMFaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtMFpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "FM")
                    {
                        txtFMaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtFMpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "HO")
                    {
                        txtHOaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtHOpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "KY")
                    {
                        txtKYaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtKYpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "JR")
                    {
                        txtJRaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtJRpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "EU")
                    {
                        txtEUaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtEUpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "TV")
                    {
                        txtTVaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtTVpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }

                    else if (CarrList[i].Split(':')[0].ToString() == "GS")
                    {
                        txtGSaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtGSpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "JD")
                    {
                        txtJDaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtJDpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "PN")
                    {
                        txtPNaount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txtPNpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }
                    else if (CarrList[i].Split(':')[0].ToString() == "8L")
                    {
                        txt8Laount.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[0];
                        txt8Lpwd.Text = Regex.Split(CarrList[i].Split(':')[1], "//", RegexOptions.IgnoreCase)[1];
                    }

                }
            }
        }
        catch (Exception)
        {

        }
    }
    protected void lbAutoCPQY_Click(object sender, EventArgs e)
    {
        Response.Write("<script> window.open('https://mapi.alipay.com/gateway.do?service=alipay.dut.third.userext.nav&partner=2088701769940874&_input_charset=GBK&sign_type=MD5&biz_type=air_dut_third%3aDEFAULT&operate_type=sign&platform_pid=2088701598548382&sign=58ca7a7d0d254e29da1e573e11ebb7b6'); </script>");
    }
    protected void lbAutoCPSave_Click(object sender, EventArgs e)
    {
        string sqlParams = " CpyNo = '" + mCompany.UninCode + "'";
        List<Bd_Base_Parameters> Bd_Base_ParametersList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { sqlParams }) as List<Bd_Base_Parameters>;

        if (this.rbAlipay.Checked)
        {
            SaveAlipay(Bd_Base_ParametersList);
        }
        else if (this.rbChinapnr.Checked)
        {
            SaveChinapnr(Bd_Base_ParametersList);
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('您必须选择一种自动出票方式');", true);
        }
    }

    #region 支付宝

    /// <summary>
    /// 支付宝
    /// </summary>
    private void SaveAlipay(List<Bd_Base_Parameters> Bd_Base_ParametersList)
    {
        //Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('请保证要保存的支付宝已经签约第三方代扣和和航空公司 B2B 地址绑定，否则自动出票失败！');", true);
        try
        {
            string msg = "";
            string alipayAount = txtAutoCPAlipay.Text.Trim();

            if (Bd_Base_ParametersList == null)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('未获取到参数,请重新尝试或联系管理员');", true);
                return;
            }
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(Bd_Base_ParametersList);

            if (alipayAount == "")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('支付宝不能为空');", true);
                return;
            }
            //自动出票方式(1，支付宝本票通；2，汇付天下出票窗)^帐号|是否签约(1，已签约；2，未签)^帐号|密码|支付方式(1，信用账户；2，付款账户)
            string Bd_Base_Parameters_insertSQL = "update Bd_Base_Parameters set SetValue =" +
                        " '1^" + txtfailcount.Text.Trim() + "^" + alipayAount + "|1^无'" +
                        " where " +
                        " CpyNo = " + " '" + mCompany.UninCode + "' and " +
                        " SetName = " + " '" + PbProject.Model.definitionParam.paramsName.autoPayAccount + "' ";//自动支付参数
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            bool sss = sqlbase.ExecuteNonQuerySQLInfo(Bd_Base_Parameters_insertSQL);
            msg = sss ? "修改成功!" : "修改失败!";
            //if (BS.KongZhiXiTong.Contains("|22|"))
            //{
            updateCarrier();
            //}
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }
        catch (Exception)
        {

            throw;
        }

        //GetData();
    }
    #endregion


    #region 汇付天下

    /// <summary>
    /// 汇付天下
    /// </summary>
    private void SaveChinapnr(List<Bd_Base_Parameters> Bd_Base_ParametersList)
    {
        try
        {
            string msg = "";
            string account = this.txtChinanprAccount.Text;
            string pwd = this.txtChinapnrPwd.Text;
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(pwd))
            {
                ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('汇付天下账号密码不能为空');", true);
                return;
            }
            if (!this.rbXinyong.Checked && !this.rbFukuan.Checked)
            {
                ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('必须选择支付方式');", true);
                return;
            }
            string payType = "1";
            if (this.rbFukuan.Checked)
                payType = "2";
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(Bd_Base_ParametersList);
            //自动出票方式(1，支付宝本票通；2，汇付天下出票窗)^帐号|是否签约(1，已签约；2，未签)^帐号|密码|支付方式(1，信用账户；2，付款账户)
            string Bd_Base_Parameters_insertSQL = "update Bd_Base_Parameters set SetValue =" +
                        " '2^" + txtfailcount.Text.Trim() + "^无^" + account + "|" + pwd + "|" + payType + "'" +
                        " where " +
                        " CpyNo = " + " '" + mCompany.UninCode + "' and " +
                        " SetName = " + " '" + PbProject.Model.definitionParam.paramsName.autoPayAccount + "' ";//自动支付参数
            PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
            bool sss = sqlbase.ExecuteNonQuerySQLInfo(Bd_Base_Parameters_insertSQL);
            msg = sss ? "修改成功!" : "修改失败!";
            //if (BS.KongZhiXiTong.Contains("|22|"))
            //{
            updateCarrier();
            //}
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }
        catch (Exception)
        {

            throw;
        }

    }
    #endregion



    /// <summary>
    /// updateCarrier
    /// </summary>
    public void updateCarrier()
    {

        //格式：CA:xxx//xxx^^^CZ:xxx//xxx^^^MU:xxx//xxx
        string SetValue = "";

        if (txtCAaount.Text.Trim() != "" && txtCApwd.Text.Trim() != "")
        {
            SetValue += "^^^CA:" + txtCAaount.Text.Trim() + "//" + txtCApwd.Text.Trim() + "";
        }

        if (txtMUaount.Text.Trim() != "" && txtMUpwd.Text.Trim() != "")
        {
            SetValue += "^^^MU:" + txtMUaount.Text.Trim() + "//" + txtMUpwd.Text.Trim() + "";
        }

        if (txtHUaount.Text.Trim() != "" && txtHUpwd.Text.Trim() != "")
        {
            SetValue += "^^^HU:" + txtHUaount.Text.Trim() + "//" + txtHUpwd.Text.Trim() + "";
        }

        if (txtZHaount.Text.Trim() != "" && txtZHpwd.Text.Trim() != "")
        {
            SetValue += "^^^ZH:" + txtZHaount.Text.Trim() + "//" + txtZHpwd.Text.Trim() + "";
        }

        if (txtKNaount.Text.Trim() != "" && txtKNpwd.Text.Trim() != "")
        {
            SetValue += "^^^KN:" + txtKNaount.Text.Trim() + "//" + txtKNpwd.Text.Trim() + "";
        }

        if (txtSCaount.Text.Trim() != "" && txtSCpwd.Text.Trim() != "")
        {
            SetValue += "^^^SC:" + txtSCaount.Text.Trim() + "//" + txtSCpwd.Text.Trim() + "";
        }

        if (txt3Uaount.Text.Trim() != "" && txt3Upwd.Text.Trim() != "")
        {
            SetValue += "^^^3U:" + txt3Uaount.Text.Trim() + "//" + txt3Upwd.Text.Trim() + "";
        }

        if (txtCZaount.Text.Trim() != "" && txtCZpwd.Text.Trim() != "")
        {
            SetValue += "^^^CZ:" + txtCZaount.Text.Trim() + "//" + txtCZpwd.Text.Trim() + "";
        }

        if (txtMFaount.Text.Trim() != "" && txtMFpwd.Text.Trim() != "")
        {
            SetValue += "^^^MF:" + txtMFaount.Text.Trim() + "//" + txtMFpwd.Text.Trim() + "";
        }

        if (txtFMaount.Text.Trim() != "" && txtFMpwd.Text.Trim() != "")
        {
            SetValue += "^^^FM:" + txtFMaount.Text.Trim() + "//" + txtFMpwd.Text.Trim() + "";
        }

        if (txtHOaount.Text.Trim() != "" && txtHOpwd.Text.Trim() != "")
        {
            SetValue += "^^^HO:" + txtHOaount.Text.Trim() + "//" + txtHOpwd.Text.Trim() + "";
        }

        if (txtKYaount.Text.Trim() != "" && txtKYpwd.Text.Trim() != "")
        {
            SetValue += "^^^KY:" + txtKYaount.Text.Trim() + "//" + txtKYpwd.Text.Trim() + "";
        }

        if (txtJRaount.Text.Trim() != "" && txtJRpwd.Text.Trim() != "")
        {
            SetValue += "^^^JR:" + txtJRaount.Text.Trim() + "//" + txtJRpwd.Text.Trim() + "";
        }

        if (txtEUaount.Text.Trim() != "" && txtEUpwd.Text.Trim() != "")
        {
            SetValue += "^^^EU:" + txtEUaount.Text.Trim() + "//" + txtEUpwd.Text.Trim() + "";
        }

        if (txtTVaount.Text.Trim() != "" && txtTVpwd.Text.Trim() != "")
        {
            SetValue += "^^^TV:" + txtTVaount.Text.Trim() + "//" + txtTVpwd.Text.Trim() + "";
        }

        if (txtGSaount.Text.Trim() != "" && txtGSpwd.Text.Trim() != "")
        {
            SetValue += "^^^GS:" + txtGSaount.Text.Trim() + "//" + txtGSpwd.Text.Trim() + "";
        }
        if (txtJDaount.Text.Trim() != "" && txtJDpwd.Text.Trim() != "")
        {
            SetValue += "^^^JD:" + txtJDaount.Text.Trim() + "//" + txtJDpwd.Text.Trim() + "";
        }
        if (txtPNaount.Text.Trim() != "" && txtPNpwd.Text.Trim() != "")
        {
            SetValue += "^^^PN:" + txtPNaount.Text.Trim() + "//" + txtPNpwd.Text.Trim() + "";
        }
        if (txt8Laount.Text.Trim() != "" && txt8Lpwd.Text.Trim() != "")
        {
            SetValue += "^^^8L:" + txt8Laount.Text.Trim() + "//" + txt8Lpwd.Text.Trim() + "";
        }

        string Bd_Base_Parameters_insertSQL = "update Bd_Base_Parameters set SetValue =" +
                        " '" + SetValue + "'" +
                        " where " +
                        " CpyNo = " + " '" + mCompany.UninCode + "' and " +
                        " SetName = " + " '" + PbProject.Model.definitionParam.paramsName.autoAccount + "' ";//自动支付参数
        PbProject.Logic.SQLEXBLL.SQLEXBLL_Base sqlbase = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base();
        bool sss = sqlbase.ExecuteNonQuerySQLInfo(Bd_Base_Parameters_insertSQL);
        string msg = sss ? "修改成功!" : "修改失败!";

        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
    }
}
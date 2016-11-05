using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;

/// <summary>
/// 收款账号绑定
/// </summary>
public partial class Financial_ReceivablesBind : BasePage
{
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
                this.currentuserid.Value = this.mUser.id.ToString();
                UseReceivablesBind();
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    private void UseReceivablesBind()
    {
        try
        {
            string tempSqlWhere = " CpyNo ='" + mUser.CpyNo + "' and SetName='" + PbProject.Model.definitionParam.paramsName.wangYinZhangHao + "'";
            List<Bd_Base_Parameters> bParametersList = new PbProject.Logic.ControlBase.Bd_Base_ParametersBLL().GetParametersListByWhere(tempSqlWhere);

            if (bParametersList != null && bParametersList.Count == 1 && bParametersList[0] != null)
            {
                if (!string.IsNullOrEmpty(bParametersList[0].SetValue))
                {
                    #region 账号绑定

                    hidden_id.Value = bParametersList[0].id.ToString();
                    hidden_strValue.Value = bParametersList[0].SetValue;

                    string[] wangYinZhangHao = bParametersList[0].SetValue.Split('|');

                    for (int i = 0; i < wangYinZhangHao.Length; i++)
                    {
                        string[] wangYinZhangHaoValue = wangYinZhangHao[i].Split('^');

                        if (i == 0)
                        {
                            //支付宝
                            if (!string.IsNullOrEmpty(wangYinZhangHaoValue[0]))
                            {
                                txtZFB.Text = wangYinZhangHaoValue[0];
                                txtZFB.Enabled = true;
                                lbtnZFBSign.Visible = false;
                                btnZFBOK.Visible = false;
                            }
                            if (!string.IsNullOrEmpty(wangYinZhangHaoValue[1]))
                            {
                                txtZFB_Repay.Text = wangYinZhangHaoValue[1];
                                txtZFB_Repay.Enabled = true;
                                lbtnZFBSign_Repay.Visible = false;
                                btnZFBOK_Repay.Visible = false;
                            }
                            hidden_ZFB.Value = wangYinZhangHaoValue[2];
                        }
                        else if (i == 1)
                        {
                            //快钱
                            if (!string.IsNullOrEmpty(wangYinZhangHaoValue[0]))
                            {
                                txtKQ.Text = wangYinZhangHaoValue[0];
                                txtKQ.Enabled = true;
                                lbtnKQSign.Visible = false;
                                btnKQOK.Visible = false;
                            }
                            if (!string.IsNullOrEmpty(wangYinZhangHaoValue[1]))
                            {
                                txtKQ_Repay.Text = wangYinZhangHaoValue[1];
                                txtKQ_Repay.Enabled = true;
                                lbtnKQSign_Repay.Visible = false;
                                btnKQOK_Repay.Visible = false;
                            }
                            hidden_KQ.Value = wangYinZhangHaoValue[2];
                        }
                        else if (i == 2)
                        {
                            //汇付
                            if (!string.IsNullOrEmpty(wangYinZhangHaoValue[0]))
                            {
                                txtHF.Text = wangYinZhangHaoValue[0];
                                txtHF.Enabled = true;
                                lbtnHFSign.Visible = false;
                                btnHFOK.Visible = false;
                            }
                            if (!string.IsNullOrEmpty(wangYinZhangHaoValue[1]))
                            {
                                txtHF_Repay.Text = wangYinZhangHaoValue[1];
                                txtHF_Repay.Enabled = true;
                                lbtnHFSign_Repay.Visible = false;
                                btnHFOK_Repay.Visible = false;
                            }
                            hidden_HF.Value = wangYinZhangHaoValue[2];

                        }
                        else if (i == 3)
                        {
                            //财付通
                            if (!string.IsNullOrEmpty(wangYinZhangHaoValue[0]))
                            {
                                txtCFT.Text = wangYinZhangHaoValue[0];
                                txtCFT.Enabled = true;
                                lbtnCFTSign.Visible = false;
                                btnCFTOK.Visible = false;
                            }
                            if (!string.IsNullOrEmpty(wangYinZhangHaoValue[1]))
                            {
                                txtCFT_Repay.Text = wangYinZhangHaoValue[1];
                                txtCFT_Repay.Enabled = true;
                                lbtnCFTSign_Repay.Visible = false;
                                btnCFTOK_Repay.Visible = false;
                            }
                            hidden_CFT.Value = wangYinZhangHaoValue[2];
                        }
                    }

                    #endregion

                    if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
                    {
                        thid.Visible = true;
                        tdzfb.Visible = true;
                        tdkq.Visible = true;
                        tbhf.Visible = true;
                        tbcft.Visible = true;
                    }
                    else if (mCompany.RoleType == 4)
                    {
                        lblShouKuan.Text = "收款账号";
                    }
                    else if (mCompany.RoleType == 1)
                    {
                        thid.Visible = true;
                        tdzfb.Visible = true;
                        tdkq.Visible = true;
                        tbhf.Visible = true;
                        tbcft.Visible = true;

                        lblShouKuan.Text = "手续费收款账号";
                    }
                }
            }
            else
            {
                //没有数据  //返回上一页
                //tbReceivables.Visible = false;
            }
        }
        catch (Exception)
        {

        }
    }

    #region 支付宝

    /// <summary>
    /// 保存 支付收款账号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnZFBOK_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            if (!string.IsNullOrEmpty(txtZFB.Text.Trim()))
            {
                PbProject.Logic.Pay.AliPay aliPay = new PbProject.Logic.Pay.AliPay();
                if (aliPay.IsUserSign(txtZFB.Text.Trim()))
                {
                    msg = BtnSave(0, 0, txtZFB.Text.Trim());
                }
                else
                {
                    msg = "账号未签约！";
                }
            }
            else
            {
                msg = "请输入签约账号！";
            }
        }
        catch (Exception ex)
        {
            msg = "操作错误！";
        }

        if (msg.Contains("成功"))
        {

        }
        else 
        {
            lbtnZFBSign.Visible = true;
        }

        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
    }

    /// <summary>
    /// 保存 充值收款账号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnZFBOK_Repay_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            if (!string.IsNullOrEmpty(txtZFB_Repay.Text.Trim()))
            {
                PbProject.Logic.Pay.AliPay aliPay = new PbProject.Logic.Pay.AliPay();
                if (aliPay.IsUserSign(txtZFB_Repay.Text.Trim()))
                {
                    msg = BtnSave(0, 1, txtZFB_Repay.Text.Trim());
                }
                else
                {
                    msg = "账号未签约！";
                }
            }
            else
            {
                msg = "请输入签约账号！";
            }
        }
        catch (Exception ex)
        {
            msg = "操作错误！";
        }

        if (msg.Contains("成功"))
        {

        }
        else
        {
            lbtnZFBSign_Repay.Visible = true;
        }

        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
    }

    /// <summary>
    /// 签约 支付收款账号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnZFBSign_Click(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(txtZFB.Text.Trim()))
            {
                lbtnZFBSign.Visible = false;
                PbProject.Logic.Pay.AliPay aliPay = new PbProject.Logic.Pay.AliPay();
                Response.Write("<script> window.open('" + aliPay.GetUserSign(txtZFB.Text.Trim()) + "'); </script>");
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('请输入签约账号！');", true);
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('操作错误！');", true);
        }
    }

    /// <summary>
    /// 签约 充值收款账号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnZFBSign_Repay_Click(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(txtZFB_Repay.Text.Trim()))
            {
                lbtnZFBSign.Visible = false;
                PbProject.Logic.Pay.AliPay aliPay = new PbProject.Logic.Pay.AliPay();
                Response.Write("<script> window.open('" + aliPay.GetUserSign(txtZFB_Repay.Text.Trim()) + "'); </script>");
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('请输入签约账号！');", true);
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('操作错误！');", true);
        }
    }

    #endregion

    #region 快钱

    /// <summary>
    /// 支付收款账号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnKQOK_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            if (!string.IsNullOrEmpty(txtKQ.Text.Trim()))
            {
                // 快钱收款账号必须是邮箱

                bool result = System.Text.RegularExpressions.Regex.IsMatch(txtKQ.Text.Trim(),
    @"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (result == true)
                {
                    msg = BtnSave(1, 0, txtKQ.Text.Trim());
                }
                else
                {
                    msg = "快钱收款账号必须是邮箱";
                }
            }
            else
            {
                msg = "请输入快钱账号！";
            }
        }
        catch (Exception ex)
        {
            msg = "操作错误！";
        }
        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);

    }

    /// <summary>
    /// 充值收款账号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnKQOK_Repay_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            if (!string.IsNullOrEmpty(txtKQ_Repay.Text.Trim()))
            {
                bool result = System.Text.RegularExpressions.Regex.IsMatch(txtKQ_Repay.Text.Trim(),
@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (result == true)
                {
                    msg = BtnSave(1, 1, txtKQ_Repay.Text.Trim());
                }
                else
                {
                    msg = "快钱收款账号必须是邮箱";
                }
            }
            else
            {
                msg = "请输入快钱账号！";
            }
        }
        catch (Exception ex)
        {
            msg = "操作错误！";
        }
        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
    }

    #endregion

    #region 汇付

    /// <summary>
    /// 签约 支付收款账号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnHFSign_Click(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(txtHF.Text.Trim()))
            {
                //PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();
                //string val = chinaPnr.Sign(txtHF.Text.Trim(), txtHF.Text.Trim());
                //Response.Write("window.open('" + val + "'); ");
                lbtnHFSign.Visible = false;
                Response.Write("<script> window.open('ChinaPnrSign.aspx?user=" + txtHF.Text.Trim() + "&oper=" + txtHF.Text.Trim() + "&currentuserid="+this.currentuserid.Value.ToString()+"') </script>");
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('请输入汇付账号!');", true);
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('操作错误!');", true);
        }
    }

    /// <summary>
    /// 保存 支付收款账号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnHFOK_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            if (!string.IsNullOrEmpty(txtHF.Text.Trim()))
            {
                PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();
                if (chinaPnr.QuerySign(txtHF.Text.Trim(), txtHF.Text.Trim()))
                {
                    msg = BtnSave(2, 0, txtHF.Text.Trim());
                }
                else
                {
                    msg = "账号未签约！";
                }
            }
            else
            {
                msg = "请输入签约账号！";
            }
        }
        catch (Exception ex)
        {
            msg = "操作错误！";
        }

        if (msg.Contains("成功"))
        {

        }
        else
        {
            lbtnHFSign.Visible = true;
        }

        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);

    }

    /// <summary>
    /// 签约 充值收款账号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnHFSign_Repay_Click(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(txtHF_Repay.Text.Trim()))
            {
               // PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();
               // string val = chinaPnr.Sign(txtHF_Repay.Text.Trim(), txtHF_Repay.Text.Trim());
                //Response.Write("<script> window.open('" + aliPay.GetUserSign(txtHF_Repay.Text.Trim()) + "'); </script>");
                lbtnHFSign.Visible = false;
               // Response.Write("<script>window.open('" + val + "')</script>");
                string url = string.Format("hf.aspx?txt={0}", txtHF_Repay.Text);
                Response.Write("<script>window.open('" + url + "')</script>");
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('请输入汇付账号!');", true);
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('操作错误!');", true);
        }
    }

    /// <summary>
    /// 保存 充值收款账号
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnHFOK_Repay_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            if (!string.IsNullOrEmpty(txtHF_Repay.Text.Trim()))
            {
                PbProject.Logic.Pay.ChinaPnr chinaPnr = new PbProject.Logic.Pay.ChinaPnr();
                if (chinaPnr.QuerySign(txtHF_Repay.Text.Trim(), txtHF_Repay.Text.Trim()))
                {
                    msg = BtnSave(2, 1, txtHF_Repay.Text.Trim());
                }
                else
                {
                    msg = "账号未签约！";
                }
            }
            else
            {
                msg = "请输入签约账号！";
            }
        }
        catch (Exception ex)
        {
            msg = "操作错误！";
        }

        if (msg.Contains("成功"))
        {

        }
        else
        {
            lbtnHFSign_Repay.Visible = true;
        }

        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);

    }

    #endregion

    #region 财付通

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnCFTSign_Click(object sender, EventArgs e)
    {
                PbProject.Logic.Pay.TenPay tenPay = new PbProject.Logic.Pay.TenPay();
                string val = tenPay.TrustRefundRequest();
                lbtnCFTSign.Visible = false;
                Response.Write("<script>window.open('" + val + "') </script>");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnCFTSign_Repay_Click(object sender, EventArgs e)
    {
        PbProject.Logic.Pay.TenPay tenPay = new PbProject.Logic.Pay.TenPay();
        string val = tenPay.TrustRefundRequest();
        lbtnCFTSign_Repay.Visible = false;
        Response.Write("<script>window.open('" + val + "') </script>");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCFTOK_Click(object sender, EventArgs e)
    {
        string msg = string.Empty;
        try
        {
            string account = txtCFT.Text;
            if (!string.IsNullOrEmpty(account))
            {
                PbProject.Logic.Pay.TenPay tenPay = new PbProject.Logic.Pay.TenPay();
                int result = tenPay.TrustRequest(account);
                switch (result)
                {
                    case 0:
                        msg = "由于网络原因，请求出现异常！";
                        break;
                    case 1:
                        msg = BtnSave(3, 0, account);
                        break;
                    case 2:
                        msg = "该帐号未签约！";
                        lbtnCFTSign.Visible = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                msg = "请输入签约帐号!";
            }
        }
        catch (Exception)
        {
            msg = "操作错误！";
        }
        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCFTOK_Repay_Click(object sender, EventArgs e)
    {
        string msg = string.Empty;
        try
        {
            string account = txtCFT_Repay.Text;
            if (!string.IsNullOrEmpty(account))
            {
                PbProject.Logic.Pay.TenPay tenPay = new PbProject.Logic.Pay.TenPay();
                int result = tenPay.TrustRequest(account);
                switch (result)
                {
                    case 0:
                        msg = "由于网络原因，请求出现异常！";
                        break;
                    case 1:
                        msg = BtnSave(3, 1, account);
                        break;
                    case 2:
                        msg = "该帐号未签约！";
                        lbtnCFTSign_Repay.Visible = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                msg = "请输入签约帐号!";
            }
        }
        catch (Exception)
        {
            msg = "操作错误";
        }
        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
    }
    #endregion

    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="type">支付方式：0 支付宝，1 快钱，2 汇付，3 财付通</param>
    /// <param name="str">0 支付收款账号，1 充值收款账号</param>
    /// <param name="strValue"></param>
    /// <returns></returns>
    public string BtnSave(int type, int str, string strValue)
    {
        string msg = "";

        try
        {
            string[] strOldValue = hidden_strValue.Value.Trim().Split('|');
            string[] strValues = strOldValue[type].Split('^');

            string strNewTemp = "";
            if (str == 0)
                strNewTemp = strValue + "^" + strValues[1] + "^" + strValues[2] + "^" + strValues[3];
            else if (str == 1)
                strNewTemp = strValues[0] + "^" + strValue + "^" + strValues[2] + "^" + strValues[3];

            switch (type)
            {
                case 0://支付宝
                    strOldValue[0] = strNewTemp;
                    break;
                case 1://快钱
                    strOldValue[1] = strNewTemp;
                    break;
                case 2://汇付
                    strOldValue[2] = strNewTemp;
                    break;
                case 3://财付通
                    strOldValue[3] = strNewTemp;
                    break;
                default:
                    break;
            }

            string strNewValue = strOldValue[0] + "|" + strOldValue[1] + "|" + strOldValue[2] + "|" + strOldValue[3];

            HashObject paramter = new HashObject();
            paramter.Add("id", hidden_id.Value);
            paramter.Add("SetValue", strNewValue);
            bool result = (bool)baseDataManage.CallMethod("Bd_Base_Parameters", "Update", null, new Object[] { paramter });

            if (result == true)
            {
                UseReceivablesBind();
                msg = "保存成功！";
            }
            else
                msg = "保存失败！";
        }
        catch (Exception)
        {
            msg = "操作错误！";
        }
        return msg;
    }
}
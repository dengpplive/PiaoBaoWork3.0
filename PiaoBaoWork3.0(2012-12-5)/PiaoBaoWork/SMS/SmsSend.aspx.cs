using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;
using System.Data;
using PbProject.Model;

public partial class SMS_SmsSend : BasePage
{
    protected string smsCount = "0";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                this.currentuserid.Value = this.mUser.id.ToString();
                //订单详情过来的发短信的订单
                if (Request["OrderId"]!=null)
                {
                    txtorderno.Text = Request["OrderId"].ToString();
                }
                if (Request["phone"]!=null)
                {
                    txtnum.Value = Request["Phone"].ToString();
                }
                if (mCompany.RoleType == 1)
                {
                    this.spansycount.Visible = false;//剩余条数隐藏
                }
                if (mCompany.RoleType == 2 || mCompany.RoleType == 5)
                {
                    lkbtxj.Visible = false;//选择下级隐藏
                }
                this.txthz.Text = mCompany.UninAllName.ToString();//默认公司名
                string tels = Request["tels"] == null ? "" : Request["tels"].ToString();
                if (tels.Length > 0)
                {
                    this.txtnum.Value = tels.Replace("，", ",");
                }
                Bindddl();

                #region 短信余额查询 只能平台查询

                if (mCompany.RoleType == 1)
                    spanSel.Visible = true; //短信余额查询

                #endregion
            }
            List<Tb_Sms_User> list = baseDataManage.CallMethod("Tb_Sms_User", "GetList", null, new Object[] { "CpyNo='" + mCompany.UninCode + "'" }) as List<Tb_Sms_User>;
            if (list != null && list.Count > 0)
            {
                lblmsgcount.Text = list[0].SmsRemainCount.ToString();
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('加载数据错误！')</script>", false);
            return;
        }
    }
    /// <summary>
    /// 绑定用户组，短信模板
    /// </summary>
    protected void Bindddl()
    {
        try
        {
            string strWhere = "";

            ddltemplate.Items.Clear();
            strWhere = ddltemptype.SelectedValue.ToString() == "0" ? "SmsTpType=0" : "SmsTpType=1 and CpyNo='" + mCompany.UninCode + "'";
            IList<Tb_Sms_Template> templatelist = baseDataManage.CallMethod("Tb_Sms_Template", "GetList", null, new Object[] { strWhere }) as List<Tb_Sms_Template>;
            ddltemplate.Items.Add(new ListItem("--选择模板--", "-1"));
            for (int i = 0; i < templatelist.Count; i++)
            {
                ddltemplate.Items.Add(new ListItem(templatelist[i].SmsTpName.ToString(), templatelist[i].id.ToString()));
            }
        }
        catch (Exception)
        {

            throw;
        }

    }
    /// <summary>
    /// 选择常旅客
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lkbtclk_Click(object sender, EventArgs e)
    {
        Response.Redirect("SmsUserGroupManager.aspx?cpyno=" + mCompany.UninCode + "&currentuserid=" + this.currentuserid.Value.ToString());
    }
    /// <summary>
    /// 选择下级用户
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lkbtxj_Click(object sender, EventArgs e)
    {
        Response.Redirect("SmsCpyGroup.aspx?currentuserid=" + this.currentuserid.Value.ToString());
    }
    /// <summary>
    /// 选择模板
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddltemplate_SelectedIndexChanged(object sender, EventArgs e)
    {
        string templateid = ddltemplate.SelectedItem.Value.ToString();
        if (templateid != "-1")
        {
            Tb_Sms_Template mTemplate = baseDataManage.CallMethod("Tb_Sms_Template", "GetById", null, new object[] { templateid.ToString() }) as Tb_Sms_Template;
            this.txtContents.Value = mTemplate.SmsTpContent;
        }
        else
        {
            this.txtContents.Value = "";
        }
    }
    /// <summary>
    /// 根据模板类型显示订单框
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddltemptype_SelectedIndexChanged(object sender, EventArgs e)
    {
        Bindddl();
    }
    /// <summary>
    /// 根据订单号导入内容
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btdrcontents_Click(object sender, EventArgs e)
    {
        if (ddltemplate.SelectedValue != "-1")
        {

            if (txtorderno.Text == "")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('请填写订单号！')</script>", false);
                return;
            }
            IList<Tb_Ticket_Order> OrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { "OrderId='" + txtorderno.Text.Trim() + "'" }) as List<Tb_Ticket_Order>;
            if (OrderList != null && OrderList.Count > 0)
            {
                IList<Tb_Ticket_SkyWay> skyWayList = baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new Object[] { "OrderId='" + OrderList[0].OrderId + "'" }) as List<Tb_Ticket_SkyWay>;
                if (ddltemplate.SelectedValue != "-1")
                {
                    string ToDate = "未知";
                    string CarryName = "";
                    if (skyWayList != null && skyWayList.Count > 0)
                    {
                        ToDate = skyWayList[0].ToDate.ToString("MM月dd日HH:mm");
                        CarryName = skyWayList[0].CarryName;
                    }
                    Tb_Sms_Template mTemp = baseDataManage.CallMethod("Tb_Sms_Template", "GetById", null, new object[] { ddltemplate.SelectedValue.ToString() }) as Tb_Sms_Template;
                    this.txtContents.Value = mTemp.SmsTpContent.ToString().Replace("[订单号]", OrderList[0].OrderId).Replace("[乘机人]", OrderList[0].PassengerName).Replace("[订单编号]", OrderList[0].OrderId).Replace("[起抵城市]", OrderList[0].Travel).Replace("[起飞城市]", OrderList[0].Travel.Split('-')[0]).Replace("[到达城市]", OrderList[0].Travel.Split('-')[1]).Replace("[起飞时间]", OrderList[0].AirTime.ToString("MM月dd日HH:mm")).Replace("[起飞日期]", OrderList[0].AirTime.ToString("MM月dd日HH:mm")).Replace("[到达日期]", ToDate).Replace("[到达时间]", ToDate).Replace("[航班号]", CarryName + OrderList[0].FlightCode);
                    if (string.IsNullOrEmpty(Request["phone"]))
                    {
                        this.txtnum.Value = OrderList[0].LinkManPhone.ToString();
                    }
                    
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('模板出错！')</script>", false);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('暂无此订单信息')</script>", false);
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('请选择模板')</script>", false);
        }
    }
    /// <summary>
    /// 导入电话
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btdy_Click(object sender, EventArgs e)
    {
        string savename = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(this.FileUpload1.FileName).ToLower();
        if (FileUpload1.FileName != "")
        {
            string fex = Path.GetExtension(FileUpload1.FileName).ToLower();
            if (fex != ".txt")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('文件格式应为txt')</script>", false);
                return;
            }
        }
        string UpFilePath = Server.MapPath("~/SMS/" + savename);
        FileUpload1.SaveAs(UpFilePath);

        try
        {

            StreamReader myreader = new StreamReader(UpFilePath, System.Text.Encoding.GetEncoding("gb2312"));
            txtnum.Value = myreader.ReadToEnd().ToString().Replace('，', ',');
            myreader.Close();
            File.Delete(UpFilePath);
        }
        catch (Exception)
        {
            throw;
        }

    }
    /// <summary>
    /// 信息发送
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btsendmsg_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            bool rs = false;
            string[] phones = txtnum.Value.ToString().Replace("\r\n", "").Trim().Replace('，', ',').Split(new char[] { ',' });
            if (txtnum.Value.Trim().Length > 0)
            {
                for (int i = 0; i < phones.Length; i++)
                {
                    if (!Regex.IsMatch(phones[i], "(^(0\\d{2,3}-?\\d{7,8})$)|(^1[358]\\d{9}$)"))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('手机号长度有误:" + phones[i] + "多个号码之间请用英文逗号隔开')</script>", false);
                        return;
                    }
                    else if (phones.Length > 100)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('手机号码最多为100个')</script>", false);
                        return;
                    }
                    else
                    {
                        rs = true;
                    }
                }
            }
            if (txtnum.Value.Trim().ToString().Length < 1)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('请输入手机号')</script>", false);
                return;
            }
            if (txtContents.Value.Length > 400 || txtContents.Value.Trim().Length < 1)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('短信内容为空或字符过长，最多400个汉字！')</script>", false);
                return;
            }
            if (txthz.Text.ToString().Length > 25 || txthz.Text.ToString().Length < 1)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('后缀为空或字符过长，最多25个汉字！')</script>", false);
                return;
            }
            //string cheackkeyword = PiaoBao.BLLLogic.Common.SelectKeyWord(txtContents.Value.ToString()).ToString();
            //if (cheackkeyword.Length > 0 && cheackkeyword != "nulls")
            //{
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('内容存在非法字词！[" + cheackkeyword + "]')</script>", false);
            //    return;
            //}
            if (rs == true)
            {
                //判断剩余条数是否足够发送
                int tcount = txtContents.Value.Length;
                decimal count = tcount / 64m;
                int count2 = (tcount / 64) + 1;
                decimal newCount = count2 > count ? count2 : count;
                decimal newTotalCount = newCount * phones.Length;
                IList<Tb_Sms_User> smsuser = baseDataManage.CallMethod("Tb_Sms_User", "GetList", null, new Object[] { "CpyNo='" + mUser.CpyNo + "'" }) as List<Tb_Sms_User>;
                #region 发送记录
                Tb_Sms_SendInfo sendinfo = new Tb_Sms_SendInfo();
                sendinfo.CpyNo = mUser.CpyNo;
                sendinfo.SmsSendContent = txtContents.Value.ToString();
                sendinfo.SmsSuffix = txthz.Text.ToString();
                sendinfo.SmsUnit = mCompany.UninAllName;
                sendinfo.SmsUserCount = short.Parse(newCount.ToString());
                #endregion
                if ((smsuser != null && smsuser.Count > 0) || mCompany.RoleType == 1)
                {
                    if ((smsuser != null && smsuser.Count > 0 && smsuser[0].SmsRemainCount >= newTotalCount) || mCompany.RoleType == 1)
                    {
                        cn.woxp.gateway.WebSMS mess2 = new cn.woxp.gateway.WebSMS();
                        string[] PhoneNums = txtnum.Value.ToString().Replace("，", ",").Trim().Split(',');
                        int countok = 0, counterr = 0;
                        for (int i = 0; i < PhoneNums.Length; i++)
                        {
                            sendinfo.SmsAcceptMobilePhone = PhoneNums[i].ToString();
                            cn.woxp.gateway.SendResult res = mess2.FastSendLongSMS("10416-ltj023-e076c91e4abca0b569beba646e3ef0ad-300", PhoneNums[i].ToString(), txtContents.Value.ToString() + "【" + this.txthz.Text.ToString() + "】", "", "");
                            if (res.ErrorDesc.ToString() == "提交成功")
                            {
                                if (mCompany.RoleType != 1)
                                {
                                    smsuser[0].SmsRemainCount = Convert.ToInt16(smsuser[0].SmsRemainCount - newCount);
                                    new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteNonQuerySQLInfo("update Tb_Sms_User set SmsRemainCount=" + smsuser[0].SmsRemainCount + " where CpyNo='" + mUser.CpyNo + "'");
                                }
                                sendinfo.SmsCreateDate = DateTime.Parse(DateTime.Now.ToString());
                                sendinfo.SmsSendState = 1;//发送状态
                                baseDataManage.CallMethod("Tb_Sms_SendInfo", "Insert", null, new Object[] { sendinfo });
                                countok++;
                            }
                            else
                            {
                                sendinfo.SmsCreateDate = DateTime.Parse(DateTime.Now.ToString());
                                sendinfo.SmsSendState = 0;//发送状态 失败
                                baseDataManage.CallMethod("Tb_Sms_SendInfo", "Insert", null, new Object[] { sendinfo });
                                counterr++;
                            }
                        }
                        msg = "成功发送:" + countok + "条,发送失败:" + counterr + "条";
                    }
                    else
                    {
                        msg = "短信条数不足";
                    }
                }
                else
                {
                    msg = "发送失败";
                }

            }
        }
        catch (Exception ex)
        {
            msg = "发送失败";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
    /// <summary>
    /// 获取公司名
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckBox1.Checked == true)
        {
            this.txthz.Text = mCompany.UninAllName;
            this.txthz.ReadOnly = true;
        }
        else
        {
            this.txthz.ReadOnly = false;
        }
    }
    /// <summary>
    /// 短信余额查询  只能平台查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSel_Click(object sender, EventArgs e)
    {
        try
        {
            string msg = "";

            if (mCompany.RoleType == 1)
            {
                cn.woxp.gateway.WebSMS webSMS = new cn.woxp.gateway.WebSMS();
                msg = "账号条数(GetBalanceCount):" + webSMS.GetBalanceCount("10416-ltj023-e076c91e4abca0b569beba646e3ef0ad-300");
                msg += " / ";
                msg += "账号余额(GetMoney):" + webSMS.GetMoney("10416-ltj023-e076c91e4abca0b569beba646e3ef0ad-300");
            }
            else
            {
                msg = "不能查询";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
        }
        catch (Exception)
        {

        }
    }
}


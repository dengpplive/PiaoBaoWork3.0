using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;

public partial class SMS_SmsGive : BasePage
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
       
    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        string msg = "";
        string CpyNo = Request["cpyno"] == null ? "" : Request["cpyno"].ToString();
        int givecount = int.Parse(txtcount.Text);//送给下级的条数
        //运营商条数
        List<Tb_Sms_User> listSmsUser = baseDataManage.CallMethod("Tb_Sms_User", "GetList", null, new Object[] { "CpyNo='" + mUser.CpyNo + "'" }) as List<Tb_Sms_User>;
        if (listSmsUser != null && listSmsUser.Count>0 && CpyNo!="")
        {
            
            if (listSmsUser[0].SmsRemainCount>= givecount)
            {
                try
                {
                    string tempSQl = "";
                    List<string> sqlList = new List<string>();
                    #region 1.修改短信用户信息(增加剩余条数)
                    tempSQl = "Update Tb_Sms_User set SmsCount=SmsCount+" + givecount + ",SmsRemainCount=SmsRemainCount+" + givecount + " where CpyNo='" + CpyNo + "'";
                    sqlList.Add(tempSQl);
                    #endregion

                    #region 2.修改运营商剩余条数（减少）
                    tempSQl = "Update Tb_Sms_User set SmsRemainCount=SmsRemainCount-" + givecount + " where CpyNo='" + mCompany.UninCode + "'";
                    sqlList.Add(tempSQl);

                    #endregion
                    bool rs = new PbProject.Dal.ControlBase.BaseData<Tb_Sms_User>().ExecuteSqlTran(sqlList);
                    if (rs == true)
                    {
                        Log_Operation logoper = new Log_Operation();
                        logoper.ModuleName = "短信赠送";
                        logoper.LoginName = mUser.LoginName;
                        logoper.UserName = mUser.UserName;
                        logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
                        logoper.CpyNo = mCompany.UninCode;
                        logoper.OperateType = "短信赠送";
                        logoper.OptContent = "赠送给公司编号为【"+CpyNo+"】,赠送短信条数【"+givecount+"】条";
                        new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);//日志
                        msg = "赠送成功！";
                    }
                    else
                    {
                        msg = "失败！";
                    }
                }
                catch (Exception)
                {
                    msg = "Error";
                }
            }
            else
            {
                msg = "运营商短信条数不足！";
            }
        }
        else
        {
            msg = "Get Date Error";
        }
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('" + msg + "')</script>", false);
    }
}
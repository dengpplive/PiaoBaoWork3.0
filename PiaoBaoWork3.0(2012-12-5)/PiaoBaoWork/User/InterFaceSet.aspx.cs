using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.WebCommon.Utility;
using System.Data;
using PbProject.Model;

public partial class User_InterFaceSet : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (mUser.IsAdmin!=0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('需管理员权限才能设置接口账号');", true);
                lbtnOK.Visible = false;
            }
            else
            {
                BindInterFaceInfo();
            }
        }
    }
    /// <summary>
    /// 获取当前登录公司参数信息(运营)
    /// </summary>
    protected void BindInterFaceInfo()
    {
        //接口账号
        List<Bd_Base_Parameters> listParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + mUser.CpyNo + "'" }) as List<Bd_Base_Parameters>;
        PbProject.Model.definitionParam.BaseSwitch pmdb = PbProject.WebCommon.Utility.BaseParams.getParams(listParameters);
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
    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnOK_Click(object sender, EventArgs e)
    {
        //接口账号集合
        string sql = "";
        string setvalues = txtJKact517.Text + "^" + txtJKpwd517.Text + "^" + txtJKkey517.Text + "^" + txtyckack517.Text + "^" + txtyckpwd517.Text + "|" +
           txtJKact51book.Text + "^" + txtJKpwd51book.Text + "^" + txtJKkey51book.Text + "^" + txtNoticeURL51book.Text + "|" +
           txtJKactBT.Text + "^" + txtJKpwdBT.Text + "^" + txtJKkeyBT.Text + "|" +
           txtJKactPM.Text + "^" + txtJKpwdPM.Text + "^" + txtJKkeyPM.Text + "|" +
           txtJKactJR.Text + "^" + txtJKpwdJR.Text + "|" +
           txtJKact8000yi.Text + "^" + txtJKpwd8000yi.Text + "^" + txtJKDKZFB8000yi.Text + "|" + txtyixing.Text + "^" + txtyixinggy.Text;
        sql = GetParameterUpSql(setvalues, mCompany.UninCode, PbProject.Model.definitionParam.paramsName.jieKouZhangHao);
        string msg = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteNonQuerySQLInfo(sql) == true ? "设置成功" : "设置失败";
        if (msg=="设置成功")
        {
            //日志
            Log_Operation logoper = new Log_Operation();
            logoper.ModuleName = "接口账号设置";
            logoper.LoginName = mUser.LoginName;
            logoper.UserName = mUser.UserName;
            logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
            logoper.CpyNo = mCompany.UninCode;
            logoper.OperateType = "接口账号设置";
            logoper.OptContent = "修改前:" + BaseParams.getParams(baseParametersList).JieKouZhangHao+"//////////修改后："+setvalues;
            new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);
        }
        
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
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
    /// <summary>
    /// 八千翼签约
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lk8000yiZFBSigning_Click(object sender, EventArgs e)
    {
        try
        {
            w_8000YService.W8000YService WSvc8000Y = new w_8000YService.W8000YService();
            PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(baseParametersList);
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
}
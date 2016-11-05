using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;
using System.Data;
public partial class Policy_drawBillTimel:BasePage
{
    //PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            string cpyno =mCompany.UninCode.Substring(0, 12);

            string sql1 = " 1=1 and CpyNo='" + cpyno + "' and SetName='" + PbProject.Model.definitionParam.paramsName.chuPiaoShiJian + "'";
            List<Bd_Base_Parameters> listcpy = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { sql1 }) as List<Bd_Base_Parameters>;
            string sql2 = "select PolicySource,SUM(CONVERT(int,isnull(DATEDIFF(mi,PayTime,CPTime),0)))/COUNT(1) as chuTime from Tb_Ticket_Order where "
                + " 1=1 and CPCpyNo='" + cpyno + "' and "
                + " OrderStatusCode=4 and CPTime>'" + DateTime.Now.AddDays(-30) + "' and PayTime>'" + DateTime.Now.AddDays(-30) + "' and PayStatus=1"
                + " and PayTime<=CPTime"
                + " group by PolicySource ";

            DataTable dt = baseDataManage.ExecuteStrSQL(sql2);
            if (listcpy != null && listcpy.Count > 0)
            {
                //0-B2B|1-BSP|2-517|3-百拓|4-8000|5-今日|6-票盟|7-51book|8-共享|9-易行
                string[] chuPiaos = listcpy[0].SetValue.Split('|');
                txtB2B.Text = checkString(chuPiaos, 0);
                txtBSP.Text = checkString(chuPiaos, 1);
                txt517.Text = checkString(chuPiaos, 2);
                txtBaiTuo.Text = checkString(chuPiaos, 3);
                txt8000Y.Text = checkString(chuPiaos, 4);
                txtToday.Text = checkString(chuPiaos, 5);
                txtPiaoMen.Text = checkString(chuPiaos, 6);
                txt51Book.Text = checkString(chuPiaos, 7);
                txtGongXiang.Text = checkString(chuPiaos, 8);
                txtYeeXing.Text = checkString(chuPiaos, 9);


                lblB2B.Text = checkString(chuPiaos, 0);
                lblBSP.Text = checkString(chuPiaos, 1);
                lbl517.Text = checkString(chuPiaos, 2);
                lblBaiTuo.Text = checkString(chuPiaos, 3);
                lbl8000Y.Text = checkString(chuPiaos, 4);
                lblToday.Text = checkString(chuPiaos, 5);
                lblPiaoMen.Text = checkString(chuPiaos, 6);
                lbl51Book.Text = checkString(chuPiaos, 7);
                lblGongXiang.Text = checkString(chuPiaos, 8);
                lblYeeXing.Text = checkString(chuPiaos, 9);
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string policySource = dt.Rows[i]["PolicySource"].ToString();
                    string chuPiaotime = dt.Rows[i]["chuTime"].ToString();
                    switch (policySource)
                    {
                        case "1":
                            lblB2B.Text = chuPiaotime;
                            break;
                        case "2":
                            lblBSP.Text = chuPiaotime;
                            break;
                        case "3":
                            lbl517.Text = chuPiaotime;
                            break;
                        case "4":
                            lblBaiTuo.Text = chuPiaotime;
                            break;
                        case "5":
                            lbl8000Y.Text = chuPiaotime;
                            break;
                        case "6":
                            lblToday.Text = chuPiaotime;
                            break;
                        case "7":
                            lblPiaoMen.Text = chuPiaotime;
                            break;
                        case "8":
                            lbl51Book.Text = chuPiaotime;
                            break;
                        case "9":
                            lblGongXiang.Text = chuPiaotime;
                            break;
                        case "10":
                            lblYeeXing.Text = chuPiaotime;
                            break;
                        default:
                            break;
                    }
                }
            }

        }
    }
    private string checkString(string[] strs, int num)
    {
        string rs = "";
        try
        {
            rs = strs[num];
        }
        catch (Exception)
        {

            rs = "";
        }
        return rs;
    }
    protected void lbtnOk_Click(object sender, EventArgs e)
    {
        string cpyno = mCompany.UninCode.Substring(0, 12);
        string chuPiaoTime = txtB2B.Text + "|" + txtBSP.Text + "|" + txt517.Text + "|" + txtBaiTuo.Text + "|" + txt8000Y.Text + "|" + txtToday.Text + "|" + txtPiaoMen.Text + "|" + txt51Book.Text + "|" + txtGongXiang.Text + "|" + txtYeeXing.Text;
        string sql = "update Bd_Base_Parameters set setValue='" + chuPiaoTime + "' where setName='" + PbProject.Model.definitionParam.paramsName.chuPiaoShiJian + "'  and cpyno='" + cpyno + "'";
        string msg = "更新失败"; ;
        bool rs = baseDataManage.ExecuteNonQuerySQLInfo(sql);
        if (rs)
        {
            msg = "更新成功";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialogmsg('" + msg + "');", true);
    }
}
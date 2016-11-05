using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using PbProject.Logic.ControlBase;
/// <summary>
/// 机型
/// </summary>
public partial class AircraftEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            if (Request.QueryString["id"] != null && Request.QueryString["id"].ToString() != "")
            {
                bind(Request.QueryString["id"].ToString());
            }
        }
    }
    /// <summary>
    /// 添加承运人
    /// </summary>
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Bd_Air_Aircraft bd_air_aircraft = null;
        if (Request.QueryString["id"] != null && Request.QueryString["id"].ToString() != "")
        {
            //修改
            bd_air_aircraft = baseDataManage.CallMethod("Bd_Air_Aircraft", "GetById", null, new object[] { Request.QueryString["id"].ToString() }) as Bd_Air_Aircraft;
        }
        else
        {
            bd_air_aircraft = new Bd_Air_Aircraft();
        }
        //国内基建
        decimal ABFeeN = 0m;
        //国外基建
        decimal ABFeeW = 0m;
        if (!decimal.TryParse(txtJJN.Text, out  ABFeeN))
        {
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialog3('输入国内机建格式错误！');", true);
            return;
        }
        if (!decimal.TryParse(txtJJW.Text, out ABFeeW))
        {
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialog3('输入国外机建格式错误！');", true);
            return;
        }
        bd_air_aircraft.ABFeeN = ABFeeN;
        bd_air_aircraft.ABFeeW = ABFeeW;
        bd_air_aircraft.Aircraft = txtJiXing.Text.Trim().Replace("'", "");
        if (Request.QueryString["id"] != null && Request.QueryString["id"].ToString() != "")
        {
            bool IsSuc = (bool)baseDataManage.CallMethod("Bd_Air_Aircraft", "Update", null, new object[] { bd_air_aircraft });
            if (IsSuc)
            {
                ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialog3('修改成功！',1);", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialog3('修改失败！');", true);
            }
        }
        else
        {
            string SQlWhere = string.Format("Aircraft='{0}'", txtJiXing.Text.Trim().Replace("'", ""));
            bool IsExist = (bool)baseDataManage.CallMethod("Bd_Air_Aircraft", "IsExist", null, new object[] { SQlWhere });
            if (!IsExist)
            {
                //添加          
                bool IsSuc = (bool)baseDataManage.CallMethod("Bd_Air_Aircraft", "Insert", null, new object[] { bd_air_aircraft });
                if (IsSuc)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialog3('添加成功！',1);", true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialog3('添加失败！');", true);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialog3('该机型已存在,不能添加重复机型,添加失败！');", true);
            }
        }
    }
    public void bind(string id)
    {
        Bd_Air_Aircraft u = baseDataManage.CallMethod("Bd_Air_Aircraft", "GetById", null, new object[] { Request.QueryString["id"].ToString() }) as Bd_Air_Aircraft;
        if (u != null)
        {
            txtJiXing.Text = u.Aircraft;
            txtJJN.Text = u.ABFeeN + "";
            txtJJW.Text = u.ABFeeW + "";
        }
    }
}
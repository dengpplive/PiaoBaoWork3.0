using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Dal.ControlBase;
using PbProject.Model;
using DataBase.Data;
using DataBase.Unique;
using PbProject.Logic.ControlBase;
/// <summary>
/// 航空公司
/// </summary>
public partial class Sys_AirlineCompanyEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] != null && Request.QueryString["id"].ToString() != "")
        {
            if (!IsPostBack)
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
        string msg = "";
        Bd_Air_Carrier bd_air_carrier = null;
        try
        {
            if (Request.QueryString["id"] != null)
            {
                bd_air_carrier = baseDataManage.CallMethod("Bd_Air_Carrier", "GetById", null, new object[] { Request.QueryString["id"].ToString() }) as Bd_Air_Carrier;
            }
            else
            {
                bd_air_carrier = new Bd_Air_Carrier();
            }
            //获取实体
            bd_air_carrier.AirName = txtAriName.Text.Trim().Replace("'", "");
            bd_air_carrier.Code = txtCode.Text.Trim().Replace("'", "");
            bd_air_carrier.ShortName = txtShortName.Text.Trim().Replace("'", "");
            bd_air_carrier.Type = int.Parse(rblGNGJ.SelectedValue);
            bd_air_carrier.A1 = int.Parse(radioXS.SelectedValue);

            if (Request.QueryString["id"] != null && Request.QueryString["id"].ToString() != "")
            {
                //修改
                msg = (bool)baseDataManage.CallMethod("Bd_Air_Carrier", "Update", null, new object[] { bd_air_carrier }) == true ? "修改成功" : "修改失败";
                
            }
            else
            {
                List<Bd_Air_Carrier> list = baseDataManage.CallMethod("Bd_Air_Carrier", "GetList", null, new Object[] { "Code='" + txtCode.Text + "'" }) as List<Bd_Air_Carrier>;
                if (list != null && list.Count > 0)
                {
                    msg = "此航空公司已存在";
                }
                else
                {
                    //添加
                    msg = (bool)baseDataManage.CallMethod("Bd_Air_Carrier", "Insert", null, new object[] { bd_air_carrier }) == true ? "添加成功" : "添加失败";
                    
                }

            }
        }
        catch (Exception)
        {

            msg = "操作异常";
        }
        
        ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('"+msg+"');", true);
    }

    public void bind(string id)
    {
        //获取实体
        Bd_Air_Carrier u = baseDataManage.CallMethod("Bd_Air_Carrier", "GetById", null, new object[] { Request.QueryString["id"].ToString() }) as Bd_Air_Carrier;
        if (u != null)
        {
            txtAriName.Text = u.AirName;
            txtCode.Text = u.Code;
            txtShortName.Text = u.ShortName;
            rblGNGJ.SelectedIndex = rblGNGJ.Items.IndexOf(rblGNGJ.Items.FindByValue(u.Type + ""));
            radioXS.SelectedIndex = (int)u.A1;
        }
    }
}
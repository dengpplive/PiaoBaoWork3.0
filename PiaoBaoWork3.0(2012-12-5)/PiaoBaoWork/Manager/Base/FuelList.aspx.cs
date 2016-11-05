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
using System.Text;
using PbProject.Logic.ControlBase;
public partial class Sys_FuelList : BasePage
{        
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            repFuelListDataBind("Load");
        }
    }
    private void repFuelListDataBind(string str)
    {
        //燃油        
        Bd_Base_Parameters[] list = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { " setname='Base_Oil'" }) as Bd_Base_Parameters[];
        if (list.Length > 0)
        {
            Bd_Base_Parameters bd_base_parameters = list[0];
            string strVal = bd_base_parameters.SetValue;
            //成人800公里以上/一下  儿童800公里以上/一下  
            string[] strData = strVal.Split('|');
            if (strData.Length == 6)
            {
                if (str == "Load")
                {
                    div_ExceedAdultFee.InnerText = strData[1];
                    div_LowAdultFee.InnerText = strData[0];

                    div_ExceedChildFee.InnerText = strData[2];
                    div_LowChildFee.InnerText = strData[3];

                    div_StartTime.InnerText = strData[4];
                    div_EndTime.InnerText = strData[5];
                }
                else
                {
                    txtExceedAdultFee.Text = strData[1];
                    txtLowAdultFee.Text = strData[0];
                    txtExceedChildFee.Text = strData[2];
                    txtLowChildFee.Text = strData[3];
                    txtStartDate.Value = strData[4];
                    txtEndDate.Value = strData[5];
                }
            }
        }
    }

    public bool IsNull()
    {
        bool IsNull = false;
        if (string.IsNullOrEmpty(txtLowAdultFee.Text.Trim()) ||
            string.IsNullOrEmpty(txtLowChildFee.Text.Trim()) ||
            string.IsNullOrEmpty(txtExceedAdultFee.Text.Trim()) ||
            string.IsNullOrEmpty(txtExceedChildFee.Text.Trim()) ||
            string.IsNullOrEmpty(txtEndDate.Value.Trim()) ||
            string.IsNullOrEmpty(txtStartDate.Value.Trim()))
        {
            IsNull = true;
        }
        return IsNull;
    }

    //添加
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (IsNull())
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('输入数据不能为空！');", true);
            return;
        }
        Bd_Base_Parameters bd_base_parameters = null;
        //Bd_Base_Parameters[] list = Manage.GetBySQLArray(" setname='Base_Oil'");
        Bd_Base_Parameters[] list = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { " setname='Base_Oil'" }) as Bd_Base_Parameters[];
        if (list.Length > 0)
        {
            //修改
            bd_base_parameters = list[0];
        }
        else
        {
            //添加
            bd_base_parameters = new Bd_Base_Parameters();
            bd_base_parameters.SetName = "Base_Oil";
        }
        StringBuilder sbVal = new StringBuilder();
        sbVal.Append(txtExceedAdultFee.Text.Trim() + "|");
        sbVal.Append(txtLowAdultFee.Text.Trim() + "|");
        sbVal.Append(txtExceedChildFee.Text.Trim() + "|");
        sbVal.Append(txtLowChildFee.Text.Trim() + "|");
        sbVal.Append(txtStartDate.Value.Trim() + "|");
        sbVal.Append(txtEndDate.Value.Trim() + "|");
        bd_base_parameters.SetValue = sbVal.ToString().Trim(new char[] { '|' });
        bd_base_parameters.StartDate = System.DateTime.Now;
        bd_base_parameters.StartDate = System.DateTime.Now.AddYears(5);
        if (list.Length > 0)
        {
            //修改
            //if (Manage.Update(bd_base_parameters))
            bool UpdateSuc = (bool)baseDataManage.CallMethod("Bd_Base_Parameters", "Update", null, new object[] { bd_base_parameters });
            if (UpdateSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('保存成功');", true);
                repFuelListDataBind("Load");
                Clear();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('保存失败');", true);
            }
        }
        else
        {
            //添加
            //if (Manage.Insert(bd_base_parameters))
            bool InsertSuc = (bool)baseDataManage.CallMethod("Bd_Base_Parameters", "Insert", null, new object[] { bd_base_parameters });
            if (InsertSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('保存成功');", true);
                repFuelListDataBind("Load");
                Clear();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('保存失败');", true);
            }
        }
    }
    public void Clear()
    {
        txtExceedAdultFee.Text = "";
        txtLowAdultFee.Text = "";
        txtExceedChildFee.Text = "";
        txtLowChildFee.Text = "";
        txtStartDate.Value = "";
        txtEndDate.Value = "";
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Clear();
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        repFuelListDataBind("update");
    }
}
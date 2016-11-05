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
/// <summary>
/// 参数表
/// </summary>
public partial class Manager_Base_BaseParametersEdit : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = Request["currentuserid"].ToString();
            btnGo.PostBackUrl = string.Format("BaseParametersList.aspx?currentuserid={0}", this.currentuserid.Value.ToString());
 
            if (Request.QueryString["id"] != null && Request.QueryString["id"].ToString() != "")
            {
                bindData(Request.QueryString["id"].ToString());
            }
            else
            {
                txtStartDate.Value = System.DateTime.Now.ToString("yyyy-MM-dd");
                txtEndDate.Value = System.DateTime.Now.AddYears(5).ToString("yyyy-MM-dd");
            }
        }
    }

    /// <summary>
    /// 显示数据
    /// </summary>
    public void bindData(string id)
    {
        //获取
        Bd_Base_Parameters bd_base_parameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetById", null, new object[] { id }) as Bd_Base_Parameters;
        if (bd_base_parameters != null)
        {
            txtCompanyNo.Text = bd_base_parameters.CpyNo;
            txtParamName.Text = bd_base_parameters.SetName;
            txtParamValue.Text = bd_base_parameters.SetValue;
            txtStartDate.Value = bd_base_parameters.StartDate.ToString("yyyy-MM-dd");
            txtEndDate.Value = bd_base_parameters.EndDate.ToString("yyyy-MM-dd");
            txtParamDescript.Text = bd_base_parameters.SetDescription;
            txtRemark.Text = bd_base_parameters.Remark;
        }
    }
    public bool InNull()
    {
        bool IsCommit = true;
        if (string.IsNullOrEmpty(txtCompanyNo.Text.Trim()))
        {
            IsCommit = false;
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('公司编号不能为空！');", true);
        }
        else if (string.IsNullOrEmpty(txtParamName.Text.Trim()))
        {
            IsCommit = false;
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('参数名不能为空！');", true);
        }
        else if (string.IsNullOrEmpty(txtStartDate.Value.Trim()))
        {
            IsCommit = false;
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('参数起始日期不能为空！');", true);
        }
        else if (string.IsNullOrEmpty(txtEndDate.Value.Trim()))
        {
            IsCommit = false;
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('参数截止日期不能为空！');", true);
        }
        return IsCommit;
    }


    //添加或者修改   
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!InNull())
        {
            return;
        }
        Bd_Base_Parameters bd_base_parameters = null;
        if (Request.QueryString["id"] != null && Request.QueryString["id"].ToString() != "")
        {
            //获取实体
            bd_base_parameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetById", null, new object[] { Request.QueryString["id"].ToString() }) as Bd_Base_Parameters;
        }
        else
        {
            bd_base_parameters = new Bd_Base_Parameters();
        }

        bd_base_parameters.CpyNo = txtCompanyNo.Text.Trim().Replace("'", "");
        bd_base_parameters.SetName = txtParamName.Text.Trim().Replace("'", "");
        bd_base_parameters.SetValue = txtParamValue.Text.Trim().Replace("'", "");
        bd_base_parameters.StartDate = DateTime.Parse(txtStartDate.Value.Replace("'", ""));
        bd_base_parameters.EndDate = DateTime.Parse(txtEndDate.Value.Replace("'", ""));
        bd_base_parameters.SetDescription = txtParamDescript.Text.Trim().Replace("'", "");
        bd_base_parameters.Remark = txtRemark.Text.Replace("'", "");
        if (Request.QueryString["id"] != null && Request.QueryString["id"].ToString() != "")
        {
            bool UpdateSuc = (bool)baseDataManage.CallMethod("Bd_Base_Parameters", "Update", null, new object[] { bd_base_parameters });
            //保存          
            if (UpdateSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('保存成功！');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('保存失败！');", true);
            }
        }
        else
        {
            string sqlWhere = string.Format(" CpyNo='{0}' and  SetName='{1}' ", txtCompanyNo.Text.Trim(), txtParamName.Text.Trim());
            //IHashObject parameter = new HashObject();
            //parameter.Add("CpyNo", txtCompanyNo.Text.Trim());
            //parameter.Add("SetName", txtParamName.Text.Trim());
            List<Bd_Base_Parameters> objList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Parameters>;
            if (objList.Count == 0)
            {
                //添加         
                bool InsertSuc = (bool)baseDataManage.CallMethod("Bd_Base_Parameters", "Insert", null, new object[] { bd_base_parameters });
                if (InsertSuc)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('添加成功！');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('添加失败！');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog3('该公司编号（" + txtCompanyNo.Text.Trim() + "）和参数名（" + txtParamName.Text.Trim() + "）已经存在！');", true);
            }
        }
    }
}
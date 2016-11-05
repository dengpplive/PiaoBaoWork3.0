using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.SqlClient;
using DataBase.Data;
using PbProject.Model;

public partial class SMS_SmsTemplateManage : BasePage
{
    protected string msg = "";
    protected Tb_Sms_Template Mtemplate = new Tb_Sms_Template();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Curr = 1;
            Con = Query();
            AspNetPager1.PageSize = 10;
            ViewState["orderBy"] = " SmsTpDate desc ";
            this.txtCreateTime.Value = DateTime.Now.AddMonths(-1).ToShortDateString();
            this.txtEndTime.Value = DateTime.Now.AddDays(1).ToShortDateString();
            BindTemplateInfo();
        }
    }
    /// <summary>
    /// 条件
    /// </summary>
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    /// <summary>
    /// 页数
    /// </summary>
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }
    #region 顶部功能（增，查）
    protected void btcheack_Click(object sender, EventArgs e)
    {
        Curr = 1;
        AspNetPager1.CurrentPageIndex = Curr;
        Con = Query();
        BindTemplateInfo();
    }
    protected void btadd_Click(object sender, EventArgs e)
    {
        this.Panel2.Visible = true;
        this.txttemplatecontent.Value = "";
        this.txttemplatename.Text = "";
        this.txttemplatename.ReadOnly = false;
        btaddtemplate.Text = "添加模板";
        this.HiddenField1.Value = "";
    }
    protected void btreset_Click(object sender, EventArgs e)
    {
        this.txttempname.Text = "";
        this.txttempname.Text = "";
        this.ddltype.SelectedIndex = 0;
        this.txtCreateTime.Value = DateTime.Now.AddMonths(-1).ToShortDateString();
        this.txtEndTime.Value = DateTime.Now.AddDays(1).ToShortDateString();
    }
    #endregion
    /// <summary>
    /// 查询条件拼接
    /// </summary>
    /// <returns></returns>
    private string Query()
    {
        StringBuilder sb = new StringBuilder(" 1=1");
        try
        {
           
            if (txttempname.Text.Trim().Length != 0)
            {
                sb.Append(" and SmsTpName like '%" + txttempname.Text.ToString() + "%'");
            }

            if (txtCreateTime.Value.Trim() != "")
            {
                sb.Append(" and SmsTpDate > '" + txtCreateTime.Value.Trim() + "'");
            }
            if (txtEndTime.Value.Trim() != "")
            {
                sb.Append(" and SmsTpDate <= '" + txtEndTime.Value.Trim() + "'");
            }
            if (ddltype.SelectedValue.ToString() != "-1")
            {
                if (ddltype.SelectedValue.ToString() == "0")
                {
                    sb.Append(" and SmsTpType = " + ddltype.SelectedValue.ToString());
                }
                else
                {
                    sb.Append(" and CpyNo = '" + mCompany.UninCode + "'");
                }
            }
            else
            {
                sb.Append(" and CpyNo = '" + mCompany.UninCode + "' or SmsTpType=0");
            }
        }
        catch (Exception)
        {
            throw;
        }
        return sb.ToString();
    }
    /// <summary>
    /// 数据绑定
    /// </summary>
    protected void BindTemplateInfo()
    {
        try
        {
            int TotalCount = 0;
            IHashObject outParams = new HashObject();
            //指定参数类型 第一个参数为out输出类型
            //key 为参数索引从1开始 value为引用类型 out ref
            outParams.Add("1", "out");
            List<Tb_Sms_Template> list = baseDataManage.CallMethod("Tb_Sms_Template", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, ViewState["orderBy"].ToString() }) as List<Tb_Sms_Template>;
            TotalCount = outParams.GetValue<int>("1");
            AspNetPager1.RecordCount = TotalCount;
            AspNetPager1.CurrentPageIndex = Curr;
            AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
           
            if (AspNetPager1.RecordCount == 0)
            {
                this.Panel1.Visible = false;
                this.Show.Visible = true;
            }
            else
            {
                this.Panel1.Visible = true;
                this.Show.Visible = false;
            }
            this.RepTempLate.DataSource = list;
            this.RepTempLate.DataBind();
        }
        catch (Exception)
        {
            throw;
        }

    }
    /// <summary>
    /// 操作项 (改，删)
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RepTempLate_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string tid = e.CommandArgument.ToString();
        if (e.CommandName == "edit")
        {
            this.HiddenField1.Value = tid.ToString();
            Mtemplate = baseDataManage.CallMethod("Tb_Sms_Template", "GetById", null, new object[] { tid }) as Tb_Sms_Template;
            txttemplatename.Text = Mtemplate.SmsTpName.ToString();
            txttemplatecontent.Value = Mtemplate.SmsTpContent.ToString();
            this.btaddtemplate.Text = "确定修改";
            this.Panel2.Visible = true;
            this.txttemplatename.ReadOnly = true;
        }
        else
        {
            msg = (bool)baseDataManage.CallMethod("Tb_Sms_Template", "DeleteById", null, new Object[] { tid }) == true ? "删除成功" : "删除失败";
            BindTemplateInfo();
            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
        }
       
    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        try
        {
            Curr = e.NewPageIndex;
            BindTemplateInfo();
        }
        catch (Exception)
        {

            throw;
        }
    }
    #region 添加模板块按钮
    /// <summary>
    /// 添加,修改，删除
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btaddtemplate_Click(object sender, EventArgs e)
    {
        try
        {
            //string cheackkeyword = PiaoBao.BLLLogic.Common.SelectKeyWord(txttemplatecontent.Value.ToString()).ToString();
            if (this.txttemplatecontent.Value.Length > 400 || txttemplatecontent.Value.Trim().ToString().Length < 1)
            {
                msg = "模板内容为空或模板内容过长，最多490字符!";
            }
            //else if (cheackkeyword.Length > 0 && cheackkeyword != "nulls")
            //{
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "key", "<script>alert('内容存在非法字词！[" + cheackkeyword + "]')</script>", false);
            //}
            else if (this.txttemplatename.Text.Trim().Length > 25 || this.txttemplatename.Text.Trim().Length < 1)
            {
                msg = "模板名为空或模板名过长，最多50字符!";
            }
            else
            {
                Mtemplate.CpyNo = mCompany.UninCode;
                Mtemplate.SmsTpType = 1;
                Mtemplate.SmsTpName = txttemplatename.Text.ToString();
                Mtemplate.SmsTpContent = txttemplatecontent.Value.ToString();
                Mtemplate.SmsTpDate = DateTime.Parse(DateTime.Now.ToString());
                if (HiddenField1.Value.Trim().Length > 0 && btaddtemplate.Text == "确定修改")
                {
                    //修改
                    Mtemplate.id = Guid.Parse(HiddenField1.Value.ToString());
                    msg = (bool)baseDataManage.CallMethod("Tb_Sms_Template", "Update", null, new object[] { Mtemplate }) == true ? "修改成功" : "修改失败";
                }
                else
                {
                    List<Tb_Sms_Template> listTemp = baseDataManage.CallMethod("Tb_Sms_Template", "GetList", null, new Object[] { "SmsTpName='" + txttemplatename.Text.Trim().ToString() + "' and CpyNo='" + mCompany.UninCode + "'" }) as List<Tb_Sms_Template>;
                    if (listTemp != null && listTemp.Count > 0)
                    {
                        msg = "模板名已存在，请从新输入！";
                    }
                    else
                    {
                        //添加
                        msg = (bool)baseDataManage.CallMethod("Tb_Sms_Template", "Insert", null, new object[] { Mtemplate }) == true ? "添加成功" : "添加失败";
                    }
                }
                BindTemplateInfo();
            }
        }
        catch (Exception)
        {
            msg = "操作失败！";
        }
       
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }
    protected void btclear_Click(object sender, EventArgs e)
    {
        this.txttemplatename.Text = "";
        this.txttemplatecontent.Value = "";
    }
    protected void btqx_Click(object sender, EventArgs e)
    {
        this.txttemplatename.Text = "";
        this.txttemplatecontent.Value = "";
        this.txttemplatename.ReadOnly = false;
        this.Panel2.Visible = false;
    }
    #endregion
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Text;
using DataBase.Data;
public partial class User_PassengerList : BasePage
{
    #region 属性
    List<Bd_Base_Dictionary> PasTypeList = null;
    List<Bd_Base_Dictionary> CardTypeList = null;
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }
    /// <summary>
    /// 排序字段和升降序
    /// </summary>
    public string OrderBy
    {
        get { return (string)ViewState["orderBy"]; }
        set { ViewState["orderBy"] = value; }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        //分页大小
        AspNetPager1.PageSize = int.Parse(Hid_pageSize.Value);
        if (!IsPostBack)
        {
            btnAdd.PostBackUrl = string.Format("PassengerEdit.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            //绑定证件号
            BindCardType();
            OrderBy = " id ";
            Con = " 1=1 ";
        }
    }

    /// <summary>
    /// 绑定数据
    /// </summary>
    public void BindCardType()
    {
        if (CardTypeList == null)
        {
            string sqlWhere = " parentid=7 order by ChildID";
            CardTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        }
        if (PasTypeList == null)
        {
            string sqlWhere = " parentid=6 order by ChildID";
            PasTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        }

        if (CardTypeList != null && CardTypeList.Count > 0)
        {
            ddlCardType.Items.Clear();
            ddlCardType.Items.Add(new ListItem("--不限--", ""));
            foreach (Bd_Base_Dictionary item in CardTypeList)
            {
                ListItem lim = new ListItem();
                lim.Text = item.ChildName;
                lim.Value = item.ChildID.ToString();
                ddlCardType.Items.Add(lim);
            }
        }
        if (PasTypeList != null && PasTypeList.Count > 0)
        {
            rdlPasType.DataSource = PasTypeList;
            rdlPasType.DataTextField = "ChildName";
            rdlPasType.DataValueField = "ChildID";
            rdlPasType.DataBind();
            rdlPasType.Items.Insert(0, new ListItem("不限", ""));
            rdlPasType.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// 绑定
    /// </summary>
    private void PageDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<User_Flyer> list = baseDataManage.CallMethod("User_Flyer", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, OrderBy }) as List<User_Flyer>;
        TotalCount = outParams.GetValue<int>("1");
        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        Repeater.DataSource = list;
        Repeater.DataBind();
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    /// <returns></returns>
    public string Query()
    {
        StringBuilder SQLWhere = new StringBuilder();
        SQLWhere.AppendFormat(" CpyNo='{0}' ", mCompany.UninCode);
        //常旅客姓名
        if (txtName.Text.Trim() != "")
        {
            SQLWhere.AppendFormat(" and Name='{0}' ", txtName.Text.Trim());
        }
        //电话
        if (txtTel.Text.Trim() != "")
        {
            SQLWhere.AppendFormat(" and Tel='{0}' ", txtTel.Text.Trim());
        }
        //旅客类型
        if (rdlPasType.SelectedValue != "")
        {
            SQLWhere.AppendFormat(" and Flyertype='{0}' ", rdlPasType.SelectedValue.Trim());
        }
        //证件类型
        if (ddlCardType.SelectedValue != "")
        {
            SQLWhere.AppendFormat(" and CertificateType='{0}' ", ddlCardType.SelectedValue.Trim());
        }
        //证件号码
        if (txtCardNum.Text.Trim() != "")
        {
            SQLWhere.AppendFormat(" and CertificateNum='{0}' ", txtCardNum.Text.Trim());
        }
        return SQLWhere.ToString();
    }
    /// <summary>
    /// 显示字段文本数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public string ShowFiled(int type, object obj)
    {
        string result = "";
        if (type == 0)
        {
            //乘客类型
            if (obj != null && obj != DBNull.Value && obj.ToString() != "")
            {
                if (obj.ToString() == "1")
                {
                    result = "成人";
                }
                else if (obj.ToString() == "2")
                {
                    result = "儿童";
                }
                else if (obj.ToString() == "3")
                {
                    result = "婴儿";
                }
            }
        }
        else if (type == 1)
        {
            //证件类型
            if (obj != null && obj != DBNull.Value && obj.ToString() != "")
            {
                if (CardTypeList == null)
                {
                    string sqlWhere = " parentid=7 order by ChildID";
                    CardTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
                }
                if (CardTypeList != null)
                {
                    Bd_Base_Dictionary dic = CardTypeList.Find(delegate(Bd_Base_Dictionary _dic)
                      {
                          return _dic.ChildID.ToString().Trim() == obj.ToString().Trim();
                      });
                    if (dic != null)
                    {
                        result = dic.ChildName;
                    }
                }
            }
        }
        else if (type == 2)
        {
            //卡号
            if (obj != null && obj != DBNull.Value && obj.ToString() != "")
            {
                result = obj.ToString().Replace("|", "<br />");
            }
        }
        return result;
    }
    //分页
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }
    public void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string CommandName = e.CommandName;
        string Id = e.CommandArgument.ToString();
        if (CommandName == "Del")
        {
            bool IsSuc = (bool)this.baseDataManage.CallMethod("User_Flyer", "DeleteById", null, new object[] { Id });
            if (IsSuc)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('删除成功');", true);
                PageDataBind();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('删除失败');", true);
            }
        }
    }
    //查询
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Curr = 1;
        Con = Query();
        PageDataBind();
    }
    //删除
    protected void btnPathDel_Click(object sender, EventArgs e)
    {
        string SQLWhere = string.Format(" id in({0}) ", Hid_SelIds.Value);

        bool IsSuc = (bool)this.baseDataManage.CallMethod("User_Flyer", "DeleteBySQL", null, new object[] { SQLWhere });
        if (IsSuc)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('删除成功');", true);
            PageDataBind();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('删除失败');", true);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PbProject.Model;
using DataBase.Data;
using System.Text;
using PbProject.WebCommon.Utility.Encoding;
using PbProject.WebCommon.Utility;
/// <summary>
/// 选择常旅客
/// </summary>
public partial class Air_Buy_FlyerList : BasePage
{
    private List<Bd_Base_Dictionary> PasTypelist = new List<Bd_Base_Dictionary>();
    private List<Bd_Base_Dictionary> CardTypelist = new List<Bd_Base_Dictionary>();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
             if (Request["LoginAccount"] != null && Request["LoginAccount"].ToString() != "" && Request["LoginID"] != null && Request["LoginID"].ToString() != "")
            {
                string LoginAccount = Request["LoginAccount"].ToString();
                string LoginID = Request["LoginID"].ToString();
                string BigNum = Request["BigNum"].ToString();
                string FgNum = Request["FgNum"].ToString();//序号


                Curr = 1;
                AspNetPager1.PageSize = int.Parse(hid_PageSize.Value);

                Hid_FgNum.Value = FgNum;
                Hid_LoginAccount.Value = LoginAccount;
                Hid_LoginID.Value = LoginID;
                Hid_BigNum.Value = BigNum;//可用座位数
                BindData();
            }
        }
    }

    public void BindData()
    {
        //条件
        Con = Query();

        //分页
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<User_Flyer> list = baseDataManage.CallMethod("User_Flyer", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, " id " }) as List<User_Flyer>;
        TotalCount = outParams.GetValue<int>("1");
        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;


        //不用分页
        // List<User_Flyer> list = baseDataManage.CallMethod("User_Flyer", "GetList", null, new object[] { Con }) as List<User_Flyer>;
        gvflyer.DataSource = list;
        gvflyer.DataBind();

        //设置隐藏域数据
        SetHidVal(list);
    }
    #region 属性
    /// <summary>
    /// 当前分页第几页
    /// </summary>
    protected int Curr
    {
        get { return (int)ViewState["currpage"]; }
        set { ViewState["currpage"] = value; }
    }
    /// <summary>
    /// 查询条件
    /// </summary>
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    /// <summary>
    /// 排序字段和升降序
    /// </summary>
    public string OrderBy
    {
        get { return (string)ViewState["orderBy"]; }
        set { ViewState["orderBy"] = value; }
    }
    /// <summary>
    /// 获取控制系统权限 
    /// </summary>
    public string KongZhiXiTong
    {
        get
        {
            return BaseParams.getParams(supBaseParametersList).KongZhiXiTong;
        }
    }
    /// <summary>
    /// 供应控制分销开关 
    /// </summary>
    public string GongYingKongZhiFenXiao
    {
        get
        {
            return BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
        }
    }
    #endregion
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        BindData();
    }

    /// <summary>
    /// 显示证件类型
    /// </summary>
    /// <returns></returns>
    public string ShowCardType(string CardType)
    {
        string strCardType = "";
        if (CardTypelist.Count == 0)
        {
            CardTypelist = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { "parentid=7 order by ChildID" }) as List<Bd_Base_Dictionary>;
        }
        foreach (Bd_Base_Dictionary item in CardTypelist)
        {
            if (CardType == item.ChildID.ToString())
            {
                strCardType = item.ChildName;
                break;
            }
        }
        return strCardType;
    }

    /// <summary>
    /// 显示乘客类型
    /// </summary>
    /// <returns></returns>
    public string ShowPasType(string PasType)
    {
        string strPasType = "";
        if (PasTypelist.Count == 0)
        {
            PasTypelist = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { "parentid=6 order by ChildID" }) as List<Bd_Base_Dictionary>;
        }
        foreach (Bd_Base_Dictionary item in PasTypelist)
        {
            if (PasType == item.ChildID.ToString())
            {
                strPasType = item.ChildName;
                break;
            }
        }
        return strPasType;
    }

    public string Query()
    {
        StringBuilder sbWhere = new StringBuilder();
        sbWhere.Append(string.Format(" MemberAccount='{0}' and RemainWithId='{1}' ", Hid_LoginAccount.Value, Hid_LoginID.Value));
        //乘客姓名
        if (!string.IsNullOrEmpty(txtName.Value.Trim()))
        {
            sbWhere.AppendFormat(" and  Name like '%{0}%' ", txtName.Value.Trim().Replace("\'", ""));
        }
        //证件号
        if (!string.IsNullOrEmpty(txtNo.Value.Trim()))
        {
            sbWhere.AppendFormat(" and  CertificateNum like '%{0}%' ", txtNo.Value.Trim().Replace("\'", ""));
        }
        //手机号
        if (!string.IsNullOrEmpty(txtPhone.Value.Trim()))
        {
            sbWhere.AppendFormat(" and  Tel like '%{0}%' ", txtPhone.Value.Trim().Replace("\'", ""));
        }
        //限制查询儿童
        if (GongYingKongZhiFenXiao != null && GongYingKongZhiFenXiao.Contains("|90|"))
        {
            sbWhere.AppendFormat(" and  Flyertype<>2 ");
        }
        return sbWhere.ToString();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSel_Click(object sender, EventArgs e)
    {
        Curr = 1;
        BindData();
    }

    /// <summary>
    /// 将实体转换为Json放到隐藏域
    /// </summary>
    /// <param name="list"></param>
    public void SetHidVal(List<User_Flyer> list)
    {
        string strVal = JsonHelper.ObjToJson<List<User_Flyer>>(list);
        Info.Value = escape(strVal);
    }
}
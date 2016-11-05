using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DataBase.Data;
using PbProject.Model;
using System.Text;
using PbProject.Logic.ControlBase;

public partial class TravelNumManage_BlackTripList : BasePage
{
    #region 属性
    private List<Bd_Base_Dictionary> dicList = new List<Bd_Base_Dictionary>();
    /// <summary>
    /// 查询条件
    /// </summary>
    protected string Con
    {
        get { return (string)ViewState["Con"]; }
        set { ViewState["Con"] = value; }
    }
    /// <summary>
    /// 当前分页第几页
    /// </summary>
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
    private string _UseAccount = string.Empty;
    /// <summary>
    /// 分发账号
    /// </summary>
    public string UseAccount
    {
        get
        {
            return _UseAccount;
        }
        set
        {
            _UseAccount = value;
        }
    }
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["OP"] != null && Request["OP"].ToString() == "huishou")
        {
            //修改
            Update();
        }
        else
        {
            if (!Page.IsPostBack)
            {
                Curr = 1;
                //分页大小
                AspNetPager1.PageSize = int.Parse(hid_PageSize.Value);
                OrderBy = " id ";
                //初始化参数
                InitParam();
                Con = Query();
                PageDataBind();
            }
        }
    }
    /// <summary>
    /// 获取请求值
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="DefaultVal"></param>
    /// <returns></returns>
    public string GetVal(string Name, string DefaultVal)
    {
        if (Request[Name] != null)
        {
            DefaultVal = HttpUtility.UrlDecode(Request[Name].ToString(), Encoding.Default);
        }
        return DefaultVal;
    }
    public void InitParam()
    {
        if (GetVal("OwnerCpyNo", "") != "")
        {
            Hid_OwnerCpyNo.Value = GetVal("OwnerCpyNo", "");
        }
        if (GetVal("UseCpyNo", "") != "")
        {
            Hid_UseCpyNo.Value = GetVal("UseCpyNo", "");
            string SqlWhere = string.Format(" CpyNo='{0}' and IsAdmin=0 ", Hid_UseCpyNo.Value);
            List<User_Employees> list = this.baseDataManage.CallMethod("User_Employees", "GetList", null, new object[] { SqlWhere }) as List<User_Employees>;
            if (list != null && list.Count > 0)
            {
                UseAccount = list[0].LoginName;
            }
        }
        if (GetVal("OwnerCpyName", "") != "")
        {
            Hid_OwnerCpyName.Value = GetVal("OwnerCpyName", "");
        }
        if (GetVal("UseCpyName", "") != "")
        {
            Hid_UseCpyName.Value = GetVal("UseCpyName", "");
        }
    }
    private void PageDataBind()
    {
        int TotalCount = 0;
        IHashObject outParams = new HashObject();
        //指定参数类型 第一个参数为out输出类型
        //key 为参数索引从1开始 value为引用类型 out ref
        outParams.Add("1", "out");
        List<Tb_TripDistribution> list = baseDataManage.CallMethod("Tb_TripDistribution", "GetBasePager1", outParams, new object[] { TotalCount, AspNetPager1.PageSize, Curr, "*", Con, OrderBy }) as List<Tb_TripDistribution>;
        TotalCount = outParams.GetValue<int>("1");
        AspNetPager1.RecordCount = TotalCount;
        AspNetPager1.CurrentPageIndex = Curr;
        AspNetPager1.CustomInfoHTML = "&nbsp;&nbsp;&nbsp;  页码 : <font color=\"red\" size='2px'>" + Curr + "</font> / " + AspNetPager1.PageCount;
        repList.DataSource = list;
        repList.DataBind();
    }
    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void AspNetPager1_PageChanging(object src, Wuqi.Webdiyer.PageChangingEventArgs e)
    {
        Curr = e.NewPageIndex;
        PageDataBind();
    }
    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Con = Query();
        Curr = 1;
        PageDataBind();
    }
    public string Query()
    {
        string tripNum = txtNumber.Text.Trim().Replace("'", "");
        StringBuilder SQLWhere = new StringBuilder();
        SQLWhere.AppendFormat(" OwnerCpyNo='{0}' and UseCpyNo='{1}' and  TripStatus=8 ", Hid_OwnerCpyNo.Value, Hid_UseCpyNo.Value);
        if (!string.IsNullOrEmpty(tripNum))
        {
            SQLWhere.AppendFormat(" and TripNum ='{0}' ", tripNum);
        }
        return SQLWhere.ToString();
    }
    /// <summary>
    /// 响应客户端结果数据
    /// </summary>
    /// <param name="result"></param>
    public void OutPut(string result)
    {
        Context.Response.ContentType = "text/plain";
        Context.Response.Clear();
        Context.Response.Write(result);
        Context.Response.Flush();
        Context.Response.End();
    }


    public void Update()
    {
        if (Request["OP"] != null && Request["OP"].ToString() == "huishou")
        {
            string OwnerCpyNo = GetVal("OwnerCpyNo", "");
            string OwnerCpyName = GetVal("OwnerCpyName", "");
            string UseCpyNo = GetVal("UseCpyNo", "");
            string UseCpyName = GetVal("UseCpyName", "");
            string Ids = GetVal("Id", "");
            string ErrMsg = "0##失败";
            try
            {
                List<string> ids = new List<string>();
                ids.AddRange(Ids.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries));
                //UseCpyNo=@UseCpyNo,UseCpyName=@UseCpyName,UseTime=GETDATE(),TripStatus=@TripStatus 
                //修改为 空白回收,已分配 
                string updateFileds = string.Format(" UseCpyNo='{0}',UseCpyName='{1}',UseTime=GETDATE(),TripStatus='11' ", UseCpyNo, UseCpyName);
                bool isSuc = (bool)this.baseDataManage.CallMethod("Tb_TripDistribution", "UpdateByIds", null, new object[] { ids, updateFileds });
                if (isSuc)
                {
                    ErrMsg = "1##发放成功";
                }
                else
                {
                    ErrMsg = "0##发放失败";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = "0##" + ex.Message;
            }
            finally
            {
                OutPut(ErrMsg);
            }
        }
    }
    /// <summary>
    /// 获取行程单状态名称
    /// </summary>
    /// <param name="parentId"></param>
    /// <param name="childId"></param>
    /// <returns></returns>
    public string GetDicName(int parentId, string childId)
    {
        string result = "";
        if (dicList.Count == 0)
        {
            dicList = new Bd_Base_DictionaryBLL().GetList();
        }
        var query = from Bd_Base_Dictionary d in dicList
                    where d.ParentID == parentId && d.ChildID == int.Parse(childId)
                    select d;
        if (query.Count<Bd_Base_Dictionary>() > 0)
        {
            List<Bd_Base_Dictionary> list = query.ToList<Bd_Base_Dictionary>();
            result = list[0].ChildName;
        }
        return result;
    }

    /// <summary>
    /// 获取状态
    /// </summary>
    /// <param name="IsUse"></param>
    /// <param name="IsInvalid"></param>
    /// <returns></returns>
    public string ShowText(int type, object obj)
    {
        string result = "";
        if (type == 0)
        {
            //行程单状态
            if (obj != null && obj.ToString() != "")
            {
                result = GetDicName(34, obj.ToString());
            }
        }
        return result;
    }
}
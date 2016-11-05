using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Text;
using System.Collections;
using PbProject.Logic;
using PbProject.WebCommon.Utility.Encoding;
public partial class Order_LineOrderApply : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (this.SessionIsNull)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面已经失效,请重新登录！');", true);
        //    return;
        //}
        if (mCompany.RoleType <= 3)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('您不能访问该页面！');", true);
            return;
        }
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            //初始化信息
            InitInfo();
            //常旅客数据
            GetFlyUser();
            //绑定乘客类型
            BindPasType();
            //绑定证件类型
            BindCardType();
            //绑定城市
            BindCity();
            //绑定机型
            BindAircraft();
        }
    }
    /// <summary>
    /// 常旅客列表
    /// </summary>
    private List<User_Flyer> flyList = new List<User_Flyer>();
    /// <summary>
    /// 查询该用户的常旅客
    /// </summary>
    public void GetFlyUser()
    {
        if (flyList.Count == 0)
        {
            string sqlWhere = string.Format(" MemberAccount='{0}' and RemainWithId='{1}' ", mUser.LoginName, mUser.id);
            //是否存在
            flyList = this.baseDataManage.CallMethod("User_Flyer", "GetList", null, new object[] { sqlWhere }) as List<User_Flyer>;
            if (flyList != null && flyList.Count > 0)
            {
                //用于页面输入查找
                Hid_FlyerList.Value = escape(JsonHelper.ObjToJson<List<User_Flyer>>(flyList));
            }
        }
    }
    public void InitInfo()
    {
        startdate_0.Value = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        enddate_0.Value = System.DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm");
        txtBirthday_0.Value = System.DateTime.Now.ToString("yyyy-MM-dd");
        linkName.Value = mCompany.ContactUser;
        linkPhone.Value = mCompany.ContactTel;
        Hid_UninCode.Value = mCompany.UninCode;
        Hid_LoginName.Value = mUser.LoginName;

        //新加 用于常旅客
        Hid_LoginAccount.Value = mUser != null ? mUser.LoginName : "";
        Hid_LoginID.Value = mUser != null ? mUser.id.ToString() : "";
        Hid_CPCpyNo.Value = mCompany.UninCode.Length >= 12 ? mCompany.UninCode.Substring(0, 12) : mCompany.UninCode;
        //示例
        txtExplame.Value = " 1.刘艳 HYPQP5  \r 2.  CA8208 Q   TH29NOV  CTUWUH HK1   1150 1325          E T2-- \r 3.CTU/T CTU/T 028-5566222/CTU QI MING INDUSTRY CO.,LTD/TONG LILI ABCDEFG   \r 4.25869587 \r 5.TL/1050/29NOV/CTU324 \r 6.SSR FOID CA HK1 NI428022198810122547/P1  \r 7.SSR ADTK 1E BY CTU14NOV12/2118 OR CXL CA ALL SEGS\r 8.OSI CA CTCT18708178001/A \r 9.RMK CA/NYDD3E\r10.CTU324   \r\r";
    }
    /// <summary>
    /// 绑定乘机人类型
    /// </summary>
    public void BindPasType()
    {
        string sqlWhere = " parentid=6  order by ChildID";
        List<Bd_Base_Dictionary> PasTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        if (PasTypeList != null && PasTypeList.Count > 0)
        {
            foreach (Bd_Base_Dictionary item in PasTypeList)
            {
                ListItem lim = new ListItem();
                lim.Text = item.ChildName;
                lim.Value = item.ChildID.ToString();
                SelPasType_0.Items.Add(lim);
            }
        }
    }
    /// <summary>
    /// 绑定证件号类型
    /// </summary>
    public void BindCardType()
    {
        string sqlWhere = " parentid=7 order by ChildID";
        List<Bd_Base_Dictionary> PasTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        if (PasTypeList != null && PasTypeList.Count > 0)
        {
            List<string> lstArr = new List<string>();
            foreach (Bd_Base_Dictionary item in PasTypeList)
            {
                ListItem lim = new ListItem();
                lim.Text = item.ChildName;
                lim.Value = item.ChildID.ToString();
                if (item.ChildName != "出生日期")
                {
                    SelCardType_0.Items.Add(lim);
                }
                lstArr.Add(item.ChildID.ToString() + "###" + item.ChildName.Replace("###", "").Replace("|", ""));
            }
            //证件类型字符串
            Hid_CardType.Value = escape(string.Join("|", lstArr.ToArray()));
        }
    }
    /// <summary>
    /// 绑定城市
    /// </summary>
    public void BindCity()
    {
        List<Bd_Air_AirPort> list = this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { "" }) as List<Bd_Air_AirPort>;
        if (list != null && list.Count > 0)
        {
            string cityData = JsonHelper.ObjToJson<List<Bd_Air_AirPort>>(list);
            Hid_CityData.Value = escape(cityData);
        }
        List<Bd_Air_Carrier> Airlist = this.baseDataManage.CallMethod("Bd_Air_Carrier", "GetList", null, new object[] { "" }) as List<Bd_Air_Carrier>;
        if (Airlist != null && Airlist.Count > 0)
        {
            string cityData = JsonHelper.ObjToJson<List<Bd_Air_Carrier>>(Airlist);
            Hid_AirData.Value = escape(cityData);
        }
        ////排序 
        //SortedList sortLst = new SortedList();
        //foreach (Bd_Air_AirPort city in list)
        //{
        //    ListItem item = new ListItem();
        //    item.Text = city.CityCodeWord + "-" + city.CityName;
        //    item.Value = city.CityCodeWord;
        //    if (!sortLst.ContainsKey(city.CityCodeWord))
        //    {
        //        sortLst.Add(city.CityCodeWord, item);
        //    }
        //}
        //ListItem[] newItem = new ListItem[sortLst.Values.Count];
        //sortLst.Values.CopyTo(newItem, 0);
        //ddlFromCity_0.Items.Clear();
        //ddlToCity_0.Items.Clear();
        //ddlFromCity_0.Items.Add(new ListItem("--出发城市--", ""));
        //ddlFromCity_0.Items.AddRange(newItem);
        //ddlToCity_0.Items.Add(new ListItem("--到达城市--", ""));
        //ddlToCity_0.Items.AddRange(newItem);
    }
    /// <summary>
    /// 绑定机型
    /// </summary>
    public void BindAircraft()
    {
        //List<Bd_Air_Aircraft> defaultList = this.baseDataManage.CallMethod("Bd_Air_Aircraft", "GetList", null, new object[] { "" }) as List<Bd_Air_Aircraft>;
        //ddlAircraft.DataSource = defaultList;
        //ddlAircraft.DataFiledText = "Aircraft";
        //ddlAircraft.DataFiledValue = "ABFeeN-ABFeeW";//国内和国外机建
        //ddlAircraft.DataBind();
    }
    /// <summary>
    /// 检查Session是否丢失
    /// </summary>
    //public bool SessionIsNull
    //{
    //    get
    //    {
    //        bool isSuc = false;
    //        if (Session[new SessionContent().USERLOGIN] == null)
    //        {
    //            isSuc = true;
    //        }
    //        return isSuc;
    //    }
    //}
}
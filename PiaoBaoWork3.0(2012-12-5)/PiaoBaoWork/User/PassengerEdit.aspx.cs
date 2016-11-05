using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Text;
using PbProject.Logic;
using PbProject.WebCommon.Utility.Encoding;
/// <summary>
/// 添加
/// </summary>
public partial class User_PassengerEdit : BasePage
{

    /// <summary>
    /// 乘机人和证件类型列表
    /// </summary>
    List<Bd_Base_Dictionary> PasAndCardTypeList = new List<Bd_Base_Dictionary>();
    /// <summary>
    /// 航空公司列表
    /// </summary>
    List<Bd_Air_Carrier> CarryList = new List<Bd_Air_Carrier>();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            //初始化
            InitParam();
            //绑定航空公司
            BindCarrayCode();
            string id = "0";
            //是编辑还是添加
            if (Request["id"] != null && Request["id"].ToString() != "")
            {
                //编辑
                id = Request["id"].ToString();
                Hid_IsEdit.Value = "1";
                btnSave.Text = "保 存";
                Hid_id.Value = id;
            }
            else
            {
                btnSave.Text = "添 加";
            }
            BindShow(id);
        }
        if (Request["save"] != null && Request["save"].ToString() != "")
        {
            btnSave_Click();
        }
    }

    public void InitParam()
    {
        if (PasAndCardTypeList.Count == 0)
        {
            string sqlWhere = " parentid in(6,7) order by ChildID";
            PasAndCardTypeList = this.baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sqlWhere }) as List<Bd_Base_Dictionary>;
        }
        if (CarryList.Count == 0)
        {
            CarryList = this.baseDataManage.CallMethod("Bd_Air_Carrier", "GetList", null, new object[] { "" }) as List<Bd_Air_Carrier>;
        }
        txtBirthday.Value = System.DateTime.Now.ToString("yyyy-MM-dd");
        txtDate.Value = System.DateTime.Now.ToString("yyyy-MM-dd");
    }
    /// <summary>
    /// 显示数据
    /// </summary>
    /// <param name="id"></param>
    public void BindShow(string id)
    {
        User_Flyer user_flyer = null;
        if (id != "0")
        {
            user_flyer = this.baseDataManage.CallMethod("User_Flyer", "GetById", null, new object[] { id }) as User_Flyer;
        }
        int pastype = 0;
        int cardtype = 0;
        int defaultCount = 0;
        if (user_flyer != null)
        {
            pastype = user_flyer.Flyertype;
            cardtype = user_flyer.CertificateType;
            Hid_Flyer.Value = JsonHelper.ObjToJson<User_Flyer>(user_flyer);
            Hid_CpyandNo.Value = string.Join("|", user_flyer.CpyandNo.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries));
        }
        string IsPasChecked = "";
        StringBuilder sbPasType = new StringBuilder();
        List<string> sbCardType = new List<string>();
        ddlCardType.Items.Clear();
        foreach (Bd_Base_Dictionary item in PasAndCardTypeList)
        {
            if (item.ParentID == 6)
            {
                //乘客类型
                if (pastype == item.ChildID)
                {
                    IsPasChecked = " checked=checked ";
                }
                if (user_flyer == null)
                {
                    if (defaultCount == 0)
                    {
                        IsPasChecked = " checked=checked ";
                    }
                    else
                    {
                        IsPasChecked = "";
                    }
                    defaultCount++;
                }
                sbPasType.AppendFormat("<label for='pastype_{0}'><input id='pastype_{0}' type='radio' name='pastype' {1} value='{0}' txt='{2}'>{2}</label>", item.ChildID, IsPasChecked, item.ChildName);
            }
            if (item.ParentID == 7)
            {
                //证件类型
                ListItem lim = new ListItem();
                lim.Text = item.ChildName;
                lim.Value = item.ChildID.ToString();
                sbCardType.Add(lim.Text + "@@" + lim.Value);
                if (cardtype == item.ChildID)
                {
                    lim.Selected = true;
                }
                else
                {
                    lim.Selected = false;
                }
                ddlCardType.Items.Add(lim);
            }
        }
        //乘客类型
        literPasType.Text = sbPasType.ToString();
        Hid_CardData.Value = string.Join("|", sbCardType.ToArray());
    }

    /// <summary>
    /// 绑定航空公司
    /// </summary>
    public void BindCarrayCode()
    {
        if (CarryList.Count == 0)
        {
            CarryList = this.baseDataManage.CallMethod("Bd_Air_Carrier", "GetList", null, new object[] { "" }) as List<Bd_Air_Carrier>;
        }
        if (CarryList.Count > 0)
        {
            ddlCarryCode_0.Items.Clear();
            foreach (Bd_Air_Carrier item in CarryList)
            {
                ListItem items = new ListItem();
                items.Text = item.Code + "-" + item.ShortName;
                items.Value = item.Code;
                ddlCarryCode_0.Items.Add(items);
            }
            ddlCarryCode_0.Items.Insert(0, new ListItem("--航空公司--", ""));
        }
    }
    public void OutPut(string result)
    {
        Response.ContentType = "text/plain";
        Response.Clear();
        Response.Write(result);
        Response.Flush();
        Response.End();
    }
    public string GetVal(string Name, string DefaultVal)
    {
        if (Request[Name] != null)
        {
            DefaultVal = HttpUtility.UrlDecode(Request[Name].ToString(), Encoding.Default);
        }
        return DefaultVal;
    }
    //保存
    protected void btnSave_Click()
    {
        string IsEdit = GetVal("IsEdit", "0");
        string Name = GetVal("Name", "");
        string Phone = GetVal("Phone", "");
        string CardType = GetVal("CardType", "1");
        string CardNum = GetVal("CardNum", "");
        string Sex = GetVal("Sex", "0");
        string Pastype = GetVal("Pastype", "1");
        string Birthday = GetVal("Birthday", "1901-01-01");
        string Remark = GetVal("Remark", "");
        string CpyandNo = GetVal("CpyandNo", "");
        string Id = GetVal("Id", "");
        User_Flyer Flyer = null;
        bool IsSuc = false;
        string errMsg = "";
        try
        {
            if (IsEdit == "0")
            {
                //添加
                Flyer = new User_Flyer();
            }
            else if (IsEdit == "1")
            {
                //编辑
                Flyer = this.baseDataManage.CallMethod("User_Flyer", "GetById", null, new object[] { Id }) as User_Flyer;
            }
            Flyer.MemberAccount = mUser.LoginName;
            Flyer.RemainWithId = mUser.id.ToString();
            Flyer.CpyNo = mCompany.UninCode;
            Flyer.Name = Name.Trim();
            Flyer.CertificateNum = CardNum.Trim();
            Flyer.CertificateType = int.Parse(CardType);
            Flyer.Tel = Phone.Trim();
            Flyer.Sex = int.Parse(Sex);
            Flyer.Flyertype = int.Parse(Pastype);
            Flyer.BronTime = DateTime.Parse(Birthday);
            Flyer.CpyandNo = CpyandNo.Trim();
            Flyer.Remark = Remark.Trim();

            if (IsEdit == "0")
            {
                string sqlWhere = string.Format(" Name='{0}' and  CertificateNum='{1}'", Flyer.Name, Flyer.CertificateNum);
                //是否存在
                bool IsExist = (bool)this.baseDataManage.CallMethod("User_Flyer", "IsExist", null, new object[] { sqlWhere });
                if (!IsExist)
                {
                    //添加
                    IsSuc = (bool)this.baseDataManage.CallMethod("User_Flyer", "Insert", null, new object[] { Flyer });
                    if (IsSuc)
                    {
                        errMsg = "添加成功！";
                    }
                    else
                    {
                        errMsg = "添加失败！";
                    }
                }
                else
                {
                    errMsg = "该常旅客及其证件号已存在,添加失败！";
                }
            }
            else if (IsEdit == "1")
            {
                //编辑
                IsSuc = (bool)this.baseDataManage.CallMethod("User_Flyer", "Update", null, new object[] { Flyer });
                if (IsSuc)
                {
                    errMsg = "保存成功！";
                }
                else
                {
                    errMsg = "保存失败！";
                }
            }
        }
        catch (Exception ex)
        {
            errMsg = ex.Message;
            DataBase.LogCommon.Log.Error("常旅客修改出错！", ex);
        }
        finally
        {
            OutPut(string.Format("{0}@@{1}", IsSuc ? "1" : "0", errMsg));
        }
    }
}
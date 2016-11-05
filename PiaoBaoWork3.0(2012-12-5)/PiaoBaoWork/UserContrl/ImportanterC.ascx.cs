using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic.ControlBase;
using DataBase.Data;
using PbProject.Model;
using System.Text;
using PbProject.Logic;

public partial class UserContrl_ImportanterC : System.Web.UI.UserControl
{
    BaseDataManage Manage = new BaseDataManage();
    protected override void OnInit(EventArgs e)
    {
        BindDictionary();
        base.OnInit(e);
    }
    /// <summary>
    /// BindDictionary
    /// </summary>
    public void BindDictionary()
    {
        //先从参数获取可以设置的开关
        string childid = Request.QueryString["Cpychildid"];
        string currentuserid=Request["currentuserid"].ToString();
        SessionContent sessionContent = new SessionContent();
        //sessionContent = Session[currentuserid] as SessionContent;
        sessionContent = Application[currentuserid] as SessionContent;
        this.CpyNo = sessionContent.COMPANY.UninCode;
        

        string sqlParams = "SetName='"+PbProject.Model.definitionParam.paramsName.gongYingKongZhiFenXiao+"' and CpyNo='" + this.CpyNo + "'";
        List<Bd_Base_Parameters> bbpList = Manage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { sqlParams }) as List<Bd_Base_Parameters>;

        string sql = " ParentName='权限标识管理 (重要标识)' and a1=0  and ChildID in  ('-1'";//默认一个不存在的,便于拼接字符串
        if (bbpList != null && bbpList.Count>0)
        {
            string[] strsSetValue = bbpList[0].SetValue.Split('|');

            foreach (string item in strsSetValue)
            {
                if (item.Trim()!="")
                {
                      sql += ",'" + item + "'";
                }
              
            }
        }
        sql += ") order by cast(ChildID as  int)";
        //再获取开关的名称,值,绑定到控件
        List<Bd_Base_Dictionary> objList = Manage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sql }) as List<Bd_Base_Dictionary>;
        string sqlParamsfx = "SetName='" + PbProject.Model.definitionParam.paramsName.gongYingKongZhiFenXiao + "' and CpyNo='" + childid + "'";
        List<Bd_Base_Parameters> fxlist = Manage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { sqlParamsfx }) as List<Bd_Base_Parameters>;

        if (objList != null && objList.Count != 0)
        {
            foreach (Bd_Base_Dictionary item in objList)
            {
                ListItem LItem = new ListItem();
                LItem.Text = item.ChildName;
                LItem.Value = item.ChildID.ToString();
                LItem.Attributes.Add("title", item.ChildDescription);
                //LItem.Attributes.Add("title", item.ChildDescription);
                ck_ImportBox.Items.Add(LItem);
                if (fxlist != null && fxlist.Count != 0)
                {
                    if (childid.Length>=24)//二级分销以及采购都直接继承一级分销的供应控制分销参数
                    {
                        LItem.Selected = true;
                    }
                    else
                    {
                        if (fxlist[0].SetValue.ToString().Contains("|" + LItem.Value + "|"))
                        {
                            LItem.Selected = true;
                        }
                    }
                    
                }
            }
        }

      

       
    }


    private string _CpyNo = "";
    /// <summary>
    ///公司编号
    /// </summary>
    public string CpyNo
    {
        get
        {
            return _CpyNo;
        }
        set
        {
            _CpyNo = value;
        }
    }

    /// <summary>
    /// 获取和设置功能参数的值格式 "|"+值+"|"
    /// </summary>
    public string ImportantMarkStr
    {
        get
        {
            return getImportValue();
        }
        set
        {
            setImportValue(value);
        }
    }
    /// <summary>
    /// 设置供应商重要标志值 (功能参数重要标志)
    /// </summary>
    /// <returns></returns>
    private bool setImportValue(string ImportValueStr)
    {
        bool flag = false;
        try
        {
            if (!string.IsNullOrEmpty(ImportValueStr) && ImportValueStr.ToUpper() != "NULL")
            {
                List<string> listNum = new List<string>();
                string[] ImportArr = ImportValueStr.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (ImportArr.Length > 0)
                {
                    listNum.AddRange(ImportArr);
                    ListItemCollection listColl = ck_ImportBox.Items;
                    ListItem item = null;
                    for (int i = 0; i < listColl.Count; i++)
                    {
                        item = listColl[i];
                        if (listNum.Contains(item.Value.Trim()))
                        {
                            item.Selected = true;
                        }
                    }
                    flag = true;
                }
            }
        }
        catch (Exception ep)
        {
            flag = false;
        }
        return flag;
    }
    /// <summary>
    /// 机票获取重要标志的值(功能参数重要标志)
    /// </summary>
    /// <returns></returns>
    private string getImportValue()
    {
        string ImportStr = "";
        List<string> strlist = new List<string>();
        ListItemCollection listColl = ck_ImportBox.Items;
        if (listColl.Count > 0)
        {
            for (int i = 0; i < listColl.Count; i++)
            {
                if (listColl[i].Selected)
                {
                    strlist.Add(listColl[i].Value);
                }
            }
        }
        if (strlist.Count > 0)
        {
            ImportStr = "|" + string.Join("|", strlist.ToArray()).Trim('|') + "|";
        }
        return ImportStr;
    }
}
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

public partial class UserContrl_Importanter : System.Web.UI.UserControl
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
    protected void BindDictionary()
    {
        string sql = "A1=" + this.Identificationtype + " and ParentName='权限标识管理 (重要标识)' order by ChildID";
        List<Bd_Base_Dictionary> objList = Manage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sql }) as List<Bd_Base_Dictionary>;

        if (objList != null)
        {
            foreach (Bd_Base_Dictionary item in objList)
            {
                ListItem LItem = new ListItem();
                LItem.Text = item.ChildName;
                LItem.Value = item.ChildID.ToString();
                LItem.Attributes.Add("title", item.ChildDescription);
                //LItem.Attributes.Add("title", item.ChildDescription);
                ck_ImportBox.Items.Add(LItem);
            }
        }
    }


    private int _RoleType = 6;
    /// <summary>
    /// 权限角色  1=平台，2=落地运营商，3=供应商，4=分销商，5=采购商 6=所有
    /// </summary>
    public int RoleType
    {
        get
        {
            return _RoleType;
        }
        set
        {
            _RoleType = value;
        }
    }
    private int _Identificationtype = -1;
    /// <summary>
    ///标识类型 -1--不显示,0--（供应控制分销权限）1--（控制系统权限） 
    /// </summary>
    public int Identificationtype
    {
        get
        {
            return _Identificationtype;
        }
        set
        {
            _Identificationtype = value;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using PbProject.Logic;

public partial class UserContrl_UserPowerControl : System.Web.UI.UserControl
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
       // SessionContent sessionContent = new SessionContent();
       // sessionContent = Session[sessionContent.USERLOGIN] as SessionContent;
        //类型2直接是个人权限,目前需求个人权限不分等级(既所有等级的个人权限都是一样),若后期要分再做修改,YYY 2013_03_24
        string sql = "A1=2 and ParentName='权限标识管理 (重要标识)'";
        List<Bd_Base_Dictionary> objList = Manage.CallMethod("Bd_Base_Dictionary", "GetList", null, new object[] { sql }) as List<Bd_Base_Dictionary>;

        if (objList != null)
        {
            foreach (Bd_Base_Dictionary item in objList)
            {
                ListItem LItem = new ListItem();
                LItem.Text = item.ChildName;
                LItem.Value = item.ChildID.ToString();
                LItem.Attributes.Add("title", item.ChildDescription);
                ck_ImportBox.Items.Add(LItem);
            }
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
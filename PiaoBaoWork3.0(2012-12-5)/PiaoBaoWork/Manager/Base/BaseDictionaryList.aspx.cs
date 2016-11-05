using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using DataBase.Data;
using PbProject.WebCommon.Utility;

/// <summary>
/// Bd_Base_Dictionary 字典表管理
/// </summary>
public partial class Manager_Base_BaseDictionaryList : BasePage
{

    /// <summary>
    /// 加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                DataBlsList();
            }
        }
        catch (Exception ex)
        {

        }
    }
    /// <summary>
    /// 显示部分字符
    /// </summary>
    /// <param name="Str"></param>
    /// <param name="Len"></param>
    /// <returns></returns>
    public string SubChar(object Str, int Len, string replaceSchar)
    {
        string reStr = "";
        if (Str == null) return reStr;
        reStr = Str.ToString();
        if (!string.IsNullOrEmpty(reStr))
        {
            if (reStr.Length > Len)
            {
                reStr = reStr.Substring(0, Len) + " " + replaceSchar;
            }
        }
        else
        {
            reStr = "";
        }
        return reStr;
    }
    /// <summary>
    /// 绑定数据
    /// </summary>
    private void DataBlsList()
    {
        try
        {
            List<Bd_Base_Dictionary> bPaseList = new Bd_Base_DictionaryBLL().GetListGroupByParentID();
            ViewState["bPaseList"] = bPaseList;

            #region 排序

            IEnumerable<Bd_Base_Dictionary> querys = null;
            querys = from aa in bPaseList orderby aa.ParentID select aa;
            bPaseList = querys.ToList<Bd_Base_Dictionary>();

            #endregion

            List<Bd_Base_Dictionary> bPaseListNew = new List<Bd_Base_Dictionary>();
            IEnumerable<IGrouping<string, Bd_Base_Dictionary>> query = bPaseList.GroupBy(cc => cc.ParentName, cc => cc);
            foreach (IGrouping<string, Bd_Base_Dictionary> info in query)
            {
                List<Bd_Base_Dictionary> sl = info.ToList<Bd_Base_Dictionary>();//分组后的集合
                //也可循环得到分组后，集合中的对象，你可以用info.Key去控制
                foreach (Bd_Base_Dictionary set in info)
                {
                    bPaseListNew.Add(set);
                    break;
                }
            }

            if (bPaseListNew != null && bPaseListNew.Count > 0)
            {
                blsList.DataSource = bPaseListNew;
                blsList.DataTextField = "ParentName";
                blsList.DataValueField = "ParentID";
                blsList.DataBind();

                if (ViewState["parentID"] == null)
                {
                    ViewState["parentName"] = blsList.Items[0].Text;
                    ViewState["parentID"] = blsList.Items[0].Value;
                }

                DataRepBaseDictionary();
            }
            else
            {
                blsList.DataSource = null;
                blsList.DataBind();
            }
        }
        catch (Exception ex)
        {

        }
    }

    /// <summary>
    /// 绑定子级
    /// </summary>
    /// <param name="ParentID"></param>
    private void DataRepBaseDictionary()
    {
        try
        {
            List<Bd_Base_Dictionary> bPaseList = ViewState["bPaseList"] as List<Bd_Base_Dictionary>;

            Bd_Base_Dictionary[] modelArray = bPaseList.ToArray<Bd_Base_Dictionary>();
            IQueryable<Bd_Base_Dictionary> query = modelArray.AsQueryable();

            lblTitle.Text = ViewState["parentName"] == null || string.IsNullOrEmpty(ViewState["parentName"].ToString()) ? "" : ViewState["parentName"].ToString();

            if (ViewState["parentID"] != null && !string.IsNullOrEmpty(ViewState["parentID"].ToString()))
            {
                query = query.Where(c => c.ParentID == int.Parse(ViewState["parentID"].ToString()));
                query = query.OrderBy(c => c.ChildID);

                bPaseList = query.ToList<Bd_Base_Dictionary>();
                //子级数据
                ViewState["ChildPaseList"] = bPaseList;

                if (bPaseList != null && bPaseList.Count != 0)
                {
                    RepBaseDictionary.DataSource = bPaseList;
                    RepBaseDictionary.DataBind();
                    tdAddChildName.Visible = true;
                }
                else
                {
                    RepBaseDictionary.DataSource = null;
                    RepBaseDictionary.DataBind();
                    tdAddChildName.Visible = false;
                }
            }
            else
            {
                RepBaseDictionary.DataSource = null;
                RepBaseDictionary.DataBind();
                tdAddChildName.Visible = false;
            }

        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 父级选择事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void blsList_Click(object sender, BulletedListEventArgs e)
    {
        try
        {
            ViewState["parentName"] = blsList.Items[e.Index].Text;
            ViewState["parentID"] = blsList.Items[e.Index].Value;

            DataRepBaseDictionary();
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 添加父级
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddParentName_Click(object sender, EventArgs e)
    {
        string msg = "";

        try
        {
            #region 添加父级

            Bd_Base_Dictionary model = new Bd_Base_Dictionary();
            int parentID = new Bd_Base_DictionaryBLL().GetBaseDictionaryMaxParentID();//获取父级最大id 
            parentID++;

            //model.id = Guid.NewGuid();
            model.ParentID = parentID;
            model.ParentName = txtParentName.Text.Trim();
            model.ChildID = 1;
            model.ChildName = "";
            model.ChildDescription = "";
            model.Remark = "";

            bool result = (bool)baseDataManage.CallMethod("Bd_Base_Dictionary", "Insert", null, new Object[] { model });

            if (result)
            {
                msg = "添加成功！";

                DataBlsList();
            }
            else
            {
                msg = "添加失败！";
            }

            #endregion
        }
        catch (Exception)
        {
            msg = "操作失败！";
        }
        finally
        {
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }
    }

    /// <summary>
    /// 添加子级
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddChildName_Click(object sender, EventArgs e)
    {
        string msg = "";

        try
        {
            List<Bd_Base_Dictionary> bPaseList = null;

            if (txtChildName.Text.Trim() == "" || txtChildName.Text.Trim() == "")
            {
                msg = "请输入子级名称！";
            }
            else
            {
                //判断名称是否重复
                bPaseList = ViewState["ChildPaseList"] as List<Bd_Base_Dictionary>;
                foreach (var item in bPaseList)
                {
                    if (item.ChildName == txtChildName.Text.Trim())
                    {
                        msg = "子级名称不能重复！";
                        break;
                    }
                }
            }



            if (msg == "" && bPaseList != null)
            {
                #region 添加子级

                int parentId = int.Parse(ViewState["parentID"].ToString());

                Bd_Base_Dictionary model = new Bd_Base_Dictionary();
                int childID = bPaseList.Max((p) => p.ChildID);// new Bd_Base_DictionaryBLL().GetBaseDictionaryMaxChildIDByParentID(parentId);//获取父级最大id 
                childID++;

                model.id = Guid.NewGuid();
                model.ParentID = parentId;
                model.ParentName = ViewState["parentName"].ToString();
                model.ChildID = childID;
                model.ChildName = txtChildName.Text.Trim();
                model.ChildDescription = txtChildDescription.Text.Trim();
                model.Remark = txtRemark.Text.Trim();
                int A1 = 0;
                model.A1 = int.TryParse(txtA1.Text.Trim(), out A1) ? A1 : 0;

                bool result = (bool)baseDataManage.CallMethod("Bd_Base_Dictionary", "Insert", null, new Object[] { model });

                if (result)
                {
                    msg = "添加成功！";

                    DataBlsList();
                }
                else
                {
                    msg = "添加失败！";
                }
                #endregion
            }
        }
        catch (Exception)
        {
            msg = "操作失败！";
        }
        finally
        {
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }
    }
    //父ID
    public string ParentID
    {
        get
        {
            string ParentId = "";
            if (ViewState["parentID"] != null && ViewState["parentID"].ToString() != "")
            {
                ParentId = ViewState["parentID"].ToString();
            }
            return ParentId;
        }
    }

    /// <summary>
    /// 操作
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RepBaseDictionary_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        string msg = "";
        bool result = false;

        try
        {
            string id = e.CommandArgument.ToString(); //要修改的 id

            if (e.CommandName == "Update")
            {
                string strParentID = (e.Item.FindControl("txt_ParentID") as TextBox).Text.Trim();
                string strParentName = (e.Item.FindControl("txt_ParentName") as TextBox).Text.Trim();
                string strChildID = (e.Item.FindControl("txt_ChildID") as TextBox).Text.Trim();
                string strChildName = (e.Item.FindControl("txt_ChildName") as TextBox).Text.Trim();
                string strChildDescription = (e.Item.FindControl("txt_ChildDescription") as TextBox).Text.Trim();
                string strRemark = (e.Item.FindControl("txt_Remark") as TextBox).Text.Trim();
                string strA1 = "0";
                string strA3 = "0";
                HashObject paramter = new HashObject();
                //自愿与非自愿
                if (ParentID == "21" || ParentID == "27")
                {
                    CheckBox ckIsZY = e.Item.FindControl("zkIsZY") as CheckBox;
                    if (ckIsZY != null)
                    {
                        strA1 = ckIsZY.Checked ? "1" : "0";
                    }
                    CheckBox ckXePnr = e.Item.FindControl("ckXePnr") as CheckBox;
                    if (ckXePnr != null)
                    {
                        strA3 = ckXePnr.Checked ? "1" : "0";
                    }
                }
                else
                {
                    TextBox txtA1 = (e.Item.FindControl("txt_A1") as TextBox);
                    TextBox txtA3 = (e.Item.FindControl("txtXePnr") as TextBox);
                    if (txtA1 != null)
                    {
                        strA1 = txtA1.Text.Trim();
                    }
                    if (txtA3 != null)
                    {
                        strA3 = txtA3.Text.Trim();
                    }
                }
                paramter.Add("id", id);
                paramter.Add("ParentID", CommonManage.TrimSQL(strParentID));
                paramter.Add("ParentName", CommonManage.TrimSQL(strParentName));
                paramter.Add("ChildID", CommonManage.TrimSQL(strChildID));
                paramter.Add("ChildName", CommonManage.TrimSQL(strChildName));
                paramter.Add("ChildDescription", CommonManage.TrimSQL(strChildDescription));
                paramter.Add("Remark", CommonManage.TrimSQL(strRemark));
                paramter.Add("A1", CommonManage.TrimSQL(strA1));
                paramter.Add("A3", strA3);

                result = (bool)baseDataManage.CallMethod("Bd_Base_Dictionary", "Update", null, new Object[] { paramter });

                msg = result ? "修改成功！" : "修改失败!";
            }
            else if (e.CommandName == "Del")
            {
                result = (bool)baseDataManage.CallMethod("Bd_Base_Dictionary", "DeleteById", null, new Object[] { id });
                msg = result ? "删除成功！" : "删除失败!";
            }
        }
        catch (Exception)
        {
            msg = "操作失败";
        }
        finally
        {
            if (msg.Contains("成功"))
            {
                DataBlsList();
            }
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "showdialog('" + msg + "');", true);
        }
    }
    protected void RepBaseDictionary_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //退票或者废票
        if (ParentID == "21" || ParentID == "27")
        {
            //自愿与非自愿
            CheckBox ckIsZY = e.Item.FindControl("zkIsZY") as CheckBox;
            HiddenField Hid_IsZY = e.Item.FindControl("Hid_IsZY") as HiddenField;
            if (ckIsZY != null && Hid_IsZY != null)
            {
                ckIsZY.Checked = Hid_IsZY.Value == "1" ? true : false;
            }
            //申请操作是否操作编码
            CheckBox ckXePnr = e.Item.FindControl("ckXePnr") as CheckBox;
            HiddenField hid_XePnr = e.Item.FindControl("Hid_XePnr") as HiddenField;
            if (ckXePnr != null && hid_XePnr != null)
            {
                ckXePnr.Checked = (hid_XePnr.Value == "1") ? true : false;
            }
        }
    }
}
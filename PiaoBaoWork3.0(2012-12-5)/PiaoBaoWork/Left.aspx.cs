using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Collections;
using PbProject.Model;

/// <summary>
/// 
/// </summary>
public partial class Left : BasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            string type = "1";
            if (Request.QueryString["type"] != null)
            {
                type = Request.QueryString["type"].ToString();
            }
            if (Request.QueryString["v"] != null)
            {
                Page.ClientScript.RegisterStartupScript(typeof(Left), "frg", "<script language='javascript'>SetState('" + Request.QueryString["v"].ToString() + "')</script>");
            }
            CreateLeft(type);
            CreateQQ();
        }
    }

    /// <summary>
    /// 生成菜单
    /// </summary>
    /// <param name="type"></param>
    private void CreateLeft(string type)
    {
        try
        {
            PbProject.Logic.ControlBase.Bd_Base_PageBLL basePage = new PbProject.Logic.ControlBase.Bd_Base_PageBLL();
            PbProject.Logic.User.User_PermissionsBLL uPermissions = new PbProject.Logic.User.User_PermissionsBLL();
            List<Bd_Base_Page> iPostResult = new List<Bd_Base_Page>();

            int roleType = mCompany.RoleType;
            //缓存获取菜单页面
            List<Bd_Base_Page> iPost = basePage.GetListByCache(roleType);
            User_Permissions mPost = null;
            if (m_UserPermissions != null)
            {
                //Session中获取登录用户页面权限
                mPost = m_UserPermissions;
            }
            else
            {
                //数据库中读取用户权限
                mPost = uPermissions.GetById(mUser.DeptId);
            }
            string strValue = "," + mPost.Permissions.Replace("，", ",") + ",";
            string temp = "";


            //得到一级菜单数
            ArrayList listValue = new ArrayList();
            ArrayList listName = new ArrayList();

            for (int j = 0; j < iPost.Count; j++)
            {
                temp = "," + iPost[j].PageIndex + ",";

                //if (temp.Contains("138 ")) 
                //{
                // string str = temp;//测试
                //}

                if (strValue.Contains(temp) && iPost[j].ModuleIndex.ToString() == type && iPost[j].TwoMenuIndex != 0 && iPost[j].RoleType == roleType)
                {
                    if (!listValue.Contains(iPost[j].OneMenuIndex))
                    {
                        listValue.Add(iPost[j].OneMenuIndex);
                        listName.Add(iPost[j].OneMenuName);
                    }
                    iPostResult.Add(iPost[j]);
                }
            }
            //控制菜单高度
            int[] iheight = new int[listValue.Count];

            //生成二级菜单
            string[] str = new string[listValue.Count];
            for (int i = 0; i < iPostResult.Count; i++)
            {
                for (int j = 0; j < listValue.Count; j++)
                {
                    if (iPostResult[i].OneMenuIndex.ToString() == listValue[j].ToString() && iPostResult[i].TwoMenuIndex != 0)
                    {
                        if (iPostResult[i].PageURL.Contains("/StrategyGroupList.aspx"))
                        {
                            //没有显示策略组的权限不显示菜单
                        }
                        else if (iPostResult[i].PageName.Contains("高返政策"))
                        {
                            //判断是否允许放高返政策 True为有 False无
                        }
                        else
                        {
                            string tempUrl = iPostResult[i].PageURL.IndexOf('?') < 0 ? string.Format("{0}?currentuserid={1}", iPostResult[i].PageURL, this.currentuserid.Value.ToString()) : string.Format("{0}&currentuserid={1}", iPostResult[i].PageURL, this.currentuserid.Value.ToString());
                            str[j] += "<li><a href=\"" + tempUrl + "\"  target=\"ALLFrame\" style=\"height:30px;\">" + iPostResult[i].TwoMenuName + "</a></li>";
                        }
                        iheight[j]++;
                    }
                }
            }
            //追加一级菜单
            for (int j = 0; j < listValue.Count; j++)
            {
                temp = str[j];
                StringBuilder _sb = new StringBuilder("");
                str[j] = "<h3><a href=\"javascript:void(0)\">" + listName[j].ToString() + "</a></h3><div  style=\"padding: 0px;\"><ul>" + temp + _sb.ToString() + "</ul></div>";
            }
            //合并字符串
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < listValue.Count; j++)
            {
                sb.Append(str[j]);
            }
            accordion.InnerHtml = sb.ToString();
        }
        catch (Exception)
        {
        }
    }
    /// <summary>
    /// QQ列表
    /// </summary>
    /// <param name="type"></param>
    private void CreateQQ()
    {
        try
        {
            StringBuilder sb = new StringBuilder();
            string cpyno = this.mCompany.UninCode;
            string queryWhere = string.Empty;
            if (cpyno.Length >= 24)
            {
                string isWhere = string.Empty;
                switch (cpyno.Length)
                {
                    //最下层采购
                    case 30:
                        isWhere = string.Format(" CpyNo in ('{0}','{1}') and SetName='isShowDuLiInfo' order by CpyNo desc", cpyno.Substring(0, 24), cpyno.Substring(0, 18));
                        break;
                    //二级分销，
                    case 24:
                        isWhere = string.Format(" CpyNo='{0}' and SetName='isShowDuLiInfo'", cpyno.Substring(0, 18));
                        break;
                    default:
                        break;
                }
                List<Bd_Base_Parameters> isL = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { isWhere }) as List<Bd_Base_Parameters>;

                if (isL != null && isL.Count > 0)
                {
                    foreach (var item in isL)
                    {
                        string IsShowDuLiInfo = item.SetValue;
                        if (IsShowDuLiInfo.Equals("1"))
                        {
                            queryWhere = string.Format(" CpyNo in ('{0}') and SetName='cssQQ'", item.CpyNo);
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(queryWhere))
                    {
                        queryWhere = string.Format(" CpyNo='{0}' and SetName='cssQQ'", cpyno.Substring(0, 12));
                    }
                }
                else
                {
                    queryWhere = string.Format(" CpyNo='{0}' and SetName='cssQQ'", cpyno.Substring(0, 12));
                }
            }
            else if (cpyno.Length == 18)
            {
                queryWhere = string.Format(" CpyNo='{0}' and SetName='cssQQ'", cpyno.Substring(0, 12));
            }
            else if (cpyno.Length == 12)
            {
                queryWhere = string.Format(" CpyNo='{0}' and SetName='cssQQ'", cpyno.Substring(0, 6));
            }
            else if (cpyno.Length == 6)
            {
                queryWhere = string.Format(" 1!=1");
            }
            List<Bd_Base_Parameters> list = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { queryWhere }) as List<Bd_Base_Parameters>;
            if (list != null && list.Count > 0)
            {
                sb.Append("<h3><a href='javascript:void(0)'>客服中心</a></h3><div style='padding:5px 20px;text-align:left;'>");
                foreach (var Topitem in list)
                {
                    string qqList = Topitem.SetValue;
                    if (!string.IsNullOrEmpty(qqList))
                    {
                        foreach (string item in qqList.Split('@'))
                        {
                            sb.Append("<span class='spanQQ'>");
                            if (!string.IsNullOrEmpty(item))
                            {
                                string[] itemcolumns = item.Split('#');
                                if (itemcolumns.Length % 3 == 0)
                                {
                                    sb.AppendFormat(" <a href='http://wpa.qq.com/msgrd?V=3&Uin={0}&Site在线业务咨询&Menu=yes' target='_blank'><img border='0' SRC='img/QQ.gif' alt='' /> {1}</a><br/>{2}", itemcolumns[2], itemcolumns[0], itemcolumns[1]);
                                }
                            }
                            sb.Append("</span><br/>");
                        }
                    }
                }
                sb.Append("</div>");
            }
            ltl_QQ.Text = sb.ToString();
        }
        catch (Exception e)
        {

        }
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using IRemoteMethodSpace;

public partial class Base_CacheList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            this.currentuserid.Value = this.mUser.id.ToString();
        UpdateCache();
    }
    //更新缓存
    public void UpdateCache()
    {
        if (Request["ctype"] != null && Request["ctype"].ToString() != "")
        {
            bool IsRefresh = false;
            string msg = "";
            try
            {
                string cacheUrl = System.Configuration.ConfigurationManager.AppSettings["CacheUrl"];
                if (!string.IsNullOrEmpty(cacheUrl))
                {
                    IRemoteMethod remoteobj = (IRemoteMethod)Activator.GetObject(typeof(IRemoteMethod), cacheUrl);
                    string ctype = Request["ctype"].ToString();
                    //基建
                    if (ctype == "0")
                    {
                        IsRefresh = remoteobj.RefreshCache(cacheTableName.Bd_Air_Aircraft);
                    }
                    //仓位
                    else if (ctype == "1")
                    {
                        IsRefresh = remoteobj.RefreshCache(cacheTableName.Bd_Air_CabinDiscount);
                    }
                    //承运人
                    else if (ctype == "2")
                    {
                        IsRefresh = remoteobj.RefreshCache(cacheTableName.Bd_Air_Carrier);
                    }
                    //票价
                    else if (ctype == "3")
                    {
                        IsRefresh = remoteobj.RefreshCache(cacheTableName.Bd_Air_Fares);
                    }
                    //燃油
                    else if (ctype == "4")
                    {
                        IsRefresh = remoteobj.RefreshCache(cacheTableName.Bd_Air_Fuel);
                    }
                    //机场
                    else if (ctype == "5")
                    {
                        IsRefresh = remoteobj.RefreshCache(cacheTableName.Bd_Air_Airport);
                    }
                    //退改签
                    else if (ctype == "6")
                    {
                        IsRefresh = remoteobj.RefreshCache(cacheTableName.Bd_Air_TGQProvision);
                    }
                    //政策
                    else if (ctype == "7")
                    {
                        IsRefresh = remoteobj.RefreshCache(cacheTableName.Tb_Ticket_Policy);
                    }
                    //所有
                    else if (ctype == "8")
                    {
                        IsRefresh = remoteobj.RefreshCache(cacheTableName.All_Table);
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            try
            {
                Response.Clear();
                int suc = 0;
                if (IsRefresh)
                {
                    suc = 1;
                }
                Response.Write(suc + "@@" + msg);
                Response.Flush();
                Response.End();
            }
            catch (Exception)
            {
            }
        }
    }


    /// <summary>
    /// 清空
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbtnBd_Base_Dictionary_Click(object sender, EventArgs e)
    {
        try
        {
            PbProject.WebCommon.Web.Cache.CacheManage cacheManage = new PbProject.WebCommon.Web.Cache.CacheManage();
            cacheManage.ClearCache();

            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "alert('清空成功!');", true);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), DateTime.Now.Ticks.ToString(), "alert('清空失败!');", true);
        }
    }
}
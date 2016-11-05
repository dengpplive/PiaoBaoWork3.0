using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic;
using PbProject.Model;

public partial class Sys_LookBulletin : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            this.currentuserid.Value = this.mUser.id.ToString();
        if (Request.QueryString["id"] == null)
        {
            Response.Redirect("NewBulletinList.aspx?currentuserid="+this.currentuserid.Value.ToString());
        }
        else
        {
            string id = Request["id"].ToString().Trim();
            bind(id);
        }
    }
    public void bind(string id)
    {
        Bd_Base_Notice Bd_Base_NoticeModel = null;
        List<Bd_Base_Notice> NoticeList = this.baseDataManage.CallMethod("Bd_Base_Notice", "GetList", null, new object[] { string.Format(" id='{0}' ", id) }) as List<Bd_Base_Notice>;
        if (NoticeList != null && NoticeList.Count > 0)
        {
            Bd_Base_NoticeModel = NoticeList[0];
            if (Bd_Base_NoticeModel.Emergency == 1)
            {
                tle.Text = "紧急公告";
            }
            Title.Text = Bd_Base_NoticeModel.Title;
            ReleaseTime.Text = Bd_Base_NoticeModel.ReleaseTime.ToString();
            Name.Text = Bd_Base_NoticeModel.ReleaseName;
            Content.InnerHtml = Bd_Base_NoticeModel.Content.Replace("&lt;", "<").Replace("&gt;", ">");
            if (Bd_Base_NoticeModel.AttachmentFileName != null
                && Bd_Base_NoticeModel.AttachmentFileName != ""
                && Bd_Base_NoticeModel.FileAttachment != null
                && Bd_Base_NoticeModel.FileAttachment.Length > 1)
            {
                attach.InnerHtml = "<a href='DownLoadFile.aspx?did=" + Bd_Base_NoticeModel.id + "&currentuserid="+this.currentuserid.Value.ToString()+"'>下载附件</a>";
            }
        }
    }
}
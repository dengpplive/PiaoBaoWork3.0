using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.IO;

public partial class Sys_NoticeDetail : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            if (Request.QueryString["id"] != null)
            {
               string id = Request.QueryString["id"].ToString().Trim();
               bind(id);
            }
            else
            {
                Response.Redirect("NoticeList.aspx?currentuserid=" + this.currentuserid.Value.ToString());
            }
          
        }
    }
    /// <summary>
    /// 绑定公告信息
    /// </summary>
    /// <param name="id"></param>
    public void bind(string id)
    {
        Bd_Base_Notice mNotice = (baseDataManage.CallMethod("Bd_Base_Notice", "GetList", null, new Object[] { "id='" + id + "'" }) as List<Bd_Base_Notice>)[0];
        if (mNotice!=null)
        {
            Title.Text = mNotice.Title;
            ReleaseTime.Text = mNotice.ReleaseTime.ToString();
            Name.Text = mNotice.ReleaseName;
            Content.InnerHtml = mNotice.Content;

            if (mNotice.AttachmentFileName != null
                && mNotice.AttachmentFileName != ""
                && mNotice.FileAttachment != null
                && mNotice.FileAttachment.Length > 1)
            {
                attach.InnerHtml = "<a href='DownLoadFile.aspx?did=" + mNotice.id + "&currentuserid="+this.currentuserid.Value.ToString()+"'>下载附件</a>";
                //attach.Visible = true;
            }
            else
            {
                //attach.Visible = false;
            }
        }
        
    }
    /// <summary>
    /// 下载附件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lbdownfile_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["id"] != null)
        {
            Bd_Base_Notice mNotice = (baseDataManage.CallMethod("Bd_Base_Notice", "GetList", null, new Object[] { "id='" + Request.QueryString["id"] + "'" }) as List<Bd_Base_Notice>)[0];
            Byte[] bytes = mNotice.FileAttachment;
            string filename = mNotice.AttachmentFileName.Split('|')[0];
            string fileType = mNotice.AttachmentFileName.Split('|')[1];
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = fileType;
            Response.AddHeader("Content-Disposition: ", "attachment;   filename= " + HttpUtility.UrlEncode(filename));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }
       
    }
}
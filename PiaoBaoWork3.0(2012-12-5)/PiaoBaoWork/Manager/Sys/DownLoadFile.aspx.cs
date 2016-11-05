using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;

public partial class Sys_DownLoadFile : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["did"] != null && Request["did"].ToString() != "" && Request["did"].ToString() != "0")
        {
                Bd_Base_Notice mNotice = (baseDataManage.CallMethod("Bd_Base_Notice", "GetList", null, new Object[] { "id='" + Request.QueryString["did"] + "'" }) as List<Bd_Base_Notice>)[0];
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
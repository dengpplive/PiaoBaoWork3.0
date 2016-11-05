using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using DataBase.Data;
using System.IO;
using PbProject.WebCommon.Utility;
public partial class Sys_NoticeEdit : BasePage
{
    protected Bd_Base_Notice mNotice = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            btnBack.PostBackUrl = string.Format("NoticeList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            DateTime dt = DateTime.Now;
            DateTime dt1 = new DateTime(dt.Year, dt.Month, 1);
            ExpirationDateSta.Value = dt.ToString("yyyy-MM-dd");
            ExpirationDateStp.Value = dt1.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            lblid.Text = Request.QueryString["id"] == null ? "0" : Request.QueryString["id"];
            if (lblid.Text != "0")
            {
                BindNoticeinfo(Guid.Parse(lblid.Text));
                trUpdate.Visible = true;
            }
            else
            {
                trUpdate.Visible = false;
            }
        }
    }
    /// <summary>
    /// 绑定要修改的信息
    /// </summary>
    /// <param name="id"></param>
    protected void BindNoticeinfo(Guid id)
    {
        mNotice = (baseDataManage.CallMethod("Bd_Base_Notice", "GetList", null, new Object[] { "id='" + id + "'" }) as List<Bd_Base_Notice>)[0];
        if (mNotice != null)
        {
            this.Title.Text = mNotice.Title; ;
            this.ExpirationDateSta.Value = Convert.ToDateTime(mNotice.StartDate).ToString("yyyy-MM-dd");
            this.ExpirationDateStp.Value = Convert.ToDateTime(mNotice.ExpirationDate).ToString("yyyy-MM-dd");
            this.IsInternal.SelectedValue = mNotice.IsInternal.ToString();
            this.Emergency.SelectedValue = mNotice.Emergency.ToString();
            this.rbisroll.SelectedValue = mNotice.RollFlag.ToString();
            this.content1.Value =  mNotice.Content;
            ClientScript.RegisterStartupScript(this.GetType(), "", " document.getElementById('trAdd').style.display = 'none';", true);
            ViewState["BdBaseNotice"] = mNotice;
        }


    }
    /// <summary>
    /// 检查文件大小是否查过限制范围
    /// </summary>
    /// <returns></returns>
    public bool FileSizeIsOver()
    {
        bool IsOver = false;
        long fileSize = fup.PostedFile.InputStream.Length;
        if (fileSize > 4 * 1024 * 1024)
        {
            IsOver = true;
        }
        return IsOver;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btsave_Click(object sender, EventArgs e)
    {
        string msg = "";
        string FileName = fup.PostedFile.FileName;
        Byte[] bytes = null;
        string AttachName = "";//附件名
        try
        {
            if (mCompany.RoleType == 1 || mCompany.RoleType == 2)
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    bytes = new Byte[fup.PostedFile.InputStream.Length];//附件内容
                    fup.PostedFile.InputStream.Read(bytes, 0, bytes.Length);
                    string ex = System.IO.Path.GetExtension(fup.PostedFile.FileName);
                    if (FileSizeIsOver())
                    {
                        msg = "文件大小不能超过4MB";
                    }
                    else
                    {
                        if (attachName.Value.Trim() != "")
                        {
                            AttachName = attachName.Value.Trim() + ex + "|" + fup.PostedFile.ContentType;
                        }
                        else
                        {
                            AttachName = System.IO.Path.GetFileName(fup.PostedFile.FileName) + "|" + fup.PostedFile.ContentType;
                        }
                    }
                }
                if (msg == "")
                {
                    string strTitle = CommonManage.TrimSQL(Title.Text.Trim());
                    string strCon =CommonManage.TrimSQL(content1.Value);
                    Bd_Base_Notice bd_base_notice = ViewState["BdBaseNotice"] as Bd_Base_Notice;
                    if (bd_base_notice == null || lblid.Text.Trim() == "0" || lblid.Text.Trim() == "")
                    {
                        bd_base_notice = new Bd_Base_Notice();
                    }
                    bd_base_notice.Title = strTitle;
                    bd_base_notice.Content = strCon;
                    bd_base_notice.ReleaseTime = DateTime.Now;
                    bd_base_notice.Emergency = int.Parse(Emergency.SelectedItem.Value);
                    bd_base_notice.RollFlag = Convert.ToInt32(rbisroll.SelectedValue);
                    bd_base_notice.IsInternal = int.Parse(IsInternal.SelectedItem.Value);
                    bd_base_notice.ReleaseAccount = mUser.LoginName;
                    bd_base_notice.ReleaseName = mUser.UserName;
                    bd_base_notice.ReleaseCpyNo = mCompany.UninCode;
                    bd_base_notice.ReleaseCpyName = mCompany.UninAllName;
                    bd_base_notice.StartDate = DateTime.Parse(ExpirationDateSta.Value);
                    bd_base_notice.ExpirationDate = DateTime.Parse(ExpirationDateStp.Value);


                    if (lblid.Text == "0" || lblid.Text.Length == 0)
                    {
                        #region 添加
                        //平台不用审核                        
                        bd_base_notice.ClickCount = 0;
                        bd_base_notice.AttachmentFileName = AttachName;
                        bd_base_notice.FileAttachment = bytes == null ? new byte[0] : bytes;
                        bd_base_notice.CallBoardType = mCompany.UninCode.Length == 6 ? 1 : 2;
                        //该标题是否已存在
                        bool IsExist = (bool)baseDataManage.CallMethod("Bd_Base_Notice", "IsExist", null, new Object[] { string.Format(" Title='{0}' and ReleaseCpyNo='{1}' ", strTitle, mCompany.UninCode) });
                        if (IsExist)
                        {
                            msg = "该标题已经存在，添加失败！";
                        }
                        else
                        {
                            msg = (bool)baseDataManage.CallMethod("Bd_Base_Notice", "Insert", null, new Object[] { bd_base_notice }) == true ? "添加成功" : "添加失败";
                        }
                        #endregion
                    }
                    else
                    {
                        #region 修改
                        bd_base_notice.id = Guid.Parse(lblid.Text);
                        switch (rbtnlFujian.SelectedValue)
                        {
                            case "0"://取消附件                               
                                bd_base_notice.FileAttachment = new byte[0];
                                bd_base_notice.AttachmentFileName = "";
                                break;
                            case "2"://重置附件                              
                                bd_base_notice.FileAttachment = bytes == null ? new byte[0] : bytes;
                                if (AttachName != "")
                                {
                                    bd_base_notice.AttachmentFileName = AttachName;
                                }
                                break;
                        }
                        msg = (bool)baseDataManage.CallMethod("Bd_Base_Notice", "Update", null, new Object[] { bd_base_notice }) == true ? "更新成功" : "更新失败";
                        #endregion
                    }
                }
            }
            else
            {
                msg = "当前用户无发布公告权限";
            }

        }
        catch (Exception)
        {
            msg = "操作异常";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);

    }
}
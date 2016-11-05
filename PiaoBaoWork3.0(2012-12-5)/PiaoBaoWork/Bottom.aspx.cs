using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;

public partial class Bottom : BasePage
{
    public bool isp = false;
    public bool isf = false;
    public string time = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //订单提醒开关
                if (mUser != null && mCompany != null && mCompany.RoleType == 2)
                {
                    PromptTime.Value = mCompany.PromptTime.ToString();
                    //0 关闭 1开启
                    IsPrompt.Value = "0";
                    //如果是员工
                    if (mUser.IsAdmin == 1)
                    {
                        if (mCompany.IsEmpPrompt == 1)
                        {
                            //0 关闭 1开启
                            IsPrompt.Value = "1";
                        }
                    }
                    else
                    {
                        //管理员
                        if (mCompany.IsPrompt == 1)
                        {
                            //0 关闭 1开启
                            IsPrompt.Value = "1";
                        }
                    }
                    Hid_RoleType.Value = mCompany.RoleType.ToString();
                    Hid_CpyNo.Value = mCompany.UninCode;
                }
            }
        }
        catch (Exception)
        {
        }
    }
}
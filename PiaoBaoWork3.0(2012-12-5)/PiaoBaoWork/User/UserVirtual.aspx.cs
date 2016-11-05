using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;

/// <summary>
/// 账号余额度
/// </summary>
public partial class User_UserVirtual : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
        try
        {
            if (!IsPostBack)
            {
                UserVirtual();
            }
        }
        catch (Exception)
        {

        }
    }

    public void UserVirtual()
    {
        try
        {
            string tempSqlWhere = " UninCode='" + mUser.CpyNo + "'";
            List<User_Company> payDetailList = new PbProject.Logic.User.User_CompanyBLL().GetListBySqlWhere(tempSqlWhere);

            if (payDetailList != null && payDetailList.Count == 1)
            {
                lblCpyName.Text = payDetailList[0].UninAllName.ToString();
                lblAccountMoney.Text = payDetailList[0].AccountMoney.ToString();
                lblMaxDebtMoney.Text = payDetailList[0].MaxDebtMoney.ToString();
                lblMaxDebtDays.Text = payDetailList[0].MaxDebtDays.ToString();
            }
        }
        catch (Exception)
        {

        }
    }
}
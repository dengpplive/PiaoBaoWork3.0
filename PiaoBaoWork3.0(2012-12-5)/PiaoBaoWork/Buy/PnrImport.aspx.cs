using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic.Buy;
using PbProject.Model;
using System.Data;
using PbProject.WebCommon.Utility;
using PbProject.Dal.SQLEXDAL;
using PbProject.Logic.ControlBase;
using PbProject.WebCommon.Utility.Encoding;
public partial class Buy_PnrImport : BasePage
{
    #region 属性
    /// <summary>
    /// 获取控制系统权限 
    /// </summary>
    public string KongZhiXiTong
    {
        get
        {
            string result = "";
            if (mCompany != null && mCompany.RoleType > 1)
            {
                result = BaseParams.getParams(supBaseParametersList).KongZhiXiTong;
            }
            return result;
        }
    }
    /// <summary>
    /// 供应控制分销开关 
    /// </summary>
    public string GongYingKongZhiFenXiao
    {
        get
        {
            string result = "";
            if (mCompany != null && mCompany.RoleType > 1)
            {
                result = BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
            }
            return result;
        }
    }

    /// <summary>
    /// 页面需要传递的对象
    /// </summary>
    public override object PageObj
    {
        get
        {
            return base.PageObj;
        }
        set
        {
            base.PageObj = value;
        }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!IsPostBack)
        {
            this.currentuserid.Value = this.mUser.id.ToString();
            //角色类型
            Hid_UserRoleType.Value = mCompany.RoleType.ToString();         
            if (mCompany.RoleType >= 4)
            {
                //关闭pnr导入是否合并
                if (KongZhiXiTong != null && KongZhiXiTong.Contains("|59|"))
                {
                    trPNRAndPata.Visible = false;
                    trPNRInfo.Visible = true;
                    trPATAInfo.Visible = true;
                    Hid_PnrConIsAll.Value = "0";
                }
                else
                {
                    trPNRAndPata.Visible = true;
                    trPNRInfo.Visible = false;
                    trPATAInfo.Visible = false;
                    Hid_PnrConIsAll.Value = "1";
                }
            }
            else
            {
                trPNRAndPata.Visible = true;
                trPNRInfo.Visible = false;
                trPATAInfo.Visible = false;
                Hid_PnrConIsAll.Value = "1";
            }
            //开启儿童编码必须关联成人编码或者成人订单号
            if (KongZhiXiTong != null && KongZhiXiTong.Contains("|95|"))//开启儿童编码必须关联成人编码或者成人订单号
            {
                Hid_CHDOPENAsAdultOrder.Value = "1";
            }
            else
            {
                Hid_CHDOPENAsAdultOrder.Value = "0";
            }
        }
        txtPNRAndPata1.Text = " 1.刘艳 HYPQP5  \r 2.  CA8208 Q   TH29NOV  CTUWUH HK1   1150 1325          E T2-- \r 3.CTU/T CTU/T 028-5566222/CTU QI MING INDUSTRY CO.,LTD/TONG LILI ABCDEFG   \r 4.25869587 \r 5.TL/1050/29NOV/CTU324 \r 6.SSR FOID CA HK1 NI428022198810122547/P1  \r 7.SSR ADTK 1E BY CTU14NOV12/2118 OR CXL CA ALL SEGS\r 8.OSI CA CTCT18708178001/A \r 9.RMK CA/NYDD3E\r10.CTU324   \r\r>PAT:A  \r01 Q FARE:CNY550.00 TAX:CNY50.00 YQ:CNY140.00  TOTAL:740.00 \rSFC:01 \r\r";
    }
}
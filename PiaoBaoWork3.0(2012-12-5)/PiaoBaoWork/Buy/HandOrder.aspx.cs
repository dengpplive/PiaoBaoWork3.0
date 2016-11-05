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

public partial class Buy_HandOrder : BasePage
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
        this.currentuserid.Value = this.mUser.id.ToString();
        if (!IsPostBack)
        {
            //角色类型
            Hid_UserRoleType.Value = mCompany.RoleType.ToString();
            //开启儿童编码必须关联成人编码或者成人订单号
            if (KongZhiXiTong != null && KongZhiXiTong.Contains("|95|"))//开启儿童编码必须关联成人编码或者成人订单号
            {
                Hid_CHDOPENAsAdultOrder.Value = "1";
            }
            else
            {
                Hid_CHDOPENAsAdultOrder.Value = "0";
            }
            //关闭pnr导入是否合并
            if (KongZhiXiTong != null && KongZhiXiTong.Contains("|59|"))
            {
                tr0_IsMerge0.Visible = true;
                tr0_IsMerge1.Visible = true;
                tr1_IsMerge.Visible = false;
                Hid_PnrConIsAll.Value = "0";
            }
            else
            {
                tr0_IsMerge0.Visible = false;
                tr0_IsMerge1.Visible = false;

                tr1_IsMerge.Visible = true;
                Hid_PnrConIsAll.Value = "1";
            }

            if (mCompany.RoleType == 1)
            {
                //绑定供应
                BindGY();
            }
            //绑定客户
            BindKH();
        }
        txtPNRAndPata1.Text = " 1.刘艳 HYPQP5  \r 2.  CA8208 Q   TH29NOV  CTUWUH HK1   1150 1325          E T2-- \r 3.CTU/T CTU/T 028-5566222/CTU QI MING INDUSTRY CO.,LTD/TONG LILI ABCDEFG   \r 4.25869587 \r 5.TL/1050/29NOV/CTU324 \r 6.SSR FOID CA HK1 NI428022198810122547/P1  \r 7.SSR ADTK 1E BY CTU14NOV12/2118 OR CXL CA ALL SEGS\r 8.OSI CA CTCT18708178001/A \r 9.RMK CA/NYDD3E\r10.CTU324   \r\r>PAT:A  \r01 Q FARE:CNY550.00 TAX:CNY50.00 YQ:CNY140.00  TOTAL:740.00 \rSFC:01 \r\r";
    }
    //绑定供应和落地运营商
    public void BindGY()
    {
        if (mCompany.RoleType == 1)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            SQLEXDAL_Base sqlexdal_base = new SQLEXDAL_Base();
            DataTable table = sqlexdal_base.GetGYEmpolyees();
            selGY_0.Items.Clear();
            selGY_0.Items.Add(new ListItem("---请选择落地运营商---", ""));
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    if (dr["RoleType"].ToString() == "2")
                    {
                        ListItem li = new ListItem(dr["LoginName"].ToString() + "-" + dr["UninAllName"].ToString(), dr["UninCode"].ToString() + "@" + dr["LoginName"].ToString() + "@" + dr["UninAllName"].ToString() + "@" + dr["Uid"].ToString() + "@" + dr["Cid"].ToString());
                        selGY_0.Items.Add(li);
                    }
                }
            }
            sw.Stop();
            string strCarry = sw.Elapsed.ToString();
            PnrAnalysis.LogText.LogWrite("BindKH:" + strCarry, "HandleTime");
        }
    }
    //绑定客户
    public void BindKH()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        if (mCompany.RoleType != 1)
        {
            SQLEXDAL_Base sqlexdal_base = new SQLEXDAL_Base();
            DataTable table = sqlexdal_base.GetGYLowerEmpolyees(mCompany.UninCode);
            selKH_0.Items.Clear();
            selKH_0.Items.Add(new ListItem("---请选择客户名称---", ""));
            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow dr in table.Rows)
                {
                    //LoginName：登录账号 UninCode：公司编号  Uid:用户id Cid:用户所在公司id UninAllName:公司全称
                    ListItem li = new ListItem(dr["LoginName"].ToString() + "-" + dr["UninAllName"].ToString(), dr["UninCode"].ToString() + "@" + dr["LoginName"].ToString() + "@" + dr["UninAllName"].ToString() + "@" + dr["Uid"].ToString() + "@" + dr["Cid"].ToString());
                    selKH_0.Items.Add(li);
                }
            }
        }
        sw.Stop();
        string strCarry = sw.Elapsed.ToString();
        PnrAnalysis.LogText.LogWrite("BindKH:" + strCarry, "HandleTime");
    }

    //PNR导入
    protected void btnH_PNRImport_Click(object sender, EventArgs e)
    {
        Import(0);
    }
    /// <summary>
    /// PNR入库记账
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnH_PNRImport1_Click(object sender, EventArgs e)
    {
        Import(2);
    }
    //PNR内容导入
    protected void btnH_PNRConImport_Click(object sender, EventArgs e)
    {
        Import(1);
    }
    /// <summary>
    /// 1PNR内容导入 0PNR导入 2pnr入库记账
    /// </summary>
    /// <param name="type"></param>
    public void Import(int type)
    {
        bool IsResponse = false;
        try
        {
            string strKongZhiXiTong = "", strGongYingKongZhiFenXiao = "";
            PbProject.Model.ConfigParam configParam = null;
            PbProject.Model.User_Company msupCompany = null;
            PbProject.Model.User_Company mcompany = null;
            PbProject.Model.User_Employees muser = null;
            string strGY = Hid_GY.Value;
            string strKH = Hid_KH.Value;
            if (mCompany.RoleType == 1)
            {
                //平台
                if (!string.IsNullOrEmpty(strGY))
                {
                    //UninCode-LoginName-UninAllName-uid-cid
                    string[] strArr = strGY.Split('@');
                    msupCompany = this.baseDataManage.CallMethod("User_Company", "GetById", null, new object[] { strArr[4] }) as User_Company;
                    if (msupCompany != null)
                    {
                        List<Bd_Base_Parameters> GYParameters = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + msupCompany.UninCode + "'" }) as List<Bd_Base_Parameters>;
                        if (GYParameters != null)
                        {
                            strKongZhiXiTong = BaseParams.getParams(GYParameters).KongZhiXiTong;
                            strGongYingKongZhiFenXiao = BaseParams.getParams(GYParameters).GongYingKongZhiFenXiao;
                            configParam = Bd_Base_ParametersBLL.GetConfigParam(GYParameters);
                        }
                    }
                }
            }
            else
            {
                //供应
                strKongZhiXiTong = BaseParams.getParams(supBaseParametersList).KongZhiXiTong;
                strGongYingKongZhiFenXiao = BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
                configParam = this.configparam;
                msupCompany = this.mSupCompany;
            }
            //选择客户
            if (!string.IsNullOrEmpty(strKH))
            {
                //UninCode-LoginName-UninAllName-uid-cid
                string[] strArr = strKH.Split('@');
                if (strArr.Length == 5)
                {
                    muser = this.baseDataManage.CallMethod("User_Employees", "GetById", null, new object[] { strArr[3] }) as User_Employees;
                    List<User_Company> uCompanyList = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + muser.CpyNo + "'" }) as List<User_Company>;

                    if (uCompanyList != null && uCompanyList.Count > 0)
                        mcompany = uCompanyList[0];
                }
            }
            PnrImportParam Param = new PnrImportParam();
            Param.m_UserInfo = muser;
            Param.m_CurCompany = mcompany;
            Param.m_SupCompany = msupCompany;
            Param.m_LoginUser = mUser;
            Param.m_LoginCompany = mCompany;
            PnrImportManage Mange = new PnrImportManage(muser, mcompany, msupCompany, strKongZhiXiTong, strGongYingKongZhiFenXiao, configParam);
            //是否生成订单 否
            Param.IsCreateOrder = false;
            Param.Source = 1;//后台
            Param.RoleType = mCompany.RoleType.ToString();
            Param.OrderId = Hid_OrderId.Value;
            if (type == 0 || type == 2)
            {
                //是否PNR入库记账
                Param.IsImportJZ = type == 2 ? 1 : 0;
                //Pnr导入 或者PNR入库记账
                Param.Pnr = (type == 2) ? txtH_PNR3.Value.Trim() : txtH_PNR.Value.Trim();
                if (Hid_IsBigCode.Value == "1")
                {
                    Param.ImportTongDao = 3;
                    Param.BigPnr = txtH_PNR.Value.Trim();
                }
            }
            else if (type == 1)
            {
                //Pnr内容导入
                Param.ImportTongDao = 4;
                //关闭PNR导入合并 
                if (KongZhiXiTong != null && KongZhiXiTong.Contains("|59|"))
                {
                    Param.IsMerge = 0;//未合并
                    Param.RTData = pnrCon.Value.Trim().Replace("'", "");
                    Param.PATData = patCon.Value.Trim().Replace("'", "");
                }
                else
                {
                    Param.IsMerge = 1;//合并
                    Param.RTAndPatData = txtPNRAndPata.Text.Trim().Replace("'", "");
                }
            }
            bool IsSuc = Mange.GetImportPnrInfo(Param);
            if (!IsSuc)
            {
                #region 出错提示

                Hid_OrderId.Value = "";
                Param.SecondPM.ErrCode = "0";
                if (Param.IsNextOK == 1)
                {
                    //继续操作
                    Param.SecondPM.OpType = "1";
                }
                else
                {
                    Param.SecondPM.Msg = Param.TipMsg;
                    //提示
                    Param.SecondPM.OpType = "0";
                }
                string result = JsonHelper.ObjToJson<PnrImportParam>(Param);
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showMsg('" + escape(Param.SecondPM.Msg) + "',{t:2,type:" + type + ",code:\"" + escape(result) + "\"});ShowDiv(false);", true);

                #endregion
            }
            else
            {
                //页面间传递数据对象
                ////成功显示数据
                //ViewState["Param"] = Param;
                //将数据传入到指定页面处理                                               
                this.PageObj = Param;
                //方案一
                //Server.Transfer("HandPnrImport.aspx", true);
                //Response.Redirect("HandPnrImport.aspx", false);
                //方案二
                System.IO.StringWriter sw = new System.IO.StringWriter();
                Server.Execute("HandPnrImport.aspx?currentuserid=" + this.currentuserid.Value, sw);
                IsResponse = true;
                Response.Clear();
                Response.Write(sw.ToString());
                Response.End();
            }
        }
        catch (Exception ex)
        {
            if (!IsResponse)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showMsg('页面异常！');ShowDiv(false);", true);
            }
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PbProject.Logic;

public partial class Buy_BackList : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.currentuserid.Value = Request["currentuserid"].ToString();
            hidIsShowShare2.Value = Request.Form["hidIsShowShare"].ToString();
            Hid_travel.Value = Request.Form["TravelType"].ToString();
            if (Hid_travel.Value == "3")
            {
                Conntitle.Style["display"] = "block";
                ConnTxt.Style["display"] = "block";
                spBack.InnerText = "中转时间：";
            }
   
            Inits();
            TBInfo();
            string os = "";
            try
            {
                int num = 0;
                PbProject.Logic.Buy.AirQurey a = new PbProject.Logic.Buy.AirQurey();
                PbProject.Model.definitionParam.SelectCityParams selectCityParams = new PbProject.Model.definitionParam.SelectCityParams();
                selectCityParams.fcity = hiStart.Value;
                selectCityParams.mcity = "";
                selectCityParams.tcity = hiTarget.Value;
                selectCityParams.time = txtReturnTime.Value;
                selectCityParams.Totime = txtBeginTime.Value;
                selectCityParams.cairry = ViewState["Carryer"].ToString().Split('^')[0];
                selectCityParams.TravelType =int.Parse(Hid_travel.Value); 
                selectCityParams.num = num;
                selectCityParams.mEmployees = mUser;
                selectCityParams.mCompany = mCompany;
                selectCityParams.IsShowGX = bool.Parse(hidIsShowShare2.Value);
                os = a.Start(selectCityParams);
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialogmsg('查询失败！请重新查询')", true);
                return;
            }
            string outstr = "";

            if (os == "")
            {
                return;
            }
            else
            {
                outstr = "<script language=\"javascript\">getAirInfo('" + os + "');</script>";

                showDiv.InnerHtml = outstr;
            }
        }
    }
    /// <summary>
    /// 初始化赋值
    /// </summary>
    private void Inits()
    {
        //承运人
        ViewState["Carryer"] = Request.Form["Carryer"].ToString();
        //航班号
        ViewState["FlyNo"] = Request.Form["FlyNo"].ToString();
        //机型
        ViewState["Aircraft"] = Request.Form["Aircraft"].ToString();
        //起止时间
        ViewState["Time"] = Request.Form["Time"].ToString();
        //起止城市
        ViewState["City"] = Request.Form["City"].ToString();
        //经停
        //ViewState["IsStop"] = Request.Form["A6"].ToString();
        //机建
        ViewState["ABFee"] = Request.Form["ABFee"].ToString();
        //燃油
        ViewState["FuelAdultFee"] = Request.Form["FuelAdultFee"].ToString();
        //折扣
        ViewState["DiscountRate"] = Request.Form["DiscountRate"].ToString();
        //座位数
        ViewState["TickNum"] = Request.Form["TickNum"].ToString();
        //销售价
        ViewState["XSFee"] = Request.Form["XSFee"].ToString();
        //政策
        //ViewState["Policy"] = Request.Form["A12"].ToString();
        //里程
        ViewState["Mileage"] = Request.Form["Mileage"].ToString();
        //舱位
        ViewState["Cabin"] = Request.Form["Cabin"].ToString();
        //餐饮
        //ViewState["IsFood"] = Request.Form["A15"].ToString();
        //票面价
        ViewState["FareFee"] = Request.Form["FareFee"].ToString();
        //有无电子票
        //ViewState["IsETicket"] = Request.Form["A17"].ToString();
        //客规
        ViewState["Reservation"] = Request.Form["Reservation"].ToString();
        //共享
        //ViewState["IsFlight"] = Request.Form["A19"].ToString();
        //政策信息
        //ViewState["PolicyInfo"] = Request.Form["A20"].ToString();
        //特价信息
        //ViewState["PolicySpInfo"] = Request.Form["A21"].ToString();
       //返回时间
        ViewState["ReturnTime"] = Request.Form["ReturnTime"].ToString();

        //特价类型
        ViewState["SpecialType"] = Request.Form["SpecialType"].ToString();
        //行程类型
        ViewState["TravelType"] = Request.Form["TravelType"].ToString();
        //航站楼
        ViewState["Terminal"] = Request.Form["Terminal"].ToString();
        //中转城市
        ViewState["hidcity"] = Request.Form["hidcity"].ToString();
      
        CheckUrl();
    }
    private void CheckUrl()
    {

        //提供数据,以便理解逻辑 
        //单程城市  ViewState["City"]=CTU-PEK^成都-北京
        //往返      ViewState["City"]=CTU-PEK^成都-北京
        //联乘      ViewState["City"]=CTU-XIY^成都-西安 ViewState["hidcity"]=PEK-北京(首都)

        if (Hid_travel.Value == "3")//联成
        {
            txtStart.Value = ViewState["City"].ToString().Split('^')[1].Split('-')[0];//界面显示的出发城市=成都
            hiStart.Value = ViewState["City"].ToString().Split('^')[0].Split('-')[1];//实际出发城市存值(中转城市值)=XIY 

            txtConn.Value = ViewState["City"].ToString().Split('^')[1].Split('-')[1];//界面显示的中转城市(第一程中转城市)=西安 
            hiConn.Value = ViewState["City"].ToString().Split('^')[0].Split('-')[1];//实际存值=XIY

            txtTarget.Value = ViewState["hidcity"].ToString().Split('-')[1];//界面显示的到达城市=北京
            hiTarget.Value = ViewState["hidcity"].ToString().Split('-')[0];//实际到达城市存值=PEK

            //存 出发,中转,到达 供匹配政策使用
            hidFCityCode.Value = ViewState["City"].ToString().Split('^')[0].Split('-')[0];//CTU
            hidMCityCode.Value = ViewState["City"].ToString().Split('^')[0].Split('-')[1];//XIY
            hidTCityCode.Value = ViewState["hidcity"].ToString().Split('-')[0];//PEK
           
            txtBeginTime.Value = ViewState["Time"].ToString().Split('=')[0];//第一程出发时间
            txtReturnTime.Value = ViewState["ReturnTime"].ToString();//第二程出发时间
        }
        if (Hid_travel.Value == "2")//往返
        {
            txtStart.Value = ViewState["City"].ToString().Split('^')[1].Split('-')[0];//界面显示的出发城市(第一程出发城市)=成都
            hiStart.Value = ViewState["City"].ToString().Split('^')[0].Split('-')[1];//第二程实际出发城市存值(返回城市值)=PEK

            txtConn.Value = "";
            hiConn.Value = "";

            txtTarget.Value = ViewState["City"].ToString().Split('^')[1].Split('-')[1];//界面显示的到达城市(第一程出发城市)=北京
            hiTarget.Value = ViewState["City"].ToString().Split('^')[0].Split('-')[0];//第二程实际出发城市存值(返回城市值)=CTU
           
            //存 出发,中转,到达 供匹配政策使用
            hidFCityCode.Value = ViewState["City"].ToString().Split('^')[0].Split('-')[0];//CTU
            hidMCityCode.Value = "";
            hidTCityCode.Value = ViewState["City"].ToString().Split('^')[0].Split('-')[1];//PEK
        }
        txtBeginTime.Value = ViewState["Time"].ToString().Split('=')[0];//第一程出发时间
        txtReturnTime.Value = ViewState["ReturnTime"].ToString();//第二程出发时间

        string info = ViewState["Carryer"] +//0 承运人
            "|" + ViewState["FlyNo"] + //1 航班号
            "|" + ViewState["Aircraft"] + //2  机型
            "|" + ViewState["Time"] + //3  起止时间
            "|" + ViewState["City"] +//4 起止城市
            //"|" + ViewState["IsStop"] + //5
            "|" + ViewState["ABFee"] + //5机建
            "|" + ViewState["FuelAdultFee"] +//6燃油
            "|" + ViewState["DiscountRate"] + //7折扣
            "|" + ViewState["TickNum"] +//8座位数
            "|" + ViewState["XSFee"] + //9销售价
            "|" + ViewState["FareFee"] + //10票面价
            //"|" + ViewState["Policy"] + //11
            "|" + ViewState["Mileage"] + //11里程
            "|" + ViewState["Cabin"] + //12舱位
            //"|" + ViewState["IsFood"] + //14
         
            //"|" + ViewState["IsETicket"] + //16
            "|" + ViewState["Reservation"]+//13客规
            //"|" + ViewState["IsFlight"] + //18
            //"|" + ViewState["PolicyInfo"] + //19
            //"|" + ViewState["PolicySpInfo"];//20
            "|" + ViewState["SpecialType"] + //14
            "|" + ViewState["TravelType"] + //15
            "|" + ViewState["Terminal"];//16
        FlyInfo.Value = info;
    }
    /// <summary>
    /// 去程信息显示绑定
    /// </summary>
    private void TBInfo()
    {
        decimal totl = decimal.Parse(ViewState["XSFee"].ToString()) + decimal.Parse(ViewState["ABFee"].ToString()) + decimal.Parse(ViewState["FuelAdultFee"].ToString());
        string tbstr = "<table><tr><td colspan=\"7\"><h5>去程航班信息</h5></td><td align=\"right\" style=\"padding-right:10px\"></td></tr>";
        tbstr += "<tr><td class=\"td1\">航班信息：</td><td class=\"td2\">" + ViewState["City"].ToString().Split('^')[1] + "</td>";
        tbstr += "<td class=\"td1\">出发日期：</td><td class=\"td2\">" + ViewState["Time"].ToString().Split('=')[0] + "</td>";
        tbstr += "<td class=\"td1\">起抵时间：</td><td class=\"td2\">" + ViewState["Time"].ToString().Split('=')[1] + "-" + ViewState["Time"].ToString().Split('=')[2] + "</td>";
        tbstr += "<td class=\"td1\">航班号：</td><td class=\"td2\">" + ViewState["Carryer"].ToString().Split('^')[0] + ViewState["FlyNo"].ToString() + "</td></tr>";
        tbstr += "<tr><td class=\"td1\">机票价格：</td><td class=\"td3\">" + decimal.Parse(ViewState["XSFee"].ToString()).ToString("f2") + "</td>";
        tbstr += "<td class=\"td1\">机场建设费：</td><td class=\"td3\">" + decimal.Parse(ViewState["ABFee"].ToString()).ToString("f2") + "</td>";
        tbstr += "<td class=\"td1\">燃油附加费：</td><td class=\"td3\">" + ViewState["FuelAdultFee"].ToString() + "</td>";
        tbstr += "<td class=\"td1\">去程合计：</td><td class=\"td3\">" + totl.ToString("f2") + "</td></tr></table>";
        ClientScript.RegisterStartupScript(this.GetType(), "", "FI('" + tbstr + "');", true);

    }
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        ViewState["ReturnTime"] = txtReturnTime.Value;
        CheckUrl();
        TBInfo();
        //txtBeginTime.Text = ViewState["Time"].ToString().Split('=')[0];
        if (txtStart.Value.Trim() == "" || txtStart.Value.Trim() == "中文/拼音")
        {
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialogmsg('请输入出发城市！');", true);
            return;
        }
        if (txtTarget.Value.Trim() == "" || txtTarget.Value.Trim() == "中文/拼音")
        {
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialogmsg('请输入抵达城市！');", true);
            return;
        }
        if (txtStart.Value.Trim() == txtTarget.Value.Trim())
        {
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialogmsg('抵达城市不能和出发城市一致！');", true);
            return;
        }
        if (txtBeginTime.Value.Trim() == "")
        {
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialogmsg('请输入出发时间！');", true);
            return;
        }
        if (DateTime.Parse(txtBeginTime.Value.Trim()) < DateTime.Today)
        {
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialogmsg('出发时间必须大于今天！');", true);
            return;
        }
        if (DateTime.Parse(txtBeginTime.Value.Trim()) <= DateTime.Parse(txtReturnTime.Value.Trim()))
        {
            ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialogmsg('出发时间必须大于返回或中转时间！');", true);
            return;
        }
        //if (DateTime.Parse(ViewState["Time"].ToString().Split('=')[0]) >= DateTime.Parse(txtReturnTime.Value.Trim()))
        //{
        //    ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "showdialogmsg('往返时间必须大于出发时间！');", true);
        //    return;
        //}
        //AirQuery aq = new AirQuery();
        //PiaoBao.BLLLogic.Interface.TestInterface tif = PiaoBao.BLLLogic.Factory_Air.CreateTestInterface(); 
        //string tra = "";
        string os = "";
        try
        {
            int num = 0;
            PbProject.Logic.Buy.AirQurey a = new PbProject.Logic.Buy.AirQurey();
            PbProject.Model.definitionParam.SelectCityParams selectCityParams = new PbProject.Model.definitionParam.SelectCityParams();
            selectCityParams.fcity = hiStart.Value;
            selectCityParams.mcity = "";
            selectCityParams.tcity = hiTarget.Value;
            selectCityParams.time = txtReturnTime.Value;
            selectCityParams.Totime = txtBeginTime.Value;
            selectCityParams.cairry = ViewState["Carryer"].ToString().Split('^')[0];
            selectCityParams.TravelType = int.Parse(Hid_travel.Value); 
            selectCityParams.num = num;
            selectCityParams.mEmployees = mUser;
            selectCityParams.mCompany = mCompany;
            selectCityParams.IsShowGX = bool.Parse(hidIsShowShare2.Value);
            os = a.Start(selectCityParams);


            //PiaoBao.BLLLogic.Interface.AirQurey a = new PiaoBao.BLLLogic.Interface.AirQurey();
            //os = a.Start(hiStart.Value, "", hiTarget.Value, txtReturnTime.Value, txtBeginTime.Value, ViewState["Carryer"].ToString().Split('^')[0], 2, 3, ref num, mUser, mCompany, bool.Parse(hidIsShowShare2.Value));
            //tra = aq.GetQueryData(hiStart.Value, hiTarget.Value, txtReturnTime.Text, ViewState["Carryer"].ToString().Split('^')[0], mUser);
            //tra = PiaoBao.BLLLogic.Factory_Air.CreateResolve().ResolveStr(tra, txtReturnTime.Text, hiStart.Value + hiTarget.Value, 1, ViewState["Carryer"].ToString().Split('^')[0]);
            //os = tif.OutString(mCompany, tra, 3, ref num, "");
        }
        catch
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialogmsg('查询失败！请重新查询')", true);
            return;
        }
        string outstr = "";
        if (os == "")
        {
            return;
        }
        else
        {
            outstr = "<script language=\"javascript\">getAirInfo('" + os + "');</script>";
            showDiv.InnerHtml = outstr;
        }
    }
}
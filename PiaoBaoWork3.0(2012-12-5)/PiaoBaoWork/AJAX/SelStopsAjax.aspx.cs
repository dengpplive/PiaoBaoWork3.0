using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PnrAnalysis;
using PbProject.Model;
using PbProject.Logic.PID;
using PbProject.Logic.ControlBase;
using PnrAnalysis.Model;
using PbProject.Logic.Policy;

/// <summary>
/// Ajax异步查询经停
/// </summary>
public partial class Ajax_SelStopsAjax : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string sReturn = "";
        try
        {
            if (Request.QueryString["stime"] != null && Request.QueryString["flyno"] != null && Request.QueryString["guid"] != null)
            {
                string sdates = Request.QueryString["stime"];
                string flynos = Request.QueryString["flyno"];
                string guid = Request.QueryString["guid"];
                sReturn = selStops(sdates, flynos, guid);
            }
        }
        catch (Exception)
        {
            throw;
        }
        Response.Write(sReturn);
    }
    /// <summary>
    /// 通过指令查询经停城市
    /// </summary>
    /// <param name="sdate"></param>
    /// <param name="flyno"></param>
    /// <returns></returns>
    private string selStops(string sdate, string flyno,string guid)
    {
        try
        {
            flyno = flyno.Replace("*", "");
            string dd = DateTime.Parse(sdate).ToString("dd");
            string mm = DateTime.Parse(sdate).ToString("MM");
            string zhiling = "FF:" + flyno + "/" + dd + strValue(mm);


            //格式化编码内容类
            PnrAnalysis.FormatPNR pnrformat = new PnrAnalysis.FormatPNR();
            string strVale = string.Empty;
            ParamEx pe = new ParamEx();
            pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
            SendInsManage SendManage = new SendInsManage(mUser.LoginName, mCompany.UninCode, pe, this.configparam);
            string Office = this.configparam.Office.Split('^')[0];
            strVale = SendManage.Send(zhiling, ref Office, 9);
            string msg="";
            LegStop ls=pnrformat.GetStop(strVale, out msg);

            if (msg == "")
            {
                strVale = strReturn(ls);
            }
            return strVale + "|" + guid;
        }
        catch (Exception ex)
        {
            return "";
        }
    }
    /// <summary>
    /// 月份转换,数字转英文
    /// </summary>
    /// <returns></returns>
    private string strValue(string str)
    {
        if (str == "1" || str == "01")
            str = "JAN";
        else if (str == "2" || str == "02")
            str = "FEB";
        else if (str == "3" || str == "03")
            str = "MAR";
        else if (str == "4" || str == "04")
            str = "APR";
        else if (str == "5" || str == "05")
            str = "MAY";
        else if (str == "6" || str == "06")
            str = "JUN";
        else if (str == "7" || str == "07")
            str = "JUL";
        else if (str == "8" || str == "08")
            str = "AUG";
        else if (str == "9" || str == "09")
            str = "SEP";
        else if (str == "10")
            str = "OCT";
        else if (str == "11")
            str = "NOV";
        else if (str == "12")
            str = "DEC";
        return str;
    }

    /// <summary>
    /// 通过三字码查询城市
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    private string strReturn(LegStop ls)
    {
        string code = "";
        try
        {
            code = ls.MiddleCityCode;
            string strim = ls.MiddleTime1;
            string etrim = ls.MiddleTime2;

            //Bd_Base_CityService bbc = new Bd_Base_CityService();
            //IList<Bd_Base_City> ibbc = bbc.GetBd_Base_City("Code='" + code + "'", 1, 2);
            BaseDataManage Manage = new BaseDataManage();
            string sqlwhere = " 1=1 and CityCodeWord='" + code + "'";
            List<Bd_Air_AirPort> objList = Manage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { sqlwhere }) as List<Bd_Air_AirPort>;




            if (objList.Count > 0)
            {
                code = objList[0].CityName;

                if (code != "")
                {
                    //code = "【经停地点】" + code + "\n【经停时间】" + strim + "--" + etrim;
                    code = code + "\r\n" + strim + "-" + etrim;
                }
            }

        }
        catch (Exception)
        {
            code = "";
        }
        return code;
    }
}
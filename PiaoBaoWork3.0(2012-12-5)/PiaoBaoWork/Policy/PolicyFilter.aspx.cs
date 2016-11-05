using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.WebCommon.Utility.Encoding;

public partial class Policy_PolicyFilter : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Init();
            BindCity();
        }
    }

    public void Init()
    {
        DateTime dt = System.DateTime.Now;
        txtStartDate.Value = dt.AddDays(-7).ToString("yyyy-MM-dd");
        txtEndDate.Value = dt.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// 绑定城市
    /// </summary>
    public void BindCity()
    {
        string sqlWhere = " IsDomestic=1 ";
        List<Bd_Air_AirPort> defaultList = this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { sqlWhere }) as List<Bd_Air_AirPort>;
        //出发城市
        ddlFromCity.DataSource = defaultList;
        ddlFromCity.DataFiledText = "CityCodeWord-CityName";
        ddlFromCity.DataFiledValue = "CityCodeWord";
        ddlFromCity.DataBind();

        //中转城市
        ddlMiddleCity.DataSource = defaultList;
        ddlMiddleCity.DataFiledText = "CityCodeWord-CityName";
        ddlMiddleCity.DataFiledValue = "CityCodeWord";
        ddlMiddleCity.DataBind();

        //到达城市
        ddlToCity.DataSource = defaultList;
        ddlToCity.DataFiledText = "CityCodeWord-CityName";
        ddlToCity.DataFiledValue = "CityCodeWord";
        ddlToCity.DataBind();
    }

}
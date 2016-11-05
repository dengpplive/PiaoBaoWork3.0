using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Logic.Pay;
using PbProject.Model;
using PbProject.Logic.Order;
using PbProject.Model.PayParam;

public partial class Default2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      
    }

    /// <summary>
    /// 测试
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button1_Click(object sender, EventArgs e)
    {
        #region 老方法

        // Tb_Ticket_OrderBLL OrderBLL = new Tb_Ticket_OrderBLL();

        // Bill data = new Bill();
        // Tb_Ticket_Order mOrder = new Tb_Ticket_Order();

        // mOrder.id = Guid.NewGuid();
        // mOrder.OrderId = OrderBLL.GetOrderId("0");//订单编号
        // mOrder.InPayNo = OrderBLL.GetIndexId();//内部流水号
        // mOrder.OrderStatusCode = 1;
        // mOrder.PayWay = 1;//支付方式（见字典表）
        // mOrder.OwnerCpyNo = "100001000008000004000003000002";//订单方
        // mOrder.CreateCpyNo = "100001000008000004000003000002";//订单方
        // mOrder.CPCpyNo = "100001000008"; //出票方
        // mOrder.PayMoney = 0;//交易金额（应收应付）

        // //mOrder.PolicyPoint = 0.13M;

        // mOrder.PolicyPoint = 0.07M;
        // mOrder.ReturnPoint = 0.07M;
        // mOrder.ReturnMoney = 20;
        // mOrder.CreateTime = DateTime.Now;

        // mOrder.DiscountDetail = "";

        //// mOrder.DiscountDetail = "100001000008000004000003^0.02^-3|100001000008000004^0.03^-5|100001000008^0.01^10";

        // List<Tb_Ticket_Passenger> mPassenger = new List<Tb_Ticket_Passenger>();

        // Tb_Ticket_Passenger tp1 = new Tb_Ticket_Passenger();

        // tp1.PassengerName = "张三";
        // tp1.id = Guid.NewGuid();
        // tp1.OrderId = mOrder.OrderId;
        // tp1.PMFee = 100; //票价
        // tp1.ABFee = 5; //基建 
        // tp1.FuelFee = 14;// 燃油

        // Tb_Ticket_Passenger tp2 = new Tb_Ticket_Passenger();
        // tp2.PassengerName = "李四";
        // tp2.id = Guid.NewGuid();
        // tp2.OrderId = mOrder.OrderId;
        // tp2.PMFee = 100;//票价
        // tp2.ABFee = 5;//基建 
        // tp2.FuelFee = 14;// 燃油

        // mPassenger.Add(tp1);
        // mPassenger.Add(tp2);
        // Data d = new Data();
        // decimal OrderPrice = d.CreateOrderPayFee(mOrder, mPassenger); //计算订单金额
        // mOrder.PayMoney = OrderPrice;
        // #region 生成订单
        // #endregion


        // data.ts(mOrder, mPassenger); //计算订单金额生成订单
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button2_Click(object sender, EventArgs e)
    {
        Tb_Ticket_OrderBLL OrderBLL = new Tb_Ticket_OrderBLL();

        Bill data = new Bill();
        Tb_Ticket_Order mOrder = new Tb_Ticket_Order();

        mOrder.OrderId = "012122411355690062";//订单编号
        mOrder.PayWay = 1;//支付方式（见字典表）
        mOrder.OwnerCpyNo = "000001000002000003000004000005";//订单方
        mOrder.CPCpyNo = "000001000002"; //出票方
        mOrder.PayMoney = 2422;//交易金额（应收应付）
        mOrder.PayMoney = 2422;//实际交易金额（实收实付）
        mOrder.InPayNo = "634919457786718750";//内部流水号

        mOrder.DiscountDetail = "000001000002000003000004^0.02^-3|000001000002000003^0.03^-5|000001000002^0.01^10";

        data.UpdateOrderAndTicketPayDetail(mOrder);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button3_Click(object sender, EventArgs e)
    {
        TenPay tenPay = new TenPay();

        string price = "10000";

        decimal total = decimal.Parse(price); //订单支付金额
        decimal supperates = 0.001M; //支付费率
        decimal paySXF = FourToFiveNum(total * supperates, 2); //支付手续费
        decimal actPrice = total - paySXF; //收款账号金额

        string orderId = DateTime.Now.Ticks.ToString();

        TenPayParam tenPayParam = new TenPayParam();

        tenPayParam.Bus_Args = "261065527^" +1 + "^1|465853660^" + 1 + "^4|" + "money@mypb.cn^" +2 + "^4";

        tenPayParam.Bus_Desc = "12345^深圳-上海^1^fady^庄^13800138000";///业务描述，特定格式的字符串，格式为：PNR^航程^机票张数^机票销售商在机票平台的id^联系人姓名^联系电话
        //tenPayParam.Bus_Desc = "测试";
        tenPayParam.Desc = "在线充值";
        tenPayParam.Orderid = orderId;
        tenPayParam.Total_Tee = total.ToString();
        tenPayParam.UserHostAddress = Page.Request.UserHostAddress;
        tenPayParam.Total_Tee = "4";
        string strFromValue = new PbProject.Logic.Pay.TenPay().SplitPayRequest(tenPayParam);

        Response.Write(strFromValue);

    }

    public decimal FourToFiveNum(decimal v, int x)
    {
        //decimal _del = Math.Round(del, 2); //四舍五入
        //return _del;

        decimal _del = Math.Round(v + 0.0000001M, x);// //四舍五入
        return _del;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button4_Click(object sender, EventArgs e)
    {

        //（只入不舍）
        decimal del = 49.009999M;
        //保留两位小数（只入不舍）
        decimal _del = Math.Round(del + 0.005M - 0.0001M, 2);
        decimal del1 = 49.000099M;
        //保留两位小数（只入不舍）
        decimal _del1 = Math.Round(del1 + 0.005M, 2);

        decimal del2 = 49.000000M;
        //保留两位小数（只入不舍）
        decimal _del2 = Math.Round(del2 + 0.005M - 0.0001M, 2);

        //（只舍不入）
        decimal del3 = 49.00099M;
        decimal result = Math.Round(del3 - 0.005M + 0.0001M, 2);

    }

    /// <summary>
    /// 登录密码加密
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button5_Click(object sender, EventArgs e)
    {
        TextBox1.Text = PbProject.WebCommon.Web.Cookie.SiteCookie.GetMD5(TextBox1.Text.Trim());
    }


    protected void Button7_Click(object sender, EventArgs e)
    {
        string url = "~/Buy/PayMent.aspx?id=" + TextBox2.Text;
        //~/Buy/PayMent.aspx

        Response.Redirect(url);
    }
    protected void Button8_Click(object sender, EventArgs e)
    {
        int payWay = int.Parse(TextBox4.Text);

        int tempNo = payWay % 4 - 1;
        if (tempNo < 0)
            tempNo += 4;

        TextBox4.Text = tempNo.ToString();
    }

    /// <summary>
    /// 通过订单号 计算订单金额
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button9_Click(object sender, EventArgs e)
    {
        string orderid = TextBox5.Text.Trim();

        string sqlWhere = " OrderId='" + orderid + "' ";

        PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
        List<Tb_Ticket_Order> mOrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
        List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Passenger>;

        //Data data = new Data();
        //TextBox5.Text = data.CreateOrderPayMoney(mOrderList[0], PassengerList).ToString();  //计算时 已经减去手续费了

        PbProject.Logic.Pay.Bill bill = new Bill();

        bill.CreateOrderAndTicketPayDetailNew(mOrderList[0], PassengerList);

        TextBox5.Text = mOrderList[0].OrderMoney.ToString();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button6_Click(object sender, EventArgs e)
    {
        try
        {

            bool result = System.Text.RegularExpressions.Regex.IsMatch(TextBox6.Text.Trim(),
@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);


            TextBox6.Text = result ? "true" : "false";

        }
        catch (Exception)
        {
            TextBox6.Text =  "err";
        }
    }
}
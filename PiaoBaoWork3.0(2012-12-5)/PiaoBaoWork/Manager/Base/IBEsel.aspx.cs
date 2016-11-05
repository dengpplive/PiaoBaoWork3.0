using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Services.Description;
using System.CodeDom;
using System.Data;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

/// <summary>
/// IBE查询
/// </summary>
public partial class Manager_Base_IBEsel : BasePage
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region 自带方法

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toCity"></param>
    /// <param name="fromCity"></param>
    /// <param name="time"></param>
    /// <param name="airCode"></param>
    /// <returns></returns>
    public string CallWebServiceMethod(string toCity, string fromCity, string time, string airCode)
    {
        string[] strings = { toCity, airCode, toCity, fromCity, time, "0000" };
        WebClient wc = new WebClient();
        string url = "http://182.151.203.243:4031/WebService1.asmx";
        Stream stream = wc.OpenRead(url + "?WSDL");
        ServiceDescription sd = ServiceDescription.Read(stream);
        ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
        sdi.AddServiceDescription(sd, "", "");
        CodeNamespace cn = new CodeNamespace();

        //生成客户端代理类代码
        CodeCompileUnit ccu = new CodeCompileUnit();
        ccu.Namespaces.Add(cn);
        sdi.Import(cn, ccu);
        CSharpCodeProvider csc = new CSharpCodeProvider();
        ICodeCompiler icc = csc.CreateCompiler();

        //设定编译参数
        CompilerParameters cplist = new CompilerParameters();
        cplist.GenerateExecutable = false;
        cplist.GenerateInMemory = true;
        cplist.ReferencedAssemblies.Add("System.dll");
        cplist.ReferencedAssemblies.Add("System.XML.dll");
        cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
        cplist.ReferencedAssemblies.Add("System.Data.dll");

        //编译代理类
        CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
        if (true == cr.Errors.HasErrors)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
            {
                sb.Append(ce.ToString());
                sb.Append(System.Environment.NewLine);
            }
            throw new Exception(sb.ToString());
        }

        //生成代理实例，并调用方法
        System.Reflection.Assembly assembly = cr.CompiledAssembly;
        Type t = assembly.GetType("WebService1");
        object obj = Activator.CreateInstance(t);
        System.Reflection.MethodInfo mi = t.GetMethod("getBasicData");
        //string Returnvalue = (string)mi.Invoke(obj, new Object[] { "test", "123", "3U8881" });
        //string Returnvalue = (string)mi.Invoke(obj, strings);
        //string Returnvalue = (string)mi.Invoke(obj,null);
        string s1 = DateTime.Now.ToString() + DateTime.Now.Millisecond;
        DataSet ds = (DataSet)mi.Invoke(obj, strings);
        string s2 = DateTime.Now.ToString() + DateTime.Now.Millisecond;
        string Returnvalue = "";
        Returnvalue = reorganizedData(ds);

        bandValue(ds, "", true);

        return Returnvalue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ds"></param>
    /// <returns></returns>
    private string reorganizedData(DataSet ds)
    {
        StringBuilder sb = new StringBuilder("");
        DataSet dsTemp = CreateTableStructure();
        try
        {
            if (!ds.Tables.Contains("errorTable"))//没有返回错误数据
            {
                int FltShopAVJourneyCount = ds.Tables["FltShopAVJourney"].Rows.Count;
                int FltShopAVOptCount = ds.Tables["FltShopAVOpt"].Rows.Count;
                int CabinAllCount = ds.Tables["CabinAll"].Rows.Count;
                int TaxAllCount = ds.Tables["TaxAll"].Rows.Count;
                int FltShopAVFltCount = ds.Tables["FltShopAVFlt"].Rows.Count;

                int FltShopPSCount = ds.Tables["FltShopPS"].Rows.Count;
                int RouteAllCount = ds.Tables["RouteAll"].Rows.Count;
                int FltShopNFareCount = ds.Tables["FltShopNFare"].Rows.Count;
                int FltShopPFareCount = ds.Tables["FltShopPFare"].Rows.Count;
                int FltShopRuleCount = ds.Tables["FltShopRule"].Rows.Count;



                for (int i = 0; i < FltShopAVJourneyCount; i++)
                {
                    //DataRow drBd_Air_CabinDiscount = dsNew.Tables["Bd_Air_CabinDiscount"].NewRow();
                    //drBd_Air_CabinDiscount["A0"] = ds.Tables["FltShopAVJourney"].Rows[i]["A0"];
                    //drBd_Air_CabinDiscount["A1"] = "";
                    //drBd_Air_CabinDiscount["A2"] = "";
                    //drBd_Air_CabinDiscount["A3"] = "";
                    //drBd_Air_CabinDiscount["A4"] = "";
                    //drBd_Air_CabinDiscount["A5"] = "";
                    //dsNew.Tables["Bd_Air_CabinDiscount"].Rows.Add(drBd_Air_CabinDiscount);
                    for (int j = 0; j < FltShopAVOptCount; j++)
                    {
                        for (int k = 0; k < FltShopAVFltCount; k++)
                        {
                            if (!ds.Tables["FltShopAVFlt"].Rows[k]["A0"].ToString().Contains(ds.Tables["FltShopAVOpt"].Rows[j]["A0"].ToString()))
                            {
                                continue;
                            }
                            if (k != 0)
                            {
                                sb.Append("^");
                            }
                            sb.Append(changeDate(ds.Tables["FltShopAVFlt"].Rows[k]["A4"].ToString()));//0.出发日期
                            sb.Append(",");
                            sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A7"].ToString().Substring(0, 2) + ":" + ds.Tables["FltShopAVFlt"].Rows[k]["A7"].ToString().Substring(2, 2));//1.起飞时间
                            sb.Append(",");
                            sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A9"].ToString().Substring(0, 2) + ":" + ds.Tables["FltShopAVFlt"].Rows[k]["A9"].ToString().Substring(2, 2));//2.到达时间
                            sb.Append(",");
                            sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A1"].ToString());//3.航空公司
                            sb.Append(",");
                            sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A2"].ToString());//4.航班号
                            sb.Append(",");
                            string getCabinNameAndAvAll = "";
                            string getChildCabinNameAndAvAll = "";
                            for (int m = 0; m < CabinAllCount; m++)
                            {
                                if (ds.Tables["CabinAll"].Rows[m]["A0"].ToString() == ds.Tables["FltShopAVFlt"].Rows[k]["A0"].ToString())
                                {
                                    if (ds.Tables["CabinAll"].Rows[m]["A1"].ToString().Length == 1)
                                    {
                                        getCabinNameAndAvAll += ds.Tables["CabinAll"].Rows[m]["A1"].ToString() + ds.Tables["CabinAll"].Rows[m]["A2"].ToString();
                                    }
                                    if (ds.Tables["CabinAll"].Rows[m]["A1"].ToString().Length > 1)
                                    {
                                        if (getChildCabinNameAndAvAll != "")
                                        {
                                            getChildCabinNameAndAvAll += " ";
                                        }
                                        getChildCabinNameAndAvAll += ds.Tables["CabinAll"].Rows[m]["A1"].ToString() + ds.Tables["CabinAll"].Rows[m]["A2"].ToString();
                                    }
                                }
                            }
                            sb.Append(getCabinNameAndAvAll + "*9");//5.舱位信息
                            sb.Append(",");
                            sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A5"].ToString());//6.起始城市
                            sb.Append(",");
                            sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A6"].ToString());//7.目的城市
                            sb.Append(",");
                            sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A11"].ToString());//8.机型
                            sb.Append(",");
                            sb.Append(ds.Tables["FltShopAVFlt"].Rows[k]["A19"].ToString());//9.经停
                            sb.Append(",");
                            string isA13 = "0";
                            if (ds.Tables["FltShopAVFlt"].Rows[k]["A13"].ToString() != "")
                            {
                                isA13 = "1";
                            }
                            sb.Append(isA13);//10.餐食
                            sb.Append(",");
                            string isA16 = "1";//基本都是电子客票,默认为1
                            if (ds.Tables["FltShopAVFlt"].Rows[k]["A16"].ToString() != "E")
                            {
                                isA16 = "0";
                            }
                            sb.Append(isA16);//11.电子客票    
                            sb.Append(",");
                            string isA22 = "False";
                            if (ds.Tables["FltShopAVFlt"].Rows[k]["A22"].ToString() != "")
                            {
                                isA22 = "True";
                            }
                            sb.Append(isA22);//12.是否共享
                            sb.Append(",");
                            sb.Append(getChildCabinNameAndAvAll);//13.子舱位
                            sb.Append(",");
                            string FltShopAVFltA20 = ds.Tables["FltShopAVFlt"].Rows[k]["A20"].ToString();
                            string FltShopAVFltA21 = ds.Tables["FltShopAVFlt"].Rows[k]["A21"].ToString();
                            if (FltShopAVFltA20 == "")
                            {
                                FltShopAVFltA20 = "--";
                            }
                            if (FltShopAVFltA21 == "")
                            {
                                FltShopAVFltA21 = "--";
                            }
                            sb.Append(FltShopAVFltA20 + FltShopAVFltA21);//14.出发城市航站楼+到达城市航站楼
                            //2012-11-20,08:00,10:30,ZH,4113,F9Y9B9MSHSKSLSQSGSVSESTSS9*9,CTU,PEK,321,0,0,1,True,M1S,T2T3^

                            //2012-11-20,08:00,10:30,ZH,4113,FAYABASA*9,CTU,PEK,321,0,,E,CA4113,,T2T3^2012-11-20,09:00,11:35,ZH,4101,FAYABASA*9,CTU,PEK,321,0,,E,CA4101,,T2T3


                        }
                    }
                }
                //获取所有承运人  
                StringBuilder chengyunren = new StringBuilder("|");
                for (int i = 0; i < FltShopPFareCount; i++)
                {
                    //参考格式如下,后期代码如有变动,请参照实际值
                    //|CA|MU|
                    string tempC = ds.Tables["FltShopPFare"].Rows[i]["A1"].ToString();
                    //已经添加了承运人不再添加
                    if (!chengyunren.ToString().Contains("|" + tempC + "|"))
                    {
                        chengyunren.Append(tempC);
                        chengyunren.Append("|");
                    }

                }

                //获取 Y舱位(全价舱),承运人,价格
                StringBuilder YPirce = new StringBuilder("|");
                for (int i = 0; i < FltShopPFareCount; i++)
                {
                    if (ds.Tables["FltShopPFare"].Rows[i]["A2"].ToString() == "Y")
                    {
                        //参考格式如下,后期代码如有变动,请参照实际值
                        //|Y^CA^1000|Y^MU^1100|  
                        YPirce.Append("Y");
                        YPirce.Append("^");
                        YPirce.Append(ds.Tables["FltShopPFare"].Rows[i]["A1"].ToString());
                        YPirce.Append("^");
                        YPirce.Append(ds.Tables["FltShopPFare"].Rows[i]["A3"].ToString());
                        YPirce.Append("|");
                        //Y舱的价格写入  全价和里程
                        DataRow tempRow = dsTemp.Tables[1].NewRow();
                        object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        ds.Tables["FltShopPFare"].Rows[i]["A3"].ToString(),
                                        ds.Tables["FltShopPFare"].Rows[i]["A3"].ToString(),
                                        ds.Tables["FltShopPFare"].Rows[i]["A1"].ToString(),
                                        ds.Tables["FltShopPFare"].Rows[i]["A5"].ToString(),
                                        ds.Tables["FltShopPFare"].Rows[i]["A6"].ToString()
                                       };
                        tempRow.ItemArray = objs;
                        //写入舱位折扣表
                        dsTemp.Tables[1].Rows.Add(tempRow);
                    }
                }
                //判断是否所有承运人的Y舱价格都已经取到,如果没有取到,则由缓存读取
                string[] chengyunrenS = chengyunren.ToString().Split('|');
                for (int i = 0; i < chengyunrenS.Length; i++)
                {
                    if (chengyunrenS[i] == "")
                    {
                        continue;
                    }
                    //如果没有,则读取缓存
                    if (!YPirce.ToString().Contains("^" + chengyunrenS[i] + "^"))
                    {
                        YPirce.Append("Y");
                        YPirce.Append("^");
                        YPirce.Append(chengyunrenS[i]);//承运人
                        YPirce.Append("^");
                        YPirce.Append("1000");//价格读取缓存中的价格
                        YPirce.Append("|");

                        //没有Y舱的情况,   全价和里程表的数据补录

                        DataRow tempRow = dsTemp.Tables[1].NewRow();
                        object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        "1440",
                                        "1440",
                                        chengyunrenS[i],//承运人
                                        "YYY",//"出发城市",
                                        "YYY"//"到达城市"
                                       };
                        tempRow.ItemArray = objs;
                        //写入舱位折扣表
                        dsTemp.Tables[1].Rows.Add(tempRow);
                    }

                }

                for (int i = 0; i < FltShopPFareCount; i++)
                {
                    DataRow tempRow = dsTemp.Tables[0].NewRow();
                    string jiage = ds.Tables["FltShopPFare"].Rows[i]["A3"].ToString();//舱位价
                    if (jiage == "")
                    {
                        jiage = "0";
                    }
                    //当前舱位价格
                    decimal jiageD = decimal.Parse(jiage);
                    //获取当前承运人
                    string dangqianChengyunren = ds.Tables["FltShopPFare"].Rows[i]["A1"].ToString();
                    //折扣等于舱位价除以Y舱价格
                    string zhekou = getDangQianChengYunZheKou(YPirce, dangqianChengyunren, jiageD);
                    object[] objs = { i+1,
                                        ds.Tables["FltShopPFare"].Rows[i]["A2"],
                                        ds.Tables["FltShopPFare"].Rows[i]["A1"],
                                        ds.Tables["FltShopPFare"].Rows[i]["A5"],
                                        ds.Tables["FltShopPFare"].Rows[i]["A6"],
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        zhekou
                                       };
                    tempRow.ItemArray = objs;
                    //写入舱位折扣表
                    dsTemp.Tables[0].Rows.Add(tempRow);
                }
                //获取燃油,取一条信息即可
                for (int i = 0; i < TaxAllCount; i++)
                {
                    if (ds.Tables["TaxAll"].Rows[i]["A1"].ToString() == "YQ")
                    {
                        decimal ranyou = decimal.Parse(ds.Tables["TaxAll"].Rows[i]["A2"].ToString() == "" ? "0" : ds.Tables["TaxAll"].Rows[i]["A2"].ToString());
                        DataRow tempRow = dsTemp.Tables[3].NewRow();
                        object[] objs = {
                                        "2000-08-09 01:01:01",
                                        "2100-08-09 01:01:01",
                                        ranyou/2,
                                        ranyou
                                       };
                        tempRow.ItemArray = objs;
                        //写入燃油表
                        dsTemp.Tables[3].Rows.Add(tempRow);
                        break;
                    }
                }
            }
            else
            {
                //OnError(ds.Tables["errorTable"].Rows[0][0].ToString(), "reorganizedData IBE接口错误信息");
                sb.Clear();
                sb.Append(ds.Tables["errorTable"].Rows[0][0].ToString());
            }

        }
        catch (Exception ex)
        {
            //OnError(ex.Message, "reorganizedData 通过IBE接口查询航班");
            sb.Clear();
            sb.Append(ex.Message);
        }
        return sb.ToString();

    }

    /// <summary>
    /// 转换时间
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    private string changeDate(string code)
    {
        //20dec12  2012-11-20
        string month = "";
        string[] str = new string[] { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] == code.Substring(2, 3))
            {
                month = (i + 1).ToString();
                break;
            }
        }
        return "20" + code.Substring(5, 2) + "-" + month + "-" + code.Substring(0, 2);
    }

    /// <summary>
    /// 创建基础数据需要的表结构
    /// </summary>
    /// <returns></returns>
    private DataSet CreateTableStructure()
    {
        DataSet dsNew = new DataSet();
        //舱位折扣
        DataTable dtNew1 = new DataTable();
        DataColumn id = new DataColumn("id");//1
        DataColumn Cabin = new DataColumn("Cabin");//2
        DataColumn AirPortCode = new DataColumn("AirPortCode");//3
        DataColumn FromCityCode = new DataColumn("FromCityCode");//4
        DataColumn ToCityCode = new DataColumn("ToCityCode");//5
        DataColumn BeginTime = new DataColumn("BeginTime");//6
        DataColumn EndTime = new DataColumn("EndTime");//7
        DataColumn DiscountRate = new DataColumn("DiscountRate");//8
        dtNew1.Columns.Add(id);
        dtNew1.Columns.Add(Cabin);
        dtNew1.Columns.Add(AirPortCode);
        dtNew1.Columns.Add(FromCityCode);
        dtNew1.Columns.Add(ToCityCode);
        dtNew1.Columns.Add(BeginTime);
        dtNew1.Columns.Add(EndTime);
        dtNew1.Columns.Add(DiscountRate);
        //全价和里程
        DataTable dtNew2 = new DataTable();
        DataColumn EffectTime = new DataColumn("EffectTime");//1
        DataColumn InvalidTime = new DataColumn("InvalidTime");//2
        DataColumn FareFee = new DataColumn("FareFee");//3
        DataColumn Mileage = new DataColumn("Mileage");//4
        DataColumn CarryCode = new DataColumn("CarryCode");//5
        DataColumn FromCityCode2 = new DataColumn("FromCityCode");//6
        DataColumn ToCityCode2 = new DataColumn("ToCityCode");//7
        dtNew2.Columns.Add(EffectTime);
        dtNew2.Columns.Add(InvalidTime);
        dtNew2.Columns.Add(FareFee);
        dtNew2.Columns.Add(Mileage);
        dtNew2.Columns.Add(CarryCode);
        dtNew2.Columns.Add(FromCityCode2);
        dtNew2.Columns.Add(ToCityCode2);
        //机建价格    
        DataTable dtNew3 = new DataTable();
        DataColumn ABFeeN = new DataColumn("ABFeeN");//1
        DataColumn Aircraft = new DataColumn("Aircraft");//2
        dtNew3.Columns.Add(ABFeeN);
        dtNew3.Columns.Add(Aircraft);
        //燃油价格
        DataTable dtNew4 = new DataTable();
        DataColumn StartTime = new DataColumn("StartTime");//1
        DataColumn EndTime4 = new DataColumn("EndTime");//2
        DataColumn LowAdultFee = new DataColumn("LowAdultFee");//3
        DataColumn ExceedAdultFee = new DataColumn("ExceedAdultFee");//4
        DataColumn LowChildFee = new DataColumn("LowChildFee");//5
        DataColumn ExceedChildFee = new DataColumn("ExceedChildFee");//6
        dtNew4.Columns.Add(StartTime);
        dtNew4.Columns.Add(EndTime4);
        dtNew4.Columns.Add(LowAdultFee);
        dtNew4.Columns.Add(ExceedAdultFee);
        dtNew4.Columns.Add(LowChildFee);
        dtNew4.Columns.Add(ExceedChildFee);

        //承运人
        DataTable dtNew5 = new DataTable();
        DataColumn Code = new DataColumn("Code");//1
        DataColumn ShortName = new DataColumn("ShortName");//2
        DataColumn A1 = new DataColumn("A1");//3
        DataColumn Type = new DataColumn("Type");//4

        dtNew5.Columns.Add(Code);
        dtNew5.Columns.Add(ShortName);
        dtNew5.Columns.Add(A1);
        dtNew5.Columns.Add(Type);

        //城市
        DataTable dtNew6 = new DataTable();
        DataColumn City = new DataColumn("City");//1
        DataColumn spell = new DataColumn("spell");//2
        DataColumn Code6 = new DataColumn("Code");//3
        DataColumn Countries = new DataColumn("Countries");//4
        DataColumn type = new DataColumn("type");//5

        dtNew6.Columns.Add(City);
        dtNew6.Columns.Add(spell);
        dtNew6.Columns.Add(Code6);
        dtNew6.Columns.Add(Countries);
        dtNew6.Columns.Add(type);

        //特价舱位信息
        DataTable dtNew7 = new DataTable();
        DataColumn CarryCode7 = new DataColumn("CarryCode");//1
        DataColumn Spaces = new DataColumn("Spaces");//2
        DataColumn LogChangePrescript = new DataColumn("LogChangePrescript");//3
        DataColumn DishonoredBillPrescript = new DataColumn("DishonoredBillPrescript");//4
        DataColumn UpCabinPrescript = new DataColumn("UpCabinPrescript");//5

        dtNew7.Columns.Add(CarryCode7);
        dtNew7.Columns.Add(Spaces);
        dtNew7.Columns.Add(LogChangePrescript);
        dtNew7.Columns.Add(DishonoredBillPrescript);
        dtNew7.Columns.Add(UpCabinPrescript);


        dsNew.Tables.Add(dtNew1);
        dsNew.Tables.Add(dtNew2);
        dsNew.Tables.Add(dtNew3);
        dsNew.Tables.Add(dtNew4);
        dsNew.Tables.Add(dtNew5);
        dsNew.Tables.Add(dtNew6);
        dsNew.Tables.Add(dtNew7);
        return dsNew;
    }

    /// <summary>
    /// 当前承运人的舱位的折扣
    /// </summary>
    /// <param name="YPirce">关联Y舱,承运人,Y舱价格的字符串</param>
    /// <param name="dangqianChengyunren">当前承运人</param>
    /// <param name="jiageD">当前舱位价格</param>
    /// <returns>折扣</returns>
    private string getDangQianChengYunZheKou(StringBuilder YPirce, string dangqianChengyunren, decimal jiageD)
    {
        string[] AllY = YPirce.ToString().Split('|');
        string dangqianY = "";
        for (int j = 0; j < AllY.Length; j++)
        {
            if (AllY[j].Contains("^" + dangqianChengyunren + "^"))
            {
                dangqianY = AllY[j].Split('^')[2];
                break;
            }
        }
        decimal dangqianYD;
        try
        {
            dangqianYD = decimal.Parse(dangqianY);
        }
        catch (Exception)
        {
            dangqianYD = jiageD;//转换有问题则默认为Y舱价格就是本身价格
        }
        return (FourToFiveNum((jiageD / dangqianYD), 2) * 100).ToString();

    }

    /// <summary>  
    /// 实现数据的四舍五入法, 保留小数
    /// </summary>  
    /// <param name="v">要进行处理的数据</param>  
    /// <param name="x">保留的小数位数</param>   
    /// <returns>四舍五入后的结果</returns>   
    public decimal FourToFiveNum(decimal v, int x)
    {
        //decimal _del = Math.Round(del, 2); //四舍五入
        //return _del;

        decimal _del = Math.Round(v + 0.0000001M, x);// //四舍五入
        return _del;

        //bool isNegative = false;
        ////如果是负数      
        //if (v < 0)
        //{
        //    isNegative = true;
        //    v = -v;
        //}
        //int IValue = 1;
        //for (int i = 1; i <= x; i++)
        //{
        //    IValue = IValue * 10;
        //}
        //decimal Int = Math.Round(v * IValue + 0.5M, 0);
        //v = Int / IValue;
        //if (isNegative)
        //{
        //    v = -v;
        //}
        //return v;
    }

    /// <summary>
    /// againWrite
    /// </summary>
    /// <param name="sb"></param>
    public void againWrite(StringBuilder sb)
    {
        StreamWriter fs = null;
        try
        {
            string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log\\" + Page + "\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + "again" + ".txt", true, System.Text.Encoding.Default);
            fs.WriteLine(sb.ToString());
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
        }
    }

    #endregion

    #region 页面方法

    /// <summary>
    /// btnQuery_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        txtReturnValue.Text = "";
        string val = CallWebServiceMethod(txtToCity.Text.Trim().ToUpper(), txtFromCity.Text.Trim().ToUpper(), txtTime.Text.Trim().ToUpper(), txtAirCode.Text.Trim().ToUpper());

        val = val.Replace("^", "^\r\n");

        int rows = 10;
        int valRows = val.Split('^').Length;

        if (valRows < rows)
        {
            rows = rows - valRows > 3 ? rows : rows + 5;
        }
        else if (valRows >= rows)
        {
            rows = valRows + 5;
        }

        txtReturnValue.Rows = rows;
        txtReturnValue.Text = val;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ds"></param>
    /// <param name="name"></param>
    /// <param name="rs"></param>
    private void bandValue(DataSet ds, string name, bool rs)
    {
        try
        {
            ViewState["vsds"] = ds;

            if (ds != null && ds.Tables.Count > 0)
            {
                if (rs)
                {
                    int count = ds.Tables.Count;
                    ddlNum.Items.Clear();

                    string strVal = "";

                    for (int i = 0; i < count; i++)
                    {
                        strVal = ds.Tables[i].TableName;

                        if (strVal == "FltShopAVJourney")
                        { strVal += " 查询信息"; }
                        else if (strVal == "FltShopAVOpt")
                        { strVal += " 编号信息"; }
                        else if (strVal == "CabinAll")
                        { strVal += " 座位信息"; }
                        else if (strVal == "TaxAll")
                        { strVal += " 基建燃油"; }
                        else if (strVal == "FltShopAVFlt")
                        { strVal += " 航班信息"; }
                        else if (strVal == "FltShopPS")
                        { strVal += " "; }
                        else if (strVal == "RouteAll")
                        { strVal += " 舱位信息"; }
                        else if (strVal == "FltShopNFare")
                        { strVal += " 特价信息"; }
                        else if (strVal == "FltShopPFare")
                        { strVal += " 票价信息"; }
                        else if (strVal == "FltShopRule")
                        { strVal += " 客规信息"; }


                        ddlNum.Items.Add(new ListItem(strVal, ds.Tables[i].TableName));
                    }
                }

                if (name == "")
                {
                    name = ddlNum.SelectedValue;
                }

                gvValue.DataSource = ds.Tables[name];
                gvValue.DataBind();
            }
        }
        catch (Exception ex)
        {
            gvValue.EmptyDataText = ex.ToString();
            gvValue.DataSource = null;
            gvValue.DataBind();
        }
    }

    /// <summary>
    /// 清空
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        txtReturnValue.Text = "";
        ddlNum.Items.Clear();
        gvValue.DataSource = null;
        gvValue.DataBind();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSelTwo_Click(object sender, EventArgs e)
    {
        if (ViewState["vsds"] != null)
        {
            DataSet ds = ViewState["vsds"] as DataSet;
            bandValue(ds, hid_selValue.Value, false);
        }
    }

    #endregion
}
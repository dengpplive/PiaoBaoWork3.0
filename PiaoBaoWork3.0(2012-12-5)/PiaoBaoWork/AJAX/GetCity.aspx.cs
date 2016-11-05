using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using PbProject.Logic.ControlBase;
using PbProject.Model;
using IRemoteMethodSpace;
public partial class js_CitySelect_GetCity : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string result = GetCityData();
        OutPut(result);
    }
    /// <summary>
    /// 响应客户端结果数据
    /// </summary>
    /// <param name="result"></param>
    public void OutPut(string result)
    {
        try
        {
            Response.ContentType = "text/plain";
            Response.ContentEncoding = Encoding.Default;
            Response.Clear();
            Response.Write(result);
            Response.Flush();
            Response.End();
        }
        catch (Exception)
        {
        }
    }
    /// <summary>
    /// 获取城市数据
    /// </summary>
    /// <returns></returns>
    public string GetCityData()
    {
        StringBuilder sbScript = new StringBuilder();
        try
        {
            BaseDataManage Manage = new BaseDataManage();
            List<Bd_Air_AirPort> CityList = null;
            string cacheUrl = System.Configuration.ConfigurationManager.AppSettings["CacheUrl"];
            //是否为国际城市 1是 0否
            string IsGJ = Request["IsGJ"] != null ? Request["IsGJ"].ToString() : "0";
            if (!string.IsNullOrEmpty(cacheUrl))
            {
                try
                {
                    //去缓存数据
                    IRemoteMethod remoteobj = (IRemoteMethod)Activator.GetObject(typeof(IRemoteMethod), cacheUrl);
                    DataSet ds = remoteobj.GetBd_Air_Airport("");
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataView dv = ds.Tables[0].DefaultView;
                        dv.Sort = " CityQuanPin ";//拼音全称排序
                        DataTable Bd_Air_AirPort = dv.ToTable();
                        CityList = PbProject.Dal.Mapping.MappingHelper<Bd_Air_AirPort>.FillModelList(Bd_Air_AirPort);
                    }
                }
                catch (Exception)
                {
                    //取数据库数据
                    CityList = Manage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { " 1=1 order by CityQuanPin " }) as List<Bd_Air_AirPort>;
                }
            }
            else
            {
                //取数据库数据
                CityList = Manage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { " 1=1 order by CityQuanPin " }) as List<Bd_Air_AirPort>;
            }
            List<string> AllData = new List<string>();
            if (CityList != null && CityList.Count > 0)
            {
                List<string> A_C_List = new List<string>();
                List<string> D_G_List = new List<string>();
                List<string> H_I_List = new List<string>();
                List<string> J_K_List = new List<string>();
                List<string> L_M_List = new List<string>();
                List<string> N_R_List = new List<string>();
                List<string> S_T_List = new List<string>();
                List<string> U_X_List = new List<string>();
                List<string> Y_Z_List = new List<string>();

                List<string> A_List = new List<string>();
                List<string> B_List = new List<string>();
                List<string> C_List = new List<string>();
                List<string> D_List = new List<string>();
                List<string> E_List = new List<string>();
                List<string> F_List = new List<string>();
                List<string> G_List = new List<string>();
                List<string> H_List = new List<string>();
                List<string> I_List = new List<string>();
                List<string> J_List = new List<string>();
                List<string> K_List = new List<string>();
                List<string> L_List = new List<string>();
                List<string> M_List = new List<string>();
                List<string> N_List = new List<string>();
                List<string> O_List = new List<string>();
                List<string> P_List = new List<string>();
                List<string> Q_List = new List<string>();
                List<string> R_List = new List<string>();
                List<string> S_List = new List<string>();
                List<string> T_List = new List<string>();
                List<string> U_List = new List<string>();
                List<string> V_List = new List<string>();
                List<string> W_List = new List<string>();
                //List<string> X_List = new List<string>();
                List<string> Y_List = new List<string>();
                List<string> Z_List = new List<string>();

                string FirstChar = "";
                foreach (var item in CityList)
                {
                    if (item.CityQuanPin.Length == 0 || item.CityCodeWord.Length == 0) continue;
                    AllData.Add(item.CityQuanPin + "|" + item.CityName + "|" + item.CityCodeWord + "|"+item.CityJianPin + "|");
                    FirstChar = item.CityQuanPin[0].ToString().ToUpper();
                    if (IsGJ == "0" && item.IsDomestic == 1)//国内
                    {
                        if ("ABC".Contains(FirstChar))
                        {
                            A_C_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("DEFG".Contains(FirstChar))
                        {
                            D_G_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("HI".Contains(FirstChar))
                        {
                            H_I_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("JK".Contains(FirstChar))
                        {
                            J_K_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("LM".Contains(FirstChar))
                        {
                            L_M_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("NOPQR".Contains(FirstChar))
                        {
                            N_R_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("ST".Contains(FirstChar))
                        {
                            S_T_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("UVWX".Contains(FirstChar))
                        {
                            U_X_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("YZ".Contains(FirstChar))
                        {
                            Y_Z_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                    }
                    else if (IsGJ == "1" && item.IsDomestic == 2)//国际
                    {
                        if ("A".Contains(FirstChar))
                        {
                            A_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("B".Contains(FirstChar))
                        {
                            B_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("C".Contains(FirstChar))
                        {
                            C_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("D".Contains(FirstChar))
                        {
                            D_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("E".Contains(FirstChar))
                        {
                            E_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("F".Contains(FirstChar))
                        {
                            F_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("G".Contains(FirstChar))
                        {
                            G_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("H".Contains(FirstChar))
                        {
                            H_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("I".Contains(FirstChar))
                        {
                            I_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("J".Contains(FirstChar))
                        {
                            J_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("K".Contains(FirstChar))
                        {
                            K_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("L".Contains(FirstChar))
                        {
                            L_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("M".Contains(FirstChar))
                        {
                            M_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("N".Contains(FirstChar))
                        {
                            N_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("O".Contains(FirstChar))
                        {
                            O_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("P".Contains(FirstChar))
                        {
                            P_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("Q".Contains(FirstChar))
                        {
                            Q_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("R".Contains(FirstChar))
                        {
                            R_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("S".Contains(FirstChar))
                        {
                            S_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("T".Contains(FirstChar))
                        {
                            T_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("U".Contains(FirstChar))
                        {
                            U_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("V".Contains(FirstChar))
                        {
                            V_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("W".Contains(FirstChar))
                        {
                            W_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        //else if ("X".Contains(FirstChar))
                        //{
                        //    X_List.Add(item.CityCodeWord + "|" + item.CityName);
                        //}
                        else if ("Y".Contains(FirstChar))
                        {
                            Y_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                        else if ("Z".Contains(FirstChar))
                        {
                            Z_List.Add(item.CityCodeWord + "|" + item.CityName);
                        }
                    }
                }
                //所有数据
                sbScript.AppendFormat("$$.module.address.source.fltdomestic =\"@{0}@\";\r\n", string.Join("@", AllData.ToArray()));
                sbScript.Append(" $$.module.address.source.fltdomestic_hotData = {\r\n");
                if (IsGJ == "0")
                {
                    //国内热门
                    sbScript.Append("\"热门\": \"@PEK|北京(首都)@NAY|北京(南苑)@SHA|上海(虹桥)@PVG|上海(浦东)@CAN|广州@SZX|深圳@CTU|成都@HGH|杭州@WUH|武汉@XIY|西安@CKG|重庆@TAO|青岛@CSX|长沙@NKG|南京@XMN|厦门@KMG|昆明@DLC|大连@TSN|天津@CGO|郑州@SYX|三亚@TNA|济南@FOC|福州\",\r\n");
                    sbScript.AppendFormat(" 'ABC': \"@{0}\",\r\n", string.Join("@", A_C_List.ToArray()));
                    sbScript.AppendFormat(" 'DEFG': \"@{0}\",\r\n", string.Join("@", D_G_List.ToArray()));
                    sbScript.AppendFormat(" 'HI': \"@{0}\",\r\n", string.Join("@", H_I_List.ToArray()));
                    sbScript.AppendFormat(" 'JK': \"@{0}\",\r\n", string.Join("@", J_K_List.ToArray()));
                    sbScript.AppendFormat(" 'LM': \"@{0}\",\r\n", string.Join("@", L_M_List.ToArray()));
                    sbScript.AppendFormat(" 'NOPQR': \"@{0}\",\r\n", string.Join("@", N_R_List.ToArray()));
                    sbScript.AppendFormat(" 'ST': \"@{0}\",\r\n", string.Join("@", S_T_List.ToArray()));
                    sbScript.AppendFormat(" 'UVWX': \"@{0}\",\r\n", string.Join("@", U_X_List.ToArray()));
                    sbScript.AppendFormat(" 'YZ': \"@{0}\"\r\n", string.Join("@", Y_Z_List.ToArray()));
                }
                else//国际热门
                {
                    sbScript.Append("\"热门\": \"@HKG|香港@TPE|台北@MFM|澳门@KHH|高雄@TYO|东京@SEL|首尔@SIN|新加坡@BKK|曼谷@NYC|纽约@KUL|吉隆坡@OSA|大阪@LAX|洛杉矶@SYD|悉尼@SFO|旧金山@YVR|温哥华@MEL|墨尔本@JKT|雅加达@CHI|芝加哥@YTO|多伦多@SGN|胡志明市@MNL|马尼拉@DPS|巴厘岛@NGO|名古屋@HKT|普吉岛@HAN|河内@MLE|马累@DXB|迪拜@PUS|釜山@KTM|加德满都\",\r\n");
                    sbScript.AppendFormat(" 'A': \"@{0}\",\r\n", string.Join("@", A_List.ToArray()));
                    sbScript.AppendFormat(" 'B': \"@{0}\",\r\n", string.Join("@", B_List.ToArray()));
                    sbScript.AppendFormat(" 'C': \"@{0}\",\r\n", string.Join("@", C_List.ToArray()));
                    sbScript.AppendFormat(" 'D': \"@{0}\",\r\n", string.Join("@", D_List.ToArray()));
                    sbScript.AppendFormat(" 'E': \"@{0}\",\r\n", string.Join("@", E_List.ToArray()));
                    sbScript.AppendFormat(" 'F': \"@{0}\",\r\n", string.Join("@", F_List.ToArray()));
                    sbScript.AppendFormat(" 'G': \"@{0}\",\r\n", string.Join("@", G_List.ToArray()));
                    sbScript.AppendFormat(" 'H': \"@{0}\",\r\n", string.Join("@", H_List.ToArray()));
                    sbScript.AppendFormat(" 'I': \"@{0}\",\r\n", string.Join("@", I_List.ToArray()));
                    sbScript.AppendFormat(" 'J': \"@{0}\",\r\n", string.Join("@", J_List.ToArray()));
                    sbScript.AppendFormat(" 'K': \"@{0}\",\r\n", string.Join("@", K_List.ToArray()));
                    sbScript.AppendFormat(" 'L': \"@{0}\",\r\n", string.Join("@", L_List.ToArray()));
                    sbScript.AppendFormat(" 'M': \"@{0}\",\r\n", string.Join("@", M_List.ToArray()));
                    sbScript.AppendFormat(" 'N': \"@{0}\",\r\n", string.Join("@", N_List.ToArray()));
                    sbScript.AppendFormat(" 'O': \"@{0}\",\r\n", string.Join("@", O_List.ToArray()));
                    sbScript.AppendFormat(" 'P': \"@{0}\",\r\n", string.Join("@", P_List.ToArray()));
                    sbScript.AppendFormat(" 'Q': \"@{0}\",\r\n", string.Join("@", Q_List.ToArray()));
                    sbScript.AppendFormat(" 'R': \"@{0}\",\r\n", string.Join("@", R_List.ToArray()));
                    sbScript.AppendFormat(" 'S': \"@{0}\",\r\n", string.Join("@", S_List.ToArray()));
                    sbScript.AppendFormat(" 'T': \"@{0}\",\r\n", string.Join("@", T_List.ToArray()));
                    sbScript.AppendFormat(" 'U': \"@{0}\",\r\n", string.Join("@", U_List.ToArray()));
                    sbScript.AppendFormat(" 'V': \"@{0}\",\r\n", string.Join("@", V_List.ToArray()));
                    sbScript.AppendFormat(" 'W': \"@{0}\",\r\n", string.Join("@", W_List.ToArray()));
                    // sbScript.AppendFormat(" 'X': \"@{0}\",\r\n", string.Join("@", X_List.ToArray()));
                    sbScript.AppendFormat(" 'Y': \"@{0}\",\r\n", string.Join("@", Y_List.ToArray()));
                    sbScript.AppendFormat(" 'Z': \"@{0}\"\r\n", string.Join("@", Z_List.ToArray()));
                }


                sbScript.Append("  };\r\n");
                sbScript.Append("  $$.module.address.source.fltdomestic_keyWord = $s2t(\"(可直接输入城市名称查找)\");\r\n");
            }
        }
        catch (Exception ex)
        {
            PnrAnalysis.LogText.LogWrite("获取城市数据：" + ex.Message, "GetCity");
        }
        return sbScript.ToString();
    }
}
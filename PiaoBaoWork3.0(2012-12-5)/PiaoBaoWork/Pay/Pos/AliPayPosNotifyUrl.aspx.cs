using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using PbProject.Model;


/// <summary>
/// Pay_AliPayNotifyUrl
/// </summary>
public partial class Pay_Pos_AliPayPosNotifyUrl : System.Web.UI.Page
{
    /// <summary>
    /// 支付宝的公钥
    /// </summary>
    private string _Public_key = ConfigurationManager.AppSettings["Public_key"].ToString();
    /// <summary>
    /// 商户的私钥
    /// </summary>
    private string _Private_key = ConfigurationManager.AppSettings["Private_key"].ToString();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            OnErrorNew("Pay_Pos_AliPayPosNotifyUrl", true);

            SetData();
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
    /// </summary>
    /// <returns>request回来的信息组成的数组</returns>
    public SortedDictionary<string, string> GetRequestPost()
    {
        int i = 0;
        SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
        NameValueCollection coll;
        //Load Form variables into NameValueCollection variable.
        coll = Request.Form;

        // Get names of all forms into a string array.
        String[] requestItem = coll.AllKeys;

        for (i = 0; i < requestItem.Length; i++)
        {
            sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
        }

        return sArray;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetData()
    {
        if (Request.InputStream.Length > 0)
        {
            //接收并读取POST过来的XML文件流
            StreamReader reader = new StreamReader(Request.InputStream);
            String xmlData = reader.ReadToEnd();

            //String xmlData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Transaction><Transaction_Header><transaction_id>MI0005</transaction_id><requester>1111111111</requester><target>CSZH</target><request_time>20120717175051</request_time><version>1.0</version><ext_attributes><delivery_dept_no>1</delivery_dept_no><delivery_dept>深圳中浩辉</delivery_dept><delivery_man>001</delivery_man><delivery_name>测试</delivery_name></ext_attributes><MAC>lMZS/PhEhRjUVZPmYdA8KlO6CqxvopSWF0YN1+roxluAoXrMvrZ6voHGR8PZFTRhye3cvG/y70Bxbdm2yAKOjkPsJLpZPH1PJ1SQITPCcDVLRqSNo3fn37ndNb9ahQCO9CAffnw89ByKhh3yT5qPRBRMHnezNS0cmZQmG/Ed19E=</MAC></Transaction_Header><Transaction_Body><order_no>DP_2012071711405968246</order_no><order_payable_amt>0.01</order_payable_amt><pay_type>02</pay_type><terminal_id>10001591</terminal_id><trace_no>000227</trace_no></Transaction_Body></Transaction>";
            XElement root = XElement.Parse(xmlData);

            string strTransaction = getXMLNodeValue(xmlData, "transaction_id");
            if (strTransaction == "MI0001")
            {
                Login(root, xmlData);//登录
            }
            else if (strTransaction == "MI0005")
            {
                Pay(root, xmlData);//支付
            }
        }
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="root"></param>
    /// <param name="xmlData"></param>
    private void Login(XElement root, string xmlData)
    {
        #region 

        //Transaction_Header
        string transaction_id = "";
        string requester = "";
        string version = "";
        string target = "";
        string request_time = "";
        string MAC = "";
        //Transaction_Body  
        string delivery_man = "";
        string userName="";
        string password = "";
        string resp_code = "01";
        string resp_msg = "失败";

        //第一段
        foreach (XElement xe in root.Elements("Transaction_Header"))
        {
            transaction_id = xe.Element("transaction_id").Value;
            requester = xe.Element("requester").Value;
            version = xe.Element("version").Value;
            target = xe.Element("target").Value;
            request_time = xe.Element("request_time").Value;
            MAC = xe.Element("MAC").Value;

        }

        //第二段
        foreach (XElement xe in root.Elements("Transaction_Body"))
        {
            delivery_man = xe.Element("delivery_man").Value;
            password = xe.Element("password").Value;
        }

        //获取报文字符串
        string content = removeXMLNodeByName(xmlData, "MAC");
        content = content.Contains("<?xml version=") ? content.Substring(content.IndexOf(">") + 1) : content;
        //开始验签
        string result = sign(content, _Private_key);
        if (verify(content, result, _Public_key))
        {
            //签证签名成功则登陆成功，暂时不和系统账号关联
            resp_code = "00";
            resp_msg = "成功";

            try
            {
                userName = new Com.Alipay.AlipayPOS().getXMLNodeValue(xmlData, "delivery_man");
                password = new Com.Alipay.AlipayPOS().getXMLNodeValue(xmlData, "password");

                //target  // 标示 POS 是那个商家
                //User_Company uCompany = null;
                //PbProject.Logic.ControlBase.BaseDataManage bataManage = new PbProject.Logic.ControlBase.BaseDataManage();
                //string tempSql = "SetName='zfbPosTarget' and SetValue='" + target + "'";
                //List<Bd_Base_Parameters> bbParametersList = bataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { tempSql }) as List<Bd_Base_Parameters>;

                //if (bbParametersList != null && bbParametersList.Count > 0)
                //{
                //    resp_msg = "登陆失败";
                //    resp_code = "08";

                //    uCompany = new PbProject.Logic.User.User_CompanyBLL().GetCompany(bbParametersList[0].CpyNo);

                //    if (uCompany != null && uCompany.AccountState == 1)
                //    {
                //        resp_msg = "登陆成功";
                //        resp_code = "00";
                //    }
                //}
                //else
                //{
                //    resp_msg = "没有此操作员";
                //    resp_code = "03";
                //}

            }
            catch
            {
                resp_code = "08";
                resp_msg = "失败";
            }
        }
        else
        {
            resp_code = "08";
            resp_msg = "失败";
        }
        #endregion

        //构造返回字符串
        StringBuilder sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        StringBuilder sb2 = new StringBuilder();
        StringBuilder sb3 = new StringBuilder();
        sb.Append("<Transaction>");
        sb.Append("<Transaction_Header>");
        sb.Append("<transaction_id>" + transaction_id + "</transaction_id>"); //MI0001 
        sb.Append("<requester>" + requester + "</requester>"); //请求方，支付宝作为发送方时，采用1111111111 为标识码
        sb.Append("<target>" + target + "</target>");//应答方，需填写航旅商户英文名称
        sb.Append("<resp_code>" + resp_code + "</resp_code>"); //正常 00
        sb.Append("<resp_msg>" + resp_msg + "</resp_msg>");//登陆成功
        sb.Append("<resp_time>" + request_time + "</resp_time>");//响应时间，格式为 yyyyMMddHHmmss
        sb.Append("<ext_attributes>");
        sb.Append("<delivery_dept_no>1</delivery_dept_no>");
        sb.Append("<delivery_dept></delivery_dept>");
        sb.Append("</ext_attributes>");

        sb3.Append("</Transaction_Header>");
        sb3.Append("<Transaction_Body>");
        sb3.Append("<delivery_man>" + delivery_man + "</delivery_man>");
        sb3.Append("<delivery_name></delivery_name>");
        sb3.Append("<delivery_zone></delivery_zone>");
        sb3.Append("</Transaction_Body>");
        sb3.Append("</Transaction>");

        content = sb.ToString() + sb3.ToString();
        content = content.Replace("<Transaction>", "");
        content = content.Replace("</Transaction>", "");
        MAC = sign(content.Substring(content.IndexOf(">") + 1), _Private_key);

        sb2.Append("<MAC>" + MAC + "</MAC>");

        Response.Write(sb.ToString() + sb2.ToString() + sb3.ToString());

        string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log\\" + Page + "\\";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        StreamWriter fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true, System.Text.Encoding.Default);
        fs.WriteLine(xmlData + "\r\n\r\n" + sb.ToString() + sb2.ToString() + sb3.ToString());
        fs.Close();
    }

    /// <summary>
    /// 支付
    /// </summary>
    /// <param name="root"></param>
    /// <param name="xmlData"></param>
    private void Pay(XElement root, string xmlData)
    {
        #region 
        //Transaction_Header
        string transaction_id = "";
        string requester = "";
        string version = "";
        string target = "";
        string request_time = "";
        string delivery_dept_no = "";
        string delivery_dept = "";
        string delivery_man = "";
        string delivery_name = "";
        string logistics_alias = "";
        string MAC = "";
        //Transaction_Body 
        string order_no = "";
        string order_payable_amt = "";
        string pay_type = "";
        string terminal_id = "";
        string trace_no = "";
        string resp_code = "";
        string resp_msg = "";

        //第一段
        foreach (XElement xe in root.Elements("Transaction_Header"))
        {
            transaction_id = xe.Element("transaction_id").Value;
            requester = xe.Element("requester").Value;
            version = xe.Element("version").Value;
            target = xe.Element("target").Value;
            request_time = xe.Element("request_time").Value;
            MAC = xe.Element("MAC").Value;
            foreach (XElement xe2 in root.Elements("ext_attributes"))
            {
                delivery_dept_no = xe.Element("delivery_dept_no").Value;
                delivery_dept = xe.Element("delivery_dept").Value;
                delivery_man = xe.Element("delivery_man").Value;
                delivery_name = xe.Element("delivery_name").Value;
                logistics_alias = xe.Element("logistics_alias").Value;
            }
        }
        //第二段
        foreach (XElement xe in root.Elements("Transaction_Body"))
        {
            order_no = xe.Element("order_no").Value; //订单号
            order_payable_amt = xe.Element("order_payable_amt").Value;//应付金额
            pay_type = xe.Element("pay_type").Value;
            terminal_id = xe.Element("terminal_id").Value; // pos机号
            trace_no = xe.Element("trace_no").Value; //凭证号
        }

        //获取报文字符串
        string content = removeXMLNodeByName(xmlData, "MAC");
        content = content.Contains("<?xml version=") ? content.Substring(content.IndexOf(">") + 1) : content;
        //开始验签
        string result = sign(content, _Private_key);
        if (verify(content, result, _Public_key))
        {
            try
            {
                //PiaoBao.BLLLogic.Pay.YeePay yeepay = new PiaoBao.BLLLogic.Pay.YeePay();

                //OnErrorNew("开始记录");
                ////支付宝支持银行卡
                //pay_type = "1";
                //yeepay.Charge(terminal_id, order_payable_amt, pay_type, order_no, request_time);
                //OnErrorNew("结束记录");

                ///// <summary>
                ///// 充值成功,生成充值账单
                ///// </summary>
                ///// <param name="orderId">订单编号</param>
                ///// <param name="payNo">交易号</param>
                ///// <param name="price">交易金额</param>
                ///// <param name="payWay"> //payWay支付方式:1支付宝/2快钱/3汇付/4/财付通/5支付宝网银/6快钱网银/7汇付网银/8财付通网银/
                ///// 9支付宝pos/10快钱pos/11汇付pos/12财付通/13易宝pos/14账户支付</param>
                ///// <param name="useId">付款人id 或者 POS机编号</param>
                ///// <param name="operReason">原因或操作描述</param>
                ///// <param name="remark">备注</param>
                
                PbProject.Logic.Pay.Bill bill = new PbProject.Logic.Pay.Bill();
                bill.CreateLogMoneyDetail("", order_no, order_payable_amt, 9, terminal_id, "POS充值", "POS充值");  // 在线充值

                resp_code = "00";
                resp_msg = "成功";
            }
            catch
            {
                resp_code = "01";
                resp_msg = "失败";
            }
        }
        else
        {
            resp_code = "01";
            resp_msg = "失败";
        }

        #endregion

        //构造返回字符串
        StringBuilder sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        StringBuilder sb2 = new StringBuilder();
        StringBuilder sb3 = new StringBuilder();
        sb.Append("<Transaction>");
        sb.Append("<Transaction_Header>");
        sb.Append("<transaction_id>" + transaction_id + "</transaction_id>"); //MI0005
        sb.Append("<requester>" + requester + "</requester>");// //请求方，支付宝作为发送方时，采用1111111111 为标识码
        sb.Append("<target>" + target + "</target>");//应答方，需填写航旅商户英文名称
        sb.Append("<resp_time>" + request_time + "</resp_time>");//响应时间，格式为 yyyyMMddHHmmss
        sb.Append("<resp_code>" + resp_code + "</resp_code>");//正常 00
        sb.Append("<resp_msg>" + resp_msg + "</resp_msg>");//成功

        sb3.Append("</Transaction_Header>");
        sb3.Append("<Transaction_Body>");
        sb3.Append("<order_no>" + order_no + "</order_no>");//订单号
        sb3.Append("</Transaction_Body>");
        sb3.Append("</Transaction>");

        content = sb.ToString() + sb3.ToString();
        content = content.Replace("<Transaction>", "");
        content = content.Replace("</Transaction>", "");

        MAC = sign(content.Substring(content.IndexOf(">") + 1), _Private_key);

        sb2.Append("<MAC>" + MAC + "</MAC>");

        Response.Write(sb.ToString() + sb2.ToString() + sb3.ToString());

        string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log\\" + Page + "\\";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        StreamWriter fs = new StreamWriter(dir + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true, System.Text.Encoding.Default);
        fs.WriteLine(xmlData + "\r\n\r\n" + sb.ToString() + sb2.ToString() + sb3.ToString());
        fs.Close();
    }

    #region XML操作


    // 从报文中截取指定名称的节点内容（包含节点本身 <transaction_id>MI0005</transaction_id>）
    /// <summary>
    /// 从报文中截取指定名称的节点内容（包含节点本身）
    /// </summary>
    /// <param name="strXML">报文文本</param>
    /// <param name="nodeName">节点名</param>
    /// <returns>节点内容</returns>
    public String getXMLByNodeName(String strXML, String nodeName)
    {
        String openNode = "<" + nodeName + ">";
        String closeNode = "</" + nodeName + ">";
        int startIdx = strXML.IndexOf(openNode);
        int endIdx = strXML.IndexOf(closeNode) + closeNode.Length - startIdx;
        if (startIdx == -1 || endIdx == -1)
        {
            return "找不到对应的节点名：" + nodeName;
        }
        else
        {
            return strXML.Substring(startIdx, endIdx);
        }
    }


    // 移除报文中指定节点名的节点
    /// <summary>
    /// 移除报文中指定节点名的节点
    /// </summary>
    /// <param name="strXML">报文文本</param>
    /// <param name="nodeName">节点名</param>
    /// <returns>移除节点后的报文</returns>
    public String removeXMLNodeByName(String strXML, String nodeName)
    {
        String openNode = "<" + nodeName + ">";
        String closeNode = "</" + nodeName + ">";
        int startIdx = strXML.IndexOf(openNode);
        int endIdx = strXML.IndexOf(closeNode) + closeNode.Length;
        if (startIdx == -1 || endIdx == -1)
        {
            return "找不到对应的节点名：" + nodeName;
        }
        else
        {
            return strXML.Substring(0, startIdx) + strXML.Substring(endIdx);
        }
    }


    // 从报文中截取指定名称的节点值（不包含节点本身）
    /// <summary>
    /// 从报文中截取指定名称的节点值（不包含节点本身）
    /// </summary>
    /// <param name="strXML">报文文本</param>
    /// <param name="nodeName">节点名</param>
    /// <returns>节点值</returns>
    public String getXMLNodeValue(String strXML, String nodeName)
    {
        String nodeXML = getXMLByNodeName(strXML, nodeName);
        int startIdx = nodeXML.IndexOf('>') + 1;
        int endIdx = nodeXML.LastIndexOf('<') - startIdx;
        if (startIdx == -1 || endIdx == -1)
        {
            return "找不到对应的节点名：" + nodeName;
        }
        else
        {
            return nodeXML.Substring(startIdx, endIdx);
        }
    }


    #endregion

    #region 签名操作


    //签名--动态读取XML，解析XML数据，返回签名
    public string RSASignFromXML(string strXML)
    {
        return sign(SignString(strXML), _Private_key);
    }

    //签名字符串组合
    private string SignString(string strXML)
    {
        string strSignString = "";
        strXML = strXML.Substring(strXML.IndexOf("?>") + 2);
        strXML = strXML.Replace("<Transaction>", "");
        strXML = strXML.Replace("</Transaction>", "");
        strXML = strXML.Replace("<Transaction_Header>", "");
        strXML = strXML.Replace("</Transaction_Header>", "");
        strXML = strXML.Replace("<Transaction_Body>", "");
        strXML = strXML.Replace("</Transaction_Body>", "");
        strSignString = removeXMLNodeByName(strXML, "MAC");
        return strSignString;
    }

    #endregion

    #region 加密

    //不对齐MD5加密
    /// <summary>
    /// 不对齐MD5加密
    /// </summary>
    /// <param name="password">要加密的密码</param>
    /// <returns>加密后的密码</returns>
    public string NotAlignmentMD5(string password)
    {
        string newPassword = password;
        //去除基数位0
        //for (int i = 0; i < password.Length; i++)
        //{
        //    //判断是否基数位
        //    if ((i + 1) % 2 == 0 || ((i + 1) % 2 != 0 && password.Substring(i, 1) != "0"))
        //    {
        //        newPassword = newPassword + password.Substring(i, 1);
        //    }
        //}


        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] t = md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(newPassword));
        StringBuilder sb = new StringBuilder(32);
        for (int i = 0; i < t.Length; i++)
        {
            sb.Append(t[i].ToString("x").PadLeft(2, '0'));
        }

        password = sb.ToString().ToUpper();
        newPassword = "";

        for (int i = 0; i < password.Length; i++)
        {
            //判断是否基数位
            if ((i + 1) % 2 == 0 || ((i + 1) % 2 != 0 && password.Substring(i, 1) != "0"))
            {
                newPassword = newPassword + password.Substring(i, 1);
            }
        }

        return newPassword;
    }

    #endregion

    #region rsa

    /// <summary>
    /// 签名
    /// </summary>
    /// <param name="content">需要签名的内容</param>
    /// <param name="privateKey">私钥</param>
    /// <returns></returns>
    public static string sign(string content, string privateKey)
    {
        try
        {
            Encoding code = Encoding.GetEncoding("utf-8");
            byte[] Data = code.GetBytes(content);
            RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
            SHA1 sh = new SHA1CryptoServiceProvider();


            byte[] signData = rsa.SignData(Data, sh);
            return Convert.ToBase64String(signData);
        }
        catch (Exception)
        {
            return "";
        }
    }
    /// <summary>
    /// 验证签名
    /// </summary>
    /// <param name="content">需要验证的内容</param>
    /// <param name="signedString">签名结果</param>
    /// <param name="publicKey">公钥</param>
    /// <returns></returns>
    public static bool verify(string content, string signedString, string publicKey)
    {
        bool result = false;

        Encoding code = Encoding.GetEncoding("utf-8");
        byte[] Data = code.GetBytes(content);
        byte[] data = Convert.FromBase64String(signedString);
        RSAParameters paraPub = ConvertFromPublicKey(publicKey);
        RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
        rsaPub.ImportParameters(paraPub);

        SHA1 sh = new SHA1CryptoServiceProvider();
        result = rsaPub.VerifyData(Data, sh, data);
        return result;
    }

    /// <summary>
    /// 用RSA解密
    /// </summary>
    /// <param name="resData">待解密字符串</param>
    /// <param name="privateKey">私钥</param>
    /// <returns>解密结果</returns>
    public static string decryptData(string resData, string privateKey)
    {

        byte[] DataToDecrypt = Convert.FromBase64String(resData);

        string result = "";


        for (int j = 0; j < DataToDecrypt.Length / 128; j++)
        {
            byte[] buf = new byte[128];
            for (int i = 0; i < 128; i++)
            {

                buf[i] = DataToDecrypt[i + 128 * j];
            }
            result += decrypt(buf, privateKey);
        }
        return result;
    }

    public static string decrypt(byte[] data, string privateKey)
    {
        string result = "";
        RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
        SHA1 sh = new SHA1CryptoServiceProvider();
        byte[] source = rsa.Decrypt(data, false);
        Encoding code = Encoding.GetEncoding("utf-8");


        char[] asciiChars = new char[code.GetCharCount(source, 0, source.Length)];
        code.GetChars(source, 0, source.Length, asciiChars, 0);
        result = new string(asciiChars);

        //result = ASCIIEncoding.ASCII.GetString(source);
        return result;
    }

    /// <summary>
    /// 解析java生成的pem文件私钥
    /// </summary>
    /// <param name="pemstr"></param>
    /// <returns></returns>
    private static RSACryptoServiceProvider DecodePemPrivateKey(String pemstr)
    {
        byte[] pkcs8privatekey;
        pkcs8privatekey = Convert.FromBase64String(pemstr);
        if (pkcs8privatekey != null)
        {

            RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8privatekey);
            return rsa;
        }
        else
            return null;
    }

    private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
    {

        byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        byte[] seq = new byte[15];

        MemoryStream mem = new MemoryStream(pkcs8);
        int lenstream = (int)mem.Length;
        BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
        byte bt = 0;
        ushort twobytes = 0;

        try
        {

            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                binr.ReadByte();	//advance 1 byte
            else if (twobytes == 0x8230)
                binr.ReadInt16();	//advance 2 bytes
            else
                return null;


            bt = binr.ReadByte();
            if (bt != 0x02)
                return null;

            twobytes = binr.ReadUInt16();

            if (twobytes != 0x0001)
                return null;

            seq = binr.ReadBytes(15);		//read the Sequence OID
            if (!CompareBytearrays(seq, SeqOID))	//make sure Sequence for OID is correct
                return null;

            bt = binr.ReadByte();
            if (bt != 0x04)	//expect an Octet string 
                return null;

            bt = binr.ReadByte();		//read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
            if (bt == 0x81)
                binr.ReadByte();
            else
                if (bt == 0x82)
                    binr.ReadUInt16();
            //------ at this stage, the remaining sequence should be the RSA private key

            byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
            RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
            return rsacsp;
        }

        catch (Exception)
        {
            return null;
        }

        finally { binr.Close(); }

    }


    private static bool CompareBytearrays(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;
        int i = 0;
        foreach (byte c in a)
        {
            if (c != b[i])
                return false;
            i++;
        }
        return true;
    }

    private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
    {
        byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

        // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
        MemoryStream mem = new MemoryStream(privkey);
        BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
        byte bt = 0;
        ushort twobytes = 0;
        int elems = 0;
        try
        {
            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                binr.ReadByte();	//advance 1 byte
            else if (twobytes == 0x8230)
                binr.ReadInt16();	//advance 2 bytes
            else
                return null;

            twobytes = binr.ReadUInt16();
            if (twobytes != 0x0102)	//version number
                return null;
            bt = binr.ReadByte();
            if (bt != 0x00)
                return null;


            //------  all private key components are Integer sequences ----
            elems = GetIntegerSize(binr);
            MODULUS = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            E = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            D = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            P = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            Q = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            DP = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            DQ = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            IQ = binr.ReadBytes(elems);

            // ------- create RSACryptoServiceProvider instance and initialize with public key -----
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSAParameters RSAparams = new RSAParameters();
            RSAparams.Modulus = MODULUS;
            RSAparams.Exponent = E;
            RSAparams.D = D;
            RSAparams.P = P;
            RSAparams.Q = Q;
            RSAparams.DP = DP;
            RSAparams.DQ = DQ;
            RSAparams.InverseQ = IQ;
            RSA.ImportParameters(RSAparams);
            return RSA;
        }
        catch (Exception)
        {
            return null;
        }
        finally { binr.Close(); }
    }

    private static int GetIntegerSize(BinaryReader binr)
    {
        byte bt = 0;
        byte lowbyte = 0x00;
        byte highbyte = 0x00;
        int count = 0;
        bt = binr.ReadByte();
        if (bt != 0x02)		//expect integer
            return 0;
        bt = binr.ReadByte();

        if (bt == 0x81)
            count = binr.ReadByte();	// data size in next byte
        else
            if (bt == 0x82)
            {
                highbyte = binr.ReadByte();	// data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;		// we already have the data size
            }



        while (binr.ReadByte() == 0x00)
        {	//remove high order zeros in data
            count -= 1;
        }
        binr.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
        return count;
    }

    #region 解析.net 生成的Pem
    private static RSAParameters ConvertFromPublicKey(string pemFileConent)
    {

        byte[] keyData = Convert.FromBase64String(pemFileConent);
        if (keyData.Length < 162)
        {
            throw new ArgumentException("pem file content is incorrect.");
        }
        byte[] pemModulus = new byte[128];
        byte[] pemPublicExponent = new byte[3];
        Array.Copy(keyData, 29, pemModulus, 0, 128);
        Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
        RSAParameters para = new RSAParameters();
        para.Modulus = pemModulus;
        para.Exponent = pemPublicExponent;
        return para;
    }

    private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
    {
        byte[] keyData = Convert.FromBase64String(pemFileConent);
        if (keyData.Length < 609)
        {
            throw new ArgumentException("pem file content is incorrect.");
        }

        int index = 11;
        byte[] pemModulus = new byte[128];
        Array.Copy(keyData, index, pemModulus, 0, 128);

        index += 128;
        index += 2;//141
        byte[] pemPublicExponent = new byte[3];
        Array.Copy(keyData, index, pemPublicExponent, 0, 3);

        index += 3;
        index += 4;//148
        byte[] pemPrivateExponent = new byte[128];
        Array.Copy(keyData, index, pemPrivateExponent, 0, 128);

        index += 128;
        index += ((int)keyData[index + 1] == 64 ? 2 : 3);//279
        byte[] pemPrime1 = new byte[64];
        Array.Copy(keyData, index, pemPrime1, 0, 64);

        index += 64;
        index += ((int)keyData[index + 1] == 64 ? 2 : 3);//346
        byte[] pemPrime2 = new byte[64];
        Array.Copy(keyData, index, pemPrime2, 0, 64);

        index += 64;
        index += ((int)keyData[index + 1] == 64 ? 2 : 3);//412/413
        byte[] pemExponent1 = new byte[64];
        Array.Copy(keyData, index, pemExponent1, 0, 64);

        index += 64;
        index += ((int)keyData[index + 1] == 64 ? 2 : 3);//479/480
        byte[] pemExponent2 = new byte[64];
        Array.Copy(keyData, index, pemExponent2, 0, 64);

        index += 64;
        index += ((int)keyData[index + 1] == 64 ? 2 : 3);//545/546
        byte[] pemCoefficient = new byte[64];
        Array.Copy(keyData, index, pemCoefficient, 0, 64);

        RSAParameters para = new RSAParameters();
        para.Modulus = pemModulus;
        para.Exponent = pemPublicExponent;
        para.D = pemPrivateExponent;
        para.P = pemPrime1;
        para.Q = pemPrime2;
        para.DP = pemExponent1;
        para.DQ = pemExponent2;
        para.InverseQ = pemCoefficient;
        return para;
    }

    #endregion

    #endregion

    /// <summary>
    /// 记录文本日志
    /// </summary>
    /// <param name="content">记录内容</param>
    /// <param name="IsPostBack">是否记录 Request 参数</param>
    private void OnErrorNew(string errContent, bool IsRecordRequest)
    {
        try
        {
            PbProject.WebCommon.Log.Log.RecordLog(Page.ToString(), errContent, IsRecordRequest, null);
        }
        catch (Exception)
        {

        }
    }
}

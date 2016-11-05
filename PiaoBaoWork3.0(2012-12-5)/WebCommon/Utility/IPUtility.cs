using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using PbProject.WebCommon.Web.Cache;

namespace PbProject.WebCommon.Utility
{
    public class IPUtility
    {
        public class IPArea
        {
            public long StartIP { get; set; }
            public long EndIP { get; set; }
            public string Position { get; set; }
            public string Province { get; set; }
            public string City { get; set; }
            public string ISP { get; set; }

            public IPArea() 
            {
                StartIP = EndIP = 0;
                Position = Province = City = ISP = "";
            }

            public IPArea(long startIP, long endIP, string country, string province, string city, string isp)
            {
                StartIP = startIP;
                EndIP = endIP;
                Position = !string.IsNullOrEmpty(country) ? country.Trim() : "";
                Province = !string.IsNullOrEmpty(province) ? country.Trim() : "";
                City = !string.IsNullOrEmpty(city) ? country.Trim() : "";
                ISP = !string.IsNullOrEmpty(isp) ? country.Trim() : "";
            }
        }

        const string CACHE_KEYNAME = "apc_ipaddress_cache";

        private HttpContext _HttpContext { get; set; }
        private bool _EnableProcessCache { get; set; }
        public string _LastError { get; set; }
        public string IPDBFileName { get; set; }

        public IPUtility()
        {
            _EnableProcessCache = false;
            _HttpContext = null;
        }

        public IPUtility(HttpContext context)
        {
            _EnableProcessCache = false;
            _HttpContext = context;
        }

        public IPUtility(bool enableProcessCache)
        {
            _EnableProcessCache = enableProcessCache;
        }

        private IPArea[] GetIPDBFromHttpContext()
        {
            IPArea[] ipList = null;
            if (_HttpContext != null)
            {
                ipList = IISSingleCachedObject<IPArea[]>.GetInstanceFromHttpContext(CACHE_KEYNAME, _HttpContext);
            }
            if (ipList == null)
            {
                _LastError = "Load from IO.";
                ipList = GetIPDBFromIO();
                if (ipList != null)
                {
                    IISSingleCachedObject<IPArea[]>.AppendCacheObject(CACHE_KEYNAME, _HttpContext, ipList, -1);
                }
            }
            else
            {
                _LastError = "Load from Cache.";
            }
            return ipList;
        }

        private string GetExecRootPath()
        {
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            string result = "";
            if (!string.IsNullOrEmpty(dllPath))
            {
                int len = dllPath.Length;
                int pos = len - 1;
                while (pos >= 0 && dllPath[pos] != '/' && dllPath[pos] != '\\')
                    pos--;
                if (pos >= 0)
                    result = dllPath.Substring(0, pos + 1);
            }

            if (result.StartsWith("file:///") && result.Length > 8)
                result = result.Substring(8);

            return result;
        }

        private IPArea[] GetIPDBFromIO()
        {
            return GetIPDBFromIO("");
        }

        public IPArea[] GetIPDBFromIO(string fileName)
        {
            IPArea[] result = null;

            if (!string.IsNullOrEmpty(fileName))
                IPDBFileName = fileName.Trim();

            if (string.IsNullOrEmpty(IPDBFileName))
                IPDBFileName = "ip.txt";

            try
            {
                string rootPath = GetExecRootPath();
                string fullPath = string.Format("{0}{1}", rootPath, IPDBFileName);

                if (!File.Exists(fullPath))
                {
                    _LastError = string.Format("{0} 不存在", fullPath);
                    return result;
                }
                    

                string lineInput = "";
                string[] ipSeg = null;
                char[] splitors = new char[] { '\t',';',','};
                long ip1 = 0, ip2 = 0;
                string country = "", province = "", city = "", isp = "";
                bool parseSucc = true;
                List<IPArea> buf = new List<IPArea>();
                using (StreamReader sr = new StreamReader(fullPath,System.Text.Encoding.UTF8))
                {
                    lineInput = sr.ReadLine();
                    while (lineInput != null)
                    {
                        ipSeg = lineInput.Split(splitors);
                        //  0 = ip1; 1= ip2; 2 = position; 3 = province; 4 = city; 5 = isp
                        if (ipSeg != null && ipSeg.Length >= 3)
                        { 
                            //  至少有前3项资料
                            if (!long.TryParse(ipSeg[0], out ip1))
                                parseSucc = false;
                            if (!long.TryParse(ipSeg[1], out ip2))
                                parseSucc = false;
                            country = ipSeg[2];
                            //  尝试读取更多资料
                            if (ipSeg.Length >= 4)
                                province = ipSeg[3];

                            if (ipSeg.Length >= 5)
                                city = ipSeg[4];

                            if (ipSeg.Length >= 6)
                                isp = ipSeg[5];

                            if (parseSucc)
                            {
                                buf.Add(new IPArea(ip1, ip2, country, province, city, isp));
                            }
                        }

                        lineInput = sr.ReadLine();
                    }
                }

                if (buf.Count > 0)
                { 
                    result = new IPArea[buf.Count];
                    buf.CopyTo(result);
                }
            }
            catch(Exception ex)
            {
                _LastError = ex.Message;
            }
            finally 
            { }
            
            return result;
        }

        public int FindIPAreaIndex(IPArea[] ipDb, long ipValue)
        {
            if (ipDb == null || ipDb.Length < 1)
                return -1;

            return FindIPAreaIndex(ipDb, 0, ipDb.Length - 1, ipValue);
        }

        public int FindIPAreaIndex(IPArea[] ipDb, int start,int end,long ipValue)
        { 
            int result = -1;
            int loopTimes = 0;
            if (ipDb == null || start >= ipDb.Length || end >= ipDb.Length || end < start)
                return result;

            int distance = end - start,pos = start,offset = start;
            IPArea ipInfo = null;
            while (distance >= 0)
            {
                if (distance % 2 == 0)
                    offset = distance / 2;
                else
                    offset = (distance + 1) / 2;

                ipInfo = ipDb[pos + offset];
                if (ipInfo != null && (ipInfo.StartIP == ipValue || (ipInfo.StartIP < ipValue && ipValue <= ipInfo.EndIP)))
                {
                    return pos + offset;
                }

                if (ipValue < ipInfo.StartIP)
                {
                    end = pos + offset - 1;
                    distance = end - pos;

                }
                else
                {
                    pos = pos + offset + 1;
                    distance = end - pos;
                }
                loopTimes++;
            }
            //  往前找,尝试找到一个宽的ip段
            pos = pos + offset - 1;
            while (pos >= 0)
            {
                ipInfo = ipDb[pos];
                if (ipInfo != null && (ipInfo.StartIP == ipValue || (ipInfo.StartIP < ipValue && ipValue <= ipInfo.EndIP)))
                {
                    return pos;
                }
                pos--;
            }

            return result;
        }

        public IPArea FindFirstIPAreaInfo(string ipAddr)
        {
            long ipValue = GetIPIntValue(ipAddr);
            if (ipValue > 0)
            {
                return FindFirstIPAreaInfo(ipValue);
            }
            return null;
        }

        public IPArea FindFirstIPAreaInfo(long ipValue)
        {
            IPArea result = null;

            IPArea[] ipDb = GetIPDBFromHttpContext();

            if (ipDb == null || ipDb.Length < 1)
                return result;

            int idx = FindIPAreaIndex(ipDb, ipValue);
            if (idx >= 0 && idx < ipDb.Length)
                result = ipDb[idx];

            return result;
        }

        /// <summary>
        /// true=有效的IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIPAddress(string ip)
        {
            bool result = false;
            try
            {
                IPAddress ipAddr = IPAddress.Parse(ip);
                result = true;
            }
            catch { result = false; }

            return result;
        }

        /// <summary>
        /// 将IP地址转为数值形式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long GetIPIntValue(IPAddress ip)
        {
            int x = 3;
            long value = 0;

            byte[] ipBytes = ip.GetAddressBytes();
            foreach (byte b in ipBytes)
            {
                value += (long)b << 8 * x--;
            }
            return value;
        }

        /// <summary>
        /// 将IP地址转为数值形式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static long GetIPIntValue(string ip)
        {
            long result = -1;
            try
            {
                IPAddress ipAddr = IPAddress.Parse(ip);
                result = GetIPIntValue(ipAddr);
            }
            catch { }
            finally { }

            return result;
        }

        /// <summary>
        /// true=局域网IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsInnerIP(string ip)
        {
            bool result = false;
            try
            {
                long ipIntValue = GetIPIntValue(ip);

                if ((ipIntValue >> 24 == 0xa) || (ipIntValue >> 16 == 0xc0a8) || (ipIntValue >> 22 == 0x2b0))
                {
                    result = true;
                }
            }
            catch { }
            finally { }

            return result;
        }

        /// <summary>
        /// true=局域网IP
        /// </summary>
        /// <param name="ipIntValue"></param>
        /// <returns></returns>
        public static bool IsInnerIP(long ipIntValue)
        {
            bool result = false;
            if ((ipIntValue >> 24 == 0xa) || (ipIntValue >> 16 == 0xc0a8) || (ipIntValue >> 22 == 0x2b0))
            {
                result = true;
            }
            return result;
        }
    }
}

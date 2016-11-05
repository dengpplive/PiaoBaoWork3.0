using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PiaoBao.API.Common.AirQuery
{
    public class AirInfoCollection
    {
        public string FlightNum { get; set; }
        private List<AirInfo> airInfoList = new List<AirInfo>();

        public List<AirInfo> AirInfoList
        {
            get { return airInfoList; }
            set { airInfoList = value; }
        }
        
    }
}
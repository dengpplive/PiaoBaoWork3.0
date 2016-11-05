using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PiaoBao.API.Common.AirQuery
{
    public class AirInfoCollectionList
    {
        private List<AirInfoCollection> firstAirInfoList = new List<AirInfoCollection>();

        public List<AirInfoCollection> FirstAirInfoList
        {
            get { return firstAirInfoList; }
            set { firstAirInfoList = value; }
        }
        private List<AirInfoCollection> secondAirInfoList = new List<AirInfoCollection>();

        public List<AirInfoCollection> SecondAirInfoList
        {
            get { return secondAirInfoList; }
            set { secondAirInfoList = value; }
        }
        public string CacheNameGuid { get; set; }
    }
}
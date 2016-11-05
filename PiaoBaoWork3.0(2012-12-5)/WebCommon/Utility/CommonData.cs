using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PbProject.WebCommon.Utility
{
    /// <summary>
    /// 公共数据处理类
    /// </summary>
    public class CommonData<T>
    {
        /// <summary>
        /// DataTable 装换道List<DataRow>
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public List<DataRow> DataTableToList(DataTable table)
        {
            List<DataRow> list = new List<DataRow>();
            int Len = 0;
            if (table != null && table.Rows.Count > 0)
            {
                Len = table.Rows.Count;
                DataRow[] drs = new DataRow[Len];
                table.Rows.CopyTo(drs, 0);
                list = drs.ToList<DataRow>();
            }
            return list;
        }
        /// <summary>
        /// List数据分页
        /// </summary>
        /// <param name="DataSource">数据源List<T>"/></param>
        /// <param name="TotalCount">总条数</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="PageSize">每页大小</param>
        /// <returns></returns>
        public List<T> GetBasePager(List<T> DataSource, out int TotalCount, int PageIndex, int PageSize)
        {
            TotalCount = 0;
            if (DataSource == null)
            {
                return null;
            }
            //总记录数
            TotalCount = DataSource.Count;
            if (PageIndex <= 0)
            {
                PageIndex = 1;
            }
            if (PageSize <= 0)
            {
                PageSize = 0;
            }
            int startIndex = (PageIndex - 1) * PageSize;
            if (startIndex <= 0)
            {
                startIndex = 0;
            }
            if (startIndex > TotalCount)
            {
                startIndex = TotalCount;
            }
            int endIndex = PageIndex * PageSize;
            if (endIndex > TotalCount)
            {
                endIndex = TotalCount;
            }
            int Len = endIndex - startIndex;
            if (Len <= 0)
            {
                Len = 0;
            }
            return DataSource.GetRange(startIndex, Len);
        }
    }
}

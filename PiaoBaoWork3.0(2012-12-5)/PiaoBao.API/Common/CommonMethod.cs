using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PiaoBao.API.Common
{
    public class CommonMethod
    {
        /// <summary>
        /// 转换get方式传过来的Date类型(20130503→2013-05-03)，地址栏日期参数必须是8位
        /// </summary>
        /// <param name="dateString">地址栏消除了“-”的日期类型string</param>
        /// <returns>返回带“-”的日期string</returns>
        public static string GetFomartDate(string dateString)
        {
            return dateString.Insert(4, "-").Insert(7, "-");
        }

        /// <summary>
        /// 转换get方式传过来的Time类型(0730→07:30)，地址栏日期参数必须是8位
        /// </summary>
        /// <param name="dateString">地址栏消除了“:”的时间类型string</param>
        /// <returns>返回带“:”的时间string</returns>
        public static string GetFomartTime(string timeString)
        {
            return timeString.Insert(2, ":");
        }

        /// <summary>
        /// 将obj转换为string，obj为null时返回string.Empty
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetFomartString(object obj)
        {
            return obj == null ? string.Empty : obj.ToString().Trim();
        }

        /// <summary>
        /// 将obj转换为string，obj为null时返回"0"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetFomartNumberString(object obj)
        {
            return obj == null || obj.ToString().Trim().Equals(string.Empty) ? "0" : obj.ToString().Trim();
        }
    }
}
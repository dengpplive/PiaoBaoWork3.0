using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbProject.Model.definitionParam
{
    public struct airSelectErrorShow
    {
        /// <summary>
        /// 缓存服务意外关闭,请联系管理员开启!
        /// </summary>
        public static readonly string cacheClose = "缓存服务意外关闭,请联系管理员开启!";
        /// <summary>
        /// 查询基础数据超时,请重新查询,多次失败,请联系管理员!
        /// </summary>
        public static readonly string cacheTimeOut = "查询基础数据超时,请重新查询,多次失败,请联系管理员!";
        /// <summary>
        /// 查询服务意外关闭,请重新查询,多次失败,请联系管理员!
        /// </summary>
        public static readonly string ibeOrPidClose = "查询服务意外关闭,请重新查询,多次失败,请联系管理员!";
        /// <summary>
        /// 查询航班服务超时,请重新查询,多次失败,请联系管理员!
        /// </summary>
        public static readonly string ibeTimeOut = "查询航班服务超时,请重新查询,多次失败,请联系管理员!";
        /// <summary>
        /// 无可用航班信息!
        /// </summary>
        public static readonly string ibeNoAV = "无可用航班信息!";
        /// <summary>
        /// 基础数据读取错误,请重新查询,多次失败,请联系管理员!
        /// </summary>
        public static readonly string dataBaseError = "基础数据读取错误,请重新查询,多次失败,请联系管理员!";
        /// <summary>
        /// 未开启任何查询配置,请联系管理员!
        /// </summary>
        public static readonly string noAV = "未开启任何查询配置,请联系管理员!";
        /// <summary>
        /// 航段信息解析失败,请联系管理员!
        /// </summary>
        public static readonly string avAnalysis = "航段信息解析失败,请联系管理员!";
        /// <summary>
        /// 代理人配置异常,请联系管理员!
        /// </summary>
        public static readonly string avHeiPinAbnormity = "代理人配置异常,请联系管理员!";
    }
}

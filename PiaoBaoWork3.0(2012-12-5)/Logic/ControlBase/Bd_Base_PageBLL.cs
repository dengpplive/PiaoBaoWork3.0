using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Dal.ControlBase;
using DataBase.Data;
using DataBase.Unique;

namespace PbProject.Logic.ControlBase
{
    /// <summary>
    /// 页面权限
    /// </summary>
    public class Bd_Base_PageBLL
    {

        PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <returns></returns>
        public List<Bd_Base_Page> GetList()
        {
            //return new Dal.ControlBase.BaseData<Bd_Base_Page>().GetList("");

            return baseDataManage.CallMethod("Bd_Base_Page", "GetList", null, new Object[] { " 1=1 order by ModuleIndex,OneMenuIndex,TwoMenuIndex " }) as List<Bd_Base_Page>;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <returns></returns>
        public List<Bd_Base_Page> GetListByRoleType(int roleType)
        {
            //List<Bd_Base_Page> bPaseList = new List<Bd_Base_Page>();
            //try
            //{
            //     //bPaseList = new Dal.ControlBase.BaseData<Bd_Base_Page>().GetList();
            //     //bPaseList = bPaseList.Where(c => c.RoleType == roleType).ToList<Bd_Base_Page>();

            //}
            //catch (Exception)
            //{
            //}
            //return bPaseList;

            return baseDataManage.CallMethod("Bd_Base_Page", "GetList", null, new Object[] { "RoleType=" + roleType + " order by ModuleIndex,OneMenuIndex,TwoMenuIndex " }) as List<Bd_Base_Page>;
        }


        /// <summary>
        /// 查询缓存：通过 角色类型 所有数据
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <returns></returns>
        public List<Bd_Base_Page> GetListByCache(int roleType)
        {
            List<Bd_Base_Page> basePageList = null;
            try
            {
                PbProject.WebCommon.Web.Cache.CacheManage cacheManage = new WebCommon.Web.Cache.CacheManage();
                basePageList = cacheManage.GetCacheData("GetBasePageList", "GetBasePageList") as List<Bd_Base_Page>;

                if (basePageList == null || basePageList.Count == 0)
                {
                    basePageList = GetList();
                    cacheManage.SetCacheData("GetBasePageList", basePageList, -1, "GetBasePageList");
                }

                if (basePageList != null && basePageList.Count > 0)
                    basePageList = basePageList.Where(w => w.RoleType == roleType).ToList<Bd_Base_Page>();

                basePageList.Sort();
            }
            catch (Exception)
            {

            }
            return basePageList;
        }


        ///// <summary>
        ///// 查询
        ///// </summary>
        ///// <param name="id">id</param>
        ///// <returns></returns>
        //public List<Bd_Base_Page> GetListById(Guid id)
        //{
        //    List<Bd_Base_Page> bPaseList = new List<Bd_Base_Page>();
        //    try
        //    {
        //        bPaseList = new Dal.ControlBase.BaseData<Bd_Base_Page>().GetList();
        //        bPaseList = bPaseList.Where(c => c.id == id).ToList<Bd_Base_Page>();

        //        //IEnumerable<Bd_Base_Page> querys = null;
        //        //querys = from c in bPaseList where c.RoleType == roleType select c;
        //        //bPaseNewList = querys.ToList<Bd_Base_Page>();
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return bPaseList;
        //}

        /// <summary>
        /// 通过角色类型 获取最大的:PageIndex
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <returns></returns>
        public int GetPageIndexMaxByRoleType(int roleType)
        {
            int result = 0;
            try
            {
                List<Bd_Base_Page> bPaseList = new Bd_Base_PageBLL().GetList();

                result = bPaseList.Where(w => w.RoleType == roleType).Select(ss => ss.PageIndex).Max();
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
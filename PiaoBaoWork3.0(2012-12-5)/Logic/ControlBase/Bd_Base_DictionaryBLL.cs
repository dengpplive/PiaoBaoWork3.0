using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Dal.ControlBase;
using DataBase.Data;
using DataBase.Unique;
using System.Web.Caching;

namespace PbProject.Logic.ControlBase
{
    /// <summary>
    /// 字典表
    /// </summary>
    public class Bd_Base_DictionaryBLL
    {
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <param name="roleType">获取所有数据</param>
        /// <returns></returns>
        public List<Bd_Base_Dictionary> GetList()
        {
            return new Dal.ControlBase.BaseData<Bd_Base_Dictionary>().GetList(" 1=1 order by ChildID");
        }

        /// <summary>
        /// 按照 字典类型 分组
        /// </summary>
        /// <returns></returns>
        public List<Bd_Base_Dictionary> GetListGroupByParentID()
        {
            List<Bd_Base_Dictionary> bPaseNewList = new List<Bd_Base_Dictionary>();
            try
            {
                bPaseNewList = GetList();

            }
            catch (Exception)
            {
            }
            return bPaseNewList;
        }

        /// <summary>
        ///  获取最大的:ParentID
        /// </summary>
        /// <returns></returns>
        public int GetBaseDictionaryMaxParentID()
        {
            int result = 0;
            try
            {
                List<Bd_Base_Dictionary> bPaseList = GetList();
                result = bPaseList.Select(ss => ss.ParentID).Max();
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 获取最大的:ChildID ,通过父级id
        /// </summary>
        /// <param name="parentID">父级 id</param>
        /// <returns></returns>
        public int GetBaseDictionaryMaxChildIDByParentID(int parentID)
        {
            int result = 0;
            try
            {
                List<Bd_Base_Dictionary> bPaseList = GetList();
                result = bPaseList.Where(w => w.ParentID == parentID).Select(ss => ss.ChildID).Max();
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 字典表缓存：通过父级 id 读取所有数据
        /// </summary>
        /// <param name="parentID">通过父级 id</param>
        /// <returns></returns>
        public List<Bd_Base_Dictionary> GetListByParentID(int parentID)
        {
            List<Bd_Base_Dictionary> DictionaryList = null;
            try
            {
                PbProject.WebCommon.Web.Cache.CacheManage cacheManage = new WebCommon.Web.Cache.CacheManage();
                DictionaryList = cacheManage.GetCacheData("GetBaseDictionaryList", "GetBaseDictionaryList") as List<Bd_Base_Dictionary>;

                if (DictionaryList == null || DictionaryList.Count == 0)
                {
                    DictionaryList = GetList();
                    cacheManage.SetCacheData("GetBaseDictionaryList", DictionaryList, -1, "GetBaseDictionaryList");
                }

                if (DictionaryList != null && DictionaryList.Count > 0)
                    DictionaryList = DictionaryList.Where(w => w.ParentID == parentID).ToList<Bd_Base_Dictionary>();

                DictionaryList.Sort();
            }
            catch (Exception)
            {

            }
            return DictionaryList;
        }

        /// <summary>
        /// 通过字典表类型 id 获取字典表数据
        /// </summary>
        /// <param name="parentId">字典表类型id</param>
        /// <param name="childId">childId</param>
        /// <returns></returns>
        public string GetDictionaryName(string parentId, string childId)
        {
            string dictionaryName = "";
            try
            {
                List<Bd_Base_Dictionary> DictionaryList = GetListByParentID(int.Parse(parentId));

                if (DictionaryList != null && DictionaryList.Count > 0)
                    DictionaryList = DictionaryList.Where(w => w.ChildID == int.Parse(childId)).ToList<Bd_Base_Dictionary>();

                if (DictionaryList != null && DictionaryList.Count > 0)
                    dictionaryName = DictionaryList[0].ChildName;
            }
            catch (Exception)
            {

            }
            return dictionaryName;
        }
    }
}
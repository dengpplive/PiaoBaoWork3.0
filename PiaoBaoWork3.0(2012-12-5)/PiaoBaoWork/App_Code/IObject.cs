using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///IObject 的摘要说明
/// </summary>
public interface IObject
{
    /// <summary>
    /// 页面之间需要传递的对象
    /// </summary>
    Object PageObj { get; set; }
}
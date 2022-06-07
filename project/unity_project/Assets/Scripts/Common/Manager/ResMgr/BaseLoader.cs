using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseLoader
{
    /// <summary>
    /// 加载完成后的回调
    /// </summary>
    public List<Action<UnityEngine.Object>> callbackList = new List<Action<UnityEngine.Object>>();

    /// <summary>
    /// 资源名称
    /// </summary>
    public string mAssetName;

    /// <summary>
    /// 资源全路径
    /// </summary>
    protected string mFullPath;

    /// <summary>
    /// 资源类型
    /// </summary>
    public Type mType;

    /// <summary>
    /// 是否已经加载完成
    /// </summary>
    protected bool mIsDone = false;

    public List<BaseLoader> dependList = new List<BaseLoader>();

    public virtual bool IsDone
    {
        get
        {
            return mIsDone;
        }
    }

    /// <summary>
    /// 开始加载
    /// </summary>
    public abstract void DoLoad();

    public virtual void AddCallback(Action<UnityEngine.Object> callback)
    {
        callbackList.Add(callback);
    }
}

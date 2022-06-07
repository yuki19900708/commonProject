using UnityEngine;
using System;
using System.IO;
using System.Collections;

/// <summary>
/// 加载Resources资源信息
/// </summary>
public class ResourcesLoader : BaseLoader
{
    private string assetPath;
    /// <summary>
    /// 资源加载的异步
    /// </summary>
    private ResourceRequest mResourceRequest;

    /// <summary>
    /// 资源
    /// </summary>
    public UnityEngine.Object Asset
    {
        get
        {
            return mResourceRequest.asset;
        }
    }

    /// <summary>
    /// 当前资源是否已经加载完成
    /// </summary>
    public override bool IsDone
    {
        get
        {
            return mResourceRequest.isDone;
        }
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <param name="type">资源类型</param>
    public ResourcesLoader(string assetName, string assetPath, Type type)
    {
        mAssetName = assetName;
        mType = type;
        mFullPath = Path.Combine(assetPath, assetName);
    }

    /// <summary>
    /// 开始加载
    /// </summary>
    public override void DoLoad()
    {
        mResourceRequest = Resources.LoadAsync(mFullPath, mType);
    }
}
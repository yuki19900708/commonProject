using UnityEngine;
using System.Collections;

public class AssetBundleInfo
{
    /// <summary>
    /// 资源完整的名称 包含后缀名
    /// </summary>
    public string assetNameWithExtension;

    /// <summary>
    /// 资源包的名称
    /// </summary>
    public string assetBundleName;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="assetNameWithExtension">资源名称 包含后缀名</param>
    /// <param name="assetBundleName">资源包的名称</param>
    public AssetBundleInfo(string assetNameWithExtension, string assetBundleName)
    {
        this.assetBundleName = assetBundleName;
        this.assetNameWithExtension = assetNameWithExtension;
    }
}

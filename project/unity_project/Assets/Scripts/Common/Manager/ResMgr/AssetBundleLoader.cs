using UnityEngine;
using System.IO;
using System.Collections;

/// <summary>
/// 加载AssetBundle资源信息
/// </summary>
public class AssetBundleLoader : BaseLoader
{
    private AssetBundleCreateRequest mAssetBundleCreateRequest;

    public string AssetBundleName
    {
        get;
        set;
    }

    public AssetBundle AssetBundle
    {
        get
        {
            return mAssetBundleCreateRequest.assetBundle;
        }
    }

    public override bool IsDone
    {
        get
        {
            return mAssetBundleCreateRequest.isDone;
        }
    }
    
    public AssetBundleLoader(string assetBundleName, string assetBundlePath)
    {
        this.AssetBundleName = assetBundleName;
        mFullPath = assetBundlePath;
    }

    public override void DoLoad()
    {
        mAssetBundleCreateRequest = AssetBundle.LoadFromFileAsync(mFullPath);
    }
}

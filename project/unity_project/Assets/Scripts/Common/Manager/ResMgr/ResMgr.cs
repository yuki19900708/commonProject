using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 封装了Resources目录与AssetBundle的资源加载管理类
/// </summary>

public class ResMgr : MonoBehaviour
{
    public static event Action<string> Event_LocalResInitFailed;
    public static event Action Event_LocalResInitFinish;

    public static ResMgr Instance
    {
        private set;
        get;
    }

    /// <summary>
    /// 约定的AssetBundle输出目录名，Unity会在该目录下一个生成同名的AssetBundle文件记录所有assetbundle的依赖关系
    /// 因此，这个变量也作用那个记录依赖关系的文件名
    /// </summary>
    public static readonly string ASSET_BUNDLE_FOLDER = "AssetBundle";
    public static readonly string ASSET_BUNDLE_INI = "assetBundleIni";
    public static readonly string RESOURCES_INI = "resIni";
    /// <summary>
    /// 安装包必须保留的AssetBundle资源，用于保证游戏首次运行所需要资源可以立即加载的到
    /// </summary>
    public static readonly string[] LOCAL_ASSET_BUNDLE_NAMES = { "AssetBundle",
                                                                "login",
                                                                "textdatatable",
                                                                "tutorialtabledatatable",
                                                                "assetbundleini",
                                                                "spriteatlasini",
                                                                "starupcomicspage1",
                                                                "starupcomicspage2",
                                                                "starupcomicspage3",
                                                                "starupcomicspage4",
                                                                "starupcomicspage5",
                                                                "starupcomicspage6",
                                                                "versionInfo.txt"};

    public static bool IS_LOCAL_RES_INITIALIZED = false;

    /// <summary>
    /// 所有Resources目录下的资源的数据，格式为：资源名=资源路径
    /// </summary>
    private Dictionary<string, string> mResourcesInfoDic = new Dictionary<string, string>();

    /// <summary>
    /// 所有生成的AssetBundle的数据，格式为：资源名(无扩展名)|资源名（有扩展名)|资源AssetBundleName
    /// </summary>
    private Dictionary<string, AssetBundleInfo> mAssetBundleInfoDic = new Dictionary<string, AssetBundleInfo>();

    /// <summary>
    /// 最大同时加载数量
    /// </summary>
    private int mValidprocessorConut = 0;

    /// <summary>
    /// 资源缓存
    /// </summary>
    private Dictionary<string, UnityEngine.Object> mCachedAssetsDic = new Dictionary<string, UnityEngine.Object>();

    /// <summary>
    /// AssetBundle缓存
    /// </summary>
    private Dictionary<string, AssetBundle> mCachedAssetBundle = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 正在进行的加载器
    /// </summary>
    private List<BaseLoader> mLoadingLoaders = new List<BaseLoader>();

    /// <summary>
    /// 等待加载的加载器
    /// </summary>
    private List<BaseLoader> mWaitingLoaders = new List<BaseLoader>();

    /// <summary>
    /// AssetBundle依赖关系汇总文件
    /// </summary>
    private AssetBundleManifest mAssetBundleDependencySummary = null;
    private AssetBundle summaryBundle = null;
    private AssetBundle assetBundleIniBundle = null;

    void Awake()
    {
        Instance = this;
        //最大加载数等于cpu核数
        mValidprocessorConut = SystemInfo.processorCount;
    }

    void Update()
    {
        ProcessLoaders();
    }

    #region Public Methods
    /// <summary>
    /// ResMgr初始化流程
    /// 1. Awake时初始化一次，保证Resources目录资源及安装包自带的AssetBundle可以正常加载
    /// 2. 如果PersistenDataPath下有之前下载的AssetBundle，也会在Awake时初始化好
    /// 3. 当热更模块更新完成后，再调用ReInit初始化一次AssetBundle
    /// </summary>
    public static void Init()
    {
        if (IS_LOCAL_RES_INITIALIZED == false)
        {
            LoadResourcesIni();
            string assetBundleSummaryFile = Path.Combine(Application.persistentDataPath, ASSET_BUNDLE_FOLDER + "/" + ASSET_BUNDLE_FOLDER);

            if (File.Exists(assetBundleSummaryFile))
            {
                InitDependencySummary();
                LoadAssetBundleIni();
            }
            IS_LOCAL_RES_INITIALIZED = true;
            if (Event_LocalResInitFinish != null)
            {
                Event_LocalResInitFinish();
            }
        }
    }

    /// <summary>
    /// 重新初始化,因为热更有可能更新了AssetBundleIni
    /// </summary>
    public static void ReInit()
    {
        //重新加载前一定要卸载
        //这两个资源不在mCachedAssetBundle中，所以单独卸载
        if (Instance.assetBundleIniBundle != null)
        {
            Instance.assetBundleIniBundle.Unload(true);
        }
        if (Instance.summaryBundle != null)
        {
            Instance.summaryBundle.Unload(true);
        }
        InitDependencySummary();
        LoadAssetBundleIni();
    }

    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <returns></returns>
    public static T Load<T>(string assetName) where T : UnityEngine.Object
    {
        if (Instance.mAssetBundleDependencySummary == null || Instance.mAssetBundleInfoDic.Count <= 0)
        {
            Debug.LogWarning("ResMgr 有人在ResMgr Init执行前就调用了加载资源");
            Init();
        }
        string mainAsset = assetName;
        if (mainAsset.Contains("/"))
        {
            mainAsset = mainAsset.Split('/')[0];
        }
        if (Instance.mAssetBundleInfoDic.ContainsKey(mainAsset))
        {
            return Instance.LoadFromAssetBundle<T>(assetName);
        }
        else if (Instance.mResourcesInfoDic.ContainsKey(mainAsset))
        {
            return Instance.LoadFromResource<T>(assetName);
        }
        else
        {
            Debug.LogError("你要加载的资源不存在 资源名称：" + assetName);
            return null;
        }
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <param name="type">资源类型</param>
    /// <param name="callback">回调</param>
    public static void LoadAsync(string assetName, Type type, Action<UnityEngine.Object> callback)
    {
        if (Instance.mAssetBundleInfoDic.ContainsKey(assetName))
        {
            Instance.LoadFromAssetBundleAsync(assetName, callback);
        }
        else if (Instance.mResourcesInfoDic.ContainsKey(assetName))
        {
            Instance.LoadFromResourcesAsync(assetName, type, callback);
        }
        else
        {
            Debug.LogError("你要加载的资源不存在 资源名称：" + assetName);
            if (callback != null)
            {
                callback(null);
            }
        }
    }

    /// <summary>
    /// 卸载Asset资源（Resources目录下的资源），调用的时候确保没有其他对象引用要卸载的资源了
    /// </summary>
    /// <param name="asset">要卸载的资源</param>
    public static void UnLoadAsset(UnityEngine.Object asset, bool forceTriggerGC = false)
    {
        Resources.UnloadAsset(asset);
        Resources.UnloadUnusedAssets();
        if (forceTriggerGC)
        {
            GC.Collect();
        }
    }

    /// <summary>
    /// 完全卸载AssetBundle资源
    /// </summary>
    /// <param name="assetBundleName">AssetBundle的名字</param>
    public static void UnloadAssetBundle(string assetBundleName)
    {
        if (Instance.mCachedAssetBundle.ContainsKey(assetBundleName))
        {
            Instance.mCachedAssetBundle[assetBundleName].Unload(true);
            Instance.mCachedAssetBundle.Remove(assetBundleName);
        }
        else
        {
            //你要卸载的资源不归ResMgr管，请自行卸载
            Debug.LogWarning("你要卸载的资源不归ResMgr管，请自行卸载");
        }
    }

    /// <summary>
    /// 完全卸载AssetBundle资源
    /// </summary>
    /// <param name="assetBundleName">AssetBundle的名字</param>
    public static void UnloadAssetBundle(AssetBundle assetBundle)
    {
        if (Instance.mCachedAssetBundle.ContainsValue(assetBundle))
        {
            //因为完全卸载后，对象就变成null了，在下次加载时，会处理null情况，于是在这里就不遍历字典查找key,然后删除该项了
            assetBundle.Unload(true);
        }
        else
        {
            //你要卸载的资源不归ResMgr管，请自行卸载
            Debug.LogWarning("(编辑器中请忽略)你要卸载的资源不归ResMgr管，请自行卸载");
        }
    }

    /// <summary>
    /// 卸载AssetBundle的内存镜像
    /// 注意，不会完全卸载
    /// 虽然不会完全卸载，但是对象仍然会变成null，然而下次加载的时候会报重复错误，使用者需要小心合理使用
    /// </summary>
    public static void UnloadAssetBundleMemoryImage(string assetBundleName)
    {
        if (Instance.mCachedAssetBundle.ContainsKey(assetBundleName))
        {
            Instance.mCachedAssetBundle[assetBundleName].Unload(false);
            Instance.mCachedAssetBundle.Remove(assetBundleName);
        }
        else
        {
            //你要卸载的资源不归ResMgr管，请自行卸载
            Debug.LogWarning("你要卸载的资源不归ResMgr管，请自行卸载");
        }
    }

    /// <summary>
    /// 卸载AssetBundle的内存镜像
    /// 注意，不会完全卸载
    /// 虽然不会完全卸载，但是对象仍然会变成null，然而下次加载的时候会报重复错误，使用者需要小心合理使用
    /// </summary>
    public static void UnloadAssetBundleMemoryImage(AssetBundle assetBundle)
    {
        if (Instance.mCachedAssetBundle.ContainsValue(assetBundle))
        {
            //因为完全卸载后，对象就变成null了，在下次加载时，会处理null情况，于是在这里就不遍历字典查找key,然后删除该项了
            assetBundle.Unload(false);
        }
        else
        {
            //你要卸载的资源不归ResMgr管，请自行卸载
            Debug.LogWarning("你要卸载的资源不归ResMgr管，请自行卸载");
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// 处理所有的Loader（异步加载）
    /// </summary>
    private void ProcessLoaders()
    {
        if (mLoadingLoaders.Count != 0)
        {
            for (int i = 0; i < mLoadingLoaders.Count;)
            {
                if (mLoadingLoaders[i].IsDone)
                {
                    AsyncLoadFinish(mLoadingLoaders[i]);
                    mLoadingLoaders.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        //由于AssetBundle加载有依赖关系必须有加载先后顺序
        //在等待加载列表中的加载请求不能简单的按顺序处理
        //必须得根据依赖关系保证先后顺序
        while (mLoadingLoaders.Count < mValidprocessorConut && mWaitingLoaders.Count != 0)
        {
            //以下代码是针对加载AssetBundle的，因为AssetBundle之间可能存在依赖关系，
            //因此要从等待队列中找出一个可以进行加载的AssetBundle请求，需要先检测它的所有依赖项是否加载完成
            int validIndex = 0;
            BaseLoader loader = mWaitingLoaders[validIndex];
            bool isDependAllFinsih = CheckDependLoaderDone(loader);
            while (isDependAllFinsih == false && validIndex < mWaitingLoaders.Count - 1)
            {
                validIndex++;
                loader = mWaitingLoaders[validIndex];
                isDependAllFinsih = CheckDependLoaderDone(loader);
            }

            if (isDependAllFinsih && validIndex < mWaitingLoaders.Count)
            {
                mLoadingLoaders.Add(loader);
                loader.DoLoad();
                mWaitingLoaders.RemoveAt(validIndex);
            }

            //如果所有的Loader检查完，仍然不能将CPU可以承受的最大队列填满，那么就break吧，不然就死循环了
            if (validIndex >= mWaitingLoaders.Count && mLoadingLoaders.Count < mValidprocessorConut)
            {
                break;
            }
        }
    }

    /// <summary>
    /// 获取AssetBundle文件的完整路径
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
    private string GetAssetBundleFullName(string assetBundleName)
    {
        string basePath = Path.Combine(Application.persistentDataPath, ASSET_BUNDLE_FOLDER);
        return Path.Combine(basePath, assetBundleName);
    }

    /// <summary>
    /// 检查依赖项的Loader是否完成
    /// </summary>
    /// <param name="loader"></param>
    /// <returns></returns>
    private bool CheckDependLoaderDone(BaseLoader loader)
    {
        bool result = true;
        foreach(BaseLoader depend in loader.dependList)
        {
            result &= CheckDependLoaderDone(depend);
            //如果有任何一个依赖项没有加载完成，直接返回false
            if (!result)
            {
                return result;
            }
        }
        return result;
    }

    private static void InitDependencySummary()
    {
        string assetBundleSummaryFile = Path.Combine(Application.persistentDataPath, ASSET_BUNDLE_FOLDER + "/" + ASSET_BUNDLE_FOLDER);
        if (File.Exists(assetBundleSummaryFile))
        {
            try
            {
                Instance.summaryBundle = AssetBundle.LoadFromFile(assetBundleSummaryFile);
                Instance.mAssetBundleDependencySummary = Instance.summaryBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            catch (Exception ex)
            {
            	//此处捕获到异常说明初始化ResMgr必须的资源文件被损失（原因可能是下载失败，下载中断，篡改等）
                if (Event_LocalResInitFailed != null)
                {
                    Event_LocalResInitFailed(ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// 加载Resources资源配置文件
    /// </summary>
    private static void LoadResourcesIni()
    {
        TextAsset txt = Resources.Load<TextAsset>(RESOURCES_INI);
        if (txt != null)
        {
            System.IO.TextReader reader = new System.IO.StringReader(txt.text);
            string lineTxt = reader.ReadLine();
            while (!string.IsNullOrEmpty(lineTxt))
            {
                string[] resData = lineTxt.Split('|');
                string key = resData[0];
                string value = resData[1];
                Instance.mResourcesInfoDic[key] = value;
                lineTxt = reader.ReadLine();
            }
            reader.Close();
        }
    }

    /// <summary>
    /// 加载AssetBundle资源配置文件
    /// </summary>
    private static void LoadAssetBundleIni()
    {
        string assetBundleIniPrimaryPath = Path.Combine(Application.persistentDataPath, ASSET_BUNDLE_FOLDER + "/assetbundleini");
        string iniData = string.Empty;
        if (File.Exists(assetBundleIniPrimaryPath))
        {
            Instance.assetBundleIniBundle = AssetBundle.LoadFromFile(assetBundleIniPrimaryPath);
            iniData = Instance.assetBundleIniBundle.LoadAsset<TextAsset>("assetBundleIni").text;
        }
        else
        {
            iniData = Resources.Load<TextAsset>(ASSET_BUNDLE_INI).text;
        }
        if (string.IsNullOrEmpty(iniData) == false)
        {
            TextReader reader = new StringReader(iniData);
            string lineTxt = reader.ReadLine();

            Instance.mAssetBundleInfoDic.Clear();
            while (!string.IsNullOrEmpty(lineTxt))
            {
                string[] abInfoData = lineTxt.Split('|');
                string assetNameWithoutExtension = abInfoData[0];
                string assetNameWithExtension = abInfoData[1];
                string assetBundleName = abInfoData[2];
                Instance.mAssetBundleInfoDic[assetNameWithoutExtension] = new AssetBundleInfo(assetNameWithExtension, assetBundleName);
                lineTxt = reader.ReadLine();
            }
            reader.Close();
        }
    }

    /// <summary>
    /// 获取缓存中的资源
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <returns></returns>
    private UnityEngine.Object GetCachedAsset(string assetName)
    {
        if (mCachedAssetsDic.ContainsKey(assetName))
        {
            UnityEngine.Object asset = mCachedAssetsDic[assetName];
            if (asset == null)
            {
                mCachedAssetsDic.Remove(assetName);
            }
            return asset;
        }
        return null;
    }

    /// <summary>
    /// 获取缓存的AssetBundle
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <returns></returns>
    private AssetBundle GetCachedAssetBundle(string assetBundleName)
    {
        if (mCachedAssetBundle.ContainsKey(assetBundleName))
        {
            AssetBundle assetBundle = mCachedAssetBundle[assetBundleName];
            if (assetBundle == null)
            {
                mCachedAssetBundle.Remove(assetBundleName);
            }
            return assetBundle;
        }
        return null;
    }

    /// <summary>
    /// 检查要加载的资源是否已经加载完成，或者正在加载，或者等待加载
    /// 是以上三种情形之一，返回false，否则返回true
    /// </summary>
    /// <param name="assetName">要加载的资源</param>
    /// <param name="callback">回调</param>
    /// <returns></returns>
    private bool IsAssetShouldLoad(string assetName, Action<UnityEngine.Object> callback)
    {
        //如果缓存里有，直接返回
        if (mCachedAssetsDic.ContainsKey(assetName))
        {
            if (callback != null)
            {
                callback(mCachedAssetsDic[assetName]);
            }
            return false;
        }

        //检查该资源是否正在加载中，如果是：直接将回调加入回调列表中
        for (int i = 0; i < mLoadingLoaders.Count; i++)
        {
            if (mLoadingLoaders[i].mAssetName == assetName)
            {
                if (!mLoadingLoaders[i].callbackList.Contains(callback))
                {
                    mLoadingLoaders[i].AddCallback(callback);
                }
                return false;
            }
        }

        //检查该资源是否正在等待加载，如果是：直接将回调加入回调列表中
        foreach (BaseLoader requestBase in mWaitingLoaders)
        {
            if (requestBase.mAssetName == assetName)
            {
                if (!requestBase.callbackList.Contains(callback))
                {
                    requestBase.AddCallback(callback);
                }
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 检查要加载的AssetBundle是否已经加载完成，或者等待加载，或者正在加载
    /// 是以上三种情形之一，返回false，否则返回true
    /// </summary>
    private bool IsAssetBundleShouldLoad(string assetBundleName)
    {
        if (mCachedAssetBundle.ContainsKey(assetBundleName))
        {
            return false;
        }

        foreach (BaseLoader loader in mWaitingLoaders)
        {
            if (loader is AssetBundleLoader)
            {
                AssetBundleLoader abLoader = loader as AssetBundleLoader;
                if (abLoader.AssetBundleName.Equals(assetBundleName))
                {
                    return false;
                }
            }
        }

        foreach (BaseLoader loader in mLoadingLoaders)
        {
            if (loader is AssetBundleLoader)
            {
                AssetBundleLoader abLoader = loader as AssetBundleLoader;
                if (abLoader.AssetBundleName.Equals(assetBundleName))
                {
                    return false;
                }
            }
        }

        return true;
    }

#region 从AssetBundle中同步加载资源
    /// <summary>
    /// 从AssetBundle中同步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName"></param>
    /// <returns></returns>
    private T LoadFromAssetBundle<T>(string assetName) where T : UnityEngine.Object
    {
        UnityEngine.Object res = GetCachedAsset(assetName);
        if (res == null)
        {
            string mainAsset = assetName;
            if (mainAsset.Contains("/"))
            {
                mainAsset = mainAsset.Split('/')[0];
            }
            AssetBundleInfo info = mAssetBundleInfoDic[mainAsset];
            string[] dependencies = mAssetBundleDependencySummary.GetAllDependencies(info.assetBundleName);
            foreach (string dependency in dependencies)
            {
                //同步方法只需要判断要加载的AssetBundle是否在缓存中即可，在即已经加载过了
                AssetBundle dependAssetBundle = GetCachedAssetBundle(dependency);
                if (dependAssetBundle == null)
                {
                    LoadAssetBundle(dependency);
                }
            }

            AssetBundle assetBundle = GetCachedAssetBundle(info.assetBundleName);
            if (assetBundle == null)
            {
                string fullName = GetAssetBundleFullName(info.assetBundleName);
                assetBundle = AssetBundle.LoadFromFile(fullName);
                mCachedAssetBundle.Add(info.assetBundleName, assetBundle);
            }
           
            if (assetBundle != null)
            {
                res = assetBundle.LoadAsset(assetName, typeof(T));
                mCachedAssetsDic.Add(assetName, res);
            }
            else
            {
                Debug.LogErrorFormat("加载AssetBundle:{0}出错", info.assetBundleName);
            }
        }
        return (T)res;
    }

    /// <summary>
    /// 加载AssetBundle
    /// </summary>
    /// <param name="assetBundleName"></param>
    private void LoadAssetBundle(string assetBundleName)
    {
        string[] dependencies = mAssetBundleDependencySummary.GetAllDependencies(assetBundleName);
        foreach (string dependency in dependencies)
        {
            //同步方法只需要判断要加载的AssetBundle是否在缓存中即可，在即已经加载过了
            AssetBundle dependAssetBundle = GetCachedAssetBundle(dependency);
            if (dependAssetBundle == null)
            {
                LoadAssetBundle(dependency);
            }
        }

        string fullName = GetAssetBundleFullName(assetBundleName);
        AssetBundle assetBundle = AssetBundle.LoadFromFile(fullName);
        mCachedAssetBundle.Add(assetBundleName, assetBundle);
    }
#endregion

#region 从AssetBundle中异步加载资源
    /// <summary>
    /// 从AssetBundle中异步加载指定资源
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <param name="callback">回调</param>
    private void LoadFromAssetBundleAsync(string assetName, Action<UnityEngine.Object> callback)
    {
        if (IsAssetShouldLoad(assetName, callback))
        {
            AssetBundleInfo info = mAssetBundleInfoDic[assetName];
            AssetBundleLoader loader = new AssetBundleLoader(info.assetNameWithExtension, info.assetBundleName);
            loader.AddCallback(callback);
            string[] dependencies = mAssetBundleDependencySummary.GetAllDependencies(info.assetBundleName);
            foreach (string dependency in dependencies)
            {
                //异步方法加载AssetBundle，检查是否加载过时，需要判断的内容较多，具体见方法说明
                if (IsAssetBundleShouldLoad(dependency))
                {
                    LoadAsssetBundleAsync(dependency, loader);
                }
            }
            mWaitingLoaders.Add(loader);
        }
    }

    /// <summary>
    /// 加载指定的AssetBundle
    /// </summary>
    /// <param name="assetBundleName">要加载的AssetBundle</param>
    /// <param name="startLoader">如果该AssetBundle是一个被依赖项，那么startLoader是依赖它的那个加载请求</param>
    private void LoadAsssetBundleAsync(string assetBundleName, AssetBundleLoader startLoader)
    {
        AssetBundleLoader loader = new AssetBundleLoader(null, assetBundleName);
        startLoader.dependList.Add(loader);
        string[] dependencies = mAssetBundleDependencySummary.GetAllDependencies(assetBundleName);
        foreach (string dependency in dependencies)
        {
            //异步方法加载AssetBundle，检查是否加载过时，需要判断的内容较多，具体见方法说明
            if (IsAssetBundleShouldLoad(dependency))
            {
                LoadAsssetBundleAsync(dependency, loader);
            }
        }
        mWaitingLoaders.Add(loader);
    }
#endregion

#region 从Resources中同步加载资源
    /// <summary>
    /// 从Resources中同步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="assetName"></param>
    /// <returns></returns>
    private T LoadFromResource<T>(string assetName) where T : UnityEngine.Object
    {
        T res = GetCachedAsset(assetName) as T;
        if (res == null)
        {
            string fullPath = Path.Combine(mResourcesInfoDic[assetName], assetName);
            res = Resources.Load<T>(fullPath);
            mCachedAssetsDic.Add(assetName, res);
        }
        return res;
    }
#endregion

#region 从Resources中异步加载资源
    /// <summary>
    /// 从Resources中异步加载指定的资源
    /// </summary>
    /// <param name="assetName">资源名称</param>
    /// <param name="type">资源类型</param>
    /// <param name="callback">回调</param>
    private void LoadFromResourcesAsync(string assetName, Type type, Action<UnityEngine.Object> callback)
    {
        if (IsAssetShouldLoad(assetName, callback))
        {
            type = type ?? typeof(GameObject);
            ResourcesLoader loader = new ResourcesLoader(assetName, mResourcesInfoDic[assetName], type);
            loader.AddCallback(callback);
            mWaitingLoaders.Add(loader);
        }
    }
#endregion

#region 异步加载完成的回调
    /// <summary>
    /// 资源加载完成
    /// </summary>
    /// <param name="loader"></param>
    private void AsyncLoadFinish(BaseLoader loader)
    {
        if (loader is AssetBundleLoader)
        {
            LoadAssetBundleAsyncFinish(loader as AssetBundleLoader);
        }
        else
        {
            LoadResourcesAsyncFinish(loader as ResourcesLoader);
        }
    }

    /// <summary>
    /// AssetBundle加载完成
    /// </summary>
    /// <param name="loader"></param>
    private void LoadAssetBundleAsyncFinish(AssetBundleLoader loader)
    {
        AssetBundle assetBundle = loader.AssetBundle;

        if (assetBundle == null)
        {
            foreach (Action<UnityEngine.Object> listener in loader.callbackList)
            {
                if (listener != null)
                {
                    listener(null);
                }
            }
        }
        else
        {
            mCachedAssetBundle.Add(assetBundle.name, assetBundle);
            //如果mAssetName是个null, 那么表示这个AssetBundle是一个被依赖项，无需加载具体资源
            if (string.IsNullOrEmpty(loader.mAssetName) == false)
            {
                StartCoroutine(LoadAssetFromAssetBundleAsync(assetBundle, loader.mAssetName, loader.callbackList));
            }
        }
    }

    /// <summary>
    /// 异步加载AssetBundle中的资源
    /// </summary>
    /// <param name="assetBundle"></param>
    /// <param name="assetName"></param>
    /// <param name="callbackList"></param>
    /// <returns></returns>
    private IEnumerator LoadAssetFromAssetBundleAsync(AssetBundle assetBundle, string assetName, List<Action<UnityEngine.Object>> callbackList)
    {
        UnityEngine.AssetBundleRequest areq = assetBundle.LoadAssetAsync(mAssetBundleInfoDic[assetName].assetNameWithExtension);
        yield return areq;
        if (areq.asset != null)
        {
            mCachedAssetsDic.Add(areq.asset.name, areq.asset);
        }
        for (int i = 0; i < callbackList.Count; i++)
        {
            if (callbackList[i] != null)
            {
                callbackList[i](areq.asset);
            }
        }
    }

    /// <summary>
    /// Resources资源加载完成
    /// </summary>
    /// <param name="loader">加载的结果</param>
    private void LoadResourcesAsyncFinish(ResourcesLoader loader)
    {
        if (loader.Asset != null)
        {
            mCachedAssetsDic.Add(loader.Asset.name, loader.Asset);
        }
        for (int i = 0; i < loader.callbackList.Count; i++)
        {
            if (loader.callbackList[i] != null)
            {
                loader.callbackList[i](loader.Asset);
            }
        }
    }
#endregion
#endregion
}

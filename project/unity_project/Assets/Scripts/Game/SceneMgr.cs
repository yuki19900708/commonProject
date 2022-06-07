using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
//using TMPro;

public class SceneMgr : MonoBehaviour
{
    public GameObject errorObj;
    public Text errorText;
    public Button fixButton;
    //public TextMeshProUGUI fixButtonText;

    //public ManualFixUI manualFixUI;
    //public TextMeshProUGUI versionTimestampText;

    void Start()
    {
        fixButton.onClick.AddListener(OnFixButtonClick);
        //if (L10NMgr.gameLanguageType == LanguageTypeEnum.Chinese)
        //{
        //    //fixButtonText.text = "自助修复";
        //}
        //else
        //{
        //    //fixButtonText.text = "Manual Fix";
        //}
        //manualFixUI.Hide();
        errorObj.SetActive(false);
        fixButton.gameObject.SetActive(false);
        StartCoroutine(PreWarmLocalRes());

        //这里只展示版本号与时间戳，因为没有UID，方便定位出问题时的版本
        //versionTimestampText.text = string.Format("{0}({1})", U3DToPlatform.GetAppVersionNumber(), GameConfig.BuildTimestamp);
    }

    private void OnFixButtonClick()
    {
        //manualFixUI.Show(SimpleFixCallback, CompleteFixCallback);
    }

    private void SimpleFixCallback()
    {
        errorObj.SetActive(false);
        fixButton.gameObject.SetActive(false);
        //简单修复完直接重新开始预热资源
        StartCoroutine(PreWarmLocalRes());
    }

    private void CompleteFixCallback()
    {
        errorObj.SetActive(false);
        fixButton.gameObject.SetActive(false);
        //重度修复完不必退出游戏，直接重新开始预热资源
        StartCoroutine(PreWarmLocalRes());
    }

    /// <summary>
    /// 本地资源预热
    /// 此时应该全面检查初始化必需资源的完整性，避免初始化失败导致卡在Loading场景UI上的bug
    /// </summary>
    private IEnumerator PreWarmLocalRes()
    {
        if (VerifyLocalInitRes())
        {
            yield return 0;
            //资源校验成功后，还需要检测游戏版本是不是发生了改变，游戏版本改变后，需要将安装包内的所有资源再向PersistentDataPath中释放一次
            //if (PlayerProfile.LoadVersionCache().Equals(U3DToPlatform.GetAppVersionNumber()) == false)
            //{
            //    //Debug.Log("RYH", "SceneMgr", "重新释放安装包内资源");
            //    //版本发生变动后，就再执行一次CopyLocalAssets，但并不保存新版本，因为进入Main场景后，GameMgr中需要再次进行版本变更判断，那时才会保存新版本
            //    //CopyLocalAssets完成后，会自动进入Main场景
            //    yield return StartCoroutine(CopyLocalAssets());
            //}
            //else
            //{
            //    EnterMainScene();
            //}
        }
        else
        {
            //校验资源失败时，根据进入游戏的次数决定修复方式
            //首次进入游戏，直接复制所有的本地资源
            //非首次进入游戏，仅将初始化必需的资源修复，剩余的交给热更修复
            int gameLaunchCount = PlayerProfile.LoadGameLaunchCount();
            if (gameLaunchCount == 0)
            {
                yield return StartCoroutine(CopyLocalAssets());
            }
            else
            {
                yield return StartCoroutine(FixLocalInitRequiredAssets());
            }
            gameLaunchCount++;
            PlayerProfile.SaveGameLaunchCount(gameLaunchCount);
        }
    }

    private void EnterMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 校验本地初始化的必需资源
    /// </summary>
    private bool VerifyLocalInitRes()
    {
        string persistentDataPath = Application.persistentDataPath;
        //先检查persistentDataPath目录versionInfo.txt文件是否存在
        string localVersionFile = Path.Combine(persistentDataPath, "AssetBundle/versionInfo.txt");
        if (File.Exists(localVersionFile))
        {
            //string localVersion;
            //Dictionary<string, ResUpdateMgr.UpdateFileInfo> localFileHashInfo;
            //再检查该文件是否可以正常解析
            //try
            //{
            //    //ResUpdateMgr.ParseVersionInfoByContent(File.ReadAllText(localVersionFile), out localVersion, out localFileHashInfo);
            //}
            //catch (Exception ex)
            //{
            //    //Debug.Log("RYH", "SceneMgr", string.Format("解析versionInfo.txt出错：{0}", ex.Message));
            //    return false;
            //}

            //try
            //{
                //比较所有必需资源的文件MD5码与记录在versionInfo.txt中MD5码，不一致就认为损坏
                //foreach (KeyValuePair<string, ResUpdateMgr.UpdateFileInfo> kvp in localFileHashInfo)
                //{
                //    string fileName = Path.GetFileName(kvp.Key);
                //    if (IsLocalInitRequiredAssets(fileName))
                //    {
                //        ResUpdateMgr.UpdateFileInfo localFileInfo = kvp.Value;
                //        string localVersionHash = localFileInfo.fileHash;
                //        string localFilePath = persistentDataPath + "/AssetBundle/" + localFileInfo.filePath;
                //        string localFileHash = HashUtil.ComputeMD5(localFilePath);
                //        if (localVersionHash != localFileHash)
                //        {
                //            Debug.Log("RYH", "SceneMgr", string.Format("必需资源：{0}损坏，将自动修复", fileName));
                //            //只要有一个必需资源不正常，算失败
                //            return false;
                //        }
                //    }
                //}
                return true;
            //}
            //catch (Exception ex)
            //{
            //    //Debug.Log("RYH", "SceneMgr", string.Format("对比必需资源MD5过程中出现异常：{0}", ex.Message));
            //    return false;
            //}
        }
        else
        {
            return false;
        }
    }

    private bool IsLocalInitRequiredAssets(string assetName)
    {
        for(int i = 0; i < ResMgr.LOCAL_ASSET_BUNDLE_NAMES.Length; i++)
        {
            if (ResMgr.LOCAL_ASSET_BUNDLE_NAMES[i] == assetName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 将游戏安装包中的初始化必需资源从StreamingAssetsPath复制到PersistentDataPath中
    /// </summary>
    private IEnumerator CopyLocalAssets()
    {
#if UNITY_ANDROID
        //Android平台为了保持包体处于最小状态，除了初始化必需的资源外，剩余的AssetBundle都不带在安装包内，首次进入游戏通过热更下载
        //保留Login界面的AssetBundle是为了保证玩家一进入游戏UI能正确加载
        //Android平台只能使用www异步加载StreamingAssets目录下的资源。或者C#与Java通信，由Unity端通知Android应用端去复制文件（仍然是异步）
        yield return StartCoroutine(FixLocalInitRequiredAssets());
#elif UNITY_IOS
        //iOS平台为了避免触犯苹果审核的最小可运行原则（不是特别确定）而被拒，需要将所有的AssetBundle放在安装包内，审核通过后再增加热更的内容
        try
        {
            Debug.Log("RYH", "SceneMgr", "IOS开始预热资源");
            FileUtilAddon.CopyFolder(Application.streamingAssetsPath, Application.persistentDataPath);
            Debug.Log("RYH", "SceneMgr", "IOS结束预热资源");
            EnterMainScene();
        }
        catch (Exception e)
        {
            Debug.LogError("SceneMgr 初始化本地AssetBundle文件出错: " + e.Message);
            errorText.text = string.Format("SceneMgr Initialize local assets error:\n{0}\nPlease try 'Manual Fix'", e.Message);
            errorObj.SetActive(true);
        
            fixButton.gameObject.SetActive(true);
        }
        yield return 0;
#else
        yield return 0;
        EnterMainScene();
#endif
    }

    private IEnumerator FixLocalInitRequiredAssets()
    {
        bool fixSucceed = true;
        string persistentDataPath = Application.persistentDataPath;
        foreach (string assetBundleName in ResMgr.LOCAL_ASSET_BUNDLE_NAMES)
        {
            string path = "AssetBundle/" + assetBundleName;
            string url = FileUtilAddon.GetStreamingAssetsPathForWWW(path);
            WWW www = new WWW(url);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                string targetPath = Path.Combine(persistentDataPath, path);
                string dir = Path.GetDirectoryName(targetPath);
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllBytes(targetPath, www.bytes);
                //Debug.Log("RYH", "SceneMgr", "预热资源：" + assetBundleName);
            }
            else
            {
                Debug.LogError(string.Format("SceneMgr 初始化本地资源：{0} 出错：{1}", url, www.error));
                errorText.text = string.Format("SceneMgr Initialize local assets error:\n{0}\n{1}\nPlease try 'Manual Fix'", url, www.error);
                errorObj.SetActive(true);
                fixSucceed = false;
            }
        }

        if (fixSucceed)
        {
            EnterMainScene();
        }
        else
        {
            fixButton.gameObject.SetActive(true);
        }
    }
}

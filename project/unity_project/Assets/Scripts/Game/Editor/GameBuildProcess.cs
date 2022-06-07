using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

public class GameBuildProcess : IPreprocessBuild, IPostprocessBuild
{
    //private static string symbolsBeforeBuild;

    public int callbackOrder
    {
        get
        {
            return 0;
        }
    }

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        ResourcesPostprocessor.enabled = false;
        ProcessSymbols(target);
        PrepareForBuild();
    }

    public void OnPostprocessBuild(BuildTarget target, string path)
    {
        RevertSymbolsModification(target);
        RevertBuildModification();
        ResourcesPostprocessor.enabled = true;
    }

    public static BuildTargetGroup BuildTargetToGroup(BuildTarget target)
    {
        if (target == BuildTarget.Android)
        {
            return BuildTargetGroup.Android;
        }
        else if (target == BuildTarget.iOS)
        {
            return BuildTargetGroup.iOS;
        }
        return BuildTargetGroup.Unknown;
    }

    public static void PrepareForBuild()
    {
        HashSet<Transform> addSpriteLoaderSet = new HashSet<Transform>();
        //给场景中使用了Sprite的GameObject添加UGUISpriteTag脚本
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            //Loading场景不对Sprite引用关系进行解除操作
            if (scene.enabled && scene.path != "Assets/Scenes/Loading.unity")
            {
                Scene scene1 = EditorSceneManager.OpenScene(scene.path);
                var images = Object.FindObjectsOfType<Image>();
                foreach (Image image in images)
                {
                    //有UGUISpriteTagImmune组件的不添加UGUISpriteTag
                    bool hasImmune = image.GetComponent<UGUISpriteTagImmune>() != null;
                    bool isValid = (image.sprite != null) &&
                                    (UGUISpriteTag.UGUI_EMBED_ATLAS.Contains(image.sprite.texture.name) == false) &&
                                    hasImmune == false;
                    if (isValid)
                    {
                        if (image.gameObject.GetComponent<UGUISpriteTag>() == null)
                        {
                            image.gameObject.AddComponent<UGUISpriteTag>();
                        }
                        Transform root = image.transform.root;
                        if (addSpriteLoaderSet.Contains(root) == false)
                        {
                            addSpriteLoaderSet.Add(root);
                            if (root.gameObject.GetComponent<UGUISpriteLoader>() == null)
                            {
                                root.gameObject.AddComponent<UGUISpriteLoader>();
                            }
                        }
                    }
                }
                var srs = Object.FindObjectsOfType<SpriteRenderer>();
                foreach (SpriteRenderer sr in srs)
                {
                    //有UGUISpriteTagImmune组件的不添加UGUISpriteTag
                    bool hasImmune = sr.GetComponent<UGUISpriteTagImmune>() != null;
                    bool isValid = (sr.sprite != null) &&
                                    (UGUISpriteTag.UGUI_EMBED_ATLAS.Contains(sr.sprite.texture.name) == false) &&
                                    hasImmune == false;

                    if (isValid)
                    {
                        if (sr.gameObject.GetComponent<UGUISpriteTag>() == null)
                        {
                            sr.gameObject.AddComponent<UGUISpriteTag>();
                        }
                        Transform root = sr.transform.root;
                        if (addSpriteLoaderSet.Contains(root) == false)
                        {
                            addSpriteLoaderSet.Add(root);
                            if (root.gameObject.GetComponent<UGUISpriteLoader>() == null)
                            {
                                root.gameObject.AddComponent<UGUISpriteLoader>();
                            }
                        }
                    }
                }
                EditorSceneManager.SaveScene(scene1, scene.path);
            }
        }


        GameObject[] assets = Resources.LoadAll<GameObject>("Prefabs");
        foreach (GameObject go in assets)
        {
            if (go.GetComponent<UGUISpriteLoader>() == null)
            {
                UGUISpriteLoader loader = go.AddComponent<UGUISpriteLoader>();
                if (go.name == "LoginUI")
                {
                    loader.loadFromLocal = true;
                }
            }
            var images = go.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                bool hasImmune = image.GetComponent<UGUISpriteTagImmune>() != null;
                bool isValid = (image.sprite != null) &&
                                (UGUISpriteTag.UGUI_EMBED_ATLAS.Contains(image.sprite.texture.name) == false) &&
                                hasImmune == false;

                if (isValid)
                {
                    if (image.gameObject.GetComponent<UGUISpriteTag>() == null)
                    {
                        image.gameObject.AddComponent<UGUISpriteTag>();
                    }
                }
            }
            var srs = go.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in srs)
            {
                //有UGUISpriteTagImmune组件的不添加UGUISpriteTag
                bool hasImmune = sr.GetComponent<UGUISpriteTagImmune>() != null;
                bool isValid = (sr.sprite != null) &&
                                (UGUISpriteTag.UGUI_EMBED_ATLAS.Contains(sr.sprite.texture.name) == false) &&
                                hasImmune == false;
                if (isValid)
                {
                    if (sr.gameObject.GetComponent<UGUISpriteTag>() == null)
                    {
                        sr.gameObject.AddComponent<UGUISpriteTag>();
                    }
                }
            }
        }
#if UNITY_ANDROID
        //安卓平台因为安装包可惜不带AssetBundle，为了保持apk包体积最小，
        //需要将StreamingAssets下的AssetBundle资源在Build之前删除（除初始化必须的资源外）
        string[] files = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "AssetBundle"));
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            bool shouldDelete = true;
            foreach (string localRes in ResMgr.LOCAL_ASSET_BUNDLE_NAMES)
            {
                //跳过本地必须保留的AssetBundle资源
                if (fileName == localRes)
                {
                    shouldDelete = false;
                    break;
                }
            }
            if (shouldDelete)
            {
                //其他资源直接删除
                File.Delete(file);
            }
        }
#elif UNITY_IOS
        //iOS平台因为安装包要带上所有的AssetBundle，因此什么也不做
#endif
    }

    /// <summary>
    /// 根据日志开关，决定最终的Script Define Symbols
    /// </summary>
    /// <param name="target"></param>
    public static void ProcessSymbols(BuildTarget target)
    {
        //LogUtil.LogToFile("GameConfig.LogEnabled: " + GameConfig.LogEnabled);
        //BuildTargetGroup targetGroup = BuildTargetToGroup(target);
        //symbolsBeforeBuild = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        //if (GameConfig.LogEnabled == false)
        //{
        //    string newSymbols = symbolsBeforeBuild.Replace("UNITY_LOG", "");
        //    LogUtil.LogToFile("NewSymbols: " + newSymbols);
        //    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newSymbols);
        //}
    }

    /// <summary>
    /// 还原symbols
    /// </summary>
    /// <param name="target"></param>
    public static void RevertSymbolsModification(BuildTarget target)
    {
        //BuildTargetGroup targetGroup = BuildTargetToGroup(target);
        //if (GameConfig.LogEnabled == false)
        //{
        //    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbolsBeforeBuild);
        //}
    }

    public static void RevertBuildModification()
    {
        Process process = new Process();
        string checkoutCmd = "\"git checkout ./project/unity_project/Assets/Scenes ./project/unity_project/Assets/Resources/Prefabs\"";
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C " + checkoutCmd;
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            process.StartInfo.FileName = "bash";
            process.StartInfo.Arguments = "-c " + checkoutCmd;
        }
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.WorkingDirectory = Path.Combine(Path.GetFullPath("."), "../../");
        process.Start();
        process.WaitForExit();
        //string result = process.StandardOutput.ReadToEnd();
        //Debug.Log("Checkout scenes " + result);
        process.Close();
    }
}

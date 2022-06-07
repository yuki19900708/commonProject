using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.Net;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.Networking;

public class AssetBundlePipeline : EditorWindow
{
    /// <summary>
    /// 自动命名AssetBundle的策略
    /// </summary>
    public enum AutoNameStrategy
    {
        /// <summary>
        /// 使用资源路径的GUID做为AssetBundle名
        /// </summary>
        AssetPathToGUID,
        /// <summary>
        /// 使用资源名做为AssetBundle名（可能会有重名资源）
        /// </summary>
        AssetFileName,
        /// <summary>
        /// 使用资源路径做为AssetBundle名（可保证唯一，路径中的/会被替换为@）
        /// </summary>
        AssetFilePath,
        /// <summary>
        /// 使用资源所在在文件夹做为AssetBundle名（可能会有重名文件夹）
        /// </summary>
        AssetFolderName,
        /// <summary>
        /// 使用资源所在在文件夹的路径做为AssetBundle名（可保证唯一，路径中的/会被替换为@）
        /// </summary>
        AssetFolderPath,
    }

    private static string uploadHost = "http://192.168.196.28/";
    private static string uploadService = "park-of-monster/upload_zip.php";
    private static string uploadURL = uploadHost + uploadService;

    private static string zipFile = @"AssetBundle.zip";
    private static string streamingAssetsPath;

    /// <summary>
    /// 要打包AssetBundle的目录
    /// </summary>
    private static string workingPath;
    /// <summary>
    /// 要排除的文件扩展名
    /// </summary>
    private static string autoNameExcludeExtension = "tpsheet meta xlsx atlas.txt";
    private static string epAutoNameExcludeExtension = "AssetBundlePipeline_autoNameExcludeExtension";
    /// <summary>
    /// 输出路径
    /// </summary>
    private static string outputPath = "AssetBundle";
    private static string epOutputPath = "AssetBundlePipeline_outputPath";
    /// <summary>
    /// 完整输出路径，包含persistentDataPath
    /// </summary>
    private static string finalOutputPath = string.Empty;
    /// <summary>
    /// AssetBundle目标平台
    /// </summary>
    private static BuildTarget buildTarget = BuildTarget.Android;
    private static string epBuildTarget = "AssetBundlePipeline_buildTarget";
    /// <summary>
    /// AssetBundle起名策略
    /// </summary>
    private static AutoNameStrategy autoNameStrategy = AutoNameStrategy.AssetPathToGUID;
    private static string epAutoNameStrategy = "AssetBundlePipeline_autoNameStrategy";
    /// <summary>
    /// AssetBundle生成配置项，默认采用 不压缩+仅记录变化
    /// 不压缩是为了读取时可以使用流式加载，仅记录变化保证在未修改的情况下生成的文件MD5相同，方便客户端判断更新
    /// </summary>
    private static BuildAssetBundleOptions buildAssetBundleOption = BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.DeterministicAssetBundle;

    /// <summary>
    /// 生成完AssetBundle后是否生成信息汇总文件,记录资源名与AssetBundle名的对应关系
    /// 当AssetBundle的名字不具有可读性（使用GUID）时，有助于加载或获取资源（资源名是明确的）
    /// </summary>
    private static bool generateIniData = true;
    private static string epGenerateIniData = "AssetBundlePipeline_generateIniData";
    /// <summary>
    /// 生成完AssetBundle后是否删除Manifest文件（记录了各资源之间的包含与引用关系），正常使用要删除，调试时不删除
    /// </summary>
    private static bool deleteManifest = true;
    private static string epDeleteManifest = "AssetBundlePipeline_deleteManifest";
    /// <summary>
    /// 生成完AssetBundle后是否上传到服务器上（具体上传方式与服务器商定）
    /// </summary>
    private static bool uploadToServer = true;
    private static string epUploadToServer = "AssetBundlePipeline_uploadToServer";
    
    private static string epIsBuildAssetBundleFinish = "AssetBundlePipeline_isBuildAssetBundleFinish";
    private static bool isZipping = false;
    private static EditorApplication.CallbackFunction editorUpdateCallback = null;

    private static UnityWebRequest downloadVersionRequets;

    private static AssetBundlePipeline instance;

    [MenuItem("Appcpi/数据生成/3.更新AssetBundle", false, 3)]
    public static void Init()
    {
        LoadSavedConfig();
        instance = EditorWindow.CreateInstance<AssetBundlePipeline>();
        instance.titleContent = new GUIContent("AssetBundlePipline");
        instance.Show();
        EditorApplication.update += EditorUpdate;
        string overrideFile = Application.dataPath + "/ResourcesRaw/UploadAssetBundleHostOverride.txt";
        if (File.Exists(overrideFile))
        {
            string overrideHost = File.ReadAllText(overrideFile);
            if (string.IsNullOrEmpty(overrideHost) == false)
            {
                uploadHost = overrideHost;
                uploadURL = uploadHost + uploadService;
            }
        }
    }

    private static void LoadSavedConfig()
    {
        autoNameExcludeExtension = EditorPrefs.GetString(epAutoNameExcludeExtension, autoNameExcludeExtension);
        //outputPathType = (OutputPathType)EditorPrefs.GetInt(epOutputPathType, (int)outputPathType);
        outputPath = EditorPrefs.GetString(epOutputPath, outputPath);
        buildTarget = (BuildTarget)EditorPrefs.GetInt(epBuildTarget, (int)buildTarget);
        autoNameStrategy = (AutoNameStrategy)EditorPrefs.GetInt(epAutoNameExcludeExtension, (int)autoNameStrategy);
        generateIniData = EditorPrefs.GetBool(epGenerateIniData, generateIniData);
        deleteManifest = EditorPrefs.GetBool(epDeleteManifest, deleteManifest);
        uploadToServer = EditorPrefs.GetBool(epUploadToServer, uploadToServer);
    }

    static void EditorUpdate()
    {
        if (isZipping)
        {
            if (downloadVersionRequets.isDone)
            {
                isZipping = false;

                DoZip(downloadVersionRequets.downloadHandler.text);
                downloadVersionRequets.Dispose();
                DoUpload();
            }
        }
    }

    /// <summary>
    /// BuildAssetBundle之后，Unity会触发一次ScriptCompile
    /// 为了避免静态变量被重置，需要等ScriptCompile结束后再开始触发打包zip，上传等逻辑
    /// </summary>
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        LoadSavedConfig();
        if (streamingAssetsPath == null)
        {
            streamingAssetsPath = Application.streamingAssetsPath;
        }
        if (editorUpdateCallback == null)
        {
            editorUpdateCallback = new EditorApplication.CallbackFunction(EditorUpdate);
            EditorApplication.update += editorUpdateCallback;
        }
    }

    void OnGUI()
    {
        if (instance == null)
        {
            instance = this;
        }
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                workingPath = GetSelectionPath();
                GUILayout.Label("工作目录：");
                GUILayout.Label(workingPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("命名策略：");
                EditorGUI.BeginChangeCheck();
                autoNameStrategy = (AutoNameStrategy)EditorGUILayout.EnumPopup(autoNameStrategy);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(epAutoNameStrategy, (int)autoNameStrategy);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("排除文件(只填扩展名，空格分隔)：");
                EditorGUI.BeginChangeCheck();
                autoNameExcludeExtension = EditorGUILayout.TextField(autoNameExcludeExtension);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(epAutoNameExcludeExtension, autoNameExcludeExtension);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("遍历设置AssetBundleName"))
                {
                    SetAssetBundleNameRecursively(workingPath);
                    AssetDatabase.RemoveUnusedAssetBundleNames();
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("目标平台：");
                EditorGUI.BeginChangeCheck();
                buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(buildTarget);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetInt(epBuildTarget, (int)buildTarget);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(string.Format("输出目录：{0}/", streamingAssetsPath));
                EditorGUI.BeginChangeCheck();
                outputPath = EditorGUILayout.TextField(outputPath);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(epOutputPath, outputPath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("是否生成assetBundleIni文件：");
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                generateIniData = EditorGUILayout.Toggle(generateIniData);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(epGenerateIniData, generateIniData);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("是否删除Manifest：");
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                deleteManifest = EditorGUILayout.Toggle(deleteManifest);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(epDeleteManifest, deleteManifest);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("是否上传服务器(若不上传，则一定在persistentDataPath保留)：");
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                uploadToServer = EditorGUILayout.Toggle(uploadToServer);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(epUploadToServer, uploadToServer);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (uploadToServer)
                {
                    GUILayout.Label("上传地址：");
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.LabelField(uploadURL);
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Build AssetBundle（异步）"))
                {
                    BuildAssetBundle();

                    if (uploadToServer)
                    {
                        if (instance != null)
                        {
                            instance.ShowNotification(new GUIContent("本地AssetBundle生成完毕，后台正在上传文件，请等待控制台输出：“AssetBundle文件上传结束：”字样后再继续"));
                        }
                        DownloadVersionInfoAsync();
                    }
                }
                if (GUILayout.Button("Build AssetBundle（同步）"))
                {
                    BuildAssetBundle();
                    if (uploadToServer)
                    {
                        string serverVersionInfo = DownloadVersionInfoSync();
                        DoZip(serverVersionInfo);
                        DoUpload();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private string GetSelectionPath()
    {
        string path = "";
        var obj = Selection.activeObject;
        if (obj == null)
        {
            path = string.Empty;
        } 
        else
        {
            GameObject go = obj as GameObject;
            if (go != null && go.scene.name != null)
            {
                path = string.Empty;
            }
            else
            {
                path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
                path = Path.GetDirectoryName(path);
            }
        }
        return path;
    }

    private static void SetAssetBundleNameRecursively(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        string dataPath = Application.dataPath.Replace("Assets", "");
        string fullPath = dataPath + path;
        DirectoryInfo dir = new DirectoryInfo(fullPath);
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
        string[] excludeExtensions = autoNameExcludeExtension.Split(' ');
        for (var i = 0; i < files.Length; ++i)
        {
            FileInfo fileInfo = files[i];
            if (CheckFileInclude(excludeExtensions, fileInfo.Name))
            {
                var assetPathInUnity = fileInfo.FullName.Substring(dataPath.Length).Replace('\\', '/');
                var importer = AssetImporter.GetAtPath(assetPathInUnity);
                switch (autoNameStrategy)
                {
                    case AutoNameStrategy.AssetPathToGUID:
                        {
                            importer.assetBundleName = AssetDatabase.AssetPathToGUID(assetPathInUnity);
                        }
                        break;
                    case AutoNameStrategy.AssetFileName:
                        {
                            importer.assetBundleName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        }
                        break;
                    case AutoNameStrategy.AssetFilePath:
                        {
                            string assetPathInAssetsFolder = assetPathInUnity.Replace("Assets/", "").Replace("/", "@");
                            assetPathInAssetsFolder = Path.GetFileNameWithoutExtension(assetPathInAssetsFolder);
                            importer.assetBundleName = assetPathInAssetsFolder;
                        }
                        break;
                    case AutoNameStrategy.AssetFolderName:
                        {
                            importer.assetBundleName = fileInfo.Directory.Name;
                        }
                        break;
                    case AutoNameStrategy.AssetFolderPath:
                        {
                            string assetFolderPathInUnity = fileInfo.DirectoryName.Substring(dataPath.Length).Replace('\\', '/');
                            string assetFolderPathInAssetsFolder = assetFolderPathInUnity.Replace("Assets/", "").Replace("/", "@");
                            importer.assetBundleName = assetFolderPathInAssetsFolder;
                        }
                        break;
                }
            }
        }
        if (instance != null)
        {
            instance.ShowNotification(new GUIContent("递归设置AssetBundleName完成"));
        }
    }

    private static bool CheckFileInclude(string[] exlucdeExtensions, string fileName)
    {
        bool include = true;
        foreach (string exlucdeExtension in exlucdeExtensions)
        {
            include &= !fileName.EndsWith(exlucdeExtension);
        }
        return include;
    }

    public static void BuildAssetBundleFromExternal()
    {
        streamingAssetsPath = Application.streamingAssetsPath;
        LoadSavedConfig();
        BuildAssetBundle();
        if (uploadToServer)
        {
            string serverVersionInfo = DownloadVersionInfoSync();
            downloadVersionRequets.Dispose();
            DoZip(serverVersionInfo);
            DoUpload();
        }
    }

    private static void BuildAssetBundle()
    {
        EditorPrefs.SetBool(epIsBuildAssetBundleFinish, false);
        
        finalOutputPath = Path.Combine(Application.streamingAssetsPath, outputPath);

        if (Directory.Exists(finalOutputPath))
        {
            Directory.Delete(finalOutputPath, true);
        }

        Directory.CreateDirectory(finalOutputPath);

        BuildPipeline.BuildAssetBundles(finalOutputPath, buildAssetBundleOption, buildTarget);

        HashUtil.ComputeVersionAndHash(finalOutputPath, "versionInfo.txt");

        if (generateIniData)
        {
            GenerateIniData();
        }

        if (deleteManifest)
        {
            DirectoryInfo dir = new DirectoryInfo(finalOutputPath);
            FileInfo[] files = dir.GetFiles("*.manifest.*", SearchOption.AllDirectories);
            {
                for (var i = 0; i < files.Length; ++i)
                {
                    FileInfo fileInfo = files[i];
                    fileInfo.Delete();
                }
            }
        }
    }

    private static void GenerateIniData()
    {
        StringBuilder sb = new StringBuilder(1000);
        string dataPath = Application.dataPath.Replace("Assets", "");
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath);
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
        string[] excludeExtensions = autoNameExcludeExtension.Split(' ');
        for (var i = 0; i < files.Length; ++i)
        {
            FileInfo fileInfo = files[i];
            if (CheckFileInclude(excludeExtensions, fileInfo.Name))
            {
                var basePath = fileInfo.FullName.Substring(dataPath.Length).Replace('\\', '/');
                var importer = AssetImporter.GetAtPath(basePath);
                if (importer != null)
                {
                    if (string.IsNullOrEmpty(importer.assetBundleName) == false)
                    {
                        string assetName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        sb.AppendLine(string.Format("{0}|{1}|{2}", assetName, fileInfo.Name, importer.assetBundleName));
                    }
                }
            }
        }
        string outputFile = Path.Combine(Application.dataPath, "Resources/assetBundleIni.txt");
        File.WriteAllText(outputFile, sb.ToString());
    }

    private static string DownloadVersionInfoSync()
    {
        UnityEngine.Debug.Log("AssetBundlePipeline 开始压缩AssetBundle");

        string platform = GetPlatform();
        string url = string.Format("{0}park-of-monster/{1}/AssetBundle/versionInfo.txt", uploadHost, platform);
        //压缩文件的时候先从服务端下载versionInfo.txt与本地新生成的AssetBundle对比，只将发生变化的文件打到zip文件中
        downloadVersionRequets = UnityWebRequest.Get(url);
        isZipping = true;
        downloadVersionRequets.Send();
        while(downloadVersionRequets.isDone == false)
        {
            Thread.Sleep(100);
        }
        isZipping = false;
        return downloadVersionRequets.downloadHandler.text;
    }

    private static void DownloadVersionInfoAsync()
    {
        UnityEngine.Debug.Log("AssetBundlePipeline 开始压缩AssetBundle");
        
        string platform = GetPlatform();
        string url = string.Format("{0}park-of-monster/{1}/AssetBundle/versionInfo.txt", uploadHost, platform);
        //压缩文件的时候先从服务端下载versionInfo.txt与本地新生成的AssetBundle对比，只将发生变化的文件打到zip文件中
        downloadVersionRequets = UnityWebRequest.Get(url);
        isZipping = true;
        downloadVersionRequets.Send();
    }

    private static void DoZip(string serverInfoText)
    {
        List<string> zipFileList = new List<string>();
        zipFileList.Add("versionInfo.txt");

        //string serverVersion;
        //Dictionary<string, ResUpdateMgr.UpdateFileInfo> serverFileHashInfo;
        //ResUpdateMgr.ParseVersionInfoByContent(serverInfoText, out serverVersion, out serverFileHashInfo);

        //string streamingAssetsVersionFile = Path.Combine(streamingAssetsPath, "AssetBundle/versionInfo.txt");
        //string localVersion;
        //Dictionary<string, ResUpdateMgr.UpdateFileInfo> localFileHashInfo;
        //ResUpdateMgr.ParseVersionInfoByPath(streamingAssetsVersionFile, out localVersion, out localFileHashInfo);

        //foreach (KeyValuePair<string, ResUpdateMgr.UpdateFileInfo> kvp in serverFileHashInfo)
        //{
        //    if (localFileHashInfo.ContainsKey(kvp.Key))
        //    {
        //        var localHashInfo = localFileHashInfo[kvp.Key];
        //        if (localHashInfo.fileHash != kvp.Value.fileHash)
        //        {
        //            zipFileList.Add(kvp.Key);
        //        }
        //    }
        //}

        //foreach (KeyValuePair<string, ResUpdateMgr.UpdateFileInfo> kvp in localFileHashInfo)
        //{
        //    if (serverFileHashInfo.ContainsKey(kvp.Key) == false)
        //    {
        //        zipFileList.Add(kvp.Key);
        //    }
        //}
        string zipFiles = string.Join("$;", zipFileList.ToArray()).Replace(".", @"\.") + "$";

        UnityEngine.Debug.Log("以下文件将被打包上传服务端：" + zipFiles);

        FastZip fastZip = new FastZip();

        fastZip.CreateZip(zipFile, streamingAssetsPath, true, zipFiles, "AssetBundle");
        UnityEngine.Debug.Log("AssetBundlePipeline 压缩AssetBundle完成");
    }

    private static void DoUpload()
    {
        WebClient webClient = new WebClient();
        webClient.Headers.Add("Content-Type", "binary/octet-stream");

        string platform = GetPlatform();

        UnityEngine.Debug.Log("AssetBundlePipeline 开始上传AssetBundle");
        webClient.UploadFileAsync(new Uri(uploadURL + "?platform=" + platform), "POST", zipFile);
        webClient.UploadFileCompleted += WebClient_UploadFileCompleted;
        webClient.UploadProgressChanged += WebClient_UploadProgressChanged;
    }

    private static string GetPlatform()
    {
        string platform = "unknow";
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.WindowsEditor ||
            buildTarget == BuildTarget.Android)
        {
            platform = "Android";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXEditor ||
            buildTarget == BuildTarget.iOS)
        {
            platform = "iOS";

            //为了优化A8CPU以下ios机型的内存使用，为它们生成专门的AssetBundle，放在iOS_PVRTC目录下
            //判断方式为，如果热更新图集的第一个的压缩格式为PVRTC，则应该上传到iOS_PVRTC目录下
            TextureImporter textureImporter = AssetImporter.GetAtPath("Assets/ResourcesRaw/Atlas/AllActivity.png") as TextureImporter;
            if (textureImporter != null)
            {
                TextureImporterPlatformSettings settings = textureImporter.GetPlatformTextureSettings("iPhone");
                if (settings.format == TextureImporterFormat.PVRTC_RGBA4)
                {
                    platform = "iOS_PVRTC";
                }
            }
            else
            {
                Debug.Log("Assets/ResourcesRaw/Atlas/AllActivity.png 未正确导入Unity");
            }
        }
        //Debug.Log("RYH", "AssetBundlePipeline", "Platform is : " + platform);
        return platform;
    }

    //这个上传进度事件目前不会被触发，原因未知
    private static void WebClient_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
    {
        float percent = e.BytesSent * 1.0f / e.TotalBytesToSend;
        UnityEngine.Debug.Log(string.Format("AssetBundle文件上传进度：{0}%", percent * 100));
        EditorUtility.DisplayProgressBar("AssetBundlePipeline", "正在上传AssetBundle 请耐心等待......", percent);
    }

    private static void WebClient_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
    {
        UnityEngine.Debug.Log("WebClient_UploadFileComplete");
        if (e.Result != null)
        {
            string s = System.Text.Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
            UnityEngine.Debug.Log("AssetBundle文件上传结束：" + s);
        }
        File.Delete(zipFile);
    }

    public static void SetAssetBundleName(string path)
    {
        workingPath = path;
        autoNameStrategy = AutoNameStrategy.AssetFileName;
        SetAssetBundleNameRecursively(workingPath);
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }
}

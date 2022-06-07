using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
//using Spine.Unity;
//using Spine;
using System.Text;
using System.Linq;

public class AppcpiMenuTool
{
    [MenuItem("Appcpi/快捷方式/打开PersistentDataPath")]
    public static void OpenPersistentDataPath()
    {
#if UNITY_EDITOR_OSX
        string path = string.Format("\"{0}\"", Application.persistentDataPath);
        System.Diagnostics.Process.Start("open", path);
#else
        Application.OpenURL(Application.persistentDataPath);
#endif
    }

    [MenuItem("Appcpi/快捷方式/打开StreamingAssetsPath")]
    public static void OpenStreamingAssetsPath()
    {
#if UNITY_EDITOR_OSX
        string path = string.Format("\"{0}\"", Application.streamingAssetsPath);
        System.Diagnostics.Process.Start("open", path);
#else
        Application.OpenURL(Application.streamingAssetsPath);
#endif
    }

    [MenuItem("Appcpi/快捷方式/打开UnityEditor日志目录")]
    public static void OpenUnityEditorPath()
    {
#if UNITY_EDITOR_OSX
        string path = "\"/Users/appcpi/Library/Logs/Unity\"";
        System.Diagnostics.Process.Start("open", path);
#else
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Unity/Editor";
        Application.OpenURL(path);
#endif
    }

    [MenuItem("Appcpi/测试工具/三清（清Cache,清PlayerPrefs,清sqlite数据库")]
    public static void Clear3Data()
    {
        string content = "加这个二次确认是为了防止误点";
        bool delete = EditorUtility.DisplayDialog("三思而后行", content, "三清", "放弃");
        if (delete)
        {
            try
            {
                string cacheFolder = Application.persistentDataPath + "/Cache";
                if (Directory.Exists(cacheFolder))
                {
                    Directory.Delete(cacheFolder, true);
                }
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();

                string[] dbFiles = Directory.GetFiles(Application.persistentDataPath, "*.db", SearchOption.TopDirectoryOnly);
                foreach (string dbFile in dbFiles)
                {
                    File.Delete(dbFile);
                }
                EditorUtility.DisplayDialog("三清", "完成", "OK");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("三清", "出现错误，请手动处理或呼叫客户端：\n" + ex.ToString(), "OK");
            }
        }
    }

    [MenuItem("Appcpi/测试工具/清除AssetBundles")]
    public static void ClearAssetBundles()
    {
        try
        {
            string assetBundleFolder = Application.persistentDataPath + "/AssetBundle";
            if (Directory.Exists(assetBundleFolder))
            {
                Directory.Delete(assetBundleFolder, true);
            }
            EditorUtility.DisplayDialog("清除AssetBundles", "完成", "OK");
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("清除AssetBundles", "出现错误，请手动处理或呼叫客户端：\n" + ex.ToString(), "OK");
        }
    }

    [MenuItem("Appcpi/测试工具/清除UnityEditor日志")]
    public static void ClearUnityEditorLog()
    {
        string logFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/Unity/Editor";
        string[] logFiles = Directory.GetFiles(logFilePath, "Editor_*.log", SearchOption.TopDirectoryOnly);
        double totalSizeByte = 0;

        foreach (string logFile in logFiles)
        {
            FileInfo fileInfo = new FileInfo(logFile);
            totalSizeByte += fileInfo.Length;
        }

        int sizeUnitIndex = 0;
        string[] sizeUnit = {"Bytes", "KB", "MB", "GB", "TB"};
        while (totalSizeByte > 1024)
        {
            totalSizeByte /= 1024;
            sizeUnitIndex++;
        }

        string content = string.Format("确定要删除所有日志嘛？\n(若非硬盘空间不够或者测试，请不要随意删除)\n当前日志总大小：{0:F2}{1}", totalSizeByte, sizeUnit[sizeUnitIndex]);
        bool delete = EditorUtility.DisplayDialog("三思而后行", content, "确定要删", "还是不了");
        if (delete)
        {
            try
            {
                foreach (string logFile in logFiles)
                {
                    File.Delete(logFile);
                }
                EditorUtility.DisplayDialog("清除UnityEditor日志", "完成", "OK");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("清除UnityEditor日志", "出现错误，请手动处理或呼叫客户端：\n" + ex.ToString(), "OK");
            }
        }
    }

    [MenuItem("Appcpi/测试工具/测试移动端数据加载流程")]
    public static void TestMobileLoadDataFlow()
    {
        ResourcesPostprocessor.enabled = false;
        GameBuildProcess.PrepareForBuild();
        ResourcesPostprocessor.enabled = true;
        EditorUtility.DisplayDialog("测试移动端数据加载流程", "完成", "OK");
    }

    [MenuItem("Appcpi/测试工具/还原测试移动端数据加载流程")]
    public static void RevertTestMobileLoadDataFlow()
    {
        ResourcesPostprocessor.enabled = false;
        GameBuildProcess.RevertBuildModification();
        ResourcesPostprocessor.enabled = true;
        EditorUtility.DisplayDialog("还原测试移动端数据加载流程", "完成", "OK");
    }

    [MenuItem("Appcpi/测试工具/导出生物动画攻击动作时间")]
    public static void ExportSpineEventTime()
    {
        StringBuilder sb = new StringBuilder();
        string[] spineFiles = Directory.GetFiles(Application.dataPath + "/ResourcesRaw/Spine/Creatures", "*.asset");
        foreach(string spineFile in spineFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(spineFile);
            int id = 0;
            int.TryParse(fileName.Substring(0, 5), out id);
            if (id > 0)
            {
                string path = spineFile.Replace(Application.dataPath, "");
                path = "Assets" + path;

                //SkeletonDataAsset spineAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(path);
                //SkeletonData skeletonData = spineAsset.GetSkeletonData(true);
                //var animation = skeletonData.FindAnimation("attack");

                //foreach (var timeline in animation.Timelines)
                //{
                    //var eventTimeline = timeline as Spine.EventTimeline;
                    //if (eventTimeline != null)
                    //{
                    //    foreach (var spineEvent in eventTimeline.Events)
                    //    {
                    //        if (spineEvent.Data.Name.Equals("shoot"))
                    //        {
                    //            sb.Append(id);
                    //            sb.Append(",");
                    //            sb.Append(spineEvent.Time.ToString("F3"));
                    //            sb.AppendLine();
                    //        }
                    //    }
                    //}
                //}
            }
        }
        File.WriteAllText("./SpineEventTime.txt", sb.ToString());
    }

    [MenuItem("Appcpi/数据生成/一键更新（按顺序执行1，2，3）", false, 0)]
    public static void GenerateAllDataBeforeBuild()
    {
        SpriteAtlasPipeline.GeneratSpriteAtlasIni();

        Excel2ScriptableObjectPipeline.BuildTableData();

        AssetBundlePipeline.BuildAssetBundleFromExternal();
    }
}

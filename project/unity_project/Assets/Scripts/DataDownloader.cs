using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class MapDataCache
{
    public int width;
    public int height;
    public List<MapGridGameData> playerDataList;

    public EditState state;
}
/// <summary>
/// 状态切换流程
/// 1.美术绘制 本地保存  Drawer
/// 2.美术发布 Drawer ==》 Publish
/// 3.产品绘制 本地保存 Publish
/// 4.产品发布 Publish==>Publish
/// </summary>
public enum EditState
{
    Drawer, //地图绘制状态，美术可随意绘制地图格子数据
    Publish//发布状态，一经发布,美术无法修改该数据，产品只能对MapGridGameData中TerrainState中状态为Locked的地块进行修改
}
public class DataDownloader : MonoBehaviour
{
    /// <summary>
    /// 数据使用说明 
    /// 0.MapDataCache整张地图的数据，MapGridGameData单个地图格子数据
    /// 1.数据结构使用相同的结构 MapDataCache，只是其中字段的数值会不同，主要区分在EditState 及MapGridGameData中的TerrainState。
    /// 2.EditState见数据定义；
    /// 3.TerrainState见数据定义
    ///
    /// 整体数据流程
    /// 0.设置版本号，编辑者身份（美术or产品）进入地图编辑器
    /// 1.根据身份和版本号确定数据来源（读取本地还是从远端拉取）
    /// 2.编辑完成后,根据身份和版本号保存数据
    ///
    /// 
    /// 
    /// 从远端拉去数据流程，
    /// 1.根据填写的版本号，拉取服务器数据与本地数据比对，远端数据根据索引比对覆盖本地数据，这里只改变playerDataList的数据
    /// 2.根据产品所填版本号进行保存 PS：这里要注意版本号是产品手动填写，属于进行版本迭代，版本号不能错误
    /// </summary>
    /// 

    private static string url = "http://127.0.0.1:8080/AssetBundle/";
    // Start is called before the first frame update
    public static IEnumerator LoadData(string path, System.Action<string> callback)
    {
        string result=string.Empty;

        UnityWebRequest www =  UnityWebRequest.Get(url + path);

        yield return www.SendWebRequest();
        string loadFrom = "server";
        if ( www.isDone)
        {
            if(string.IsNullOrEmpty(www.error))
            {
                result = www.downloadHandler.text;
            }
            else
            {
                Debug.Log("error:" + www.error);
            }
        }

        if (string.IsNullOrEmpty(result))
        {
            path = Path.Combine(Application.persistentDataPath, path);
            result = ReadConfiCache(path);
            loadFrom = "local";
        }
        Debug.Log("loadFrom:" + loadFrom);

        if (callback != null)
        {
            callback(result);
        }
    }

    public static IEnumerator SaveData(string path, string data, System.Action callback, bool toLocal = true)
    {
        if (toLocal)
        {
            path = Path.Combine(Application.persistentDataPath, path);
            WriteConfigCache(path, data);
            yield return null;
        }
        else
        {
            //todo:发布到远端
            UnityWebRequest www = new UnityWebRequest(url + path);
            yield return www.SendWebRequest();
            if (www.isDone)
            {
                if (string.IsNullOrEmpty(www.error))
                {
                }
                else
                {
                    Debug.Log("error:" + www.error);
                }
            }
        }

        Debug.Log(path + "-----save:" + data);

        if (callback != null)
        {
            callback();
        }
    }

     static void CheckDirectory(string path)
    {
        string directoryName = Path.GetDirectoryName(path);
        if (directoryName != null && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
    }

     static void WriteConfigCache(string configPath, string data)
    {
        CheckDirectory(configPath);
        File.WriteAllText(configPath, data);
    }

    static string ReadConfiCache(string configPath)
    {
        if (!File.Exists(configPath))
        {
            return string.Empty;
        }
            return File.ReadAllText(configPath);
    }
}

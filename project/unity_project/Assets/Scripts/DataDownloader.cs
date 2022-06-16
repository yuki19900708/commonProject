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
/// ״̬�л�����
/// 1.�������� ���ر���  Drawer
/// 2.�������� Drawer ==�� Publish
/// 3.��Ʒ���� ���ر��� Publish
/// 4.��Ʒ���� Publish==>Publish
/// </summary>
public enum EditState
{
    Drawer, //��ͼ����״̬��������������Ƶ�ͼ��������
    Publish//����״̬��һ������,�����޷��޸ĸ����ݣ���Ʒֻ�ܶ�MapGridGameData��TerrainState��״̬ΪLocked�ĵؿ�����޸�
}
public class DataDownloader : MonoBehaviour
{
    /// <summary>
    /// ����ʹ��˵�� 
    /// 0.MapDataCache���ŵ�ͼ�����ݣ�MapGridGameData������ͼ��������
    /// 1.���ݽṹʹ����ͬ�Ľṹ MapDataCache��ֻ�������ֶε���ֵ�᲻ͬ����Ҫ������EditState ��MapGridGameData�е�TerrainState��
    /// 2.EditState�����ݶ��壻
    /// 3.TerrainState�����ݶ���
    ///
    /// ������������
    /// 0.���ð汾�ţ��༭����ݣ�����or��Ʒ�������ͼ�༭��
    /// 1.������ݺͰ汾��ȷ��������Դ����ȡ���ػ��Ǵ�Զ����ȡ��
    /// 2.�༭��ɺ�,������ݺͰ汾�ű�������
    ///
    /// 
    /// 
    /// ��Զ����ȥ�������̣�
    /// 1.������д�İ汾�ţ���ȡ�����������뱾�����ݱȶԣ�Զ�����ݸ��������ȶԸ��Ǳ������ݣ�����ֻ�ı�playerDataList������
    /// 2.���ݲ�Ʒ����汾�Ž��б��� PS������Ҫע��汾���ǲ�Ʒ�ֶ���д�����ڽ��а汾�������汾�Ų��ܴ���
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
            //todo:������Զ��
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

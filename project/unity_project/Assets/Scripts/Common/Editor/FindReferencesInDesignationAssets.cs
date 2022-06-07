using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class FindReferencesInDesignationAssets
{
    public static List<DataResPath> PlayerPlayerPrefsData = new List<DataResPath>();
    public static string filePath = Application.dataPath + "/Resources/Guide/FindReferencesInDesignationAssets";
    //鼠标右键点击组件出现，并进行操作
    [MenuItem("Assets/FindReferencesInDesignationAssets")]   //CONTEXT（大写） 组件名  按钮名
    static void OpenChangeParticleLayerWindow(MenuCommand cmd)
    {

        FileInfo file = new FileInfo(filePath);   //这里是重点,会在下面细说的,这里只需要知道它只是一个路径
        string jsonData = "";
        if (file.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            jsonData = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            PlayerPlayerPrefsData = SimpleJson.SimpleJson.DeserializeObject<List<DataResPath>>(jsonData);
            FindReferencesInDesignationWindow.bothData = AddAllPath(Application.dataPath);
        }
        else
        {
            FindReferencesInDesignationWindow.bothData = AddAllPath(Application.dataPath);
        }
        EditorWindow.GetWindow(typeof(FindReferencesInDesignationWindow));
    }
    static List<DataResPath> AddAllPath(string path)
    {
        List<DataResPath> allData = new List<DataResPath>();

        DirectoryInfo root = new DirectoryInfo(path);
        foreach (DirectoryInfo d in root.GetDirectories())
        {
            DataResPath oneData = new DataResPath();
            oneData.path = d.Name;
            oneData.fullpath = path + "/" + d.Name;
            oneData.flag = true;
            oneData.data = new List<DataResPath>();
            oneData.data = AddAllPath(oneData.fullpath);
            //Debug.Log(oneData.data.Count);
            allData.Add(oneData);
        }

        FileInfo file = new FileInfo(filePath);   //这里是重点,会在下面细说的,这里只需要知道它只是一个路径
        if (file.Exists)
        {
            InitialValue(allData, PlayerPlayerPrefsData);
        }

        return allData;
    }

    void LoadFile()
    {
        FileInfo file = new FileInfo(filePath);   //这里是重点,会在下面细说的,这里只需要知道它只是一个路径
        string jsonData = "";
        if (file.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            jsonData = reader.ReadToEnd();
            reader.Close();
            reader.Dispose();
            PlayerPlayerPrefsData = SimpleJson.SimpleJson.DeserializeObject<List<DataResPath>>(jsonData);
        }
    }

    static void InitialValue(List<DataResPath> newData, List<DataResPath> oldData)
    {
        for (int i = 0; i < newData.Count; i++)
        {
            for (int j = 0; j < oldData.Count; j++)
            {
                if (newData[i].fullpath == oldData[j].fullpath)
                {
                    newData[i].show = false;
                    newData[i].flag = oldData[j].flag;
                    InitialValue(newData[i].data, oldData[j].data);
                }
            }
        }
    }
}

public class FindReferencesInDesignationWindow : EditorWindow
{
    public static List<DataResPath> bothData = new List<DataResPath>();
    public static List<Object> allObjectList = new List<Object>();
    private static int startIndex=0;
    private static string[] files;
    private static List<string> matchList;
    private static string path;
    private static int instanceId;


    Vector3 scrollPos = Vector2.zero;
    void OnGUI()
    {
        if (GUILayout.Button("选择全部"))
        {
            for (int i = 0; i < bothData.Count; i++)
            {
                bothData[i].flag = true;
                bothData[i].show = false;
            }
        }

        scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height),
            scrollPos, new Rect(0, 0, 600, 10000));
        for (int i = 0; i < bothData.Count;i++)
        {
            DrawWindow(bothData[i]);
        }
        //创建一个按钮
        if (GUILayout.Button("寻找指定路径索引"))
        {
            string activityDataString = SimpleJson.SimpleJson.SerializeObject(bothData);
            FileInfo file = new FileInfo(FindReferencesInDesignationAssets.filePath);
            StreamWriter writer = file.CreateText();   //用文本写入的方式
            writer.Write(activityDataString);   //写入数据
            writer.Close();   //关闭写指针
            writer.Dispose();    //销毁写指针
            Find();
        }
        GUI.EndScrollView();
    }

    void DrawWindow(DataResPath _Data,float offset=0)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.MaxWidth(offset+5));
        _Data.flag = EditorGUILayout.Toggle(_Data.path, _Data.flag);
        GUILayout.EndHorizontal();
        if (_Data.flag)
        {
            if (_Data.data.Count > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.MaxWidth(offset+10));
                _Data.show = EditorGUILayout.Foldout(_Data.show, "",true);
                GUILayout.EndHorizontal();
                if (_Data.show)
                {
                    for (int i = 0; i < _Data.data.Count; i++)
                    {
                        DrawWindow(_Data.data[i], offset+10);
                    }
                }
            }
        }
    }
    static List<string>  bothFindFile = new List<string>();
    public static void Find()
    {
        instanceId = Selection.activeObject.GetInstanceID();
        path = AssetDatabase.GetAssetPath(Selection.activeObject);
        bothFindFile = new List<string>();
        #region 匹配当前Assets下指定的文件
        for (int i = 0; i < bothData.Count; i++)
        {
            FindPathString(bothData[i]);
        }

        matchList = new List<string>();

        for (int i = 0; i < bothFindFile.Count; i++)
        {
            Debug.Log(bothFindFile[i]);
            files = Directory.GetFiles(bothFindFile[i], "*.*", SearchOption.TopDirectoryOnly);
            for (int j = 0; j < files.Length; j++)
            {
                if (files[j].EndsWith(".prefab")
                    || files[j].EndsWith(".unity")
                    || files[j].EndsWith(".mat")
                    || files[j].EndsWith(".asset")
                    || files[j].EndsWith(".anim")
                    || files[j].EndsWith(".controller")
                    )
                {
                    matchList.Add(files[j]);
                }
            }
        }

        if (matchList.Count > 0)
        {
            Debug.LogFormat("寻找{0}引用:", Selection.activeObject.name);
            EditorApplication.update += Event_FindReferences;
        }
        else
        {
            Debug.LogFormat("为选择文件夹");
        }
        #endregion
    }

    static void FindPathString(DataResPath data)
    {
         if (data.flag)
        {
            bothFindFile.Add(data.fullpath);
            for (int i = 0; i < data.data.Count; i++)
            {
                if (data.data[i].flag == true)
                {
                    FindPathString(data.data[i]);
                }
            }
        }

    }

    static void Event_FindReferences()
    {

        string findPath = GetRelativeAssetsPath(matchList[startIndex]);
        bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", findPath, (float)startIndex / (float)files.Length);
        Object targetObject = AssetDatabase.LoadAssetAtPath<Object>(findPath);
        //移除自身的路径
        //Debug.Log(findPath);
        if (findPath != path)
        {
            Object[] roots = new Object[] { targetObject };
            Object[] objs = EditorUtility.CollectDependencies(roots);
            foreach (Object obj in objs)
            {
                //如果他们的引用ID相等那么记录他们
                if (obj != null)
                {
                    if (obj.GetInstanceID() == instanceId)
                    {
                        //Debug.Log("<color=green>"+findPath+"</color>");
                        //?这个打不出来
                        UnityEngine.Debug.Log("<color=green>" + findPath + "</color>", AssetDatabase.LoadAssetAtPath<Object>(findPath));
                        allObjectList.Add(AssetDatabase.LoadAssetAtPath<Object>(findPath));
                    }
                }
            }
        }

        startIndex++;
        if (isCancel || startIndex >= matchList.Count)
        {
            EditorUtility.ClearProgressBar();
            startIndex = 0;
            Debug.Log("匹配结束");
            Object[] toSelection = new Object[allObjectList.Count];
            for (int i = 0; i < allObjectList.Count; i++)
            {
                toSelection[i] = allObjectList[i];
            }
            Selection.objects = toSelection;
            EditorApplication.update -= Event_FindReferences;
        }
    }

    static private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }

}

public class DataResPath
{
    public bool flag = false;
    public bool show = false;
    public string path = "";
    public string fullpath = "";
    public List<DataResPath> data;
}
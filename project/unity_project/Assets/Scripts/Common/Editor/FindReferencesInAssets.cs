using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
/// <summary>
/// 查找Assets中的引用
/// </summary>
public class FindReferencesInAssets : EditorWindow
{
    public static List<Object> allObjectList = new List<Object>();
    private static int startIndex;
    private static string[] files;
    private static List<string> matchList;
    private static string path;
    private static int instanceId;

    [MenuItem("Assets/FindReferencesInAssets &Q")]
    public static void Find()
    {
        instanceId = Selection.activeObject.GetInstanceID();
        path = AssetDatabase.GetAssetPath(Selection.activeObject);
        #region 匹配当前Assets下指定的文件
        files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        matchList = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".prefab") 
                || files[i].EndsWith(".unity")
                || files[i].EndsWith(".mat")
                || files[i].EndsWith(".asset")
                || files[i].EndsWith(".anim")
                || files[i].EndsWith(".controller")
                )
            {
                matchList.Add(files[i]);
            }
        }
        Debug.LogFormat("寻找{0}引用:", Selection.activeObject.name);
        EditorApplication.update += Event_FindReferences;
        #endregion
    }

    [MenuItem("Assets/FindVFXReferencesInAssets %E")]
    public static void FindVFXReference()
    {
        instanceId = Selection.activeObject.GetInstanceID();
        path = AssetDatabase.GetAssetPath(Selection.activeObject);
        #region 匹配当前Assets下指定的文件
        matchList = new List<string>();
        files = Directory.GetFiles(Application.dataPath + "/Resources/Prefabs/VFX", "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".prefab")
                || files[i].EndsWith(".unity")
                || files[i].EndsWith(".mat")
                || files[i].EndsWith(".asset")
                || files[i].EndsWith(".anim")
                || files[i].EndsWith(".controller")
                )
            {
                matchList.Add(files[i]);
            }
        }
        files = Directory.GetFiles(Application.dataPath + "/ResourcesRaw/VFX", "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".prefab")
                || files[i].EndsWith(".unity")
                || files[i].EndsWith(".mat")
                || files[i].EndsWith(".asset")
                || files[i].EndsWith(".anim")
                || files[i].EndsWith(".controller")
                )
            {
                matchList.Add(files[i]);
            }
        }
        Debug.LogFormat("寻找{0}引用:", Selection.activeObject.name);
        EditorApplication.update += Event_FindReferences;
        #endregion
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

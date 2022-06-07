using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 查找场景中的引用
/// </summary>
public class FindReferencesInCurrentSence : EditorWindow
{

    public static int instanceId;
    public static string targetGuid;
    public static List<GameObject> searchResults = new List<GameObject>();

    [MenuItem("Assets/FindReferencesInCurrentSence %Q")]
    public static void FindInCurrentSence()
    {
        objList.Clear();
        FindSenceGameObject();
        FindReferencesInSence();
    }

    static void FindReferencesInSence()
    {
        instanceId = 0; ;
        targetGuid = "";
        searchResults.Clear();

        instanceId = Selection.activeObject.GetInstanceID();
        Type target = Selection.activeObject.GetType();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        targetGuid = AssetDatabase.AssetPathToGUID(path);
        //Debug.Log(Selection.activeObject.GetFileID());
        //Debug.Log(target + targetGuid);
        // return;
        if (target == typeof(GameObject))
        {
            Debug.Log("寻找Predef:");
            ///预设在一个游戏物体中 读取出instanceId与自身的instanceId无法匹配
            ///通过判断是否为预设的引用来处理
            #region 处理预设
            for (int i = 0; i < objList.Count; i++)
            {
                //判断GameObject是否为一个Prefab的引用
                if (PrefabUtility.GetPrefabType(objList[i]) == PrefabType.PrefabInstance)
                {
                    UnityEngine.Object parentObject = PrefabUtility.GetPrefabParent(objList[i]);
                    if (AssetDatabase.GetAssetPath(parentObject) == path)
                    {
                        GameObject tagertGameObejct = PrefabUtility.FindPrefabRoot((GameObject)objList[i]);
                        if (searchResults.Contains(tagertGameObejct) == false)
                        {
                            searchResults.Add(tagertGameObejct);
                            //输出场景名，以及Prefab引用的路径
                            string sencePath = "";
                            GetNodePath(((GameObject)objList[i]).transform, ref sencePath);
                            Debug.LogFormat("场景中的路径:<color=green>{0}</color>\n\r", sencePath);
                        }
                    }
                }
            }
            Selection.objects = searchResults.ToArray();
            #endregion
        }
        else 
        {
            Debug.Log("寻找"+ Selection.activeObject.GetType().Name);
            for (int i = 0; i < objList.Count; i++)
            {
                FindRefWithGameObject((GameObject)objList[i]);
            }
        }
    }

    /// <summary>
    /// 寻找GameObject下的所有组件信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public static void FindRefWithGameObject(GameObject obj) 
    {
        var coms = obj.GetComponents<Component>();
        foreach (var com in coms)
        {
            if (com != null)
            {
               // Debug.Log("****" + com.name);
                var so = new SerializedObject(com);
                FindRef(so, obj);
            }
        }
    }

    /// <summary>
    /// 查找与当前选择相同的引用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="gameObject"></param>
    public static void FindRef(SerializedObject obj, GameObject gameObject)
    {
        var prop = obj.GetIterator();
        while (prop.Next(true))
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                var value = prop.objectReferenceValue;
                if (value != null)
                {
                    //  Debug.Log(value);
                }
                if (value != null&&value.GetInstanceID()== instanceId)
                {
                    string path = AssetDatabase.GetAssetPath(value);
                    targetGuid = AssetDatabase.AssetPathToGUID(path);
                    //Debug.Log(value.GetInstanceID() + targetGuid);
                    //Debug.Log(value.GetFileID());
                    //Debug.Log(prop.propertyPath + ":" + path + "Name:" + gameObject.name);
                    searchResults.Add(gameObject);
                    string sencePath = "";
                    GetNodePath(gameObject.transform, ref sencePath);
                    Debug.LogFormat("场景中的路径:<color=green>{0}</color>\n\r", sencePath);
                }
            }
        }
        Selection.objects = searchResults.ToArray();
    }


    #region 获取当前场景中的所有游戏物体
    /// <summary>
    /// 保存场景中的所有游戏物体的列表
    /// </summary>
    static List<UnityEngine.Object> objList = new List<UnityEngine.Object>();
    public static void FindSenceGameObject()
    {
        GameObject[] allGameObjectArray = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < allGameObjectArray.Length; i++)
        {
            objList.Add(allGameObjectArray[i]);
            RecursiveAllObjs(allGameObjectArray[i].transform);
        }
    }


    public static void RecursiveAllObjs(Transform parent)//参数：根节点物体的transform
    {
        for (int i = 0; i < parent.childCount; i++) //childCount的数量包括不显示的物体
        {
            Transform temp = parent.GetChild(i);
            objList.Add(temp.gameObject);
            if (temp.childCount > 0)
            {
                RecursiveAllObjs(temp);
            }
        }
    }
    #endregion

    #region 获取物体路径
    static void GetNodePath(Transform trans, ref string path)
    {
        if (path == "")
        {
            path = trans.name;
        }
        else
        {
            path = trans.name + "/" + path;
        }

        if (trans.parent != null)
        {
            GetNodePath(trans.parent, ref path);
        }
    }
    #endregion
}

public static class ObjectExp
{
    private static PropertyInfo inspectorMode = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
    public static long GetFileID(this UnityEngine.Object target)
    {
        SerializedObject serializedObject = new SerializedObject(target);
        inspectorMode.SetValue(serializedObject, InspectorMode.Debug, null);
        SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");
        return localIdProp.longValue;
    }
}

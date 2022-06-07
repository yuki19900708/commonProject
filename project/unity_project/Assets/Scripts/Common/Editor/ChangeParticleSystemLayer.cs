
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEditorInternal;

public class ChangeParticleSystemLayer
{
    //鼠标右键点击组件出现，并进行操作
    [MenuItem("CONTEXT/ParticleSystem/改变粒子层级")]    //CONTEXT（大写） 组件名  按钮名
    static void OpenChangeParticleLayerWindow(MenuCommand cmd)
    {
        EditorWindow.GetWindow(typeof(ChangeParticleLayerWindow));
    }
}

public class ChangeParticleLayerWindow : EditorWindow
{
    int index = 0;
    int orderIndex;

    void OnGUI()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        string[] data = (string[])sortingLayersProperty.GetValue(null, new object[0]);
        index = EditorGUILayout.Popup(index, data);
        int.TryParse(EditorGUILayout.TextField("SortOrder", orderIndex.ToString()), out orderIndex);
        //创建一个按钮
        if (GUI.Button(new Rect(20, 40, 300, 50), "改变层级(保留Sorting layer相互之间的层级关系)"))
        {
            string layerName = "";
            int order = 0;
            Dictionary<GameObject, int> layerDic = new Dictionary<GameObject, int>();
            if (Selection.transforms.Length <= 0)
            {
                return;
            }
            layerName = data[index];
            order = orderIndex;
            int litValue = 10000;
            bool layerDifferent = false;
            foreach (Transform selectedObj in Selection.transforms)
            {
                ParticleSystem[] particleSystem = selectedObj.gameObject.GetComponentsInChildren<ParticleSystem>();
                Debug.Log(particleSystem.Length);
                //判断所有的子类Layer是否拥有相同的SortLayer
                for (int i = 0; i < particleSystem.Length; i++)
                {
                    Renderer tagert = particleSystem[i].GetComponent<Renderer>();
                    if (tagert.sortingLayerName != layerName)
                    {
                        layerDifferent = true;
                    }
                    //记录的SortOrder
                    if (tagert.sortingOrder < litValue)
                    {
                        litValue = tagert.sortingOrder;
                    }
                    layerDic.Add(tagert.gameObject, tagert.sortingOrder);
                }

                int delValue = 0;
                delValue = order - litValue;

                if (layerDifferent)
                {
                    Debug.LogWarning("子类粒子与父类sortingLayer不同相同的情况 Name:");
                }

                foreach (GameObject key in layerDic.Keys)
                {
                    Renderer tagert = key.GetComponent<Renderer>();
                    tagert.sortingLayerName = layerName;
                    tagert.sortingOrder = layerDic[key] + delValue;
                }

            }
        }

        //创建一个按钮
        if (GUI.Button(new Rect(20, 100, 300, 50), "改变层级(替换所有SortOrder为当前值)"))
        {
            string layerName = "";
            int order = 0;
            Debug.Log(data[index]);
            layerName = data[index];
            order = orderIndex;
            foreach (Transform selectedObj in Selection.transforms)
            {
                ParticleSystem[] particleSystem = selectedObj.gameObject.GetComponentsInChildren<ParticleSystem>();
                //判断所有的子类Layer是否拥有相同的SortLayer
                for (int i = 0; i < particleSystem.Length; i++)
                {
                    Renderer tagert = particleSystem[i].GetComponent<Renderer>();
                    if (tagert.sortingLayerName != layerName)
                    {
                        Debug.LogWarning("子类粒子与父类sortingLayer不同的 Name:" + tagert.name);
                    }
                    tagert.sortingLayerName = layerName;
                    tagert.sortingOrder = order;
                }
            }
        }
    }
}


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class HiddenObjectExplorer : EditorWindow
{
    private List<GameObject> allObjects = new List<GameObject>();
    private List<GameObject> displayList = new List<GameObject>();
    private Vector2 scrollPos = Vector2.zero;
    private string searchName = string.Empty;
    private bool onlyTopLevel = false;
    private bool onlyScene = false;
    private bool isSearchName = false;
    private int listItemHeight = 15;

    [MenuItem("Thridparty/HiddenObjectExplorer")]
    static void Init()
    {
        GetWindow<HiddenObjectExplorer>();
    }

    void OnEnable()
    {
        FindAllObjects();
    }

    void FindAllObjects()
    {
        var objs = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        allObjects.Clear();
        allObjects.AddRange(objs);
        ApplyFilter();
    }

    void ApplyFilter()
    {
        displayList.Clear();
        foreach (var go in allObjects)
        {
            if (onlyScene)
            {
                if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                    continue;
                if (EditorUtility.IsPersistent(go.transform.root.gameObject))
                    continue;
            }
            if (onlyTopLevel)
            {
                var top = go.transform.root.gameObject;
                if (go != top)
                {
                    continue;
                }
                if (displayList.Contains(top))
                {
                    continue;
                }
            }
            if (isSearchName)
            {
                if (!go.name.Contains(searchName))
                {
                    continue;
                }
            }
            displayList.Add(go);
        }
    }

    HideFlags HideFlagsButton(string aTitle, HideFlags aFlags, HideFlags aValue)
    {
        if (GUILayout.Toggle((aFlags & aValue) > 0, aTitle, "Button", GUILayout.Height(listItemHeight)))
            aFlags |= aValue;
        else
            aFlags &= ~aValue;
        return aFlags;
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh"))
        {
            FindAllObjects();
        }
        EditorGUI.BeginChangeCheck();
        onlyTopLevel = GUILayout.Toggle(onlyTopLevel, "Only Top Level", "Button");
        onlyScene = GUILayout.Toggle(onlyScene, "Only Scene", "Button");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search Name: ");
        searchName = EditorGUILayout.TextField(searchName);
        GUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck())
        {
            isSearchName = !string.IsNullOrEmpty(searchName);
            ApplyFilter();
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < displayList.Count; i++)
        {
            GameObject O = displayList[i];
            if (O == null)
                continue;
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(O.name, O, typeof(GameObject), true, GUILayout.Height(listItemHeight));
            HideFlags flags = O.hideFlags;
            flags = HideFlagsButton("HideInHierarchy", flags, HideFlags.HideInHierarchy);
            flags = HideFlagsButton("HideInInspector", flags, HideFlags.HideInInspector);
            flags = HideFlagsButton("DontSave", flags, HideFlags.DontSave);
            flags = HideFlagsButton("NotEditable", flags, HideFlags.NotEditable);
            O.hideFlags = flags;
            GUILayout.Label("" + ((int)flags), GUILayout.Width(20), GUILayout.Height(listItemHeight));
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }
}

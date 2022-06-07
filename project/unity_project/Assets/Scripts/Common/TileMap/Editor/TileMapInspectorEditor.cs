using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

namespace Universal.TileMapping
{
    partial class TileMapEditor : Editor
    {
        SerializedProperty spMapWidth;
        SerializedProperty spMapHeight;
        SerializedProperty spGridSize;
        SerializedProperty spXRotation;
        SerializedProperty spYRotation;
        SerializedProperty spIsoWidth;
        SerializedProperty spIsoHeight;
        SerializedProperty spMapLayout;
        SerializedProperty spOuterRadius;
        SerializedProperty spOrientation;

        partial void OnInspectorEnable()
        {
            spMapWidth = serializedObject.FindProperty("mapWidth");
            spMapHeight = serializedObject.FindProperty("mapHeight");
            spIsoWidth = serializedObject.FindProperty("isoWidth");
            spXRotation = serializedObject.FindProperty("xRotation");
            spYRotation = serializedObject.FindProperty("yRotation");
            spGridSize = serializedObject.FindProperty("gridSize");
            spIsoHeight = serializedObject.FindProperty("isoHeight");
            spMapLayout = serializedObject.FindProperty("mapLayout");
            spOuterRadius = serializedObject.FindProperty("outerRadius");
            spOrientation = serializedObject.FindProperty("orientation");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            TileRenderer renderer = tileMap.GetComponent<TileRenderer>();
            if (renderer == null)
            {
                Texture2D tex = EditorGUIUtility.FindTexture("console.erroricon");
                if (GUILayout.Button(new GUIContent("No TileRenderer attached! Click here to add one.", tex), EditorStyles.helpBox))
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (Type type in Assembly.GetAssembly(typeof(TileRenderer)).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TileRenderer))))
                    {
                        menu.AddItem(new GUIContent(type.Name), false, () => { tileMap.gameObject.AddComponent(type); });
                    }
                    menu.ShowAsContext();
                }
            }
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            isInEditMode = GUILayout.Toggle(isInEditMode, "", "Button", GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.5f));
            string toggleButtonText = (isInEditMode ? "Exit" : "Enter") + " Edit Mode[`]";
            GUI.Label(GUILayoutUtility.GetLastRect(), toggleButtonText, TileMapGUIStyles.centerBoldLabel);

            if (EditorGUI.EndChangeCheck())
            {
                if (isInEditMode)
                    OnEnterEditMode();
                else
                    OnExitEditMode();
            }
            EditorGUILayout.Space();

            GUILayout.Label("Settings", TileMapGUIStyles.leftBoldLabel);
            EditorGUI.BeginDisabledGroup(!isInEditMode);
            int mapWidth = spMapWidth.intValue;
            mapWidth = EditorGUILayout.IntField(spMapWidth.displayName, mapWidth);
            int mapHeight = spMapHeight.intValue;
            mapHeight = EditorGUILayout.IntField(spMapHeight.displayName, mapHeight);

            bool isMapSizeChanged = (mapWidth != spMapWidth.intValue || mapHeight != spMapHeight.intValue);
            if (isMapSizeChanged)
            {
                tileMap.ResizeMap(mapWidth, mapHeight);
            }

            TileMap.Layout layout = (TileMap.Layout)spMapLayout.enumValueIndex;
            layout = (TileMap.Layout)EditorGUILayout.EnumPopup(spMapWidth.displayName, layout);
            if (layout != tileMap.MapLayout)
                tileMap.ChangeLayout(layout);

            if (layout == TileMap.Layout.CartesianCoordinate)
            {
                float gridSize = spGridSize.floatValue;
                gridSize = EditorGUILayout.FloatField(spGridSize.displayName, gridSize);
                if (gridSize != tileMap.GridSize)
                {
                    tileMap.ResizeGrid(gridSize);
                }
            }
            else if (layout == TileMap.Layout.Hexagonal)
            {
                float outerRadius = spOuterRadius.floatValue;
                outerRadius = EditorGUILayout.FloatField(spOuterRadius.displayName, outerRadius);
                if (outerRadius != tileMap.OuterRadius)
                {
                    tileMap.ResizeHexGrid(outerRadius);
                }

                TileMap.HexOrientation orientation = (TileMap.HexOrientation)spOrientation.enumValueIndex;
                orientation = (TileMap.HexOrientation)EditorGUILayout.EnumPopup(spOrientation.displayName, orientation);
                if (orientation != tileMap.Orientation)
                    tileMap.ResizeHexOrientation(orientation);
            }
            else
            {
                float isoWidth = spIsoWidth.floatValue;
                isoWidth = EditorGUILayout.FloatField(spIsoWidth.displayName, isoWidth);
                float isoHeight = spIsoHeight.floatValue;
                isoHeight = EditorGUILayout.FloatField(spIsoHeight.displayName, isoHeight);

                bool isIsoSizeChanged = (isoWidth != spIsoWidth.floatValue || isoHeight != spIsoHeight.floatValue);
                if (isIsoSizeChanged)
                {
                    tileMap.ResizeIso(isoWidth, isoHeight);
                }
                if (layout == TileMap.Layout.IsometricDiamondFreestyle)
                {
                    float xRotation = spXRotation.floatValue;
                    xRotation = EditorGUILayout.FloatField(spXRotation.displayName, xRotation);
                    float yRotation = spYRotation.floatValue;
                    yRotation = EditorGUILayout.FloatField(spYRotation.displayName, yRotation);
                    if (xRotation != tileMap.XRotation || yRotation != tileMap.YRotation)
                    {
                        tileMap.ChangeFreestyleRotation(xRotation, yRotation);
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();

            GUILayout.Label("Tools", TileMapGUIStyles.leftBoldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Force Refresh"))
            {
                tileMap.UpdateTileMap();
            }
            GUI.color = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("Clear All Tiles"))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to clear this tilemap without saving?", "Okay", "Cancel"))
                {
                    tileMap.Reset();
                    SetTileMapDirty();
                }
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Import"))
            {
                string path = EditorUtility.OpenFilePanelWithFilters("Select .tilemap File", "Assets", new string[] { "ScriptableObject", "asset" });
                if (path != string.Empty)
                {
                    int cutoffFrom = path.IndexOf("Assets");
                    path = path.Substring(cutoffFrom);
                    Debug.Log(path);
                    TileMapContainer container = AssetDatabase.LoadAssetAtPath<TileMapContainer>(path) as TileMapContainer;
                    if (EditorUtility.DisplayDialog("Are you sure?", "Importing this tilemap will override the current one without saving it.", "Okay", "Cancel"))
                    {
                        tileMap.ResizeMap(container.mapWidth, container.mapHeight);
                        for (int x = 0; x < container.mapWidth; x++)
                        {
                            for (int y = 0; y < container.mapHeight; y++)
                            {
                                tileMap.SetTileAt(x, y, container.map[x + y * container.mapWidth]);
                            }
                        }
                    }
                }
            }
            if (GUILayout.Button("Export"))
            {
                TileMapContainer container = ScriptableObjectUtility.CreateAsset<TileMapContainer>(tileMap.name + ".asset");
                container.map = new ScriptableTile[tileMap.MapWidth * tileMap.MapHeight];
                for(int x = 0; x < tileMap.MapWidth; x++)
                {
                    for (int y = 0; y < tileMap.MapHeight; y++)
                    {
                        container.map[x + y * tileMap.MapWidth] = tileMap.Map[x, y];
                    }
                }
                container.mapWidth = tileMap.MapWidth;
                container.mapHeight = tileMap.MapHeight;
                container.gridSize = tileMap.GridSize;
                container.isoWidth = tileMap.IsoWidth;
                container.isoHeight = tileMap.IsoHeight;
                container.mapLayout = tileMap.MapLayout;
                container.xRotation = tileMap.XRotation;
                container.yRotation = tileMap.YRotation;
                container.outerRadius = tileMap.OuterRadius;
                container.orientation = tileMap.Orientation;
            }
            GUILayout.EndHorizontal();
            if (Event.current.type == EventType.KeyDown && Event.current.isKey && Event.current.keyCode == KeyCode.BackQuote)
            {
                isInEditMode = !isInEditMode;
            }
            if (Event.current.type == EventType.KeyDown && Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
            {
                isInEditMode = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
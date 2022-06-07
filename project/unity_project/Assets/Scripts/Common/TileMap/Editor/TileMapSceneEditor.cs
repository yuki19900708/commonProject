using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Universal.TileMapping
{
    partial class TileMapEditor : Editor
    {
        bool isInEditMode = false;

        List<ScriptableTool> scriptableToolCache = new List<ScriptableTool>();
        List<ScriptableTile> scriptableTileCache = new List<ScriptableTile>();

        ScriptableTile primaryTile;
        ScriptableTile secondaryTile;

        Rect toolbarWindowPosition;
        Rect tilePickerWindowPosition;
        Vector2 tilePickerScrollView;

        int selectedScriptableTool = -1;
        int lastSelectedScriptableTool = -1;

        bool primaryTilePickerToggle = false;
        bool secondaryTilePickerToggle = false;

        bool showGrid = true;
        string searchText = string.Empty;

        private void Update()
        {
            if (isInEditMode)
            {
                Selection.activeObject = tileMap;
                SceneView currentView = SceneView.lastActiveSceneView;
                if (currentView)
                    currentView.in2DMode = true;

                Tools.current = Tool.None;
                SceneModeUtility.SearchForType(typeof(TileMap));
                if (TileMap.pickedTile != null)
                {
                    primaryTile = TileMap.pickedTile;
                    TileMap.pickedTile = null;
                }
            }
        }

        partial void OnSceneEnable()
        {
            EditorApplication.update += Update;
        }

        partial void OnSceneDisable()
        {
            EditorApplication.update -= Update;
        }

        private void OnEnterEditMode()
        {
            RefreshScriptableToolCache();
            RefreshScriptableTileCache();

            Tools.hidden = true;
            Tools.current = Tool.None;
        }

        private void OnExitEditMode()
        {
            SceneModeUtility.SearchForType(null);
            if (!Application.isPlaying)
                EditorSceneManager.SaveOpenScenes();
            Tools.hidden = false;
            Tools.current = Tool.Move;
        }

        private void OnSceneGUI()
        {
            DrawGrid();
            if (isInEditMode)
            {
                int toolbarControlID = GUIUtility.GetControlID("ToolbarWindow".GetHashCode(), FocusType.Passive);

                if (Event.current.type == EventType.Layout)
                {
                    HandleUtility.AddDefaultControl(toolbarControlID);
                }
                toolbarWindowPosition = GUILayout.Window(toolbarControlID, toolbarWindowPosition, ToolbarWindow, new GUIContent("Toolbar"));

                if (!secondaryTilePickerToggle && !primaryTilePickerToggle)
                {
                    tilePickerWindowPosition = new Rect(new Vector2(toolbarWindowPosition.xMax, 0), toolbarWindowPosition.size);
                }

                if (primaryTilePickerToggle)
                {
                    tilePickerWindowPosition = ClampToScreen(GUILayout.Window(GUIUtility.GetControlID(FocusType.Passive), tilePickerWindowPosition, (int id) => { TilepickerWindow(id, ref primaryTile); }, new GUIContent("Pick primary tile..."), GUILayout.Width(100)));
                }
                else if (secondaryTilePickerToggle)
                {
                    tilePickerWindowPosition = ClampToScreen(GUILayout.Window(GUIUtility.GetControlID(FocusType.Passive), tilePickerWindowPosition, (int id) => { TilepickerWindow(id, ref secondaryTile); }, new GUIContent("Pick secondary tile..."), GUILayout.Width(100)));
                }

                HandleKeyboardEvents();
                HandleMouseEvents();
                
                tileMap.transform.localScale = Vector3.one;
                SceneView.RepaintAll();
            }
            else
            {
                toolbarWindowPosition = new Rect(5, 25, 148, 480);
            }
        }

        private void ToolbarWindow(int id)
        {
            string text = "None";
            float pickerSize = toolbarWindowPosition.width * 0.4f;
            float pickerPadding = toolbarWindowPosition.width * 0.2f / 3;
            Rect secondaryTileRect = new Rect(pickerPadding * 2 + pickerSize, 25, pickerSize, pickerSize);

            GUI.DrawTexture(secondaryTileRect, new Texture2D(16, 16));
            if (secondaryTile)
            {
                GUI.DrawTexture(secondaryTileRect, secondaryTile.GetPreview());
                text = secondaryTile.Name;
            }
            GUI.Label(secondaryTileRect, text, TileMapGUIStyles.centerWhiteMiniLabel);

            EditorGUI.BeginChangeCheck();
            secondaryTilePickerToggle = GUI.Toggle(secondaryTileRect, secondaryTilePickerToggle, "Secondary");
            if (EditorGUI.EndChangeCheck())
            {
                primaryTilePickerToggle = false;
            }

            Rect primaryTileRect = new Rect(pickerPadding, 25, pickerSize, pickerSize);

            text = "None";

            GUI.DrawTexture(primaryTileRect, new Texture2D(16, 16));
            if (primaryTile)
            {
                GUI.DrawTexture(primaryTileRect, primaryTile.GetPreview());
                text = primaryTile.Name;
            }
            GUI.Label(primaryTileRect, text, TileMapGUIStyles.centerWhiteMiniLabel);
            EditorGUI.BeginChangeCheck();
            primaryTilePickerToggle = GUI.Toggle(primaryTileRect, primaryTilePickerToggle, "Primary");
            if (EditorGUI.EndChangeCheck())
            {
                secondaryTilePickerToggle = false;
            }
            GUI.contentColor = Color.white;
            
            GUILayout.Space(pickerPadding + pickerSize);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Swap [X]"))
            {
                Swap<ScriptableTile>(ref primaryTile, ref secondaryTile);
            }
            GUI.color = new Color(1, 0.5f, 0.5f);
            if (GUILayout.Button("Clear [ ]"))
            {
                primaryTile = null;
                secondaryTile = null;
            }

            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = tileMap.CanUndo;
            if (GUILayout.Button("Undo [Z]"))
            {
                tileMap.Undo();
                SetTileMapDirty();
            }
            GUI.enabled = tileMap.CanRedo;
            if (GUILayout.Button("Redo [R]"))
            {
                tileMap.Redo();
                SetTileMapDirty();
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            
            GUILayout.Label("Tools", TileMapGUIStyles.leftBoldLabel);
            EditorGUILayout.HelpBox("Use [Right Mouse Button] to toggle between recent two tool", MessageType.Info, true);

            for (int i = 0; i < scriptableToolCache.Count; i++)
            {
                if (scriptableToolCache[i] is Pencil && selectedScriptableTool == -1)
                {
                    selectedScriptableTool = i;
                }
                bool selected = (i == selectedScriptableTool);
                EditorGUI.BeginChangeCheck();
                string labelName = scriptableToolCache[i].Shortcut != KeyCode.None ?
                            string.Format("{1} [{0}]", scriptableToolCache[i].Shortcut.ToString(), scriptableToolCache[i].Name) :
                            scriptableToolCache[i].Name;

                GUIContent content = new GUIContent(labelName, scriptableToolCache[i].Description);
                GUILayout.Toggle(selected, content, EditorStyles.radioButton, GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    lastSelectedScriptableTool = selectedScriptableTool;

                    selectedScriptableTool = i;
                }
            }

            GUILayout.Label("Visual", TileMapGUIStyles.leftBoldLabel, GUILayout.Width(100));
            GUIContent showGridContent = new GUIContent("Show Grid", "Hide or Display Grid");
            showGrid = GUILayout.Toggle(showGrid, showGridContent, EditorStyles.toggle, GUILayout.Width(100));
            if (selectedScriptableTool == -1)
            {
                EditorGUILayout.HelpBox("No Tool selected, select one from above.", MessageType.Warning, true);
            }
            if (selectedScriptableTool >= 0 && selectedScriptableTool < scriptableToolCache.Count)
            {
                const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
                FieldInfo[] fields = scriptableToolCache[selectedScriptableTool].GetType().GetFields(flags);

                if (fields.Length > 0)
                {
                    GUILayout.Label("Settings", TileMapGUIStyles.leftBoldLabel, GUILayout.Width(100));
                    for (int i = 0; i < fields.Length; i++)
                    {
                        FieldInfo field = fields[i];
                        Type type = field.FieldType;
                        string fieldName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(field.Name);

                        GUILayout.BeginHorizontal(GUILayout.Width(100));
                        GUILayout.Label(fieldName, EditorStyles.miniLabel);
                        if (type == typeof(bool))
                        {
                            bool v = (bool)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            bool nv = EditorGUILayout.Toggle(v);
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(float))
                        {
                            float v = (float)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            float nv = EditorGUILayout.FloatField(v);
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(int))
                        {
                            int v = (int)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            int nv = EditorGUILayout.IntField(v);
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(Enum))
                        {
                            int v = (int)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            int nv = EditorGUILayout.IntField(v);
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(Vector2))
                        {
                            Vector2 v = (Vector2)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            Vector2 nv = Vector2.zero;
                            nv.x = EditorGUILayout.FloatField(v.x);
                            nv.y = EditorGUILayout.FloatField(v.y);
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(Vector3))
                        {
                            Vector3 v = (Vector3)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            Vector3 nv = Vector3.zero;
                            nv.x = EditorGUILayout.FloatField(v.x);
                            nv.y = EditorGUILayout.FloatField(v.y);
                            nv.z = EditorGUILayout.FloatField(v.z);
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(Color))
                        {
                            Color v = (Color)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            Color nv = EditorGUILayout.ColorField(v);
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(AnimationCurve))
                        {
                            AnimationCurve v = (AnimationCurve)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            AnimationCurve nv = EditorGUILayout.CurveField(v);
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(GameObject))
                        {
                            GameObject v = (GameObject)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            GameObject nv = EditorGUILayout.ObjectField(v, typeof(GameObject), false) as GameObject;
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(Texture2D))
                        {
                            Texture2D v = (Texture2D)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            Texture2D nv = EditorGUILayout.ObjectField(v, typeof(Texture2D), false) as Texture2D;
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(Sprite))
                        {
                            Sprite v = (Sprite)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            Sprite nv = EditorGUILayout.ObjectField(v, typeof(Sprite), false) as Sprite;
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type == typeof(UnityEngine.Object))
                        {
                            UnityEngine.Object v = (UnityEngine.Object)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            UnityEngine.Object nv = EditorGUILayout.ObjectField(v, typeof(UnityEngine.Object), false) as UnityEngine.Object;
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else if (type.IsEnum)
                        {
                            int v = (int)field.GetValue(scriptableToolCache[selectedScriptableTool]);
                            int nv = EditorGUILayout.Popup(v, Enum.GetNames(type));
                            field.SetValue(scriptableToolCache[selectedScriptableTool], nv);
                        }
                        else
                        {
                            Debug.LogErrorFormat("Exposing public variable type '{0}' is currently not supported by Tooles \n Feel free to add support for it though!", type.Name);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Tool has no public variables to edit.", MessageType.Info, true);
                }
            }
            GUI.DragWindow();
        }

        private void TilepickerWindow(int id, ref ScriptableTile tileToChange)
        {
            GUI.BringWindowToFront(id);

            if (scriptableTileCache.Count > 0)
            {
                float tileWidth = tilePickerWindowPosition.width - 35;

                GUILayout.BeginHorizontal();
                GUILayout.Label("Search: ", GUILayout.Width(50));
                GUI.SetNextControlName("SearchInput");
                searchText = GUILayout.TextArea(searchText);
                GUILayout.EndHorizontal();
                
                tilePickerScrollView = GUILayout.BeginScrollView(tilePickerScrollView, false, true, GUILayout.Height(tilePickerWindowPosition.height - 70));

                GUILayout.BeginVertical();
                for (int index = -1; index < scriptableTileCache.Count; index++)
                {
                    if (index == -1)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("None", GUILayout.Width(tileWidth)))
                        {
                            searchText = string.Empty;
                            tileToChange = null;
                            primaryTilePickerToggle = secondaryTilePickerToggle = false;
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        string tileText = scriptableTileCache[index].Name;
                        if (!string.IsNullOrEmpty(searchText) && !tileText.Contains(searchText))
                        {
                            continue;
                        }
                        GUI.color = scriptableTileCache[index].IsValid ? Color.white : new Color(1, 0.5f, 0.5f);

                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(tileText, GUILayout.Width(tileWidth)))
                        {
                            searchText = string.Empty;
                            if (scriptableTileCache[index].IsValid)
                            {
                                tileToChange = scriptableTileCache[index];
                                primaryTilePickerToggle = secondaryTilePickerToggle = false;
                            }
                            else
                            {
                                Type pt = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ProjectBrowser");
                                EditorWindow.GetWindow(pt).Show();
                                EditorGUIUtility.PingObject(scriptableTileCache[index]);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("No Tiles found. Try force refresh or \nIn the project window, 'Create > Tilemap > Tiles > ...' to create one", MessageType.Warning);
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Force Refresh"))
            {
                RefreshScriptableTileCache();
            }
            GUI.color = new Color(1f, 0.5f, 0.5f);
            if (GUILayout.Button("Close Tile Picker"))
            {
                primaryTilePickerToggle = secondaryTilePickerToggle = false;
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }
        private void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        private void RefreshScriptableToolCache()
        {
            List<ScriptableTool> toRemove = new List<ScriptableTool>();
            for (int i = 0; i < scriptableToolCache.Count; i++)
            {
                if (scriptableToolCache[i] == null)
                    toRemove.Add(scriptableToolCache[i]);
            }
            scriptableToolCache = scriptableToolCache.Except(toRemove).ToList();
            foreach (Type type in Assembly.GetAssembly(typeof(ScriptableTool)).GetTypes()
                     .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(ScriptableTool))))
            {
                bool containsType = false;
                for (int i = 0; i < scriptableToolCache.Count; i++)
                {
                    if (scriptableToolCache[i].GetType() == type)
                    {
                        containsType = true;
                        break;
                    }
                }
                if (!containsType)
                    scriptableToolCache.Add((ScriptableTool)Activator.CreateInstance(type));
            }
        }

        private void RefreshScriptableTileCache()
        {
            List<ScriptableTile> toRemove = new List<ScriptableTile>();
            for (int i = 0; i < scriptableTileCache.Count; i++)
            {
                if (scriptableTileCache[i] == null)
                    toRemove.Add(scriptableTileCache[i]);
            }
            AssetDatabase.Refresh();
            scriptableTileCache = scriptableTileCache.Except(toRemove).ToList();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(ScriptableTile)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                ScriptableTile asset = AssetDatabase.LoadAssetAtPath<ScriptableTile>(assetPath);
                if (asset != null && !scriptableTileCache.Contains(asset))
                {
                    scriptableTileCache.Add(asset);
                }
            }
        }

        private void DrawGrid()
        {
            if (showGrid == false)
            {
                return;
            }
            int mapWidth = tileMap.MapWidth;
            int mapHeight = tileMap.MapHeight;
            TileMap.Layout mapLayout = tileMap.MapLayout;

            Handles.color = Color.black;

            if (mapLayout == TileMap.Layout.CartesianCoordinate ||
                mapLayout == TileMap.Layout.IsometricDiamond ||
                mapLayout == TileMap.Layout.IsometricDiamondFreestyle)
            {
                //Draw horizontal grid lines
                for (int i = 0; i <= mapHeight; i++)
                {
                    Vector3 start = tileMap.Coordinate2WorldPosition(0, i);
                    Vector3 end = tileMap.Coordinate2WorldPosition(mapWidth, i);
                    Handles.DrawLine(start, end);
                }
                //Draw vertical grid lines
                for (int i = 0; i <= mapWidth; i++)
                {
                    Vector3 start = tileMap.Coordinate2WorldPosition(i, 0);
                    Vector3 end = tileMap.Coordinate2WorldPosition(i, mapHeight);
                    Handles.DrawLine(start, end);
                }
            }
            else if (mapLayout == TileMap.Layout.IsometricStaggered)
            {
                //Draw horizontal(/) grid lines
                int hlineCount = 2 + (mapHeight - 1) / 2 + (mapWidth - 1);
                for (int i = 0; i < hlineCount; i++)
                {
                    Vector3 start = Vector3.zero;
                    if (i < (mapHeight + 1) / 2)
                    {
                        int startX = -1;
                        int startY = mapHeight - i * 2 - 1 + mapHeight % 2;
                        start = tileMap.Coordinate2WorldPosition(startX, startY);
                    }
                    else
                    {
                        int startX = i - (mapHeight + 1) / 2;
                        int startY = 0;
                        start = tileMap.Coordinate2WorldPosition(startX, startY);
                    }

                    Vector3 end = Vector3.zero;
                    if (i < mapWidth)
                    {
                        int endX = i;
                        int endY = mapHeight + 1;
                        end = tileMap.Coordinate2WorldPosition(endX, endY);
                    }
                    else if (i <= mapWidth)
                    {
                        int endX = i - mapHeight % 2;
                        int endY = mapHeight;
                        end = tileMap.Coordinate2WorldPosition(endX, endY);
                    }
                    else
                    {
                        int endX = mapWidth;
                        int endY = (mapHeight + mapHeight % 2) - (i - mapWidth) * 2;
                        end = tileMap.Coordinate2WorldPosition(endX, endY);
                    }
                    Handles.DrawLine(start, end);
                }
                //Draw vertical(\) grid lines3
                int vlineCount = 2 + mapHeight / 2 + (mapWidth - 1);
                for (int i = 0; i < vlineCount; i++)
                {
                    Vector3 start = Vector3.zero;
                    if (i < (mapHeight + 1) / 2)
                    {
                        int startX = -1;
                        int startY = i * 2 + 1;
                        start = tileMap.Coordinate2WorldPosition(startX, startY);
                    }
                    else if (i == (mapHeight + 1) / 2)
                    {
                        int startX = 0;
                        int startY = (mapHeight + 1) / 2 * 2;
                        start = tileMap.Coordinate2WorldPosition(startX, startY);
                    }
                    else
                    {
                        int startX = i - (mapHeight + 1) / 2 - (1 - mapHeight % 2);
                        int startY = mapHeight + 1;
                        start = tileMap.Coordinate2WorldPosition(startX, startY);
                    }

                    Vector3 end = Vector3.zero;
                    if (i < mapWidth)
                    {
                        int endX = i;
                        int endY = 0;
                        end = tileMap.Coordinate2WorldPosition(endX, endY);
                    }
                    else if (i == mapWidth)
                    {
                        int endX = mapWidth - 1;
                        int endY = 1;
                        end = tileMap.Coordinate2WorldPosition(endX, endY);
                    }
                    else
                    {
                        int endX = mapWidth;
                        int endY = (i - mapWidth) * 2;
                        end = tileMap.Coordinate2WorldPosition(endX, endY);
                    }
                    Handles.DrawLine(start, end);
                }
            }
            else if (mapLayout == TileMap.Layout.Hexagonal)
            {
                if (tileMap.Orientation == TileMap.HexOrientation.PointySideUp)
                {
                    int pointCountH = 2 * mapWidth + 1;
                    //Last horizontal line
                    Vector3[] points = new Vector3[pointCountH];
                    points[0] = tileMap.Coordinate2WorldPosition(0, mapHeight - 1) + (Vector3)tileMap.HexCornersOffset[5];
                    for (int j = 0; j < mapWidth; j++)
                    {
                        Vector3 baseCorner = tileMap.Coordinate2WorldPosition(j, mapHeight - 1);
                        points[j * 2 + 1] = baseCorner + (Vector3)tileMap.HexCornersOffset[4];
                        points[j * 2 + 2] = baseCorner + (Vector3)tileMap.HexCornersOffset[3];
                    }
                    Handles.DrawPolyLine(points);
                    //Draw horizontal grid lines
                    for (int i = 0; i < mapHeight; i++)
                    {
                        points = new Vector3[pointCountH];
                        points[0] = tileMap.Coordinate2WorldPosition(0, i) + (Vector3)tileMap.HexCornersOffset[0];
                        for (int j = 0; j < mapWidth; j++)
                        {
                            Vector3 baseCorner = tileMap.Coordinate2WorldPosition(j, i);
                            points[j * 2 + 1] = baseCorner + (Vector3)tileMap.HexCornersOffset[1];
                            points[j * 2 + 2] = baseCorner + (Vector3)tileMap.HexCornersOffset[2];
                        }

                        Handles.DrawPolyLine(points);
                    }

                    int pointCountV = 2 * mapHeight + 1;
                    //Draw vertical grid lines
                    for (int i = 0; i < mapWidth; i++)
                    {
                        points = new Vector3[pointCountV];
                        for (int j = 0; j < mapHeight; j++)
                        {
                            Vector3 baseCorner = tileMap.Coordinate2WorldPosition(i, j);
                            points[j * 2] = baseCorner + (Vector3)tileMap.HexCornersOffset[0];
                            points[j * 2 + 1] = baseCorner + (Vector3)tileMap.HexCornersOffset[5];
                        }
                        Vector3 finalCorner = tileMap.Coordinate2WorldPosition(i, mapHeight - 1);
                        points[pointCountV - 1] = finalCorner + (Vector3)tileMap.HexCornersOffset[4];

                        Handles.DrawPolyLine(points);
                    }

                    //Last vertical line
                    points = new Vector3[pointCountV - 1];
                    for (int j = 0; j < mapHeight; j++)
                    {
                        Vector3 baseCorner = tileMap.Coordinate2WorldPosition(mapWidth, j);
                        points[j * 2] = baseCorner + (Vector3)tileMap.HexCornersOffset[0];
                        points[j * 2 + 1] = baseCorner + (Vector3)tileMap.HexCornersOffset[5];
                    }

                    Handles.DrawPolyLine(points);
                }
                else
                {
                    int pointCountH = 2 * mapWidth + 1;
                    //first horizontal line
                    Vector3[] points = new Vector3[pointCountH - 1];
                    for (int j = 0; j < mapWidth; j++)
                    {
                        Vector3 baseCorner = tileMap.Coordinate2WorldPosition(j, 0);
                        points[j * 2] = baseCorner + (Vector3)tileMap.HexCornersOffset[0];
                        points[j * 2 + 1] = baseCorner + (Vector3)tileMap.HexCornersOffset[1];
                    }
                    Handles.DrawPolyLine(points);
                    //Draw horizontal grid lines
                    for (int i = 0; i < mapHeight; i++)
                    {
                        points = new Vector3[pointCountH];
                        points[0] = tileMap.Coordinate2WorldPosition(0, i) + (Vector3)tileMap.HexCornersOffset[5];
                        for (int j = 0; j < mapWidth; j++)
                        {
                            Vector3 baseCorner = tileMap.Coordinate2WorldPosition(j, i);
                            points[j * 2 + 1] = baseCorner + (Vector3)tileMap.HexCornersOffset[4];
                            points[j * 2 + 2] = baseCorner + (Vector3)tileMap.HexCornersOffset[3];
                        }

                        Handles.DrawPolyLine(points);
                    }
                    
                    int pointCountV = 2 * mapHeight + 1;
                    //first vertical line
                    points = new Vector3[pointCountV - 1];
                    for (int j = 0; j < mapHeight; j++)
                    {
                        Vector3 baseCorner = tileMap.Coordinate2WorldPosition(0, j);
                        points[j * 2] = baseCorner + (Vector3)tileMap.HexCornersOffset[0];
                        points[j * 2 + 1] = baseCorner + (Vector3)tileMap.HexCornersOffset[5];
                    }
                    Handles.DrawPolyLine(points);
                    //Draw vertical grid lines
                    for (int i = 0; i < mapWidth; i++)
                    {
                        points = new Vector3[pointCountV];
                        Vector3 finalCorner = tileMap.Coordinate2WorldPosition(i, 0);
                        points[0] = finalCorner + (Vector3)tileMap.HexCornersOffset[1];
                        for (int j = 0; j < mapHeight; j++)
                        {
                            Vector3 baseCorner = tileMap.Coordinate2WorldPosition(i, j);
                            points[j * 2 + 1] = baseCorner + (Vector3)tileMap.HexCornersOffset[2];
                            points[j * 2 + 2] = baseCorner + (Vector3)tileMap.HexCornersOffset[3];
                        }

                        Handles.DrawPolyLine(points);
                    }
                }
            }
        }

        private void HandleKeyboardEvents()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.isKey && Event.current.keyCode == KeyCode.Z)
            {
                tileMap.Undo();
                SetTileMapDirty();
            }

            if (Event.current.type == EventType.KeyDown && Event.current.isKey && Event.current.keyCode == KeyCode.R)
            {
                tileMap.Redo();
                SetTileMapDirty();
            }

            if (Event.current.type == EventType.KeyDown && Event.current.isKey && Event.current.keyCode == KeyCode.X)
            {
                Swap<ScriptableTile>(ref primaryTile, ref secondaryTile);
            }

            for (int i = 0; i < scriptableToolCache.Count; i++)
            {
                if (Event.current.isKey && Event.current.keyCode == scriptableToolCache[i].Shortcut)
                {
                    Event.current.Use();
                    lastSelectedScriptableTool = selectedScriptableTool;
                    selectedScriptableTool = i;
                }
            }
        }

        private void HandleMouseEvents()
        {
            Event e = Event.current;
            Point point = new Point(0, 0);

            if (selectedScriptableTool >= 0 && selectedScriptableTool < scriptableToolCache.Count)
            {
                if (e.button == 0)
                {
                    GetMousePosition(ref point);

                    if (e.type == EventType.MouseDrag)
                    {
                        scriptableToolCache[selectedScriptableTool].OnClick(point, primaryTile, tileMap);
                    }
                    if (e.type == EventType.MouseDown)
                    {
                        scriptableToolCache[selectedScriptableTool].OnClickDown(point, primaryTile, tileMap);
                    }
                    if (e.type == EventType.MouseUp)
                    {
                        scriptableToolCache[selectedScriptableTool].OnClickUp(point, primaryTile, tileMap);

                        SetTileMapDirty();
                    }

                    List<Point> region = scriptableToolCache[selectedScriptableTool].GetRegion(point, primaryTile, tileMap);

                    for (int i = 0; i < region.Count; i++)
                    {
                        point = region[i];
                        if (tileMap.MapLayout == TileMap.Layout.CartesianCoordinate ||
                            tileMap.MapLayout == TileMap.Layout.IsometricDiamond ||
                            tileMap.MapLayout == TileMap.Layout.IsometricDiamondFreestyle)
                        {
                            Vector3 p1 = tileMap.Coordinate2WorldPosition(point);
                            Vector3 p2 = tileMap.Coordinate2WorldPosition(point.Right);
                            Vector3 p3 = tileMap.Coordinate2WorldPosition(point.Right.Up);
                            Vector3 p4 = tileMap.Coordinate2WorldPosition(point.Up);
                            Handles.color = tileMap.IsInBounds(point) ? Color.green : Color.red;
                            Handles.DrawAAPolyLine(6, new Vector3[] { p1, p2, p3, p4, p1 });
                        }
                        else if (tileMap.MapLayout == TileMap.Layout.IsometricStaggered)
                        {
                            Handles.color = tileMap.IsInBounds(point) ? Color.green : Color.red;
                            if (point.y % 2 == 0)
                            {
                                Vector3 p1 = tileMap.Coordinate2WorldPosition(point);
                                Vector3 p2 = tileMap.Coordinate2WorldPosition(point.Up);
                                Vector3 p3 = tileMap.Coordinate2WorldPosition(point.Up.Up);
                                Vector3 p4 = tileMap.Coordinate2WorldPosition(point.Left.Up);
                                Handles.DrawAAPolyLine(6, new Vector3[] { p1, p2, p3, p4, p1 });
                            }
                            else
                            {
                                Vector3 p1 = tileMap.Coordinate2WorldPosition(point);
                                Vector3 p2 = tileMap.Coordinate2WorldPosition(point.Up);
                                Vector3 p3 = tileMap.Coordinate2WorldPosition(point.Up.Up);
                                Vector3 p4 = tileMap.Coordinate2WorldPosition(point.Right.Up);
                                Handles.DrawAAPolyLine(6, new Vector3[] { p1, p2, p3, p4, p1 });
                            }
                        }
                        else if (tileMap.MapLayout == TileMap.Layout.Hexagonal)
                        {
                            Handles.color = tileMap.IsInBounds(point) ? Color.green : Color.red;
                            Vector3 center = tileMap.Coordinate2WorldPosition(point);
                            Vector3 p1 = center + (Vector3)tileMap.HexCornersOffset[0];
                            Vector3 p2 = center + (Vector3)tileMap.HexCornersOffset[1];
                            Vector3 p3 = center + (Vector3)tileMap.HexCornersOffset[2];
                            Vector3 p4 = center + (Vector3)tileMap.HexCornersOffset[3];
                            Vector3 p5 = center + (Vector3)tileMap.HexCornersOffset[4];
                            Vector3 p6 = center + (Vector3)tileMap.HexCornersOffset[5];
                            Handles.DrawAAPolyLine(6, new Vector3[] { p1, p2, p3, p4, p5, p6, p1 });
                        }
                    }

                    Handles.color = Color.white;
                    GUIStyle style = new GUIStyle(TileMapGUIStyles.centerWhiteBoldLabel);
                    style.normal.textColor = Handles.color;
                    Vector3 worldPos = tileMap.Coordinate2WorldPosition(point);
                    Handles.Label(worldPos, point.ToString(), style);
                }
            }
            if ((e.type == EventType.MouseDown) && e.button == 1)
            {
                Swap<int>(ref selectedScriptableTool, ref lastSelectedScriptableTool);
            }
        }

        private bool GetMousePosition(ref Point point)
        {
            if (SceneView.currentDrawingSceneView == null)
                return false;

            Plane plane = new Plane(tileMap.transform.TransformDirection(Vector3.forward), tileMap.transform.position);
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            float isoWidth = tileMap.IsoWidth;
            float isoHeight = tileMap.IsoHeight;

            float distance;
            if (plane.Raycast(ray, out distance))
            {
                Vector3 worldPos = ray.origin + (ray.direction.normalized * distance);
                point = tileMap.WorldPosition2Coordinate(worldPos);
            }
            bool result = (point.x >= 0 && point.x < tileMap.MapWidth && point.y >= 0 && point.y < tileMap.MapHeight);
            return result;
        }

        private Rect ClampToScreen(Rect r)
        {
            r.x = Mathf.Clamp(r.x, 5, Screen.width - r.width - 5);
            r.y = Mathf.Clamp(r.y, 25, Screen.height - r.height - 25);
            return r;
        }
        private void SetTileMapDirty()
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                EditorUtility.SetDirty(tileMap);
            }
        }
    }
}
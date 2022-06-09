using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Universal.TileMapping
{
    [CustomEditor(typeof(AutoTile))]
    public class AutoTileEditor : ScriptableTileEditor
    {
        private AutoTile autoTile;

        private int[,] indexLookup4 = new int[,]
        {
            { 0, 1, 2, 3  },
            { 4, 5 ,6, 7 }
        };

        private int[,] indexLookup8 = new int[,]
        {
            { 15,  31,  47,  61,  63,  79,  95 },
            { 107, 111, 121, 123, 125, 127, 143 },
            { 158, 159, 175, 188, 189, 190, 191 },
            { 199, 207, 214, 215, 222, 223, 227 },
            { 231, 235, 239, 240, 241, 242, 243 },
            { 244, 245, 246, 247, 248, 249, 250 },
            { 251, 252, 253, 254, 255, -1,  -1, },
        };

        private void OnEnable()
        {
            autoTile = (AutoTile)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Fill the grid below with tiles or prefabs.", MessageType.Info);

            GUILayout.Label("Settings:", TileMapGUIStyles.leftBoldLabel);
            autoTile.mode = (AutoTile.AutoTileMode)EditorGUILayout.EnumPopup("Tiling Mode", autoTile.mode);
            
            autoTile.neighbourMode = (AutoTile.NeighbourMode)EditorGUILayout.EnumPopup("Neighbour Mode", autoTile.neighbourMode);

            GUILayout.Label("Tiles: T/P means use a random tile or just prefab", TileMapGUIStyles.leftBoldLabel);

            GUILayout.BeginVertical();
            int rowCount = 2;
            int columnCount = 4;
            int[,] indexLookup;
            if (autoTile.neighbourMode == AutoTile.NeighbourMode.Eight)
            {
                rowCount = 7;
                columnCount = 7;
                indexLookup = indexLookup8;
            }
            else
            {
                indexLookup = indexLookup4;
            }
            float sizePerButton = EditorGUIUtility.currentViewWidth / columnCount - 15;
            for (int x = 0; x < rowCount; x++)
            {
                GUILayout.BeginHorizontal();
                for (int y = 0; y < columnCount; y++)
                {
                    int index = indexLookup[x, y];
                    if (index == -1)
                    {
                        continue;
                    }
                    GUILayout.BeginVertical();

                    string labelText = index.ToString();

                    if (!autoTile.IsElementValid(index))
                    {
                        GUI.color = new Color(1, 0.9f, 0.5f);
                        labelText = string.Format("{0}: Empty", labelText);
                    }
//                    autoTile.tileOrPrefab[index] = GUILayout.Toggle(autoTile.tileOrPrefab[index], "T/P");

                    if (GUILayout.Button(GUIContent.none, TileMapGUIStyles.centerWhiteBoldLabel, GUILayout.Width(sizePerButton), GUILayout.Height(sizePerButton)))
                    {
//                        if (autoTile.tileOrPrefab[index])
//                        {
//                            RandomTile tile = autoTile.bitmaskRandomTiles[index] ? autoTile.bitmaskRandomTiles[index] : null;
//                            EditorGUIUtility.ShowObjectPicker<RandomTile>(tile, false, "", index);
//                        }
//                        else
//                        {
                            GameObject prefab = autoTile.bitmaskPrefabs[index] ? autoTile.bitmaskPrefabs[index] : null;
                            EditorGUIUtility.ShowObjectPicker<GameObject>(prefab, false, "", index);
//                        }
                    }
                    Rect r = GUILayoutUtility.GetLastRect();
                    if (GUILayout.Button("Ping"))
                    {
                        Type pt = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ProjectBrowser");
                        EditorWindow.GetWindow(pt).Show();
//                        if (autoTile.tileOrPrefab[index])
//                        {
//                            RandomTile tile = autoTile.bitmaskRandomTiles[index] ? autoTile.bitmaskRandomTiles[index] : null;
//                            EditorGUIUtility.PingObject(tile);
//                        }
//                        else
//                        {
                            GameObject prefab = autoTile.bitmaskPrefabs[index] ? autoTile.bitmaskPrefabs[index] : null;
                            EditorGUIUtility.PingObject(prefab);
//                        }
                    }
                    if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                    {
                        if (r.Contains(Event.current.mousePosition))
                        {
                            UnityEngine.Object dragItem = DragAndDrop.objectReferences[0];
                            bool droppable = false;
//                            if (autoTile.tileOrPrefab[index])
//                            {
//                                droppable = dragItem is RandomTile;
//                            }
//                            else
//                            {
                                droppable = dragItem is GameObject;
//                            }
                            if (droppable)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                if (Event.current.type == EventType.DragPerform)
                                {
                                    DragAndDrop.AcceptDrag();
									autoTile.bitmaskPrefabs[index] = dragItem as GameObject;

                                }
                            }
                        }
                    }

                    Texture2D texture = autoTile.GetPreview(index);
                    if (texture == null)
                    {
                        texture = new Texture2D(16, 16);
                    }
                    GUI.DrawTexture(r, texture);
                    GUI.color = Color.white;

                    GUIStyle labelStyle = new GUIStyle(TileMapGUIStyles.centerWhiteBoldLabel);

                    GUI.Label(r, labelText, labelStyle);
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            int controlID = EditorGUIUtility.GetObjectPickerControlID();
            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
//                if (autoTile.tileOrPrefab[controlID])
//                {
//                    autoTile.bitmaskRandomTiles[controlID] = EditorGUIUtility.GetObjectPickerObject() as RandomTile;
//                }
//                else
//                {
                    autoTile.bitmaskPrefabs[controlID] = EditorGUIUtility.GetObjectPickerObject() as GameObject;
//                }
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(autoTile);
            }
        }
    }
}
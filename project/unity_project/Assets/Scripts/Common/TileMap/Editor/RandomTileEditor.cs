using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Universal.TileMapping
{
    [CustomEditor(typeof(RandomTile))]
    public class RandomTileEditor : ScriptableTileEditor
    {
        private RandomTile randomTile;

        private void OnEnable()
        {
            randomTile = (RandomTile)target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("This tile randomly chooses a prefab from the list below to render.", MessageType.Info);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Prefabs:", TileMapGUIStyles.leftBoldLabel);
            GUILayout.FlexibleSpace();
            GUI.color = new Color(0.5f, 1, 0.5f);
            if (GUILayout.Button("Add New"))
            {
                randomTile.prefabs.Add(null);
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            float width = EditorGUIUtility.labelWidth / 3;
            for (int i = 0; i < randomTile.prefabs.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (!randomTile.IsValid)
                {
                    GUI.color = new Color(1, 0.5f, 0.5f);
                }
                if (GUILayout.Button(GUIContent.none, TileMapGUIStyles.centerWhiteBoldLabel, GUILayout.Width(width), GUILayout.Height(width)))
                {
                    EditorGUIUtility.ShowObjectPicker<GameObject>(randomTile.prefabs[i], false, "", i);
                }
                Rect r = GUILayoutUtility.GetLastRect();
                if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                {
                    if (r.Contains(Event.current.mousePosition))
                    {
                        UnityEngine.Object dragItem = DragAndDrop.objectReferences[0];
                        bool droppable = dragItem is GameObject;
                        if (droppable)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            if (Event.current.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();
                                randomTile.prefabs[i] = dragItem as GameObject;
                            }
                        }
                    }
                }

                Texture2D texture = randomTile.GetPreview(i);
                if (texture == null)
                {
                    new Texture2D(16, 16);
                }
                GUI.DrawTexture(r, texture);
                GUI.color = Color.white;

                GUIStyle labelStyle = new GUIStyle(TileMapGUIStyles.centerWhiteBoldLabel);
                if (!randomTile.prefabs[i])
                    GUI.Label(r, "Tile not valid!\nPrefab cannot be left empty", labelStyle);

                GUILayout.FlexibleSpace();
                GUI.color = new Color(1, 0.5f, 0.5f);
                if (GUILayout.Button("Delete"))
                {
                    randomTile.prefabs.RemoveAt(i);
                }
                GUI.color = Color.white;
                if (GUILayout.Button("Ping"))
                {
                    Type pt = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ProjectBrowser");
                    EditorWindow.GetWindow(pt).Show();
                    EditorGUIUtility.PingObject(randomTile.prefabs[i]);
                }
                GUILayout.EndHorizontal();
                TileMapGUILayout.Splitter();
            }
            
            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                int index = EditorGUIUtility.GetObjectPickerControlID();
                randomTile.prefabs[index] = EditorGUIUtility.GetObjectPickerObject() as GameObject;
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(randomTile);
            }
        }
    }
}
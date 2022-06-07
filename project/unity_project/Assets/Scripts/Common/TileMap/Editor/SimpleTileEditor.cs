using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Universal.TileMapping
{
    [CustomEditor(typeof(SimpleTile))]
    public class TileCustomEditor : ScriptableTileEditor
    {
        private SimpleTile tile;

        private void OnEnable()
        {
            tile = (SimpleTile)target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("As simple as it comes, renders the prefab selected", MessageType.Info);
            GUILayout.Space(10);

            GUILayout.Label("Sprite:", TileMapGUIStyles.leftBoldLabel);

            float width = EditorGUIUtility.labelWidth;
            Texture2D texture = null;
            if (!tile.IsValid)
            {
                GUI.color = new Color(1, 0.5f, 0.5f);
            }
            else
            {
                texture = tile.GetPreview();
            }
            if (texture == null)
            {
                texture = new Texture2D(16, 16);
            }
            if (GUILayout.Button(GUIContent.none, TileMapGUIStyles.centerWhiteBoldLabel, GUILayout.Width(width), GUILayout.Height(width)))
            {
                EditorGUIUtility.ShowObjectPicker<GameObject>(tile.prefab, false, "", 0);
                Type pt = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ProjectBrowser");
                EditorWindow.GetWindow(pt).Show();
                EditorGUIUtility.PingObject(tile);
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
                            tile.prefab = dragItem as GameObject;
                        }
                    }
                }
            }

            GUI.DrawTexture(r, texture);
            GUI.color = Color.white;
            if (GUILayout.Button("Ping", GUILayout.Width(width)))
            {
                Type pt = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ProjectBrowser");
                EditorWindow.GetWindow(pt).Show();
                EditorGUIUtility.PingObject(tile.prefab);
            }

            GUIStyle labelStyle = new GUIStyle(TileMapGUIStyles.centerWhiteBoldLabel);
            if (!tile.IsValid)
                GUI.Label(r, "Tile not valid!\nGameObject cannot be left empty", labelStyle);

            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                tile.prefab = EditorGUIUtility.GetObjectPickerObject() as GameObject;
                EditorUtility.SetDirty(tile);
            }
        }
    }
}
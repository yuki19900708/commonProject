using UnityEngine;
using UnityEditor;

namespace Universal.TileMapping
{
    [CustomEditor(typeof(TileMap)), CanEditMultipleObjects]
    partial class TileMapEditor : Editor
    {
        [MenuItem("GameObject/2D Object/TileMap")]
        private static void CreateTileMapGameObject()
        {
            new GameObject("New TileMap", typeof(TileMap));
        }

        private TileMap tileMap;

        partial void OnInspectorEnable();
        partial void OnInspectorDisable();
        partial void OnSceneEnable();
        partial void OnSceneDisable();

        private void OnEnable()
        {
            tileMap = (TileMap)target;

            OnInspectorEnable();
            OnSceneEnable();
        }

        private void OnDisable()
        {
            OnInspectorDisable();
            OnSceneDisable();
        }

        [MenuItem("Custom/OpenFolder")]
        public static void OpenFolder()
        {
            System.Diagnostics.Process.Start(Application.persistentDataPath);
        }
    }

}
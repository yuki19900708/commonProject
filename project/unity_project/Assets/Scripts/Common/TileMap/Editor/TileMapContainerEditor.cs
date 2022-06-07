using UnityEditor;
using UnityEngine;

namespace Universal.TileMapping
{
    [CustomEditor(typeof(TileMapContainer))]
    public class TileMapContainerEditor : Editor
    {
        TileMapContainer container;

        public override void OnInspectorGUI()
        {
            container = (target as TileMapContainer);
            EditorGUILayout.HelpBox("This is just a container of tilemap data", MessageType.Info);

            TileMapGUILayout.Splitter();
            if (GUILayout.Button("Import as New TileMap"))
            {
                GameObject go = new GameObject(container.name);
                TileMap map = go.AddComponent<TileMap>();
                map.ResizeGrid(container.gridSize);
                map.ResizeIso(container.isoWidth, container.isoHeight);
                map.ResizeMap(container.mapWidth, container.mapHeight);
                map.ChangeFreestyleRotation(container.xRotation, container.yRotation);
                map.ResizeHexGrid(container.outerRadius);
                map.ResizeHexOrientation(container.orientation);
                for (int x = 0; x < container.mapWidth; x++)
                {
                    for (int y = 0; y < container.mapHeight; y++)
                    {
                        map.SetTileAt(x, y, container.map[x + y * container.mapWidth]);
                    }
                }
                map.ChangeLayout(container.mapLayout);
            }
        }
    }
}
using UnityEngine;
using System;

namespace Universal.TileMapping
{
    [AddComponentMenu("2D/Renderer/TileGameObjectRenderer")]
    public class TileGameObjectRenderer : TileRenderer
    {
        [SerializeField]
        private MapObject[] gameObjectMap = new MapObject[0];

        public override void Resize(int width, int height)
        {
            if (width * height == gameObjectMap.Length)
                return;

            ClearChildren();

            gameObjectMap = new MapObject[width * height];
        }

        public override void UpdateTileAt(int x, int y)
        {
            int index = x + y * tileMap.MapWidth;
            MapObject current = gameObjectMap[index];
            if (current != null)
            {
                int tmpIndex = current.gridIndex;
                if(tmpIndex == index)
                {
                    DestroyImmediate(current);
                    gameObjectMap[index] = null;
                }
            }
            Point point = new Point(x, y);
            ScriptableTile tile = tileMap.GetTileAt(x, y);
            GameObject prefab = tile ? tile.GetTile(tileMap, point) : null;

            if (prefab != null)
            {
				current = Instantiate(prefab).GetComponent<MapObject>();
                if(current == null)
                {
                    Debug.LogError(prefab);
                    return;
                }
                current.name = string.Format("[{0},{1}]_{2}", x, y, prefab.name);
                current.transform.SetParent(parent);

                int[] area = current.GetArea();
                current.gridIndex = index;
                for (int i = 0; i < area[0]; i++)
                {
                    for (int j = 0; j < area[1]; j++)
                    {
                        index = (x + i) + (y + j) * tileMap.MapWidth;
                        gameObjectMap[index] = current;
                    }
                }
                
                Vector3 offset = new Vector3((area[0] - 1) * tileMap.GridSize * 0.5f, (area[1] - 1) * tileMap.GridSize * 0.5f, 0);
                current.transform.localPosition = tileMap.Coordinate2WorldPosition(x, y) + offset;
              
            }
        }

        public override void CompleteReset()
        {
            foreach (MapObject go in gameObjectMap)
            {
                if (go != null)
                {
                    Destroy(go);
                }
            }
        }

        public MapObject GetMapObject(int x, int y)
        {
            int index = x + y * tileMap.MapWidth;
            if (index < 0 || index >= gameObjectMap.Length)
            {
                return null;
            }

            return gameObjectMap[index];
        }
    }
}
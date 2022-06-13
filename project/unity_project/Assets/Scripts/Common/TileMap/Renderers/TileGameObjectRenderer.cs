using UnityEngine;
using System;

namespace Universal.TileMapping
{
    [AddComponentMenu("2D/Renderer/TileGameObjectRenderer")]
    public class TileGameObjectRenderer : TileRenderer
    {
        //public event Action<int, int, GameObject> OnRenderTile;
//        public event Func<int, int, GameObject, GameObject> OverrideCreateInstance;
        //public event Action<int, int, GameObject> OverrideDestoryInstance;
        //public event Action<int, int, SortingOrderTag, Renderer> OverrideReOrder;

        [SerializeField]
        private GameObject[] gameObjectMap = new GameObject[0];

        public override void Resize(int width, int height)
        {
            if (width * height == gameObjectMap.Length)
                return;

            ClearChildren();

            gameObjectMap = new GameObject[width * height];
        }

        public override void UpdateTileAt(int x, int y)
        {
            int index = x + y * tileMap.MapWidth;
            GameObject current = gameObjectMap[index];
            if (current != null)
            {
                DestroyImmediate(current);

                gameObjectMap[index] = null;
                current = null;
            }
            Point point = new Point(x, y);
            ScriptableTile tile = tileMap.GetTileAt(x, y);
            GameObject prefab = tile ? tile.GetTile(tileMap, point) : null;

            if (prefab != null)
            {
				current = Instantiate(prefab);


                ITileIndexSetter tileIndexSetter = current.GetComponent<ITileIndexSetter>();
                if (tileIndexSetter != null)
                {
                    tileIndexSetter.SetTileIndex(tile.GetTileIndex(tileMap, point));
                }

                current.name = string.Format("[{0},{1}]_{2}", x, y, prefab.name);
                current.transform.SetParent(parent);

                int[] area = current.GetComponent<MapObject>().GetArea();
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
            foreach (GameObject go in gameObjectMap)
            {
                if (go != null)
                {
                    Destroy(go);
                }
            }
        }

        public GameObject GetTileGameObject(int x, int y)
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
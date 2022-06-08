using UnityEngine;
using System;

namespace Universal.TileMapping
{
    [AddComponentMenu("2D/Renderer/TileGameObjectRenderer")]
    public class TileGameObjectRenderer : TileRenderer
    {
        public event Action<int, int, GameObject> OnRenderTile;
//        public event Func<int, int, GameObject, GameObject> OverrideCreateInstance;
        public event Action<int, int, GameObject> OverrideDestoryInstance;
        public event Action<int, int, SortingOrderTag, Renderer> OverrideReOrder;

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
                if (OverrideDestoryInstance != null)
                {
                    OverrideDestoryInstance(x, y, current);
                }
                else
                {
                    DestroyImmediate(current);
                }
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

                CheckSoringOrderTag(current);
				Debug.LogError ( current.name);
                current.name = string.Format("[{0},{1}]_{2}", x, y, prefab.name);
                current.transform.SetParent(parent);
                gameObjectMap[index] = current;
                current.transform.localPosition = tileMap.Coordinate2WorldPosition(x, y);
                if (autoPivot)
                {
                    //pivot change will affect the sprite transform position, because tile map system is base on 
                    //fixed pivot, so the keypoint is calculate the offset between actual pivot and bottomCenter
                    Vector2 pivotTileMap;
                    if (tileMap.MapLayout == TileMap.Layout.CartesianCoordinate)
                    {
                        pivotTileMap = new Vector2(0f, 0f);
                    }
                    else if (tileMap.MapLayout == TileMap.Layout.Hexagonal)
                    {
                        pivotTileMap = new Vector2(0.5f, 0.5f);
                    }
                    else if (tileMap.MapLayout == TileMap.Layout.IsometricDiamondFreestyle)
                    {
                        pivotTileMap = tileMap.FreestylePivot;
                    }
                    else
                    {
                        pivotTileMap = new Vector2(0.5f, 0);
                    }
                    Vector2 pivotActual = new Vector2(0.5f, 0.5f);
                    SpriteRenderer sr = current.transform.GetComponentInChildren<SpriteRenderer>();
                    if (sr != null && sr.sprite != null)
                    {
                        pivotActual = new Vector2(sr.sprite.pivot.x / sr.sprite.rect.width, sr.sprite.pivot.y / sr.sprite.rect.height);
                    }
                    Vector2 pivotOffset = pivotActual - pivotTileMap;
                    //Then translate the pivotOffset's world position to tilemap coordinate
                    Vector3 offset = Vector3.zero;
                    if (tileMap.MapLayout == TileMap.Layout.CartesianCoordinate)
                    {
                        offset = new Vector3(pivotOffset.x * tileMap.GridSize, pivotOffset.y * tileMap.GridSize);
                    }
                    else if (tileMap.MapLayout == TileMap.Layout.Hexagonal)
                    {
                        if (tileMap.Orientation == TileMap.HexOrientation.PointySideUp)
                        {
                            offset = new Vector3(pivotOffset.x * tileMap.InnerRadius * 2, pivotOffset.y * tileMap.OuterRadius * 2);
                        }
                        else
                        {
                            offset = new Vector3(pivotOffset.x * tileMap.OuterRadius * 2, pivotOffset.y * tileMap.InnerRadius * 2);
                        }
                    }
                    else if (tileMap.MapLayout == TileMap.Layout.IsometricDiamondFreestyle)
                    {
                        offset = new Vector3(pivotOffset.x * tileMap.FreestyleWidth, pivotOffset.y * tileMap.FreestyleHeight);
                    }
                    else
                    {
                        offset = new Vector3(pivotOffset.x * tileMap.IsoWidth, pivotOffset.y * tileMap.IsoHeight);
                    }
                    current.transform.localPosition = tileMap.Coordinate2LocalPosition(x, y) + offset;
                }
                else
                {
                    current.transform.localPosition = tileMap.Coordinate2LocalPosition(x, y);
                }

                //current.transform.localScale = Vector2.one;
                ReOrder(x, y, current);
            }

            if (OnRenderTile != null)
            {
                OnRenderTile(x, y, current);
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

        public GameObject GetTileGameObject(Point point)
        {
            return GetTileGameObject(point.x, point.y);
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

        public void ChangeTileMap(int x, int y, GameObject go)
        {
            int index = x + y * tileMap.MapWidth;
            gameObjectMap[index] = go;
            if (go != null)
            {
                go.transform.SetParent(parent);
            }
        }

        public void SetSortingMethodOrder(GameObject go, string sortingLayerName)
        {
            if (sortingMethod == SortingMethod.SortingLayer)
            {
                SpriteRenderer[] srs = go.GetComponentsInChildren<SpriteRenderer>(true);
                foreach (SpriteRenderer sr in srs)
                {
                    sr.sortingLayerName = sortingLayerName;
                }
            }
        }

        public float GetSortingOrderValue(int x, int y, GameObject go)
        {
            if (sortingMethod == SortingMethod.SortingLayer)
            {
                SortingOrderTag tag = go.GetComponent<SortingOrderTag>();
                int sortingOrder = tag.sortingOrder + (orderInLayer - y - x) * orderDelta;
                return sortingOrder;
            }
            else
            {
                SortingOrderTag tag = go.GetComponent<SortingOrderTag>();
                float sortingOrder = tag.zOrder + (y * tileMap.MapWidth + x) * zDepthDelta;
                return sortingOrder;
            }
        }

        public void ReOrder(int x, int y, GameObject go)
        {
            if (sortingMethod == SortingMethod.SortingLayer)
            {
                Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    SortingOrderTag tag = renderer.GetComponent<SortingOrderTag>();
                    //生成时在该物体中加入动态粒子时，Tag为
                    if (tag == null)
                    {
                        continue;
                    }
                    int sortingOrder = tag.sortingOrder + (orderInLayer - y - x) * orderDelta;
                    renderer.sortingOrder = sortingOrder;
                    renderer.sortingLayerID = sortingLayer;
                    if (OverrideReOrder != null)
                    {
                         OverrideReOrder(x, y, tag, renderer);
                    }
                }
            }
            else if (sortingMethod == SortingMethod.ZDepth)
            {
                Transform[] transforms = go.GetComponentsInChildren<Transform>(true);
                foreach (Transform transform in transforms)
                {
                    SortingOrderTag tag = transform.GetComponent<SortingOrderTag>();
                    Vector3 position = transform.localPosition;
                    position.z = tag.zOrder + (y * tileMap.MapWidth + x) * zDepthDelta;
                    go.transform.localPosition = position;
                }
            }
        }

        private void CheckSoringOrderTag(GameObject go)
        {
            if (sortingMethod == SortingMethod.SortingLayer)
            {
                Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    SortingOrderTag tag = renderer.GetComponent<SortingOrderTag>();
                    if (tag == null)
                    {
                        tag = renderer.gameObject.AddComponent<SortingOrderTag>();
                        tag.sortingOrder = renderer.sortingOrder;
                        tag.zOrder = renderer.transform.localPosition.z;
                    }
                }
            }
            else if (sortingMethod == SortingMethod.ZDepth)
            {
                Transform[] transforms = go.GetComponentsInChildren<Transform>(true);
                foreach (Transform transform in transforms)
                {
                    SortingOrderTag tag = transform.GetComponent<SortingOrderTag>();
                    if (tag != null)
                    {
                        return;
                    }
                    else
                    {
                        tag = transform.gameObject.AddComponent<SortingOrderTag>();
                        tag.zOrder = transform.localPosition.z;
                    }
                }
            }
        }
    }
}
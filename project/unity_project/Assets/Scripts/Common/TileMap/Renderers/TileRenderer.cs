using UnityEngine;

namespace Universal.TileMapping
{
    [ExecuteInEditMode, DisallowMultipleComponent, RequireComponent(typeof(TileMap))]
    public abstract class TileRenderer : MonoBehaviour
    {
        public enum SortingMethod
        {
            None,
            SortingLayer,
            ZDepth
        }

        public Color color;
        private bool changeColor = false;

        public bool ChangeColor
        {
            get
            {
                return changeColor;
            }
        }
        public SortingMethod sortingMethod = SortingMethod.SortingLayer;
        public int sortingLayer;
        public int orderInLayer;
        public int orderDelta = 10; //层级之间的间隔，当一个物体由多个元素构成时，需要拉开层级差，这样避免物体的某些部分的层次超过其前后物体层级
        public float zDepthDelta = 0.1f;

        public bool autoPivot = true;
        public Vector3 parentOffset = Vector3.zero;

        [SerializeField]
        protected TileMap tileMap;
        [SerializeField]
        protected Transform parent;

        public virtual void Start()
        {
            //RefreshMap();
        }

        public virtual void Reset()
        {
            if (!tileMap)
                tileMap = GetComponent<TileMap>();

            color = Color.white;

            parent = transform.Find("TileRenderer");
            if (parent == null)
            {
                parent = new GameObject("TileRenderer").GetComponent<Transform>();
                parent.SetParent(transform);
                parent.localPosition = Vector3.zero;
                parent.localScale = Vector3.one;
            }

            Resize(tileMap.MapWidth, tileMap.MapHeight);
            RefreshMap();
        }

        public virtual void OnEnable()
        {
            if (!tileMap)
                tileMap = GetComponent<TileMap>();

            tileMap.OnUpdateTileAt += UpdateTileAt;
            tileMap.OnUpdateTileMap += RefreshMap;
            tileMap.OnResize += Resize;
            tileMap.OnCompleteReset += CompleteReset;
        }

        public virtual void OnDisable()
        {
            tileMap.OnUpdateTileAt -= UpdateTileAt;
            tileMap.OnUpdateTileMap -= RefreshMap;
            tileMap.OnResize -= Resize;
            tileMap.OnCompleteReset -= CompleteReset;
        }

        public void ClearChildren()
        {
            if (parent == null)
            {
                parent = transform.Find("TileRenderer");
            }
            if (parent.childCount > 0)
            {
                if (!Application.isPlaying)
                {
                    while (parent.childCount > 0)
                    {
                        Object.DestroyImmediate(parent.GetChild(0).gameObject);
                    }
                }
                else
                {
                    foreach (Transform child in parent)
                    {
                        Object.Destroy(child.gameObject);
                    }
                }
            }
        }
        public void OnDestroy()
        {
            ClearChildren();
        }
        public abstract void Resize(int width, int height);

        public virtual void RefreshMap()
        {
            for (int x = 0; x < tileMap.MapWidth; x++)
            {
                for (int y = 0; y < tileMap.MapHeight; y++)
                {
                    UpdateTileAt(x, y);
                }
            }
            if (parent != null)
            {
                parent.localPosition = parentOffset;
            }
        }

        public abstract void UpdateTileAt(int x, int y);
        public virtual void CompleteReset() { }

        public void SetChangeColor(bool result)
        {
            changeColor = result;
        }
    }
}
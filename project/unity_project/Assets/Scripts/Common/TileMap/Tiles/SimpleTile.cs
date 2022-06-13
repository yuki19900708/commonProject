#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Universal.TileMapping
{
    [CreateAssetMenu(fileName = "New SimpleTile", menuName = "Tilemap/Tiles/SimpleTile")]
    public class SimpleTile : ScriptableTile
    {
        public GameObject prefab;

        public override bool IsValid
        {
            get
            {
                return prefab != null;
            }
        }

        public override GameObject GetTile(TileMap tileMap = null, Point position = default(Point))
        {
            Debug.LogError("draw" + prefab.name);
            return prefab;
        }

        public override Texture2D GetPreview()
        {
            Texture2D texture = null;
            if (!IsValid)
            {
                texture = new Texture2D(16, 16);
            }
#if UNITY_EDITOR
            texture = AssetPreview.GetAssetPreview(prefab);
#endif
            if (texture == null)
            {
                texture = new Texture2D(16, 16);
            }
            return texture;
        }

        public override int GetTileIndex(TileMap tileMap = null, Point position = default(Point))
        {
            return 0;
        }
    }
}
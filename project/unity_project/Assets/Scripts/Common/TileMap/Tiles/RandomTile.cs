using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Universal.TileMapping
{
    //Remember to change these names to something more meaningful!
    [CreateAssetMenu(fileName = "New RandomTile", menuName = "Tilemap/Tiles/RandomTile")]
    public class RandomTile : ScriptableTile
    {
        static System.Random prng = new System.Random();
        public List<GameObject> prefabs = new List<GameObject>();

        //Returns if this tile is okay to be used in the tile map
        //For example: if this tile doesn't have a Read/Write enabled sprite it will return false
        public override bool IsValid
        {
            get
            {
                return prefabs != null && prefabs.Count > 0;
            }
        }

        public override GameObject GetTile(TileMap tilemap, Point position = default(Point))
        {
            //Get a pseudo random index based on position
            int index = GetTileIndex(tilemap, position);
            return prefabs[index];
        }

        public override int GetTileIndex(TileMap tilemap, Point position = default(Point))
        {
            return prng.Next(0, prefabs.Count);
        }

        public override Texture2D GetPreview()
        {
            Texture2D preview = GetPreview(0);
            if (preview == null)
            {
                for (int i = 0; i < prefabs.Count; i++)
                {
                    preview = GetPreview(i);
                    if (preview != null)
                    {
                        break;
                    }
                }
            }
            if (preview == null)
            {
                preview = new Texture2D(16, 16);
            }
            return preview;
        }

        public Texture2D GetPreview(int index)
        {
            Texture2D texture = null;
#if UNITY_EDITOR
            texture = AssetPreview.GetAssetPreview(prefabs[index]);
#endif
            return texture;
        }
    }
}

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Universal.TileMapping
{
    //Remember to change these names to something more meaningful!
    [CreateAssetMenu(fileName = "AutoTile", menuName = "Tilemap/Tiles/AutoTile")]
    public class AutoTile : ScriptableTile
    {
        public struct NeighbourMap
        {
            public int[] froms;
            public int to;
        }

        public GameObject[] bitmaskPrefabs = new GameObject[10];

        public enum AutoTileMode { Everything, SameTile, None }
        public AutoTileMode mode;

        public enum NeighbourMode { Four, Eight}
        public NeighbourMode neighbourMode;
     

        private ScriptableTile[] neighbourScriptableTiles = new ScriptableTile[8];

        private int[] bitmasks = new int[] {
                1, 2, 4, 8, 16, 32, 64, 128
            };

        //Returns if this tile is okay to be used in the tile map
        //For example: if this tile doesn't have a Read/Write enabled sprite it will return false
        public override bool IsValid
        {
            get
            {
                foreach(GameObject prefab in bitmaskPrefabs)
                {
                    if (prefab != null)
                    {
                        return true;
                    }
                }
        
                return false;
            }
        }

        public bool IsElementValid(int index)
        {
			if (bitmaskPrefabs == null) {
				return false;
			}

			if (bitmaskPrefabs.Length <= index) {
				return false;
			}
			if ( bitmaskPrefabs [index] == null) {
				return false;
			}
			return true;
        }

        public override GameObject GetTile(TileMap tilemap = null, Point position = default(Point))
        {
            if (tilemap == null)
                return bitmaskPrefabs[0];

            int tileIndex = GetTileIndex(tilemap, position);
			if(IsElementValid(tileIndex))
			{
				return bitmaskPrefabs[tileIndex];
			}
			return null;
        }

        public override int GetTileIndex(TileMap tilemap, Point position = default(Point))
        {
			return 0;
        }

        public override Texture2D GetPreview()
        {
            Texture2D preview = GetPreview(0);
            return preview;
        }

        public Texture2D GetPreview(int index)
        {
			Texture2D texture = null;

			if (bitmaskPrefabs!= null && bitmaskPrefabs.Length > index)
            {
				if (bitmaskPrefabs [index] != null) 
				{
					//texture=  AssetPreview.GetAssetPreview(bitmaskPrefabs[index]);

				}
            }

			if (texture == null)
			{
				texture = new Texture2D(16, 16);
			}
			return texture;
        }
	}
}

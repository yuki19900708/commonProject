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

        public GameObject[] bitmaskPrefabs = new GameObject[256];
        public RandomTile[] bitmaskRandomTiles = new RandomTile[256];
        public bool[] tileOrPrefab = new bool[256];

        public enum AutoTileMode { Everything, SameTile, None }
        public AutoTileMode mode;

        public enum NeighbourMode { Four, Eight}
        public NeighbourMode neighbourMode;
        
        public NeighbourMap[] neighbourMaps = new NeighbourMap[] {
            new NeighbourMap{ froms = new int[] { 45, 29, 13 }, to = 61 },
            new NeighbourMap{ froms = new int[] { 75, 43, 11 }, to = 107 },
            new NeighbourMap{ froms = new int[] { 105, 89, 73, 57, 41, 25, 9 }, to = 121 },
            new NeighbourMap{ froms = new int[] { 91, 59, 27 }, to = 123 },
            new NeighbourMap{ froms = new int[] { 109, 93, 77 }, to = 125 },
            new NeighbourMap{ froms = new int[] { 142, 30, 14 }, to = 158 },
            new NeighbourMap{ froms = new int[] { 172, 156, 140, 60, 44, 28, 12 }, to = 188 },
            new NeighbourMap{ froms = new int[] { 173, 157, 141 }, to = 189 },
            new NeighbourMap{ froms = new int[] { 174, 62, 46 }, to = 190 },
            new NeighbourMap{ froms = new int[] { 135, 71, 7 }, to = 199 },
            new NeighbourMap{ froms = new int[] { 198, 150, 134, 86, 70, 22, 6 }, to = 214 },
            new NeighbourMap{ froms = new int[] { 151, 87, 23 }, to = 215 },
            new NeighbourMap{ froms = new int[] { 206, 94, 78 }, to = 222 },
            new NeighbourMap{ froms = new int[] { 195, 163, 131, 99, 67, 35, 3 }, to = 227 },
            new NeighbourMap{ froms = new int[] { 167, 103, 39 }, to = 231 },
            new NeighbourMap{ froms = new int[] { 203, 171, 139 }, to = 235 },
            new NeighbourMap{ froms = new int[] { 224, 208, 192, 176, 160, 144, 128, 112, 96, 80, 64, 48, 32, 16, 0 }, to = 240 },
            new NeighbourMap{ froms = new int[] { 225, 209, 193, 177, 161, 145, 129, 113, 97, 81, 65, 49, 33, 17, 1}, to = 241 },
            new NeighbourMap{ froms = new int[] { 226, 210, 194, 178, 162, 146, 130, 114, 98, 82, 66, 50, 34, 18, 2 }, to = 242 },
            new NeighbourMap{ froms = new int[] { 211, 179, 147, 115, 83, 51, 19 }, to = 243 },
            new NeighbourMap{ froms = new int[] { 228, 212, 196, 180, 164, 148, 132, 116, 100, 84, 68, 52, 36, 20, 4 }, to = 244 },
            new NeighbourMap{ froms = new int[] { 229, 213, 197, 181, 165, 149, 133, 117, 101, 85, 69, 53, 37, 21, 5 }, to = 245 },
            new NeighbourMap{ froms = new int[] { 230, 182, 166, 118, 102, 54, 38 }, to = 246 },
            new NeighbourMap{ froms = new int[] { 183, 119, 55 }, to = 247 },
            new NeighbourMap{ froms = new int[] { 232, 216, 200, 184, 168, 152, 136, 120, 104, 88, 72, 56, 40, 24, 8}, to = 248 },
            new NeighbourMap{ froms = new int[] { 233, 217, 201, 185, 169, 153, 137 }, to = 249 },
            new NeighbourMap{ froms = new int[] { 234, 218, 202, 186, 170, 154, 138, 122, 106, 90, 74, 58, 42, 26, 10 }, to = 250 },
            new NeighbourMap{ froms = new int[] { 219, 187, 155 }, to = 251 },
            new NeighbourMap{ froms = new int[] { 236, 220, 204, 124, 108, 92, 76 }, to = 252 },
            new NeighbourMap{ froms = new int[] { 237, 221, 205 }, to = 253 },
            new NeighbourMap{ froms = new int[] { 238, 126, 110 }, to = 254 }
        };

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
                foreach (RandomTile tile in bitmaskRandomTiles)
                {
                    if (tile != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsElementValid(int index)
        {
            return (tileOrPrefab[index] == false && bitmaskPrefabs != null && bitmaskPrefabs[index]) ||
                (tileOrPrefab[index] && bitmaskRandomTiles != null && bitmaskRandomTiles[index]);
        }

        public override GameObject GetTile(TileMap tilemap = null, Point position = default(Point))
        {
            if (tilemap == null)
                return bitmaskPrefabs[15];

            int tileIndex = GetTileIndex(tilemap, position);
            
            if (neighbourMode == NeighbourMode.Four)
            {
                if (bitmaskPrefabs[tileIndex] || tileOrPrefab[tileIndex])
                {
                    if (tileOrPrefab[tileIndex])
                    {
                        return bitmaskRandomTiles[tileIndex].GetTile(tilemap, position);
                    }
                    else
                    {
                        return bitmaskPrefabs[tileIndex];
                    }
                }
                else
                {
                    //Debug.LogWarning(string.Format("AutoTile : {0} missing bitmask4 : {1}", this.name, tileIndex));
                    return null;
                }
            }
            else if (neighbourMode == NeighbourMode.Eight)
            {
                if (bitmaskPrefabs[tileIndex] || tileOrPrefab[tileIndex])
                {
                    if (tileOrPrefab[tileIndex])
                    {
                        return bitmaskRandomTiles[tileIndex].GetTile(tilemap, position); ;
                    }
                    else
                    {
                        return bitmaskPrefabs[tileIndex];
                    }
                }
                else
                {
                    //如果正常的eightIndex没找到，就将其映射一次后，再查找，因为很多形状是重复的，所以这些重复的形状的Index就做了一个映射表
                    int mapEightIndex = GetMapIndex(tileIndex);
                    if (bitmaskPrefabs[mapEightIndex] || tileOrPrefab[mapEightIndex])
                    {
                        if (tileOrPrefab[mapEightIndex])
                        {
                            return bitmaskRandomTiles[mapEightIndex].GetTile(tilemap, position); ;
                        }
                        else
                        {
                            return bitmaskPrefabs[mapEightIndex];
                        }
                    }
                    else
                    {
                        //Debug.LogWarning(string.Format("AutoTile : {0} missing bitmask8 : {1} bitmask8Map : {2}", this.name, tileIndex, mapEightIndex));
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public override int GetTileIndex(TileMap tilemap, Point position = default(Point))
        {
            neighbourScriptableTiles[0] = tilemap.GetTileAt(position.x, position.y - 1);
            neighbourScriptableTiles[1] = tilemap.GetTileAt(position.x + 1, position.y);
            neighbourScriptableTiles[2] = tilemap.GetTileAt(position.x, position.y + 1);
            neighbourScriptableTiles[3] = tilemap.GetTileAt(position.x - 1, position.y);
            neighbourScriptableTiles[4] = tilemap.GetTileAt(position.x + 1, position.y - 1);
            neighbourScriptableTiles[5] = tilemap.GetTileAt(position.x + 1, position.y + 1);
            neighbourScriptableTiles[6] = tilemap.GetTileAt(position.x - 1, position.y + 1);
            neighbourScriptableTiles[7] = tilemap.GetTileAt(position.x - 1, position.y - 1);

            int fourIndex = 0;
            int eightIndex = 0;
            bool isCurrentOnEdge = tilemap.IsEdge(position);
            if (isCurrentOnEdge)
            {
                for (int i = 0; i < neighbourScriptableTiles.Length; i++)
                {
                    bool exists = neighbourScriptableTiles[i] != null;
                    bool isSame = exists && neighbourScriptableTiles[i].ID == this.ID;

                    if ((exists && mode == AutoTileMode.Everything) || (exists && (mode == AutoTileMode.SameTile && isSame)))
                    {
                        if (i < 4)
                        {
                            fourIndex += bitmasks[i];
                        }
                        eightIndex += bitmasks[i];
                    }
                }
            }
            else
            {
                fourIndex = 15;
                eightIndex = 255;
            }

            if (neighbourMode == NeighbourMode.Four)
            {
                return fourIndex;
            }
            else if (neighbourMode == NeighbourMode.Eight)
            {
                return eightIndex;
            }
            return -1;
        }

        public override Texture2D GetPreview()
        {
            Texture2D preview = GetPreview(15);
            if (preview == null)
            {
                for(int i = 0; i < bitmaskPrefabs.Length; i++)
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
            if (tileOrPrefab[index])
            {
                if (bitmaskRandomTiles[index])
                {
                    texture = bitmaskRandomTiles[index].GetPreview();
                }
            }
            else
            {
#if UNITY_EDITOR
                texture = AssetPreview.GetAssetPreview(bitmaskPrefabs[index]);
#endif
            }
            return texture;
        }

        /// <summary>
        /// 获取from映射的Index，注意此处的Map是映射的意思，不是地图
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        private int GetMapIndex(int from)
        {
            foreach(NeighbourMap map in neighbourMaps)
            {
                foreach(int fromValue in map.froms)
                {
                    if (fromValue == from)
                    {
                        return map.to;
                    }
                }
            }
            return -1;
        }
    }
}

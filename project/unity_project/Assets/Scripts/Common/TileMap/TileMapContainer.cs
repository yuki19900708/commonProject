using UnityEngine;

namespace Universal.TileMapping
{
    public class TileMapContainer : ScriptableObject
    {
        public int mapWidth;
        public int mapHeight;
        public float gridSize;
        public float isoWidth;
        public float isoHeight;
        public float xRotation;
        public float yRotation;
        public float outerRadius;
        public TileMap.HexOrientation orientation;
        public TileMap.Layout mapLayout;
        public ScriptableTile[] map;
    }
}

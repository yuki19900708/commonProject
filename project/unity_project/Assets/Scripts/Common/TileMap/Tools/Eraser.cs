using UnityEngine;
using System;

namespace Universal.TileMapping
{
    [Serializable]
    public class Eraser : Brush
    {
        public override KeyCode Shortcut { get { return KeyCode.E; } }
        public override string Description { get { return "Sets the painted tile to nothing"; } }

        public Eraser() : base()
        {
            radius = 1;
        }

        public override void OnClick(Point point, ScriptableTile tile, TileMap map)
        {
            base.OnClick(point, null, map);
        }
    }
}

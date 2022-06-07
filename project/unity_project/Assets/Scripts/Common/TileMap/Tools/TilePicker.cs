using UnityEngine;
using System;

namespace Universal.TileMapping
{
    [Serializable]
    public class TilePicker : ScriptableTool
    {
        public TilePicker() : base()
        {

        }
        public override KeyCode Shortcut { get { return KeyCode.T; } }
        public override string Description { get { return "Sets the primary tile to whatever you click"; } }

        public override void OnClickDown(Point point, ScriptableTile tile, TileMap map)
        {
        }
        public override void OnClick(Point point, ScriptableTile tile, TileMap map)
		{

		}	
        public override void OnClickUp(Point point, ScriptableTile tile, TileMap map)
        {
#if UNITY_EDITOR
            TileMap.pickedTile = map.GetTileAt(point);
#endif
        }	
    }
}

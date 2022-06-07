using UnityEngine;
using System;
using System.Collections.Generic;

namespace Universal.TileMapping
{
    [Serializable]
    public abstract class CustomShape : ScriptableTool
    {
        protected Point start;
        protected Point end;
        
        public CustomShape() : base()
        {
        }

        //Called when the left mouse button is held down
        public override void OnClick(Point point, ScriptableTile tile, TileMap map)
        {
            //Return if the tilemap is null/empty
            if (map == null)
                return;

            //If we haven't already started an operation, start one now
            //This is for undo/ redo support
            if (!map.OperationInProgress())
                map.BeginOperation();

            end = point;
            //Set the tile at the specified point to the specified tile
        }

        //Called when the left mouse button is initially held down
        public override void OnClickDown(Point point, ScriptableTile tile, TileMap map)
        {
            base.OnClickDown(point, tile, map);
            start = end = point;
        }

        public override void OnClickUp(Point point, ScriptableTile tile, TileMap map)
        {
            base.OnClickUp(point, tile, map);
            for (int i = 0; i < region.Count; i++)
            {
                map.SetTileAt(region[i], tile);
            }
            start = end = point;
            region = new List<Point>();
        }
    }
}
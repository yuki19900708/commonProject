﻿using UnityEngine;
using System;

namespace Universal.TileMapping
{
    [Serializable]
    public class Pencil : ScriptableTool
    {
        public override KeyCode Shortcut
        {
            get { return KeyCode.P; }
        }

        public override string Description
        {
            get { return "The simplest brush"; }
        }

        public Pencil() : base()
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

            //Set the tile at the specified point to the specified tile
            map.SetTileAt(point, tile);
        }

        //Called when the left mouse button is initially held down
        public override void OnClickDown(Point point, ScriptableTile tile, TileMap map)
        {
            OnClick(point, tile, map);
        }
    }
}

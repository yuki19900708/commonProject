using UnityEngine;
using System;
using System.Collections.Generic;

namespace Universal.TileMapping
{
    [Serializable]
    public class Rectangle : FilledShape
    {
        public override KeyCode Shortcut
        {
            get { return KeyCode.G; }
        }
        //Sets the tooltip description
        public override string Description
        {
            get { return "Draws a rectangle"; }
        }

        public Rectangle() : base()
        {

        }

        public override List<Point> GetRegion(Point point, ScriptableTile tile, TileMap map)
        {
            region = new List<Point>();
            if (end == start)
                return base.GetRegion(point, tile, map);

            int x0 = Mathf.Min(start.x, end.x),
                x1 = Mathf.Max(start.x, end.x),
                y0 = Mathf.Min(start.y, end.y),
                y1 = Mathf.Max(start.y, end.y);

            for (int x = x0; x <= x1; x++)
            {
                for (int y = y0; y <= y1; y++)
                {
                    if(filled || (x == x0 || x == x1 || y == y0 || y == y1))
                        region.Add(new Point(x, y));
                }
            }

            return region;
        }
    }
}
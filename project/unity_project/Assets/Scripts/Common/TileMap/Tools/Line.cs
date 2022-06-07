using UnityEngine;
using System;
using System.Collections.Generic;

namespace Universal.TileMapping
{
    [Serializable]
    public class Line : CustomShape
    {
        public override KeyCode Shortcut
        {
            get { return KeyCode.L; }
        }

        public override string Description
        {
            get { return "Draws a line"; }
        }

        public Line() : base()
        {

        }

        public override List<Point> GetRegion(Point point, ScriptableTile tile, TileMap map)
        {
            region = new List<Point>();
            if (end == start)
                return base.GetRegion(point, tile, map);

            int x0 = start.x,
                x1 = end.x,
                y0 = start.y,
                y1 = end.y;

            int w = x1 - x0;
            int h = y1 - y0;

            int dx0 = 0, dy0 = 0, dx1 = 0, dy1 = 0;

            if (w < 0) dx0 = -1;
            else if (w > 0) dx0 = 1;

            if (h < 0) dy0 = -1;
            else if (h > 0) dy0 = 1;

            if (w < 0) dx1 = -1;
            else if (w > 0) dx1 = 1;

            int longest = Mathf.Abs(w);
            int shortest = Mathf.Abs(h);
            if (!(longest > shortest))
            {
                longest = Mathf.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy1 = -1;
                else if (h > 0) dy1 = 1;
                dx1 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                region.Add(new Point(x0, y0));
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x0 += dx0;
                    y0 += dy0;
                }
                else
                {
                    x0 += dx1;
                    y0 += dy1;
                }
            }
            return region;
        }
    }
}
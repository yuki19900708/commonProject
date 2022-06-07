using UnityEngine;
using System;
using System.Collections.Generic;

namespace Universal.TileMapping
{
    [Serializable]
    public class Ellipse : FilledShape
    {
        public override KeyCode Shortcut
        {
            get { return KeyCode.W; }
        }
        
        public override string Description
        {
            get { return "Draws a rectangle"; }
        }
        
        public Ellipse() : base()
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

            int xc = Mathf.FloorToInt(x0 + (x1 - x0) / 2f);
            int yc = Mathf.FloorToInt(y0 + (y1 - y0) / 2f);
            int rx = x1 - xc;
            int ry = y1 - yc;


            int rxSq = rx * rx;
            int rySq = ry * ry;
            int x = 0, y = ry, p;
            int px = 0, py = 2 * rxSq * y;

            DrawQuadrants(xc, yc, x, y);

            //Region 1
            p = (int)(rySq - (rxSq * ry) + (0.25f * rxSq));
            while (px < py)
            {
                x++;
                px = px + 2 * rySq;
                if (p < 0)
                    p = p + rySq + px;
                else
                {
                    y--;
                    py = py - 2 * rxSq;
                    p = p + rySq + px - py;
                }
                DrawQuadrants(xc, yc, x, y);
            }

            //Region 2
            p = (int)(rySq * (x + 0.5f) * (x + 0.5f) + rxSq * (y - 1) * (y - 1) - rxSq * rySq);
            while (y > 0)
            {
                y--;
                py = py - 2 * rxSq;
                if (p > 0)
                    p = p + rxSq - py;
                else
                {
                    x++;
                    px = px + 2 * rySq;
                    p = p + rxSq - py + px;
                }
                DrawQuadrants(xc, yc, x, y);
            }

            return region;
        }

        void DrawQuadrants(int xc, int yc, int x, int y)
        {
            /*This function plots a pixel at coordinates(x,y) specified by first 2 arguments and third argument specifies the color of the pixel*/
            if (filled)
            {
                for (int xd = xc - x; xd <= xc + x; xd++)
                {
                    for (int yd = yc - y; yd <= yc + y; yd++)
                    {
                        Point p = new Point(xd, yd);
                        if (!region.Contains(p))
                            region.Add(p);
                    }
                }
            }
            else
            {
                region.Add(new Point(xc + x, yc + y));
                region.Add(new Point(xc - x, yc + y));
                region.Add(new Point(xc + x, yc - y));
                region.Add(new Point(xc - x, yc - y));
            }
        }
    }
}
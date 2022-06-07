using UnityEngine;

[System.Serializable]
public struct Point
{
	public static Point zero = new Point (0, 0);
	public static Point one = new Point (1, 1);

	public static Point north = new Point (0, 1);
	public static Point south = new Point (0, -1);
	public static Point west = new Point (-1, 0);
	public static Point east = new Point (1, 0);
    public static Point northEast = new Point(1, 1);
    public static Point northWest = new Point(-1, 1);
    public static Point southEast = new Point(1, -1);
    public static Point southWest = new Point(-1, -1);
    public static Point[] neighbour = new Point[] { south, east, north, west, southEast, northEast, northWest, southWest };

    public int x;
	public int y;

	public Point (Vector2 v)
	{
		this.x = Mathf.RoundToInt(v.x);
		this.y = Mathf.RoundToInt(v.y);
	}
	public Point (Vector3 v)
	{
		this.x = Mathf.RoundToInt(v.x);
		this.y = Mathf.RoundToInt(v.y);
	}
	public Point (Point p)
	{
		this.x = p.x;
		this.y = p.y;
	}
	public Point (int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Point Up { get { return this + north; }}
	public Point Down { get { return this + south; }}
	public Point Left { get { return this + west; }}
	public Point Right { get { return this + east; }}

    public Point UpLeft { get { return this + northWest; } }
    public Point UpRight { get { return this + northEast; } }
    public Point DownLeft { get { return this + southWest; } }
    public Point DownRight { get { return this + southEast; } }


    public Point[] Surrounded { get { return new Point[] { Left, Up, Right, Down }; } }

    public override string ToString ()
	{
		return "[" + x + "," + y + "]";
	}

    public bool CheckAdjacent(Point point)
    {
        return Mathf.Abs(point.x - this.x) == 1 || Mathf.Abs(point.y - this.y) == 1;
    }

	public override bool Equals (object obj)
	{
        if (obj is Point)
        {
            Point target = (Point)obj;
            return this.x == target.x && this.y == target.y;
        }
        return false;
	}
	public override int GetHashCode ()
	{
		return x.GetHashCode () ^ y.GetHashCode ();
	}

	public static explicit operator Point (Vector2 v)
	{
		return new Point (Mathf.FloorToInt (v.x), Mathf.FloorToInt (v.y));
	}
	public static explicit operator Point (Vector3 v)
	{
		return new Point (Mathf.FloorToInt(v.x), Mathf.FloorToInt (v.y));
	}
	public static explicit operator Vector2 (Point c)
	{
		return new Vector2 (c.x, c.y);
	}
	public static explicit operator Vector3 (Point c)
	{
		return new Vector3 (c.x, c.y);
	}
	public static Point operator + (Point a, Point b)
	{
		return new Point (a.x + b.x, a.y + b.y);
	}
	public static Point operator - (Point a, Point b)
	{
		return new Point (a.x - b.x, a.y - b.y);
	}
	public static Point operator * (Point a, int b)
	{
		return new Point (a.x * b, a.y * b);
	}

	public static bool operator == (Point a, Point b)
	{
		return a.x == b.x && a.y == b.y;
	}
	public static bool operator != (Point a, Point b)
	{
		return !(a == b);
	}
}
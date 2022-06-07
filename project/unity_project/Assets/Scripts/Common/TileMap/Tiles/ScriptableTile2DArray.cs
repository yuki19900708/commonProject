using System;
using UnityEngine;
using Universal.TileMapping;

[Serializable]
public class ScriptableTile2DArray
{
    [SerializeField]
    public ScriptableTile[] tileArray;

    [SerializeField]
    private int height;
    [SerializeField]
    private int width;

    public ScriptableTile this[int x, int y]
    {
        get
        {
            return tileArray[x + y * width];
        }
        set
        {
            tileArray[x + y * width] = value;
        }
    }

    public int Row
    {
        get
        {
            return height;
        }
    }

    public int ColumnCount
    {
        get
        {
            return width;
        }
    }

    public ScriptableTile2DArray(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.tileArray = new ScriptableTile[width * height];
    }
}

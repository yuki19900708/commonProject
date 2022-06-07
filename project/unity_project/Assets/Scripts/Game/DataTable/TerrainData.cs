using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class TerrainData
{
    /// <summary>地形编号</summary>
    public int index;
    /// <summary>地形占地</summary>
    public int[] area;


    public TerrainData Clone()
    {
        return (TerrainData)this.MemberwiseClone();
    }
}
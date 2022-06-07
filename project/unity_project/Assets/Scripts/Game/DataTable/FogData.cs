using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class FogData
{
    /// <summary>烟雾编号</summary>
    public int id;
    /// <summary>坐标</summary>
    public int[] pos;
    /// <summary>包含区域</summary>
    public int[] area;


    public FogData Clone()
    {
        return (FogData)this.MemberwiseClone();
    }
}
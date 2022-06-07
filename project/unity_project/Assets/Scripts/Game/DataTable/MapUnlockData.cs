using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class MapUnlockData
{
    /// <summary>迷雾编号</summary>
    public int id;
    /// <summary>解锁类型</summary>
    public int type;
    /// <summary>消费钻石</summary>
    public int consume;
    /// <summary>计数</summary>
    public int count;
    /// <summary>进度条坐标</summary>
    public int[] barPos;
    /// <summary>包含坐标</summary>
    public int[] area;


    public MapUnlockData Clone()
    {
        return (MapUnlockData)this.MemberwiseClone();
    }
}
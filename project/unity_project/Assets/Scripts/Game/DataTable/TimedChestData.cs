using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class TimedChestData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>类型</summary>
    public int type;
    /// <summary>权重</summary>
    public int weight;
    /// <summary>解锁时间</summary>
    public int unlockTime;
    /// <summary>快速解锁消耗</summary>
    public int skipNeedDiamond;
    /// <summary>产出物件</summary>
    public int outputIndex;


    public TimedChestData Clone()
    {
        return (TimedChestData)this.MemberwiseClone();
    }
}
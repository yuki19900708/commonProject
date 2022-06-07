using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class DestructEventData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>生命值</summary>
    public int hp;
    /// <summary>摧毁产出数量</summary>
    public int[] outputCount;
    /// <summary>产出列表</summary>
    public OutputData outputItems;
    /// <summary>额外产出数量</summary>
    public int[] extraOutputCount;
    /// <summary>额外产出列表</summary>
    public OutputData extraOutputItems;


    public DestructEventData Clone()
    {
        return (DestructEventData)this.MemberwiseClone();
    }
}
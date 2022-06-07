using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class LootData
{
    /// <summary>序号</summary>
    public int id;
    /// <summary>产出列表</summary>
    public OutputData outputItems;


    public LootData Clone()
    {
        return (LootData)this.MemberwiseClone();
    }
}
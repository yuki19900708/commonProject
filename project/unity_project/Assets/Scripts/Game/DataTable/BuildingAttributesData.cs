using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class BuildingAttributesData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>类型</summary>
    public int type;
    /// <summary>休息时间</summary>
    public int time;
    /// <summary>金币存储</summary>
    public int coinStorage;
    /// <summary>石砖存储</summary>
    public int stoneStorage;


    public BuildingAttributesData Clone()
    {
        return (BuildingAttributesData)this.MemberwiseClone();
    }
}
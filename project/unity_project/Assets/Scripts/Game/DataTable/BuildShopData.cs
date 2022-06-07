using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class BuildShopData
{
    /// <summary>商品编号</summary>
    public int id;
    /// <summary>物件编号</summary>
    public int buildingID;
    /// <summary>解锁星数</summary>
    public int unlockStarCount;
    /// <summary>基础售价</summary>
    public int basePrice;
    /// <summary>成本增量</summary>
    public int costIncrease;
    /// <summary>增量影响</summary>
    public int[] increaseRelated;
    /// <summary>货币类型</summary>
    public int currency;
    /// <summary>建造时间</summary>
    public int buildTime;
    /// <summary>建筑工地编号</summary>
    public int buildTerrainID;
    /// <summary>加速花费</summary>
    public int skipNeedDiamond;


    public BuildShopData Clone()
    {
        return (BuildShopData)this.MemberwiseClone();
    }
}
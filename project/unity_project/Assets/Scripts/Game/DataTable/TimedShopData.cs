using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class TimedShopData
{
    /// <summary>商品编号</summary>
    public int id;
    /// <summary>物件编号</summary>
    public OutputData items;
    /// <summary>位置</summary>
    public int pos;
    /// <summary>权重</summary>
    public int weight;
    /// <summary>开启星数</summary>
    public int startStars;
    /// <summary>终止星数</summary>
    public int endStars;
    /// <summary>基础售价</summary>
    public int basePrice;
    /// <summary>货币类型</summary>
    public int currency;
    /// <summary>价值增长</summary>
    public int costIncrease;
    /// <summary>限购区间</summary>
    public int[] buyCount;
    /// <summary>必须发现</summary>
    public int needItemIndex;


    public TimedShopData Clone()
    {
        return (TimedShopData)this.MemberwiseClone();
    }
}
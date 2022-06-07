using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class RandomShopData
{
    /// <summary>商品编号</summary>
    public int id;
    /// <summary>物件编号</summary>
    public string index;
    /// <summary>权重</summary>
    public int weight;
    /// <summary>价值等级</summary>
    public int price;
    /// <summary>开启星数</summary>
    public int startStarCount;
    /// <summary>终止星数</summary>
    public int endStarCount;
    /// <summary>数量</summary>
    public int number;
    /// <summary>基础售价</summary>
    public int basePrice;
    /// <summary>原价</summary>
    public int originalPrice;
    /// <summary>折扣</summary>
    public float discount;
    /// <summary>货币类型</summary>
    public int currency;
    /// <summary>必须发现</summary>
    public int needItemID;


    public RandomShopData Clone()
    {
        return (RandomShopData)this.MemberwiseClone();
    }
}
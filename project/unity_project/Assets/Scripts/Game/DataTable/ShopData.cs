using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class ShopData
{
    /// <summary>商品编号</summary>
    public int id;
    /// <summary>物件编号</summary>
    public int index;
    /// <summary>类型</summary>
    public int type;
    /// <summary>解锁星数</summary>
    public int unlockStar;
    /// <summary>售价</summary>
    public int price;
    /// <summary>价格上限</summary>
    public int priceLimit;
    /// <summary>成本增量</summary>
    public int costIncrease;
    /// <summary>货币类型</summary>
    public int currency;
    /// <summary>需要发现</summary>
    public int unlockNeedItemID;


    public ShopData Clone()
    {
        return (ShopData)this.MemberwiseClone();
    }
}
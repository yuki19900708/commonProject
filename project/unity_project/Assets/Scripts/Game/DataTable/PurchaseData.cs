using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class PurchaseData
{
    /// <summary>商品编号</summary>
    public int id;
    /// <summary>商品名称</summary>
    public int productName;
    /// <summary>类型</summary>
    public int type;
    /// <summary>商品图标</summary>
    public string productIcon;
    /// <summary>标签类型</summary>
    public int tag;
    /// <summary>标签文本</summary>
    public int tagName;
    /// <summary>钻石数量</summary>
    public int diamondCount;
    /// <summary>持续天数</summary>
    public int continueDays;
    /// <summary>价格文本</summary>
    public int priceText;
    /// <summary>价格：元</summary>
    public int priceCN;
    /// <summary>价格：美元</summary>
    public float priceUS;
    /// <summary>产品ID</summary>
    public string productId;
    /// <summary>限购数</summary>
    public int limitBuyCount;
    /// <summary>VIP经验</summary>
    public int vipExp;
    /// <summary>物件奖励</summary>
    public int[] reward;
    /// <summary>解锁地块</summary>
    public int unlock;


    public PurchaseData Clone()
    {
        return (PurchaseData)this.MemberwiseClone();
    }
}
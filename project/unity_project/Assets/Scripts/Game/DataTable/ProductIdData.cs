using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class ProductIdData
{
    /// <summary>序号</summary>
    public int id;
    /// <summary>文本编号</summary>
    public int priceText;
    /// <summary>中文价格</summary>
    public float priceCN;
    /// <summary>英文价格</summary>
    public float priceUS;
    /// <summary>充值码</summary>
    public string productId;


    public ProductIdData Clone()
    {
        return (ProductIdData)this.MemberwiseClone();
    }
}
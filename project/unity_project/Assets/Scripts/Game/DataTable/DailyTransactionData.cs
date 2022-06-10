using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class DailyTransactionData
{
    /// <summary>编号</summary>
    public int id;
    /// <summary>起始次数</summary>
    public int count;
    /// <summary>权重</summary>
    public int weight;
    /// <summary>索要物件</summary>
    public int needItem;
    /// <summary>索要数量</summary>
    public int[] needCount;
    /// <summary>回馈物件</summary>
    /// <summary>回馈数量</summary>
    public int rewardCount;
    /// <summary>额外奖励几率</summary>
    public int extraRewardRate;
    /// <summary>额外奖励物件</summary>
    /// <summary>额外奖励数量</summary>
    public int extraRewardCount;


    public DailyTransactionData Clone()
    {
        return (DailyTransactionData)this.MemberwiseClone();
    }
}
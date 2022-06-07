using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class ActivityData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>活动类型</summary>
    public int activityType;
    /// <summary>活动标题_Cn</summary>
    public string nameCn;
    /// <summary>活动标题_En</summary>
    public string nameEn;
    /// <summary>活动描述_Cn</summary>
    public string desCn;
    /// <summary>活动描述_En</summary>
    public string desEn;
    /// <summary>开启时间</summary>
    public int startTime;
    /// <summary>结束时间</summary>
    public int endTime;
    /// <summary>最大参与次数</summary>
    public int maxMount;
    /// <summary>价格</summary>
    public float price;
    /// <summary>充值码</summary>
    public string rechargeCode;
    /// <summary>折扣</summary>
    public int discount;
    /// <summary>VIP经验</summary>
    public int vipExp;
    /// <summary>活动奖励列表</summary>
    public int[] rewardDataList;
    /// <summary>URL</summary>
    public string url;
    /// <summary>高端区ID</summary>
    public int stepId;
    /// <summary>奖池类型</summary>
    public int prizepoolType;
    /// <summary>填充次数上限</summary>
    public int saveTimes;
    /// <summary>合成物体的ID</summary>
    public int mergeArticleID;
    /// <summary>合成物体的等级</summary>
    public int mergeArticleLevel;


    public ActivityData Clone()
    {
        return (ActivityData)this.MemberwiseClone();
    }
}
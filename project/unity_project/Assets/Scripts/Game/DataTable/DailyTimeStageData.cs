using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class DailyTimeStageData
{
    /// <summary>时间</summary>
    public int time;
    /// <summary>奖励列表1</summary>
    public int[] RewardList1;
    /// <summary>奖励列表2</summary>
    public int[] RewardList2;
    /// <summary>奖励列表3</summary>
    public int[] RewardList3;


    public DailyTimeStageData Clone()
    {
        return (DailyTimeStageData)this.MemberwiseClone();
    }
}
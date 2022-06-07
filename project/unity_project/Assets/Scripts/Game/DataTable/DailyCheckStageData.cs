using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class DailyCheckStageData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>天数</summary>
    public int day;
    /// <summary>奖励</summary>
    public int[] reward;


    public DailyCheckStageData Clone()
    {
        return (DailyCheckStageData)this.MemberwiseClone();
    }
}
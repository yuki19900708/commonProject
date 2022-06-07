using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class NoviceWelfareData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>天数</summary>
    public int day;
    /// <summary>奖励</summary>
    public int[] reward;


    public NoviceWelfareData Clone()
    {
        return (NoviceWelfareData)this.MemberwiseClone();
    }
}
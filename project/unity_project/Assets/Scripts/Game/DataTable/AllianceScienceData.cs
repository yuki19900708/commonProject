using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class AllianceScienceData
{
    /// <summary>编号</summary>
    public int id;
    /// <summary>科技类型</summary>
    public int type;
    /// <summary>科技等级</summary>
    public int level;
    /// <summary>货币类型</summary>
    public int currency;
    /// <summary>消耗数量</summary>
    public int cost;
    /// <summary>单次消耗</summary>
    public int[] singleCost;
    /// <summary>攻击附加</summary>
    public int attackIncrease;
    /// <summary>生命附加</summary>
    public int hpIncrease;


    public AllianceScienceData Clone()
    {
        return (AllianceScienceData)this.MemberwiseClone();
    }
}
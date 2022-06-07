using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class RankRewardData
{
    /// <summary>奖励编号</summary>
    public int id;
    /// <summary>榜单编号</summary>
    public int rankID;
    /// <summary>名次区间</summary>
    public int[] range;
    /// <summary>奖励组1</summary>
    public int[] reward1;
    /// <summary>奖励组2</summary>
    public int[] reward2;
    /// <summary>奖励组3</summary>
    public int[] reward3;


    public RankRewardData Clone()
    {
        return (RankRewardData)this.MemberwiseClone();
    }
}
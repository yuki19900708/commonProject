using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class RankData
{
    /// <summary>段位编号</summary>
    public int id;
    /// <summary>段位名</summary>
    public int name;
    /// <summary>所需积分</summary>
    public int needScore;
    /// <summary>K值</summary>
    public int kValue;
    /// <summary>掉段</summary>
    public int dropSegment;


    public RankData Clone()
    {
        return (RankData)this.MemberwiseClone();
    }
}
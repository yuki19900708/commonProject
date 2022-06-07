using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class ShieldData
{
    /// <summary>编号</summary>
    public int id;
    /// <summary>名称</summary>
    public string name;
    /// <summary>保护时间</summary>
    public int time;
    /// <summary>所需钻石</summary>
    public int cost;


    public ShieldData Clone()
    {
        return (ShieldData)this.MemberwiseClone();
    }
}
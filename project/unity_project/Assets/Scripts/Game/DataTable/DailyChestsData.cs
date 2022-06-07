using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class DailyChestsData
{
    /// <summary>序列</summary>
    public int id;
    /// <summary>宝箱ID</summary>
    public int index;


    public DailyChestsData Clone()
    {
        return (DailyChestsData)this.MemberwiseClone();
    }
}
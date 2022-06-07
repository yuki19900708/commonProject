using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class MonsterPortalData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>巨龙类型</summary>
    public int[] typeList;


    public MonsterPortalData Clone()
    {
        return (MonsterPortalData)this.MemberwiseClone();
    }
}
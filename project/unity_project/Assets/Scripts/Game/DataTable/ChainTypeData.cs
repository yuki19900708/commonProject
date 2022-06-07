using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class ChainTypeData
{
    /// <summary>类型ID</summary>
    public int id;
    /// <summary>类型名称</summary>
    public int name;
    /// <summary>组别ID</summary>
    public int ilustrationID;
    /// <summary>图鉴序列</summary>
    public int[] illustrationList;


    public ChainTypeData Clone()
    {
        return (ChainTypeData)this.MemberwiseClone();
    }
}
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class ChainGroupData
{
    /// <summary>组别ID</summary>
    public int id;
    /// <summary>类型名称</summary>
    public int name;
    /// <summary>包含类型序列</summary>
    public int[] items;
    /// <summary>组别图标</summary>
    public string imagePath;


    public ChainGroupData Clone()
    {
        return (ChainGroupData)this.MemberwiseClone();
    }
}
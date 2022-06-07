using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class HeadBoxData
{
    /// <summary>头像框编号</summary>
    public int id;
    /// <summary>解锁类型</summary>
    public int unlockType;
    /// <summary>解锁数据</summary>
    public int unlockNumber;
    /// <summary>目标计数</summary>
    public int needCount;
    /// <summary>描述文本</summary>
    public int describe;
    /// <summary>图素资源路径</summary>
    public string imagePath;


    public HeadBoxData Clone()
    {
        return (HeadBoxData)this.MemberwiseClone();
    }
}
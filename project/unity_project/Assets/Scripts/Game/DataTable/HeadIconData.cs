using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class HeadIconData
{
    /// <summary>头像编号</summary>
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


    public HeadIconData Clone()
    {
        return (HeadIconData)this.MemberwiseClone();
    }
}
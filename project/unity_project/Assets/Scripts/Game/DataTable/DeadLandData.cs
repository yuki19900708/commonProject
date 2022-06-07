using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class DeadLandData
{
    /// <summary>死地编号</summary>
    public int id;
    /// <summary>饱和度</summary>
    public float saturation;
    /// <summary>亮度</summary>
    public float brightness;
    /// <summary>免疫净化</summary>
    public bool ignorePure;
    /// <summary>净化所需</summary>
    public int pureNeed1;
    /// <summary>获得经验</summary>
    public int exp;


    public DeadLandData Clone()
    {
        return (DeadLandData)this.MemberwiseClone();
    }
}
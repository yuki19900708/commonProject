using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class BulletInfoData
{
    /// <summary>子弹编号</summary>
    public int id;
    /// <summary>类型</summary>
    public int type;
    /// <summary>子弹预设</summary>
    public string prefab;
    /// <summary>命中特效</summary>
    public string hitEffect;
    /// <summary>音效名称</summary>
    public string sound;


    public BulletInfoData Clone()
    {
        return (BulletInfoData)this.MemberwiseClone();
    }
}
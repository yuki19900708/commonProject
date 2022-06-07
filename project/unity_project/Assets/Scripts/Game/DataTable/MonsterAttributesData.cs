using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class MonsterAttributesData
{
    /// <summary>怪物编号</summary>
    public int id;
    /// <summary>怪物类型名</summary>
    public string typeName;
    /// <summary>怪物类型</summary>
    public int type;
    /// <summary>龙之力</summary>
    public int dragonPower;
    /// <summary>体力</summary>
    public int ap;
    /// <summary>行走速度</summary>
    public float moveSpeed;
    /// <summary>负重速度</summary>
    public float weightSpeed;
    /// <summary>采集系数</summary>
    public float collectRate;
    /// <summary>采集暴击几率</summary>
    public int collectCritRate;
    /// <summary>建造系数</summary>
    public float buildRate;
    /// <summary>战斗力</summary>
    public int fightingValue;
    /// <summary>单位时间伤害</summary>
    public int unitDamage;
    /// <summary>血量</summary>
    public int bloodVolume;
    /// <summary>攻击距离</summary>
    public float[] attackDistance;
    /// <summary>子弹速度</summary>
    public float bulletSpeed;
    /// <summary>前摇时间</summary>
    public float frontTime;
    /// <summary>后摇时间</summary>
    public float rearTime;
    /// <summary>连射弹数</summary>
    public int bulletCount;
    /// <summary>连射间隔</summary>
    public float bulletInterval;
    /// <summary>攻击动画原长</summary>
    public float spineShootTime;
    /// <summary>子弹伤害</summary>
    public int bulletDamage;
    /// <summary>五行图</summary>
    public int[] diagram;
    /// <summary>子弹编号</summary>
    public int bulletId;


    public MonsterAttributesData Clone()
    {
        return (MonsterAttributesData)this.MemberwiseClone();
    }
}
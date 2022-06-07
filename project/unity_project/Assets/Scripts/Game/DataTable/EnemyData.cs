using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class EnemyData
{
    /// <summary>怪物编号</summary>
    public int id;
    /// <summary>停滞时间</summary>
    public int[] stayTime;
    /// <summary>摧毁行为概率</summary>
    public int breakEventRate;
    /// <summary>污染行为概率</summary>
    public int polluteEventRate;
    /// <summary>死地等级</summary>
    public int deadLandID;
    /// <summary>污染时间</summary>
    public int polluteTime;
    /// <summary>攻击距离</summary>
    public float[] attackDistance;
    /// <summary>行走速度</summary>
    public float moveSpeed;
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
    /// <summary>子弹伤害</summary>
    public int bulletDamage;
    /// <summary>子弹编号</summary>
    public int bulletId;


    public EnemyData Clone()
    {
        return (EnemyData)this.MemberwiseClone();
    }
}
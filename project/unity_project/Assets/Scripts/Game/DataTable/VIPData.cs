using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class VIPData
{
    /// <summary>等级</summary>
    public int id;
    /// <summary>所需经验</summary>
    public int exp;
    /// <summary>体力上限</summary>
    public int apLimit;
    /// <summary>VIP商店栏位</summary>
    public int storeField;
    /// <summary>自动采集开关</summary>
    public bool autoHarvestSwitch;
    /// <summary>关卡宝箱栏位</summary>
    public int chestField;
    /// <summary>体力恢复时间</summary>
    public int apRecoverTime;
    /// <summary>龙休息缩短</summary>
    public float sleepTimeReduceRate;
    /// <summary>建造时间缩短</summary>
    public float builtReduceTimeRate;
    /// <summary>宝箱时间缩短</summary>
    public float chestReduceRate;
    /// <summary>金币石砖上限</summary>
    public int resourcesLimit;
    /// <summary>解锁头像框</summary>
    public int avatarFrame;
    /// <summary>双倍签到</summary>
    public bool doubleSignin;
    /// <summary>补签次数</summary>
    public int remedyNum;
    /// <summary>龙蛋奖励</summary>
    public int[] eggsReward;
    /// <summary>稀有奖励</summary>
    public int[] reward;
    /// <summary>图标资源路径</summary>
    public string imagePath;


    public VIPData Clone()
    {
        return (VIPData)this.MemberwiseClone();
    }
}
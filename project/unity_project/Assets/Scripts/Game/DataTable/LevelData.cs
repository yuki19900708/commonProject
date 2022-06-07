using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class LevelData
{
    /// <summary>关卡ID</summary>
    public int id;
    /// <summary>关卡名称</summary>
    public int name;
    /// <summary>体力消耗</summary>
    public int needAP;
    /// <summary>是否为挑战</summary>
    public bool challengeLevel;
    /// <summary>关卡完成条件</summary>
    public int finishCondition;
    /// <summary>所需数量</summary>
    public int needCount;
    /// <summary>解锁关卡列表</summary>
    public int[] unlockLevelList;
    /// <summary>商品列表</summary>
    public int[] productList;
    /// <summary>任务列表</summary>
    public int[] taskList;
    /// <summary>钻石上限</summary>
    public int diamondCap;
    /// <summary>挑战奖励</summary>
    public int[] challengeReward;
    /// <summary>宝箱列表</summary>
    public int[] chestList;
    /// <summary>免费掉落1</summary>
    public OutputData freeReward1;
    /// <summary>免费掉落2</summary>
    public OutputData freeReward2;
    /// <summary>付费掉落1</summary>
    public OutputData payReward1;
    /// <summary>付费掉落2</summary>
    public OutputData payReward2;
    /// <summary>付费掉落3</summary>
    public OutputData payReward3;
    /// <summary>付费掉落4</summary>
    public OutputData payReward4;
    /// <summary>付费掉落5</summary>
    public OutputData payReward5;
    /// <summary>奖励展示</summary>
    public int[] rewardShow;
    /// <summary>风产出列表</summary>
    public OutputData windItems;
    /// <summary>风产出速度</summary>
    public float[] windOutputSpeed;
    /// <summary>产出物间隔</summary>
    public int[] windOutputInterval;
    /// <summary>产出物数量</summary>
    public int[] windOutputCount;
    /// <summary>风间隔</summary>
    public int[] windInterval;
    /// <summary>Good秒数</summary>
    public int challengeLevel1;
    /// <summary>Great秒数</summary>
    public int challengeLevel2;
    /// <summary>Epic秒数</summary>
    public int challengeLevel3;
    /// <summary>Good分数</summary>
    public int normalLevel1;
    /// <summary>Great分数</summary>
    public int normalLevel2;
    /// <summary>Epic分数</summary>
    public int normalLevel3;
    /// <summary>阶段奖励</summary>
    public int stageReward;
    /// <summary>结算衍生</summary>
    public OutputData levelSpawn;
    /// <summary>关卡图标</summary>
    public string imagePath;
    /// <summary>魔物关卡</summary>
    public int hasEnemy;
    /// <summary>地编文件</summary>
    public string fileName;
    /// <summary>镜头位置</summary>
    public int[] cameraPos;


    public LevelData Clone()
    {
        return (LevelData)this.MemberwiseClone();
    }
}
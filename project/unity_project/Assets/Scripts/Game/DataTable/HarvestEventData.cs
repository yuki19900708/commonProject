using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class HarvestEventData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>同时采集数</summary>
    public int harvestAtSameTime;
    /// <summary>采集产出</summary>
    public OutputData outputItems;
    /// <summary>采集时间</summary>
    public int time;
    /// <summary>充能时间</summary>
    public int cooldown;
    /// <summary>可采集次数</summary>
    public int count;
    /// <summary>基地不可采集</summary>
    public bool foundationCanHarvest;
    /// <summary>采集缩小</summary>
    public bool scaleSmall;
    /// <summary>次数耗尽销毁</summary>
    public bool noCountDestroy;


    public HarvestEventData Clone()
    {
        return (HarvestEventData)this.MemberwiseClone();
    }
}
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class TapEventData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>产出类型</summary>
    public int outputType;
    /// <summary>变量类型</summary>
    public int itemType;
    /// <summary>动态数量</summary>
    public bool dynamicCount;
    /// <summary>产出数量</summary>
    public int outputCount;
    /// <summary>显示箭头</summary>
    public int showArrow;
    /// <summary>是否提示</summary>
    public bool showTip;
    /// <summary>钻石消耗</summary>
    public int needDiamond;
    /// <summary>折扣</summary>
    public float discount;
    /// <summary>UI展示奖励</summary>
    public int showReward;
    /// <summary>主要产出列表</summary>
    /// <summary>主要产出数量</summary>
    public int[] mainOutputCount;
    /// <summary>其他产出列表</summary>
    /// <summary>其他产出数量</summary>
    public int[] otherOutputCount;
    /// <summary>额外产出列表</summary>    /// <summary>额外产出概率</summary>
    public int extraOutputRate;
    /// <summary>有效点击次数</summary>
    public int[] clickTimes;
    /// <summary>充能时间</summary>
    public int cooldown;
    /// <summary>点击缩小</summary>
    public bool scaleSmall;
    /// <summary>是否销毁</summary>
    public bool isDestroy;
    /// <summary>产出放入气泡</summary>
    public bool outputInBubble;


    public TapEventData Clone()
    {
        return (TapEventData)this.MemberwiseClone();
    }
}
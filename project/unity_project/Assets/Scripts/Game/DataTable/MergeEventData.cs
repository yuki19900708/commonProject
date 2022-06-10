using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class MergeEventData
{
    /// <summary>物件ID</summary>
    public int id;
    /// <summary>可参与列表</summary>
    public int[] mergeList;
    /// <summary>目标物件</summary>
    public int targetID;
    /// <summary>合成经验</summary>
    public int exp;
    /// <summary>关卡积分道具</summary>
    /// <summary>特惠触发几率</summary>
    public int specialTriggerRate;
    /// <summary>额外特惠几率</summary>
    public int extraSpecialTriggerRate;
    /// <summary>特惠消耗</summary>
    public int specialNeedDiamond;
    /// <summary>必出缺省产出</summary>
    public bool mustHaveOutput;
    /// <summary>其他产出概率</summary>
    public int extraOutputRate;
    /// <summary>额外1组产出</summary>
    /// <summary>额外1组数量</summary>
    public int[] extraItemCount1;
    /// <summary>额外1组概率</summary>
    public int extraRate1;
    /// <summary>额外2组产出</summary>
    /// <summary>额外2组数量</summary>
    public int[] extraItemCount2;
    /// <summary>额外2组概率</summary>
    public int extraRate2;
    /// <summary>额外3组产出</summary>
    /// <summary>额外3组数量</summary>
    public int[] extraItemCount3;
    /// <summary>额外3组概率</summary>
    public int extraRate3;
    /// <summary>额外4组产出</summary>
    /// <summary>额外4组数量</summary>
    public int[] extraItemCount4;
    /// <summary>额外4组概率</summary>
    public int extraRate4;


    public MergeEventData Clone()
    {
        return (MergeEventData)this.MemberwiseClone();
    }
}
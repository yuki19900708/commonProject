using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class MapObjectData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>原名称_Cn</summary>
    public string standbyName;
    /// <summary>当前名称_Cn</summary>
    public string currentName;
    /// <summary>物件名称</summary>
    public int name;
    /// <summary>图鉴组别</summary>
    public int illustration;
    /// <summary>组名备注</summary>
    public string illustrationType;
    /// <summary>类型</summary>
    public int objectType;
    /// <summary>类型名称</summary>
    public string typeName;
    /// <summary>任务对象标签</summary>
    public int[] targetTag;
    /// <summary>物件定价</summary>
    public int originalPrice;
    /// <summary>占地</summary>
    public int[] area;
    /// <summary>存储</summary>
    public bool save;
    /// <summary>稀有度</summary>
    public int rarity;
    /// <summary>图鉴积分</summary>
    public int chainScore;
    /// <summary>图鉴特效</summary>
    public int chainEffect;
    /// <summary>等级展示</summary>
    public int level;
    /// <summary>文本描述</summary>
    public int describe;
    /// <summary>是否可合成</summary>
    public bool canMerge;
    /// <summary>是否为死物</summary>
    public bool isDeadObject;
    /// <summary>关卡结束替换</summary>
    public int isLevelComplete;
    /// <summary>是否可掠夺</summary>
    public bool canScramble;
    /// <summary>采集积分</summary>
    public int harvestScore;
    /// <summary>掠夺是否封地</summary>
    public bool scrambleLockTerrain;
    /// <summary>是否可拖动</summary>
    public bool canDrag;
    /// <summary>是否可选择</summary>
    public bool canSelect;
    /// <summary>关卡奖励兑换</summary>
    public int exchangeReward;
    /// <summary>售卖或删除</summary>
    public int canSell;
    /// <summary>销售价值</summary>
    public int price;
    /// <summary>确认售卖</summary>
    public bool ensureSale;
    /// <summary>可摧毁类型</summary>
    public int destructType;
    /// <summary>点击事件</summary>
    public bool canClick;
    /// <summary>点击动画</summary>
    public int clickAnimation;
    /// <summary>初始锁定</summary>
    public bool initLock;
    /// <summary>衍生事件</summary>
    public bool canSpawn;
    /// <summary>采集事件</summary>
    public bool canHarvest;
    /// <summary>可脱离网格</summary>
    public bool detachGrid;
    /// <summary>存活时间</summary>
    public int[] liveTime; 

    /// <summary>点击特效</summary>
    public int clickParticle;
    /// <summary>死亡动画</summary>
    public int deadAnimation;
    /// <summary>死亡特效</summary>
    public int deadParticle;
    /// <summary>衍生动画</summary>
    public int spawnAnimation;
    /// <summary>衍生特效</summary>
    public int spawnParticle;
    /// <summary>新生动画</summary>
    public int appearAnimation;
    /// <summary>新生特效</summary>
    public int appearParticle;
    /// <summary>待机动画</summary>
    public int idleAnimation;
    /// <summary>预设路径</summary>
    public string prefabPath;


    public MapObjectData Clone()
    {
        return (MapObjectData)this.MemberwiseClone();
    }
}
using DG.Tweening;
using GameProto;
//using Spine;
//using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// 图鉴类型枚举
/// </summary>
public enum Handbook
{
    /// <summary>
    /// 资源类
    /// </summary>
    Resources = 1,
    /// <summary>
    /// 奇迹类
    /// </summary>
    Miracle,
    /// <summary>
    /// 建筑类
    /// </summary>
    Building,
    /// <summary>
    /// 宝箱类
    /// </summary>
    Chest,
    /// <summary>
    /// 怪物类
    /// </summary>
    Monster,
    /// <summary>
    /// 其他类
    /// </summary>
    Other
}

/// <summary>
/// 稀有度等级
/// </summary>
public enum Rarity
{
    /// <summary>
    /// 普通
    /// </summary>
    Common = 1,
    /// <summary>
    /// 罕见
    /// </summary>
    UnCommon = 2,
    /// <summary>
    /// 稀有
    /// </summary>
    Rare = 3,
    /// <summary>
    /// 神话
    /// </summary>
    Mythic = 4,
    /// <summary>
    /// 史诗
    /// </summary>
    Epic = 5,
    /// <summary>
    /// 传说
    /// </summary>
    Legend = 6
}

/// <summary>
/// 被攻击类型
/// </summary>
public enum DestructType
{
    /// <summary>
    /// 不可被摧毁
    /// </summary>
    CannotBeDestroy,
    /// <summary>
    /// 可被敌人摧毁
    /// </summary>
    DestroyedByEnemy,
    /// <summary>
    /// 可被己方单位摧毁
    /// </summary>
    DestroyedByAlly
}

/// <summary>
/// 生物类型
/// </summary>
public enum MonsterType
{
    //未知
    None = 0,
    /// <summary>
    /// 均衡
    /// </summary>
    Normal = 1,
    /// <summary>
    /// 采集
    /// </summary>
    Harvester,
    /// <summary>
    /// 建造
    /// </summary>
    Builder,
    /// <summary>
    /// 攻击
    /// </summary>
    Attacker,
    /// <summary>
    /// 坚守
    /// </summary>
    Defender,
}

/// <summary>
/// 物体间的行为
/// </summary>
public enum BehaviourOfObjects
{
    None,
    /// <summary>
    /// 攻击
    /// </summary>
    NormalAttack,
    /// <summary>
    /// 掠夺战斗
    /// </summary>
    PlunderAttack,
    /// <summary>
    /// 污染
    /// </summary>
    Pollute,
    /// <summary>
    /// 采集
    /// </summary>
    Collect,
    /// <summary>
    /// 建造
    /// </summary>
    Build,
    /// <summary>
    /// 休息
    /// </summary>
    Sleep,
    /// <summary>
    /// 运输
    /// </summary>
    Transport
}

public enum ItemStatus
{
    None,
    CollectTarget,
    BeCollecting,
    CollectMonsterFull,
    AttackTarget,
    BeAttacking,
    PolluteTarget,
    BePollute,
    BuildTarget,
    BeBuilding,
    SleepTarget,
    SleepBuildingWorking
}

public partial class MapObject : MapItem
{
    public static readonly string ANI_NAME_IDLE = "idle";
    public static readonly string ANI_NAME_COLLECT = "interact";
    public static readonly string ANI_NAME_ATTACK = "attack";
    public static readonly string[] ANI_NAME_ARRAY = { ANI_NAME_IDLE , ANI_NAME_COLLECT, ANI_NAME_ATTACK };


    public const float DOUBLE_CLICLK_SPEED = 5;
    public const float Attack_Move_Speed = 3;


    /// <summary>
    /// 生命之球
    /// </summary>
    public const int OBJECT_TYPE_PURIFY_BALL = 101;
    /// <summary>
    /// 地精洞穴
    /// </summary>
    public const int OBJECT_TYPE_GOBLIN_CAVE = 204;
    /// <summary>
    /// 净化神树
    /// </summary>
    public const int OBJECT_TYPE_PURIFY_GOD_TREE = 206;
    /// <summary>
    /// 石砖房
    /// </summary>
    public const int OBJECT_TYPE_STONE_BUILDING = 302;
    /// <summary>
    /// 金币房
    /// </summary>
    public const int OBJECT_TYPE_GOLD_BUILDING = 303;
    /// <summary>
    /// 云雾
    /// </summary>
    public const int OBJECT_TYPE_FOGGY_MOUNTAIN = 605;
    /// <summary>
    /// 我方图腾
    /// </summary>
    public const int OBJECT_TYPE_TOTEM = 606;
    /// <summary>
    /// 净化者
    /// </summary>
    public const int OBJECT_TYPE_PURIFIER = 608;
    /// <summary>
    /// 魔物
    /// </summary>
    public const int OBJECT_TYPE_EVIL_MONSTER = 611;
    /// <summary>
    /// 怪石阵
    /// </summary>
    public const int OBJECT_TYPE_STRANGE_STONE_ARRAY = 614;
    /// <summary>
    /// 净化之力
    /// </summary>
    public const int OBJECT_TYPE_PURIFY_POWER = 701;
    /// <summary>
    /// 魔物雕像
    /// </summary>
    public const int OBJECT_TYPE_ENEMY_TOTEM = 714;
    /// <summary>
    /// 小龙蛋巢
    /// </summary>
    public const int OBJECT_TYPE_DRAG_EGG_NEST_SMALL = 718;
    /// <summary>
    /// 大龙蛋巢
    /// </summary>
    public const int OBJECT_TYPE_DRAG_EGG_NEST_BIG = 719;
    /// <summary>
    /// 魔窟洞穴
    /// </summary>
    public const int OBJECT_TYPE_MAGIC_CAVE = 728;
    /// <summary>
    /// 战利品球
    /// </summary>
    public const int OBJECT_TYPE_TROPHY_BALL = 729;
    /// <summary>
    /// 连击球
    /// </summary>
    public const int OBJECT_TYPE_BATTER_BALL = 730;

    #region 需要花钻石解锁物品
    /// <summary>
    /// 蛋箱1
    /// </summary>
    public const int OBJECT_TYPE_EGG_CHEST_1 = 720;
    /// <summary>
    /// 蛋箱2
    /// </summary>
    public const int OBJECT_TYPE_EGG_CHEST_2 = 721;
    /// <summary>
    /// 巢箱1
    /// </summary>
    public const int OBJECT_TYPE_EGG_GROUP_CHEST_1 = 722;
    /// <summary>
    /// 巢箱2
    /// </summary>
    public const int OBJECT_TYPE_EGG_GROUP_CHEST_2 = 723;
    /// <summary>
    /// 巢箱3
    /// </summary>
    public const int OBJECT_TYPE_EGG_GROUP_CHEST_3 = 724;
    /// <summary>
    /// 神秘箱
    /// </summary>
    public const int OBJECT_TYPE_MYSTERIOUS_CHEST = 733;
    #endregion

    #region 直接点击就打开的箱子类物品
    /// <summary>
    /// 关卡宝箱
    /// </summary>
    public const int OBJECT_TYPE_FREE_CHEST = 401;
    /// <summary>
    /// 商店宝箱A
    /// </summary>
    public const int OBJECT_TYPE_SHOP_CHEST_A = 402;
    /// <summary>
    /// 商店宝箱B
    /// </summary>
    public const int OBJECT_TYPE_SHOP_CHEST_B = 403;
    /// <summary>
    /// 商店宝箱C
    /// </summary>
    public const int OBJECT_TYPE_SHOP_CHEST_C = 404;
    /// <summary>
    /// 商店宝箱D
    /// </summary>
    public const int OBJECT_TYPE_SHOP_CHEST_D = 405;
    /// <summary>
    /// 商店宝箱E
    /// </summary>
    public const int OBJECT_TYPE_SHOP_CHEST_E = 406;
    /// <summary>
    /// 格森姆箱
    /// </summary>
    public const int OBJECT_TYPE_GRIMM_CHEST = 407;
    /// <summary>
    /// 关卡宝箱
    /// </summary>
    public const int OBJECT_TYPE_CHAPTER_CHEST = 725;
    /// <summary>
    /// 日常宝箱
    /// </summary>
    public const int OBJECT_TYPE_DAILY_CHEST = 726;
    /// <summary>
    /// 掠夺宝箱
    /// </summary>
    public const int OBJECT_TYPE_PLUNDER_CHEST = 734;
#endregion

    /// <summary>
    /// 雨云
    /// </summary>
    public const int OBJECT_ID_RAIN_CLOUD = 70013;
    /// <summary>
    /// 云一
    /// </summary>
    public const int OBJECT_ID_CLOUD_1 = 70014;
    /// <summary>
    /// 云二
    /// </summary>
    public const int OBJECT_ID_CLOUD_2 = 70015;
    /// <summary>
    /// 溢出石砖
    /// </summary>
    public const int OBJECT_ID_SPILLING_STONE = 70017;
    /// <summary>
    /// 关卡宝箱入口物体
    /// </summary>
    public const int OBJECT_ID_CHAPTER_CHEST = 70020;
    /// <summary>
    /// 古神的指引
    /// </summary>
    public const int OBJECT_ID_MAGIC_BOOK = 70022;
    /// <summary>
    /// 溢出金币
    /// </summary>
    public const int OBJECT_ID_SPILLING_COIN = 70024;
    /// <summary>
    /// 特惠球
    /// </summary>
    public const int OBJECT_ID_SPECIAL_BALL = 73101;
    /// <summary>
    /// 真 建筑地基。73202与73203都不是
    /// </summary>
    public const int OBJECT_ID_BUILDING_FOUNDATION = 73201;
    /// <summary>
    /// 积分道具
    /// </summary>
    public const int POINT_PROPS = 103;

    /// <summary>
    /// 自然死亡事件
    /// </summary>
    //public event Action<int> Event_NaturalDeath;

    /// <summary>
    /// 用于对象池出池时使用
    /// </summary>
    public int tableID;

    private Handbook handbook;
    private DestructType destructType;
    private Rarity rarity;
    //剩余熟量 溢出金币，与溢出石块
    private int remain_amount;
    private MapObject targetObject;
    //private IStatus currentStatus;
    private ItemStatus currentItemStatus;
    private MapObjectData basicData;
    //private Dictionary<StatusName, IStatus> statusDic = new Dictionary<StatusName, IStatus>();
    private int onlyId;
    private bool isDoubleClick;
    /// <summary>
    /// 当前能否被操作 （主要用于飞翔的神圣粒子，与龙采集成功挂载的物体）
    /// </summary>
    private bool isCanBeSelected = true;
    private bool isDraged = false;
    /// <summary>
    /// 是否时间静止
    /// </summary>
    private bool isTimeStill = false;
    /// <summary>
    /// 是否处于玩家操作状态
    /// </summary>
    private bool isPlayerOperate;
    private Tweener floatingAnim;

    private float idleTimer;
    private float idleMoment;
    private Transform magicBookTip;

    private MAP_OBJECT_STATUS mapObjectStatus= MAP_OBJECT_STATUS.Normal;
    //骨骼动画
    //public SkeletonAnimation skeletonAnimation;

    #region 物体待机相关动画
    public enum IdleAnimType
    {
        play,
        pause,
        stop
    }
    /// <summary>
    /// 是否正在播放待机动画
    /// </summary>
    private IdleAnimType PlayingEnityIdleAnimation = IdleAnimType.stop;
    /// <summary>
    /// 待机动画播放间隔
    /// </summary>
    private float entityIdleAnimationInterval = 0;
    /// <summary>
    /// 待机动画播放间隔计时器
    /// </summary>
    private float entityIdleAnimationTimer = 0;
    public Transform entityTransform;
    public Tweener entityTweener;
    private Vector3 entityTransformInitPos;
    public Transform shadowTransform;
    private Vector3 shadowTransformInitScale;
    #endregion
    public Transform tipMountPosition;
    //public ClickToCollectTip clickToCollectTip;
    //public ClickToCollectTip chapterTip;
    public int Id { get { return basicData.id; } }

    /// <summary>
    /// 物品图鉴类型
    /// </summary>
    public Handbook HandBook { get { return handbook; } }

    /// <summary>
    /// 被摧毁类型
    /// </summary>
    public DestructType DestructType { get { return destructType; } }

    public int ObjectType { get { return basicData.objectType; } }

    /// <summary>
    /// 稀有度等级
    /// </summary>
    public Rarity Rarity { get { return rarity; } }

    public int Level { get { return basicData.level; } }

    public int Describe { get { return basicData.describe; } }

    public MapObjectData BasicData { get { return basicData; } }

    public bool IsPlayerOperate { get { return isPlayerOperate; } }

    public bool IsDraged
    {
        get
        {
            return isDraged;
        }
        set
        {
            isDraged = value;
            //if (currentStatus.Name != StatusName.AttackFront
            //    && currentStatus.Name != StatusName.AttackAnimation
            //    && currentStatus.Name != StatusName.ShootWait
            //    && currentStatus.Name != StatusName.DeadAnimationStatus
            //    && currentStatus.Name != StatusName.DeadStatus
            //    && CurrentStatus.Name != StatusName.EnterHomeAnimation
            //    && CurrentStatus.Name != StatusName.QuitHomeAnimation)
            //{
            //    if (isDraged)
            //    {
            //        SwitchStatus(StatusName.Drag);
            //    }
            //    else
            //    {
            //        SwitchStatus(StatusName.Idle);
            //    }
            //}
        }
    }

    //public IStatus CurrentStatus
    //{
    //    get { return currentStatus; }
    //}

    public ItemStatus CurrentItemStatus
    {
        get { return currentItemStatus; }
    }

    public MapObject TargetObject
    {
        get { return targetObject; }
        set
        {
            SpecifiedTargetObject(value);
        }
    }

    /// <summary>
    /// 球中存在的物品
    /// </summary>
    public List<int> included_objects = new List<int>();

    public int Remain_amount
    {
        get
        {
            return remain_amount;
        }

        set
        {
            remain_amount = value;
        }
    }

    /// <summary>
    /// 是否是箱子
    /// </summary>
    public bool IsChest
    {
        get
        {
            return (this.ObjectType == OBJECT_TYPE_FREE_CHEST ||
                this.ObjectType == OBJECT_TYPE_SHOP_CHEST_A ||
                this.ObjectType == OBJECT_TYPE_SHOP_CHEST_B ||
                this.ObjectType == OBJECT_TYPE_SHOP_CHEST_C ||
                this.ObjectType == OBJECT_TYPE_SHOP_CHEST_D ||
                this.ObjectType == OBJECT_TYPE_SHOP_CHEST_E ||
                this.ObjectType == OBJECT_TYPE_GRIMM_CHEST ||
                this.ObjectType == OBJECT_TYPE_CHAPTER_CHEST ||
                this.ObjectType == OBJECT_TYPE_DAILY_CHEST ||
                this.ObjectType == OBJECT_TYPE_PLUNDER_CHEST ||

                this.ObjectType == OBJECT_TYPE_EGG_CHEST_1 ||
                this.ObjectType == OBJECT_TYPE_EGG_CHEST_2 ||
                this.ObjectType == OBJECT_TYPE_EGG_GROUP_CHEST_1 ||
                this.ObjectType == OBJECT_TYPE_EGG_GROUP_CHEST_2 ||
                this.ObjectType == OBJECT_TYPE_EGG_GROUP_CHEST_3 ||
                this.ObjectType == OBJECT_TYPE_MYSTERIOUS_CHEST);
        }
    }
    /// <summary>
    /// 是否为怪兽
    /// </summary>
    public bool IsMonster
    {
        get
        {
            if (handbook == Handbook.Monster && basicData.detachGrid)
            {
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// 是否为敌怪
    /// </summary>
    public bool IsZombies
    {
        get { return ObjectType == OBJECT_TYPE_EVIL_MONSTER; }
    }
  
    /// <summary>
    /// 是否为地面物体
    /// </summary>
    public bool IsGroundObject
    {
        get
        {
            return !basicData.detachGrid;
        }
    }

    public bool IsCanBeSelected
    {
        get
        {
            return isCanBeSelected;
        }

        set
        {
            isCanBeSelected = value;
        }
    }

    public int MonsterOnlyId
    {
        get { return onlyId; }
        set { onlyId = value; }
    }

    public bool IsDoubleClick
    {
        get { return isDoubleClick; }
    }

    public MAP_OBJECT_STATUS MapObjectStatus
    {
        get
        {
            return mapObjectStatus;
        }
        set
        {
            mapObjectStatus = value;
        }
    }

    //public Dictionary<StatusName, IStatus> StatusDict
    //{
    //    get
    //    {
    //        return statusDic;
    //    }
    //}

    #region Unity Method

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    private void Start()
    {
        if (magicBookTip == null)
        {
            magicBookTip = transform.Find("tipAnimation");
            magicBookTip.DOLocalMoveY(1.89f, 0.8f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
//		Debug.LogError (23);
        //if (skeletonAnimation != null)
        //{
        //    MeshRenderer mr = skeletonAnimation.GetComponent<MeshRenderer>();
        //    mr.sortingLayerName = "Floating";
        //    mr.sortingOrder= 10;
        //    //skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
        //    //skeletonAnimation.AnimationState.Complete += HandleAnimationStateComplete;
        //}
        //if (currentStatus.Name == StatusName.Idle)
        //{
        //    PlayIdleAnimation();
        //}
        //else if (currentStatus.Name == StatusName.AttackAnimation)
        //{
        //    PlayAttackAnimation();
        //}
        //else if (currentStatus.Name == StatusName.Collecting)
        //{
        //    PlayCollectAnimation();
        //}
    }

    #endregion

    #region 操所数据相关

    /// <summary>
    /// 切换生物朝向
    /// </summary>
    /// <param name="targetIsGrid"></param>
    public void UpdateFaceDirection(Vector3 targetPos)
    {
        if (targetPos.x > transform.position.x)
        {
            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
        else if (targetPos.x < transform.position.x)
        {
            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// 切换状态（物体的被动状态）
    /// </summary>
    /// <param name="status"></param>
    /// <param name="enabledEnterAndQuit"></param>
    public void SwitchItemStatus(ItemStatus status)
    {
        currentItemStatus = status;
    }

    /// <summary>
    /// 移动
    /// </summary>
    public void Move()
    {
        //if (collectArticleMapObject != null)
        //{
        //    transform.position = Vector2.MoveTowards(transform.position, targetObject.transform.position, WeightSpeed * Time.deltaTime);
        //}
        //else
        //{
        //    if (isDoubleClick)
        //    {
        //        transform.position = Vector2.MoveTowards(transform.position, targetObject.transform.position, moveSpeed * Time.deltaTime*DOUBLE_CLICLK_SPEED);
        //    }
        //    else
        //    {
        //        transform.position = Vector2.MoveTowards(transform.position, targetObject.transform.position, moveSpeed * Time.deltaTime);
        //    }
        //}
        //if (doublePFXList != null && doublePFXList.Count > 0)
        //{
        //    for (int i = 0; i < doublePFXList.Count; i++)
        //    {
        //        doublePFXList[i].transform.position = new Vector2(entityTransform.position.x, entityTransform.position.y);
        //    }
        //}
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,0);
        UpdateFaceDirection(targetObject.transform.position);
    }

    public float CalculateDistance()
    {
        return Vector2.Distance(transform.position, targetObject.transform.position);
    }

    /// <summary>
    /// 指定目标物体
    /// </summary>
    /// <param name="value"></param>
    public void SpecifiedTargetObject(MapObject value)
    {
        targetObject = value;
        if (targetObject != null)
        {
            //BehaviourOfObjects behaviour = MapMgr.CalculateBehaviour(this, targetObject);
            //if (behaviour == BehaviourOfObjects.NormalAttack || behaviour == BehaviourOfObjects.PlunderAttack)
            //{
            //    //攻击行为随机攻击距离
            //    RandomAttackDistance();
            //    SwitchStatus(StatusName.MoveToAttack);
            //}
            //else if (behaviour == BehaviourOfObjects.Build)
            //{
            //    SwitchStatus(StatusName.MoveToBuild);
            //}
            //else if (behaviour == BehaviourOfObjects.Collect)
            //{
            //    SwitchStatus(StatusName.MoveToCollect);
            //}
            //else if (behaviour == BehaviourOfObjects.Pollute)
            //{
            //    SwitchStatus(StatusName.MoveToPollute);
            //}
            //else if (behaviour == BehaviourOfObjects.Sleep)
            //{
            //    SwitchStatus(StatusName.MoveToSleep);
            //}
            //else if (behaviour == BehaviourOfObjects.Transport)
            //{
            //    SwitchStatus(StatusName.Transport);
            //}
        }
    }

    /// <summary>
    /// 当一个物品归池的时候，清除对象上的引用(所有的引用与变量都需要清除)
    /// </summary>
    public void ReturnPoolClearAssociation()
    {
        isDraged = false;
        isPlayerOperate = false;
        isDoubleClick = false;
        isTimeStill = false;
        transform.localScale = Vector3.one;
        //归池时将所有的物体的碰撞再次打开（被锁或者封印的地形的Collider是false）
        if (gameObject.GetComponent<Collider2D>() != null)
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }

        DOTween.Kill(transform);
        DOTween.Kill(entityTransform);

        ////龙巢被清除时，赶出所有的龙
        //if (ObjectType == SleepBuilding_ObjectType)
        //{
        //    ClearAllSleeper();
        ////}
        ////地基被清除时，清除所有的建造者
        //if (ObjectType == Foundation_ObjectType)
        //{
        //    ClearAllBuilder();
        //}

        //被攻击的物体清除攻击者
        if (destructType != DestructType.CannotBeDestroy || IsMonster)
        {
            //ClearAllAttacker();
        }

        //清除当前采集的关联
        //if (basicData.canHarvest)
        //{
        //    CollectMgr.CollectInterrupt(this);
        //}

        //if (IsMonster)
        //{
        //    //龙背负物体
        //    if (collectArticleMapObject != null)
        //    {
        //        if (this.transform.localScale.x != 1)
        //        {
        //            Debug.LogWarning("LFL  采集ICon 在归池前 父物体的Scale 不为1");
        //        }
        //        MapMgr.CollectBallPool.RecycleInstance(collectArticleMapObject);
        //        collectArticleMapObject = null;
        //    }
        //}
        //SwitchStatus(StatusName.Idle);
        //清除合成动画
        //ClearMergeAnimation();
        //清除衍生所有关联
        //DerivativeClear(true);
        //清除采集所有关联
        //CollectTimeClear();
        //清除存活时间的记录值
        //SurvivalTimeClear();
        //点击产出清除关联
        //ClearClickOutPutTime();
        //清除净化器的信息
        //ClearPurifier();
        //清楚基础属性
        ClearObjectAttribute();
        //清除漂浮叶子关联
        //RecycleLeaf();
        //清楚战利品球关联
        //StopTrophyBallTimer();
        //清除特惠关联；
        //RecycleSpecialData();
        //RecycleBatterball();
        HideTip();

        //清除特效
        //if (entityTransform != null)
        //{
            //VFXMgr.RemoveTrailVFX(this.entityTransform);
        //}
        //VFXMgr.RemoveMapChestUnlockedVFX(this);

        //清楚Doublick点击效果
        //if (doublePFXList != null && doublePFXList.Count > 0)
        //{
        //    for (int i = 0; i < doublePFXList.Count; i++)
        //    {
        //        MapMgr.DoubleClickSightTipPool.RecycleInstance(doublePFXList[i]);
        //    }
        //    doublePFXList.Clear();
        //}
    }

    /// <summary>
    /// 地图上的物体在被挤走时，清除关联
    /// </summary>
    //public void BeingSqueezeAwayClearAssociation()
    //{
        ////地基被挤走时
        //if (ObjectType == Foundation_ObjectType)
        //{
        //    ClearBuilder();
        //}
        //else if (basicData.canHarvest)
        //{
        //    //CollectMgr.CollectInterrupt(this);
        //}
    //}

    /// <summary>
    /// 合成的时候清除关联
    /// </summary>
    public void ReadyMergeClearAssociation()
    {
        if (IsMonster)
        {
            SwitchStatusToIdle();
        }
        else
        {
            if (basicData.canHarvest)
            {
                //CollectMgr.CollectInterrupt(this);
            }
        }
    }

    #region Idle状态相关

    public void IdleStatusEnter()
    {
        ResetIdleTimer();
        targetObject = null;
        SetPlayerOperateTag(false);
        SetDoubleClickTag(false);
        PlayIdleAnimation();
    }

    public void IdleStatusUpdate()
    {
        Assert.IsNull(targetObject, "TargetObject should be null");
        if (IsMonster)
        {
            MonsterAutonomousBehaviour();
        }
        else if (ObjectType == OBJECT_TYPE_EVIL_MONSTER)
        {
            ZombiesMonsterAutonomousBehaviour();
        }
    }

    private void MonsterAutonomousBehaviour()
    {
        ////大本营模式与关卡模式AI行为
        //if (GlobalVariable.GameState == GameState.MainSceneMode)
        //{
        //    BaseCampAI();
        //}
        //else if (GlobalVariable.GameState == GameState.LevelModel)
        //{
        //    LevelModeAI();
        //}
        //else if (GlobalVariable.GameState == GameState.PlunderMode)
        //{
        //    MapMgr.MonsterAIForPlunder(this);
        //}
    }

    private void ZombiesMonsterAutonomousBehaviour()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleMoment)
        {
            ResetIdleTimer();
            //MapMgr.ZombieAIForLevel(this);
        }
    }

    private void BaseCampAI()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleMoment)
        {
            ResetIdleTimer();
            //MapMgr.MonsterAIForBasecamp(this);
        }
    }

    private void LevelModeAI()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleMoment)
        {
            ResetIdleTimer();
            //MapMgr.MonsterAIForChapter(this);
        }
    }

    private void ResetIdleTimer()
    {
        idleTimer = 0;
        idleMoment = UnityEngine.Random.Range(2.5f, 5.5f);
    }

    public void IdleStatusQuit()
    {
        
    }

    public void SwitchStatusToIdle()
    {
        //if (collectArticleMapObject != null)
        //{
        //    UnloadingCollectObject(transform.position);
        //}
        //SwitchStatus(StatusName.Idle);
    }

    #endregion

    #region Drag状态相关

    public void DragStatusEnter()
    {
        targetObject = null;
        SetPlayerOperateTag(true);
        //雨云停止移动
        //RainCloudStopMove();
        //StopLeafMove();
        MapObejctTimeout(false);
        //if (ObjectType == Foundation_ObjectType)
        //{
        //    ClearBuilder();
        //}
        if (basicData.canHarvest)
        {
            //StopAllWorkingMonster(false);
        }
    }

    public void DragStatusEnd()
    {
        //雨云开始移动
        //RainCloudStartMove();
        //StartLeafMove();
        MapObejctTimeRecovery(false);
    }

    #endregion

    #region Select状态相关

    public void SelectStatusEnter()
    {
        targetObject = null;
        SetPlayerOperateTag(true);
        //中断龙的采集
        //Debug.Log("GL", "MapObject", string.Format("{0}进入选中状态", name));
        //EndCollect();
        //切换为冻结状态(如果生物正在采集那么采集也会被切断)
        //雨云停止移动
        //RainCloudStopMove();
        //冻结云1 使其刚体不能受力的作用
        //CloudOneFreeze();
        //卸掉龙采集成功的物体
        //UnloadingCollectObject(this.transform.position);
        //StopLeafMove();
    }

    public void SelectStatusQuit()
    {
        //雨云开始移动
        //RainCloudStartMove();
        //冻结云1 使其刚体恢复受力作用
        //CloudOneThaw();
        //StartLeafMove();
    }

    #endregion

    /// <summary>
    /// 时间暂停
    /// 如果为True 那么所有时间暂停 如果为False 只是在拖动情况下暂停数据
    /// </summary>
    public void MapObejctTimeout(bool stopAll = true)
    {
        if (stopAll)
        {
            isTimeStill = true;
            //CollectMgr.StopTimer();
        }

        //存活时间暂停
        //SurvivalTimeTimeOut();
        //采集时间暂停
        //CollectChargeTimeTimeOut();
        //点击事件暂停
        //ClickOutChargeTimeTimeOut();
        //暂停衍生充能时间计时
        //DerivativeChargeTimeTimeOut();

        //StopLeafMove();

        //僵尸暂停
        //StopPolluteSlider();
    }

    /// <summary>
    /// 时间恢复
    ///  如果为True 那么所有时间暂停 如果为False 只是在拖动情况下恢复数据
    /// </summary>
    public void MapObejctTimeRecovery(bool restoreAll=true)
    {
        if (restoreAll)
        {
            isTimeStill = false;
            //CollectMgr.StartTimer();
        }

        //存活时间恢复
        //RestoreSurvivalTimeTimeOut();
        //采集时间恢复
        //RestoreCollectChargeTimeTimeOut();
        //点击事件恢复
        ////RestoreClickOutChargeTimeTimeOut();
        //恢复衍生充能时间计时
        //RestoreDerivativeChargeTimeTimeOut();
        //StartLeafMove();
        //StartPolluteSlider();
    }

    /// <summary>
    /// 设置玩家操作标志
    /// </summary>
    /// <param name="isPlayerOperate"></param>
    public void SetPlayerOperateTag(bool isPlayerOperate)
    {
        if (IsMonster)
        {
            this.isPlayerOperate = isPlayerOperate;
        }
    }

    public void SetDoubleClickTag(bool isDoubleClick)
    {
        this.isDoubleClick = isDoubleClick;
    }

    /// <summary>
    /// 判断此物体是否执行双击行为
    /// </summary>
    public void PlayIdleAnimation()
    {
        //if (skeletonAnimation != null && skeletonAnimation.state != null)
        //{
        //    skeletonAnimation.timeScale = 1;
        //    skeletonAnimation.state.SetAnimation(0, ANI_NAME_IDLE, true);
        //    if (floatingAnim != null)
        //    {
        //        floatingAnim.Play();
        //    }
        //    else
        //    {
        //        floatingAnim = AnimMgr.PlayFloatingAnimation(skeletonAnimation.transform, entityTransformInitPos.y, 0.2f, 1f);
        //    }
        //}
    }

    public void PlayCollectAnimation()
    {
        //if (skeletonAnimation != null && skeletonAnimation.state != null)
        //{
        //    skeletonAnimation.timeScale = 1;
        //    skeletonAnimation.state.ClearTracks();
        //    skeletonAnimation.Skeleton.SetToSetupPose();
        //    skeletonAnimation.state.SetAnimation(0, ANI_NAME_COLLECT, true);
        //    floatingAnim.Pause();
        //}
    }

    public void PlayAttackAnimation()
    {
        //if (skeletonAnimation != null && skeletonAnimation.state != null)
        //{
        //    skeletonAnimation.state.ClearTracks();
        //    skeletonAnimation.Skeleton.SetToSetupPose();
        //    skeletonAnimation.state.SetAnimation(0, ANI_NAME_ATTACK, true);
        //    //当龙的攻击间隔不为0，并且攻击间隔<原动画时常时进行动画的缩处理
        //    if (IsMonster)
        //    {
        //        //if (bulletInterval != 0 && spineShootTime > bulletInterval)
        //        //{
        //        //    skeletonAnimation.timeScale = spineShootTime / bulletInterval;
        //        //}
        //    }
        //    floatingAnim.Pause();
        //}
    }

    public bool CanDoubleClick()
    {
        //掠夺模式下玩家无法响应双击操作
        //if (GlobalVariable.GameState == GameState.PlunderMode)
        //{
        return false;
        //}
        //else if (basicData.detachGrid)
        //{
        //    if (ObjectType == OBJECT_TYPE_EVIL_MONSTER)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //else
        //{
        //    //首先时净化过的
        //    if (IsPurified)
        //    {
        //        //其次是可建造、可采集、可被龙攻击的物品，才会执行双击行为
        //        if (ObjectType == Foundation_ObjectType)
        //        {
        //            return true;
        //        }
        //        else if (destructType == DestructType.DestroyedByAlly)
        //        {
        //            return true;
        //        }
        //        else if (basicData.canHarvest && CanCollected == CanNoBeCollectedReasonEnum.Normal)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }

    /// <summary>
    /// 启动类型为Entity的物体的待机动画
    /// </summary>
    private void EntityIdleAnimationStart()
    {
        Tweener tweener = null;
        switch (basicData.idleAnimation)
        {
            case 1: // 随机抖动
                //tweener = AnimMgr.PlayShakeAnimation(entityTransform, 0.5f).OnComplete(EntityIdleAnimationEnd);
                break;
            case 2: // 呼吸动画
                //启动这个动画时，可能与新生缩放动画冲突
                if (DOTween.IsTweening(entityTransform) == false)
                {
                    //tweener = AnimMgr.PlayBreathAnimation(entityTransform);
                }
                break;
            case 3: // 上下浮动动画
                //tweener = AnimMgr.PlayFloatingAnimation(entityTransform, entityTransformInitPos.y).SetDelay(UnityEngine.Random.Range(0, 3));
                break;
        }
        //注意：由于新生物体有统一的新生动画，待机动画需要等新生动画完成后再继续播放
        if (tweener != null)
        {
            PlayingEnityIdleAnimation = IdleAnimType.play;
        }
    }

    /// <summary>
    /// 重置类型为Entity的物体的待机动画间隔
    /// </summary>
    private void ResetEntityIdleAnimationInterval()
    {
        switch (basicData.idleAnimation)
        {
            case 1: // 随机抖动
                entityIdleAnimationInterval = UnityEngine.Random.Range(4, 30);
                break;
            case 2: // 呼吸动画
            case 3: // 上下浮动动画
                entityIdleAnimationInterval = 0;
                break;
        }
    }

    /// <summary>
    /// 类型为Entity的物体的待机动画播放结束，仅适用于不循环的待机动画
    /// </summary>
    private void EntityIdleAnimationEnd()
    {
        PlayingEnityIdleAnimation = IdleAnimType.stop;
    }

    /// <summary>
    /// 停止类型为Entity的物体的待机动画
    /// </summary>
    private void StopEntityIdleAnimation()
    {
        if (entityTransform != null)
        {
            DOTween.Kill(this.entityTransform);
            entityTransform.localPosition = entityTransformInitPos;
            entityTransform.localScale = Vector3.one;
        }
    }

    private void EntityIdleAnimationPause()
    {
        PlayingEnityIdleAnimation = IdleAnimType.pause;
        if (entityTransform != null)
        {
            DOTween.Kill(this.entityTransform);
            entityTransform.localPosition = entityTransformInitPos;
            entityTransform.localScale = Vector3.one;
        }
    }

    #region 物品提示动画

    //public void ShowMapObjectTip(SenceTipEnum type = SenceTipEnum.TextAndIcon)
    //{
    //    if (GlobalVariable.GameState == GameState.PlunderMode)
    //    {
    //        return;
    //    }
    //    if (IsPurified || basicData.detachGrid)
    //    {
    //        if (basicData.id == OBJECT_ID_SPILLING_STONE || basicData.id == OBJECT_ID_SPILLING_COIN)
    //        {
    //            if (clickToCollectTip == null)
    //            {
    //                clickToCollectTip = MapMgr.clickToCollectTipPool.GetInstance();
    //            }
    //            clickToCollectTip.ShowTip(tipMountPosition.position, "x" + remain_amount.ToString(), type, tipMountPosition);
    //        }
    //        else if (basicData.objectType == OBJECT_TYPE_BATTER_BALL)
    //        {
    //            if (clickToCollectTip == null)
    //            {
    //                clickToCollectTip = MapMgr.clickToCollectTipPool.GetInstance();
    //            }
    //            //clickToCollectTip.ShowTip(tipMountPosition.position, L10NMgr.GetText(22900002), SenceTipEnum.Text, tipMountPosition);
    //        }
    //    }

    //    if (MapMgr.isMapInitialzeLoading)
    //    {
    //        return;
    //    }
    //    if (GlobalVariable.GameState == GameState.MainSceneMode|| GlobalVariable.GameState == GameState.LevelModel)
    //    {
    //        if (IsPurified || basicData.detachGrid)
    //        {
    //            if (basicData.canClick)
    //            {
    //                TapEventData data = TableDataMgr.GetSingleTapEventData(basicData.id);
    //                if (data != null)
    //                {
    //                    if (data.showArrow > 0)
    //                    {
    //                        if (clickToCollectTip == null)
    //                        {
    //                            clickToCollectTip = MapMgr.clickToCollectTipPool.GetInstance();
    //                        }
    //                        if (data.showArrow == 1)
    //                        {
    //                            clickToCollectTip.ShowTip(tipMountPosition.position, L10NMgr.GetText(22900003), SenceTipEnum.TextAndIcon, tipMountPosition);
    //                            //显示5s后消失
    //                            if (MapMgr.AlreadyClickTip(basicData.id))
    //                            {
    //                                clickToCollectTip.HideAfterDelay(5);
    //                            }
    //                            else
    //                            {
    //                                MapMgr.AddClickTip(basicData.id);
    //                            }
    //                        }
    //                        else if (data.showArrow == 2)
    //                        {
    //                            //永久显示；
    //                            clickToCollectTip.ShowTip(tipMountPosition.position, L10NMgr.GetText(22900003), SenceTipEnum.TextAndIcon, tipMountPosition);
    //                        }
    //                        clickToCollectTip.gameObject.SetActive(true);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public void HideTip()
    {
   //     if (clickToCollectTip != null)
   //     {
			//clickToCollectTip.ClearHideTimer();
   //         MapMgr.clickToCollectTipPool.RecycleInstance(clickToCollectTip);
   //         clickToCollectTip = null;
   //     }
    }

    #endregion

    #region 断网临时保存计时器时间
    public void TemporarySaveTimerTime()
    {
        if (BasicData.canSpawn)
        {
            //if (TimerMgr.IsTimerOpen(derivativeChargeTimer))
            //{
            //    derivativeChargeTimeOutBrokenLine = TimerMgr.GetRemainTimeToInt(derivativeChargeTimer);
            //}
        }
        if (basicData.liveTime != null && basicData.liveTime.Length == 2)
        {
            //if (TimerMgr.IsTimerOpen(survivalTimer))
            //{
            //    survivalTimeOutTimeBrokenLine = TimerMgr.GetRemainTimeToInt(survivalTimer);
            //}

        }
        if (BasicData.canHarvest)
        {
            //if (TimerMgr.IsTimerOpen(collectChargeTimer))
            //{
            //    collectChargeTimeOutTimeBrokenLine = TimerMgr.GetRemainTimeToInt(collectChargeTimer);
            //}
        }
        if (BasicData.canClick)
        {
            //if (TimerMgr.IsTimerOpen(clickOutPutChargeTimer))
            //{
            //    clickOutPutChargeTimeBrokenLine = TimerMgr.GetRemainTimeToFloat(clickOutPutChargeTimer);
            //}
        }


    }
    #endregion
    #endregion

    public void SleepStatusEnter()
    {

    }

    public void SleepStatusQuit()
    {

    }

    public void SetMovingTag(bool state)
    {

    }

    public void StartSleep()
    {

    }
    
}

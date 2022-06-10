﻿using DG.Tweening;
using GameProto;
//using Spine;
//using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


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
    /// <summary>
    /// 用于对象池出池时使用
    /// </summary>
    public int tableID;

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
    /// 是否处于玩家操作状态
    /// </summary>
    private bool isPlayerOperate;
    private Transform magicBookTip;

    private MAP_OBJECT_STATUS mapObjectStatus= MAP_OBJECT_STATUS.Normal;

    #region 物体待机相关动画
    public enum IdleAnimType
    {
        play,
        pause,
        stop
    }

   
    public Transform entityTransform;
    public Tweener entityTweener;
    public Transform shadowTransform;
    #endregion
    public Transform tipMountPosition;

    public int Id { get { return basicData.id; } }

    public int ObjectType { get { return basicData.objectType; } }

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
    /// 是否为怪兽
    /// </summary>
    public bool IsMonster
    {
        get
        {
            return false;
        }
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
        transform.localScale = Vector3.one;
        //归池时将所有的物体的碰撞再次打开（被锁或者封印的地形的Collider是false）
        if (gameObject.GetComponent<Collider2D>() != null)
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }

        DOTween.Kill(transform);
        DOTween.Kill(entityTransform);

     
        ClearObjectAttribute();
        HideTip();

        
    }

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

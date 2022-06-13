using DG.Tweening;
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
    #region Unity Method

    private void Awake()
    {
    }

    #endregion

}

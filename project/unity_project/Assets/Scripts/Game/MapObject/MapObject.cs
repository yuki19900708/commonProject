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
        mpb = new MaterialPropertyBlock();
    }

    #endregion


    public MaterialPropertyBlock mpb;

    /// <summary>
    /// 物体的格子被净化了
    /// </summary>
    public void DisplayAsUnLockAndCured()
    {
        Renderer[] entityRendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in entityRendererList)
        {
            if (this.VegetationData != null)
            {
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", this.VegetationData.hueValue);
                mpb.SetFloat("_Saturation", this.VegetationData.saturation);
                mpb.SetFloat("_Value", this.VegetationData.brightness);
                renderer.SetPropertyBlock(mpb);
            }
            else
            {
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", 0);
                mpb.SetFloat("_Saturation", 1);
                mpb.SetFloat("_Value", 1);
                renderer.SetPropertyBlock(mpb);
            }
        }
    }

    public void ClearObjectAttribute()
    {
        Renderer[] entityRendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in entityRendererList)
        {
            renderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Hue", 0);
            mpb.SetFloat("_Saturation", 1);
            mpb.SetFloat("_Value", 1);
            renderer.SetPropertyBlock(mpb);
        }
    }
}

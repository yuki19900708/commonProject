using DG.Tweening;
using System;
using UnityEngine;

public partial class MapObject
{
    public Action<MapObject> Event_StartPurifier;

    private bool isPurifier = false;
    private bool purifierIsStart = false;

    public bool IsPurifier
    {
        get { return isPurifier; }
        set
        {
            isPurifier = value;
        }
    }

    public bool PurifierIsStart
    {
        get { return purifierIsStart; }
    }

    /// <summary>
    /// 播放净化器动画
    ///// </summary>
    //public void PlayPurifierAnimation()
    //{
    //    SurvivalTimeClear();
    ////    Dead();
    //}

    public void PurifierImmediatelyTriggerPurifier()
    {
        //VFXMgr.PlayPurifierActivatedVFX(this);
        if (Event_StartPurifier != null)
        {
            Event_StartPurifier(this);
        }
    }

    /// <summary>
    /// 立刻触发净化器
    /// </summary>
    public void ImmediatelyTriggerPurifier()
    {
        transform.DOKill();

        if (Event_StartPurifier != null)
        {
            Event_StartPurifier(this);
        }
        MapMgr.Instance.RemoveMapObjectEntity(this);
    }

    /// <summary>
    /// 净化器自动启动
    /// </summary>
    public void PurifierStart()
    {
        purifierIsStart = true;
        //Debug.Log("LY", string.Format("净化器的倒计时启动 - 还有{0}秒到达战场", SurvivalTime));
    } 

    public void ClearPurifier()
    {
        if (isPurifier)
        {
            transform.DOKill();
            purifierIsStart = false;
            IsPurifier = false;
            entityTransform.localScale = Vector3.one;
        }
    }
}

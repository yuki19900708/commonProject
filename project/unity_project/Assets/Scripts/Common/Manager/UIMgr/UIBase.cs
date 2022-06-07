using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class UIBase : BindableMonoBehaviour
{
    public event Action Event_Show;
    public event Action Event_Hide;
    public event Action Event_BecameTopCauseByOtherUIClose;

    public string Name
    {
        get;
        set;
    }

    /// <summary>
    /// 如果需要获取UI GameObject的名字，使用这个属性，避免直接使用Object.name属性，会引起GC Alloc
    /// </summary>
    public string ObjectName
    {
        get;
        set;
    }

    public UIBase PreviousUI
    {
        get;
        set;
    }

    public UIBase NextUI
    {
        get;
        set;
    }

    public virtual void Show()
    {
        UIMgr.ShowUI(this);
    }

    public void Hide()
    {
        UIMgr.HideUI(this);
    }

    #region 派生类需重写的方法，不建议外部调用，由UIMgr调用
    public virtual void OnCreate()
    {
    }

    public virtual void OnShow()
    {
        if (Event_Show != null)
        {
            Event_Show();
        }
    }

    /// <summary>
    /// 当UI因为其他UI的关闭而成为最项部UI时调用
    /// </summary>
    public virtual void OnBecameTopCauseByOtherUIClose()
    {
        if (Event_BecameTopCauseByOtherUIClose != null)
        {
            Event_BecameTopCauseByOtherUIClose();
        }
    }

    public virtual void OnHide()
    {
        if (Event_Hide != null)
        {
            Event_Hide();
        }
    }

    public virtual void OnShow(object param1)
    {
        if (Event_Show != null)
        {
            Event_Show();
        }
    }

    public virtual void OnSetActiveTrue()
    {
    }
    #endregion
}

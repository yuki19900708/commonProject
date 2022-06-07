using System;
using UnityEngine;

public abstract class APMessageBoxBase :
#if INHERIT_UIBASE
UIBase
#else
MonoBehaviour
#endif
{
    public event Action Event_CloseMessageBox;
    public bool isModal;

    private void Awake()
    {
        InitializeMessageBox();
    }

    #region Virtual Method

    protected virtual void InitializeMessageBox()
    {

    }

    protected virtual void CloseMessageBox()
    {
        gameObject.SetActive(false);
        if (Event_CloseMessageBox != null)
        {
            Event_CloseMessageBox();
        }
    }

    #endregion

}
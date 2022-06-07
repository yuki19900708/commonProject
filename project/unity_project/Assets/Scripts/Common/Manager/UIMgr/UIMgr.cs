using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour
{
    public static event Action<UIBase> Event_UICreated;
    public static event Action<UIBase> Event_UIShow;
    public static event Action<UIBase> Event_UIHide;
    public static event Action<UIBase> Event_UIBecameTopCauseByOtherUIClose;

    public static UIMgr instance;
    /// <summary>
    /// 所有已经创建过UI实例字典（其中的UI实例不一定处于显示状态）
    /// </summary>
    public static Dictionary<string, UIBase> UIDict = new Dictionary<string, UIBase>();

    /// <summary>
    /// 所有处于显示状态的UI集合（双向链表结构）
    /// </summary>
    protected static LinkedList<UIBase> UILinkedList = new LinkedList<UIBase>();

    /// <summary>
    /// UI 根节点
    /// </summary>
    public Transform UIRoot;
    /// <summary>
    /// 动画使用的曲线
    /// </summary>
    public AnimationCurve useCurve;
    /// <summary>
    /// 当前界面最上层的UI
    /// </summary>
    public static UIBase CurrentUI
    {
        get
        {
            return UILinkedList.Count > 0 ? UILinkedList.Last.Value : null;
        }
    }
    
    void Awake()
    {
        instance = this;
    }

    #region Public Methods

    public static void ShowUI(UIBase ui)
    {
        Debug.Log("UIMgr ShowUI: " + ui.Name);
        if (CurrentUI != null && CurrentUI != ui)
        {
            CurrentUI.NextUI = ui;
            ui.PreviousUI = CurrentUI;
        }
        UILinkedList.AddLast(ui);
        ui.gameObject.SetActive(true);
        ui.OnShow();
        if (Event_UIShow != null)
        {
            Event_UIShow(ui);
        }
    }

    public static void ShowUI(UIBase ui, object param)
    {
        Debug.Log("UIMgr ShowUI: " + ui.Name);
        if (CurrentUI != null && CurrentUI != ui)
        {
            CurrentUI.NextUI = ui;
            ui.PreviousUI = CurrentUI;
        }
        UILinkedList.AddLast(ui);
        ui.gameObject.SetActive(true);
        ui.OnShow(param);
        if (Event_UIShow != null)
        {
            Event_UIShow(ui);
        }
    }

    public static UIBase ShowUI(string name)
    {
        if (CurrentUI != null && CurrentUI.name == name)
        {
            return CurrentUI;
        }
        else
        {
            UIBase ui = null;
            if (UIDict.ContainsKey(name))
            {
                ui = UIDict[name];
            }
            else
            {
                ui = CreateUI(name);
            }

            if(ui != null)
            {
                ShowUI(ui);
            }
            return ui;
        }
    }

    public static UIBase ShowUI(string name, object param)
    {
        if (CurrentUI != null && CurrentUI.name == name)
        {
            return CurrentUI;
        }
        else
        {
            UIBase ui = null;
            if (UIDict.ContainsKey(name))
            {
                ui = UIDict[name];
            }
            else
            {
                ui = CreateUI(name);
            }

            if (ui != null)
            {
                ShowUI(ui, param);
            }
            return ui;
        }
    }

    public static void HideUI(UIBase ui)
    {
        //Debug.Log("LFL", "UI被隐藏了" + ui.name);
        if (CurrentUI == ui)
        {
            //当前UI不是头结点
            if (CurrentUI.PreviousUI != null)
            {
                CurrentUI.PreviousUI.NextUI = null;
            }

            CurrentUI.PreviousUI = null;
            CurrentUI.NextUI = null;
            UILinkedList.RemoveLast();
        }
        else
        {
            if (UILinkedList.Contains(ui))
            {
                //如果头结点，更新后一个UI的PreviousUI为空
                if (ui.PreviousUI == null)
                {
                    if (ui.NextUI != null)
                    {
                        ui.NextUI.PreviousUI = null;
                    }
                }
                else
                {
                    //如果中间结点，更新前后UI的对应的Next与PreviousUI；
                    ui.PreviousUI.NextUI = ui.NextUI;
                    ui.NextUI.PreviousUI = ui.PreviousUI;
                }

                if (UILinkedList.Count > 0)
                {
                    UILinkedList.Remove(ui);
                }
            }
        }
        ui.gameObject.SetActive(false);
        ui.OnHide();
        if (Event_UIHide != null)
        {
            Event_UIHide(ui);
        }
        if (CurrentUI != null)
        {
            CurrentUI.OnBecameTopCauseByOtherUIClose();
        }
        if (Event_UIBecameTopCauseByOtherUIClose != null)
        {
            Event_UIBecameTopCauseByOtherUIClose(ui);
        }
    }

    public static void HideUI(string name)
    {
        if (CurrentUI == null)
        {
            return;
        }
        if (CurrentUI.Name == name)
        {
            HideUI(CurrentUI);
        }
        else
        {
            if (UIDict.ContainsKey(name))
            {
                HideUI(UIDict[name]);
            }
            else
            {
                Debug.LogWarning("试图Hide不在实例字典中的UI: " + name + "最上层的UI: " + CurrentUI.Name);
            }
        }
    }

    public static bool IsOpen(string uiName)
    {
        if (UILinkedList.Count > 0)
        {
            foreach (UIBase ui in UILinkedList)
            {
                if (ui.Name == uiName)
                    return true;
            }
        }
        return false;
    }

    public static T GetUI<T>(string uiName) where T : class
    {
        if (UILinkedList.Count > 0)
        {
            foreach (UIBase ui in UILinkedList)
            {
                if (ui.Name == uiName)
                    return ui as T;
            }
        }
        return default(T);
    }

    public static void SetActive(string uiName, bool isActive, bool isSpecial = false)
    {
        if (UIDict.ContainsKey(uiName))
        {
            UIDict[uiName].gameObject.SetActive(isActive);
            if (isActive)
            {
                if (isSpecial)
                {
                    UIDict[uiName].OnSetActiveTrue();
                }
                else
                {
                    UIDict[uiName].OnShow();
                }
            }
        }
    }

    public static void DestoryUI(string uiName)
    {
        if (UIDict.ContainsKey(uiName))
        {
            GameObject.Destroy(UIDict[uiName].gameObject);
            UIDict.Remove(uiName);
        }
    }

    public static void BackwardToUI(UIBase ui)
    {
        if (UILinkedList.Contains(ui))
        {
            while (CurrentUI != ui)
            {
                HideUI(CurrentUI);
            }
        }
        else
        {
            Debug.LogError("回退目标不存在: " + ui.Name);
        }
    }

    public static void BackwardToUI(string name)
    {
        if (UIDict.ContainsKey(name))
        {
            BackwardToUI(UIDict[name]);
        }
        else
        {
            Debug.LogError("回退目标不存在: " + name);
        }
    }

    public static void HideAllUI()
    {
        while (CurrentUI != null)
        {
            CurrentUI.Hide();
        }
    }
    #endregion

    private static UIBase CreateUI(string name)
    {
        GameObject prefab = ResMgr.Load<GameObject>(name);
        if (prefab != null)
        {
            GameObject go = GameObject.Instantiate(prefab, instance.UIRoot);
            go.name = name;
            go.gameObject.SetActive(false);

            //加这段代码因为UI上要使用特效， 所以画布rendermode从overlay改为screenspace
            Canvas canvas = go.GetComponent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
            }
            CanvasScaler canvasScaler = go.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1334, 750);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            UIBase ui = go.GetComponent<UIBase>();
            ui.Name = name;
            UIDict.Add(name, ui);
            ui.ObjectName = go.name;
            ui.OnCreate();
            UIDef.SetOrderLayer(ui, name);
            if (Event_UICreated != null)
            {
                Event_UICreated(ui);
            }
            return ui;
        }
        else
        {
            Debug.LogError("要创建的UI不存在: " + name);
            return null;
        }
    }

}

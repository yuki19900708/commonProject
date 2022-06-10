using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public enum FingerMgrOperation
{
    None,
    /// <summary>
    /// 操作物体
    /// </summary>
    OperationObject,
    /// <summary>
    /// 操作地图
    /// </summary>
    OperationMap,

}

[RequireComponent(typeof(FingerDownDetector))]
[RequireComponent(typeof(FingerUpDetector))]
[RequireComponent(typeof(LongPressRecognizer))]
[RequireComponent(typeof(DragRecognizer))]
[RequireComponent(typeof(PinchRecognizer))]
[RequireComponent(typeof(FingerGestures))]
[RequireComponent(typeof(ScreenRaycaster))]
public class FingerMgr : MonoBehaviour {
    public static FingerMgr Instance;
    public Action Event_CloseMoreFeatures;
    /// <summary>
    /// 拖动
    /// </summary>
    public Action<FingerMgrOperation, DragGesture> Event_Drag;
    /// <summary>
    /// 缩放
    /// </summary>
    public Action<FingerMgrOperation,PinchGesture> Event_Pinch;
    /// <summary>
    /// 长按
    /// </summary>
    public Action<FingerMgrOperation,LongPressGesture> Event_LongPress;
    /// <summary>
    /// 按下
    /// </summary>
    public Action<FingerMgrOperation,FingerDownEvent> Event_FingerDown;
    /// <summary>
    /// 抬起
    /// </summary>
    public Action<FingerMgrOperation,FingerUpEvent> Event_FingerUp;

    public FingerMgrOperation fingerMgrOperation = FingerMgrOperation.None;
    private bool disableGesture = false;
    private bool draging = false;
    void Awake()
    {
        Instance = this;
    }
    private void OnDrag(DragGesture gesture)
    {
        if (Event_CloseMoreFeatures != null)
        {
            Event_CloseMoreFeatures();
        }
        if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            if (Event_Drag != null)
            {
                Event_Drag(fingerMgrOperation, gesture);
            }
            draging = false;
        }
        if (disableGesture)
        {
            return;
        }

        if (SceneObjectGestureMgr.Instance.dragMgr.dragObject != null
            && SceneObjectGestureMgr.Instance.dragMgr.startResponse)
        {
              CameraGestureMgr.Instance.MoveCameraToIncludePosition();
        }

        //if (DisableGestureFunction(true))
        //{
        //    return;
        //}
        if (gesture.Phase != ContinuousGesturePhase.Ended)
        {
            if (Event_Drag != null)
            {
                Event_Drag(fingerMgrOperation, gesture);
            }
        }
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            draging = true;
        }

    }

    private void OnPinch(PinchGesture gesture)
    {
        if (Event_CloseMoreFeatures != null)
        {
            Event_CloseMoreFeatures();
        }
        if (disableGesture)
        {
            fingerMgrOperation = FingerMgrOperation.None;
            return;
        }
        if (DisableGestureFunction(false))
        {
            fingerMgrOperation = FingerMgrOperation.None;
            return;
        }
        fingerMgrOperation = FingerMgrOperation.OperationMap;
        if (Event_Pinch != null)
        {
            Event_Pinch(fingerMgrOperation,gesture);
        }
    }

    private void OnLongPress(LongPressGesture gesture)
    {
        if (disableGesture)
        {
            return;
        }
        if (Event_LongPress != null)
        {
            Event_LongPress(fingerMgrOperation,gesture);
        }
    }

    void OnFingerDown(FingerDownEvent e)
    {
        if (disableGesture)
        {
            fingerMgrOperation = FingerMgrOperation.None;
            return;
        }
        if (DisableGestureFunction(false))
        {
            fingerMgrOperation = FingerMgrOperation.None;
            return;
        }

#if UNITY_EDITOR
        if (TerrainEditorUICtrl.Instance)
        {
            if (Input.GetMouseButton(2) == false)
            {
                if (TerrainEditorUICtrl.Instance.isEditorToggle.isOn)
                {
                    fingerMgrOperation = FingerMgrOperation.OperationObject;
                }
                else
                {
                    fingerMgrOperation = FingerMgrOperation.OperationMap;
                }
            }
            else
            {

            }
        }
        else
#endif
        {

            if (e.Selection != null)
            {
                //如果碰触的手指大于1时停止
                if (Input.touchCount > 1)
                {
                    fingerMgrOperation = FingerMgrOperation.OperationMap;
                }
                else if (e.Selection.layer == LayerMask.NameToLayer("CollectBall"))
                {
                    if (Event_CloseMoreFeatures != null)
                    {
                        Event_CloseMoreFeatures();
                    }
                    //e.Selection.GetComponent<CollectBall>().UnloadingMapObject();
                    fingerMgrOperation = FingerMgrOperation.OperationMap;
                }
                else if (e.Selection.layer == LayerMask.NameToLayer("DragObject") || e.Selection.layer == LayerMask.NameToLayer("Cloud"))
                {
                    if (e.Selection.GetComponent<MapObject>().IsCanBeSelected == false)
                    {
                        fingerMgrOperation = FingerMgrOperation.OperationMap;
                    }
                    else if (JugeObjectStateInGrid(e.Selection.GetComponent<MapObject>()))
                    {
                        if (Event_CloseMoreFeatures != null)
                        {
                            Event_CloseMoreFeatures();
                        }
                        fingerMgrOperation = FingerMgrOperation.OperationObject;
                    }
                    else
                    {
                        fingerMgrOperation = FingerMgrOperation.OperationMap;
                    }
                }
                else
                {
                    fingerMgrOperation = FingerMgrOperation.OperationMap;
                }
            }
            else
            {
                fingerMgrOperation = FingerMgrOperation.OperationMap;
            }
        }
        if (Event_FingerDown != null)
        {
            Event_FingerDown(fingerMgrOperation,e);
        }
    }

    void OnFingerUp(FingerUpEvent e)
    {
        if (disableGesture)
        {
            return;
        }
        if (Event_FingerUp != null)
        {
            Event_FingerUp(fingerMgrOperation,e);
        }
        if(draging == false)
        {
            fingerMgrOperation = FingerMgrOperation.None;
        }
    }

    /// <summary>是否禁用手势插件 </summary>
    private bool DisableGestureFunction(bool isDragging)
    {
        if (ReturnIsTouchOnUGUI(isDragging))
        {
            return true;
        }
        return false;
    }

    /// <summary>是否触控位置在UGUI控件上 </summary>
    private bool ReturnIsTouchOnUGUI(bool isDragging)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //将点击位置的屏幕坐标赋值给点击事件  
        Vector2 position = Input.mousePosition;
        eventDataCurrentPosition.position = position;
        eventDataCurrentPosition.pressPosition = position;

        List<RaycastResult> results = new List<RaycastResult>();
        //向点击处发射射线  
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        int mount = 0;
        for (int i = 0; i < results.Count; i++)
        {
            if (isDragging == false || LayerMask.LayerToName(results[i].gameObject.layer) != "UIInGame")
            {
                mount++;
            }
        }
        return mount > 0;
    }

    private bool JugeObjectStateInGrid(MapObject a)
    {
        bool canDrag = false;
        //if (GlobalVariable.GameState == GameState.PlunderMode)
        //{
        //    return false;
        //}
        if (a.BasicData.area.Length >= 1)
        {
            int mount = 0;
            for (int i = 0; i < a.StaticMapGridList.Count; i++)
            {
                
                    mount++;
                
            }

            if (mount == a.StaticMapGridList.Count)
            {
                canDrag = true;
            }
        }
        else
        {
            canDrag = true;
        }
        return canDrag;
    }
}

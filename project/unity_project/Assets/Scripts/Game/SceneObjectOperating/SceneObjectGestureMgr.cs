using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlObjectType
{
    HavePosition,
    NoPostion,
}

[RequireComponent(typeof(DragObjectMgr))]
public class SceneObjectGestureMgr : MonoBehaviour
{
    public static SceneObjectGestureMgr Instance;

    public DragObjectMgr dragMgr;
    private Camera mainCamera;
    private MapObject selectMapObject;

    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                mainCamera = CameraGestureMgr.Instance.gameCamera;
            }
            return mainCamera;
        }
    }

    void Awake()
    {
        Instance = this;
        dragMgr = GetComponent<DragObjectMgr>();
    }


    void Start()
    {

        //注册拖动事件
        FingerMgr.Instance.Event_Drag += Event_OnDrag;
    }


    private void MapEditorDrag(FingerMgrOperation type, DragGesture gesture)
    {
        if (type == FingerMgrOperation.OperationObject)
        {
            if (gesture.Phase == ContinuousGesturePhase.Started)
            {
#if UNITY_EDITOR
                if (Input.GetMouseButton(2) == false && TerrainEditorUICtrl.Instance)
                {
                    if (TerrainEditorUICtrl.IsEditor)
                        TerrainEditorUICtrl.Instance.DragStart();
                    return;
                }
#endif
            }
            if (gesture.Phase == ContinuousGesturePhase.Updated)
            {
#if UNITY_EDITOR
                if (Input.GetMouseButton(2) == false && TerrainEditorUICtrl.Instance)
                {
                    if (TerrainEditorUICtrl.IsEditor)
                        TerrainEditorUICtrl.Instance.Drag();
                    return;
                }
#endif
            }
            if (gesture.Phase == ContinuousGesturePhase.Ended)
            {
#if UNITY_EDITOR
                if (Input.GetMouseButton(2) == false && TerrainEditorUICtrl.Instance)
                {
                    if (TerrainEditorUICtrl.IsEditor)
                        TerrainEditorUICtrl.Instance.DragEnd();
                    return;
                }
#endif
            }
        }
    }

    private void Event_OnDrag(FingerMgrOperation type, DragGesture gesture)
    {
        MapEditorDrag(type, gesture);
        ////掠夺模式下无法拖动任何物体
        //if (GlobalVariable.GameState == GameState.PlunderMode)
        //{
        //    return;
        //}

        //物体不能被拖动时不能调用
        if (selectMapObject == null
         || selectMapObject.BasicData.canDrag == false
         || selectMapObject.CurrentItemStatus == ItemStatus.BeAttacking
         || selectMapObject.CurrentItemStatus == ItemStatus.AttackTarget)
        {
            dragMgr.dragObject = null;
            return;
        }

        if (type == FingerMgrOperation.OperationObject)
        {
            if (gesture.Phase == ContinuousGesturePhase.Started)
            {
                if (selectMapObject != null)
                {
                    selectMapObject.IsDraged = true;
                }

                Vector3 postion = MainCamera.ScreenToWorldPoint(gesture.Position);
                dragMgr.DragStart(postion);
            }
            if (gesture.Phase == ContinuousGesturePhase.Updated)
            {
                if (selectMapObject != null)
                {
                    Vector3 postion = MainCamera.ScreenToWorldPoint(gesture.Position);
                    dragMgr.DragObject(postion);
                }
            }
            if (gesture.Phase == ContinuousGesturePhase.Ended)
            {
                Vector3 postion = MainCamera.ScreenToWorldPoint(gesture.Position);
                if (selectMapObject != null)
                {
                    selectMapObject.IsDraged = false;
                    selectMapObject = null;
                }
                dragMgr.DragEnd(postion);
                dragMgr.dragObject = null;
            }
        }
    }

}

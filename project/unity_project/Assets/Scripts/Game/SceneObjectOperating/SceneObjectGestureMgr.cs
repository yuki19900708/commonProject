using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlObjectType
{
    HavePosition,
    NoPostion,
}

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
        FingerMgr.Instance.Event_Drag += MapEditorDrag;
    }


    private void MapEditorDrag(FingerMgrOperation type, DragGesture gesture)
    {
        if (TerrainEditorUICtrl.Instance == null || TerrainEditorUICtrl.IsEditor == false)
        {
            return;
        }
        if (type == FingerMgrOperation.OperationObject)
        {
            if (gesture.Phase == ContinuousGesturePhase.Started)
            {
                if (Input.GetMouseButton(2) == false)
                {
                    TerrainEditorUICtrl.Instance.DragStart();
                    return;
                }
            }
            if (gesture.Phase == ContinuousGesturePhase.Updated)
            {
                if (Input.GetMouseButton(2) == false)
                {
                    TerrainEditorUICtrl.Instance.Drag();
                    return;
                }
            }
            if (gesture.Phase == ContinuousGesturePhase.Ended)
            {
                if (Input.GetMouseButton(2) == false)
                {
                    TerrainEditorUICtrl.Instance.DragEnd();
                    return;
                }
            }
        }
    }

}

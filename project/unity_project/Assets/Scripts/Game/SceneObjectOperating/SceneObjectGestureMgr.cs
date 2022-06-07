using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlObjectType
{
    HavePosition,
    NoPostion,
}
[RequireComponent(typeof(ClickObjectMgr))]
[RequireComponent(typeof(DoubleClickMgr))]
[RequireComponent(typeof(DragObjectMgr))]
public class SceneObjectGestureMgr : MonoBehaviour
{
    public static SceneObjectGestureMgr Instance;
    /// <summary>
    /// Outline 的缓存池
    /// </summary>
    public static MonoBehaviourPool<DragOutline> DragOutlinePool;
    // 拖动物体的outline
    public static List<DragOutline> mapObjectOutLine = new List<DragOutline>();

    // 地形上的Outline
    public static List<DragOutline> mapGridOutLine = new List<DragOutline>();

    // 合成的Outline
    public static List<DragOutline> mergeOutLine = new List<DragOutline>();


    // 物品提示的
    //public static List<ClickToCollectTip> DragMapObjectTip = new List<ClickToCollectTip>();
    /// <summary>
    /// 点击事件响应的时间间隔
    /// </summary>
    public const float PRESS_UP_TIME = 1f;
    public ClickObjectMgr clickMgr;
    public DoubleClickMgr doubleClickMgr;
    public DragObjectMgr dragMgr;
    private Camera mainCamera;
    private MapObject selectMapObject;
    float PressUpTimeInterval = 0;

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
        clickMgr = GetComponent<ClickObjectMgr>();
        doubleClickMgr = GetComponent<DoubleClickMgr>();
        dragMgr = GetComponent<DragObjectMgr>();

        ResMgr.Event_LocalResInitFinish += Init;
    }

    private void Init()
    {
        DragOutlinePool = new MonoBehaviourPool<DragOutline>(ResMgr.Load<DragOutline>("DragOutline"), CommonObjectMgr.PoolParent);
    }

    void Start()
    {

        //注册拖动事件
        FingerMgr.Instance.Event_Drag += Event_OnDrag;
        FingerMgr.Instance.Event_FingerDown += Event_FingerDown;
        FingerMgr.Instance.Event_FingerUp += Event_FingerUp;
    }

    void Update()
    {
        PressUpTimeInterval += Time.deltaTime;
    }

    void OnDestory()
    {

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

        PressUpTimeInterval = PRESS_UP_TIME+0.1f;
        if (type == FingerMgrOperation.OperationObject)
        {
            if (gesture.Phase == ContinuousGesturePhase.Started)
            {
                if (selectMapObject != null)
                {
                    //推出选择状态，进入拖拽状态
                    if (selectMapObject.BasicData.objectType != MapObject.OBJECT_TYPE_EVIL_MONSTER)
                    {
                        selectMapObject.IsSelected = false;
                    }
                    selectMapObject.IsDraged = true;
                    clickMgr.choiceMapObject = null;
                    //按下了一个物体
                    clickMgr.ClickObject(Vector3.zero, null);
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
                    clickMgr.choiceMapObject = null;
                    clickMgr.ClickObject(Vector3.zero, null);
                }
                dragMgr.DragEnd(postion);
                dragMgr.dragObject = null;
            }
        }
    }

    private void Event_FingerUp(FingerMgrOperation type, FingerUpEvent obj)
    {
#if UNITY_EDITOR
        if (TerrainEditorUICtrl.Instance)
        {
            return;
        }
#endif
        //掠夺模式下无法响应抬起操作
        //if (GlobalVariable.GameState == GameState.PlunderMode)
        //{
        //    return;
        //}


        if (type == FingerMgrOperation.OperationObject)
        {
            if (PressUpTimeInterval < PRESS_UP_TIME)
            {
                //Vector3 postion = MainCamera.ScreenToWorldPoint(obj.Position);
                ///掠夺模式下不响应点击采集
                //if (GlobalVariable.GameState == GameState.PlunderMode)
                //{
                //    return;
                //}
                //else
                //{
                //    if (selectMapObject != null
                //        && selectMapObject.CurrentItemStatus != ItemStatus.BeAttacking
                //        && selectMapObject.CurrentItemStatus != ItemStatus.AttackTarget)
                //    {
                //        clickMgr.ClickObjectInPeriodTimeLiftUp(postion, selectMapObject);
                //    }
                //    else if (selectMapObject != null
                //        && (selectMapObject.CurrentItemStatus == ItemStatus.BeAttacking ||
                //         selectMapObject.CurrentItemStatus == ItemStatus.AttackTarget) 
                //         && selectMapObject.BasicData.destructType == 2&& selectMapObject.BasicData.detachGrid==false)
                //    {
                //        //生物在攻击魔物是，魔物响应点击事件
                //        clickMgr.ClickObjectInPeriodTimeLiftUp(postion, selectMapObject);
                //    }
                //}
            }

            if (selectMapObject != null && selectMapObject.BasicData.objectType == MapObject.OBJECT_TYPE_EVIL_MONSTER)
            {
                
            }
            else if (selectMapObject != null&& selectMapObject.BasicData.detachGrid&& selectMapObject.IsMonster==false)
            {
                selectMapObject.IsSelected = false;
            }
        }
    }

    private void Event_FingerDown(FingerMgrOperation type, FingerDownEvent obj)
    {
#if UNITY_EDITOR
        if (TerrainEditorUICtrl.Instance)
        {
            return;
        }
#endif
        //if (clickMgr.confirmClickGameObject!=null)
        //{
        //    clickMgr.confirmClickGameObject.gameObject.SetActive(false);
        //}
        //if (type != FingerMgrOperation.OperationObject)
        //{
        //    //掠夺模式下不显示未解锁区域的进度
        //    if (GlobalVariable.GameState != GameState.PlunderMode)
        //    {
        //        //在这里判断这个点是否点击到地形
        //        Vector3 postion = MainCamera.ScreenToWorldPoint(obj.Position);
        //        clickMgr.ClickUnlockButNotCuredObject(postion);
        //        //没有点击到物体选择框消失
        //        clickMgr.ClickObject(Vector3.zero, null);
        //        if (selectMapObject != null)
        //        {
        //            if (selectMapObject.IsSelected)
        //            {
        //                if (selectMapObject.BasicData.objectType != MapObject.OBJECT_TYPE_EVIL_MONSTER)
        //                {
        //                    selectMapObject.IsSelected = false;
        //                }
        //            }
        //            selectMapObject = null;
        //        }
        //        if (Input.touchCount > 1)
        //        {
        //            if (dragMgr.dragObject != null)
        //            {
        //                if (dragMgr.dragObject.IsDraged)
        //                {
        //                    Debug.Log("LFL", "Plunder", "多手指操作 放到当前位置");
        //                    dragMgr.DragEnd(dragMgr.transform.position);
        //                    dragMgr.dragObject.IsDraged = false;
        //                    dragMgr.dragObject = null;
        //                }
        //            }

        //            if (selectMapObject != null)
        //            {
        //                selectMapObject.IsDraged = false;
        //                selectMapObject = null;
        //            }
        //        }
        //    }
        //}

        dragMgr.dragObject = null;

        if (type == FingerMgrOperation.OperationObject)
        {
            if (obj.Selection != null)
            {
                PressUpTimeInterval = 0;
                MapObject newSelectMapObject = obj.Selection.GetComponent<MapObject>();
                //物体被点击以后提示显示
                if (newSelectMapObject != null)
                {
                    if (newSelectMapObject.Id != MapObject.OBJECT_ID_SPILLING_COIN
                      && newSelectMapObject.Id != MapObject.OBJECT_ID_SPILLING_STONE)
                    {
                        newSelectMapObject.HideTip();
                    }
                }

                if (newSelectMapObject != null 
                    &&(newSelectMapObject.CurrentItemStatus == ItemStatus.BeAttacking
                    || newSelectMapObject.CurrentItemStatus == ItemStatus.AttackTarget))
                {
                    //当该物体被魔物攻击时，只弹出物品说明
                    //点击了一个物体
                    doubleClickMgr.DoubleClick(Vector2.zero, newSelectMapObject.gameObject);
                    Vector3 postionCurrent = MainCamera.ScreenToWorldPoint(obj.Position);
                    clickMgr.ClickObject(postionCurrent, newSelectMapObject);
                    if (newSelectMapObject.BasicData.destructType != 2)
                    {
                        newSelectMapObject = null;
                        selectMapObject = null;
                    }
                    else
                    {
                        selectMapObject = newSelectMapObject;
                    }
                    dragMgr.dragObject = null;
                    return;
                }
                if (newSelectMapObject != null)
                {
                    if (selectMapObject != null)
                    {
                        if (selectMapObject.BasicData.objectType != MapObject.OBJECT_TYPE_EVIL_MONSTER)
                        {
                            selectMapObject.IsSelected = false;
                        }
                    }

                    if (newSelectMapObject.BasicData.objectType != MapObject.OBJECT_TYPE_EVIL_MONSTER)
                    {
                        newSelectMapObject.IsSelected = true;
                    }

                    selectMapObject = newSelectMapObject;
                }
                else
                {
                    //Debug.Log("LFL", "Plunder", "当前选择的物体没有MapObject");
                }


                //if (GlobalVariable.GameState != GameState.PlunderMode)
                //{
                //    dragMgr.dragObject = obj.Selection.GetComponent<MapObject>();
                //}
                //else
                //{
                //    dragMgr.dragObject = null;
                //}

                Vector3 postion = MainCamera.ScreenToWorldPoint(obj.Position);
               
                //按下了一个物体
                clickMgr.ClickObject(postion, selectMapObject);
                //if (GlobalVariable.GameState != GameState.PlunderMode)
                //{
                //    //点击了一个物体
                //    doubleClickMgr.DoubleClick(postion, selectMapObject.gameObject);
                //}
            }
            else
            {
                //没有点击到物体选择框消失
                clickMgr.ClickObject(Vector3.zero, null);
                if (selectMapObject != null)
                {
                    if (selectMapObject.BasicData.objectType != MapObject.OBJECT_TYPE_EVIL_MONSTER)
                    {
                        selectMapObject.IsSelected = false;
                    }
                    selectMapObject = null;
                }
            }
        }
    }

    /// <summary>
    /// 初始化OutLine
    /// </summary>
    public static void RecycleInstanceAllOutLine()
    {
        for (int i = 0; i < mapObjectOutLine.Count; i++)
        {
            DragOutlinePool.RecycleInstance(mapObjectOutLine[i]);
        }
        mapObjectOutLine.Clear();
    }

    /// <summary>
    /// 初始化OutLine
    /// </summary>
    public static void RecycleInstanceAllMapGridOutLine()
    {
        for (int i = 0; i < mapGridOutLine.Count; i++)
        {
            DragOutlinePool.RecycleInstance(mapGridOutLine[i]);
        }
        mapGridOutLine.Clear();
    }

    public static void RecycleAllMergeOutLine()
    {
        for (int i = 0; i < mergeOutLine.Count; i++)
        {
            DragOutlinePool.RecycleInstance(mergeOutLine[i]);
        }
        mergeOutLine.Clear();
    }

    /// <summary>
    /// 回收所有Drag时提示
    /// </summary>
    public static void RecycleAllDragMapObjectTip()
    {
        //for (int i = 0; i < SceneObjectGestureMgr.DragMapObjectTip.Count; i++)
        //{
        //    MapMgr.clickToCollectTipPool.RecycleInstance(SceneObjectGestureMgr.DragMapObjectTip[i]);
        //}
        //DragMapObjectTip.Clear();
    }
}

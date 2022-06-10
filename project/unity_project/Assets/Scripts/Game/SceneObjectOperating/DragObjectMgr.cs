using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
/// <summary>
/// 物体拖动控制
/// </summary>
public class DragObjectMgr : BindableMonoBehaviour
{
    public static DragObjectMgr Instance;
    public const float CHECK_DISTANCE = 0.35F;
    /// <summary>
    /// 当拖动结束时事件
    /// </summary>
    public Action<Vector2, UnityEngine.Object> Event_DragObjectEnd;
    /// <summary>
    /// 拖动的游戏物体实例
    /// </summary>
    public MapObject dragObject;
    /// <summary>
    /// 玩家拖动游戏物体时，Drag在UI响应DragStart但Mgr没有获取到
    /// 导致Drag的边框没有跟随（如果响应过DragStart那么走正常逻辑）
    /// </summary>
    public bool startResponse = false;
    /// <summary>
    /// 拖动的物体挂载点
    /// </summary>
    // public GameObject dragMoveGameObject;
    /// <summary>
    /// 拖动物体的起点
    /// </summary>
    //private MapGrid startMapGrid;
    /// <summary>
    /// 拖动最后的有效点
    /// </summary>
    //private MapGrid lastEffectiveArea;
    /// <summary>
    /// 当前拖动到的MapGrid
    /// </summary>
    private MapGrid moveMapGrid;
    /// <summary>
    /// 变更前的位置
    /// </summary>
    private MapGrid changeMapGrid;
    /// <summary>
    /// 拖动的Tweener动画
    /// </summary>
    private Tweener dragTweener;
    ///// <summary>
    ///// Outline 的缓存池
    ///// </summary>
    //public static MonoBehaviourPool<DragOutline> DragOutlinePool;
    //#region 拖动物体的outline
    //public List<DragOutline> mapObjectOutLine = new List<DragOutline>();
    //#endregion
    //#region 地形上的Outline
    //public List<DragOutline> mapGridOutLine = new List<DragOutline>();
    //#endregion
    /// <summary>
    /// 判断他的拖动方式
    /// </summary>
    public bool hasPostion = false;
    /// <summary>
    /// 拖动的偏移位置
    /// </summary>
    public Vector2 offsetposition;
    /// <summary>
    /// 向合成发送位置信息
    /// </summary>
    public List<Vector2> dragObjectAllPostion = new List<Vector2>();
    //private bool canPutInto = false;
    private Vector3 dragEndPostion;

    void Awake()
    {
        Instance = this;
        //MapMgr.Event_MapObjectRemove += Event_MapObjectRemove;
    }

    private void Event_MapObjectRemove(MapObject obj)
    {
        if (dragObject != null && dragObject == obj)
        {
            dragObject.transform.parent = null;
            dragObject = null;
            SceneObjectGestureMgr.RecycleInstanceAllOutLine();
            SceneObjectGestureMgr.RecycleInstanceAllMapGridOutLine();
        }
    }

    public void DragStart(Vector2 postion)
    {
        if (dragObject == null)
        {
            return;
        }
        //canPutInto = true;
        SceneObjectGestureMgr.RecycleInstanceAllOutLine();
        SceneObjectGestureMgr.RecycleInstanceAllMapGridOutLine();

        MapObject dragMapObject = dragObject;
        if (dragMapObject.IsGroundObject)
        {
            // 产生Outline 
            for (int i = 0; i < dragMapObject.StaticMapGridList.Count; i++)
            {
                DragOutline dragOutline = SceneObjectGestureMgr.DragOutlinePool.GetInstance();
                dragOutline.transform.position = MapMgr.Instance.GetWorldPosByPointCenter(dragMapObject.StaticMapGridList[i].point);
                dragOutline.transform.SetParent(dragObject.transform, true);
                if (dragMapObject.BasicData.canMerge)
                {
                    dragOutline.SetSpriteColor(DragOutline.CAN_MOVE_CAN_MERGE);
                }
                else
                {
                    dragOutline.SetSpriteColor(DragOutline.CAN_MOVE_NO_MERGE);
                }
                SceneObjectGestureMgr.mapObjectOutLine.Add(dragOutline);
            }

            offsetposition = Vector2.zero;
            hasPostion = true;
            offsetposition = postion - (Vector2)dragMapObject.transform.position;
            moveMapGrid = null;
            changeMapGrid = dragMapObject.StaticMapGridList[0];
            startResponse = true;
        }
        else
        {
            offsetposition = postion - (Vector2)dragMapObject.transform.position;
            DragOutline dragOutline = SceneObjectGestureMgr.DragOutlinePool.GetInstance();
            dragOutline.transform.SetParent(dragObject.transform, true);
            if (dragObject.shadowTransform != null)
            {
                dragOutline.transform.localPosition = dragObject.shadowTransform.localPosition;
            }
            else
            {
                dragOutline.transform.localPosition = Vector3.zero;
            }
            dragOutline.InitNoPostion(1);
            if (dragObject.BasicData.canMerge)
            {
                dragOutline.SetSpriteColor(DragOutline.CAN_MOVE_CAN_MERGE);
            }
            else
            {
                dragOutline.SetSpriteColor(DragOutline.CAN_MOVE_NO_MERGE);
            }
            SceneObjectGestureMgr.mapObjectOutLine.Add(dragOutline);
            //dragMapObject.transform.SetParent(dragObject.transform, true);
            hasPostion = false;
            dragEndPostion = postion - offsetposition;
            startResponse = true;
        }
    }

    public void DragObject(Vector2 worldPostion)
    {
        if (startResponse)
        {
            if (hasPostion)
            {
                if (dragObject != null)
                {
                    // Vector3 checkPostion = new Vector3(worldPostion.x - offsetposition.x, worldPostion.y - offsetposition.y, 0);

                    MapGrid currentMapGrid = null;

                    MapGrid willChangeMapGrid = MapMgr.Instance.GetMapGridData(worldPostion);
                    if (willChangeMapGrid!=null&&MapMgr.Instance.CheckAreaIntheBoundary(willChangeMapGrid.point, dragObject.Area))
                    {

                        currentMapGrid = willChangeMapGrid;
                    }
                    else
                    {
                        //Debug.Log("物体已经拖出界外");
                    }

                    if (currentMapGrid != null)
                    {
                        if (moveMapGrid == currentMapGrid)
                        {
                        }
                        else
                        {

                            if (moveMapGrid != null)
                            {
                                // 停止上一次操作的动画
                                if (dragObject != null)
                                {
                                    //dragObject.DragEndChangePosition(moveMapGrid);
                                }
                            }

                            //如果存在引导限制点那么禁止向这个目标移动
                            //if (TutorialMgr.Instance.LimitDrag(currentMapGrid.point))
                            //{
                            //    moveMapGrid = currentMapGrid;
                            //    if (moveMapGrid != null && moveMapGrid.Entity != null)
                            //    {
                            //        // Debug.Log(moveMapGrid.Entity);
                            //    }

                            //    if (JugeObjectMove())
                            //    {
                            //        //已偏移移动优先
                            //    }
                            //    else
                            //    {
                            //        //如果偏移移动无法移动 那么已手指当前的位置为判断的起始点

                            //        willChangeMapGrid = MapMgr.Instance.GetMapGridData(worldPostion);
                            //        if (willChangeMapGrid != null && MapMgr.Instance.CheckAreaIntheBoundary(willChangeMapGrid.point, dragObject.Area))
                            //        {

                            //            currentMapGrid = willChangeMapGrid;
                            //        }
                            //        else
                            //        {
                            //            //Debug.Log("物体已经拖出界外");
                            //        }
                            //        if (moveMapGrid == currentMapGrid)
                            //        {
                            //        }
                            //        else
                            //        {

                            //            if (moveMapGrid != null)
                            //            {
                            //                // 停止上一次操作的动画
                            //                if (dragObject != null)
                            //                {
                            //                    dragObject.DragEndChangePosition(moveMapGrid);
                            //                }
                            //            }
                                      
                            //            //位置改变
                            //            moveMapGrid = currentMapGrid;

                            //            JugeObjectMove();
                            //        }
                            //    }
                            //}

                        }
                    }
                }
            }
            else
            {
                if (dragTweener != null)
                {
                    dragTweener.Kill();
                    dragTweener = null;
                }
                if (dragObject != null)
                {
                    if (dragObject.IsMonster)
                    {
                        //DrawDragTipOutLine(worldPostion);
                    }
                    Vector3 oldPosition = dragObject.transform.position;
                    oldPosition.x = worldPostion.x - offsetposition.x;
                    oldPosition.y = worldPostion.y - offsetposition.y;
                    dragObject.transform.position = oldPosition;
                    if (Math.Abs(Vector3.Distance(dragEndPostion, dragObject.transform.position)) < CHECK_DISTANCE)
                    {
                        return;
                    }
                    dragEndPostion = dragObject.transform.position;

                    MapGrid currentMapGrid = MapMgr.Instance.GetMapGridData(worldPostion);
                    if (currentMapGrid != null)
                    {
                        //dragObject.DragEndChangePosition(currentMapGrid);
                        //dragObject.DragStartChangePosition(currentMapGrid);
                        moveMapGrid = currentMapGrid;
                    }
                    else
                    {
                        if (moveMapGrid != null)
                        {
                            //dragObject.DragEndChangePosition(moveMapGrid);
                        }
                    }
                }
            }
        }
        else
        {
            dragObject = null;
        }
    }

    public void DragEnd(Vector3 worldPostion)
    {
        startResponse = false;
        if (dragTweener != null)
        {
            dragTweener.Kill(false);
            dragTweener = null;
        }
        if (hasPostion)
        {
            if (dragObject != null)
            {
                if (moveMapGrid == null)
                {
                    //canPutInto = false;
                }
                //else
                //{
                //    if (CheckCanBePlace())
                //    {
                //        //lastEffectiveArea = moveMapGrid;
                //        //canPutInto = true;
                //    }
                //    else
                //    {
                //        Debug.Log("无效区域");
                //        //canPutInto = false;
                //    }
                //}

                //dragObject.DragEndChangePosition(moveMapGrid);
                //if (moveMapGrid != null)
                //{
                //    dragObject.DragStartChangePosition(moveMapGrid);
                //    dragObject.DragEnd(moveMapGrid, lastEffectiveArea, canPutInto);
                //}
            }
        }
        else
        {
            if (dragObject != null)
            {
                MapGrid currentMapGrid = MapMgr.Instance.GetMapGridData(worldPostion);
                if (currentMapGrid != null)
                {
                    //    dragObject.DragEndChangePosition(currentMapGrid);
                    //    dragObject.DragStartChangePosition(currentMapGrid);
                    moveMapGrid = currentMapGrid;
                }
                else
                {
                    if (moveMapGrid != null)
                    {
                        //dragObject.DragEndChangePosition(moveMapGrid);
                    }
                }
                //dragObject.DragEnd(worldPostion);
            }
        }


        SceneObjectGestureMgr.RecycleInstanceAllOutLine();
        SceneObjectGestureMgr.RecycleInstanceAllMapGridOutLine();
        SceneObjectGestureMgr.RecycleAllDragMapObjectTip();
    }

    public void BecomesDeathTrapCauseDragEnd(MapGrid grid)
    {
        //if (GlobalVariable.GameState == GameState.LevelModel&& hasPostion)
        //{
        //    bool causeDragEnd = true;
        //    if (dragObject != null && dragObject.CurrentStatus.Name == StatusName.Drag)
        //    {
        //        List<MapGrid> allGrid = new List<MapGrid>();
        //        bool result = MapMgr.Instance.IsLegalMovement(moveMapGrid.point, dragObject.Area, ref allGrid);
        //        if (allGrid.Contains(grid))
        //        {
        //            //检查当前是否可以放入
        //            if (result)
        //            {
        //                //判断当前区域是否存在除自己以外其他物品
        //                for (int i = 0; i < allGrid.Count; i++)
        //                {
        //                    if (allGrid[i].Entity != null && allGrid[i].Entity != dragObject)
        //                    {
        //                        causeDragEnd = false;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                causeDragEnd = false;
        //            }

        //            if (causeDragEnd)
        //            {
        //                MapMgr.Instance.TakeOutMapObjectEntity(dragObject);
        //                MapMgr.Instance.SetMapObjectEntity(moveMapGrid.point, dragObject);
        //                dragObject.IsDraged = false;
        //                dragObject.SwitchStatus(StatusName.Idle);
        //                dragObject = null;
        //                SceneObjectGestureMgr.RecycleInstanceAllOutLine();
        //                SceneObjectGestureMgr.RecycleInstanceAllMapGridOutLine();
        //                SceneObjectGestureMgr.RecycleAllDragMapObjectTip();
        //            }
        //        }
        //    }

        //    if (dragObject != null && causeDragEnd == false)
        //    {
        //        if (CheckCanBePlace())
        //        {
        //            lastEffectiveArea = moveMapGrid;
        //            canPutInto = true;
        //        }
        //        else
        //        {
        //            Debug.Log("无效区域");
        //            canPutInto = false;
        //        }
        //        dragObject.DragEndChangePosition(moveMapGrid);
        //        dragObject.DragStartChangePosition(moveMapGrid);
        //        dragObject.transform.position = MapMgr.Instance.GetWorldPosByPoint(moveMapGrid.point);
        //    }
        //}

    }

    bool CheckCanMoveTo()
    {
        List<MapGrid> allGrid = new List<MapGrid>();

        bool result = MapMgr.Instance.IsLegalMovement(moveMapGrid.point, dragObject.Area, ref allGrid);

        if (result == false)
        {
            result = MapMgr.Instance.IsLegalMovementOptimization(moveMapGrid.point, changeMapGrid.point, dragObject.Area, ref allGrid);
            if (result == true)
            {
                moveMapGrid = allGrid[0];
            }
        }

        if (result)
        {

            if (moveMapGrid == allGrid[0])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 龙在拖动到物体时，给予物体的提示框（可采集，可建造，可攻击的物品）
    /// </summary>
    /// <param name="worldPostion"></param>
    void DrawDragTipOutLine(Vector3 worldPostion)
    {

        for (int i = 0; i < SceneObjectGestureMgr.mapGridOutLine.Count; i++)
        {
            SceneObjectGestureMgr.DragOutlinePool.RecycleInstance(SceneObjectGestureMgr.mapGridOutLine[i]);
        }
        SceneObjectGestureMgr.mapGridOutLine.Clear();

        //for (int i = 0; i < SceneObjectGestureMgr.DragMapObjectTip.Count; i++)
        //{
        //    MapMgr.clickToCollectTipPool.RecycleInstance(SceneObjectGestureMgr.DragMapObjectTip[i]);
        //}
        //SceneObjectGestureMgr.DragMapObjectTip.Clear();

        MapGrid gridData = MapMgr.Instance.GetMapGridData(worldPostion);
        if (gridData == null)
        {
            return;
        }
        //ClickToCollectTip dragTip;
        //string str = "";
        //if (gridData.Entity != null
        //    && gridData.Entity.IsPurified &&
        //    ((gridData.Entity.BasicData.canHarvest && gridData.Entity.FoundationCanHarvest == false)
        //    || gridData.Entity.ObjectType == MapObject.Foundation_ObjectType
        //    || gridData.Entity.BasicData.destructType == 2))
        //{
            //dragTip = MapMgr.clickToCollectTipPool.GetInstance();
            //SceneObjectGestureMgr.DragMapObjectTip.Add(dragTip);
            //str = "";
            //if (gridData.Entity.BasicData.canHarvest)
            //{
            //    str = L10NMgr.GetText(23300012);
            //}
            //else if (gridData.Entity.ObjectType == MapObject.Foundation_ObjectType)
            //{
            //    str = L10NMgr.GetText(23300013);
            //}
            //else if (gridData.Entity.BasicData.destructType == 2)
            //{
            //    str = L10NMgr.GetText(23300010);
            //}
        //}
        //else
        //{
        //    return;
        //}

        //if (gridData.Entity != null)
        //{
        //    List<MapGrid> allGrid = new List<MapGrid>();
        //    MapMgr.Instance.IsLegalMovement(gridData.Entity.StaticPos, gridData.Entity.Area, ref allGrid);

        //    for (int i = 0; i < allGrid.Count; i++)
        //    {
        //        DragOutline dragOutline = SceneObjectGestureMgr.DragOutlinePool.GetInstance();
        //        dragOutline.transform.position = MapMgr.Instance.GetWorldPosByPointCenter(allGrid[i].point);
        //        dragOutline.Init(allGrid[i].Status);
        //        dragOutline.SetSpriteColor(DragOutline.TIP);
        //        SceneObjectGestureMgr.mapGridOutLine.Add(dragOutline);
        //    }
        //    //dragTip.ShowTip(gridData.Entity.tipMountPosition.position, str, SenceTipEnum.TextAndIcon, gridData.Entity.tipMountPosition);
        //    //dragTip.transform.position = gridData.Entity.tipMountPosition.position;
        //}
    }

}

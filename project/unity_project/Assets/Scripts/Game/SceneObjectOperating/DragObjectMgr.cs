﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
/// <summary>
/// 物体拖动控制
/// </summary>
public class DragObjectMgr : MonoBehaviour
{
    public static DragObjectMgr Instance;
    public const float CHECK_DISTANCE = 0.35F;
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
    /// 当前拖动到的MapGrid
    /// </summary>
    private MapGrid moveMapGrid;
 
    /// <summary>
    /// 拖动的Tweener动画
    /// </summary>
    private Tweener dragTweener;
  
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
    }

  

    public void DragStart(Vector2 postion)
    {
        if (dragObject == null)
        {
            return;
        }
        //canPutInto = true;

        MapObject dragMapObject = dragObject;
        if (dragMapObject.IsGroundObject)
        {
    
            offsetposition = Vector2.zero;
            hasPostion = true;
            offsetposition = postion - (Vector2)dragMapObject.transform.position;
            moveMapGrid = null;
            startResponse = true;
        }
        else
        {
            offsetposition = postion - (Vector2)dragMapObject.transform.position;
           
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

    }
}

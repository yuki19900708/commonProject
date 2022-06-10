using System.Collections;
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
    /// 判断他的拖动方式
    /// </summary>
    public bool hasPostion = false;
    /// <summary>
    /// 拖动的偏移位置
    /// </summary>
    public Vector2 offsetposition;

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

        MapObject dragMapObject = dragObject;
        if (dragMapObject.IsGroundObject)
        {
            offsetposition = Vector2.zero;
            hasPostion = true;
            offsetposition = postion - (Vector2)dragMapObject.transform.position;
            startResponse = true;
        }
        else
        {
            offsetposition = postion - (Vector2)dragMapObject.transform.position;
           
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
            }
            else
            {
                if (dragObject != null)
                {
                    Vector3 oldPosition = dragObject.transform.position;
                    oldPosition.x = worldPostion.x - offsetposition.x;
                    oldPosition.y = worldPostion.y - offsetposition.y;
                    dragObject.transform.position = oldPosition;
                    if (Math.Abs(Vector3.Distance(dragEndPostion, dragObject.transform.position)) < CHECK_DISTANCE)
                    {
                        return;
                    }
                    dragEndPostion = dragObject.transform.position;
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
    }
}

using UnityEngine;
using System;
/// <summary>
/// 物体双击选择
/// </summary>
public class DoubleClickMgr : MonoBehaviour
{
    public static DoubleClickMgr Instance;

    public Action<Vector2, MapObject> Event_DoubleClick;
    public MapObject currentGameObject = null;
    private float intervalTime = 0.3f;
    private float timer = 0;
    private int clicktTimes = 0;
    private bool startMonitor = false;

    public void Awake()
    {
        Instance = this;
    }

    public void DoubleClick(Vector2 postion, GameObject tagert)
    {
        MapObject clickTarget = tagert.GetComponent<MapObject>();
        if (clickTarget != null)
        {
            //第一次点击
            if (clicktTimes == 0)
            {
                startMonitor = true;
                clicktTimes++;
            }
            //第二次点击
            else if(clicktTimes == 1)
            {
                //点中的是相同物体
                if (currentGameObject == clickTarget)
                {
                    //在合适的间隔范围内
                    if (timer <= intervalTime)
                    {
                        //clickTarget.DoubleClick();
                        clicktTimes = 0;
                        startMonitor = false;
                        timer = 0;
                    }
                    else
                    {
                        timer = 0;
                        startMonitor = true;
                    }
                }
                //如果第二次点击不同的物体，开启第二个物体的双击检测
                else
                {
                    timer = 0;
                    startMonitor = true;
                }
            }
            currentGameObject = clickTarget;
        }
        else
        {
            startMonitor = false;
            timer = 0;
        }
    }

    void Update()
    {
        if (startMonitor)
        {
            timer += Time.deltaTime;
        }
    }
}

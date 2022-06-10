using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerMgr : MonoBehaviour
{
    private static int onlyID = 0;
    private static GenericPool<TimerData> timerPool = new GenericPool<TimerData>();
    private static Dictionary<int, TimerData> usingTimerDic = new Dictionary<int, TimerData>();

    //删除列表，且触发回调
	private static HashSet<int> deleteListWithCallback = new HashSet<int>();
	//删除列表，但不触发回调
    private static HashSet<int> deleteListNoCallback = new HashSet<int>();

    internal static long mLocalTimestampLong;
    internal static long mServerTimestampLong;

    private static Action getTimestampCallback;

    private void Awake()
    {
        //NetAPI.Event_GetServerTime += RequestServerStampCallback;
    }

    void Update()
    {
        mServerTimestampLong += (int)(Time.unscaledDeltaTime * 1000);
        mLocalTimestampLong += (int)(Time.unscaledDeltaTime * 1000);
        Timer.Update();
		if (Input.GetKey(KeyCode.LeftControl))
        {
			if (Input.GetKeyDown(KeyCode.Q))
			{
				//Debug.Log("RYH", "TimerMgr", "服务器时间：" + GetDateTime(GetSeconds()));
			}
        }

        UpdateAllTimer();
    }

    #region 外部操作计时器方法
	/// <summary>
	/// 清除所有计时器
	/// </summary>
    public static void ClearAllTimer()
	{
		//Debug.Log("RYH", "TimerMgr", "清除所有计时器（全部加入删除列表且不触发结束事件）");
        foreach (var item in usingTimerDic.Keys)
        {
            JoinDeleteList(item, false);
        }
		ProcessDeleteListNoCallback();
    }

    /// <summary>
    /// 开启一个计时器，（传入时间段单位为秒）
    /// </summary>
    /// <param name="totalTime"></param>
    /// <param name="callBack"></param>
    /// <param name="param"></param>
    /// <param name="loop"></param>
    /// <returns></returns>
    public static int OpenOneTimerWithSecond(float totalTime, TimerCallBack callBack = null, object param = null, bool loop = false)
    {
        if (totalTime <= 0)
        {
            Debug.LogError("计时器的计时时间存在问题：" + totalTime);
            return 0;
        }
        onlyID++;
        TimerData data = timerPool.GetInstance();
        data.InitializeWithSecond(onlyID, totalTime, callBack, loop, param);
        if (!usingTimerDic.ContainsKey(onlyID))
        {
            usingTimerDic.Add(onlyID,data);
        }
        else
        {
            Debug.LogError("获取到了一个正在使用的计时器" + onlyID);
        }
		//Debug.Log("RYH", "TimerMgr", "开启一个计时器：" + onlyID);
        return onlyID;
    }

    /// <summary>
    /// 开启一个计时器（传入时间段单位为毫秒）
    /// </summary>
    /// <returns></returns>
    public static int OpenOneTimerWithMillisecond(long totalTime, TimerCallBack callBack = null, object param = null, bool loop = false)
    {
        if (totalTime <= 0)
        {
            Debug.LogError("计时器的计时时间存在问题：" + totalTime);
            return 0;
        }
        onlyID++;
        TimerData data = timerPool.GetInstance();
        data.InitializeWithMillisecond(onlyID, totalTime, callBack, loop, param);
        if (!usingTimerDic.ContainsKey(onlyID))
        {
            usingTimerDic.Add(onlyID, data);
        }
        else
        {
            Debug.LogError("获取到了一个正在使用的计时器" + onlyID);
        }
        //Debug.Log("RYH", "TimerMgr", "开启一个计时器：" + onlyID);
        return onlyID;
    }

    public static bool IsTimerOpen(int onlyID)
    {
        if (usingTimerDic.ContainsKey(onlyID))
        {
            if (usingTimerDic[onlyID].Running)
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

    public static void CloseOneTimer(int onlyID)
    {
        if (usingTimerDic.ContainsKey(onlyID))
        {
            if (usingTimerDic[onlyID].Running)
            {
                //Debug.Log("RYH", "TimerMgr", "外部主动关闭计时器（不触发结束事件）：" + onlyID);
                JoinDeleteList(onlyID, false);
            }
            else
            {
                //Debug.Log("RYH", "TimerMgr", "外部主动关闭计时器,但计时器已经关闭" + onlyID);
            }
        }
        else
        {
            //Debug.Log("RYH", "TimerMgr", "外部主动关闭计时器,但正在计时的列表中没有此计时器:" + onlyID);
        }
    }

    public static long GetRemainTimeToLong(int id)
    {
        if (usingTimerDic.ContainsKey(id))
        {
            return usingTimerDic[id].GetRemainTimeToLong();
        }
        else
        {
            Debug.LogError("计时器已经结束，还在获取剩余时间");
            return 0;
        }
    }

    public static float GetRemainTimeToFloat(int id)
    {
        if (usingTimerDic.ContainsKey(id))
        {
            return usingTimerDic[id].GetRemainTimeToFloat();
        }
        else
        {
            Debug.LogError("计时器已经结束，还在获取剩余时间" + id);
            return 0;
        }
    }

    public static int GetRemainTimeToInt(int id)
    {
        if (usingTimerDic.ContainsKey(id))
        {
            return (int)usingTimerDic[id].GetRemainTimeToFloat();
        }
        else
        {
            Debug.LogError("计时器已经结束，还在获取剩余时间");
            return 0;
        }
    }

    public static void Reset(int onlyID)
    {
        if (usingTimerDic.ContainsKey(onlyID))
        {
            if (usingTimerDic[onlyID].Running)
            {
                usingTimerDic[onlyID].Reset();
            }
        }
    }

    public static void TimerPause(int onlyID) 
    {
        if (usingTimerDic.ContainsKey(onlyID))
        {
            if (usingTimerDic[onlyID].Running)
            {
                usingTimerDic[onlyID].TimerPuase();
            }
        }
    }

    public static void TimerContinue(int onlyID)
    {
        if (usingTimerDic.ContainsKey(onlyID))
        {
            if (usingTimerDic[onlyID].Running)
            {
                usingTimerDic[onlyID].TimerContinue();
            }
        }
    }

    /// <summary>
    /// 追加时间，延长计时器结束时间（如果计时器已经关闭，则无效）
    /// </summary>
    public static void AppendTime(int onlyID, float delayTime)
    {
        if (usingTimerDic.ContainsKey(onlyID))
        {
            if (usingTimerDic[onlyID].Running)
            {
                usingTimerDic[onlyID].IncreaseTime(delayTime);
            }
        }
    }
    #endregion

    private static void UpdateAllTimer()
	{
        foreach (var item in usingTimerDic.Values)
        {
			//item的Update方法中会将结束计时的Timer加入删除列表
            item.Update();
		}

		if (deleteListNoCallback.Count > 0)
		{
			ProcessDeleteListNoCallback();
		}

		if (deleteListWithCallback.Count > 0)
		{
			ProcessDeleteListWithCallback();
		}
    }

    /// <summary>
    /// 处理带结束事件的删除列表
    /// </summary>
    private static void ProcessDeleteListWithCallback()
	{
		//遍历删除列表，触发结束回调，并移除Timer
		foreach (var onlyID in deleteListWithCallback)
		{
			TimerData timerData = usingTimerDic[onlyID];
			//触发结束回调
			timerData.TriggerCallback();
			//回收Timer
			RecycleTimer(onlyID);
		}

		//清空删除列表
		deleteListWithCallback.Clear();
	}

	/// <summary>
	/// 处理没有结束事件的删除列表
	/// </summary>
	private static void ProcessDeleteListNoCallback()
	{
		//遍历删除列表，触发结束回调，并移除Timer
		foreach (var onlyID in deleteListNoCallback)
		{
			TimerData timerData = usingTimerDic[onlyID];
			//回收Timer
			RecycleTimer(onlyID);
		}

		//清空删除列表
		deleteListNoCallback.Clear();
	}

    /// <summary>
    /// 将计时器添加入删除列表
    /// </summary>
    /// <param name="onlyID">计时器的ID</param>
    /// <param name="triggerCallback">是否触发结束事件</param>
    public static void JoinDeleteList(int onlyID, bool triggerCallback)
    {
        if (usingTimerDic.ContainsKey(onlyID))
        {
            usingTimerDic[onlyID].CloseTimer();
            if (triggerCallback)
            {
                if (deleteListWithCallback.Contains(onlyID) == false)
                {
                    //Debug.Log("RYH", "TimerMgr", "将计时器加入删除列表（将触发结束事件）：" + onlyID);
                    deleteListWithCallback.Add(onlyID);
                }
                else
                {
                    //Debug.Log("RYH", "TimerMgrError", "删除列表（将触发结束事件）中已经包含了此计时器，重复添加：" + onlyID);
                }
            }
            else
            {
                if (deleteListNoCallback.Contains(onlyID) == false)
                {
                    //Debug.Log("RYH", "TimerMgr", "将计时器加入删除列表（不触发结束事件）：" + onlyID);
                    deleteListNoCallback.Add(onlyID);
                }
                else
                {
                    //Debug.Log("RYH", "TimerMgrError", "删除列表（不触发结束事件）中已经包含了此计时器，重复添加：" + onlyID);
                }
            }
        }
        else
        {
            //Debug.Log("RYH", "TimerMgrError", "正在使用的计时器列表中没有这个计时器，却有人想回收掉这个计时器：" + onlyID);
        }
    }

    private static void RecycleTimer(int onlyID)
    {
        if (usingTimerDic.ContainsKey(onlyID))
		{
			//Debug.Log("RYH", "TimerMgr", "计时器归池：" + onlyID);
            timerPool.RecycleInstance(usingTimerDic[onlyID]);
            usingTimerDic.Remove(onlyID);
        }
        else
        {
			//Debug.Log("RYH", "TimerMgrError", "要回收的计时器不在使用列表中:" + onlyID);
        }
    }

    public static void RequestTimeStamp(Action callback)
    {
        getTimestampCallback = callback;
        //NetMgr.Send(NetAPIDef.eCTS_ACCESS_GET_TIME, null);
    }

    /// <summary>
    /// 获取当前时间（秒）
    /// </summary>
    /// <returns></returns>
    public static int GetSeconds()
    {
        return (int)(mServerTimestampLong / 1000);
    }

    /// <summary>
    /// 获取当前时间（毫秒）
    /// </summary>
    /// <returns></returns>
    public static long GetMillisecond()
    {
        return mServerTimestampLong;
    }

 

    public static DateTime GetDateTime()
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = (mServerTimestampLong * 10000);
        TimeSpan toNow = new TimeSpan(lTime);
        DateTime targetDt = dtStart.Add(toNow);
        return targetDt;
    }
}

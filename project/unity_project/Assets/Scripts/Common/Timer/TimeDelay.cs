using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void DelayCallback(object callbackObj);
public delegate void DelayCallbackFactory(object callbackObj, int exceedTime);
public delegate void DelayCallbackPhase(object callbackObj, int phaseCount);

public static class TimeDelay
{
    /// <summary>
    /// 所有计时器
    /// </summary>
    private static List<TimeDelayData> timeDelayDatas = new List<TimeDelayData>();

    public static void AttachTimeDelay(TimeDelayData data)
    {
        if (!timeDelayDatas.Contains(data))
        {
            timeDelayDatas.Add(data);
        }
    }

    public static void DetachTimeDelay(TimeDelayData data)
    {
        if (timeDelayDatas.Contains(data))
        {
            timeDelayDatas.Remove(data);
        }
    }

    public static void Update()
    {
        for (int i = 0; i < timeDelayDatas.Count;i++ )
        {
            timeDelayDatas[i].Update();
        }
    }

	public static TimeDelayData Delay(float timeToDelay,
                                    DelayCallback delayCallback,
                                    object callbackObj = null,
                                    bool loop = false,
                                    DelayCallbackPhase phaseCallback = null,
                                    int phaseCount = 0,
                                    float totalTimeLength = 0)
	{
        TimeDelayData timeData = null;
		if (delayCallback != null && timeToDelay > 0)
        {
            if (totalTimeLength < timeToDelay)
            {
                totalTimeLength = timeToDelay;
            }
            timeData = new TimeDelayData(timeToDelay, delayCallback, callbackObj, loop, phaseCallback, phaseCount, totalTimeLength);
			AttachTimeDelay(timeData);
		}
		return timeData;
	}

	public static TimeDelayData DelayFactory(float timeToDelay,
                                    DelayCallbackFactory delayCallback,
                                    object callbackObj = null,
                                    bool loop = false)
    {
        TimeDelayData timeData = null;
		if (delayCallback != null && timeToDelay > 0)
        {
            timeData = new TimeDelayData(timeToDelay, delayCallback, callbackObj, loop);
            AttachTimeDelay(timeData);
        }
        return timeData;
    }

    public static void ClearAllTimeDelay()
    {
        timeDelayDatas.Clear();
    }
}

public class TimeDelayData
{
    /// <summary>
    /// 是否循环
    /// </summary>
    private bool mLoop = false;
    /// <summary>
    /// 多久的时间执行(毫秒）
    /// </summary>
    private long mTimeToDelay = 0;
    /// <summary>
    /// 是否开始计时了
    /// </summary>
    private bool isRunning = false;
    /// <summary>
    /// 回调的一个对象
    /// </summary>
    private object mCallbackObj = null;
    /// <summary>
    /// 计时开始时间戳, 单位毫秒
    /// </summary>
    private long mStartTimeStamp;
    /// <summary>
    /// 计时结束时间戳, 单位毫秒
    /// </summary>
    private long mEndTimeStamp;
    /// <summary>
    /// 计时回调
    /// </summary>
    private DelayCallback mCallback = null;

    private DelayCallbackFactory mCallbackFactory = null;

    #region 计时阶段回调
    /// <summary>
    /// 计时阶段回调
    /// </summary>
    private DelayCallbackPhase mPhaseCallback = null;
    /// 当前计时阶段
    /// </summary>
    private float mPhaseCurrentCount = 0;
    /// <summary>
    /// 单个阶段的时间, 单位毫秒
    /// </summary>
    private float mPhaseInterval = 1;

    public bool IsRunning
    {
        get
        {
            return isRunning;
        }

        set
        {
            isRunning = value;
        }
    }
    #endregion

    public TimeDelayData(float timeToDelay,
                        DelayCallback callback,
                        object callbackObj,
                        bool loop,
                        DelayCallbackPhase delayCallbackPhase = null,
                        int phaseCount = 0,
                        float totalTimeLength = 0)
    {
        mTimeToDelay = (long)(timeToDelay * 1000);
        long totalTime = (long)(totalTimeLength * 1000);
        long pastTime = totalTime - mTimeToDelay;
        mStartTimeStamp = TimerMgr.mServerTimestampLong - pastTime;
        mEndTimeStamp = mStartTimeStamp + totalTime;
        mCallback = callback;
        mLoop = loop;
        mCallbackObj = callbackObj;
        mPhaseCallback = delayCallbackPhase;
        mPhaseCurrentCount = -1;
        if (phaseCount > 2)
        {
            mPhaseInterval = totalTime / (phaseCount - 1);
        }
        else
        {
            mPhaseInterval = totalTime;
        }
        IsRunning = true;
    }

    public TimeDelayData(float timeToDelay,
                        DelayCallbackFactory callback,
                        object callbackObj,
                        bool loop)
    {
        mTimeToDelay = (int)(timeToDelay * 1000);
        mStartTimeStamp = TimerMgr.mServerTimestampLong;
        mEndTimeStamp = mStartTimeStamp + mTimeToDelay;
        mCallbackFactory = callback;
        mLoop = loop;
        mCallbackObj = callbackObj;
        IsRunning = true;
    }
    
    public void Update()
    {
        if (IsRunning)
        {
            if (mPhaseCallback != null)
            {
                int instantPhaseCount = (int)((TimerMgr.mServerTimestampLong - mStartTimeStamp) / mPhaseInterval);
                if (mPhaseCurrentCount != instantPhaseCount)
                {
                    mPhaseCurrentCount = instantPhaseCount;
                    mPhaseCallback(mCallbackObj, (int)mPhaseCurrentCount);
                }   
            }   
            IsRunning = TimerMgr.mServerTimestampLong < mEndTimeStamp;
            if (IsRunning == false)
            {
                if (mCallback != null)
                {
                    mCallback(mCallbackObj);
                }
                if (mCallbackFactory != null)
                {
                    //计时器超出的时间，用于工厂补偿到下一个箱子的生产时间上，单位是秒
                    float exceedTime = (TimerMgr.mServerTimestampLong - mEndTimeStamp) / 1000f;
                    if (exceedTime < 0)
                    {
                        exceedTime = 0;
                    }
                    mCallbackFactory(mCallbackObj, (int)exceedTime);
                }
                
                if (mLoop)
                {
                    Reset();
                }
                else
                {
                    TimeDelay.DetachTimeDelay(this);
                }
            }
        }
    }

    public void Reset()
    {
        mStartTimeStamp = TimerMgr.mServerTimestampLong;
        mEndTimeStamp = mStartTimeStamp + mTimeToDelay;
    }

    //更改回调对象
    public void UpdateObject(object newObj)
    {
        mCallbackObj = newObj;
    }
    /// <summary>
    /// 获取剩余时间, 单位秒
    /// </summary>
    /// <returns></returns>
    public float GetRemainingTime()
    {
        float remainTime = (mEndTimeStamp - TimerMgr.mServerTimestampLong) / 1000f;
        return remainTime;
    }

    public float GetPassTime()
    {
        float passTime = (TimerMgr.mServerTimestampLong - mStartTimeStamp) / 1000f;
        return passTime;
    }

    /// <summary>
    /// 减掉剩余时间
    /// </summary>
    /// <param name="DelTime">Del time.</param>
    public void DelDelayTime(float DelTime)
    {
        mEndTimeStamp -= (int)(DelTime * 1000);
    }

    public void AddDelayTime(float time)
    {
        mEndTimeStamp += (int)(time * 1000);
    }
}

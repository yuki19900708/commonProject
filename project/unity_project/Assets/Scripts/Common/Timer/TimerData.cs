using System;

/// <summary>
/// 计时结束回调（在此时计时器虽然未回收，但没有继续运行，同时计时器自动结束会自动回收，外部无须回收）
/// </summary>
public delegate void TimerCallBack(object param);


public class TimerData
{
    public Action<int> Event_TimeOver;

    private int onlyID;
    private bool loop;
    private bool pause;
    private long startTimestamp;
    private long endTimestamp;
    private long totalTime;
    private object data;
    private TimerCallBack callBack;
    private bool running;

    public int OnlyID { get { return onlyID; } }
    public bool Loop { get { return loop; } }
    public bool Pause { get { return pause; } }
    public long StartTimestamp { get { return startTimestamp; } }
    public long EndTimestamp { get { return endTimestamp; } }
    public bool Running { get { return running; } }

    #region Public Method

    public TimerData()
    {

    }

    /// <summary>
    /// 初始化计时器，秒级别
    /// </summary>
    /// <param name="onlyID"></param>
    /// <param name="totalTime"></param>
    /// <param name="callBack"></param>
    /// <param name="loop"></param>
    /// <param name="data"></param>
    public void InitializeWithSecond(int onlyID,float totalTime, TimerCallBack callBack, bool loop, object data)
    {
        this.onlyID = onlyID;
        this.totalTime = (long)(totalTime * 1000);
        this.callBack = callBack;
        this.loop = loop;
        this.pause = false;
        this.data = data;
        startTimestamp = TimerMgr.mServerTimestampLong;
        endTimestamp = TimerMgr.mServerTimestampLong + this.totalTime;
        RefreshRuning();
    }

    /// <summary>
    /// 初始化计时器毫秒级别
    /// </summary>
    /// <param name="onlyID"></param>
    /// <param name="totalTime"></param>
    /// <param name="callBack"></param>
    /// <param name="loop"></param>
    /// <param name="data"></param>
    public void InitializeWithMillisecond(int onlyID, long totalTime, TimerCallBack callBack, bool loop, object data)
    {
        this.onlyID = onlyID;
        this.totalTime = totalTime;
        this.callBack = callBack;
        this.loop = loop;
        this.pause = false;
        this.data = data;
        startTimestamp = TimerMgr.mServerTimestampLong;
        endTimestamp = TimerMgr.mServerTimestampLong + this.totalTime;
        RefreshRuning();
    }

    public void Update()
    {
        if (running)
        {
            Timing();
        }
    }

    public long GetRemainTimeToLong()
    {
        return endTimestamp - TimerMgr.mServerTimestampLong;
    }

    public float GetRemainTimeToFloat()
    {
        return (endTimestamp - TimerMgr.mServerTimestampLong) / 1000f;
    }

    public long GetPastTime()
    {
        return TimerMgr.mServerTimestampLong - startTimestamp;
    }

    public void TimerPuase() 
    {
        pause = true;
    }

    public void TimerContinue()
    {
        pause = false;
    }

    public void IncreaseTime(float time)
    {
        if (running)
        {
            endTimestamp += (long)(time * 1000);
        }
        else
        {
            //Debug.LogError("已经计时结束，不能添加计时");
        }
    }

    public void DecreaseTime(float time)
    {
        if (running)
        {
            endTimestamp -= (long)(time * 1000);
            Timing();
        }
        else
        {
            //Debug.LogError("已经计时结束，不能减少计时");
        }
    }

    public void Reset()
    {
        startTimestamp = TimerMgr.mServerTimestampLong;
        endTimestamp = TimerMgr.mServerTimestampLong + totalTime;
        Timing();
    }

    public void CloseTimer()
    {
        running = false;
    }

	public void TriggerCallback()
	{
		if (callBack != null)
		{
			callBack(data);
		}
	}

    #endregion

    #region Private Method

    private void Timing()
    {
        RefreshRuning();
        if (running == false)
        {
            if (loop)
            {
                Reset();
            }
            else
            {
				//将计时结束的Timer加入删除列表，在删除的时候触发结束事件，此处不触发
				//若在此处触发结束事件，使用使用者可能在事件响应方法中添加一个新的Timer，导致TimerMgr的Update中out sync错误
				//Debug.Log("RYH", "TimerMgr", "计时结束，加入删除列表（将触发结束事件）：" + onlyID);
				TimerMgr.JoinDeleteList(onlyID, true);
            }
        }
    }

    private void RefreshRuning()
    {
        if (pause == false) 
        {
            running = TimerMgr.mServerTimestampLong < endTimestamp;
        }
    }

    #endregion
}

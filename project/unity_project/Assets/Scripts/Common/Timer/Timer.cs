using System.Collections.Generic;

/// <summary>
/// 时间管理类的回调  ---有开始时间 --- 回调函数
/// </summary>
public struct DelayAction
{
    public long endTimeStamp;
    public System.Action action;
    public System.Action<MapGrid> param_action;
    public MapGrid grid;
}

/// <summary>
/// 时间管理类
/// </summary>
public static class Timer
{
    static bool isUnTime  = true;
    /// <summary>
    /// 定义一个时间管理器的集合----用LinkedList属于数据结构中的 顺序存储或链式存储
    /// </summary>
    static LinkedList<DelayAction> m_delayActoinList = new LinkedList<DelayAction>();
    /// <summary>
    /// 用于存储一帧内要删除的所有计算对象
    /// </summary>
    static List<DelayAction> willRemoveActionList = new List<DelayAction>(10);
    /// <summary>
    /// 用于存储一帧内要增加的所有计算对象
    /// </summary>
    static List<DelayAction> willAddActionList = new List<DelayAction>(10);
    /// <summary>
    /// 增加延迟回调
    /// </summary>
    /// <param name="time">延迟事件</param>
    /// <param name="action">回调参数</param>
    public static DelayAction AddDelayFunc(float time, System.Action action,bool _isUnTime = true)
    {
        //重写回调类
        DelayAction act = new DelayAction();
        isUnTime = _isUnTime;
        if (isUnTime)
        {       
            //回调类的时间为 到达事件+回调延迟时间
            act.endTimeStamp = TimerMgr.mServerTimestampLong + (int)(time * 1000);
        }
        else
        {
            //回调类的时间为 到达事件+回调延迟时间
            act.endTimeStamp = TimerMgr.mLocalTimestampLong + (int)(time * 1000);
        }

        //回调为传入的回调
        act.action = action;
        //把回调类加入到集合
        willAddActionList.Add(act);
        return act;
    }

    /// <summary>
    /// 增加延迟回调
    /// </summary>
    /// <param name="time">延迟事件</param>
    /// <param name="action">回调参数</param>
    public static DelayAction AddDelayFunc(float time, System.Action<MapGrid> action, MapGrid grid, bool _isUnTime = true)
    {
        //重写回调类
        DelayAction act = new DelayAction();
        isUnTime = _isUnTime;
        if (isUnTime)
        {
            //回调类的时间为 到达事件+回调延迟时间
            act.endTimeStamp = TimerMgr.mServerTimestampLong + (int)(time * 1000);
        }
        else
        {
            //回调类的时间为 到达事件+回调延迟时间
            act.endTimeStamp = TimerMgr.mLocalTimestampLong + (int)(time * 1000);
        }

        //回调为传入的回调
        act.param_action = action;
        act.grid = grid;
        //把回调类加入到集合
        willAddActionList.Add(act);
        return act;
    }

    public static void Remove(DelayAction ac)
    {
        if (m_delayActoinList.Contains(ac))
        {
            m_delayActoinList.Remove(ac);
        }
        if (willAddActionList.Contains(ac))
        {
            willAddActionList.Remove(ac);
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    public static void Update()
    {
        //新的计时器的添加要延迟一帧进行
        foreach (DelayAction delayAction in willAddActionList)
        {
            m_delayActoinList.AddLast(delayAction);
        }
        willAddActionList.Clear();

        if (m_delayActoinList.Count > 0)
        {
            var dic = m_delayActoinList.GetEnumerator();

            while (dic.MoveNext())
            {
                if (isUnTime)
                {
                    if (TimerMgr.mServerTimestampLong > dic.Current.endTimeStamp)
                    {
                        if (dic.Current.action != null)
                        {
                            dic.Current.action();
                        }
                        if (dic.Current.param_action != null)
                        {
                            dic.Current.param_action(dic.Current.grid);
                        }
                        willRemoveActionList.Add(dic.Current);
                    }
                }
                else
                {
                    if (TimerMgr.mLocalTimestampLong > dic.Current.endTimeStamp)
                    {
                        if (dic.Current.action != null)
                        {
                            dic.Current.action();
                        }
                        if (dic.Current.param_action != null)
                        {
                            dic.Current.param_action(dic.Current.grid);
                        }
                        willRemoveActionList.Add(dic.Current);
                    }
                }
            
            }

            //必须要在一帧内完成所有计时器的判定
            foreach(DelayAction delayAction in willRemoveActionList)
            {
                m_delayActoinList.Remove(delayAction);
            }
            willRemoveActionList.Clear();
        }
    }
}

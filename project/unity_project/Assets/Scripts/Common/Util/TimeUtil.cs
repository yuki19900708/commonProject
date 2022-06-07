using System;
using UnityEngine;

public class TimeUtil
{
    public static bool IsNewDay(DateTime oriTime)
    {
        TimeSpan ts = TimerMgr.GetDateTime().Subtract(oriTime);

        if (ts.TotalSeconds > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static TimeSpan GetTodayRemainingTime(DateTime endTime)
    {
        TimeSpan value = new TimeSpan(0, 0, (int)(endTime - TimerMgr.GetDateTime()).TotalSeconds);
        return value;
    }

    public static DateTime GetTodayEndTime()
    {
        DateTime endTime = DateTime.Parse(TimerMgr.GetDateTime().ToString("MM/dd/yyyy 23:59:59"));
        return endTime;
    }

    /// <summary>
    /// 将秒数转为00:00:00的时间字符串
    /// </summary>
    /// <param name="seconds">秒数</param>
    /// <param name="trimZeroHour">当小时为0时是否省略</param>
    /// <returns></returns>
    public static string ConvertSecondsToDateTimeString(int seconds, bool trimZeroHour = false)
    {
        int hour = seconds / 3600;
        int minute = seconds / 60 % 60;
        int second = seconds % 60;
        if (hour == 0 && trimZeroHour)
        {
            return string.Format("{0:D2}:{1:D2}", minute, second);
        }
        return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
    }

    /// <summary>
    /// 将秒数转为00h 00m 00s的时间字符串
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string ConvertSecondsToCountDown(int seconds)
    {
        string result = "";
        int hour = seconds / 3600;
        int minute = seconds / 60 % 60;
        int second = seconds % 60;
        if (hour >= 1)
        {
            result = string.Format("{0:D2}h{1:D2}m{2:D2}s", hour, minute, second);
        }
        else if (minute >= 1)
        {
            result = string.Format("{0:D2}m{1:D2}s", minute, second);
        }
        else
        {
            result = string.Format("{0:D2}s", second);
        }
        return result;
    }
}

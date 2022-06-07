using System;
using UnityEngine;


public enum EaseType
{
    Liner,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc,
}

public static class EaseUtil
{

    /// <summary>
    /// 缓动函数公式
    /// </summary>
    /// <param name="currentTime">当前时间（从动画开始播放计时）</param>
    /// <param name="beginValue">起始值</param>
    /// <param name="changeValue">改变值</param>
    /// <param name="duration">总动画时间</param>
    /// <returns></returns>
    public static float EasingMethod(float currentTime, float beginValue, float changeValue, float duration, EaseType easeType)
    {
        if (easeType == EaseType.Liner)
        {
            return changeValue * currentTime / duration + beginValue;
        }
        else if (easeType == EaseType.EaseInQuad)
        {
            return changeValue * (currentTime /= duration) * currentTime + beginValue;
        }
        else if (easeType == EaseType.EaseOutQuad)
        {
            return -changeValue * (currentTime /= duration) * (currentTime - 2) + beginValue;
        }
        else if (easeType == EaseType.EaseInOutQuad)
        {
            if ((currentTime /= duration / 2) < 1)
            {
                return changeValue / 2 * Mathf.Pow(currentTime, 2) + beginValue;
            }
            else
            {
                return -changeValue / 2 * ((--currentTime) * (currentTime - 2) - 1) + beginValue;
            }
        }
        else if (easeType == EaseType.EaseInCubic)
        {
            return changeValue * (currentTime /= duration) * Mathf.Pow(currentTime, 2) + beginValue;
        }
        else if (easeType == EaseType.EaseOutCubic)
        {
            return changeValue * ((currentTime = currentTime / duration - 1) * Mathf.Pow(currentTime, 2) + 1) + beginValue;
        }
        else if (easeType == EaseType.EaseInOutCubic)
        {
            if ((currentTime /= duration / 2) < 1)
            {
                return changeValue / 2 * Mathf.Pow(currentTime, 3) + beginValue;
            }
            else
            {
                return changeValue / 2 * ((currentTime -= 2) * Mathf.Pow(currentTime, 2) + 2) + beginValue;
            }
        }
        else if (easeType == EaseType.EaseInQuart)
        {
            return changeValue * (currentTime /= duration) * Mathf.Pow(currentTime, 3) + beginValue;
        }
        else if (easeType == EaseType.EaseOutQuart)
        {
            return -changeValue * ((currentTime = currentTime / duration - 1) * Mathf.Pow(currentTime, 3) - 1) + beginValue;
        }
        else if (easeType == EaseType.EaseInOutQuart)
        {
            if ((currentTime /= duration / 2) < 1)
            {
                return (float)(changeValue / 2 * Math.Pow(currentTime, 4) + beginValue);
            }
            else
            {
                return -changeValue / 2 * ((currentTime -= 2) * Mathf.Pow(currentTime, 3) - 2) + beginValue;
            }
        }
        else if (easeType == EaseType.EaseInQuint)
        {
            return changeValue * (currentTime /= duration) * Mathf.Pow(currentTime, 4) + beginValue;
        }
        else if (easeType == EaseType.EaseOutQuint)
        {
            return changeValue * ((currentTime = currentTime / duration - 1) * Mathf.Pow(currentTime, 4) + 1) + beginValue;
        }
        else if (easeType == EaseType.EaseInOutQuint)
        {
            if ((currentTime /= duration / 2) < 1)
            {
                return changeValue / 2 * Mathf.Pow(currentTime, 5) + beginValue;
            }
            else
            {
                return changeValue / 2 * ((currentTime -= 2) * Mathf.Pow(currentTime, 4) + 2) + beginValue;
            }
        }
        else if (easeType == EaseType.EaseInSine)
        {
            return -changeValue * Mathf.Cos(currentTime / duration * Mathf.PI / 2) + changeValue + beginValue;
        }
        else if (easeType == EaseType.EaseOutSine)
        {
            return changeValue * Mathf.Sin(currentTime / duration * Mathf.PI / 2) + beginValue;
        }
        else if (easeType == EaseType.EaseInOutSine)
        {
            return -changeValue / 2 * (Mathf.Cos((float)Math.PI * currentTime / duration) - 1) + beginValue;
        }
        else if (easeType == EaseType.EaseInExpo)
        {
            if (currentTime == 0)
            {
                return beginValue;
            }
            else
            {
                return changeValue * Mathf.Pow(2, 10 * (currentTime / duration - 1)) + beginValue;
            }
        }
        else if (easeType == EaseType.EaseOutExpo)
        {
            if (currentTime == duration)
            {
                return beginValue + changeValue;
            }
            else
            {
                return changeValue * (-Mathf.Pow(2, -10 * currentTime / duration) + 1) + beginValue;
            }
        }
        else if (easeType == EaseType.EaseInOutExpo)
        {
            if (currentTime == 0)
            {
                return beginValue;
            }
            if (currentTime == duration)
            {
                return beginValue + changeValue;
            }
            if ((currentTime /= duration / 2) < 1)
            {
                return changeValue / 2 * Mathf.Pow(2, 10 * (currentTime - 1)) + beginValue;
            }
            else
            {
                return changeValue / 2 * (-Mathf.Pow(2, -10 * --currentTime) + 2) + beginValue;
            }
        }
        else if (easeType == EaseType.EaseInCirc)
        {
            return -changeValue * (Mathf.Sqrt(1 - (currentTime /= duration) * currentTime) - 1) + beginValue;
        }
        else if (easeType == EaseType.EaseOutCirc)
        {
            return changeValue * Mathf.Sqrt(1 - (currentTime = currentTime / duration - 1) * currentTime) + beginValue;
        }
        else if (easeType == EaseType.EaseInOutCirc)
        {
            if ((currentTime /= duration / 2) < 1)
            {
                return -changeValue / 2 * (Mathf.Sqrt(1 - Mathf.Pow(currentTime, 2)) - 1) + beginValue;
            }
            else
            {
                return changeValue / 2 * (Mathf.Sqrt(1 - (currentTime -= 2) * currentTime) + 1) + beginValue;
            }
        }
        else
        {
            return 0;
        }
    }

}
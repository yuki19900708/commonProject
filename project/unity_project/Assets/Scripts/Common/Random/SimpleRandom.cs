using System;

/// <summary>
/// 非常简单的使用线性同余法的随机器
/// </summary>
public class SimpleRandom
{
    int a = 1664525;
    int c = 1013904223;
    long m = 1L << 32;

    long previous;

    public long Previous
    {
        get
        {
            return previous;
        }
    }
    
    /// <summary>
    /// 使用时间变量TickCount初始化
    /// </summary>
    public SimpleRandom() : this((uint)Environment.TickCount)
    {
    }

    /// <summary>
    /// 使用种子初始化
    /// </summary>
    /// <param name="seed">种子</param>
    public SimpleRandom(uint seed)
    {
        previous = seed;
    }

    /// <summary>
    /// 强制设定前一次的结果，将便下一次结果与输入值相关
    /// </summary>
    /// <param name="previous"></param>
    public virtual void SetPrevious(uint previous)
    {
        this.previous = previous;
    }

    /// <summary>
    /// 获取下一个随机无符号整型值
    /// </summary>
    /// <returns></returns>
    public virtual uint Next()
    {
        uint num = (uint)((a * previous + c) % m);
        previous = num;
        return num;
    }

    /// <summary>
    /// 获取下一个随机无符号整型值, 范围[0, maxValue]
    /// </summary>
    /// <param name="maxValue">最大值</param>
    /// <returns></returns>
    public virtual uint Next(uint maxValue)
    {
        return Next(0, maxValue);
    }

    /// <summary>
    /// 获取下一个随机无符号整型值, 范围[minValue, maxValue]
    /// </summary>
    /// <param name="minValue">最小值</param>
    /// <param name="maxValue">最大值</param>
    /// <returns></returns>
    public virtual uint Next(uint minValue, uint maxValue)
    {
        if (minValue == maxValue)
        {
            return minValue;
        }
        else if (minValue < maxValue)
        {
            return Next() % (maxValue - minValue + 1) + minValue;
        }
        else
        {
            return Next() % (minValue - maxValue + 1) + maxValue;
        }
    }
}

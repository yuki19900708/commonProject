using System;

/// <summary>
/// �ǳ��򵥵�ʹ������ͬ�෨�������
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
    /// ʹ��ʱ�����TickCount��ʼ��
    /// </summary>
    public SimpleRandom() : this((uint)Environment.TickCount)
    {
    }

    /// <summary>
    /// ʹ�����ӳ�ʼ��
    /// </summary>
    /// <param name="seed">����</param>
    public SimpleRandom(uint seed)
    {
        previous = seed;
    }

    /// <summary>
    /// ǿ���趨ǰһ�εĽ����������һ�ν��������ֵ���
    /// </summary>
    /// <param name="previous"></param>
    public virtual void SetPrevious(uint previous)
    {
        this.previous = previous;
    }

    /// <summary>
    /// ��ȡ��һ������޷�������ֵ
    /// </summary>
    /// <returns></returns>
    public virtual uint Next()
    {
        uint num = (uint)((a * previous + c) % m);
        previous = num;
        return num;
    }

    /// <summary>
    /// ��ȡ��һ������޷�������ֵ, ��Χ[0, maxValue]
    /// </summary>
    /// <param name="maxValue">���ֵ</param>
    /// <returns></returns>
    public virtual uint Next(uint maxValue)
    {
        return Next(0, maxValue);
    }

    /// <summary>
    /// ��ȡ��һ������޷�������ֵ, ��Χ[minValue, maxValue]
    /// </summary>
    /// <param name="minValue">��Сֵ</param>
    /// <param name="maxValue">���ֵ</param>
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

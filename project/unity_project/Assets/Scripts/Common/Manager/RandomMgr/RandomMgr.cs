using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameProto;

public static class RandomMgr
{
    public const uint MAX_VALUE = 10000;
    public static List<Random_data> randList = new List<Random_data>();
    private static SimpleRandom random = new SimpleRandom();

    public static SimpleRandom Random
    {
        get
        {
            return random;
        }
    }

    public static Random_data GetRandomData(uint value, Random_action type, Random_result result, uint resultValue)
    {
        Random_data data = new Random_data();
        data.RandomMaxValue = value;
        data.RandomAction = type;
        data.RandomResult = result;
        data.RandomResultValue = resultValue;
        return data;
    }

    /// <summary>
    /// 设置Previous
    /// </summary>
    /// <param name="previous"></param>
    public static void SetSetPrevious(int previous)
    {
        Random.SetPrevious((uint)previous);
    }

    /// <summary>
    /// 开始随机
    /// </summary>
    /// <param name="previous"></param>
    public static void StartRandom()
    {
        randList.Clear();
    }

    /// <summary>
    /// 填入随机的值，行为类型 结果
    /// </summary>
    /// <param name="startValue"></param>
    /// <param name="endValue"></param>
    /// <param name="actionType"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static int RandomRange(int startValue,int endValue, Random_action actionType, Random_result result)
    {
        int randValue = (int)Random.Next((uint)startValue, (uint)endValue+1);
        Random_data oneData = new Random_data();
        oneData.RandomMinValue = (uint)startValue;
        oneData.RandomMaxValue = (uint)endValue;
        oneData.RandomAction = actionType;
        oneData.RandomResult = result;
        oneData.RandomResultValue = (uint)randValue;
        randList.Add(oneData);
        return randValue;
    }

    /// <summary>
    /// 随机数量
    /// </summary>
    /// <param name="startValue"></param>
    /// <param name="endValue"></param>
    /// <param name="actionType"></param>
    /// <returns></returns>
    public static int RandMount(int startValue, int endValue,Random_action actionType)
    {
        int randValue = (int)Random.Next((uint)startValue, (uint)endValue);
        Random_data oneData = new Random_data();
        oneData.RandomMinValue = (uint)startValue;
        oneData.RandomMaxValue = (uint)endValue;
        oneData.RandomAction = actionType;
        oneData.RandomResult =  Random_result.ObjectCount;
        oneData.RandomResultValue = (uint)randValue;
        randList.Add(oneData);
        return randValue;
    }

    /// <summary>
    /// 随机事件
    /// </summary>
    /// <param name="startValue"></param>
    /// <param name="endValue"></param>
    /// <param name="actionType"></param>
    /// <returns></returns>
    public static int RandTime(int startValue, int endValue, Random_action actionType)
    {
        int randValue = (int)Random.Next((uint)startValue, (uint)endValue + 1);
        Random_data oneData = new Random_data();
        oneData.RandomMinValue = (uint)startValue;
        oneData.RandomMaxValue = (uint)endValue;
        oneData.RandomAction = actionType;
        oneData.RandomResult = Random_result.Time;
        oneData.RandomResultValue = (uint)randValue;
        randList.Add(oneData);
        return randValue;
    }

    /// <summary>
    /// 获取结果
    /// </summary>
    /// <returns></returns>
    public static List<Random_data> GetResult()
    {
        return randList;
    }
}

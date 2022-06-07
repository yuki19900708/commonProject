using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 协同工具类，在代码中的任意地方开启Unity3D的协同
/// </summary>
public class CoroutineUtil : MonoBehaviour
{

    private static CoroutineUtil mInstance = null;
    private static bool hasDestroyed = false;


    public static bool HasDestroyed
    {
        get
        {
            return hasDestroyed;
        }
    }

    private static CoroutineUtil instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType(typeof(CoroutineUtil)) as CoroutineUtil;

                if (mInstance == null)
                {
                    mInstance = new GameObject("CoroutineTool").AddComponent<CoroutineUtil>();
                }
            }
            return mInstance;
        }
    }

    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this as CoroutineUtil;
        }
    }

    void OnDestroy()
    {
        hasDestroyed = true;
    }

    IEnumerator Perform(IEnumerator coroutine, Action callback)
    {
        yield return StartCoroutine(coroutine);
        if (callback != null)
        {
            callback();
        }
    }

    /// <summary>
    /// 开始一个协同
    /// </summary>
    /// <param name="coroutine">协同函数</param>
    /// <param name="callback">协同完成回调函数</param>
    public static void DoCoroutine(IEnumerator coroutine, Action callback = null)
    {
        instance.StartCoroutine(instance.Perform(coroutine, callback));

    }
}
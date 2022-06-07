using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadUtil : MonoBehaviour
{
    private static ThreadUtil instance;
    private static Queue<Action> actionList = new Queue<Action>();

    public static void QueueActionToMainThread(Action action)
    {
        if (instance == null)
        {
            Debug.LogError("Please add ThreadUtil to a active gameObject");
        }
        if (action != null)
        {
            actionList.Enqueue(action);
        }
    }

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        actionList.Clear();
    }

    // Update is called once per frame
    private void Update()
    {
        while (actionList.Count > 0)
        {
            Action action = actionList.Dequeue();
            action();
        }
    }
}

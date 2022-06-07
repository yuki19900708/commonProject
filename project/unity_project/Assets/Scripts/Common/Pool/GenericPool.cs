using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPool<T> where T : new()
{
    /// <summary>
    /// 从池子中取出，并且创建一个预制
    /// </summary>
    public event Action<T> Event_CreatPrefab;
    /// <summary>
    /// 从池子中取出的时候进行初始化操作
    /// </summary>
    public event Action<T> Event_OutPool;
    /// <summary>
    /// 返回池子的时候进行归池逻辑处理
    /// </summary>
    public event Action<T> Event_ReturnPool;

    /// <summary>
    /// 池子
    /// </summary>
    protected Queue<T> pool;

    /// <summary>
    ///  所有出池的队列
    /// </summary>
    protected List<T> outPutList;

    public Queue<T> GetPoolData()
    {
        return pool;
    }

    public List<T> GetOutPoolList()
    {
        List<T> resultList = new List<T>();
        resultList.AddRange(outPutList);
        return resultList;
    }

    public GenericPool()
    {
        pool = new Queue<T>();
        outPutList = new List<T>();
    }

    public virtual void Clear()
    {
        pool.Clear();
    }

    public virtual T GetInstance()
    {
        T ret = default(T);
        if (pool.Count > 0)
        {
            ret = pool.Dequeue();
        }
        else
        {
            if (ret == null)
            {
                ret = CreateInstance();
                if (Event_CreatPrefab != null)
                {
                    Event_CreatPrefab(ret);
                }

            }
        }
        if (ret != null && Event_OutPool != null)
        {
            Event_OutPool(ret);
        }
        outPutList.Add(ret);
        return ret;
    }

    public virtual T CreateInstance()
    {
        return new T();
    }

    public virtual void RecycleInstance(T obj)
    {
        if (Event_ReturnPool != null)
        {
            Event_ReturnPool(obj);
        }
        outPutList.Remove(obj);
        pool.Enqueue(obj);
    }
}

public class UnityEngineObjectPool<T> : GenericPool<T> where T : UnityEngine.Object, new()
{
    /// <summary>
    /// 对象的基础预设
    /// </summary>
    public T prefab;

    public UnityEngineObjectPool(T prefab) : base()
    {
        this.prefab = prefab;
    }

    public override T CreateInstance()
    {
        UnityEngine.Object obj = UnityEngine.Object.Instantiate(prefab);
        obj.name = prefab.name;
        return obj as T;
    }
}

public class GameObjectPool : UnityEngineObjectPool<GameObject>, ITranformParent
{
    public Transform Parent
    {
        get;
        set;
    }

    public GameObjectPool(GameObject prefab, Transform transform) : base(prefab)
    {
        Parent = new GameObject().transform;
        Parent.name = prefab.name;
        if (transform != null)
        {
            Parent.SetParent(transform);
        }
    }

    public override GameObject GetInstance()
    {
        GameObject go = base.GetInstance();
        go.transform.localScale = Vector3.one;
        go.SetActive(true);
        return go;
    }

    public override void Clear()
    {
        foreach (GameObject obj in pool)
        {
            GameObject.Destroy(obj);
        }
        base.Clear();
    }

    public override void RecycleInstance(GameObject obj)
    {
        obj.transform.SetParent(Parent, false);
        obj.gameObject.SetActive(false);
        base.RecycleInstance(obj);
    }
}

public class MonoBehaviourPool<T> : UnityEngineObjectPool<T>, ITranformParent where T : MonoBehaviour, new()
{
    public Transform Parent
    {
        get;
        set;
    }

    public override T GetInstance()
    {
        T t = base.GetInstance();
        t.transform.SetParent(Parent, false);
        t.gameObject.SetActive(true);

        if (pool.Contains(t))
        {
            Debug.LogError("非常非常非常致命的错误 ： 东西拿出来以后  本体仍然在池子里  叫卢洋！！！" + t.gameObject.name);
        }
        return t;
    }

    public void CreateAndAddInstance()
    {
        T t = GetInstance();
        t.gameObject.SetActive(false);
        pool.Enqueue(t);
    }

    public MonoBehaviourPool(T prefab, Transform transform = null) : base(prefab)
    {
        Parent = new GameObject().transform;
        Parent.name = prefab.name;
        if (transform != null)
        {
            Parent.SetParent(transform);
            Parent.localScale = Vector3.one;
        }
    }

    public override void Clear()
    {
        foreach (UnityEngine.Object obj in pool)
        {
            Component.Destroy(obj);
        }
    }

    public override void RecycleInstance(T obj)
    {
        if (Parent != null)
        {
            //Debug.Log(Parent.name);
        }
        else
        {
            //Debug.Log(obj.name);
        }
        if (pool.Contains(obj))
        {
            Debug.LogError("非常非常非常致命的错误 ： 池子里的东西进行了二次回收  叫卢洋！！！" + obj.name);
        }
        obj.transform.SetParent(Parent, false);
        obj.gameObject.SetActive(false);
        base.RecycleInstance(obj);
    }
}

public interface ITranformParent
{
    /// <summary>
    /// 池子物体所在的父物体
    /// </summary>
    Transform Parent
    {
        get;
        set;
    }
}

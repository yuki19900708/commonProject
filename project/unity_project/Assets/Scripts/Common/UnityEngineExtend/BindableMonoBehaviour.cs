
using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

public class BindableMonoBehaviour : MonoBehaviour {
    /// <summary>[ContextMenu("TestSetVar")]
    /// 该特性 可以 在Unity->Inspector面板右键弹出菜单添加一个按钮。
    /// 点击触发下面的方法 快速对已经定义的变量进行赋值
    /// </summary>
    [ContextMenu("Bind Property")]
    void BindProperty()
    {
        Bind(this);//使用工具类对当前选中对象进行 变量赋值
    }

    public static void Bind(MonoBehaviour targetMonoBhv)
    {
        //反射遍历一个类的属性，参照文章：https://www.cnblogs.com/cuihongyu3503319/archive/2011/11/04/2235634.html
        CacheGameObjet cacehGmObj = new CacheGameObjet(targetMonoBhv);
        Type t = targetMonoBhv.GetType();
        //注意， 目标类的变量必须要设置了get 和set方法的属性,反射才能获得该属性
        FieldInfo[] propertyList = t.GetFields();
        List<string> strX = new List<string>();
        foreach (FieldInfo item in propertyList)
        {
            //作用于排除继承于MonoBehaviour，MonoBehaviour 的自带 变量
            if (item == null)
            {
                Debug.Log("挂载的脚本未赋值");
            }

            //对下面这个鞋类型不去检测与获取
            if (item.FieldType == typeof(float) ||
                item.FieldType == typeof(int) ||
                item.FieldType == typeof(double) ||
                item.FieldType == typeof(double) ||
                item.FieldType == typeof(Vector2) ||
                item.FieldType == typeof(Vector3) ||
                item.FieldType == typeof(Vector4) ||
                item.FieldType == typeof(Sprite) ||
                item.FieldType == typeof(Enum) ||
                item.FieldType == typeof(Quaternion) ||
                item.FieldType == typeof(Rect) ||
                item.FieldType == typeof(AnimationCurve)
                )
            {
                continue;
            }

            Transform gmObj = cacehGmObj.GetObj(item.Name);//从子对象里面找名字相同的对象。
            if (gmObj == null)
            {
                Debug.Log("<color=red>请注意，这个变量没有赋值(不存在此名字的Object)！！这个变量:[" + item.Name + "]</color>");
                continue;
            }

            if (gmObj != null)
            {
                Type monobhv = item.FieldType;//.GetType();
                if (item.FieldType == gmObj.gameObject.GetType())
                {
                    if (item.GetValue(targetMonoBhv) == null)
                    {
                        strX.Add("Type:" + gmObj.gameObject.GetType() + " Name:<color=green>" + item.Name + "</color>");
                    }
                    item.SetValue(targetMonoBhv, gmObj.gameObject);//给变量赋值
                    continue;
                }
                object tagertObject = null;
                try
                {
                    tagertObject = gmObj.GetComponent(monobhv);
                }
                catch
                {

                }
                if (tagertObject != null)
                {
                    if (item.GetValue(targetMonoBhv) == null)
                    {
                        strX.Add("Type:" + monobhv + "  Name:<color=green>" + item.Name + "</color>");
                    }
                    item.SetValue(targetMonoBhv, tagertObject);//给变量赋值
                }
                else
                {
                    Debug.Log("<color=red>请注意，这个变量没有赋值！！这个变量:[" + item.Name + "]1.有对应的GameObject,但是没有想要的组件：" + monobhv.ToString() + "</color>");
                }
            }
        }
        Debug.Log("赋值列表数：" + strX.Count + "\n");
        for (int i = 0; i < strX.Count; i++)
        {
            Debug.Log(strX[i] + "\n");
        }


    }
    }

    public struct CacheGameObjet
    {
        public Transform[] allChilds;
        public CacheGameObjet(MonoBehaviour parent)
        {
            allChilds = parent.GetComponentsInChildren<Transform>();
        }

        /// <summary>从子对象里查找 指定Name  的 对象并返回
        /// </summary>
        /// <param name="nameX"></param>
        /// <returns></returns>
        public Transform GetObj(string name)
        {
            for (int i = 0; i < allChilds.Length; i++)
            {
                if (allChilds[i].name.ToUpper() == name.ToUpper())
                {
                    return allChilds[i];
                }
            }
            return null;
        }
    }

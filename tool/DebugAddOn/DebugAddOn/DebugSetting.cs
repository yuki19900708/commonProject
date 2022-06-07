using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Debug注册模版
/// </summary>
[CreateAssetMenu(menuName = "DebugAddOn/Create DebugSetting")]
public class DebugSetting : ScriptableObject
{
    /// <summary>
    /// 日志颜色配置列表
    /// </summary>
    [SerializeField]
    [Header("日志颜色配置列表")]
    public List<LogColorConfigItem> logColorConfigItemList;

    static DebugSetting instance;

    static List<string> nameList = new List<string>();
    /// <summary>
    /// 外部调用的实例
    /// </summary>
    public static DebugSetting Instance
    {
        get
        {
            if (instance == null)
            {
                nameList.Clear();
                instance = Resources.Load<DebugSetting>("DebugAddOnSettings");

                if (instance != null)
                {
                    foreach (LogColorConfigItem item in instance.logColorConfigItemList)
                    {
                        nameList.Add(item.developerName);
                    }
                }

                DebugSetting[] debugArray = Resources.LoadAll<DebugSetting>("DebugAddOn");

                foreach (DebugSetting set in debugArray)
                {
                    SetLogConfig(set);
                }
            }
            return instance;
        }
    }

    static void SetLogConfig(DebugSetting debugSetting)
    {
        foreach (LogColorConfigItem item in debugSetting.logColorConfigItemList)
        {
            for (int i = 0; i < instance.logColorConfigItemList.Count; i++)
            {
                if (instance.logColorConfigItemList[i].developerName == item.developerName)
                {
                    instance.logColorConfigItemList.Remove(instance.logColorConfigItemList[i]);
                    i--;
                }
            }
            instance.logColorConfigItemList.Add(item);
        }
    }
}

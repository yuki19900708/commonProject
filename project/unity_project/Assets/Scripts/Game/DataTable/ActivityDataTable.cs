using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ActivityDataTable : ScriptableObject
{
    public List<ActivityData> activityDataTable = new List<ActivityData>();
    public Dictionary<int, ActivityData> activityDataDic = new Dictionary<int, ActivityData>();

    public void SetDatas(object[] obj)
    {
        activityDataTable.Clear();
        foreach (object o in obj)
        {
            activityDataTable.Add(o as ActivityData);
        }
    }

    public List<ActivityData> GetAllData()
    {
        if (activityDataTable == null || activityDataTable.Count == 0)
        {
            Debug.LogError("ActivityDataTable未导入asset");
        }
        return activityDataTable;
    }

    public IList<ActivityData> GetAllReadonlyData()
    {
        if (activityDataTable == null || activityDataTable.Count == 0)
        {
            Debug.LogError("ActivityDataTable未导入asset");
        }

        ReadOnlyCollection<ActivityData> readOnlyActivityData = new ReadOnlyCollection<ActivityData>(activityDataTable);

        return readOnlyActivityData ;
    }

    public ActivityData GetData(int index)
    {
        if (activityDataTable == null || activityDataTable.Count == 0)
        {
            Debug.LogError("ActivityDataTable未导入asset");
        }
        if (activityDataDic.Count == 0)
        {
            ReadOnlyCollection<ActivityData> readOnlyActivityData = new ReadOnlyCollection<ActivityData>(activityDataTable);
            foreach (ActivityData value in readOnlyActivityData)
            {
                if (activityDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //activityDataDic.Add(value.id, value.Clone());
                activityDataDic.Add(value.id, value);
            }
        }
        if (activityDataDic.ContainsKey(index))
        {
            return activityDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


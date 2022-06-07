using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TimedChestDataTable : ScriptableObject
{
    public List<TimedChestData> timedChestDataTable = new List<TimedChestData>();
    public Dictionary<int, TimedChestData> timedChestDataDic = new Dictionary<int, TimedChestData>();

    public void SetDatas(object[] obj)
    {
        timedChestDataTable.Clear();
        foreach (object o in obj)
        {
            timedChestDataTable.Add(o as TimedChestData);
        }
    }

    public List<TimedChestData> GetAllData()
    {
        if (timedChestDataTable == null || timedChestDataTable.Count == 0)
        {
            Debug.LogError("TimedChestDataTable未导入asset");
        }
        return timedChestDataTable;
    }

    public IList<TimedChestData> GetAllReadonlyData()
    {
        if (timedChestDataTable == null || timedChestDataTable.Count == 0)
        {
            Debug.LogError("TimedChestDataTable未导入asset");
        }

        ReadOnlyCollection<TimedChestData> readOnlyTimedChestData = new ReadOnlyCollection<TimedChestData>(timedChestDataTable);

        return readOnlyTimedChestData ;
    }

    public TimedChestData GetData(int index)
    {
        if (timedChestDataTable == null || timedChestDataTable.Count == 0)
        {
            Debug.LogError("TimedChestDataTable未导入asset");
        }
        if (timedChestDataDic.Count == 0)
        {
            ReadOnlyCollection<TimedChestData> readOnlyTimedChestData = new ReadOnlyCollection<TimedChestData>(timedChestDataTable);
            foreach (TimedChestData value in readOnlyTimedChestData)
            {
                if (timedChestDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //timedChestDataDic.Add(value.id, value.Clone());
                timedChestDataDic.Add(value.id, value);
            }
        }
        if (timedChestDataDic.ContainsKey(index))
        {
            return timedChestDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


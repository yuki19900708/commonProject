using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class DailyCheckDataTable : ScriptableObject
{
    public List<DailyCheckData> dailyCheckDataTable = new List<DailyCheckData>();
    public Dictionary<int, DailyCheckData> dailyCheckDataDic = new Dictionary<int, DailyCheckData>();

    public void SetDatas(object[] obj)
    {
        dailyCheckDataTable.Clear();
        foreach (object o in obj)
        {
            dailyCheckDataTable.Add(o as DailyCheckData);
        }
    }

    public List<DailyCheckData> GetAllData()
    {
        if (dailyCheckDataTable == null || dailyCheckDataTable.Count == 0)
        {
            Debug.LogError("DailyCheckDataTable未导入asset");
        }
        return dailyCheckDataTable;
    }

    public IList<DailyCheckData> GetAllReadonlyData()
    {
        if (dailyCheckDataTable == null || dailyCheckDataTable.Count == 0)
        {
            Debug.LogError("DailyCheckDataTable未导入asset");
        }

        ReadOnlyCollection<DailyCheckData> readOnlyDailyCheckData = new ReadOnlyCollection<DailyCheckData>(dailyCheckDataTable);

        return readOnlyDailyCheckData ;
    }

    public DailyCheckData GetData(int index)
    {
        if (dailyCheckDataTable == null || dailyCheckDataTable.Count == 0)
        {
            Debug.LogError("DailyCheckDataTable未导入asset");
        }
        if (dailyCheckDataDic.Count == 0)
        {
            ReadOnlyCollection<DailyCheckData> readOnlyDailyCheckData = new ReadOnlyCollection<DailyCheckData>(dailyCheckDataTable);
            foreach (DailyCheckData value in readOnlyDailyCheckData)
            {
                if (dailyCheckDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //dailyCheckDataDic.Add(value.id, value.Clone());
                dailyCheckDataDic.Add(value.id, value);
            }
        }
        if (dailyCheckDataDic.ContainsKey(index))
        {
            return dailyCheckDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class DailyCheckStageDataTable : ScriptableObject
{
    public List<DailyCheckStageData> dailyCheckStageDataTable = new List<DailyCheckStageData>();
    public Dictionary<int, DailyCheckStageData> dailyCheckStageDataDic = new Dictionary<int, DailyCheckStageData>();

    public void SetDatas(object[] obj)
    {
        dailyCheckStageDataTable.Clear();
        foreach (object o in obj)
        {
            dailyCheckStageDataTable.Add(o as DailyCheckStageData);
        }
    }

    public List<DailyCheckStageData> GetAllData()
    {
        if (dailyCheckStageDataTable == null || dailyCheckStageDataTable.Count == 0)
        {
            Debug.LogError("DailyCheckStageDataTable未导入asset");
        }
        return dailyCheckStageDataTable;
    }

    public IList<DailyCheckStageData> GetAllReadonlyData()
    {
        if (dailyCheckStageDataTable == null || dailyCheckStageDataTable.Count == 0)
        {
            Debug.LogError("DailyCheckStageDataTable未导入asset");
        }

        ReadOnlyCollection<DailyCheckStageData> readOnlyDailyCheckStageData = new ReadOnlyCollection<DailyCheckStageData>(dailyCheckStageDataTable);

        return readOnlyDailyCheckStageData ;
    }

    public DailyCheckStageData GetData(int index)
    {
        if (dailyCheckStageDataTable == null || dailyCheckStageDataTable.Count == 0)
        {
            Debug.LogError("DailyCheckStageDataTable未导入asset");
        }
        if (dailyCheckStageDataDic.Count == 0)
        {
            ReadOnlyCollection<DailyCheckStageData> readOnlyDailyCheckStageData = new ReadOnlyCollection<DailyCheckStageData>(dailyCheckStageDataTable);
            foreach (DailyCheckStageData value in readOnlyDailyCheckStageData)
            {
                if (dailyCheckStageDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //dailyCheckStageDataDic.Add(value.id, value.Clone());
                dailyCheckStageDataDic.Add(value.id, value);
            }
        }
        if (dailyCheckStageDataDic.ContainsKey(index))
        {
            return dailyCheckStageDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


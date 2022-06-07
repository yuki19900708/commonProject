using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class DailyTimeStageDataTable : ScriptableObject
{
    public List<DailyTimeStageData> dailyTimeStageDataTable = new List<DailyTimeStageData>();
    public Dictionary<int, DailyTimeStageData> dailyTimeStageDataDic = new Dictionary<int, DailyTimeStageData>();

    public void SetDatas(object[] obj)
    {
        dailyTimeStageDataTable.Clear();
        foreach (object o in obj)
        {
            dailyTimeStageDataTable.Add(o as DailyTimeStageData);
        }
    }

    public List<DailyTimeStageData> GetAllData()
    {
        if (dailyTimeStageDataTable == null || dailyTimeStageDataTable.Count == 0)
        {
            Debug.LogError("DailyTimeStageDataTable未导入asset");
        }
        return dailyTimeStageDataTable;
    }

    public IList<DailyTimeStageData> GetAllReadonlyData()
    {
        if (dailyTimeStageDataTable == null || dailyTimeStageDataTable.Count == 0)
        {
            Debug.LogError("DailyTimeStageDataTable未导入asset");
        }

        ReadOnlyCollection<DailyTimeStageData> readOnlyDailyTimeStageData = new ReadOnlyCollection<DailyTimeStageData>(dailyTimeStageDataTable);

        return readOnlyDailyTimeStageData ;
    }

    public DailyTimeStageData GetData(int index)
    {
return null;
    }
}


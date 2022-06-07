using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class DailyChestsDataTable : ScriptableObject
{
    public List<DailyChestsData> dailyChestsDataTable = new List<DailyChestsData>();
    public Dictionary<int, DailyChestsData> dailyChestsDataDic = new Dictionary<int, DailyChestsData>();

    public void SetDatas(object[] obj)
    {
        dailyChestsDataTable.Clear();
        foreach (object o in obj)
        {
            dailyChestsDataTable.Add(o as DailyChestsData);
        }
    }

    public List<DailyChestsData> GetAllData()
    {
        if (dailyChestsDataTable == null || dailyChestsDataTable.Count == 0)
        {
            Debug.LogError("DailyChestsDataTable未导入asset");
        }
        return dailyChestsDataTable;
    }

    public IList<DailyChestsData> GetAllReadonlyData()
    {
        if (dailyChestsDataTable == null || dailyChestsDataTable.Count == 0)
        {
            Debug.LogError("DailyChestsDataTable未导入asset");
        }

        ReadOnlyCollection<DailyChestsData> readOnlyDailyChestsData = new ReadOnlyCollection<DailyChestsData>(dailyChestsDataTable);

        return readOnlyDailyChestsData ;
    }

    public DailyChestsData GetData(int index)
    {
        if (dailyChestsDataTable == null || dailyChestsDataTable.Count == 0)
        {
            Debug.LogError("DailyChestsDataTable未导入asset");
        }
        if (dailyChestsDataDic.Count == 0)
        {
            ReadOnlyCollection<DailyChestsData> readOnlyDailyChestsData = new ReadOnlyCollection<DailyChestsData>(dailyChestsDataTable);
            foreach (DailyChestsData value in readOnlyDailyChestsData)
            {
                if (dailyChestsDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //dailyChestsDataDic.Add(value.id, value.Clone());
                dailyChestsDataDic.Add(value.id, value);
            }
        }
        if (dailyChestsDataDic.ContainsKey(index))
        {
            return dailyChestsDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


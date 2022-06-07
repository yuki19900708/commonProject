using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class DailyTransactionDataTable : ScriptableObject
{
    public List<DailyTransactionData> dailyTransactionDataTable = new List<DailyTransactionData>();
    public Dictionary<int, DailyTransactionData> dailyTransactionDataDic = new Dictionary<int, DailyTransactionData>();

    public void SetDatas(object[] obj)
    {
        dailyTransactionDataTable.Clear();
        foreach (object o in obj)
        {
            dailyTransactionDataTable.Add(o as DailyTransactionData);
        }
    }

    public List<DailyTransactionData> GetAllData()
    {
        if (dailyTransactionDataTable == null || dailyTransactionDataTable.Count == 0)
        {
            Debug.LogError("DailyTransactionDataTable未导入asset");
        }
        return dailyTransactionDataTable;
    }

    public IList<DailyTransactionData> GetAllReadonlyData()
    {
        if (dailyTransactionDataTable == null || dailyTransactionDataTable.Count == 0)
        {
            Debug.LogError("DailyTransactionDataTable未导入asset");
        }

        ReadOnlyCollection<DailyTransactionData> readOnlyDailyTransactionData = new ReadOnlyCollection<DailyTransactionData>(dailyTransactionDataTable);

        return readOnlyDailyTransactionData ;
    }

    public DailyTransactionData GetData(int index)
    {
        if (dailyTransactionDataTable == null || dailyTransactionDataTable.Count == 0)
        {
            Debug.LogError("DailyTransactionDataTable未导入asset");
        }
        if (dailyTransactionDataDic.Count == 0)
        {
            ReadOnlyCollection<DailyTransactionData> readOnlyDailyTransactionData = new ReadOnlyCollection<DailyTransactionData>(dailyTransactionDataTable);
            foreach (DailyTransactionData value in readOnlyDailyTransactionData)
            {
                if (dailyTransactionDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //dailyTransactionDataDic.Add(value.id, value.Clone());
                dailyTransactionDataDic.Add(value.id, value);
            }
        }
        if (dailyTransactionDataDic.ContainsKey(index))
        {
            return dailyTransactionDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


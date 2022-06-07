using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TimedShopDataTable : ScriptableObject
{
    public List<TimedShopData> timedShopDataTable = new List<TimedShopData>();
    public Dictionary<int, TimedShopData> timedShopDataDic = new Dictionary<int, TimedShopData>();

    public void SetDatas(object[] obj)
    {
        timedShopDataTable.Clear();
        foreach (object o in obj)
        {
            timedShopDataTable.Add(o as TimedShopData);
        }
    }

    public List<TimedShopData> GetAllData()
    {
        if (timedShopDataTable == null || timedShopDataTable.Count == 0)
        {
            Debug.LogError("TimedShopDataTable未导入asset");
        }
        return timedShopDataTable;
    }

    public IList<TimedShopData> GetAllReadonlyData()
    {
        if (timedShopDataTable == null || timedShopDataTable.Count == 0)
        {
            Debug.LogError("TimedShopDataTable未导入asset");
        }

        ReadOnlyCollection<TimedShopData> readOnlyTimedShopData = new ReadOnlyCollection<TimedShopData>(timedShopDataTable);

        return readOnlyTimedShopData ;
    }

    public TimedShopData GetData(int index)
    {
        if (timedShopDataTable == null || timedShopDataTable.Count == 0)
        {
            Debug.LogError("TimedShopDataTable未导入asset");
        }
        if (timedShopDataDic.Count == 0)
        {
            ReadOnlyCollection<TimedShopData> readOnlyTimedShopData = new ReadOnlyCollection<TimedShopData>(timedShopDataTable);
            foreach (TimedShopData value in readOnlyTimedShopData)
            {
                if (timedShopDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //timedShopDataDic.Add(value.id, value.Clone());
                timedShopDataDic.Add(value.id, value);
            }
        }
        if (timedShopDataDic.ContainsKey(index))
        {
            return timedShopDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


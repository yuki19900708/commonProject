using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class LifePowerDataTable : ScriptableObject
{
    public List<LifePowerData> lifePowerDataTable = new List<LifePowerData>();
    public Dictionary<int, LifePowerData> lifePowerDataDic = new Dictionary<int, LifePowerData>();

    public void SetDatas(object[] obj)
    {
        lifePowerDataTable.Clear();
        foreach (object o in obj)
        {
            lifePowerDataTable.Add(o as LifePowerData);
        }
    }

    public List<LifePowerData> GetAllData()
    {
        if (lifePowerDataTable == null || lifePowerDataTable.Count == 0)
        {
            Debug.LogError("LifePowerDataTable未导入asset");
        }
        return lifePowerDataTable;
    }

    public IList<LifePowerData> GetAllReadonlyData()
    {
        if (lifePowerDataTable == null || lifePowerDataTable.Count == 0)
        {
            Debug.LogError("LifePowerDataTable未导入asset");
        }

        ReadOnlyCollection<LifePowerData> readOnlyLifePowerData = new ReadOnlyCollection<LifePowerData>(lifePowerDataTable);

        return readOnlyLifePowerData ;
    }

    public LifePowerData GetData(int index)
    {
        if (lifePowerDataTable == null || lifePowerDataTable.Count == 0)
        {
            Debug.LogError("LifePowerDataTable未导入asset");
        }
        if (lifePowerDataDic.Count == 0)
        {
            ReadOnlyCollection<LifePowerData> readOnlyLifePowerData = new ReadOnlyCollection<LifePowerData>(lifePowerDataTable);
            foreach (LifePowerData value in readOnlyLifePowerData)
            {
                if (lifePowerDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //lifePowerDataDic.Add(value.id, value.Clone());
                lifePowerDataDic.Add(value.id, value);
            }
        }
        if (lifePowerDataDic.ContainsKey(index))
        {
            return lifePowerDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class HarvestEventDataTable : ScriptableObject
{
    public List<HarvestEventData> harvestEventDataTable = new List<HarvestEventData>();
    public Dictionary<int, HarvestEventData> harvestEventDataDic = new Dictionary<int, HarvestEventData>();

    public void SetDatas(object[] obj)
    {
        harvestEventDataTable.Clear();
        foreach (object o in obj)
        {
            harvestEventDataTable.Add(o as HarvestEventData);
        }
    }

    public List<HarvestEventData> GetAllData()
    {
        if (harvestEventDataTable == null || harvestEventDataTable.Count == 0)
        {
            Debug.LogError("HarvestEventDataTable未导入asset");
        }
        return harvestEventDataTable;
    }

    public IList<HarvestEventData> GetAllReadonlyData()
    {
        if (harvestEventDataTable == null || harvestEventDataTable.Count == 0)
        {
            Debug.LogError("HarvestEventDataTable未导入asset");
        }

        ReadOnlyCollection<HarvestEventData> readOnlyHarvestEventData = new ReadOnlyCollection<HarvestEventData>(harvestEventDataTable);

        return readOnlyHarvestEventData ;
    }

    public HarvestEventData GetData(int index)
    {
        if (harvestEventDataTable == null || harvestEventDataTable.Count == 0)
        {
            Debug.LogError("HarvestEventDataTable未导入asset");
        }
        if (harvestEventDataDic.Count == 0)
        {
            ReadOnlyCollection<HarvestEventData> readOnlyHarvestEventData = new ReadOnlyCollection<HarvestEventData>(harvestEventDataTable);
            foreach (HarvestEventData value in readOnlyHarvestEventData)
            {
                if (harvestEventDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //harvestEventDataDic.Add(value.id, value.Clone());
                harvestEventDataDic.Add(value.id, value);
            }
        }
        if (harvestEventDataDic.ContainsKey(index))
        {
            return harvestEventDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class VegetationDataTable : ScriptableObject
{
    public List<VegetationData> vegetationDataTable = new List<VegetationData>();
    public Dictionary<int, VegetationData> vegetationDataDic = new Dictionary<int, VegetationData>();

    public void SetDatas(object[] obj)
    {
        vegetationDataTable.Clear();
        foreach (object o in obj)
        {
            vegetationDataTable.Add(o as VegetationData);
        }
    }

    public List<VegetationData> GetAllData()
    {
        if (vegetationDataTable == null || vegetationDataTable.Count == 0)
        {
            Debug.LogError("VegetationDataTable未导入asset");
        }
        return vegetationDataTable;
    }

    public IList<VegetationData> GetAllReadonlyData()
    {
        if (vegetationDataTable == null || vegetationDataTable.Count == 0)
        {
            Debug.LogError("VegetationDataTable未导入asset");
        }

        ReadOnlyCollection<VegetationData> readOnlyVegetationData = new ReadOnlyCollection<VegetationData>(vegetationDataTable);

        return readOnlyVegetationData ;
    }

    public VegetationData GetData(int index)
    {
        if (vegetationDataTable == null || vegetationDataTable.Count == 0)
        {
            Debug.LogError("VegetationDataTable未导入asset");
        }
        if (vegetationDataDic.Count == 0)
        {
            ReadOnlyCollection<VegetationData> readOnlyVegetationData = new ReadOnlyCollection<VegetationData>(vegetationDataTable);
            foreach (VegetationData value in readOnlyVegetationData)
            {
                if (vegetationDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //vegetationDataDic.Add(value.id, value.Clone());
                vegetationDataDic.Add(value.id, value);
            }
        }
        if (vegetationDataDic.ContainsKey(index))
        {
            return vegetationDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


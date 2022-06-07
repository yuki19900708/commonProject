using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ChainTypeDataTable : ScriptableObject
{
    public List<ChainTypeData> chainTypeDataTable = new List<ChainTypeData>();
    public Dictionary<int, ChainTypeData> chainTypeDataDic = new Dictionary<int, ChainTypeData>();

    public void SetDatas(object[] obj)
    {
        chainTypeDataTable.Clear();
        foreach (object o in obj)
        {
            chainTypeDataTable.Add(o as ChainTypeData);
        }
    }

    public List<ChainTypeData> GetAllData()
    {
        if (chainTypeDataTable == null || chainTypeDataTable.Count == 0)
        {
            Debug.LogError("ChainTypeDataTable未导入asset");
        }
        return chainTypeDataTable;
    }

    public IList<ChainTypeData> GetAllReadonlyData()
    {
        if (chainTypeDataTable == null || chainTypeDataTable.Count == 0)
        {
            Debug.LogError("ChainTypeDataTable未导入asset");
        }

        ReadOnlyCollection<ChainTypeData> readOnlyChainTypeData = new ReadOnlyCollection<ChainTypeData>(chainTypeDataTable);

        return readOnlyChainTypeData ;
    }

    public ChainTypeData GetData(int index)
    {
        if (chainTypeDataTable == null || chainTypeDataTable.Count == 0)
        {
            Debug.LogError("ChainTypeDataTable未导入asset");
        }
        if (chainTypeDataDic.Count == 0)
        {
            ReadOnlyCollection<ChainTypeData> readOnlyChainTypeData = new ReadOnlyCollection<ChainTypeData>(chainTypeDataTable);
            foreach (ChainTypeData value in readOnlyChainTypeData)
            {
                if (chainTypeDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //chainTypeDataDic.Add(value.id, value.Clone());
                chainTypeDataDic.Add(value.id, value);
            }
        }
        if (chainTypeDataDic.ContainsKey(index))
        {
            return chainTypeDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ChainGroupDataTable : ScriptableObject
{
    public List<ChainGroupData> chainGroupDataTable = new List<ChainGroupData>();
    public Dictionary<int, ChainGroupData> chainGroupDataDic = new Dictionary<int, ChainGroupData>();

    public void SetDatas(object[] obj)
    {
        chainGroupDataTable.Clear();
        foreach (object o in obj)
        {
            chainGroupDataTable.Add(o as ChainGroupData);
        }
    }

    public List<ChainGroupData> GetAllData()
    {
        if (chainGroupDataTable == null || chainGroupDataTable.Count == 0)
        {
            Debug.LogError("ChainGroupDataTable未导入asset");
        }
        return chainGroupDataTable;
    }

    public IList<ChainGroupData> GetAllReadonlyData()
    {
        if (chainGroupDataTable == null || chainGroupDataTable.Count == 0)
        {
            Debug.LogError("ChainGroupDataTable未导入asset");
        }

        ReadOnlyCollection<ChainGroupData> readOnlyChainGroupData = new ReadOnlyCollection<ChainGroupData>(chainGroupDataTable);

        return readOnlyChainGroupData ;
    }

    public ChainGroupData GetData(int index)
    {
        if (chainGroupDataTable == null || chainGroupDataTable.Count == 0)
        {
            Debug.LogError("ChainGroupDataTable未导入asset");
        }
        if (chainGroupDataDic.Count == 0)
        {
            ReadOnlyCollection<ChainGroupData> readOnlyChainGroupData = new ReadOnlyCollection<ChainGroupData>(chainGroupDataTable);
            foreach (ChainGroupData value in readOnlyChainGroupData)
            {
                if (chainGroupDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //chainGroupDataDic.Add(value.id, value.Clone());
                chainGroupDataDic.Add(value.id, value);
            }
        }
        if (chainGroupDataDic.ContainsKey(index))
        {
            return chainGroupDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


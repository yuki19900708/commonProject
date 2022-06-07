using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class MergeEventDataTable : ScriptableObject
{
    public List<MergeEventData> mergeEventDataTable = new List<MergeEventData>();
    public Dictionary<int, MergeEventData> mergeEventDataDic = new Dictionary<int, MergeEventData>();

    public void SetDatas(object[] obj)
    {
        mergeEventDataTable.Clear();
        foreach (object o in obj)
        {
            mergeEventDataTable.Add(o as MergeEventData);
        }
    }

    public List<MergeEventData> GetAllData()
    {
        if (mergeEventDataTable == null || mergeEventDataTable.Count == 0)
        {
            Debug.LogError("MergeEventDataTable未导入asset");
        }
        return mergeEventDataTable;
    }

    public IList<MergeEventData> GetAllReadonlyData()
    {
        if (mergeEventDataTable == null || mergeEventDataTable.Count == 0)
        {
            Debug.LogError("MergeEventDataTable未导入asset");
        }

        ReadOnlyCollection<MergeEventData> readOnlyMergeEventData = new ReadOnlyCollection<MergeEventData>(mergeEventDataTable);

        return readOnlyMergeEventData ;
    }

    public MergeEventData GetData(int index)
    {
        if (mergeEventDataTable == null || mergeEventDataTable.Count == 0)
        {
            Debug.LogError("MergeEventDataTable未导入asset");
        }
        if (mergeEventDataDic.Count == 0)
        {
            ReadOnlyCollection<MergeEventData> readOnlyMergeEventData = new ReadOnlyCollection<MergeEventData>(mergeEventDataTable);
            foreach (MergeEventData value in readOnlyMergeEventData)
            {
                if (mergeEventDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //mergeEventDataDic.Add(value.id, value.Clone());
                mergeEventDataDic.Add(value.id, value);
            }
        }
        if (mergeEventDataDic.ContainsKey(index))
        {
            return mergeEventDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


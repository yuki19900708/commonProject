using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TipsDataTable : ScriptableObject
{
    public List<TipsData> tipsDataTable = new List<TipsData>();
    public Dictionary<int, TipsData> tipsDataDic = new Dictionary<int, TipsData>();

    public void SetDatas(object[] obj)
    {
        tipsDataTable.Clear();
        foreach (object o in obj)
        {
            tipsDataTable.Add(o as TipsData);
        }
    }

    public List<TipsData> GetAllData()
    {
        if (tipsDataTable == null || tipsDataTable.Count == 0)
        {
            Debug.LogError("TipsDataTable未导入asset");
        }
        return tipsDataTable;
    }

    public IList<TipsData> GetAllReadonlyData()
    {
        if (tipsDataTable == null || tipsDataTable.Count == 0)
        {
            Debug.LogError("TipsDataTable未导入asset");
        }

        ReadOnlyCollection<TipsData> readOnlyTipsData = new ReadOnlyCollection<TipsData>(tipsDataTable);

        return readOnlyTipsData ;
    }

    public TipsData GetData(int index)
    {
        if (tipsDataTable == null || tipsDataTable.Count == 0)
        {
            Debug.LogError("TipsDataTable未导入asset");
        }
        if (tipsDataDic.Count == 0)
        {
            ReadOnlyCollection<TipsData> readOnlyTipsData = new ReadOnlyCollection<TipsData>(tipsDataTable);
            foreach (TipsData value in readOnlyTipsData)
            {
                if (tipsDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //tipsDataDic.Add(value.id, value.Clone());
                tipsDataDic.Add(value.id, value);
            }
        }
        if (tipsDataDic.ContainsKey(index))
        {
            return tipsDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


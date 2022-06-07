using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class RankDataTable : ScriptableObject
{
    public List<RankData> rankDataTable = new List<RankData>();
    public Dictionary<int, RankData> rankDataDic = new Dictionary<int, RankData>();

    public void SetDatas(object[] obj)
    {
        rankDataTable.Clear();
        foreach (object o in obj)
        {
            rankDataTable.Add(o as RankData);
        }
    }

    public List<RankData> GetAllData()
    {
        if (rankDataTable == null || rankDataTable.Count == 0)
        {
            Debug.LogError("RankDataTable未导入asset");
        }
        return rankDataTable;
    }

    public IList<RankData> GetAllReadonlyData()
    {
        if (rankDataTable == null || rankDataTable.Count == 0)
        {
            Debug.LogError("RankDataTable未导入asset");
        }

        ReadOnlyCollection<RankData> readOnlyRankData = new ReadOnlyCollection<RankData>(rankDataTable);

        return readOnlyRankData ;
    }

    public RankData GetData(int index)
    {
        if (rankDataTable == null || rankDataTable.Count == 0)
        {
            Debug.LogError("RankDataTable未导入asset");
        }
        if (rankDataDic.Count == 0)
        {
            ReadOnlyCollection<RankData> readOnlyRankData = new ReadOnlyCollection<RankData>(rankDataTable);
            foreach (RankData value in readOnlyRankData)
            {
                if (rankDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //rankDataDic.Add(value.id, value.Clone());
                rankDataDic.Add(value.id, value);
            }
        }
        if (rankDataDic.ContainsKey(index))
        {
            return rankDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


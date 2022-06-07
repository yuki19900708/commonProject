using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class RankRewardDataTable : ScriptableObject
{
    public List<RankRewardData> rankRewardDataTable = new List<RankRewardData>();
    public Dictionary<int, RankRewardData> rankRewardDataDic = new Dictionary<int, RankRewardData>();

    public void SetDatas(object[] obj)
    {
        rankRewardDataTable.Clear();
        foreach (object o in obj)
        {
            rankRewardDataTable.Add(o as RankRewardData);
        }
    }

    public List<RankRewardData> GetAllData()
    {
        if (rankRewardDataTable == null || rankRewardDataTable.Count == 0)
        {
            Debug.LogError("RankRewardDataTable未导入asset");
        }
        return rankRewardDataTable;
    }

    public IList<RankRewardData> GetAllReadonlyData()
    {
        if (rankRewardDataTable == null || rankRewardDataTable.Count == 0)
        {
            Debug.LogError("RankRewardDataTable未导入asset");
        }

        ReadOnlyCollection<RankRewardData> readOnlyRankRewardData = new ReadOnlyCollection<RankRewardData>(rankRewardDataTable);

        return readOnlyRankRewardData ;
    }

    public RankRewardData GetData(int index)
    {
        if (rankRewardDataTable == null || rankRewardDataTable.Count == 0)
        {
            Debug.LogError("RankRewardDataTable未导入asset");
        }
        if (rankRewardDataDic.Count == 0)
        {
            ReadOnlyCollection<RankRewardData> readOnlyRankRewardData = new ReadOnlyCollection<RankRewardData>(rankRewardDataTable);
            foreach (RankRewardData value in readOnlyRankRewardData)
            {
                if (rankRewardDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //rankRewardDataDic.Add(value.id, value.Clone());
                rankRewardDataDic.Add(value.id, value);
            }
        }
        if (rankRewardDataDic.ContainsKey(index))
        {
            return rankRewardDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


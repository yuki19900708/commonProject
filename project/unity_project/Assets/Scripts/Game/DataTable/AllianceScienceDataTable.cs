using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class AllianceScienceDataTable : ScriptableObject
{
    public List<AllianceScienceData> allianceScienceDataTable = new List<AllianceScienceData>();
    public Dictionary<int, AllianceScienceData> allianceScienceDataDic = new Dictionary<int, AllianceScienceData>();

    public void SetDatas(object[] obj)
    {
        allianceScienceDataTable.Clear();
        foreach (object o in obj)
        {
            allianceScienceDataTable.Add(o as AllianceScienceData);
        }
    }

    public List<AllianceScienceData> GetAllData()
    {
        if (allianceScienceDataTable == null || allianceScienceDataTable.Count == 0)
        {
            Debug.LogError("AllianceScienceDataTable未导入asset");
        }
        return allianceScienceDataTable;
    }

    public IList<AllianceScienceData> GetAllReadonlyData()
    {
        if (allianceScienceDataTable == null || allianceScienceDataTable.Count == 0)
        {
            Debug.LogError("AllianceScienceDataTable未导入asset");
        }

        ReadOnlyCollection<AllianceScienceData> readOnlyAllianceScienceData = new ReadOnlyCollection<AllianceScienceData>(allianceScienceDataTable);

        return readOnlyAllianceScienceData ;
    }

    public AllianceScienceData GetData(int index)
    {
        if (allianceScienceDataTable == null || allianceScienceDataTable.Count == 0)
        {
            Debug.LogError("AllianceScienceDataTable未导入asset");
        }
        if (allianceScienceDataDic.Count == 0)
        {
            ReadOnlyCollection<AllianceScienceData> readOnlyAllianceScienceData = new ReadOnlyCollection<AllianceScienceData>(allianceScienceDataTable);
            foreach (AllianceScienceData value in readOnlyAllianceScienceData)
            {
                if (allianceScienceDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //allianceScienceDataDic.Add(value.id, value.Clone());
                allianceScienceDataDic.Add(value.id, value);
            }
        }
        if (allianceScienceDataDic.ContainsKey(index))
        {
            return allianceScienceDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


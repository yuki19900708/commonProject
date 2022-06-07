using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class MonsterPortalDataTable : ScriptableObject
{
    public List<MonsterPortalData> monsterPortalDataTable = new List<MonsterPortalData>();
    public Dictionary<int, MonsterPortalData> monsterPortalDataDic = new Dictionary<int, MonsterPortalData>();

    public void SetDatas(object[] obj)
    {
        monsterPortalDataTable.Clear();
        foreach (object o in obj)
        {
            monsterPortalDataTable.Add(o as MonsterPortalData);
        }
    }

    public List<MonsterPortalData> GetAllData()
    {
        if (monsterPortalDataTable == null || monsterPortalDataTable.Count == 0)
        {
            Debug.LogError("MonsterPortalDataTable未导入asset");
        }
        return monsterPortalDataTable;
    }

    public IList<MonsterPortalData> GetAllReadonlyData()
    {
        if (monsterPortalDataTable == null || monsterPortalDataTable.Count == 0)
        {
            Debug.LogError("MonsterPortalDataTable未导入asset");
        }

        ReadOnlyCollection<MonsterPortalData> readOnlyMonsterPortalData = new ReadOnlyCollection<MonsterPortalData>(monsterPortalDataTable);

        return readOnlyMonsterPortalData ;
    }

    public MonsterPortalData GetData(int index)
    {
        if (monsterPortalDataTable == null || monsterPortalDataTable.Count == 0)
        {
            Debug.LogError("MonsterPortalDataTable未导入asset");
        }
        if (monsterPortalDataDic.Count == 0)
        {
            ReadOnlyCollection<MonsterPortalData> readOnlyMonsterPortalData = new ReadOnlyCollection<MonsterPortalData>(monsterPortalDataTable);
            foreach (MonsterPortalData value in readOnlyMonsterPortalData)
            {
                if (monsterPortalDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //monsterPortalDataDic.Add(value.id, value.Clone());
                monsterPortalDataDic.Add(value.id, value);
            }
        }
        if (monsterPortalDataDic.ContainsKey(index))
        {
            return monsterPortalDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


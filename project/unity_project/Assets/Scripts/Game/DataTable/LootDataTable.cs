using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class LootDataTable : ScriptableObject
{
    public List<LootData> lootDataTable = new List<LootData>();
    public Dictionary<int, LootData> lootDataDic = new Dictionary<int, LootData>();

    public void SetDatas(object[] obj)
    {
        lootDataTable.Clear();
        foreach (object o in obj)
        {
            lootDataTable.Add(o as LootData);
        }
    }

    public List<LootData> GetAllData()
    {
        if (lootDataTable == null || lootDataTable.Count == 0)
        {
            Debug.LogError("LootDataTable未导入asset");
        }
        return lootDataTable;
    }

    public IList<LootData> GetAllReadonlyData()
    {
        if (lootDataTable == null || lootDataTable.Count == 0)
        {
            Debug.LogError("LootDataTable未导入asset");
        }

        ReadOnlyCollection<LootData> readOnlyLootData = new ReadOnlyCollection<LootData>(lootDataTable);

        return readOnlyLootData ;
    }

    public LootData GetData(int index)
    {
        if (lootDataTable == null || lootDataTable.Count == 0)
        {
            Debug.LogError("LootDataTable未导入asset");
        }
        if (lootDataDic.Count == 0)
        {
            ReadOnlyCollection<LootData> readOnlyLootData = new ReadOnlyCollection<LootData>(lootDataTable);
            foreach (LootData value in readOnlyLootData)
            {
                if (lootDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //lootDataDic.Add(value.id, value.Clone());
                lootDataDic.Add(value.id, value);
            }
        }
        if (lootDataDic.ContainsKey(index))
        {
            return lootDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


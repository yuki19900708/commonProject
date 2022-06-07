using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class SpawnEventDataTable : ScriptableObject
{
    public List<SpawnEventData> spawnEventDataTable = new List<SpawnEventData>();
    public Dictionary<int, SpawnEventData> spawnEventDataDic = new Dictionary<int, SpawnEventData>();

    public void SetDatas(object[] obj)
    {
        spawnEventDataTable.Clear();
        foreach (object o in obj)
        {
            spawnEventDataTable.Add(o as SpawnEventData);
        }
    }

    public List<SpawnEventData> GetAllData()
    {
        if (spawnEventDataTable == null || spawnEventDataTable.Count == 0)
        {
            Debug.LogError("SpawnEventDataTable未导入asset");
        }
        return spawnEventDataTable;
    }

    public IList<SpawnEventData> GetAllReadonlyData()
    {
        if (spawnEventDataTable == null || spawnEventDataTable.Count == 0)
        {
            Debug.LogError("SpawnEventDataTable未导入asset");
        }

        ReadOnlyCollection<SpawnEventData> readOnlySpawnEventData = new ReadOnlyCollection<SpawnEventData>(spawnEventDataTable);

        return readOnlySpawnEventData ;
    }

    public SpawnEventData GetData(int index)
    {
        if (spawnEventDataTable == null || spawnEventDataTable.Count == 0)
        {
            Debug.LogError("SpawnEventDataTable未导入asset");
        }
        if (spawnEventDataDic.Count == 0)
        {
            ReadOnlyCollection<SpawnEventData> readOnlySpawnEventData = new ReadOnlyCollection<SpawnEventData>(spawnEventDataTable);
            foreach (SpawnEventData value in readOnlySpawnEventData)
            {
                if (spawnEventDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //spawnEventDataDic.Add(value.id, value.Clone());
                spawnEventDataDic.Add(value.id, value);
            }
        }
        if (spawnEventDataDic.ContainsKey(index))
        {
            return spawnEventDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


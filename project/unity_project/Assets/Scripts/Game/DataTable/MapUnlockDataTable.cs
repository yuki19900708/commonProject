using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class MapUnlockDataTable : ScriptableObject
{
    public List<MapUnlockData> mapUnlockDataTable = new List<MapUnlockData>();
    public Dictionary<int, MapUnlockData> mapUnlockDataDic = new Dictionary<int, MapUnlockData>();

    public void SetDatas(object[] obj)
    {
        mapUnlockDataTable.Clear();
        foreach (object o in obj)
        {
            mapUnlockDataTable.Add(o as MapUnlockData);
        }
    }

    public List<MapUnlockData> GetAllData()
    {
        if (mapUnlockDataTable == null || mapUnlockDataTable.Count == 0)
        {
            Debug.LogError("MapUnlockDataTable未导入asset");
        }
        return mapUnlockDataTable;
    }

    public IList<MapUnlockData> GetAllReadonlyData()
    {
        if (mapUnlockDataTable == null || mapUnlockDataTable.Count == 0)
        {
            Debug.LogError("MapUnlockDataTable未导入asset");
        }

        ReadOnlyCollection<MapUnlockData> readOnlyMapUnlockData = new ReadOnlyCollection<MapUnlockData>(mapUnlockDataTable);

        return readOnlyMapUnlockData ;
    }

    public MapUnlockData GetData(int index)
    {
        if (mapUnlockDataTable == null || mapUnlockDataTable.Count == 0)
        {
            Debug.LogError("MapUnlockDataTable未导入asset");
        }
        if (mapUnlockDataDic.Count == 0)
        {
            ReadOnlyCollection<MapUnlockData> readOnlyMapUnlockData = new ReadOnlyCollection<MapUnlockData>(mapUnlockDataTable);
            foreach (MapUnlockData value in readOnlyMapUnlockData)
            {
                if (mapUnlockDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //mapUnlockDataDic.Add(value.id, value.Clone());
                mapUnlockDataDic.Add(value.id, value);
            }
        }
        if (mapUnlockDataDic.ContainsKey(index))
        {
            return mapUnlockDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


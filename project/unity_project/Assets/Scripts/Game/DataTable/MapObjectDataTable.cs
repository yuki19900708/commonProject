using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class MapObjectDataTable : ScriptableObject
{
    public List<MapObjectData> mapObjectDataTable = new List<MapObjectData>();
    public Dictionary<int, MapObjectData> mapObjectDataDic = new Dictionary<int, MapObjectData>();

    public void SetDatas(object[] obj)
    {
        mapObjectDataTable.Clear();
        foreach (object o in obj)
        {
            mapObjectDataTable.Add(o as MapObjectData);
        }
    }

    public List<MapObjectData> GetAllData()
    {
        if (mapObjectDataTable == null || mapObjectDataTable.Count == 0)
        {
            Debug.LogError("MapObjectDataTable未导入asset");
        }
        return mapObjectDataTable;
    }

    public IList<MapObjectData> GetAllReadonlyData()
    {
        if (mapObjectDataTable == null || mapObjectDataTable.Count == 0)
        {
            Debug.LogError("MapObjectDataTable未导入asset");
        }

        ReadOnlyCollection<MapObjectData> readOnlyMapObjectData = new ReadOnlyCollection<MapObjectData>(mapObjectDataTable);

        return readOnlyMapObjectData ;
    }

    public MapObjectData GetData(int index)
    {
        if (mapObjectDataTable == null || mapObjectDataTable.Count == 0)
        {
            Debug.LogError("MapObjectDataTable未导入asset");
        }
        if (mapObjectDataDic.Count == 0)
        {
            ReadOnlyCollection<MapObjectData> readOnlyMapObjectData = new ReadOnlyCollection<MapObjectData>(mapObjectDataTable);
            foreach (MapObjectData value in readOnlyMapObjectData)
            {
                if (mapObjectDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //mapObjectDataDic.Add(value.id, value.Clone());
                mapObjectDataDic.Add(value.id, value);
            }
        }
        if (mapObjectDataDic.ContainsKey(index))
        {
            return mapObjectDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


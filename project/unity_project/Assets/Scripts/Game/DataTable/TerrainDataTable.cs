using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TerrainDataTable : ScriptableObject
{
    public List<TerrainData> terrainDataTable = new List<TerrainData>();
    public Dictionary<int, TerrainData> terrainDataDic = new Dictionary<int, TerrainData>();

    public void SetDatas(object[] obj)
    {
        terrainDataTable.Clear();
        foreach (object o in obj)
        {
            terrainDataTable.Add(o as TerrainData);
        }
    }

    public List<TerrainData> GetAllData()
    {
        if (terrainDataTable == null || terrainDataTable.Count == 0)
        {
            Debug.LogError("TerrainDataTable未导入asset");
        }
        return terrainDataTable;
    }

    public IList<TerrainData> GetAllReadonlyData()
    {
        if (terrainDataTable == null || terrainDataTable.Count == 0)
        {
            Debug.LogError("TerrainDataTable未导入asset");
        }

        ReadOnlyCollection<TerrainData> readOnlyTerrainData = new ReadOnlyCollection<TerrainData>(terrainDataTable);

        return readOnlyTerrainData ;
    }

    public TerrainData GetData(int index)
    {
return null;
    }
}


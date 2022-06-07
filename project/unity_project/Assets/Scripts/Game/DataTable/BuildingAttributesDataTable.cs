using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class BuildingAttributesDataTable : ScriptableObject
{
    public List<BuildingAttributesData> buildingAttributesDataTable = new List<BuildingAttributesData>();
    public Dictionary<int, BuildingAttributesData> buildingAttributesDataDic = new Dictionary<int, BuildingAttributesData>();

    public void SetDatas(object[] obj)
    {
        buildingAttributesDataTable.Clear();
        foreach (object o in obj)
        {
            buildingAttributesDataTable.Add(o as BuildingAttributesData);
        }
    }

    public List<BuildingAttributesData> GetAllData()
    {
        if (buildingAttributesDataTable == null || buildingAttributesDataTable.Count == 0)
        {
            Debug.LogError("BuildingAttributesDataTable未导入asset");
        }
        return buildingAttributesDataTable;
    }

    public IList<BuildingAttributesData> GetAllReadonlyData()
    {
        if (buildingAttributesDataTable == null || buildingAttributesDataTable.Count == 0)
        {
            Debug.LogError("BuildingAttributesDataTable未导入asset");
        }

        ReadOnlyCollection<BuildingAttributesData> readOnlyBuildingAttributesData = new ReadOnlyCollection<BuildingAttributesData>(buildingAttributesDataTable);

        return readOnlyBuildingAttributesData ;
    }

    public BuildingAttributesData GetData(int index)
    {
        if (buildingAttributesDataTable == null || buildingAttributesDataTable.Count == 0)
        {
            Debug.LogError("BuildingAttributesDataTable未导入asset");
        }
        if (buildingAttributesDataDic.Count == 0)
        {
            ReadOnlyCollection<BuildingAttributesData> readOnlyBuildingAttributesData = new ReadOnlyCollection<BuildingAttributesData>(buildingAttributesDataTable);
            foreach (BuildingAttributesData value in readOnlyBuildingAttributesData)
            {
                if (buildingAttributesDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //buildingAttributesDataDic.Add(value.id, value.Clone());
                buildingAttributesDataDic.Add(value.id, value);
            }
        }
        if (buildingAttributesDataDic.ContainsKey(index))
        {
            return buildingAttributesDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


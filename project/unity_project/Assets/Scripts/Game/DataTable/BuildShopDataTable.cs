using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class BuildShopDataTable : ScriptableObject
{
    public List<BuildShopData> buildShopDataTable = new List<BuildShopData>();
    public Dictionary<int, BuildShopData> buildShopDataDic = new Dictionary<int, BuildShopData>();

    public void SetDatas(object[] obj)
    {
        buildShopDataTable.Clear();
        foreach (object o in obj)
        {
            buildShopDataTable.Add(o as BuildShopData);
        }
    }

    public List<BuildShopData> GetAllData()
    {
        if (buildShopDataTable == null || buildShopDataTable.Count == 0)
        {
            Debug.LogError("BuildShopDataTable未导入asset");
        }
        return buildShopDataTable;
    }

    public IList<BuildShopData> GetAllReadonlyData()
    {
        if (buildShopDataTable == null || buildShopDataTable.Count == 0)
        {
            Debug.LogError("BuildShopDataTable未导入asset");
        }

        ReadOnlyCollection<BuildShopData> readOnlyBuildShopData = new ReadOnlyCollection<BuildShopData>(buildShopDataTable);

        return readOnlyBuildShopData ;
    }

    public BuildShopData GetData(int index)
    {
        if (buildShopDataTable == null || buildShopDataTable.Count == 0)
        {
            Debug.LogError("BuildShopDataTable未导入asset");
        }
        if (buildShopDataDic.Count == 0)
        {
            ReadOnlyCollection<BuildShopData> readOnlyBuildShopData = new ReadOnlyCollection<BuildShopData>(buildShopDataTable);
            foreach (BuildShopData value in readOnlyBuildShopData)
            {
                if (buildShopDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //buildShopDataDic.Add(value.id, value.Clone());
                buildShopDataDic.Add(value.id, value);
            }
        }
        if (buildShopDataDic.ContainsKey(index))
        {
            return buildShopDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


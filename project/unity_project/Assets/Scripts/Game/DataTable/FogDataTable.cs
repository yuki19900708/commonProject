using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class FogDataTable : ScriptableObject
{
    public List<FogData> fogDataTable = new List<FogData>();
    public Dictionary<int, FogData> fogDataDic = new Dictionary<int, FogData>();

    public void SetDatas(object[] obj)
    {
        fogDataTable.Clear();
        foreach (object o in obj)
        {
            fogDataTable.Add(o as FogData);
        }
    }

    public List<FogData> GetAllData()
    {
        if (fogDataTable == null || fogDataTable.Count == 0)
        {
            Debug.LogError("FogDataTable未导入asset");
        }
        return fogDataTable;
    }

    public IList<FogData> GetAllReadonlyData()
    {
        if (fogDataTable == null || fogDataTable.Count == 0)
        {
            Debug.LogError("FogDataTable未导入asset");
        }

        ReadOnlyCollection<FogData> readOnlyFogData = new ReadOnlyCollection<FogData>(fogDataTable);

        return readOnlyFogData ;
    }

    public FogData GetData(int index)
    {
        if (fogDataTable == null || fogDataTable.Count == 0)
        {
            Debug.LogError("FogDataTable未导入asset");
        }
        if (fogDataDic.Count == 0)
        {
            ReadOnlyCollection<FogData> readOnlyFogData = new ReadOnlyCollection<FogData>(fogDataTable);
            foreach (FogData value in readOnlyFogData)
            {
                if (fogDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //fogDataDic.Add(value.id, value.Clone());
                fogDataDic.Add(value.id, value);
            }
        }
        if (fogDataDic.ContainsKey(index))
        {
            return fogDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


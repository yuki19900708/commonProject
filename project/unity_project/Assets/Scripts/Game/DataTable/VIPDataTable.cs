using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class VIPDataTable : ScriptableObject
{
    public List<VIPData> vIPDataTable = new List<VIPData>();
    public Dictionary<int, VIPData> vIPDataDic = new Dictionary<int, VIPData>();

    public void SetDatas(object[] obj)
    {
        vIPDataTable.Clear();
        foreach (object o in obj)
        {
            vIPDataTable.Add(o as VIPData);
        }
    }

    public List<VIPData> GetAllData()
    {
        if (vIPDataTable == null || vIPDataTable.Count == 0)
        {
            Debug.LogError("VIPDataTable未导入asset");
        }
        return vIPDataTable;
    }

    public IList<VIPData> GetAllReadonlyData()
    {
        if (vIPDataTable == null || vIPDataTable.Count == 0)
        {
            Debug.LogError("VIPDataTable未导入asset");
        }

        ReadOnlyCollection<VIPData> readOnlyVIPData = new ReadOnlyCollection<VIPData>(vIPDataTable);

        return readOnlyVIPData ;
    }

    public VIPData GetData(int index)
    {
        if (vIPDataTable == null || vIPDataTable.Count == 0)
        {
            Debug.LogError("VIPDataTable未导入asset");
        }
        if (vIPDataDic.Count == 0)
        {
            ReadOnlyCollection<VIPData> readOnlyVIPData = new ReadOnlyCollection<VIPData>(vIPDataTable);
            foreach (VIPData value in readOnlyVIPData)
            {
                if (vIPDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //vIPDataDic.Add(value.id, value.Clone());
                vIPDataDic.Add(value.id, value);
            }
        }
        if (vIPDataDic.ContainsKey(index))
        {
            return vIPDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


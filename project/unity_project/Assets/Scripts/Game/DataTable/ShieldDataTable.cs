using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ShieldDataTable : ScriptableObject
{
    public List<ShieldData> shieldDataTable = new List<ShieldData>();
    public Dictionary<int, ShieldData> shieldDataDic = new Dictionary<int, ShieldData>();

    public void SetDatas(object[] obj)
    {
        shieldDataTable.Clear();
        foreach (object o in obj)
        {
            shieldDataTable.Add(o as ShieldData);
        }
    }

    public List<ShieldData> GetAllData()
    {
        if (shieldDataTable == null || shieldDataTable.Count == 0)
        {
            Debug.LogError("ShieldDataTable未导入asset");
        }
        return shieldDataTable;
    }

    public IList<ShieldData> GetAllReadonlyData()
    {
        if (shieldDataTable == null || shieldDataTable.Count == 0)
        {
            Debug.LogError("ShieldDataTable未导入asset");
        }

        ReadOnlyCollection<ShieldData> readOnlyShieldData = new ReadOnlyCollection<ShieldData>(shieldDataTable);

        return readOnlyShieldData ;
    }

    public ShieldData GetData(int index)
    {
        if (shieldDataTable == null || shieldDataTable.Count == 0)
        {
            Debug.LogError("ShieldDataTable未导入asset");
        }
        if (shieldDataDic.Count == 0)
        {
            ReadOnlyCollection<ShieldData> readOnlyShieldData = new ReadOnlyCollection<ShieldData>(shieldDataTable);
            foreach (ShieldData value in readOnlyShieldData)
            {
                if (shieldDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //shieldDataDic.Add(value.id, value.Clone());
                shieldDataDic.Add(value.id, value);
            }
        }
        if (shieldDataDic.ContainsKey(index))
        {
            return shieldDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


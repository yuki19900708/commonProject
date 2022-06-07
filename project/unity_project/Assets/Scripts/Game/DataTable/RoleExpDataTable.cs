using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class RoleExpDataTable : ScriptableObject
{
    public List<RoleExpData> roleExpDataTable = new List<RoleExpData>();
    public Dictionary<int, RoleExpData> roleExpDataDic = new Dictionary<int, RoleExpData>();

    public void SetDatas(object[] obj)
    {
        roleExpDataTable.Clear();
        foreach (object o in obj)
        {
            roleExpDataTable.Add(o as RoleExpData);
        }
    }

    public List<RoleExpData> GetAllData()
    {
        if (roleExpDataTable == null || roleExpDataTable.Count == 0)
        {
            Debug.LogError("RoleExpDataTable未导入asset");
        }
        return roleExpDataTable;
    }

    public IList<RoleExpData> GetAllReadonlyData()
    {
        if (roleExpDataTable == null || roleExpDataTable.Count == 0)
        {
            Debug.LogError("RoleExpDataTable未导入asset");
        }

        ReadOnlyCollection<RoleExpData> readOnlyRoleExpData = new ReadOnlyCollection<RoleExpData>(roleExpDataTable);

        return readOnlyRoleExpData ;
    }

    public RoleExpData GetData(int index)
    {
        if (roleExpDataTable == null || roleExpDataTable.Count == 0)
        {
            Debug.LogError("RoleExpDataTable未导入asset");
        }
        if (roleExpDataDic.Count == 0)
        {
            ReadOnlyCollection<RoleExpData> readOnlyRoleExpData = new ReadOnlyCollection<RoleExpData>(roleExpDataTable);
            foreach (RoleExpData value in readOnlyRoleExpData)
            {
                if (roleExpDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //roleExpDataDic.Add(value.id, value.Clone());
                roleExpDataDic.Add(value.id, value);
            }
        }
        if (roleExpDataDic.ContainsKey(index))
        {
            return roleExpDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


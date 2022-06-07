using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class BannedResponsTable : ScriptableObject
{
    public List<BannedRespons> bannedResponsTable = new List<BannedRespons>();
    public Dictionary<int, BannedRespons> bannedResponsDic = new Dictionary<int, BannedRespons>();

    public void SetDatas(object[] obj)
    {
        bannedResponsTable.Clear();
        foreach (object o in obj)
        {
            bannedResponsTable.Add(o as BannedRespons);
        }
    }

    public List<BannedRespons> GetAllData()
    {
        if (bannedResponsTable == null || bannedResponsTable.Count == 0)
        {
            Debug.LogError("BannedResponsTable未导入asset");
        }
        return bannedResponsTable;
    }

    public IList<BannedRespons> GetAllReadonlyData()
    {
        if (bannedResponsTable == null || bannedResponsTable.Count == 0)
        {
            Debug.LogError("BannedResponsTable未导入asset");
        }

        ReadOnlyCollection<BannedRespons> readOnlyBannedRespons = new ReadOnlyCollection<BannedRespons>(bannedResponsTable);

        return readOnlyBannedRespons ;
    }

    public BannedRespons GetData(int index)
    {
        if (bannedResponsTable == null || bannedResponsTable.Count == 0)
        {
            Debug.LogError("BannedResponsTable未导入asset");
        }
        if (bannedResponsDic.Count == 0)
        {
            ReadOnlyCollection<BannedRespons> readOnlyBannedRespons = new ReadOnlyCollection<BannedRespons>(bannedResponsTable);
            foreach (BannedRespons value in readOnlyBannedRespons)
            {
                if (bannedResponsDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //bannedResponsDic.Add(value.id, value.Clone());
                bannedResponsDic.Add(value.id, value);
            }
        }
        if (bannedResponsDic.ContainsKey(index))
        {
            return bannedResponsDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


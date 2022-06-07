using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class DeadLandDataTable : ScriptableObject
{
    public List<DeadLandData> deadLandDataTable = new List<DeadLandData>();
    public Dictionary<int, DeadLandData> deadLandDataDic = new Dictionary<int, DeadLandData>();

    public void SetDatas(object[] obj)
    {
        deadLandDataTable.Clear();
        foreach (object o in obj)
        {
            deadLandDataTable.Add(o as DeadLandData);
        }
    }

    public List<DeadLandData> GetAllData()
    {
        if (deadLandDataTable == null || deadLandDataTable.Count == 0)
        {
            Debug.LogError("DeadLandDataTable未导入asset");
        }
        return deadLandDataTable;
    }

    public IList<DeadLandData> GetAllReadonlyData()
    {
        if (deadLandDataTable == null || deadLandDataTable.Count == 0)
        {
            Debug.LogError("DeadLandDataTable未导入asset");
        }

        ReadOnlyCollection<DeadLandData> readOnlyDeadLandData = new ReadOnlyCollection<DeadLandData>(deadLandDataTable);

        return readOnlyDeadLandData ;
    }

    public DeadLandData GetData(int index)
    {
        if (deadLandDataTable == null || deadLandDataTable.Count == 0)
        {
            Debug.LogError("DeadLandDataTable未导入asset");
        }
        if (deadLandDataDic.Count == 0)
        {
            ReadOnlyCollection<DeadLandData> readOnlyDeadLandData = new ReadOnlyCollection<DeadLandData>(deadLandDataTable);
            foreach (DeadLandData value in readOnlyDeadLandData)
            {
                if (deadLandDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //deadLandDataDic.Add(value.id, value.Clone());
                deadLandDataDic.Add(value.id, value);
            }
        }
        if (deadLandDataDic.ContainsKey(index))
        {
            return deadLandDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


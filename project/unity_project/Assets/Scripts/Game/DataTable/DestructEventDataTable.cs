using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class DestructEventDataTable : ScriptableObject
{
    public List<DestructEventData> destructEventDataTable = new List<DestructEventData>();
    public Dictionary<int, DestructEventData> destructEventDataDic = new Dictionary<int, DestructEventData>();

    public void SetDatas(object[] obj)
    {
        destructEventDataTable.Clear();
        foreach (object o in obj)
        {
            destructEventDataTable.Add(o as DestructEventData);
        }
    }

    public List<DestructEventData> GetAllData()
    {
        if (destructEventDataTable == null || destructEventDataTable.Count == 0)
        {
            Debug.LogError("DestructEventDataTable未导入asset");
        }
        return destructEventDataTable;
    }

    public IList<DestructEventData> GetAllReadonlyData()
    {
        if (destructEventDataTable == null || destructEventDataTable.Count == 0)
        {
            Debug.LogError("DestructEventDataTable未导入asset");
        }

        ReadOnlyCollection<DestructEventData> readOnlyDestructEventData = new ReadOnlyCollection<DestructEventData>(destructEventDataTable);

        return readOnlyDestructEventData ;
    }

    public DestructEventData GetData(int index)
    {
        if (destructEventDataTable == null || destructEventDataTable.Count == 0)
        {
            Debug.LogError("DestructEventDataTable未导入asset");
        }
        if (destructEventDataDic.Count == 0)
        {
            ReadOnlyCollection<DestructEventData> readOnlyDestructEventData = new ReadOnlyCollection<DestructEventData>(destructEventDataTable);
            foreach (DestructEventData value in readOnlyDestructEventData)
            {
                if (destructEventDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //destructEventDataDic.Add(value.id, value.Clone());
                destructEventDataDic.Add(value.id, value);
            }
        }
        if (destructEventDataDic.ContainsKey(index))
        {
            return destructEventDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TapEventDataTable : ScriptableObject
{
    public List<TapEventData> tapEventDataTable = new List<TapEventData>();
    public Dictionary<int, TapEventData> tapEventDataDic = new Dictionary<int, TapEventData>();

    public void SetDatas(object[] obj)
    {
        tapEventDataTable.Clear();
        foreach (object o in obj)
        {
            tapEventDataTable.Add(o as TapEventData);
        }
    }

    public List<TapEventData> GetAllData()
    {
        if (tapEventDataTable == null || tapEventDataTable.Count == 0)
        {
            Debug.LogError("TapEventDataTable未导入asset");
        }
        return tapEventDataTable;
    }

    public IList<TapEventData> GetAllReadonlyData()
    {
        if (tapEventDataTable == null || tapEventDataTable.Count == 0)
        {
            Debug.LogError("TapEventDataTable未导入asset");
        }

        ReadOnlyCollection<TapEventData> readOnlyTapEventData = new ReadOnlyCollection<TapEventData>(tapEventDataTable);

        return readOnlyTapEventData ;
    }

    public TapEventData GetData(int index)
    {
        if (tapEventDataTable == null || tapEventDataTable.Count == 0)
        {
            Debug.LogError("TapEventDataTable未导入asset");
        }
        if (tapEventDataDic.Count == 0)
        {
            ReadOnlyCollection<TapEventData> readOnlyTapEventData = new ReadOnlyCollection<TapEventData>(tapEventDataTable);
            foreach (TapEventData value in readOnlyTapEventData)
            {
                if (tapEventDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //tapEventDataDic.Add(value.id, value.Clone());
                tapEventDataDic.Add(value.id, value);
            }
        }
        if (tapEventDataDic.ContainsKey(index))
        {
            return tapEventDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


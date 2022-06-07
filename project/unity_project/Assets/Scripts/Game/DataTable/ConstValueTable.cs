using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ConstValueTable : ScriptableObject
{
    public List<ConstValue> constValueTable = new List<ConstValue>();
    public Dictionary<int, ConstValue> constValueDic = new Dictionary<int, ConstValue>();

    public void SetDatas(object[] obj)
    {
        constValueTable.Clear();
        foreach (object o in obj)
        {
            constValueTable.Add(o as ConstValue);
        }
    }

    public List<ConstValue> GetAllData()
    {
        if (constValueTable == null || constValueTable.Count == 0)
        {
            Debug.LogError("ConstValueTable未导入asset");
        }
        return constValueTable;
    }

    public IList<ConstValue> GetAllReadonlyData()
    {
        if (constValueTable == null || constValueTable.Count == 0)
        {
            Debug.LogError("ConstValueTable未导入asset");
        }

        ReadOnlyCollection<ConstValue> readOnlyConstValue = new ReadOnlyCollection<ConstValue>(constValueTable);

        return readOnlyConstValue ;
    }

    public ConstValue GetData(int index)
    {
        if (constValueTable == null || constValueTable.Count == 0)
        {
            Debug.LogError("ConstValueTable未导入asset");
        }
        if (constValueDic.Count == 0)
        {
            ReadOnlyCollection<ConstValue> readOnlyConstValue = new ReadOnlyCollection<ConstValue>(constValueTable);
            foreach (ConstValue value in readOnlyConstValue)
            {
                if (constValueDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //constValueDic.Add(value.id, value.Clone());
                constValueDic.Add(value.id, value);
            }
        }
        if (constValueDic.ContainsKey(index))
        {
            return constValueDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


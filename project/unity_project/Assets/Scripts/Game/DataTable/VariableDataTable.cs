using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class VariableDataTable : ScriptableObject
{
    public List<VariableData> variableDataTable = new List<VariableData>();
    public Dictionary<int, VariableData> variableDataDic = new Dictionary<int, VariableData>();

    public void SetDatas(object[] obj)
    {
        variableDataTable.Clear();
        foreach (object o in obj)
        {
            variableDataTable.Add(o as VariableData);
        }
    }

    public List<VariableData> GetAllData()
    {
        if (variableDataTable == null || variableDataTable.Count == 0)
        {
            Debug.LogError("VariableDataTable未导入asset");
        }
        return variableDataTable;
    }

    public IList<VariableData> GetAllReadonlyData()
    {
        if (variableDataTable == null || variableDataTable.Count == 0)
        {
            Debug.LogError("VariableDataTable未导入asset");
        }

        ReadOnlyCollection<VariableData> readOnlyVariableData = new ReadOnlyCollection<VariableData>(variableDataTable);

        return readOnlyVariableData ;
    }

    public VariableData GetData(int index)
    {
        if (variableDataTable == null || variableDataTable.Count == 0)
        {
            Debug.LogError("VariableDataTable未导入asset");
        }
        if (variableDataDic.Count == 0)
        {
            ReadOnlyCollection<VariableData> readOnlyVariableData = new ReadOnlyCollection<VariableData>(variableDataTable);
            foreach (VariableData value in readOnlyVariableData)
            {
                if (variableDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //variableDataDic.Add(value.id, value.Clone());
                variableDataDic.Add(value.id, value);
            }
        }
        if (variableDataDic.ContainsKey(index))
        {
            return variableDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


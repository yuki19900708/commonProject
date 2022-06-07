using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TextDataTable : ScriptableObject
{
    public List<TextData> textDataTable = new List<TextData>();
    public Dictionary<int, TextData> textDataDic = new Dictionary<int, TextData>();

    public void SetDatas(object[] obj)
    {
        textDataTable.Clear();
        foreach (object o in obj)
        {
            textDataTable.Add(o as TextData);
        }
    }

    public List<TextData> GetAllData()
    {
        if (textDataTable == null || textDataTable.Count == 0)
        {
            Debug.LogError("TextDataTable未导入asset");
        }
        return textDataTable;
    }

    public IList<TextData> GetAllReadonlyData()
    {
        if (textDataTable == null || textDataTable.Count == 0)
        {
            Debug.LogError("TextDataTable未导入asset");
        }

        ReadOnlyCollection<TextData> readOnlyTextData = new ReadOnlyCollection<TextData>(textDataTable);

        return readOnlyTextData ;
    }

    public TextData GetData(int index)
    {
        if (textDataTable == null || textDataTable.Count == 0)
        {
            Debug.LogError("TextDataTable未导入asset");
        }
        if (textDataDic.Count == 0)
        {
            ReadOnlyCollection<TextData> readOnlyTextData = new ReadOnlyCollection<TextData>(textDataTable);
            foreach (TextData value in readOnlyTextData)
            {
                if (textDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //textDataDic.Add(value.id, value.Clone());
                textDataDic.Add(value.id, value);
            }
        }
        if (textDataDic.ContainsKey(index))
        {
            return textDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


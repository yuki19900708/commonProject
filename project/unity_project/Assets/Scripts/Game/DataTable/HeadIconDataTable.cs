using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class HeadIconDataTable : ScriptableObject
{
    public List<HeadIconData> headIconDataTable = new List<HeadIconData>();
    public Dictionary<int, HeadIconData> headIconDataDic = new Dictionary<int, HeadIconData>();

    public void SetDatas(object[] obj)
    {
        headIconDataTable.Clear();
        foreach (object o in obj)
        {
            headIconDataTable.Add(o as HeadIconData);
        }
    }

    public List<HeadIconData> GetAllData()
    {
        if (headIconDataTable == null || headIconDataTable.Count == 0)
        {
            Debug.LogError("HeadIconDataTable未导入asset");
        }
        return headIconDataTable;
    }

    public IList<HeadIconData> GetAllReadonlyData()
    {
        if (headIconDataTable == null || headIconDataTable.Count == 0)
        {
            Debug.LogError("HeadIconDataTable未导入asset");
        }

        ReadOnlyCollection<HeadIconData> readOnlyHeadIconData = new ReadOnlyCollection<HeadIconData>(headIconDataTable);

        return readOnlyHeadIconData ;
    }

    public HeadIconData GetData(int index)
    {
        if (headIconDataTable == null || headIconDataTable.Count == 0)
        {
            Debug.LogError("HeadIconDataTable未导入asset");
        }
        if (headIconDataDic.Count == 0)
        {
            ReadOnlyCollection<HeadIconData> readOnlyHeadIconData = new ReadOnlyCollection<HeadIconData>(headIconDataTable);
            foreach (HeadIconData value in readOnlyHeadIconData)
            {
                if (headIconDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //headIconDataDic.Add(value.id, value.Clone());
                headIconDataDic.Add(value.id, value);
            }
        }
        if (headIconDataDic.ContainsKey(index))
        {
            return headIconDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


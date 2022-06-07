using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class HeadBoxDataTable : ScriptableObject
{
    public List<HeadBoxData> headBoxDataTable = new List<HeadBoxData>();
    public Dictionary<int, HeadBoxData> headBoxDataDic = new Dictionary<int, HeadBoxData>();

    public void SetDatas(object[] obj)
    {
        headBoxDataTable.Clear();
        foreach (object o in obj)
        {
            headBoxDataTable.Add(o as HeadBoxData);
        }
    }

    public List<HeadBoxData> GetAllData()
    {
        if (headBoxDataTable == null || headBoxDataTable.Count == 0)
        {
            Debug.LogError("HeadBoxDataTable未导入asset");
        }
        return headBoxDataTable;
    }

    public IList<HeadBoxData> GetAllReadonlyData()
    {
        if (headBoxDataTable == null || headBoxDataTable.Count == 0)
        {
            Debug.LogError("HeadBoxDataTable未导入asset");
        }

        ReadOnlyCollection<HeadBoxData> readOnlyHeadBoxData = new ReadOnlyCollection<HeadBoxData>(headBoxDataTable);

        return readOnlyHeadBoxData ;
    }

    public HeadBoxData GetData(int index)
    {
        if (headBoxDataTable == null || headBoxDataTable.Count == 0)
        {
            Debug.LogError("HeadBoxDataTable未导入asset");
        }
        if (headBoxDataDic.Count == 0)
        {
            ReadOnlyCollection<HeadBoxData> readOnlyHeadBoxData = new ReadOnlyCollection<HeadBoxData>(headBoxDataTable);
            foreach (HeadBoxData value in readOnlyHeadBoxData)
            {
                if (headBoxDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //headBoxDataDic.Add(value.id, value.Clone());
                headBoxDataDic.Add(value.id, value);
            }
        }
        if (headBoxDataDic.ContainsKey(index))
        {
            return headBoxDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


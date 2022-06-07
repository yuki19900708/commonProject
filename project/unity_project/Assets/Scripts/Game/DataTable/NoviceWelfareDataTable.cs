using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class NoviceWelfareDataTable : ScriptableObject
{
    public List<NoviceWelfareData> noviceWelfareDataTable = new List<NoviceWelfareData>();
    public Dictionary<int, NoviceWelfareData> noviceWelfareDataDic = new Dictionary<int, NoviceWelfareData>();

    public void SetDatas(object[] obj)
    {
        noviceWelfareDataTable.Clear();
        foreach (object o in obj)
        {
            noviceWelfareDataTable.Add(o as NoviceWelfareData);
        }
    }

    public List<NoviceWelfareData> GetAllData()
    {
        if (noviceWelfareDataTable == null || noviceWelfareDataTable.Count == 0)
        {
            Debug.LogError("NoviceWelfareDataTable未导入asset");
        }
        return noviceWelfareDataTable;
    }

    public IList<NoviceWelfareData> GetAllReadonlyData()
    {
        if (noviceWelfareDataTable == null || noviceWelfareDataTable.Count == 0)
        {
            Debug.LogError("NoviceWelfareDataTable未导入asset");
        }

        ReadOnlyCollection<NoviceWelfareData> readOnlyNoviceWelfareData = new ReadOnlyCollection<NoviceWelfareData>(noviceWelfareDataTable);

        return readOnlyNoviceWelfareData ;
    }

    public NoviceWelfareData GetData(int index)
    {
        if (noviceWelfareDataTable == null || noviceWelfareDataTable.Count == 0)
        {
            Debug.LogError("NoviceWelfareDataTable未导入asset");
        }
        if (noviceWelfareDataDic.Count == 0)
        {
            ReadOnlyCollection<NoviceWelfareData> readOnlyNoviceWelfareData = new ReadOnlyCollection<NoviceWelfareData>(noviceWelfareDataTable);
            foreach (NoviceWelfareData value in readOnlyNoviceWelfareData)
            {
                if (noviceWelfareDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //noviceWelfareDataDic.Add(value.id, value.Clone());
                noviceWelfareDataDic.Add(value.id, value);
            }
        }
        if (noviceWelfareDataDic.ContainsKey(index))
        {
            return noviceWelfareDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


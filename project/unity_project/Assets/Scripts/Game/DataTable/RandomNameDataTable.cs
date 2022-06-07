using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class RandomNameDataTable : ScriptableObject
{
    public List<RandomNameData> randomNameDataTable = new List<RandomNameData>();
    public Dictionary<int, RandomNameData> randomNameDataDic = new Dictionary<int, RandomNameData>();

    public void SetDatas(object[] obj)
    {
        randomNameDataTable.Clear();
        foreach (object o in obj)
        {
            randomNameDataTable.Add(o as RandomNameData);
        }
    }

    public List<RandomNameData> GetAllData()
    {
        if (randomNameDataTable == null || randomNameDataTable.Count == 0)
        {
            Debug.LogError("RandomNameDataTable未导入asset");
        }
        return randomNameDataTable;
    }

    public IList<RandomNameData> GetAllReadonlyData()
    {
        if (randomNameDataTable == null || randomNameDataTable.Count == 0)
        {
            Debug.LogError("RandomNameDataTable未导入asset");
        }

        ReadOnlyCollection<RandomNameData> readOnlyRandomNameData = new ReadOnlyCollection<RandomNameData>(randomNameDataTable);

        return readOnlyRandomNameData ;
    }

    public RandomNameData GetData(int index)
    {
        if (randomNameDataTable == null || randomNameDataTable.Count == 0)
        {
            Debug.LogError("RandomNameDataTable未导入asset");
        }
        if (randomNameDataDic.Count == 0)
        {
            ReadOnlyCollection<RandomNameData> readOnlyRandomNameData = new ReadOnlyCollection<RandomNameData>(randomNameDataTable);
            foreach (RandomNameData value in readOnlyRandomNameData)
            {
                if (randomNameDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //randomNameDataDic.Add(value.id, value.Clone());
                randomNameDataDic.Add(value.id, value);
            }
        }
        if (randomNameDataDic.ContainsKey(index))
        {
            return randomNameDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


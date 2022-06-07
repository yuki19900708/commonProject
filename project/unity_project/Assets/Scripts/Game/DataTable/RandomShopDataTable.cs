using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class RandomShopDataTable : ScriptableObject
{
    public List<RandomShopData> randomShopDataTable = new List<RandomShopData>();
    public Dictionary<int, RandomShopData> randomShopDataDic = new Dictionary<int, RandomShopData>();

    public void SetDatas(object[] obj)
    {
        randomShopDataTable.Clear();
        foreach (object o in obj)
        {
            randomShopDataTable.Add(o as RandomShopData);
        }
    }

    public List<RandomShopData> GetAllData()
    {
        if (randomShopDataTable == null || randomShopDataTable.Count == 0)
        {
            Debug.LogError("RandomShopDataTable未导入asset");
        }
        return randomShopDataTable;
    }

    public IList<RandomShopData> GetAllReadonlyData()
    {
        if (randomShopDataTable == null || randomShopDataTable.Count == 0)
        {
            Debug.LogError("RandomShopDataTable未导入asset");
        }

        ReadOnlyCollection<RandomShopData> readOnlyRandomShopData = new ReadOnlyCollection<RandomShopData>(randomShopDataTable);

        return readOnlyRandomShopData ;
    }

    public RandomShopData GetData(int index)
    {
        if (randomShopDataTable == null || randomShopDataTable.Count == 0)
        {
            Debug.LogError("RandomShopDataTable未导入asset");
        }
        if (randomShopDataDic.Count == 0)
        {
            ReadOnlyCollection<RandomShopData> readOnlyRandomShopData = new ReadOnlyCollection<RandomShopData>(randomShopDataTable);
            foreach (RandomShopData value in readOnlyRandomShopData)
            {
                if (randomShopDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //randomShopDataDic.Add(value.id, value.Clone());
                randomShopDataDic.Add(value.id, value);
            }
        }
        if (randomShopDataDic.ContainsKey(index))
        {
            return randomShopDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


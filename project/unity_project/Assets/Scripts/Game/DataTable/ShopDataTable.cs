using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ShopDataTable : ScriptableObject
{
    public List<ShopData> shopDataTable = new List<ShopData>();
    public Dictionary<int, ShopData> shopDataDic = new Dictionary<int, ShopData>();

    public void SetDatas(object[] obj)
    {
        shopDataTable.Clear();
        foreach (object o in obj)
        {
            shopDataTable.Add(o as ShopData);
        }
    }

    public List<ShopData> GetAllData()
    {
        if (shopDataTable == null || shopDataTable.Count == 0)
        {
            Debug.LogError("ShopDataTable未导入asset");
        }
        return shopDataTable;
    }

    public IList<ShopData> GetAllReadonlyData()
    {
        if (shopDataTable == null || shopDataTable.Count == 0)
        {
            Debug.LogError("ShopDataTable未导入asset");
        }

        ReadOnlyCollection<ShopData> readOnlyShopData = new ReadOnlyCollection<ShopData>(shopDataTable);

        return readOnlyShopData ;
    }

    public ShopData GetData(int index)
    {
        if (shopDataTable == null || shopDataTable.Count == 0)
        {
            Debug.LogError("ShopDataTable未导入asset");
        }
        if (shopDataDic.Count == 0)
        {
            ReadOnlyCollection<ShopData> readOnlyShopData = new ReadOnlyCollection<ShopData>(shopDataTable);
            foreach (ShopData value in readOnlyShopData)
            {
                if (shopDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //shopDataDic.Add(value.id, value.Clone());
                shopDataDic.Add(value.id, value);
            }
        }
        if (shopDataDic.ContainsKey(index))
        {
            return shopDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ProductIdDataTable : ScriptableObject
{
    public List<ProductIdData> productIdDataTable = new List<ProductIdData>();
    public Dictionary<int, ProductIdData> productIdDataDic = new Dictionary<int, ProductIdData>();

    public void SetDatas(object[] obj)
    {
        productIdDataTable.Clear();
        foreach (object o in obj)
        {
            productIdDataTable.Add(o as ProductIdData);
        }
    }

    public List<ProductIdData> GetAllData()
    {
        if (productIdDataTable == null || productIdDataTable.Count == 0)
        {
            Debug.LogError("ProductIdDataTable未导入asset");
        }
        return productIdDataTable;
    }

    public IList<ProductIdData> GetAllReadonlyData()
    {
        if (productIdDataTable == null || productIdDataTable.Count == 0)
        {
            Debug.LogError("ProductIdDataTable未导入asset");
        }

        ReadOnlyCollection<ProductIdData> readOnlyProductIdData = new ReadOnlyCollection<ProductIdData>(productIdDataTable);

        return readOnlyProductIdData ;
    }

    public ProductIdData GetData(int index)
    {
        if (productIdDataTable == null || productIdDataTable.Count == 0)
        {
            Debug.LogError("ProductIdDataTable未导入asset");
        }
        if (productIdDataDic.Count == 0)
        {
            ReadOnlyCollection<ProductIdData> readOnlyProductIdData = new ReadOnlyCollection<ProductIdData>(productIdDataTable);
            foreach (ProductIdData value in readOnlyProductIdData)
            {
                if (productIdDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //productIdDataDic.Add(value.id, value.Clone());
                productIdDataDic.Add(value.id, value);
            }
        }
        if (productIdDataDic.ContainsKey(index))
        {
            return productIdDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


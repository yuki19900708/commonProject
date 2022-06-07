using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class PurchaseDataTable : ScriptableObject
{
    public List<PurchaseData> purchaseDataTable = new List<PurchaseData>();
    public Dictionary<int, PurchaseData> purchaseDataDic = new Dictionary<int, PurchaseData>();

    public void SetDatas(object[] obj)
    {
        purchaseDataTable.Clear();
        foreach (object o in obj)
        {
            purchaseDataTable.Add(o as PurchaseData);
        }
    }

    public List<PurchaseData> GetAllData()
    {
        if (purchaseDataTable == null || purchaseDataTable.Count == 0)
        {
            Debug.LogError("PurchaseDataTable未导入asset");
        }
        return purchaseDataTable;
    }

    public IList<PurchaseData> GetAllReadonlyData()
    {
        if (purchaseDataTable == null || purchaseDataTable.Count == 0)
        {
            Debug.LogError("PurchaseDataTable未导入asset");
        }

        ReadOnlyCollection<PurchaseData> readOnlyPurchaseData = new ReadOnlyCollection<PurchaseData>(purchaseDataTable);

        return readOnlyPurchaseData ;
    }

    public PurchaseData GetData(int index)
    {
        if (purchaseDataTable == null || purchaseDataTable.Count == 0)
        {
            Debug.LogError("PurchaseDataTable未导入asset");
        }
        if (purchaseDataDic.Count == 0)
        {
            ReadOnlyCollection<PurchaseData> readOnlyPurchaseData = new ReadOnlyCollection<PurchaseData>(purchaseDataTable);
            foreach (PurchaseData value in readOnlyPurchaseData)
            {
                if (purchaseDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //purchaseDataDic.Add(value.id, value.Clone());
                purchaseDataDic.Add(value.id, value);
            }
        }
        if (purchaseDataDic.ContainsKey(index))
        {
            return purchaseDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


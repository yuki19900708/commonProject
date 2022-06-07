using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class BulletInfoDataTable : ScriptableObject
{
    public List<BulletInfoData> bulletInfoDataTable = new List<BulletInfoData>();
    public Dictionary<int, BulletInfoData> bulletInfoDataDic = new Dictionary<int, BulletInfoData>();

    public void SetDatas(object[] obj)
    {
        bulletInfoDataTable.Clear();
        foreach (object o in obj)
        {
            bulletInfoDataTable.Add(o as BulletInfoData);
        }
    }

    public List<BulletInfoData> GetAllData()
    {
        if (bulletInfoDataTable == null || bulletInfoDataTable.Count == 0)
        {
            Debug.LogError("BulletInfoDataTable未导入asset");
        }
        return bulletInfoDataTable;
    }

    public IList<BulletInfoData> GetAllReadonlyData()
    {
        if (bulletInfoDataTable == null || bulletInfoDataTable.Count == 0)
        {
            Debug.LogError("BulletInfoDataTable未导入asset");
        }

        ReadOnlyCollection<BulletInfoData> readOnlyBulletInfoData = new ReadOnlyCollection<BulletInfoData>(bulletInfoDataTable);

        return readOnlyBulletInfoData ;
    }

    public BulletInfoData GetData(int index)
    {
        if (bulletInfoDataTable == null || bulletInfoDataTable.Count == 0)
        {
            Debug.LogError("BulletInfoDataTable未导入asset");
        }
        if (bulletInfoDataDic.Count == 0)
        {
            ReadOnlyCollection<BulletInfoData> readOnlyBulletInfoData = new ReadOnlyCollection<BulletInfoData>(bulletInfoDataTable);
            foreach (BulletInfoData value in readOnlyBulletInfoData)
            {
                if (bulletInfoDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //bulletInfoDataDic.Add(value.id, value.Clone());
                bulletInfoDataDic.Add(value.id, value);
            }
        }
        if (bulletInfoDataDic.ContainsKey(index))
        {
            return bulletInfoDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class EnemyDataTable : ScriptableObject
{
    public List<EnemyData> enemyDataTable = new List<EnemyData>();
    public Dictionary<int, EnemyData> enemyDataDic = new Dictionary<int, EnemyData>();

    public void SetDatas(object[] obj)
    {
        enemyDataTable.Clear();
        foreach (object o in obj)
        {
            enemyDataTable.Add(o as EnemyData);
        }
    }

    public List<EnemyData> GetAllData()
    {
        if (enemyDataTable == null || enemyDataTable.Count == 0)
        {
            Debug.LogError("EnemyDataTable未导入asset");
        }
        return enemyDataTable;
    }

    public IList<EnemyData> GetAllReadonlyData()
    {
        if (enemyDataTable == null || enemyDataTable.Count == 0)
        {
            Debug.LogError("EnemyDataTable未导入asset");
        }

        ReadOnlyCollection<EnemyData> readOnlyEnemyData = new ReadOnlyCollection<EnemyData>(enemyDataTable);

        return readOnlyEnemyData ;
    }

    public EnemyData GetData(int index)
    {
        if (enemyDataTable == null || enemyDataTable.Count == 0)
        {
            Debug.LogError("EnemyDataTable未导入asset");
        }
        if (enemyDataDic.Count == 0)
        {
            ReadOnlyCollection<EnemyData> readOnlyEnemyData = new ReadOnlyCollection<EnemyData>(enemyDataTable);
            foreach (EnemyData value in readOnlyEnemyData)
            {
                if (enemyDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //enemyDataDic.Add(value.id, value.Clone());
                enemyDataDic.Add(value.id, value);
            }
        }
        if (enemyDataDic.ContainsKey(index))
        {
            return enemyDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


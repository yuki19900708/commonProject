using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class MonsterAttributesDataTable : ScriptableObject
{
    public List<MonsterAttributesData> monsterAttributesDataTable = new List<MonsterAttributesData>();
    public Dictionary<int, MonsterAttributesData> monsterAttributesDataDic = new Dictionary<int, MonsterAttributesData>();

    public void SetDatas(object[] obj)
    {
        monsterAttributesDataTable.Clear();
        foreach (object o in obj)
        {
            monsterAttributesDataTable.Add(o as MonsterAttributesData);
        }
    }

    public List<MonsterAttributesData> GetAllData()
    {
        if (monsterAttributesDataTable == null || monsterAttributesDataTable.Count == 0)
        {
            Debug.LogError("MonsterAttributesDataTable未导入asset");
        }
        return monsterAttributesDataTable;
    }

    public IList<MonsterAttributesData> GetAllReadonlyData()
    {
        if (monsterAttributesDataTable == null || monsterAttributesDataTable.Count == 0)
        {
            Debug.LogError("MonsterAttributesDataTable未导入asset");
        }

        ReadOnlyCollection<MonsterAttributesData> readOnlyMonsterAttributesData = new ReadOnlyCollection<MonsterAttributesData>(monsterAttributesDataTable);

        return readOnlyMonsterAttributesData ;
    }

    public MonsterAttributesData GetData(int index)
    {
        if (monsterAttributesDataTable == null || monsterAttributesDataTable.Count == 0)
        {
            Debug.LogError("MonsterAttributesDataTable未导入asset");
        }
        if (monsterAttributesDataDic.Count == 0)
        {
            ReadOnlyCollection<MonsterAttributesData> readOnlyMonsterAttributesData = new ReadOnlyCollection<MonsterAttributesData>(monsterAttributesDataTable);
            foreach (MonsterAttributesData value in readOnlyMonsterAttributesData)
            {
                if (monsterAttributesDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //monsterAttributesDataDic.Add(value.id, value.Clone());
                monsterAttributesDataDic.Add(value.id, value);
            }
        }
        if (monsterAttributesDataDic.ContainsKey(index))
        {
            return monsterAttributesDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


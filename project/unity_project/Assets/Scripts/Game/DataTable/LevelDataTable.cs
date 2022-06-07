using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class LevelDataTable : ScriptableObject
{
    public List<LevelData> levelDataTable = new List<LevelData>();
    public Dictionary<int, LevelData> levelDataDic = new Dictionary<int, LevelData>();

    public void SetDatas(object[] obj)
    {
        levelDataTable.Clear();
        foreach (object o in obj)
        {
            levelDataTable.Add(o as LevelData);
        }
    }

    public List<LevelData> GetAllData()
    {
        if (levelDataTable == null || levelDataTable.Count == 0)
        {
            Debug.LogError("LevelDataTable未导入asset");
        }
        return levelDataTable;
    }

    public IList<LevelData> GetAllReadonlyData()
    {
        if (levelDataTable == null || levelDataTable.Count == 0)
        {
            Debug.LogError("LevelDataTable未导入asset");
        }

        ReadOnlyCollection<LevelData> readOnlyLevelData = new ReadOnlyCollection<LevelData>(levelDataTable);

        return readOnlyLevelData ;
    }

    public LevelData GetData(int index)
    {
        if (levelDataTable == null || levelDataTable.Count == 0)
        {
            Debug.LogError("LevelDataTable未导入asset");
        }
        if (levelDataDic.Count == 0)
        {
            ReadOnlyCollection<LevelData> readOnlyLevelData = new ReadOnlyCollection<LevelData>(levelDataTable);
            foreach (LevelData value in readOnlyLevelData)
            {
                if (levelDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //levelDataDic.Add(value.id, value.Clone());
                levelDataDic.Add(value.id, value);
            }
        }
        if (levelDataDic.ContainsKey(index))
        {
            return levelDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


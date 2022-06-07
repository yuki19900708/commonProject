using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TutorialTableDataTable : ScriptableObject
{
    public List<TutorialTableData> tutorialTableDataTable = new List<TutorialTableData>();
    public Dictionary<int, TutorialTableData> tutorialTableDataDic = new Dictionary<int, TutorialTableData>();

    public void SetDatas(object[] obj)
    {
        tutorialTableDataTable.Clear();
        foreach (object o in obj)
        {
            tutorialTableDataTable.Add(o as TutorialTableData);
        }
    }

    public List<TutorialTableData> GetAllData()
    {
        if (tutorialTableDataTable == null || tutorialTableDataTable.Count == 0)
        {
            Debug.LogError("TutorialTableDataTable未导入asset");
        }
        return tutorialTableDataTable;
    }

    public IList<TutorialTableData> GetAllReadonlyData()
    {
        if (tutorialTableDataTable == null || tutorialTableDataTable.Count == 0)
        {
            Debug.LogError("TutorialTableDataTable未导入asset");
        }

        ReadOnlyCollection<TutorialTableData> readOnlyTutorialTableData = new ReadOnlyCollection<TutorialTableData>(tutorialTableDataTable);

        return readOnlyTutorialTableData ;
    }

    public TutorialTableData GetData(int index)
    {
        if (tutorialTableDataTable == null || tutorialTableDataTable.Count == 0)
        {
            Debug.LogError("TutorialTableDataTable未导入asset");
        }
        if (tutorialTableDataDic.Count == 0)
        {
            ReadOnlyCollection<TutorialTableData> readOnlyTutorialTableData = new ReadOnlyCollection<TutorialTableData>(tutorialTableDataTable);
            foreach (TutorialTableData value in readOnlyTutorialTableData)
            {
                if (tutorialTableDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //tutorialTableDataDic.Add(value.id, value.Clone());
                tutorialTableDataDic.Add(value.id, value);
            }
        }
        if (tutorialTableDataDic.ContainsKey(index))
        {
            return tutorialTableDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


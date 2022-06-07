using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class GuideDataTable : ScriptableObject
{
    public List<GuideData> guideDataTable = new List<GuideData>();
    public Dictionary<int, GuideData> guideDataDic = new Dictionary<int, GuideData>();

    public void SetDatas(object[] obj)
    {
        guideDataTable.Clear();
        foreach (object o in obj)
        {
            guideDataTable.Add(o as GuideData);
        }
    }

    public IList<GuideData> GetAllData()
    {
        if (guideDataTable == null || guideDataTable.Count == 0)
        {
            Debug.LogError("GuideDataTable未导入asset");
        }

        ReadOnlyCollection<GuideData> readOnlyGuideData = new ReadOnlyCollection<GuideData>(guideDataTable);

        return readOnlyGuideData ;
    }

    public GuideData GetData(int index)
    {
return null;
    }
}


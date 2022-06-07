using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class CountryDataTable : ScriptableObject
{
    public List<CountryData> countryDataTable = new List<CountryData>();
    public Dictionary<int, CountryData> countryDataDic = new Dictionary<int, CountryData>();

    public void SetDatas(object[] obj)
    {
        countryDataTable.Clear();
        foreach (object o in obj)
        {
            countryDataTable.Add(o as CountryData);
        }
    }

    public List<CountryData> GetAllData()
    {
        if (countryDataTable == null || countryDataTable.Count == 0)
        {
            Debug.LogError("CountryDataTable未导入asset");
        }
        return countryDataTable;
    }

    public IList<CountryData> GetAllReadonlyData()
    {
        if (countryDataTable == null || countryDataTable.Count == 0)
        {
            Debug.LogError("CountryDataTable未导入asset");
        }

        ReadOnlyCollection<CountryData> readOnlyCountryData = new ReadOnlyCollection<CountryData>(countryDataTable);

        return readOnlyCountryData ;
    }

    public CountryData GetData(int index)
    {
        if (countryDataTable == null || countryDataTable.Count == 0)
        {
            Debug.LogError("CountryDataTable未导入asset");
        }
        if (countryDataDic.Count == 0)
        {
            ReadOnlyCollection<CountryData> readOnlyCountryData = new ReadOnlyCollection<CountryData>(countryDataTable);
            foreach (CountryData value in readOnlyCountryData)
            {
                if (countryDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //countryDataDic.Add(value.id, value.Clone());
                countryDataDic.Add(value.id, value);
            }
        }
        if (countryDataDic.ContainsKey(index))
        {
            return countryDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


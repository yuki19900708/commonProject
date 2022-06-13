using System;
using System.Collections.Generic;
using UnityEngine;

public enum TableDataName
{
       VegetationDataTable,
}

public partial class TableDataMgr
{
    #region
    public static event Func<string, VegetationDataTable> Event_LoadVegetationDataTable;

    private static VegetationDataTable vegetationData ;

    public static VegetationDataTable VegetationDataTable
    {
       get
       {
           if (vegetationData == null)
           {
               if(Event_LoadVegetationDataTable != null)
               {
                  vegetationData = Event_LoadVegetationDataTable("VegetationDataTable");
               }
           }
           if (vegetationData == null)
           {
              Debug.LogError("vegetationData不存在");
              return null;
           }
            return vegetationData ;
        }
        set
        {
            vegetationData  = value;
        }
    }
    
    #endregion
    private static Dictionary<TableDataName, ScriptableObject> dataDic = new Dictionary<TableDataName, ScriptableObject>();

    public static Dictionary<TableDataName, ScriptableObject> DataDic
    {
        get
        {
            return dataDic;
        }
        set
        {
            dataDic = value;
        }
    }

    public static List<VegetationData> GetAllVegetationDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<VegetationData>(VegetationDataTable.GetAllReadonlyData());
        }
        else
        {
           return VegetationDataTable.GetAllData();
        }
    }

    public static VegetationData GetSingleVegetationData(int id)
    {
        return VegetationDataTable.GetData(id);
    }
}

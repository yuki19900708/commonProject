using System;
using System.Collections.Generic;
using UnityEngine;

public static class TableDataEventMgr
{
    public static void BindAllEvent()
    {
       TableDataMgr.Event_LoadVegetationDataTable += Load<VegetationDataTable>;
    }

	public static T Load<T>(string assetName) where T : UnityEngine.Object
	{
		return Resources.Load<T> ("DataAsset/" + assetName);
	}
}

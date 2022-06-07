using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MapObejctSourcesType
{
    /// <summary>默认状态</summary>
    None,
    /// <summary>新生成的状态  刚从池子中拿出来</summary>
    NewGeneration,
    /// <summary>来自于商店购买</summary>
    ShopBuy,
}

public partial class MapObject
{
    public MapObejctSourcesType mapObjectSourcesType = MapObejctSourcesType.None;
    public void InitEffect()
    {
        bool result = false;

        if ((BasicData.appearParticle & 1) == 1)
        {
            //if (GlobalVariable.GameState == GameState.MainSceneMode && 
            //    mapObjectSourcesType == MapObejctSourcesType.NewGeneration)
            //{
            //    result = true;
            //}
        }

        if ((BasicData.appearParticle & 2) == 2)
        {
            //if (GlobalVariable.GameState == GameState.LevelModel &&
            //    mapObjectSourcesType == MapObejctSourcesType.NewGeneration)
            //{
            //    result = true;
            //}
        }

        if ((BasicData.appearParticle & 4) == 4)
        {
            //if (GlobalVariable.GameState == GameState.LevelModel &&
            //    mapObjectSourcesType == MapObejctSourcesType.ShopBuy)
            //{
            //    result = true;
            //}
        }

        if ((BasicData.appearParticle & 8) == 8)
        {
            //if (GlobalVariable.GameState == GameState.MainSceneMode &&
            //    mapObjectSourcesType == MapObejctSourcesType.ShopBuy)
            //{
            //    result = true;
            //}
        }

        if (result && MapMgr.isMapInitialzeLoading == false)
        {
            //VFXMgr.PlayItemShowEffect(this.transform);
        }
    }
}

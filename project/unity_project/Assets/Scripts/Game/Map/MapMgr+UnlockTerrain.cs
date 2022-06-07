using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameProto;
using Universal.TileMapping;
using System;
using DG.Tweening;

public partial class MapMgr
{
    const string High_Area_Seal_Lock = "sealLock{0}_Tile";
    public static Action<int> Event_UnLockHighArea;
    public static Action<int> Event_BiologicalPowerChange;
    /// <summary>地图解锁缓存</summary>
    private Dictionary<int, MapUnlcokCache> mapUnlockCacheDict = new Dictionary<int, MapUnlcokCache>();
    /// <summary>
    /// 生物之力
    /// </summary>
    public static int biologicalPower;
    List<int> allLockIndexList = new List<int>();
    /// <summary>
    /// 生成缠绕森林的粒子后会存储其计时器结束的回调  用于在清理的时候进行清理使用
    /// </summary>
    Dictionary<MapGrid, DelayAction> sealLockMapGridEffectTimerList = new Dictionary<MapGrid, DelayAction>();
    List<FogData> somkingList = new List<FogData>();
    List<MapGrid> allLockMapGridList = new List<MapGrid>();

    public static void ShowDragonPowerTip(int total)
    {
        UIMgr.ShowUI(UIDef.DragonPowerTipUI, total);
    }

    public static void ShowUnlockHighEndArea(DragonPowerSlider item)
    {
        if (PlayerProfile.LoadFristClickHighArea() == 0)
        {
            //TDGAModel.UserDefineEvent(SelfEvent.FRIST_CLICK_HIGH_AREA_LEVEL, PlayerModel.Data.Level.ToString());
            PlayerProfile.SaveFristClickHighArea(1);
        }
        switch (item.data.id)
        {
            case 501:
                TDGAModel.UserDefineEvent(SelfEvent.BASECAMP_DAILY_CHEST_BUTTON_CLICK, "1");
                break;
            case 502:
                TDGAModel.UserDefineEvent(SelfEvent.BASECAMP_DAILY_CHEST_BUTTON_CLICK, "2");
                break;
            case 503:
                TDGAModel.UserDefineEvent(SelfEvent.BASECAMP_DAILY_CHEST_BUTTON_CLICK, "3");
                break;
        }
        //High_EndAreaUICtrl ctrl = (High_EndAreaUICtrl)UIMgr.ShowUI(UIDef.High_EndAreaUI);
        //ctrl.InitData(item);
    }

    /// <summary>
    /// 根据传入的参数直接解锁相应的高端区
    /// </summary>
    public static void UnlockHighEndArea(int areaId)
    {
        if (Instance.mapUnlockCacheDict.ContainsKey(areaId))
        {
            Instance.mapUnlockCacheDict[areaId].slider.RunUnlock();
        }
        else
        {
            Debug.LogError("此高端区已经解锁 或是 不存在这个ID的高端区  请检查！！！" + areaId);
        }
    }

    /// <summary>
    /// 获取这个高端区是否已经解锁
    /// </summary>
    public static bool IsUnlockHighEndArea(int areaId)
    {
        if (Instance.mapUnlockCacheDict.ContainsKey(areaId))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void UnlockMapGrid(int x, int y)
    {
        //AnimMgr.PlaySeallockObjectDisappearAnimation(Instance.GetMapGridData(x, y).sealLock.transform, () =>
        //{
        //    Instance.sealLockMap.SetTileAt(x, y, null, false);

        //    if (Instance.mapGrid[x, y].deadLandData == null)
        //    {
        //        Instance.mapGrid[x, y].SetStatus(MapGridState.UnlockAndCured);
        //    }
        //    else
        //    {
        //        Instance.mapGrid[x, y].SetStatus(MapGridState.UnlockButDead);
        //    }
        //});
    }

    public void SealLockAnimation(int x, int y)
    {

    }

    #region 地图解锁数据的初始化
    private void MapUnlockInit()
    {
        somkingList = TableDataMgr.GetAllFogDatas(true);

        List<MapUnlockData> list = TableDataMgr.GetAllMapUnlockDatas(true);

        foreach (MapUnlockData data in list)
        {
            if (mapGrid[data.area[0], data.area[1]].Status != MapGridState.Locked)
            {
                for (int i = 0; i < somkingList.Count; i++)
                {
                    List<int> somkingAreaList = new List<int>(somkingList[i].area);
                    if (somkingAreaList.Contains(data.id) || somkingAreaList.Count <= 0 || somkingList[i].pos.Length <= 1)
                    {
                        somkingList.Remove(somkingList[i]);
                        i--;
                    }
                }
            }

            if (data.area == null || data.area.Length < 2)
            {
                continue;
            }
            if (mapGrid[data.area[0], data.area[1]].Status == MapGridState.Locked)
            {
                allLockIndexList.Add(data.id);
            }
        }

        foreach (MapUnlockData data in list)
        {
            if (data.area == null || data.area.Length < 2 || mapGrid[data.area[0], data.area[1]].Status != MapGridState.Locked)
            {
                continue;
            }
            MapUnlcokCache cache = new MapUnlcokCache();
            cache.data = data;
            //TODO:这里要根据服务器给过来的数据来进行判断操作  如果为true的话 需要变更已经加载出来的数据  或者是直接一开始就在原始数据上更改
            cache.isUnlock = false;
            bool showSlider = false;
            if (data.area.Length > 1 && cache.isUnlock == false)
            {
                //高端区
                if (data.type == 2)
                {
                    ScriptableTile tile = ResMgr.Load<ScriptableTile>(string.Format(High_Area_Seal_Lock, data.id));
                    if (tile != null)
                    {
                        sealLock_HighAreaTile = tile;
                    }
                    else
                    {
                        sealLock_HighAreaTile = sealLockTile;
                    }
                    showSlider = true;
                    effectiveBlocksCount -= cache.data.area.Length / 2;
                    for (int i = 0; i < data.area.Length; i++)
                    {
                        if (i % 2 == 0)
                        {
                            Point point = new Point(data.area[i], data.area[i + 1]);
                            sealLockMap.SetTileAndUpdateNeighbours(point, sealLock_HighAreaTile);
                        }
                    }
                }
                else
                {
                    //目前算法改为：对于普通锁定而言 必然显示离解锁最近的3个
                    for (int i = 0; i < 3; i++)
                    {
                        if (i < allLockIndexList.Count)
                        {
                            if (data.id == allLockIndexList[i])
                            {
                                showSlider = true;
                            }
                        }
                    }
                }

                if (showSlider)
                {
                    //cache.slider = dragonPowerPool.GetInstance();
                    int count = cache.data.area.Length / 4;
                    Point pos = new Point(cache.data.area[count], cache.data.area[count + 1]);
                    if (cache.data.barPos.Length > 1)
                    {
                        pos = new Point(cache.data.barPos[0], cache.data.barPos[1]);
                    }
                    cache.slider.InitData(pos, data);
                    cache.slider.Event_MapUnlcok = EventMapUnlock;
                }
                if (mapUnlockCacheDict.ContainsKey(data.id) == false)
                {
                    mapUnlockCacheDict.Add(data.id, cache);
                }
            }
        }

        for (int i = 0; i < somkingList.Count; i++)
        {
            //Point point = new Point(somkingList[i].pos[0], somkingList[i].pos[1]);
            //VFXMgr.PlaySealLockSmokeVFX(point, GetWorldPosByPoint(point));
        }

        SealLockMapGridAddEffectInit();
    }

    /// <summary>
    /// 解锁一片地之后 获取新的解锁位置
    /// </summary>
    /// <param name="item"></param>
    private void EventMapUnlock(DragonPowerSlider item)
    {
        mapUnlockCacheDict[item.data.id].isUnlock = true;
        mapUnlockCacheDict[item.data.id].slider.ClearPrefab();
        //dragonPowerPool.RecycleInstance(mapUnlockCacheDict[item.data.id].slider);
        mapUnlockCacheDict[item.data.id].slider = null;

        allLockIndexList.Remove(item.data.id);
        mapUnlockCacheDict.Remove(item.data.id);

        for (int i = 0; i < somkingList.Count; i++)
        {
            List<int> somkingAreaList = new List<int>(somkingList[i].area);
            if (somkingAreaList.Contains(item.data.id))
            {
                //VFXMgr.RemoveSealLockSmokeVFX(new Point(somkingList[i].pos[0], somkingList[i].pos[1]));
            }
        }

        foreach (MapUnlcokCache cache in mapUnlockCacheDict.Values)
        {
            if (cache.data.area.Length > 1 && cache.isUnlock == false && cache.slider == null)
            {
                //cache.slider = dragonPowerPool.GetInstance();
                int count = cache.data.area.Length / 4;
                Point pos = new Point(cache.data.area[count], cache.data.area[count + 1]);
                if (cache.data.barPos.Length > 1)
                {
                    pos = new Point(cache.data.barPos[0], cache.data.barPos[1]);
                }
                cache.slider.InitData(pos, cache.data);
                cache.slider.Event_MapUnlcok = EventMapUnlock;
                return;
            }
        }
    }
    #endregion
    /// <summary>
    /// 发送给服务器 告知进行了地块解锁
    /// </summary>
    public static void SendServerUnlockTerrain(DragonPowerSlider item)
    {
        if (item.data.type == 1)
        {
            Unlock_foggy_event foggy = new Unlock_foggy_event();
            foggy.Id = (uint)item.data.id;
            //Debug.Log("LY", "告知了服务器  进行了迷雾解锁操作 解锁ID 为 ：" + item.data.id);
            //NetMgr.CacheSend(NetAPIDef.eCTS_GAMESER_UNLOCK_FOGGY, foggy);
            TDGAModel.UserDefineEvent(SelfEvent.MAP_UNLOCK, item.data.id.ToString());
        }
        else
        {

//#if UNITY_EDITOR || UNITY_ANDROID
            Unlock_foggy_event foggy = new Unlock_foggy_event();
            foggy.Id = (uint)item.data.id;
            //Debug.Log("LY", "告知了服务器  进行了迷雾解锁操作 解锁ID 为 ：" + item.data.id);
            //NetMgr.CacheSend(NetAPIDef.eCTS_GAMESER_UNLOCK_FOGGY, foggy);
//#endif
            TDGAModel.UserDefineEvent( SelfEvent.MAP_BUY,item.data.id.ToString());
            if (item.data.id == 501)
            {
                //TDGAModel.UserDefineEvent(SelfEvent.UNLOCK_FIRST_PIECE_HIGH_AREA_LEVEL, PlayerModel.Data.Level.ToString());
            }
        }


        for (int i = 0; i < item.data.area.Length; i++)
        {
            UpdateTerrainInfo info = new UpdateTerrainInfo();
            if (i % 2 == 0)
            {
                info.point = new Point(item.data.area[i], item.data.area[i + 1]);
                if (Instance.mapGrid[info.point.x, info.point.y].deadLandData == null)
                {
                    info.state = Grid_state.UnlockAndCured;
                }
                else
                {
                    info.state = Grid_state.UnlockAndDead;
                }

                if (Instance.mapGrid[info.point.x, info.point.y].Entity != null && info.state == Grid_state.UnlockAndCured &&
                    Instance.mapGrid[info.point.x, info.point.y].Entity.BasicData.objectType == 711)
                {
                    //衍生初始化
                    Instance.mapGrid[info.point.x, info.point.y].Entity.InitializeExtraGameData();
                    //info.entity = Instance.AddMapEntityToTrophyBall(Instance.mapGrid[info.point.x, info.point.y].Entity.StaticPos,
                        //Instance.mapGrid[info.point.x, info.point.y].Entity.BasicData.id);
                    Instance.RemoveMapObjectEntity(Instance.mapGrid[info.point.x, info.point.y].Entity);
                }

                if (Instance.sealLockMapGridEffectTimerList.ContainsKey(Instance.mapGrid[info.point.x, info.point.y]))
                {
                    //VFXMgr.RemoveSealLockForestVFX(info.point);
                    Timer.Remove(Instance.sealLockMapGridEffectTimerList[Instance.mapGrid[info.point.x, info.point.y]]);
                    Instance.sealLockMapGridEffectTimerList.Remove(Instance.mapGrid[info.point.x, info.point.y]);
                }
                Instance.allLockMapGridList.Remove(Instance.mapGrid[info.point.x, info.point.y]);
            }
        }
    }

    /// <summary>
    /// 初始为所有锁定树林添加粒子
    /// </summary>
    private void SealLockMapGridAddEffectInit(int count = 0)
    {
        if (count == 0)
        {
            count = allLockMapGridList.Count / 100;
        }

        for (int i = 0; i < count; i++)
        {
            int index = GetIndex();
            MapGrid mGrid = allLockMapGridList[index];
            int time = UnityEngine.Random.Range(20, 60);
            //VFXMgr.PlaySealLockForestVFX(mGrid, time);
            DelayAction at = Timer.AddDelayFunc(time, a =>
            {
                SealLockMapGridAddEffectInit(1);
                //VFXMgr.RemoveSealLockForestVFX(a.point);
                sealLockMapGridEffectTimerList.Remove(a);
            }, mGrid);
            sealLockMapGridEffectTimerList.Add(mGrid, at);
        }
    }

    private static void ClearSealLockMapGridEffect()
    {
        foreach (DelayAction ac in Instance.sealLockMapGridEffectTimerList.Values)
        {
            Timer.Remove(ac);
        }
        Instance.sealLockMapGridEffectTimerList.Clear();
        Instance.allLockMapGridList.Clear();
    }

    private int GetIndex()
    {
        int index = UnityEngine.Random.Range(0, allLockMapGridList.Count);
        if (sealLockMapGridEffectTimerList.ContainsKey(allLockMapGridList[index]))
        {
            return GetIndex();
        }
        else
        {
            return index;
        }
    }
}
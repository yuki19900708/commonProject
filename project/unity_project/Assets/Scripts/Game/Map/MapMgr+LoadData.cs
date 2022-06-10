using GameProto;
using Google.Protobuf.Collections;
using SimpleJson;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using UnityEngine;
using Universal.TileMapping;
//using Universal.Tutorial;

public enum ObjectId
{
    Purifier = 70004,
}

public class MapUnlcokCache
{
    public bool isUnlock;
    public MapUnlockData data;
    //public DragonPowerSlider slider;
}

public enum MapClearType
{
    BaseCamp,
    Chapter,
    Plunder,
}

public class MapGridPurificationDataCache
{
    public List<PurificationData> purificationList = new List<PurificationData>();
}
public class PurificationData
{
    public int x;
    public int y;
    public int value;
}
public partial class MapMgr
{
    /// <summary>
    /// 大本营数据 在本地数据中  缓存的 表名
    /// </summary>
    const string BASE_CAMP_MAP_DATA_TABLE_NAME = "base_camp_map_data";
    /// <summary> 解锁并治愈的地形列表  任务统计模块的需要使用   /// </summary>
    public static List<MapObject> unlockAndCuredTerrainList = new List<MapObject>();
    /// <summary> 关卡默认地图数据加载路径/// </summary>
    const string chapterPath = "MapData/LevelData/";
    /// <summary> 大本营默认地图数据加载路径/// </summary>
    const string baseCampPath = "MapData/BaseCampData/BaseCamp";
    /// <summary> 游戏当前正在加载的路径/// </summary>
    static string currentLoadPath = "";

    /// <summary>
    /// 第一次進入遊戲的第一次load加載
    /// </summary>
    public static bool isInitialzeLoad = true;
    /// <summary>
    /// 当前地图正在进行初始化加载
    /// </summary>
    public static bool isMapInitialzeLoading = true;
    public static bool isMapClearing = false;

    /// <summary>
    /// 地图加载完成（大本营 关卡）向外分发的事件
    /// </summary>
    public static Action Event_MapInitialzeLoadFinish;

    public static Action<int, int> Event_LoadingMapContent;

    /// <summary>地图的宽</summary>
    public static int mapWidth = 35;
    /// <summary>地图的高</summary>
    public static int mapHeight = 35;

    /// <summary>
    /// 本地缓存中的大本营数据
    /// </summary>
    public static MapDataCache mapDataCache;

    /// <summary>大本营的格子已净化数值的本地数据存储</summary>
    public static MapGridPurificationDataCache mapGridPurificationDataCache;
    /// <summary>
    /// 从本地读取到缓存之后 - 再从服务器获取到服务器的数据会整合到这个类中的字典里(使用字典可以优化循环次数)在之后的生成流程中进行操作
    /// </summary>
    //public static EntityAndMonsterDataTempCache entityAndMonsterDataTempCache;

    /// <summary>
    /// 因为本地的json不支持缓存字典 ，所以保留list的类用来在需要的时候对其进行值的改变
    /// 大本营所有生物的数据本地存储
    /// </summary>
    public static EntityAndMonsterDataCache entityAndMonsterDataCache;

    /// <summary>
    /// 所有非高端区的有效地块数量
    /// </summary>
    private int effectiveBlocksCount = 0;
    /// <summary>
    /// 当前地图上所有格子的数据  
    /// </summary>
    private MapGrid[,] mapGrid;

    public MapGrid[,] MapGrids
    {
        get
        {
            return mapGrid;
        }
    }
    /// <summary>
    /// 所有非高端区的有效地块数量
    /// </summary>
    public static int EffectiveBlocksCount
    {
        get
        {
            return Instance.effectiveBlocksCount;
        }
    }

    class SimpleJsonSerializerStrategyVector2 : PocoJsonSerializerStrategy
    {

        public override object DeserializeObject(object value, Type type)
        {
            if (type == typeof(Vector2))
            {
                float x = 0, y = 0;
                int z = 0;
                string[] sr = value.ToString().Split(new char[] { '{', '}', ',' });
                for (int i = 0; i < sr.Length; i++)
                {
                    if (z == 0)
                    {
                        if (float.TryParse(sr[i].ToString(), out x))
                        {
                            z += 1;
                        }
                    }
                    else if (z == 1)
                    {
                        if (float.TryParse(sr[i].ToString(), out y))
                        {
                            z += 1;
                        }
                    }

                }
                return new Vector2(x, y);
            }
            else
            {
                return base.DeserializeObject(value, type);
            }
        }

        public override bool TrySerializeNonPrimitiveObject(object input, out object output)
        {
            if (input is Vector2)
            {
                Vector2 v2 = (Vector2)input;
                output = "{" + string.Format("{0},{1}", v2.x, v2.y) + "}";
                return true;
            }
            else
            {
                return base.TrySerializeNonPrimitiveObject(input, out output);
            }
        }
    }

    #region 大本营  敌方 以及 管卡初始数据加载

    #region 从本地初始化数据
    /// <summary>
    /// 从本地数据库初始化数据
    /// </summary>
    public static void LoadGameDataFromLocalCache()
    {
        mapDataCache = new MapDataCache();
        entityAndMonsterDataCache = new EntityAndMonsterDataCache();
        //DataTable table = LocalCache.LoadTable(BASE_CAMP_MAP_DATA_TABLE_NAME);
        //if (table.Rows.Count > 0)
        //{
        //    foreach (DataRow dr in table.Rows)
        //    {
        //        string map_data = Convert.ToString(dr["data"]);
        //        mapDataCache = SimpleJson.SimpleJson.DeserializeObject<MapDataCache>(map_data);
        //        string purification_data = Convert.ToString(dr["purification_data"]);

        //        if (string.IsNullOrEmpty(purification_data))
        //        {
        //            Debug.Log("有致命错误 -- >>找卢洋", LogFontSize.General, "red");
        //        }
        //        else
        //        {
        //            mapGridPurificationDataCache = SimpleJson.SimpleJson.DeserializeObject<MapGridPurificationDataCache>(purification_data);
        //        }

        //        string entity_data = Convert.ToString(dr["entity_data"]);

        //        if (string.IsNullOrEmpty(entity_data))
        //        {
        //            entityAndMonsterDataCache = new EntityAndMonsterDataCache();
        //        }
        //        else
        //        {
        //            entityAndMonsterDataCache = SimpleJson.SimpleJson.DeserializeObject<EntityAndMonsterDataCache>(entity_data);
        //        }
        //    }
        //}
        //else
        //{
        //    currentLoadPath = baseCampPath;

        //    TextAsset ta = Resources.Load(currentLoadPath) as TextAsset;
        //    if (ta == null)
        //    {
        //        Debug.LogError("LY --->>> 大本营数据不存在!");
        //    }
        //    else
        //    {
        //        mapDataCache = SimpleJson.SimpleJson.DeserializeObject<MapDataCache>(ta.text);
        //        mapGridPurificationDataCache = new MapGridPurificationDataCache();
        //        foreach (MapGridGameData data in mapDataCache.playerDataList)
        //        {
        //            PurificationData purData = new PurificationData();
        //            purData.x = data.x;
        //            purData.y = data.y;
        //            purData.value = 0;
        //            mapGridPurificationDataCache.purificationList.Add(purData);
        //        }
        //    }
        //}
    }
    #endregion

    #region 服务器初始化数据
    /// <summary>
    /// 从服务器加载生物数据
    /// </summary>
    /// <param name="monsterData"></param>
    public static void LoadDataFromServer(Game_data data)
    {

        //if (NetMgr.shouldLoadDataFromLocal)
        //{
        //    Debug.Log("使用本地缓存 ： 不需要理会服务器数据 完全使用本地缓存加载!");
        //    return;
        //}
        if (data == null && data.CampData != null)
        {
            //Debug.LogError("本该使用服务器数据的情况下 但是服务器给发送的data 是个空的");
            return;
        }
        mapDataCache = new MapDataCache();
        mapDataCache.width = (int)data.CampData.MapSize.Width;
        mapDataCache.height = (int)data.CampData.MapSize.Height;
        mapDataCache.playerDataList = new List<MapGridGameData>(mapDataCache.width * mapDataCache.height);
        mapGridPurificationDataCache = new MapGridPurificationDataCache();

        entityAndMonsterDataCache = new EntityAndMonsterDataCache();
        entityAndMonsterDataCache.chapterIndex = 0;

        MapDataCache tmpMapDataCache = GetMapDataCache();

        for (int i = 0; i < data.CampData.Grid.Count; i++)
        {
            Grid_data grid = data.CampData.Grid[i];
            MapGridGameData mapData = new MapGridGameData();
            mapData.x = (int)grid.Coord.X;
            mapData.y = (int)grid.Coord.Y;


            //这里要最新地图放上去以后判断地形  草皮  的正确性
            if (grid.Terrain != null)
            {
                mapData.hasVegetation = tmpMapDataCache.playerDataList[i].hasVegetation;
            }

            PurificationData purificationData = new PurificationData();
            purificationData.x = mapData.x;
            purificationData.y = mapData.y;
            purificationData.value = (int)grid.CureCount;

            //给每个地块赋值净化值
            mapGridPurificationDataCache.purificationList.Add(purificationData);

//            if (grid.Entity != null)
//            {
////                mapData.entityId = (int)grid.Entity.Id;
//
//                //MapObjectGameData objData = DataConvert.ConverServeMapObjectData(grid.Entity, mapData.x, mapData.y);
//                //if (entityAndMonsterDataCache.entityList.Contains(objData))
//                //{
//                //    //Debug.LogError("卢洋的Log : 严重错误-->> 有重复的key添加!" + objData.id);
//                //}
//                //else
//                //{
//                //    entityAndMonsterDataCache.entityList.Add(objData);
//                //}
//            }
            mapDataCache.playerDataList.Add(mapData);
        }

        LoadExtraGameData(data.MonsterList);
    }

    private static MapDataCache GetMapDataCache()
    {
        MapDataCache tmpMapDataCache = new MapDataCache();

        //DataTable table = LocalCache.LoadTable(BASE_CAMP_MAP_DATA_TABLE_NAME);
        //if (table.Rows.Count > 0)
        //{
        //    foreach (DataRow dr in table.Rows)
        //    {
        //        string map_data = Convert.ToString(dr["data"]);
        //        tmpMapDataCache = SimpleJson.SimpleJson.DeserializeObject<MapDataCache>(map_data);
        //    }
        //}
        //else
        //{
        //    currentLoadPath = baseCampPath;

        //    TextAsset ta = Resources.Load(currentLoadPath) as TextAsset;
        //    if (ta == null)
        //    {
        //        Debug.LogError("LY --->>> 大本营数据不存在!");
        //    }
        //    else
        //    {
        //        tmpMapDataCache = SimpleJson.SimpleJson.DeserializeObject<MapDataCache>(ta.text);
        //    }
        //}
        return tmpMapDataCache;
    }

    /// <summary>
    /// 加载出本地的数据和服务器发送过来的数据进行匹配 （某些物体的值是服务器不会记录的 所以在这里进行对应赋值）  /// </summary>
    /// <param name="monsterData"></param>
    private static void LoadExtraGameData(RepeatedField<Monster_id_data> monsterData)
    {
        bool isHaveLocaCache = false;
        EntityAndMonsterDataCache tmpEntityAndMonsterData = new EntityAndMonsterDataCache();

        //DataTable table = LocalCache.LoadTable(BASE_CAMP_MAP_DATA_TABLE_NAME);
        //if (table.Rows.Count > 0)
        //{
        //    foreach (DataRow dr in table.Rows)
        //    {
        //        string entity_data = Convert.ToString(dr["entity_data"]);

        //        if (string.IsNullOrEmpty(entity_data))
        //        {
        //            tmpEntityAndMonsterData = new EntityAndMonsterDataCache();
        //        }
        //        else
        //        {
        //            isHaveLocaCache = true;
        //            tmpEntityAndMonsterData = SimpleJson.SimpleJson.DeserializeObject<EntityAndMonsterDataCache>(entity_data);
        //        }
        //    }
        //}

        foreach (MapObjectGameData data in entityAndMonsterDataCache.entityList)
        {
            //服务器的物品都赋值完了  但是仍然还有可以赋值的
            if (tmpEntityAndMonsterData.entityList.Count == 0)
            {
                //当换设备的时候 因为这几个值是本地缓存的  所以需要手动从表中给赋一个值  否则会立即触发对应的事件
                HarvestEventData harvestData = TableDataMgr.GetSingleHarvestEventData(data.id);
                if (harvestData != null && harvestData.cooldown > 0)
                {
                    if (isHaveLocaCache)
                    {
                        //Debug.Log("服务器有的数据 但是本地没有：之前的采集充能相关操作会导致的问题 卢洋!!!" + data.id, LogFontSize.Big, "yellow");
                    }
                    data.collect_recharge_time = harvestData.cooldown;
                }

                MapObjectData tableData = TableDataMgr.GetSingleMapObjectData(data.id);
                if (tableData != null)
                {
                    if (data.id != MapObject.OBJECT_ID_SPECIAL_BALL)
                    {
                        if (tableData.liveTime != null && tableData.liveTime.Length > 1)
                        {
                            if (isHaveLocaCache)
                            {
                                //Debug.Log("服务器有的数据 但是本地没有：之前的自然死亡时间操作会导致的问题 卢洋!!!" + data.id, LogFontSize.Big, "yellow");
                            }
                            data.dead_time = tableData.liveTime[1];
                            ///时间戳，删除物品的时间
                            data.delete_time = tableData.liveTime[1] + TimerMgr.GetSeconds();
                        }
                    }
                }

                SpawnEventData spawnData = TableDataMgr.GetSingleSpawnEventData(data.id);
                if (spawnData != null)
                {
                    if (spawnData.interval != null && spawnData.interval.Length > 1)
                    {
                        if (isHaveLocaCache)
                        {
                            //Debug.Log("服务器有的数据 但是本地没有：之前的剩余衍生时间操作会导致的问题 卢洋!!!" + data.id, LogFontSize.Big, "yellow");
                        }
                        data.spawn_recharge_time = spawnData.interval[1];
                    }
                }

                TapEventData tapData = TableDataMgr.GetSingleTapEventData(data.id);
                if (tapData != null && tapData.cooldown > 0)
                {
                    if (isHaveLocaCache)
                    {
                        //Debug.Log("服务器有的数据 但是本地没有：之前的剩余充能时间操作会导致的问题 卢洋!!!" + data.id, LogFontSize.Big, "yellow");
                    }
                    data.tap_recharge_time = tapData.cooldown;
                }
            }

            for (int i = 0; i < tmpEntityAndMonsterData.entityList.Count; i++)
            {
                MapObjectGameData tmpData = tmpEntityAndMonsterData.entityList[i];
                if (data.id == tmpData.id && data.x == tmpData.x && data.y == tmpData.y)
                {
                    data.x = tmpData.x;
                    data.y = tmpData.y;
                    data.collect_recharge_time = tmpData.collect_recharge_time;
                    data.dead_time = tmpData.dead_time;
                    data.delete_time = tmpData.delete_time;
                    data.spawn_recharge_time = tmpData.spawn_recharge_time;
                    data.tap_recharge_time = tmpData.tap_recharge_time;
                    data.foundation_progress = tmpData.foundation_progress;
                    tmpEntityAndMonsterData.entityList.Remove(tmpData);
                    i--;
                    break;
                }
            }
        }

        ///将所有龙数据添加到字典  ----  气泡在郑阳那边存着 所以这里不做存储
        foreach (Monster_id_data mo in monsterData)
        {
            foreach (Monster_data monster in mo.List)
            {
                //MapObjectGameData objData = DataConvert.ConverServerMapObjectData(monster, (int)mo.MonsterId);

                //if (entityAndMonsterDataCache.entityList.Contains(objData))
                //{
                //    //Debug.LogError("卢洋的Log : 严重错误-->> 有重复的key添加!" + objData.id);
                //}
                //else
                //{
                //    entityAndMonsterDataCache.entityList.Add(objData);
                //}
            }
        }
    }
    #endregion


    /// <summary>
    /// 对大本营 - 关卡 - 或掠夺场景 进行缓存清除操作
    /// </summary>
    /// <param name="type"></param>
    public static void ClearBaseCampOrPlunderData()
    {
        //ClearSealLockMapGridEffect();
        //TipFlyMgr.ClearAllTipFly();
        CommonObjectMgr.ClearTip();
        //VFXMgr.Clear();

        if (Instance.mapGrid == null)
        {
            return;
        }
        isMapClearing = true;
        if (isInitialzeLoad == false)
        {
            ClearPurificationList();
            foreach (MapGrid grid in Instance.mapGrid)
            {
                if (grid.Vegetation != null)
                {
                    //RecyleTerrainFurlToPool(grid.Vegetation);
                    Instance.vegetationRenderer.ChangeTileMap(grid.point.x, grid.point.y, null);
                    grid.Vegetation = null;
                }

            }

            //foreach (List<MapObject> list in entityMapObjectDic.Values)
            //{
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        RecycleEntityMapObject(list[i]);
            //        i--;
            //    }
            //}

            //foreach (List<MapObject> list in noSaveEntityMapObjectDic.Values)
            //{
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        RecycleNoSaveEntityMapObject(list[i]);
            //        i--;
            //    }
            //}

            //noSaveEntityMapObjectDic.Clear();
          /*  foreach (MapUnlcokCache cache in Instance.mapUnlockCacheDict.Values)
            {
                if (cache.slider != null)
                {
                    cache.slider.ClearPrefab();
                    //dragonPowerPool.RecycleInstance(cache.slider);
                    cache.slider = null;
                }
            }*/

            //对净化粒子进行清理
            foreach (MapObject obj in purification_Automatic_GridCureValueChangeList.Values)
            {
                //obj.PurificationMoveKill();
            }
            purification_Automatic_GridCureValueChangeList.Clear();
        }
        //Instance.mapUnlockCacheDict.Clear();
        Instance.wallMap.CompleteReset();
        Instance.terrainMap.CompleteReset();
        Instance.vegetationMap.CompleteReset();
        Instance.entityMap.CompleteReset();
        Instance.sealLockMap.CompleteReset();
        Instance.mapGrid = null;
        unlockAndCuredTerrainList.Clear();
        //ClearMapObjectWhenClearMap();
        isMapClearing = false;
    }

    /// <summary>
    /// 大本营缓存数据发生变化
    /// </summary>
    public static void BaseCampLocaCacheChange()
    {
        if (isMapInitialzeLoading || Instance.mapGrid == null)
        {
            return;
        }
        //将数据格式解析成类
        if (entityAndMonsterDataCache == null)
        {
            entityAndMonsterDataCache = new EntityAndMonsterDataCache();
        }
        entityAndMonsterDataCache.entityList.Clear();

        //地图数据的赋值
        for (int i = 0; i < mapDataCache.playerDataList.Count; i++)
        {
//            mapDataCache.playerDataList[i].entityId = 0;
        }
        entityAndMonsterDataCache.chapterIndex = 0;
    }

    /// <summary>
    /// 加载关卡数据
    /// </summary>
    /// <param name="chapterId"></param>
    //public void LoadChapterData(int chapterId, LocalChapterInfo info)
    //{
    //    //ChapterModel.CurrentChapterIndex = info.index;
    //    GlobalVariable.GameState = GameState.LevelModel;
    //    TextAsset ta = null;
    //    if (info.has_cache >= (int)ChapterCacheType.Cache_Have)
    //    {
    //        //进行关卡的临时数据字典生成  用于加载
    //        EntityAndMonsterDataTempCache cache = new EntityAndMonsterDataTempCache();
    //        foreach (MapObjectGameData data in info.entity_data.entityList)
    //        {
    //            MapObjectData objData = TableDataMgr.GetSingleMapObjectData(data.id);
    //            if (objData.objectType != 711 && objData.objectType != 729)
    //            {
    //                MapObjectGameDataKey key = null;
    //                if (objData.detachGrid && objData.illustration == (int)Handbook.Monster ||
    //                    objData.objectType == MapObject.OBJECT_TYPE_EVIL_MONSTER)
    //                {
    //                    key = new MapObjectGameDataKey(0, 0, data.id, data.only_id);
    //                }
    //                else
    //                {
    //                    key = new MapObjectGameDataKey(data.x, data.y, data.id, 0);
    //                }
    //                if (cache.dict.ContainsKey(key))
    //                {
    //                    Debug.LogError("这里有点问题，需要排查一下  key居然相同了！！！" + cache.dict[key].id + "-" + cache.dict[key].x +
    //                        "-" + cache.dict[key].y);
    //                }
    //                else
    //                {
    //                    cache.dict.Add(key, data);
    //                }
    //            }
    //        }

    //        LoadMapData(info.map_data, cache, info.purification_data);
    //    }
    //    else
    //    {
       
    //        ta = ResMgr.Load<TextAsset>(chapterId.ToString());

    //        MapDataCache saveEditor = SimpleJson.SimpleJson.DeserializeObject<MapDataCache>(ta.text);
    //        LoadMapData(saveEditor, null, null);
    //    }
    //}

    /// <summary>
    /// 加载大本营 
    /// </summary>
    public void LoadBaseCampData()
    {
        //ChapterModel.CurrentChapterIndex = GlobalVariable.BASE_CAMP;
        //GlobalVariable.GameState = GameState.MainSceneMode;

        //进行关卡的临时数据字典生成  用于加载
        EntityAndMonsterDataTempCache cache = new EntityAndMonsterDataTempCache();
        foreach (MapObjectGameData data in entityAndMonsterDataCache.entityList)
        {
            MapObjectData objData = TableDataMgr.GetSingleMapObjectData(data.id);

            if (objData == null)
            {
                //Debug.Log("表数据里并不存在服务器发送过来的物品id" + data.id);
            }

            MapObjectGameDataKey key = null;
            if (objData.detachGrid && objData.illustration == (int)Handbook.Monster)
            {
                key = new MapObjectGameDataKey(0, 0, data.id, data.only_id);
            }
            else
            {
                key = new MapObjectGameDataKey(data.x, data.y, data.id, 0);
            }

            if (cache.dict.ContainsKey(key))
            {
                //Debug.LogError("有相同的key ：算法有问题" + key.id);
            }
            cache.dict.Add(key, data);
        }

        LoadMapData(mapDataCache, cache, mapGridPurificationDataCache);
    }

    /// <summary>
    /// 进行地图尺寸的初始化 以及摄像机尺寸的初始化
    /// </summary>
    /// <param name="saveEditor"></param>
    /// <param name="entityCacheData"></param>
    /// <param name="purificationData"></param>
    private void LoadMapData(MapDataCache saveEditor, EntityAndMonsterDataTempCache entityCacheData, MapGridPurificationDataCache purificationData)
    {
        //LoginUIModel.ChangeLoadingSliderTextType(LoadingSliderTextType.LoadingGameData1);
        if (isInitialzeLoad)
        {
            UIMgr.ShowUI(UIDef.LoginUI);
            //if (GlobalVariable.GameState == GameState.MainSceneMode)
            //{
            //    UIMgr.SetActive(UIDef.MainUI, false);
            //}
            //else if (GlobalVariable.GameState == GameState.LevelModel)
            //{
            //    UIMgr.SetActive(UIDef.ChapterSceneUI, false);
            //}
        }
        else
        {
            UIMgr.ShowUI(UIDef.MainAndChapterSwichUI);
            //if (GlobalVariable.GameState == GameState.LevelModel)
            //{
            //    UIMgr.SetActive(UIDef.ChapterSceneUI, false);
            //}
        }
        if (entityCacheData == null)
        {
            entityCacheData = new EntityAndMonsterDataTempCache();
        }

        mapWidth = saveEditor.width;
        mapHeight = saveEditor.height;
//
//		float mapXSize = mapWidth;
//		float mapYSize = mapHeight;
//
//        if (mapXSize < 36)
//        {
//            mapXSize = 36;
//        }
//        if (mapYSize < 27)
//        {
//            mapYSize = 27;
//        }

        //float offsize = 0;
        //offsize = (float)mapWidth / (float)mapHeight - 1;
        //if (mapWidth == mapHeight)
        //{
        //    mapRect = new Rect(-mapXSize / (2 + offsize), -8, mapXSize, mapYSize - mapHeight * 0.5f + 2);
        //}
        //else
        //{
        //    mapRect = new Rect(-mapXSize / (2 + offsize), -8, mapXSize, mapYSize + 2);
        //}

        //CameraGestureMgr.Instance.Init(4, mapRect);
        //if (GlobalVariable.GameState == GameState.MainSceneMode)
        //{
        //    CameraGestureMgr.Instance.ScaleCameraInstant(PlayerProfile.LoadGameCameraOrthographicSize() / 10f);
        //}
        //else
        //{
        //    CameraGestureMgr.Instance.ScaleCameraInstant(ComUtil.Camera_InitSize);
        //}

        //if (GlobalVariable.GameState == GameState.LevelModel)
        //{
        //    //Point cameraPoint = new Point(ChapterModel.CurrentLevelData.cameraPos[0], ChapterModel.CurrentLevelData.cameraPos[1]);
            //Vector3 cameraPos = GetWorldPosByPoint(cameraPoint);
            //cameraPos.x = 0;
            //CameraGestureMgr.Instance.MoveCamera(cameraPos);
        //}
        //else
        //{
        //    CameraGestureMgr.Instance.MoveCamera(ComUtil.Camera_InitPos);
        //}

        wallMap.ResizeMap(mapWidth, mapHeight, false);
        terrainMap.ResizeMap(mapWidth, mapHeight, false);
        vegetationMap.ResizeMap(mapWidth, mapHeight, false);
        entityMap.ResizeMap(mapWidth, mapHeight, false);
        purificationMap.ResizeMap(mapWidth, mapHeight, false);
        sealLockMap.ResizeMap(mapWidth, mapHeight, false);

        mapGrid = new MapGrid[mapWidth, mapHeight];

        StartCoroutine(LoadPlayerData(saveEditor.playerDataList, entityCacheData.dict, purificationData));
    }

    /// <summary>
    /// 进行大本营-关卡的场景内物体数据生成  -- 至函数结束时加载流程完毕
    /// </summary>
    /// <param name="playerDataList"></param>
    /// <param name="entityDataDict"></param>
    private IEnumerator LoadPlayerData(List<MapGridGameData> playerDataList, Dictionary<MapObjectGameDataKey, MapObjectGameData> entityDataDict, MapGridPurificationDataCache purificationData)
    {
        if (PlayerProfile.LoadFristLoadBaseCamp() == 0)
        {
            TDGAModel.UserDefineEvent(SelfEvent.FRIST_LOAD_BASE_CAMP);
            PlayerProfile.SaveFristLoadBaseCamp(1);
        }

        int totalcount = mapGrid.Length * 4;
        int count = 0;
        int yieldDealy = 0;
        isMapInitialzeLoading = true;
        effectiveBlocksCount = 0;
        //地形和阴影
        foreach (MapGridGameData info in playerDataList)
        {
            mapGrid[info.x, info.y] = new MapGrid();
            mapGrid[info.x, info.y].point = new Point(info.x, info.y);

            if (info.hasVegetation > 0)
            {
                VegetationData vegetData = TableDataMgr.GetSingleVegetationData(info.hasVegetation);
                mapGrid[info.x, info.y].vegetationHue = vegetData.hueValue;
                mapGrid[info.x, info.y].vegetationId = info.hasVegetation;
            }

//            if (info.hasTerrain == 1)
//            {
//                terrainMap.SetTileAndUpdateNeighbours(info.x, info.y, terrainTile);
//                wallMap.SetTileAndUpdateNeighbours(info.x, info.y, wallTile);
//                effectiveBlocksCount++;
//            }
//            else if (info.hasTerrain == 2)
//            {
//                terrainMap.SetTileAndUpdateNeighbours(info.x, info.y, bridgeTile);
//            }
            count++;
            if (Event_LoadingMapContent != null)
            {
                Event_LoadingMapContent(count, totalcount);
            }
            yieldDealy++;
            if (yieldDealy % 35 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        //LoginUIModel.ChangeLoadingSliderTextType(LoadingSliderTextType.LoadingGameData2);

        //草皮
        foreach (MapGridGameData info in playerDataList)
        {
            if (info.hasVegetation != 0)
            {
                vegetationMap.SetTileAndUpdateNeighbours(info.x, info.y, vegetationTile);
            }
            count++;
            if (Event_LoadingMapContent != null)
            {
                Event_LoadingMapContent(count, totalcount);
            }
            yieldDealy++;
            if (yieldDealy % 35 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        //LoginUIModel.ChangeLoadingSliderTextType(LoadingSliderTextType.LoadingGameData3);

        //实体
        foreach (MapGridGameData info in playerDataList)
        {
//            if (info.entityId != 0)
//            {
//                MapObjectData data = TableDataMgr.GetSingleMapObjectData(info.entityId);
//                if (data.illustration == (int)Handbook.Monster && data.detachGrid)
//                {
//                    //GetEntityMapObject(info.entityId);
//                }
//                else
//                {
//                    SimpleTile simpleTile = ResMgr.Load<SimpleTile>(ComUtil.GetStringAdd("Tile", info.entityId));
//                    mapGrid[info.x, info.y].entityId = info.entityId;
//                    entityMap.SetTileAt(info.x, info.y, simpleTile, false);
//                }
//            }
            count++;
            if (Event_LoadingMapContent != null)
            {
                Event_LoadingMapContent(count, totalcount);
            }
            yieldDealy++;
            if (yieldDealy % 35 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        //封印
        foreach (MapGridGameData info in playerDataList)
        {
//            if (info.sealLockId >= 100)
//            {
//                sealLockMap.SetTileAndUpdateNeighbours(info.x, info.y, sealLockTile);
//                allLockMapGridList.Add(mapGrid[info.x, info.y]);
//            }
            count++;
            if (Event_LoadingMapContent != null)
            {
                Event_LoadingMapContent(count, totalcount);
            }
            yieldDealy++;
            if (yieldDealy % 35 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        //判断状态
        foreach (MapGridGameData info in playerDataList)
        {
//            mapGrid[info.x, info.y].deadLandData = TableDataMgr.GetSingleDeadLandData(info.purificationLevel);
//
//            if (info.sealLockId != 0 || info.hasTerrain == 0)
//            {
//                mapGrid[info.x, info.y].SetStatus(MapGridState.Locked);
//            }
//            else if (info.purificationLevel != 0)
//            {
//                mapGrid[info.x, info.y].SetStatus(MapGridState.UnlockButDead);
//            }
//            else if (info.hasTerrain == 2)
//            {
//                mapGrid[info.x, info.y].SetStatus(MapGridState.Locked);
//            }
//            else
//            {
               
//            }
        }


        //如果有本地数据 那么需要依照本地数据进行 龙的物体生成以及数据赋值
        if (entityDataDict.Count > 0)
        {
            MapObjectInitializeOnObjectLoadInGridOver(entityDataDict);
        }
        else
        {
            //如果没有本地缓存数据 那么就需要对场地上的所有物体进行默认的初始化赋值
            MapObjectInitialize();
        }

        //if (GlobalVariable.GameState == GameState.MainSceneMode)
        //{
        //    //进行锁定逻辑处理
        //    MapUnlockInit();
        //    //进行被掠夺封印处理
        //    PlunderSealLand();
        //}
        //else
        //{
        //    //如果为关卡状态  所有物体生成的时候 就需要进行计时器暂停操作，有关卡自行进行计时器开始的操作
        //    AllEntityTimerPause();
        //}

        //InitializeSeaEffect();

        AdjustWaveSortingOrder();
        //InitializeAllSleepBuilding();

        //当前地图准备完毕
        isMapInitialzeLoading = false;
        if (Event_MapInitialzeLoadFinish != null)
        {
            Event_MapInitialzeLoadFinish();
        }

        //对地图的所有操作完成之后才能去展示主界面让玩家操作，（如果还没完成，就让玩家操作，会出现意料之外的问题，，顺序不能更改）
        if (isInitialzeLoad)
        {
            UIMgr.HideUI(UIDef.LoginUI);
            //if (GlobalVariable.GameState == GameState.MainSceneMode)
            //{
                UIMgr.SetActive(UIDef.MainUI, true);
                //if (TutorialModel.IsTutorialComplete(TutorialSection.NewTask))
                //{
                //    if (TutorialModel.IsTutorialUnlockAndNotComplete(TutorialSection.UnlockingPlunder) == false)
                //    {
                //        if (AnnouncementsAndMailUIModel.isHaveAnnouncement)
                //        {
                //            UIMgr.ShowUI(UIDef.InGameAnnouncementUI);
                //        }
                //        else if (PlunderInitialPromptUIModel.showPlunderInitialPromptUI)
                //        {
                //            UIMgr.ShowUI(UIDef.PlunderInitialPromptUI);
                //        }
                //    }
                //}
            //}
            //else if (GlobalVariable.GameState == GameState.LevelModel)
            //{
            //    UIMgr.SetActive(UIDef.ChapterSceneUI, true, true);
            //}
        }
        else
        {
            //UIMgr.HideUI(UIDef.MainAndChapterSwichUI);
            //if (GlobalVariable.GameState == GameState.LevelModel)
            //{
            //    UIMgr.SetActive(UIDef.ChapterSceneUI, true, true);
            //}
        }

        //游戏的初始初始化完毕
        isInitialzeLoad = false;
        //if (GlobalVariable.GameState == GameState.MainSceneMode)
        //{
            //if (PlunderInitialPromptUIModel.showPlunderInitialPromptUI)
            //{
            //    UIMgr.ShowUI(UIDef.PlunderInitialPromptUI);
            //}
        //}
        //Debug.Log("所有的有效地块总数(不为空 并且 非高端区的) = " + effectiveBlocksCount);

        if (PlayerProfile.LoadFristLoadBaseCamp() == 1)
        {
            TDGAModel.UserDefineEvent(SelfEvent.FIRST_END_BASECAMP_LOAD_INTERFACE);
            PlayerProfile.SaveFristLoadBaseCamp(2);
        }

    }

    /// <summary>
    /// 解决浪花与墙面的层次问题
    /// </summary>
    private void AdjustWaveSortingOrder()
    {
        //将浪花的sortingOrder改为其所属连续行列前一格的sortingOrder值
        for (int x = 0; x < wallMap.MapWidth; x++)
        {
            for (int y = 0; y < wallMap.MapWidth; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                GameObject wallObj = wallRenderer.GetTileGameObject(x, y);
                if (wallObj == null)
                {
                    continue;
                }
                int minX = x;
                int minY = y;

                //先看y方向有没有
                GameObject wallObjY_1 = wallRenderer.GetTileGameObject(x, y - 1);
                if (wallObjY_1 == null || y < 1)
                {
                    if (x > 0)
                    {
                        GameObject wallNeightour = wallRenderer.GetTileGameObject(x - 1, y);
                        if (wallNeightour != null)
                        {
                            minX--;
                        }
                    }
                }
                //再看x方向有没有
                GameObject wallObjX_1 = wallRenderer.GetTileGameObject(x - 1, y);
                if (wallObjX_1 == null || x < 1)
                {
                    if (y > 0)
                    {
                        GameObject wallNeightour = wallRenderer.GetTileGameObject(x, y - 1);
                        if (wallNeightour != null)
                        {
                            minY--;
                        }
                    }
                }

                ChangeWaveSortingOrder(wallObj, minX, minY);
            }
        }
    }

    /// <summary>
    /// 修改墙体的浪花的层次
    /// </summary>
    /// <param name="wall">墙体</param>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    private void ChangeWaveSortingOrder(GameObject wall, int x, int y)
    {
        SpriteRenderer[] srs = wall.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in srs)
        {
            if (sr.name.Equals("wave_side"))
            {
                sr.sortingOrder = (int)wallRenderer.GetSortingOrderValue(x, y, sr.gameObject);
            }
        }
    }

    /// <summary>
    /// 在没有本地缓存的情况下 对场景上的物体进行初始化赋值
    /// </summary>
    private static void MapObjectInitialize()
    {
        //foreach (List<MapObject> list in entityMapObjectDic.Values)
        //{
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        list[i].InitializeExtraGameData();
        //    }
        //}
    }

    /// <summary>
    /// 在拥有初始话数据的情况下
    /// 对地图内的物体和龙进行数据初始化
    /// </summary>
    private static void MapObjectInitializeOnObjectLoadInGridOver(Dictionary<MapObjectGameDataKey, MapObjectGameData> Dict)
    {
        monsterOnlyStartId = 1;
        //加载所有的龙的数据
        foreach (MapObjectGameData objData in Dict.Values)
        {
            //MapObjectData data = TableDataMgr.GetSingleMapObjectData(objData.id);
            //if (GlobalVariable.GameState == GameState.LevelModel && data.objectType == MapObject.OBJECT_TYPE_EVIL_MONSTER)
            //{
            //    MapObject obj = GetEntityMapObject(data.id);
            //    obj.InitializeGameData(objData);
            //}
            //else if (data.detachGrid && data.illustration == (int)Handbook.Monster)
            //{
            //    MapObject obj = GetEntityMapObject(data.id, objData.only_id);
            //    if (monsterOnlyStartId <= objData.only_id)
            //    {
            //        monsterOnlyStartId = objData.only_id + 1;
            //    }
            //    obj.InitializeGameData(objData);
            //}
            //else
            //{
            //    if (Instance.mapGrid[objData.x, objData.y].Entity == null)
            //    {
            //        Debug.LogError(string.Format("这是一个异常(合成或是拖拽等操作没有对物体进行TakeOut造成了数据残留):{0}-{1}-{2}", objData.x, objData.y, objData.id));
            //    }
            //    else
            //    {
            //        Instance.mapGrid[objData.x, objData.y].Entity.InitializeGameData(objData);
            //    }
            //    //TODO:正在尝试是否可以在生成物体的时候给物体就赋值了  如果尝试的不太顺利就在这里赋值
            //}
        }
    }

    /// <summary>
    /// 草皮绘制完毕后的回调
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="go"></param>
    private void VegetationOnRenderTile(int x, int y, GameObject go)
    {
        if (mapGrid != null && mapGrid[x, y] != null)
        {
            if (go != null)
            {
                mapGrid[x, y].Vegetation = go.GetComponent<MapObject>();
                mapGrid[x, y].Vegetation.StaticPos = mapGrid[x, y].point;
                mapGrid[x, y].Vegetation.StaticMapGridList.Clear();
                mapGrid[x, y].Vegetation.StaticMapGridList.Add(mapGrid[x, y]);
                vegetationRenderer.ReOrder(x, y, go);
            }
            else
            {
                if (mapGrid[x, y].Vegetation != null)
                {
                    mapGrid[x, y].Vegetation.StaticMapGridList.Clear();
                    mapGrid[x, y].Vegetation = null;
                }
            }
        }
    }

    #endregion

  

    /// <summary>
    /// 根据服务器发送过来的被掠夺数据 对相应的格子进行封印（被掠夺的建筑会变为死地）
    /// </summary>
    public void PlunderSealLand()
    {
        Dictionary<Point, UpdateTerrainInfo> updateTerrainDict = new Dictionary<Point, UpdateTerrainInfo>();

        //foreach (Point point in PlunderInitialPromptUIModel.pointList)
        //{
        //    MapGrid data = GetMapGridData(point);
        //    data.deadLandData = TableDataMgr.GetSingleDeadLandData(1);
        //    if (data.Status != MapGridState.Locked)
        //    {
        //        data.SetStatus(MapGridState.UnlockButDead);
        //        UpdateTerrainInfo info = new UpdateTerrainInfo();
        //        info.dead_level = 1;
        //        info.state = Grid_state.UnlockAndDead;
        //        info.point = point;
        //        if (updateTerrainDict.ContainsKey(point) == false)
        //        {
        //            updateTerrainDict.Add(point, info);
        //        }
        //    }
        //}

        if (updateTerrainDict.Count > 0)
        {
            SendGridStatusChange(UpdateTerrainEventType.StatusAndDeadLevelChange, updateTerrainDict);
        }
    }

   
   

    /// <summary>
    /// 从大本营缓存中随机获取一只生物（召唤法阵合成后 会在关卡中召唤一直大本营的龙到关卡内）
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetAllMonsterFromBaseCampCache(int id)
    {
        MonsterPortalData data = TableDataMgr.GetSingleMonsterPortalData(id);
        if (data == null)
        {
            return 0;
        }
        List<int> typeList = new List<int>();
        typeList = new List<int>(data.typeList);
        int type = typeList[UnityEngine.Random.Range(0, typeList.Count)];

        List<MapObjectGameData> monsterList = new List<MapObjectGameData>();
        List<MapObjectGameData> allMonsterList = new List<MapObjectGameData>();

        foreach (MapObjectGameData monster in entityAndMonsterDataCache.entityList)
        {
            MonsterAttributesData monsterData = TableDataMgr.GetSingleMonsterAttributesData(monster.id);
            if (monsterData != null)
            {
                if (monsterData.type == type)
                {
                    monsterList.Add(monster);
                }
                else
                {
                    allMonsterList.Add(monster);
                }
            }
        }

        //如果大本营没有龙那么产生一个固定的龙50302
        if (allMonsterList.Count == 0)
        {
            return 50302;
        }
        else if (monsterList.Count == 0)
        {
            int index = UnityEngine.Random.Range(0, allMonsterList.Count);
            return allMonsterList[index].id;
        }
        else
        {
            int index = UnityEngine.Random.Range(0, monsterList.Count);
            return monsterList[index].id;
        }
    }

    public static List<MapObject> GetCuredTerrain()
    {
        return unlockAndCuredTerrainList;
    }
}

/// <summary>
/// 实体以及生物的数据临时加载缓存
/// 字典形式：因为json无法将字典形式存储进本地数据库  但是用字典在使用中查找比较方便 所以制作一个临时的类
/// 用于加载使用 （将本地数据库读取出的list形式缓存转为字典形式进行加载使用）
/// </summary>
public class EntityAndMonsterDataTempCache
{
    public int chapterIndex = 0;
    public Dictionary<MapObjectGameDataKey, MapObjectGameData> dict = new Dictionary<MapObjectGameDataKey, MapObjectGameData>();
}

/// <summary>
/// 实体以及生物的数据缓存
/// </summary>
public class EntityAndMonsterDataCache
{
    public int chapterIndex = 0;
    public List<MapObjectGameData> entityList = new List<MapObjectGameData>();
}

/// <summary>
/// 实体以及生物的数据字典的key
/// 因为物体的 x y id 都有可能重复  所以单独构造了这个类作为字典的key 用来使字典可以兼容生物以及实体
/// </summary>
public class MapObjectGameDataKey
{
    public int x;
    public int y;
    public int id;
    public int onlyId;

    public MapObjectGameDataKey(int x, int y, int id, int onlyId)
    {
        this.x = x;
        this.y = y;
        this.id = id;
        this.onlyId = onlyId;
    }

    public override int GetHashCode()
    {
        return x ^ (y * 3) ^ (id * 7) ^ (onlyId * 13);
    }

    public override bool Equals(object obj)
    {
        if (obj == this) return true;
        if (!(obj is MapObjectGameDataKey))
        {
            return false;
        }
        MapObjectGameDataKey key = (MapObjectGameDataKey)obj;
        return key.x == x &&
            key.y == y &&
            key.id == id &&
            key.onlyId == onlyId;
    }
}

/// <summary>
/// 缓存格子物体的数据
/// </summary>
public class MapObjectGameData
{
    public int id = 0;
    public int x = 0;
    public int y = 0;
    public int current_HP = 0;
    public int status = 0;

    #region 龙
    public int only_id = 0;
    public string name = "";
    public int actionPoints = 0;
    #endregion

    #region 金币钻石
    /// <summary>
    /// 剩余金币数量
    /// </summary>
    public int remain_amount = 0;
    #endregion

    #region 可点击物品
    /// <summary>
    /// 剩余点击次数
    /// </summary>
    public int left_tap_count = 0;
    /// <summary>
    /// 最大点击次数
    /// </summary>
    public int tap_max_mount = 0;
    /// <summary>
    /// 剩余充能时间
    /// </summary>
    public int tap_recharge_time = 0;//本地记录
    #endregion

    #region 衍生物品
    /// <summary>
    /// 剩余衍生数量
    /// </summary>
    public int left_spawn_count = 0;
    /// <summary>
    /// 最大衍生数量
    /// </summary>
    public int spawn_max_mount = 0;
    /// <summary>
    /// 剩余衍生时间
    /// </summary>
    public int spawn_recharge_time = 0;//本地记录
    #endregion

    #region 自然死亡物品
    /// <summary>
    /// 剩余自然死亡时间
    /// </summary>
    public int dead_time = 0;//本地记录
    /// <summary>
    /// 删除物品的时间(时间戳) 用于在用户切换账号后，保存该时间的真实性
    /// </summary>
    public int delete_time = 0;//本地记录(服务器保存)
    #endregion

    #region 被采集物品
    /// <summary>
    /// 剩余采集数量
    /// </summary>
    public int left_collect_count = 0;
    /// <summary>
    /// 最大采集数量
    /// </summary>
    public int current_max_collect_mount = 0;
    /// <summary>
    /// 采集充能时间
    /// </summary>
    public int collect_recharge_time = 0;//本地记录
    #endregion

    #region 地基
    /// <summary>
    /// 地基在建筑商店的商品id
    /// </summary>
    public int shop_id;
    /// <summary>
    /// 当前建筑进度
    /// </summary>
    public int foundation_progress;
    #endregion

    #region 休息建筑
    /// <summary>
    /// 休息建筑时间戳
    /// </summary>
    public long sleep_TimeStamp;
    /// <summary>
    /// 正在此建筑中休息的龙
    /// </summary>
    public List<MonsterIDData> sleeper_List = new List<MonsterIDData>();
    #endregion

    #region 战利品球
    /// <summary>
    /// 关卡ID
    /// </summary>
    public int level_id;
    /// <summary>
    /// 战力品球中的物品
    /// </summary>
    public List<MapObjectGameData> object_list = new List<MapObjectGameData>();
    #endregion
}

/// <summary>
/// 确定唯一的一条龙的数据
/// </summary>
public class MonsterIDData
{
    public int id = 0;
    public int only_id = 0;
}
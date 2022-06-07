using GameProto;
using System.Collections.Generic;
using UnityEngine;
using Universal.TileMapping;

public class UpdateTerrainInfo
{
    public Point point;
    public Grid_state state;
    public int dead_level = 0;
    public int cure_count = 0;
    public MapObject entity = null;

    public int exp = 0;
}

public enum UpdateTerrainEventType
{
    /// <summary>
    /// 无意义
    /// </summary>
    None = 0,
    /// <summary>
    /// 格子的状态进行改变 ---- 解锁地形迷雾触发
    /// </summary>
    StatusChange = 1,
    /// <summary>
    /// 死地等级进行改变 ---- 
    /// </summary>
    DeadLevelChange = 2,
    /// <summary>
    /// 死地顶级和状态一起改变 ---- 龙封印地形
    /// </summary>
    StatusAndDeadLevelChange = 3,
    /// <summary>
    /// 净化值进行改变 ---- 净化之力触发
    /// </summary>
    CureCountChange = 4,
    /// <summary>
    /// 貌似目前不存在这种情况
    /// </summary>
    DeadLevelAndCureCountChange = 6,
    /// <summary>
    /// 死地等级和净化值同时改变 ---- 地块被净化完成 合成的时候有死地  净化器触发
    /// </summary>
    AllChange = 7,
}

public partial class MapMgr
{
    public static System.Action<MapObject> Event_AddEntityToMap;

    public TileMap wallMap;
    public TileGameObjectRenderer wallRenderer;
    public ScriptableTile wallTile;
    public TileMap terrainMap;
    public TileGameObjectRenderer terrainRenderer;
    public ScriptableTile terrainTile;
    public ScriptableTile bridgeTile;
    public TileMap vegetationMap;
    public TileGameObjectRenderer vegetationRenderer;
    public ScriptableTile vegetationTile;
    public TileMap entityMap;
    public TileGameObjectRenderer entityRenderer;
    public ScriptableTile entityTile;
    public TileMap purificationMap;
    public TileGameObjectRenderer purificationRenderer;
    public ScriptableTile purificationTile;
    public TileMap sealLockMap;
    public TileGameObjectRenderer sealLockRenderer;
    public ScriptableTile sealLockTile;
    public ScriptableTile sealLock_HighAreaTile;

    private List<MapGrid> placedGridList = new List<MapGrid>();
    private List<MapGrid> realTimeGridList = new List<MapGrid>();

    private static Dictionary<Point, UpdateTerrainInfo> purification_GridAllChangeList = new Dictionary<Point, UpdateTerrainInfo>();
    private static Dictionary<Point, UpdateTerrainInfo> purification_GridCureValueChangeList = new Dictionary<Point, UpdateTerrainInfo>();

    private static Dictionary<int, MapObject> purification_Automatic_GridCureValueChangeList = new Dictionary<int, MapObject>();
    /// <summary>
    /// 所有生命粒子的缓存列表， 用于在清空地图的时候对其进行归池操作：比如断线重连时
    /// </summary>
    private static List<MapObject> purificationList = new List<MapObject>();
    /// <summary>
    /// 用来区分每一个净化粒子是不一样的
    /// </summary>
    private static int purificationIndex = 0;

    private static int purificationCount;
    /// <summary>
    /// 净化过程中获取到的经验值
    /// </summary>
    public static int purifyingExp = 0;

    public static int PurificationIndex
    {
        get { return purificationIndex; }
        set
        {
            purificationIndex = value;
        }
    }

    /// <summary>
    /// 游戏正处在生命粒子净化的过程中--此时不能点击进入关卡的按钮
    /// </summary>
    public static bool IsPurifying
    {
        get
        {
            if (purificationCount > 0)
            {
                return true;
            }
            return false;
        }
    }

    public static int PurificationCount
    {
        get
        {
            return purificationCount;
        }
        set
        {
            purificationCount = value;
            if (purificationCount == 0)
            {
                SendGridStatusChange(UpdateTerrainEventType.AllChange, purification_GridAllChangeList);
                SendGridStatusChange(UpdateTerrainEventType.CureCountChange, purification_GridCureValueChangeList);

                if (purifyingExp > 0)
                {
                    //if (GlobalVariable.GameState == GameState.MainSceneMode)
                    //{
                    //    PlayerModel.ChangePlayerDataSendServer(MATERIAL_TYPE.MaterialExp, AFFECT_MATERIAL_TYPE.EcmtClickTheBallOfLife, purifyingExp);
                    //}
                }
                purifyingExp = 0;
                purification_GridAllChangeList.Clear();
                purification_GridCureValueChangeList.Clear();
            }
        }
    }

    /// <summary>
    /// 检测地图上是否拥有怪物 并且返回怪物的MapObject
    /// </summary>
    public static MapObject GetEnemyInstance()
    {
        //if (zombieList.Count > 0)
        //{
        //    return zombieList[Random.Range(0, zombieList.Count)];
        //}
        return null;
    }

    /// <summary>
    /// 检测地图上是否有可以合成敌人的物品
    /// </summary>
    /// <returns></returns>
    public static bool CheckMapHasEnemyProducer()
    {
        //foreach (int id in entityPoolDict.Keys)
        //{
        //    int objectType = id / 100;
        //    if (objectType == MapObject.OBJECT_TYPE_ENEMY_TOTEM)
        //    {
        //        return true;
        //    }
        //}
        return false;
    }

    /// <summary>
    /// 获取到地图格子内的数据信息
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public MapGrid GetMapGridData(Vector3 worldPos)
    {
        Point point = terrainMap.WorldPosition2Coordinate(worldPos);
        if (terrainMap.IsInBounds(point.x, point.y) && mapGrid != null)
        {
            return mapGrid[point.x, point.y];
        }
        return null;
    }

    /// <summary>
    /// 根据数组下标获取到格子数据信息
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public MapGrid GetMapGridData(int x, int y)
    {
        if (terrainMap.IsInBounds(x, y) && mapGrid[x, y] != null)
        {
            return mapGrid[x, y];
        }
        return null;
    }

    //public void GetMapObjectDatasById(int id, ref List<MapObject> list)
    //{
    //    list.Clear();
    //    foreach (MapObject obj in entityMapObjectDic[id])
    //    {
    //        list.Add(obj);
    //    }
    //}

    /// <summary>
    /// 获取格子数据信息
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public MapGrid GetMapGridData(Point point)
    {
        if (terrainMap.IsInBounds(point.x, point.y) && mapGrid[point.x, point.y] != null)
        {
            return mapGrid[point.x, point.y];
        }
        return null;
    }

    /// <summary>
    /// 将一个格子变为未净化状态
    /// </summary>
    /// <param name="purificationLevel">死地等级</param>
    public void ChangeMapGridStatusToDead(Point point, int purificationLevel)
    {
        mapGrid[point.x, point.y].deadLandData = TableDataMgr.GetSingleDeadLandData(purificationLevel);
        mapGrid[point.x, point.y].CurPurificationValue = 0;
        if (unlockAndCuredTerrainList.Contains(mapGrid[point.x, point.y].Terrain))
        {
            unlockAndCuredTerrainList.Remove(mapGrid[point.x, point.y].Terrain);
            mapGrid[point.x, point.y].SetStatus(MapGridState.UnlockButDead);
        }
        else
        {
            Debug.LogError("存储的净化地形有误，请排查");
        }
    }

    /// <summary>
    /// 边界检测
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool CheckAreaIntheBoundary(Point point, Point area)
    {

        //Debug.Log("x:" + (point.x + area.x - 1), "y:" + (point.y + area.y - 1));
        if (terrainMap.IsInBounds(point.x + area.x - 1, point.y + area.y - 1) == false)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 根据Point获取其所占用的所有格子信息
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public List<MapGrid> GetMapGridDatas(Point point, Point area)
    {
        realTimeGridList.Clear();
        for (int i = 0; i < area.x; i++)
        {
            for (int j = 0; j < area.y; j++)
            {
                if (terrainMap.IsInBounds(point.x + i, point.y + j) == false)
                {
                    Debug.LogError("致命错误 找卢洋说明：GetMapGridDatas(Point point,Point area)  = 原因：调用者传入的point不在格子内 用法错误 传入的X：" + point.x + "--传入的Y：" + point.y);
                }
                realTimeGridList.Add(mapGrid[point.x + i, point.y + j]);
            }
        }
        return realTimeGridList;
    }

    /// <summary>
    /// 根据Point与区域 获取包含的所有格子信息
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public List<MapGrid> GetContainMapGridDatas(Point point, Point area)
    {
        realTimeGridList.Clear();
        for (int i = 0; i < area.x; i++)
        {
            for (int j = 0; j < area.y; j++)
            {
                if (terrainMap.IsInBounds(point.x + i, point.y + j) == true)
                {
                    realTimeGridList.Add(mapGrid[point.x + i, point.y + j]);
                }
            }
        }
        return realTimeGridList;
    }

    /// <summary>
    /// 根据数组下标获取到世界坐标
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Vector3 GetWorldPosByPoint(int x, int y)
    {
        return terrainMap.Coordinate2WorldPosition(x, y);
    }

    /// <summary>
    /// 根据Point获取世界坐标
    /// </summary>
    /// <param name="point"></param>
    public Vector3 GetWorldPosByPoint(Point point)
    {
        return terrainMap.Coordinate2WorldPosition(point)+new Vector3(0,0, (point.x + point.y) * 0.01f);
    }

    /// <summary>
    /// 根据Point获取世界坐标
    /// </summary>
    /// <param name="point"></param>
    public Vector3 GetWorldPosByPointCenter(Point point)
    {
        return terrainMap.Coordinate2WorldPosition(point) + new Vector3(0, terrainMap.IsoHeight / 2, 0)+new Vector3(0, 0, (point.x + point.y) * 0.01f);
    }

    /// <summary>
    /// 根据世界坐标获取Point
    /// </summary>
    /// <param name="v3"></param>
    /// <returns></returns>
    public Point GetPointByWorldPos(Vector3 v3)
    {
        return terrainMap.WorldPosition2Coordinate(v3);
    }

    /// <summary>
    /// 从MapGrid移除已有实体物体
    /// </summary>
    /// <param name="entity"></param>
    public void RemoveMapObjectEntity(MapObject entity)
    {
        entity.transform.localScale = Vector3.one;
        for (int i = 0; i < entity.StaticMapGridList.Count; i++)
        {
            Point point = entity.StaticMapGridList[i].point;
            entityRenderer.ChangeTileMap(point.x, point.y, null);
            entity.StaticMapGridList[i].Entity = null;
        }
        entity.StaticMapGridList.Clear();

        //RecycleEntityMapObject(entity);
    }

    /// <summary>
    /// 从MapGrid移除已有实体物体
    /// </summary>
    /// <param name="point"></param>
    public void RemoveMapObjectEntity(Point point)
    {
        if (terrainMap.IsInBounds(point.x, point.y) && mapGrid[point.x, point.y].Entity != null)
        {
            RemoveMapObjectEntity(mapGrid[point.x, point.y].Entity);
        }
        else
        {
            //Debug.Log("LY", "格子内并不包含这个需要移除的物体联系卢洋 RemoveMapObjectEntity(Point point)" + point.x + "--" + point.y);
        }
    }

    /// <summary>
    /// 从MapGrid移除已有实体物体
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void RemoveMapObjectEntity(int x, int y)
    {
        Point point = new Point(x, y);
        RemoveMapObjectEntity(point);
    }

    /// <summary>
    /// 取出MapGrid中的物体实例
    /// </summary>
    /// <param name="mapGrid"></param>
    public void TakeOutMapObjectEntity(MapGrid mapGrid)
    {
        TakeOutMapObjectEntity(mapGrid.Entity);
    }

    /// <summary>
    /// 取出MapGrid中的物体实例
    /// </summary>
    /// <param name="entity"></param>
    public void TakeOutMapObjectEntity(MapObject entity)
    {
        Point area = entity.Area;
        int x = entity.StaticPos.x;
        int y = entity.StaticPos.y;
        if (mapGrid[x, y] != null)
        {
            entityRenderer.SetSortingMethodOrder(mapGrid[x, y].Entity.gameObject, ComUtil.SortingLayerObjectEffect);
            for (int i = 0; i < area.x; i++)
            {
                for (int j = 0; j < area.y; j++)
                {
                    mapGrid[x + i, y + j].Entity = null;
                    entityRenderer.ChangeTileMap(x + i, y + j, null);
                }
            }
        }
        else
        {
            //Debug.Log("LY", "格子内并不包含这个需要移除的物体");
        }
    }

    /// <summary>
    /// 移动格子上的实体物体到新的格子
    /// </summary>
    public MapObject MoveMapObject(Point point, MapObject entity)
    {
        Vector3 startPostion = entity.transform.position;
        TakeOutMapObjectEntity(entity);
        SetMapObjectEntity(point, entity);
        //AnimMgr.PlayMoveAnimation(entity, startPostion, entity.transform.position);
        return entity;
    }

    /// <summary>
    ///摆放已有实体物体到MapGrid 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="entity"></param>
    public void SetMapObjectEntity(int x, int y, MapObject entity)
    {
        entity.transform.parent = null;
        Point point = new Point(x, y);
        SetMapObjectEntity(point, entity);
    }

    /// <summary>
    /// 摆放已有实体物体到MapGrid
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="point"></param>
    public MapObject SetMapObjectEntity(Point point, MapObject entity)
    {
        entity.transform.parent = null;
        entity.StaticMapGridList.Clear();
        Point area = entity.Area;
        int x = point.x;
        int y = point.y;

        if (terrainMap.IsInBounds(point.x, point.y) && mapGrid[point.x, point.y] != null)
        {
            for (int i = 0; i < area.x; i++)
            {
                for (int j = 0; j < area.y; j++)
                {
                    if (terrainMap.IsInBounds(x + i, y + j) == false)
                    {
                        Debug.Log("出现问题了");
                    }
                    mapGrid[x + i, y + j].Entity = entity;
                    entity.StaticMapGridList.Add(mapGrid[x + i, y + j]);
                    entityRenderer.ChangeTileMap(x + i, y + j, entity.gameObject);
                }
            }
        }
        else
        {
            //Debug.Log("LY", "发生了一些奇怪的事情 联系卢洋：SetMapObjectEntity(MapObject entity,Point point)");
        }
        entity.StaticPos = point;
        SetMapObjectToNormolPostion(entity, point);
        return entity;
    }

    public void SetMapObjectToNormolPostion(MapObject entity, Point point)
    {
        entity.transform.localPosition = terrainMap.Coordinate2WorldPosition(point.x, point.y);
        entityRenderer.ReOrder(point.x, point.y, entity.gameObject);
    }

    /// <summary>
    /// 添加新的实体物体到MapGrid
    /// </summary>
    /// <param name="point"></param>
    /// <param name="entityId"></param>
    public MapObject AddMapObjectEntity(Point point, int entityId)
    {
        if (terrainMap.IsInBounds(point.x, point.y) && mapGrid[point.x, point.y] != null)
        {
            SimpleTile simpleTile = ResMgr.Load<SimpleTile>(ComUtil.GetStringAdd("Tile", entityId));
            mapGrid[point.x, point.y].entityId = entityId;
            entityMap.SetTileAt(point.x, point.y, simpleTile, false);
            if (simpleTile == null)
            {
                Debug.LogError("需要制作预设以及simpleTile" + entityId);
            }
            //对格子上物体数据的初始化加载
            if (mapGrid[point.x, point.y].Entity.BasicData.detachGrid == false)
            {
                mapGrid[point.x, point.y].Entity.InitializeExtraGameData();
            }

            //if (GlobalVariable.GameState == GameState.LevelModel)
            //{
            //    ChapterModel.CheckGameSucceed();
            //}
            //RecordMapObjectWhenGameProcess(mapGrid[point.x, point.y].Entity);
            return mapGrid[point.x, point.y].Entity;
        }
        else
        {
            //Debug.Log("LY", "格子内并不包含这个需要移除的物体");
            return null;
        }
    }

    public MapObject AddMapObjectEntity(Point point, MapObject mapObject)
    {
        return AddMapObjectEntity(point, mapObject.Id);
    }

    /// <summary>
    /// 创建生命粒子寻路逻辑
    /// </summary>
    /// <param name="point"></param>
    /// <param name="worldPostion"></param>
    /// <param name="data"></param>
    /// <param name="material_type"></param>
    /// <param name="affectMaterialType"></param>
    //public static void CreatPurificationPowerLogic(Point point, Vector3 worldPostion, int id,
    //  MATERIAL_TYPE material_type, AFFECT_MATERIAL_TYPE affectMaterialType, PurificationPowerSourceType type, int powerValue)
    //{
        //bool isAttackMonster = true;
        //MapObject monster = GlobalVariable.FindMonsterAttack();
        //if (Random.Range(0, 100) > 30)
        //{
        //    isAttackMonster = fals/*e*/;
        //}
        //if (monster != null && isAttackMonster)
        //{
        //    //有龙 要对龙进行攻击！！！！
        //    MapObject obj = MapMgr.GetEntityMapObject(id);
        //    obj.transform.position = worldPostion;
        //    obj.PurificationPowerAttackMonster(monster, powerValue, type);
        //}
        //else
        //{
            //MapGrid grid = GlobalVariable.FindDeadLandGrid(point);
            //if (grid == null)
            //{
            //    if (monster != null)
            //    {
            //        //有龙 要对龙进行攻击！！！！
            //        MapObject obj = MapMgr.GetEntityMapObject(id);
            //        obj.transform.position = worldPostion;
            //        obj.PurificationPowerAttackMonster(monster, powerValue, type);
            //    }
            //    else
            //    {
            //        //能量转化为分数添加
            //        int score = TableDataMgr.GetSingleLifePowerData(id).value;
            //        PlayerModel.ChangePlayerDataSendServer(material_type, affectMaterialType, score);
            //        AnimMgr.PlayExpFirstLevelTip(score, worldPostion + new Vector3(0, 1.5f, 0));
            //        VFXMgr.PlayPurificationPowerToScoreVFX(worldPostion + new Vector3(0, 1.5f, 0));
            //    }
            //}
            //else
            //{

            //    //产生一个生命之力实体
            //    MapObject obj = MapMgr.GetEntityMapObject(id);
            //    obj.transform.position = worldPostion;
            //    obj.PurificationPowerMoveTo(grid, powerValue, type);
            //    VFXMgr.PlayTrailVFX(obj.entityTransform, SortingLayerDef.MAP_EFFECT);
            //}
        //}
    //}

    /// <summary>
    /// 产出一个物品
    /// </summary>
    /// <param name="worldPostion">产出起始位置</param>
    /// <param name="data">产出物数据</param>
    /// <param name="remainAmount">溢出金币或石砖的数量</param>
    /// <param name="type">净化之力来源</param>
    /// <returns></returns>
    //public bool OutPutOneMapObject(Vector3 worldPostion, MapObjectData data, int remainAmount = 0, PurificationPowerSourceType type = PurificationPowerSourceType.None)
    //{
    //    bool enterTrophyBall = false;
    //    DataConvert.currentAddMapObject = null;
    //    Point worldPoint = GetPointByWorldPos(worldPostion);
    //    if (data.detachGrid)
    //    {
    //        if (data.objectType == MapObject.OBJECT_TYPE_PURIFY_POWER)
    //        {
    //            LifePowerData da = TableDataMgr.GetSingleLifePowerData(data.id);
    //            CreatPurificationPowerLogic(worldPoint, worldPostion, data.id, MATERIAL_TYPE.MaterialExp, AFFECT_MATERIAL_TYPE.EcmtClickTheBallOfLife, type, da.value);
    //        }
    //        else
    //        {
    //            //TODO 云 ,僵尸等漂浮类物体
    //            MapObject mapObject = GetEntityMapObject(data.id);
    //            mapObject.name = "object" + data.id;
    //            mapObject.transform.position = worldPostion + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);

    //            ////向服务器发送增加龙
    //            //if (mapObject.IsMonster && GlobalVariable.GameState == GameState.MainSceneMode)
    //            //{
    //            //    NetMgr.CacheSend(NetAPIDef.eCTS_GAMESER_UPDATE_MONSTER, DataConvert.AddMonsterToServer(mapObject));
    //            //}
    //        }
    //    }
    //    else
    //    {
    //        List<MapGrid> mapGridList = new List<MapGrid>();
    //        if (GetPlacableGrids(worldPoint, data, null, ref mapGridList))
    //        {
    //            MapObject mapObject = AddMapObjectEntity(mapGridList[0].point, data.id);
    //            if (data.id == MapObject.OBJECT_ID_SPILLING_COIN || data.id == MapObject.OBJECT_ID_SPILLING_STONE)
    //            {
    //                mapObject.Remain_amount = remainAmount;
    //                //mapObject.ShowMapObjectTip();
    //            }

    //            VFXMgr.PlaySynthesisVFX(worldPostion);
    //            //AnimMgr.PlayMoveAnimation(mapObject, worldPostion, mapObject.transform.position);
    //            DataConvert.currentAddMapObject = mapObject;
    //        }
    //        else
    //        {
    //            enterTrophyBall = true;
    //        }
    //    }
    //    return enterTrophyBall;
    //}

    #region 添加一个气泡物体
    //地图被封锁的浮空物体在净化后添加进战利品球管理类；
    //public MapObject AddMapEntityToTrophyBall(Point point, int entityId, bool newAdd = true)
    //{
    //    //MapObject obj = GetEntityMapObject(entityId);
    //    //obj.StaticPos = point;
    //    //obj.transform.localPosition = terrainMap.Coordinate2WorldPosition(point.x, point.y);

    //    if (newAdd)
    //    {
    //        //MapObjectGameData data = DataConvert.ConverLocaCacheMapObjectData(obj);

    //        //if (GlobalVariable.GameState == GameState.MainSceneMode)
    //        //{
    //        //    TrophyBallMgr.AddBasecampTrophyBallData(data);
    //        //}
    //        //else if (GlobalVariable.GameState == GameState.LevelModel)
    //        //{
    //        //    TrophyBallMgr.AddChapterEmptyTrophyBallData(data, ChapterModel.CurrentChapterIndex);
    //        //}
    //    }
    //    return obj;
    //}

    //添加物体进入战利品球；
    //public MapObject AddTrophyBall(Point point, int entityId, bool newAdd = true)
    //{
    //    return AddTrophyBall(point, new List<int>() { entityId }, newAdd);
    //}

    //public MapObject AddTrophyBall(Point point, List<int> entityIdList, bool newAdd = true)
    //{
    //    int ballID = TrophyBallMgr.GetTrophyBallID(entityIdList);
    //    MapObject obj = GetEntityMapObject(ballID);
    //    obj.TrophyballInit(entityIdList);
    //    obj.transform.localPosition = terrainMap.Coordinate2WorldPosition(point.x, point.y);
    //    VFXMgr.PlayTrophyBall(obj.transform);

    //    if (newAdd)
    //    {
    //        //TrophyBallMgr.AddNewObj(entityIdList, ChapterModel.CurrentChapterIndex);
    //    }
    //    AdjustTrophyBallPosition(obj);
    //    return obj;
    //}

    //public MapObject AddTrophyBall(Vector3 position, int entityId, bool newAdd = true)
    //{
    //    return AddTrophyBall(GetPointByWorldPos(position), entityId, newAdd);
    //}

    //public MapObject AddTrophyBall(Vector3 position, MapObjectGameData gameData)
    //{
    //    Point point = GetPointByWorldPos(position);
    //    int ballID = TrophyBallMgr.GetTrophyBallID(gameData);
    //    MapObject obj = GetEntityMapObject(ballID);
    //    obj.TrophyBallInit(gameData);
    //    obj.transform.localPosition = terrainMap.Coordinate2WorldPosition(point.x, point.y);
    //    VFXMgr.PlayTrophyBall(obj.transform);
    //    AdjustTrophyBallPosition(obj);
    //    return obj;
    //}

    //public MapObject AddTrophyBallWithCachedPos(MapObjectGameData gameData, Vector3 cachedWorldPos)
    //{
    //    int ballID = TrophyBallMgr.GetTrophyBallID(gameData);
    //    MapObject obj = GetEntityMapObject(ballID);
    //    obj.TrophyBallInit(gameData);
    //    obj.transform.position = cachedWorldPos;
    //    VFXMgr.PlayTrophyBall(obj.transform);
    //    AdjustTrophyBallPosition(obj);
    //    return obj;
    //}

    /// <summary>已经初始化过数据的变成战利品球 </summary>
    public MapObject AddTrophyBallWithInitializedData(Vector3 postion, MapObject entityMapObject)
    {
        MapObject trophyBallMapObject = null;
        //if (GlobalVariable.GameState == GameState.MainSceneMode)
        //{
        //    int ballID = TrophyBallMgr.GetTrophyBallID(entityMapObject.Id);
        //    MapObjectGameData gameData = new MapObjectGameData();
        //    MapObjectGameData includeData = DataConvert.ConverLocaCacheMapObjectData(entityMapObject);
        //    gameData.object_list.Add(includeData);
        //    gameData.id = ballID;
        //    gameData.x = entityMapObject.InstantPos.x;
        //    gameData.y = entityMapObject.InstantPos.y;
        //    trophyBallMapObject = AddTrophyBall(postion, gameData);

        //    TrophyBallMgr.AddBasecampTrophyBallData(gameData);
        //}
        //else if (GlobalVariable.GameState == GameState.LevelModel)
        //{
        //    int ballID = TrophyBallMgr.GetTrophyBallID(entityMapObject.Id);
        //    MapObjectGameData gameData = new MapObjectGameData();
        //    MapObjectGameData includeData = DataConvert.ConverLocaCacheMapObjectData(entityMapObject);
        //    gameData.object_list.Add(includeData);
        //    gameData.id = ballID;
        //    gameData.x = entityMapObject.InstantPos.x;
        //    gameData.y = entityMapObject.InstantPos.y;
        //    trophyBallMapObject = AddTrophyBall(postion, gameData);
        //    TrophyBallMgr.AddChapterEmptyTrophyBallData(gameData, ChapterModel.CurrentChapterIndex);
        //}
        return trophyBallMapObject;
    }

    //商店 掠夺 关卡宝箱过来的数据值添加本地，服务器会自动添加购买的数据；
    public MapObject AddTrophyBallToLocal(Vector3 position, List<int> entityIdList)
    {
        MapObject obj = null;
        //if (GlobalVariable.GameState == GameState.MainSceneMode)
        //{
        //    int ballID = TrophyBallMgr.GetTrophyBallID(entityIdList);
        //    Point point = GetPointByWorldPos(position);
        //    obj = GetEntityMapObject(ballID);
        //    obj.TrophyballInit(entityIdList);
        //    obj.transform.localPosition = terrainMap.Coordinate2WorldPosition(point.x, point.y);
        //    VFXMgr.PlayTrophyBall(obj.transform);
        //}

        //TrophyBallMgr.AddNewObj(entityIdList, GlobalVariable.BASE_CAMP);
        //AdjustTrophyBallPosition(obj);
        return obj;
    }

    /// <summary>
    /// 特惠球变成战利品球
    /// </summary>
    /// <param name="position"></param>
    /// <param name="entityId"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    //public MapObject AddSpecialBallToTrophy(Vector3 position, int entityId, int key)
    //{
    //    MapObject obj = null;
    //    Point point = GetPointByWorldPos(position);
    //    int ballID = TrophyBallMgr.GetTrophyBallID(entityId);
    //    MapObjectGameData gameData = new MapObjectGameData();
    //    gameData.id = ballID;
    //    gameData.x = point.x;
    //    gameData.y = point.y;

    //    MapObjectGameData includeData = DataConvert.ConvetTrophyBallDataById(entityId, key);
    //    int[] liveTime = TableDataMgr.GetSingleMapObjectData(entityId).liveTime;
    //    includeData.status = (int)MAP_OBJECT_STATUS.Lock;
    //    includeData.dead_time = Random.Range(liveTime[0], liveTime[1]);
    //    includeData.delete_time = Random.Range(liveTime[0], liveTime[1]) + TimerMgr.GetSeconds();
    //    gameData.object_list.Add(includeData);

    //    //if (GlobalVariable.GameState == GameState.MainSceneMode)
    //    //{
    //    //    obj = GetEntityMapObject(ballID);
    //    //    obj.TrophyBallInit(gameData);
    //    //    obj.transform.localPosition = terrainMap.Coordinate2WorldPosition(point.x, point.y);
    //    //    VFXMgr.PlayTrophyBall(obj.transform);
    //    //}
    //    TrophyBallMgr.AddBasecampTrophyBallData(gameData);
    //    AdjustTrophyBallPosition(obj);
    //    return obj;
    //}

    /// <summary>
    /// 商店购买建筑地基时，空地不够，加到战利品球中或被挤入
    /// </summary>
    /// <param name="position"></param>
    /// <param name="entityId"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    //public MapObject AddShopFoudationToTrophy(Vector3 position, int entityId, int key)
    //{
    //    MapObject obj = null;
    //    Point point = GetPointByWorldPos(position);
    //    int ballID = TrophyBallMgr.GetTrophyBallID(entityId);
    //    MapObjectGameData gameData = new MapObjectGameData();
    //    gameData.id = ballID;
    //    gameData.x = point.x;
    //    gameData.y = point.y;

    //    MapObjectGameData includeData = DataConvert.ConvetTrophyBallDataById(entityId, key);
    //    gameData.object_list.Add(includeData);

    //    //if (GlobalVariable.GameState == GameState.MainSceneMode)
    //    //{
    //    //    obj = GetEntityMapObject(ballID);
    //    //    obj.TrophyBallInit(gameData);
    //    //    obj.transform.localPosition = terrainMap.Coordinate2WorldPosition(point.x, point.y);
    //    //    VFXMgr.PlayTrophyBall(obj.transform);
    //    //}
    //    TrophyBallMgr.AddBasecampTrophyBallData(gameData);
    //    AdjustTrophyBallPosition(obj);
    //    return obj;
    //}
    #endregion

    //public void AddEntityToMap(MapObjectData mapData)
    //{
    //    List<MapGrid> gridList = new List<MapGrid>();
    //    Vector3 targetPos = Vector3.zero;
    //    if (GetPlacableGrids(Point.zero, mapData, null, ref gridList))
    //    {
    //        AddMapObjectEntity(gridList[0].point, mapData.id);
    //        targetPos = MapMgr.Instance.GetWorldPosByPoint(gridList[0].point);
    //        CameraGestureMgr.Instance.MoveCamera(targetPos);
    //    }
    //    else
    //    {
    //        AddTrophyBall(Point.zero, mapData.id);
    //    }
    //}

    /// <summary>
    /// 检查是否可以移动到这个位置  并反馈占用的格子列表
    /// </summary>
    /// <param name="point"></param>
    /// <param name="area"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public bool IsLegalMovement(Point point, Point area, ref List<MapGrid> list)
    {
        //point
        //point.Up
        //point.Up.Up;

        for (int x = 0; x < area.x; x++)
        {
            for (int y = 0; y < area.y; y++)
            {
                if (terrainMap.IsInBounds(point.x + x, point.y + y) == false ||
                    mapGrid[point.x + x, point.y + y].Status == MapGridState.Locked
                    || mapGrid[point.x + x, point.y + y].Terrain == null)
                {
                    return false;
                }
                list.Add(mapGrid[point.x + x, point.y + y]);
            }
        }
        return true;
    }

    /// <summary>
    /// 检查是否可以移动到这个位置  并反馈占用的格子列表(优化处理边界问题)
    /// </summary>
    /// <param name="point"></param>
    /// <param name="area"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public bool IsLegalMovementOptimization(Point inputPoint, Point stayChange, Point area, ref List<MapGrid> list)
    {
        //point
        //point.Up
        //point.Up.Up;
        bool result = true;
        Point validationPoint = new Point(stayChange.x, stayChange.y);
        //只有边界优化
        list.Clear();
        if (inputPoint.x - stayChange.x > 0)
        {
            validationPoint.x = stayChange.x + 1;
        }
        else if (inputPoint.x - stayChange.x < 0)
        {
            validationPoint.x = stayChange.x - 1;
        }
        result = true;
        for (int x = 0; x < area.x; x++)
        {
            for (int y = 0; y < area.y; y++)
            {
                if (terrainMap.IsInBounds(validationPoint.x + x, validationPoint.y + y) == false ||
                    mapGrid[validationPoint.x + x, validationPoint.y + y].Status == MapGridState.Locked
                    || mapGrid[validationPoint.x + x, validationPoint.y + y].Terrain == null)
                {
                    result = false;
                    break;
                }
                list.Add(mapGrid[validationPoint.x + x, validationPoint.y + y]);
            }
        }

        if (result)
        {
            return result;
        }

        list.Clear();
        validationPoint = new Point(stayChange.x, stayChange.y);
        if (inputPoint.y - stayChange.y > 0)
        {
            validationPoint.y = stayChange.y + 1;
        }
        else if (inputPoint.y - stayChange.y < 0)
        {
            validationPoint.y = stayChange.y - 1;
        }

        result = true;
        for (int x = 0; x < area.x; x++)
        {
            for (int y = 0; y < area.y; y++)
            {
                if (terrainMap.IsInBounds(validationPoint.x + x, validationPoint.y + y) == false ||
                    mapGrid[validationPoint.x + x, validationPoint.y + y].Status == MapGridState.Locked
                    || mapGrid[validationPoint.x + x, validationPoint.y + y].Terrain == null)
                {
                    result = false;
                    break;
                }
                list.Add(mapGrid[validationPoint.x + x, validationPoint.y + y]);
            }
        }

        if (result)
        {
            return result;
        }
        return result;
    }

    /// <summary>
    /// 根据给出的位置以及占用大小，以及范围得到是否可以衍生出物体
    /// </summary>
    /// <param name="point">位置</param>
    /// <param name="selfArea">衍生主体所占用的尺寸</param>
    /// <param name="area">生成的尺寸</param>
    /// <param name="radius">范围</param>
    /// <param name="mapList">反馈的结果</param>
    /// <returns></returns>
    public bool GetDerivativedArea(Point point, int[] selfArea, int[] area, int radius, ref List<MapGrid> mapList)
    {
        placedGridList.Clear();

        SpiralOrder(mapGrid, point, selfArea, mapWidth, mapHeight, radius);
        foreach (MapGrid map in placedGridList)
        {
            mapList.Clear();
            bool hasEntity = false;
            int x = map.point.x;
            int y = map.point.y;

            for (int i = 0; i < area[0]; i++)
            {
                for (int j = 0; j < area[1]; j++)
                {
                    if (terrainMap.IsInBounds(x + i, y + j) == false ||
                        mapGrid[x + i, y + j].Terrain == null ||
                        mapGrid[x + i, y + j].Entity != null ||
                        mapGrid[x + i, y + j].Status != MapGridState.UnlockAndCured ||
                        placedGridList.Contains(mapGrid[x + i, y + j]) == false)
                    {
                        hasEntity = true;
                        break;
                    }
                    mapList.Add(mapGrid[x + i, y + j]);
                }
            }
            if (hasEntity == false)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 根据给出的坐标点 以及 占用的大小 得到最近的一个可放置的格子列表
    /// </summary>
    /// <param name="point"></param>
    /// <param name="area"></param>
    /// <returns></returns>
    //public bool GetPlacableGrids(Point point, MapObjectData gridData, List<MapGrid> ignoreList, ref List<MapGrid> mapList, bool toTheSame = false)
    //{
    //    if (IsInBouds(point) == false)
    //    {
    //        point = new Point(0, 0);
    //    }
    //    int[] area = gridData.area;
    //    placedGridList.Clear();

    //    if (toTheSame)
    //    {
    //        bool isSameId = false;
    //        if (entityMapObjectDic.ContainsKey(gridData.id))
    //        {
    //            //从物品缓存字典内获取到相同物品的列表
    //            List<MapObject> sameItemList = entityMapObjectDic[gridData.id];
    //            //判断  这个ID是否存在在物品列表中  并且这个列表的长度不为0
    //            if (sameItemList.Count > 0)
    //            {
    //                Point placePoint = new Point(0, 0);
    //                foreach (MapObject obj in sameItemList)
    //                {
    //                    if (obj.StaticMapGridList[0].Status == MapGridState.UnlockAndCured)
    //                    {
    //                        if (placePoint.x == 0 && placePoint.y == 0)
    //                        {
    //                            placePoint = obj.StaticMapGridList[0].point;
    //                        }
                        
    //                        //记录的最近放置点 到 采集点的距离
    //                        float minPlaceDistance = Vector3.Distance(GetWorldPosByPoint(placePoint), GetWorldPosByPoint(point));
    //                        //当前物体 到 采集点的距离
    //                        float currentPlaceDistance = Vector3.Distance(GetWorldPosByPoint(point), GetWorldPosByPoint(obj.StaticMapGridList[0].point));

    //                        if (minPlaceDistance > currentPlaceDistance)
    //                        {
    //                            placePoint = obj.StaticMapGridList[0].point;
    //                        }
    //                        isSameId = true;
    //                    }
    //                }
    //                point = placePoint;
    //            }
    //        }

    //        if (isSameId == false)
    //        {
    //            MapObject sameTypeEntity = GetMapObjectEntityByType(gridData.objectType);
    //            if (sameTypeEntity != null)
    //            {
    //                point = sameTypeEntity.StaticMapGridList[0].point;
    //            }
    //        }
    //    }

    //    SpiralOrder(mapGrid, point, area, mapWidth, mapHeight);

    //    foreach (MapGrid map in placedGridList)
    //    {
    //        mapList.Clear();
    //        bool hasEntity = false;
    //        int x = map.point.x;
    //        int y = map.point.y;

    //        for (int i = 0; i < area[0]; i++)
    //        {
    //            for (int j = 0; j < area[1]; j++)
    //            {
    //                if (terrainMap.IsInBounds(x + i, y + j) == false ||
    //                    mapGrid[x + i, y + j].Terrain == null ||
    //                    mapGrid[x + i, y + j].Entity != null ||
    //                    mapGrid[x + i, y + j].Status != MapGridState.UnlockAndCured)
    //                {
    //                    hasEntity = true;
    //                    break;
    //                }

    //                if (ignoreList != null)
    //                {
    //                    foreach (MapGrid data in ignoreList)
    //                    {
    //                        if (data.point == mapGrid[x + i, y + j].point)
    //                        {
    //                            hasEntity = true;
    //                            break;
    //                        }
    //                    }
    //                }

    //                mapList.Add(mapGrid[x + i, y + j]);
    //            }
    //        }
    //        if (hasEntity == false)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //public bool GetPlacableGrids(Vector3 targetPos, MapObjectData gridData, List<MapGrid> ignoreList, ref List<MapGrid> mapList, bool toTheSame = false)
    //{
    //    Point targetPoint = GetPointByWorldPos(targetPos);
    //    return GetPlacableGrids(targetPoint, gridData, ignoreList, ref mapList, toTheSame);
    //}

    /// <summary>
    /// 获取最近的三块死地
    /// </summary>
    /// <param name="point"></param>
    /// <param name="mapList"></param>
    /// <returns></returns>
    public List<MapGrid> GetUnLockButDeadMapGrids(Point point)
    {
        List<MapGrid> gridList = new List<MapGrid>();
        placedGridList.Clear();
        SpiralOrder(mapGrid, point, new int[] { 1, 1 }, mapWidth, mapHeight);

        foreach (MapGrid map in placedGridList)
        {
            if (map.Status == MapGridState.UnlockButDead)
            {
                gridList.Add(map);
            }
            if (gridList.Count >= 3)
            {
                break;
            }
        }
        return gridList;
    }

    /////是否存在空地块
    //public bool HasEmptyPlot()
    //{
    //    List<MapGrid> mapGridList = new List<MapGrid>();
    //    MapObjectData data = TableDataMgr.GetSingleMapObjectData(10108);
    //    return GetPlacableGrids(Vector3.zero, data, null, ref mapGridList);
    //}

    public static int GetEmptyPlotCount()
    {
        int count = 0;
        foreach (var item in Instance.mapGrid)
        {
            if (item.Status == MapGridState.UnlockAndCured)
            {
                if (item.Entity == null)
                {
                    count++;
                }
            }
        }
        return count;
    }

    /// <summary>
    /// 没有精华掉格子
    /// </summary>
    /// <param name="info"></param>
    public static void SetPurificationPowerGridCureValueChangeList(UpdateTerrainInfo info)
    {
        if (purification_GridCureValueChangeList.ContainsKey(info.point))
        {
            purification_GridCureValueChangeList[info.point] = info;
        }
        else
        {
            purification_GridCureValueChangeList.Add(info.point, info);
        }
    }

    /// <summary>
    /// 将净化粒子添加到净化粒子列表中
    /// </summary>
    /// <param name="obj"></param>
    public static void AddPurification(MapObject obj)
    {
        if (purificationList.Contains(obj) == false)
        {
            purificationList.Add(obj);
        }
        else
        {
            Debug.LogError("严重错误：---->>>>>  精华粒子已经在列表了 不能进行二次添加！！！");
        }
    }

    /// <summary>
    /// 从净化粒子列表内移除已经处理完毕的粒子
    /// </summary>
    /// <param name="obj"></param>
    public static void RemovePurification(MapObject obj)
    {
        if (purificationList.Contains(obj))
        {
            purificationList.Remove(obj);
        }
        else
        {
            Debug.LogError("严重错误：---->>>>>  精华粒子不存在于列表中 不能进行删除！！！");
        }
    }

    public static void ClearPurificationList()
    {
        for (int i = 0; i < purificationList.Count; i++)
        {
            //purificationList[i].PurificationMoveKill();
            i--;
        }
    }

    /// <summary>
    /// 检查传入的point是否在格子范围内
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsInBouds(Point point)
    {
        return (point.x >= 0 && point.x < mapWidth && point.y >= 0 && point.y < mapHeight);
    }

    /// <summary>
    /// 存储自动生成的净化之力
    /// </summary>
    /// <param name="index"></param>
    /// <param name="obj"></param>
    public static void SetPurificationPowerAutomaticCureGird(int index, MapObject obj)
    {
        if (purification_Automatic_GridCureValueChangeList.ContainsKey(index) == false)
        {
            purification_Automatic_GridCureValueChangeList.Add(index, obj);
        }
        else
        {
            Debug.LogError("错误：同一物体进行了多次添加");
        }
    }
    /// <summary>
    /// 移除自动生成的净化之力
    /// </summary>
    /// <param name="index"></param>
    /// <param name="obj"></param>
    public static void RemovePurificationPowerAutomaticCureGird(int index)
    {
        if (purification_Automatic_GridCureValueChangeList.ContainsKey(index))
        {
            purification_Automatic_GridCureValueChangeList.Remove(index);
        }
        else
        {
            Debug.LogError("错误：同一物体进行了多次添加");
        }
    }
    /// <summary>
    /// 净化掉格子
    /// </summary>
    /// <param name="info"></param>
    public static void SetPurificationPowerGridAllChangeList(UpdateTerrainInfo info)
    {
        if (purification_GridAllChangeList.ContainsKey(info.point))
        {
            purification_GridAllChangeList[info.point] = info;
        }
        else
        {
            purification_GridAllChangeList.Add(info.point, info);
            if (purification_GridCureValueChangeList.ContainsKey(info.point))
            {
                purification_GridCureValueChangeList.Remove(info.point);
            }
        }
        SendGridStatusChange(UpdateTerrainEventType.AllChange, purification_GridAllChangeList);
        purification_GridAllChangeList.Clear();
    }

    public static void SendGridStatusChange(UpdateTerrainEventType type, Dictionary<Point, UpdateTerrainInfo> infos)
    {
        if (infos.Count == 0)
        {
            return;
        }

        //if (GlobalVariable.GameState == GameState.MainSceneMode)
        //{
        //    Update_terrain_event sendData = new Update_terrain_event();
        //    foreach (UpdateTerrainInfo updataInfo in infos.Values)
        //    {
        //        Update_terrain_info info = new Update_terrain_info();
        //        //info.GridIndex = GetPointAndPintMappingRelation(updataInfo.point);
        //        Coord coord = new Coord();
        //        coord.X = (uint)updataInfo.point.x;
        //        coord.Y = (uint)updataInfo.point.y;
        //        info.Coord = coord;
        //        Map_object_data obj = new Map_object_data();

        //        info.Type = (uint)type;
        //        switch (type)
        //        {
        //            case UpdateTerrainEventType.None:
        //                break;
        //            case UpdateTerrainEventType.StatusChange:
        //                info.State = updataInfo.state;
        //                break;
        //            case UpdateTerrainEventType.DeadLevelChange:
        //                info.DeadLevel = (uint)updataInfo.dead_level;
        //                break;
        //            case UpdateTerrainEventType.StatusAndDeadLevelChange:
        //                info.State = updataInfo.state;
        //                info.DeadLevel = (uint)updataInfo.dead_level;
        //                break;
        //            case UpdateTerrainEventType.CureCountChange:
        //                info.CureCount = (uint)updataInfo.cure_count;
        //                break;
        //            case UpdateTerrainEventType.DeadLevelAndCureCountChange:
        //                info.DeadLevel = (uint)updataInfo.dead_level;
        //                info.CureCount = (uint)updataInfo.cure_count;
        //                break;
        //            case UpdateTerrainEventType.AllChange:
        //                info.State = updataInfo.state;
        //                info.CureCount = (uint)updataInfo.cure_count;
        //                info.DeadLevel = (uint)updataInfo.dead_level;
        //                break;
        //        }

        //        if (updataInfo.entity != null && updataInfo.state == Grid_state.UnlockAndCured)
        //        {
        //            info.RemoveFlag = true;

        //            obj.Id = (uint)updataInfo.entity.BasicData.id; //TrophyBallMgr.GetTrophyBallID(Instance.GetMapGridData(updataInfo.point).Entity.Id);

        //            sendData.Object.Add(DataConvert.GetMapObjectData(updataInfo.entity));
        //        }

        //        //如果这次数据里有净化就把经验添加上
        //        if (updataInfo.exp > 0)
        //        {
        //            PlayerModel.ChangePlayerDataSendServer(MATERIAL_TYPE.MaterialExp, AFFECT_MATERIAL_TYPE.EcmtPurifyTheDeadGround, updataInfo.exp);
        //        }

        //        sendData.List.Add(info);
        //    }

        //    NetMgr.CacheSend(NetAPIDef.eCTS_GAMESER_UPDATE_TERRAIN_STATE, sendData);
        //}
        //else if (GlobalVariable.GameState == GameState.LevelModel)
        //{
        //    //TODO:关卡内的交互协议
        //}
    }

    #region 整理功能
    public static void SortOutAllGrid()
    {
        //第一步 ：获取到所有已经解锁了的格子 存储格式为 Point mapobject  加入列表后进行takeout处理  
        List<MapObject> unlockAndCureEntityList = new List<MapObject>();
        foreach (MapGrid grid in Instance.mapGrid) 
        { 
			if(grid.Status == MapGridState.UnlockAndCured && grid.Entity != null && 
				grid.Entity.BasicData.canDrag && grid.Entity.BasicData.objectType != 301) 
            {
                //DataConvert.removeList.Add(DataConvert.GetCoord(grid.Entity.StaticPos));
                unlockAndCureEntityList.Add(grid.Entity);
				Debug.Log(grid.Entity.name);
                Instance.TakeOutMapObjectEntity(grid);
            }
        }

        //第二步 ：进行排序
        unlockAndCureEntityList.Sort((x, y) => x.BasicData.id.CompareTo(y.BasicData.id));

        //int lastId = 0;

        //第三步 ：遍历所有格子 依照次序向内摆放物体 并进行服务器发送记录。发送服务器添加物体操作。
    //    foreach (MapGrid grid in Instance.mapGrid)
    //    {
    //        if (grid.Status == MapGridState.UnlockAndCured && grid.Entity == null && unlockAndCureEntityList.Count > 0)
    //        {
				//List<MapGrid> placableList = new List<MapGrid>();
				//if (Instance.GetPlacableGrids(grid.point, unlockAndCureEntityList[0].BasicData, null, ref placableList))
				//{
				//	Instance.SetMapObjectEntity(grid.point, unlockAndCureEntityList[0]);
				//	unlockAndCureEntityList.RemoveAt(0);
				//	DataConvert.addList.Add(grid.Entity);
				//	lastId = grid.Entity.BasicData.id;

				//	if (unlockAndCureEntityList.Count > 0 && lastId == unlockAndCureEntityList[0].BasicData.id)
				//	{
				//		for (int i = 0; i < unlockAndCureEntityList.Count; i++)
				//		{
				//			if (lastId == unlockAndCureEntityList[0].BasicData.id)
				//			{
				//				placableList.Clear();
				//				if (Instance.GetPlacableGrids(grid.point, unlockAndCureEntityList[i].BasicData, null, ref placableList))
				//				{
				//					Instance.SetMapObjectEntity(placableList[0].point, unlockAndCureEntityList[i]);
				//					unlockAndCureEntityList.Remove(unlockAndCureEntityList[i]);
				//					DataConvert.addList.Add(placableList[0].Entity);
				//					i--;
				//				}
				//			}
				//			else
				//			{
				//				break;
				//			}
				//		}
				//	}
				//}
    //        }
    //    }

		//第三步 ：遍历所有格子 依照次序向内摆放物体 并进行服务器发送记录。发送服务器添加物体操作。
		//foreach (MapGrid grid in Instance.mapGrid)
		//{
		//	if (grid.Status == MapGridState.UnlockAndCured && grid.Entity == null && unlockAndCureEntityList.Count > 0)
		//	{
		//		List<MapGrid> placableList = new List<MapGrid>();
		//		if (Instance.GetPlacableGrids(grid.point, unlockAndCureEntityList[0].BasicData, null, ref placableList))
		//		{
		//			Instance.SetMapObjectEntity(grid.point, unlockAndCureEntityList[0]);
		//			unlockAndCureEntityList.RemoveAt(0);
		//			DataConvert.addList.Add(grid.Entity);
		//			lastId = grid.Entity.BasicData.id;

		//			if (unlockAndCureEntityList.Count > 0 && lastId == unlockAndCureEntityList[0].BasicData.id)
		//			{
		//				for (int i = 0; i < unlockAndCureEntityList.Count; i++)
		//				{
		//					if (lastId == unlockAndCureEntityList[0].BasicData.id)
		//					{
		//						placableList.Clear();
		//						if (Instance.GetPlacableGrids(grid.point, unlockAndCureEntityList[i].BasicData, null, ref placableList))
		//						{
		//							Instance.SetMapObjectEntity(placableList[0].point, unlockAndCureEntityList[i]);
		//							unlockAndCureEntityList.Remove(unlockAndCureEntityList[i]);
		//							DataConvert.addList.Add(placableList[0].Entity);
		//							i--;
		//						}
		//					}
		//					else
		//					{
		//						break;
		//					}
		//				}
		//			}
		//		}
		//	}
		//}
		//DataConvert.ConverUpdateObjectToSendServer(Object_act_type.Move);
    }
    #endregion

    #region 螺旋矩阵 点到外
    private void SpiralOrder(MapGrid[,] map, Point point, int[] area, int width, int height, int radius = -1)
    {
        int totalCount = radius;
        if (radius < 0)
        {
            //totalCount = width - point.x > height - point.y ? width - point.x : height - point.y;
            totalCount = width > height ? width : height;
        }

        int curCount = 0;
        if (terrainMap.IsInBounds(point.x, point.y))
        {
            placedGridList.Add(map[point.x, point.y]);
        }
        while (curCount < totalCount)
        {
            //初始的一次左
            point = point.Left;
            if (IsInBouds(point, width, height) && mapGrid[point.x, point.y].sealLock == null)
            {
                placedGridList.Add(map[point.x, point.y]);
            }
            //初始的1次上
            for (int i = 0; i < (area[1] - 1) + 1 + 2 * curCount; i++)
            {
                point = point.Up;
                if (IsInBouds(point, width, height) && mapGrid[point.x, point.y].sealLock == null)
                {
                    placedGridList.Add(map[point.x, point.y]);
                }
            }
            //初始的两次右
            for (int i = 0; i < (area[0] - 1) + 2 + 2 * curCount; i++)
            {
                point = point.Right;
                if (IsInBouds(point, width, height) && mapGrid[point.x, point.y].sealLock == null)
                {
                    placedGridList.Add(map[point.x, point.y]);
                }
            }
            //初始的两次下
            for (int i = 0; i < (area[1] - 1) + 2 + 2 * curCount; i++)
            {
                point = point.Down;
                if (IsInBouds(point, width, height) && mapGrid[point.x, point.y].sealLock == null)
                {
                    placedGridList.Add(map[point.x, point.y]);
                }
            }
            //初始的两次左
            for (int i = 0; i < (area[0] - 1) + 2 + 2 * curCount; i++)
            {
                point = point.Left;
                if (IsInBouds(point, width, height) && mapGrid[point.x, point.y].sealLock == null)
                {
                    placedGridList.Add(map[point.x, point.y]);
                }
            }
            curCount++;
        }
    }

    private bool IsInBouds(Point point, int mapWidth, int mapHeight)
    {
        return (point.x >= 0 && point.x < mapWidth && point.y >= 0 && point.y < mapHeight);
    }
    #endregion

    #region 螺旋矩阵 外到内
    private static List<int> SpiralOrder(Point point, int[] area, int[,] map, int right)
    {
        List<int> list = new List<int>();
        if (map == null || map.Length == 0) return list;
        int top = 0, left = 0;
        int bottom = 8;

        while (top <= bottom && left <= right)
        {
            OutEdge(list, map, left++, top++, right--, bottom--);
        }
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log(list[i]);
        }
        Debug.Log(list.Count);
        return list;
    }
    private static void OutEdge(List<int> res, int[,] matrix, int left, int top, int right, int bottom)
    {
        if (top == bottom)
        {
            for (int i = left; i <= right; i++)
            {
                res.Add(top + i * 9);
            }
        }
        else if (left == right)
        {
            for (int i = top; i <= bottom; i++)
            {
                res.Add(i + left * 9);
            }
        }
        else
        {
            int curRow = top;
            int curCol = left;
            while (curCol < right)
            {
                res.Add(top + curCol++ * 9);
            }
            while (curRow < bottom)
            {
                res.Add((curRow++) + right * 9);
            }
            while (curCol > left)
            {
                res.Add(bottom + curCol-- * 9);
            }
            while (curRow > top)
            {
                res.Add((curRow--) + left * 9);
            }
        }
    }
    #endregion
}

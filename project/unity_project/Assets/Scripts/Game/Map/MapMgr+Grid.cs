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
                if (terrainMap.IsInBounds(point.x + x, point.y + y) == false)
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
                if (terrainMap.IsInBounds(validationPoint.x + x, validationPoint.y + y) == false)
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
                if (terrainMap.IsInBounds(validationPoint.x + x, validationPoint.y + y) == false)
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



    #region 螺旋矩阵 点到外
  
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

using GameProto;
using Google.Protobuf;
using SimpleJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPoolData
{
    public string name;
    public Type type;
}

public partial class MapMgr : MonoBehaviour
{
    public static Action Event_CostDiamondTotal;
    public const float MAP_GRID_WIDTH = 3;
    public const float MAP_GRID_HEIGHT = 1.5f;
    //public static MonoBehaviourPool<ClickToCollectTip> clickToCollectTipPool;
    public static UnityEngineObjectPool<SpriteRenderer> doubleClickSightTipPool;
    public static Dictionary<int, int> clickToCollectTipDic = new Dictionary<int, int>();
    private static int costDiamondTotal = 0;
    public static int CostDiamondTotal
    {
        get
        {
            return costDiamondTotal;
        }
        set
        {
            costDiamondTotal = value;
           if(Event_CostDiamondTotal != null)
            {
                Event_CostDiamondTotal();
            }
        }
    }

    public static UnityEngineObjectPool<SpriteRenderer> DoubleClickSightTipPool
    {
        get
        {
            if (doubleClickSightTipPool == null)
            {
                SpriteRenderer sp = ResMgr.Load<SpriteRenderer>("DoubleClicSight");
                doubleClickSightTipPool = new UnityEngineObjectPool<SpriteRenderer>(sp);
            }
            return doubleClickSightTipPool;
        }
    }
    public static MapMgr Instance;
    //private static Transform dragonPowerPoolParent;
    /// <summary>
    /// 从服务器或者缓存数据中拉取时获取生物的唯一Id从1开始
    /// </summary>
    public static int monsterOnlyStartId = 1;

    private void Awake()
    {
        Instance = this;
        //PlayerModel.Data.Event_DiamondChange += DiamondChange;
        UIMgr.Event_UIShow += EventUIShow;
        UIMgr.Event_UIHide += EventUIHide;
        //ChapterModel.Event_ChapterStateChangeNotice += EventChapterStateChangeNotice;
        //InitializeMonsterRlate();
        //dragonPowerPoolParent = CommonObjectMgr.DragonPowerParent;
        ResMgr.Event_LocalResInitFinish += InitRes;
        InitClickToCollectDic();
    }

    private void DiamondChange(int obj)
    {
        //if (obj < 0 && GlobalVariable.GameState == GameState.MainSceneMode)
        //{
        //    CostDiamondTotal -= obj;
        //}
    }

    private void Update()
    {
        //SeaEffectUpdate();
        //if (delayTriggerControlMonsterLimit)
        //{
        //    delayTriggerControlMonsterLimit = false;
        //    //ControlMonsterLimit();
        //}
        //MonsterRelatedUpdate();
        //TaskModel.Update();
	}

	/// <summary>
	/// 根据 服务器的日志log 恢复玩家数据
	/// </summary>
	private void RestorePlayerDataByServerUpdataMapobjectLog()
	{
		//第一步 ：获取到所有已经解锁了的格子 存储格式为 Point mapobject  加入列表后进行takeout处理  
		List<MapObject> unlockAndCureEntityList = new List<MapObject>();
		foreach (MapGrid grid in Instance.mapGrid)
		{
			if (grid.Status == MapGridState.UnlockAndCured && grid.Entity != null && grid.Entity.BasicData.canDrag)
			{
				//DataConvert.removeList.Add(DataConvert.GetCoord(grid.Entity.StaticPos));
				unlockAndCureEntityList.Add(grid.Entity);
				Instance.TakeOutMapObjectEntity(grid);
			}
		}

		//DataConvert.ConverUpdateObjectToSendServer(Object_act_type.Delete);

		//Debug.Log(GlobalVariable.ServerUpdateMapObjectLogData);
		//string str = GlobalVariable.ServerUpdateMapObjectLogData.Replace("add_object", "*");
		//string[] sr = str.Split('*');
		//int i = 0;

		//Update_object_event ev = new Update_object_event();
		//foreach(string s in sr)
		//{
		//	i++;
		//	string addObjectLog = s;
		//	if (i > 1)
		//	{	
		//		Add_object_info info = new Add_object_info();

		//		Coord coord = GetCoord(ref addObjectLog);
		//		Debug.Log(addObjectLog);
		//		Map_object_data data = GetMapObjectDataItem(ref addObjectLog);
		//		info.Coord = coord;
		//		info.Object = data;

		//		ev.AddObject.Add(info);
		//		Debug.Log(data.ToString());
		//	}
		//}
		//NetMgr.CacheSend(NetAPIDef.eCTS_GAMESER_UPDATE_OBJECT, ev);
	}

	//分解update——mapobject 里的 object——info
	private Map_object_data GetMapObjectDataItem(ref string updateObjeLog)
	{
		Map_object_data objectData = new Map_object_data();
		updateObjeLog = updateObjeLog.Replace(" ", "");
		updateObjeLog = updateObjeLog.Replace("}", "");
		string[] objData = updateObjeLog.Split(new String[16] {"{id:", "name:", "hp:", "left_tap_count:",
			"tap_max_mount:", "left_collect_count:", "collect_max_count:", "left_spawn_count:",
			"spawn_max_count:", "remain_amount:", "shop_id:", "include_objects:", "building_data",
			"status:", "delete_time:", "dead_time:",
		}, StringSplitOptions.RemoveEmptyEntries);
			
		List<string> allObjectDatalist = new List<string>(objData);
		for (int i = 0; i < allObjectDatalist.Count; i++)
		{
			if (string.IsNullOrEmpty(allObjectDatalist[i]))
			{
				allObjectDatalist.RemoveAt(i);
				i--;
			}
		}

		Debug.Log(allObjectDatalist.Count);
		if (updateObjeLog.Contains("id"))
		{
			objectData.Id = uint.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("name"))
		{
			Debug.Log(allObjectDatalist[0] + "---- name");
			objectData.Name = allObjectDatalist[0];
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("hp"))
		{
			objectData.Hp = uint.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("left_tap_count"))
		{
			objectData.LeftTapCount = int.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("tap_max_mount"))
		{
			objectData.TapMaxMount = int.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("left_collect_count"))
		{
			objectData.LeftCollectCount = int.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("collect_max_count"))
		{
			objectData.CollectMaxCount = int.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("left_spawn_count"))
		{
			objectData.LeftSpawnCount = int.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("spawn_max_count"))
		{
			objectData.SpawnMaxCount = int.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("remain_amount"))
		{
			objectData.RemainAmount = uint.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("shop_id"))
		{
			objectData.ShopId = uint.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("include_objects"))
		{
			

		}
		if (updateObjeLog.Contains("building_data"))
		{
			Debug.Log(allObjectDatalist[0]);
			objectData.BuildingData = GetResting_BuildingData(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("status"))
		{
			switch(updateObjeLog)
			{
			case "MAP_OBJECT_STATUS_NULL":
				objectData.Status = MAP_OBJECT_STATUS.Null;
				break;
			case "MAP_OBJECT_STATUS_NORMAl":
				objectData.Status = MAP_OBJECT_STATUS.Normal;
				break;
			case "MAP_OBJECT_STATUS_LOCK":
				objectData.Status = MAP_OBJECT_STATUS.Lock;
				break;
			}
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("delete_time"))
		{
			Debug.Log(allObjectDatalist[0]);
			objectData.DeleteTime = ulong.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}
		if (updateObjeLog.Contains("dead_time"))
		{
			objectData.DeadTime = uint.Parse(allObjectDatalist[0]);
			allObjectDatalist.RemoveAt(0);
		}

		return objectData;
	}

	//分解 mapobje-info 里的 building-data
	private Resting_building_data GetResting_BuildingData(string s)
	{
		Resting_building_data buildingDate = new Resting_building_data();
		s = s.Replace(" ", "");
		string[] objData = s.Split(new String[4] { "{", "timestamp:", "monster_list", "}",
		}, StringSplitOptions.RemoveEmptyEntries);

		List<string> monsterIdinfolist = new List<string>(objData);
		for (int i = 0; i < monsterIdinfolist.Count; i++)
		{
			if (string.IsNullOrEmpty(monsterIdinfolist[i]))
			{
				monsterIdinfolist.RemoveAt(i);
				i--;
			}
		}

		if (monsterIdinfolist.Count > 0)
		{
			for (int i = 0; i < monsterIdinfolist.Count; i++)
			{
				if (i == 0)
				{
					buildingDate.Timestamp = long.Parse(monsterIdinfolist[0]);
				}
				else
				{
					string[] li = monsterIdinfolist[i].Split(new String[2] {"only_id:", "monster_id:",
					}, StringSplitOptions.RemoveEmptyEntries);

					Debug.Log(li[0] + "--" + li[1]);
					Monster_identity_data monster = new Monster_identity_data();

					monster.OnlyId = uint.Parse(li[0]);
					monster.MonsterId = uint.Parse(li[1]);

					buildingDate.MonsterList.Add(monster);
					Debug.Log(monsterIdinfolist[i]  + " 龙的数据");
				}
			}
		}
		return buildingDate;
	} 

	private Coord GetCoord(ref string s)
	{
		Debug.Log(s);
		Coord coord = new Coord();	
		if (s.Contains("coord"))
		{
			string[] coordStr = s.Split(new String[2]{ "coord", "object" }, StringSplitOptions.RemoveEmptyEntries);
			coordStr[1] = coordStr[1].Replace(" ", "");
			string[] xyStr = coordStr[1].Split(new String[4]{"{", "x:", "y:","}"}, StringSplitOptions.RemoveEmptyEntries);

			List<uint> list = new List<uint>();
			foreach(string xy in xyStr)
			{
				if (string.IsNullOrEmpty(xy) == false)
				{
					Debug.Log(xy + "---");
					list.Add(uint.Parse(xy));
				}
			}

			if (coordStr[1].Contains("x"))
			{
				coord.X = list[0];
				if (coordStr[1].Contains("y"))
				{
					coord.Y = list[1];
				}
				else
				{	
					coord.Y = 0;
				}
			}
			else
			{
				coord.X = 0;
				if (coordStr[1].Contains("y"))
				{
					coord.Y = list[0];
				}
				else
				{	
					coord.Y = 0;
				}
			}
			s = coordStr[2];
			Debug.Log(coord.X + "----" + coord.Y);
		}
		return coord;
	}

    //private void EventChapterStateChangeNotice(ChapterState type)
    //{
    //    switch (type)
    //    {
    //        //case ChapterState.ChapterPause:
    //        //    AllEntityTimerPause();
    //        //    break;
    //        //case ChapterState.ChapterRun:
    //        //    AllEntityTimerRecovery();
    //        //    break;
    //        //case ChapterState.InChapterScene:
    //        //    //Debug.Log("--");
    //        //    break;
    //        //case ChapterState.ChallengeCountDown:
    //        //    //Debug.Log("--");
    //        //    break;
    //        //case ChapterState.ChapterStart:
    //        //    //Debug.Log("--");
    //        //    break;
    //    }
    //}

    private void AllEntityTimerPause()
    {
        //Debug.Log("LY","暂停所有物体的计时器");
        foreach (MapGrid grid in mapGrid)
        {
            if (grid.Terrain != null)
            {
                grid.Terrain.MapObejctTimeout();
            }
        }

        //foreach (List<MapObject> list in entityMapObjectDic.Values)
        //{
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        list[i].MapObejctTimeout();
        //    }
        //}

        //foreach (List<MapObject> list in noSaveEntityMapObjectDic.Values)
        //{
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        list[i].MapObejctTimeout();
        //    }
        //}

        //LeafMgr.Instance.Pause = true;
    }

    private void AllEntityTimerRecovery()
    {
        //Debug.Log("LY","开始进行所有物体的计时器");
        foreach (MapGrid grid in mapGrid)
        {
            if (grid.Terrain != null)
            {
                grid.Terrain.MapObejctTimeRecovery();
            }
        }

        //foreach (List<MapObject> list in entityMapObjectDic.Values)
        //{
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        list[i].MapObejctTimeRecovery();
        //    }
        //}

        //foreach (List<MapObject> list in noSaveEntityMapObjectDic.Values)
        //{
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        list[i].MapObejctTimeRecovery();
        //    }
        //}
        //LeafMgr.Instance.Pause = false;
    }

    private void EventUIHide(UIBase obj)
    {
        if (obj.Name == UIDef.MainUI)
        {
            BaseCampLocaCacheChange();
            ClearBaseCampOrPlunderData();
        }
        else if (obj.Name == UIDef.ChapterSceneUI)
        {
            ClearBaseCampOrPlunderData();
        }
        else if (obj.name == UIDef.PlunderInProgressUI)
        {
            ClearBaseCampOrPlunderData();
        }
    }

    private void EventUIShow(UIBase obj)
    {
        if (obj.Name == UIDef.MainUI)
        {
            //MapSceneMgr.Init();
            ClickObjectMgr.LoadSuperDeadLandTip();
            LoadBaseCampData();
            //AnnouncementsAndMailUIModel.RequestMailLisForServer();
            //NetMgr.Send(NetAPIDef.eCTS_GAMESER_COST_DIAMOND_TOTAL, null, NetCallBack);
        }
        else if (obj.Name == UIDef.ChapterSceneUI)
        {
            //MapSceneMgr.Init();
            ClickObjectMgr.LoadSuperDeadLandTip();
            //LoadChapterData(ChapterModel.CurrentChapterInfo.index, ChapterModel.CurrentChapterInfo);
        }
    }

    private void NetCallBack(int msgID, IMessage response, bool isSucceed)
    {
        Response_cost_diamond_total costDiamond = (Response_cost_diamond_total)response;
        CostDiamondTotal = (int)costDiamond.Total;
    }

    private void Start()
    {
        wallRenderer.OnRenderTile += WallOnRenderTile;
        //wallRenderer.OverrideCreateInstance += GetTileGameObjectFromPool;
        wallRenderer.OverrideDestoryInstance += PutTileGameObjectIntoPool;
        wallRenderer.OverrideReOrder += ReOrderWave;
        terrainRenderer.OnRenderTile += TerrainOnRenderTile;
        //terrainRenderer.OverrideCreateInstance += GetTileGameObjectFromPool;
        terrainRenderer.OverrideDestoryInstance += PutTileGameObjectIntoPool;
        terrainRenderer.OverrideReOrder += OverrideTerrainReOrder;
        vegetationRenderer.OnRenderTile += VegetationOnRenderTile;
        //vegetationRenderer.OverrideCreateInstance += GetTileGameObjectFromPool;
        vegetationRenderer.OverrideDestoryInstance += PutTileGameObjectIntoPool;
        entityRenderer.OnRenderTile += EntityOnRenderTile;
        //entityRenderer.OverrideCreateInstance += GetTileGameObjectFromPool;
        entityRenderer.OverrideDestoryInstance += PutTileGameObjectIntoPool;
        sealLockRenderer.OnRenderTile += SealLockOnRenderTile;
        //sealLockRenderer.OverrideCreateInstance += GetTileGameObjectFromPool;
        sealLockRenderer.OverrideDestoryInstance += PutTileGameObjectIntoPool;
    }

    private void OverrideTerrainReOrder(int x, int y, SortingOrderTag tag, Renderer renderer)
    {
        if (tag.parentObjectName.Contains("bridge"))
        {
            renderer.sortingLayerName = "Object";
        }
    }

    private void PutTileGameObjectIntoPool(int x, int y, GameObject go)
    {
        //MapObject obj = go.GetComponent<MapObject>();
        //if (obj.BasicData != null)
        //{
        //    RecycleEntityMapObject(obj);
        //}
        //else
        //{
        //    RecyleTerrainFurlToPool(obj);
        //}
    }

    //private GameObject GetTileGameObjectFromPool(int x, int y, GameObject prefab)
    //{
    //    int id = 0;
    //    if (int.TryParse(prefab.name, out id))
    //    {
    //        return MapMgr.GetEntityMapObject(id).gameObject;
    //    }
    //    else
    //    {
    //        GameObject go = MapMgr.GetInstanceByName(prefab.name).gameObject;
    //        MapObjectTag tag = go.GetComponent<MapObjectTag>();
    //        if (tag == null)
    //        {
    //            tag = go.AddComponent<MapObjectTag>();
    //            tag.poolKey = prefab.name;
    //        }
    //        return go;
    //    }
    //}

    void ReOrderWave(int x, int y, SortingOrderTag tag, Renderer renderer)
    {
        if (tag.objectName.Contains("wave_side_"))
        {
            if (tag.objectName != "wave_side_corner")
            {
                renderer.sortingOrder = (int)wallRenderer.GetSortingOrderValue(x, y - 1, renderer.gameObject);
            }
        }
    }

    private void InitRes()
    {
        //InitializePool();
    }

    private void InitClickToCollectDic()
    {
        string value = PlayerProfile.LoadKeyClickToCollect();
        if (string.IsNullOrEmpty(value) == false)
        {
            JsonObject json = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(value);
            clickToCollectTipDic.Clear();
            if (json.ContainsKey("key"))
            {
                List<int> jsonKey = SimpleJson.SimpleJson.DeserializeObject<List<int>>(json["key"].ToString());
                List<int> jsonValue = SimpleJson.SimpleJson.DeserializeObject<List<int>>(json["value"].ToString());

                for (int i = 0; i < jsonKey.Count; i++)
                {
                    clickToCollectTipDic.Add((int)jsonKey[i], (int)jsonValue[i]);
                }
            }
        }
    }

    public static Vector3 GetSpatialPostion(MapObject mapObject)
    {
        if (mapObject.BasicData.area.Length > 0)
        {
            if (mapObject.BasicData.area[0] == 1 && mapObject.BasicData.area[1] == 1)
            {
                return mapObject.transform.position+new Vector3(0,0.75f);
            }
            else if (mapObject.BasicData.area[0] == 2 && mapObject.BasicData.area[1] == 1)
            {
                return mapObject.transform.position + new Vector3(0.75f, 1.2f);
            }
            else if (mapObject.BasicData.area[0] == 1 && mapObject.BasicData.area[1] == 2)
            {
                return mapObject.transform.position + new Vector3(-1.46f, 0.75f);
            }
            else if (mapObject.BasicData.area[0] == 2 && mapObject.BasicData.area[1] == 2)
            {
                return mapObject.transform.position + new Vector3(0, 1.5f);
            }
        }
        else
        {
            return mapObject.transform.position;
        }
        return mapObject.transform.position;
    }

    public static bool AlreadyClickTip(int id)
    {
        if (clickToCollectTipDic.ContainsKey(id))
        {
            if (clickToCollectTipDic[id] > 0)
            {
                return true;
            }
        }
        return false;
    }

    public static void AddClickTip(int id)
    {
        if (clickToCollectTipDic.ContainsKey(id))
        {
            clickToCollectTipDic[id] = 1;
        }
        else
        {
            clickToCollectTipDic.Add(id, 1);
        }

        JsonObject jsonData = new JsonObject();
        JsonArray jsonKey = new JsonArray();
        JsonArray jsonValue = new JsonArray();

        foreach (int key in clickToCollectTipDic.Keys)
        {
            jsonKey.Add(key);
        }
        jsonData.Add("key", jsonKey.ToString());

        foreach (int value in clickToCollectTipDic.Values)
        {
            jsonValue.Add(value);
        }
        jsonData.Add("value", jsonValue.ToString());
        PlayerProfile.SaveKeyClickToCollect(jsonData.ToString());
    }

    public static SpriteRenderer ShowAttackSight(MapObject obj)
    {
        Vector2 v3 = obj.entityTransform.position;
        SpriteRenderer vfx = DoubleClickSightTipPool.GetInstance();
        vfx.transform.position = v3;
        return vfx;
    }
}

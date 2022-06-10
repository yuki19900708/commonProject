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
        InitClickToCollectDic();
    }

  

    private void NetCallBack(int msgID, IMessage response, bool isSucceed)
    {
        Response_cost_diamond_total costDiamond = (Response_cost_diamond_total)response;
        CostDiamondTotal = (int)costDiamond.Total;
    }

    private void Start()
    {
        //wallRenderer.OverrideCreateInstance += GetTileGameObjectFromPool;
        wallRenderer.OverrideDestoryInstance += PutTileGameObjectIntoPool;
        wallRenderer.OverrideReOrder += ReOrderWave;
        //terrainRenderer.OverrideCreateInstance += GetTileGameObjectFromPool;
        terrainRenderer.OverrideDestoryInstance += PutTileGameObjectIntoPool;
        terrainRenderer.OverrideReOrder += OverrideTerrainReOrder;
        vegetationRenderer.OnRenderTile += VegetationOnRenderTile;
        //vegetationRenderer.OverrideCreateInstance += GetTileGameObjectFromPool;
        vegetationRenderer.OverrideDestoryInstance += PutTileGameObjectIntoPool;
        //entityRenderer.OverrideCreateInstance += GetTileGameObjectFromPool;
        entityRenderer.OverrideDestoryInstance += PutTileGameObjectIntoPool;
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

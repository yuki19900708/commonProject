using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameProto;

[System.Serializable]
public class OutputData
{
    public List<ObjectInfo> rateLootList = new List<ObjectInfo>();

    public ObjectInfo defaultLoot;

    public MapObjectData GetOutputResult()
    {
        bool isFind = false;

        if (rateLootList != null && rateLootList.Count > 0)
        {
            for (int i = 0; i < rateLootList.Count; i++)
            {
                int rateValue = (int)RandomMgr.Random.Next(RandomMgr.MAX_VALUE);

                if (rateLootList[i].rate > rateValue)
                {
                    MapObjectData data = null;
                    //指向产出配置表
                    if (rateLootList[i].itemType == 2)
                    {
                         data = GetObjectIndexFromLootTable(rateLootList[i].itemIndex);
                         isFind = true;
                         return data;
                    }
                    else if (rateLootList[i].itemType == 1)
                    {
                        //直接指向物件表
                        data = TableDataMgr.GetSingleMapObjectData(rateLootList[i].itemIndex);
                        isFind = true;
                        return data;
                    }
                }
            }
        }
        if (defaultLoot != null && isFind == false)
        {
            if (defaultLoot.itemType == 2)
            {
                MapObjectData data = GetObjectIndexFromLootTable(defaultLoot.itemIndex);
                return data;
            }
            else if (defaultLoot.itemType == 1)
            {
                //直接指向物件表
                MapObjectData data = TableDataMgr.GetSingleMapObjectData(defaultLoot.itemIndex);
                return data;
            }

        }
        return null;
    }

    private MapObjectData GetObjectIndexFromLootTable(int id)
    {
        return TableDataMgr.GetSingleLootData(id).outputItems.GetOutputResult();
    }

    public MapObjectData GetRandOutputResult(Random_action action)
    {
        bool isFind = false;

        if (rateLootList != null && rateLootList.Count > 0)
        {
            for (int i = 0; i < rateLootList.Count; i++)
            {
                MapObjectData data = null;
                if (rateLootList[i].itemType == 2)
                {
                    int rateValue = (int)RandomMgr.RandomRange(0, 10000, action, Random_result.LootId);
                    if (rateLootList[i].rate > rateValue)
                    {
                        data = GetRandObjectIndexFromLootTable(rateLootList[i].itemIndex, action);
                        isFind = true;
                        return data;
                    }
                }
                else if (rateLootList[i].itemType == 1)
                {
                    //直接指向物件表
                    int rateValue = (int)RandomMgr.RandomRange(0, 10000, action, Random_result.ObjectId);
                    if (rateLootList[i].rate > rateValue)
                    {
                        data = TableDataMgr.GetSingleMapObjectData(rateLootList[i].itemIndex);
                        isFind = true;
                        return data;
                    }
                }
            }
        }

        if (defaultLoot != null && isFind == false)
        {
            if (defaultLoot.itemType == 2)
            {
                MapObjectData data = GetRandObjectIndexFromLootTable(defaultLoot.itemIndex, action);
                return data;
            }
            else if (defaultLoot.itemType == 1)
            {
                //直接指向物件表
                MapObjectData data = TableDataMgr.GetSingleMapObjectData(defaultLoot.itemIndex);
                return data;
            }

        }
        return null;
    }

    private MapObjectData GetRandObjectIndexFromLootTable(int id, Random_action action)
    {
        return TableDataMgr.GetSingleLootData(id).outputItems.GetRandOutputResult(action);
    }
}

[System.Serializable]
public class ObjectInfo
{
    public int itemType;
    public int itemIndex;
    public int rate;
}

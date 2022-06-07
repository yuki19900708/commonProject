using System;
using System.Collections.Generic;
using UnityEngine;

public static class TableDataEventMgr
{
    public static void BindAllEvent()
    {
        TableDataMgr.Event_LoadVIPDataTable += Load<VIPDataTable>;
            TableDataMgr.Event_LoadLootDataTable += Load<LootDataTable>;
            TableDataMgr.Event_LoadTaskDataTable += Load<TaskDataTable>;
            TableDataMgr.Event_LoadProductIdDataTable += Load<ProductIdDataTable>;
            TableDataMgr.Event_LoadPurchaseDataTable += Load<PurchaseDataTable>;
            TableDataMgr.Event_LoadTimedChestDataTable += Load<TimedChestDataTable>;
            TableDataMgr.Event_LoadLevelDataTable += Load<LevelDataTable>;
            TableDataMgr.Event_LoadLifePowerDataTable += Load<LifePowerDataTable>;
            TableDataMgr.Event_LoadVariableDataTable += Load<VariableDataTable>;
            TableDataMgr.Event_LoadMonsterPortalDataTable += Load<MonsterPortalDataTable>;
            TableDataMgr.Event_LoadMergeEventDataTable += Load<MergeEventDataTable>;
            TableDataMgr.Event_LoadShopDataTable += Load<ShopDataTable>;
            TableDataMgr.Event_LoadCountryDataTable += Load<CountryDataTable>;
            TableDataMgr.Event_LoadChainTypeDataTable += Load<ChainTypeDataTable>;
            TableDataMgr.Event_LoadChainGroupDataTable += Load<ChainGroupDataTable>;
            TableDataMgr.Event_LoadMapUnlockDataTable += Load<MapUnlockDataTable>;
            TableDataMgr.Event_LoadTerrainDataTable += Load<TerrainDataTable>;
            TableDataMgr.Event_LoadHeadBoxDataTable += Load<HeadBoxDataTable>;
            TableDataMgr.Event_LoadHeadIconDataTable += Load<HeadIconDataTable>;
            TableDataMgr.Event_LoadBulletInfoDataTable += Load<BulletInfoDataTable>;
            TableDataMgr.Event_LoadBannedResponsTable += Load<BannedResponsTable>;
            TableDataMgr.Event_LoadConstValueTable += Load<ConstValueTable>;
            TableDataMgr.Event_LoadBuildingAttributesDataTable += Load<BuildingAttributesDataTable>;
            TableDataMgr.Event_LoadBuildShopDataTable += Load<BuildShopDataTable>;
            TableDataMgr.Event_LoadTutorialTableDataTable += Load<TutorialTableDataTable>;
            TableDataMgr.Event_LoadMonsterAttributesDataTable += Load<MonsterAttributesDataTable>;
            TableDataMgr.Event_LoadShieldDataTable += Load<ShieldDataTable>;
            TableDataMgr.Event_LoadRankRewardDataTable += Load<RankRewardDataTable>;
            TableDataMgr.Event_LoadTipsDataTable += Load<TipsDataTable>;
            TableDataMgr.Event_LoadDestructEventDataTable += Load<DestructEventDataTable>;
            TableDataMgr.Event_LoadEnemyDataTable += Load<EnemyDataTable>;
            TableDataMgr.Event_LoadTextDataTable += Load<TextDataTable>;
            TableDataMgr.Event_LoadDeadLandDataTable += Load<DeadLandDataTable>;
            TableDataMgr.Event_LoadRankDataTable += Load<RankDataTable>;
            TableDataMgr.Event_LoadDailyTransactionDataTable += Load<DailyTransactionDataTable>;
            TableDataMgr.Event_LoadDailyChestsDataTable += Load<DailyChestsDataTable>;
            TableDataMgr.Event_LoadTapEventDataTable += Load<TapEventDataTable>;
            TableDataMgr.Event_LoadFogDataTable += Load<FogDataTable>;
            TableDataMgr.Event_LoadMapObjectDataTable += Load<MapObjectDataTable>;
            TableDataMgr.Event_LoadRoleExpDataTable += Load<RoleExpDataTable>;
            TableDataMgr.Event_LoadAllianceScienceDataTable += Load<AllianceScienceDataTable>;
            TableDataMgr.Event_LoadVegetationDataTable += Load<VegetationDataTable>;
            TableDataMgr.Event_LoadSpawnEventDataTable += Load<SpawnEventDataTable>;
            TableDataMgr.Event_LoadEmojiDataTable += Load<EmojiDataTable>;
            TableDataMgr.Event_LoadHarvestEventDataTable += Load<HarvestEventDataTable>;
            TableDataMgr.Event_LoadTimedShopDataTable += Load<TimedShopDataTable>;
            TableDataMgr.Event_LoadRandomNameDataTable += Load<RandomNameDataTable>;
            TableDataMgr.Event_LoadRandomShopDataTable += Load<RandomShopDataTable>;
    }

	public static T Load<T>(string assetName) where T : UnityEngine.Object
	{
		return Resources.Load<T> ("DataAsset/" + assetName);
	}
}

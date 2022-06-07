using System;
using System.Collections.Generic;
using UnityEngine;

public enum TableDataName
{
    VIPDataTable,
    LootDataTable,
    TaskDataTable,
    ProductIdDataTable,
    PurchaseDataTable,
    TimedChestDataTable,
    LevelDataTable,
    LifePowerDataTable,
    VariableDataTable,
    MonsterPortalDataTable,
    MergeEventDataTable,
    ShopDataTable,
    CountryDataTable,
    ChainTypeDataTable,
    ChainGroupDataTable,
    MapUnlockDataTable,
    TerrainDataTable,
    HeadBoxDataTable,
    HeadIconDataTable,
    BulletInfoDataTable,
    BannedResponsTable,
    ConstValueTable,
    BuildingAttributesDataTable,
    BuildShopDataTable,
    TutorialTableDataTable,
    MonsterAttributesDataTable,
    ShieldDataTable,
    RankRewardDataTable,
    TipsDataTable,
    DestructEventDataTable,
    EnemyDataTable,
    TextDataTable,
    DeadLandDataTable,
    RankDataTable,
    DailyTransactionDataTable,
    DailyChestsDataTable,
    TapEventDataTable,
    FogDataTable,
    MapObjectDataTable,
    RoleExpDataTable,
    AllianceScienceDataTable,
    VegetationDataTable,
    SpawnEventDataTable,
    EmojiDataTable,
    HarvestEventDataTable,
    TimedShopDataTable,
    RandomNameDataTable,
    RandomShopDataTable,
    
}

public partial class TableDataMgr
{
    #region
    public static event Func<string, VIPDataTable> Event_LoadVIPDataTable;

    private static VIPDataTable vIPData ;

    public static VIPDataTable VIPDataTable
    {
       get
       {
           if (vIPData == null)
           {
               if(Event_LoadVIPDataTable != null)
               {
                  vIPData = Event_LoadVIPDataTable("VIPDataTable");
               }
           }
           if (vIPData == null)
           {
              Debug.LogError("vIPData不存在");
              return null;
           }
            return vIPData ;
        }
        set
        {
            vIPData  = value;
        }
    }
    public static event Func<string, LootDataTable> Event_LoadLootDataTable;

    private static LootDataTable lootData ;

    public static LootDataTable LootDataTable
    {
       get
       {
           if (lootData == null)
           {
               if(Event_LoadLootDataTable != null)
               {
                  lootData = Event_LoadLootDataTable("LootDataTable");
               }
           }
           if (lootData == null)
           {
              Debug.LogError("lootData不存在");
              return null;
           }
            return lootData ;
        }
        set
        {
            lootData  = value;
        }
    }
    public static event Func<string, TaskDataTable> Event_LoadTaskDataTable;

    private static TaskDataTable taskData ;

    public static TaskDataTable TaskDataTable
    {
       get
       {
           if (taskData == null)
           {
               if(Event_LoadTaskDataTable != null)
               {
                  taskData = Event_LoadTaskDataTable("TaskDataTable");
               }
           }
           if (taskData == null)
           {
              Debug.LogError("taskData不存在");
              return null;
           }
            return taskData ;
        }
        set
        {
            taskData  = value;
        }
    }
    public static event Func<string, ProductIdDataTable> Event_LoadProductIdDataTable;

    private static ProductIdDataTable productIdData ;

    public static ProductIdDataTable ProductIdDataTable
    {
       get
       {
           if (productIdData == null)
           {
               if(Event_LoadProductIdDataTable != null)
               {
                  productIdData = Event_LoadProductIdDataTable("ProductIdDataTable");
               }
           }
           if (productIdData == null)
           {
              Debug.LogError("productIdData不存在");
              return null;
           }
            return productIdData ;
        }
        set
        {
            productIdData  = value;
        }
    }
    public static event Func<string, PurchaseDataTable> Event_LoadPurchaseDataTable;

    private static PurchaseDataTable purchaseData ;

    public static PurchaseDataTable PurchaseDataTable
    {
       get
       {
           if (purchaseData == null)
           {
               if(Event_LoadPurchaseDataTable != null)
               {
                  purchaseData = Event_LoadPurchaseDataTable("PurchaseDataTable");
               }
           }
           if (purchaseData == null)
           {
              Debug.LogError("purchaseData不存在");
              return null;
           }
            return purchaseData ;
        }
        set
        {
            purchaseData  = value;
        }
    }
    public static event Func<string, TimedChestDataTable> Event_LoadTimedChestDataTable;

    private static TimedChestDataTable timedChestData ;

    public static TimedChestDataTable TimedChestDataTable
    {
       get
       {
           if (timedChestData == null)
           {
               if(Event_LoadTimedChestDataTable != null)
               {
                  timedChestData = Event_LoadTimedChestDataTable("TimedChestDataTable");
               }
           }
           if (timedChestData == null)
           {
              Debug.LogError("timedChestData不存在");
              return null;
           }
            return timedChestData ;
        }
        set
        {
            timedChestData  = value;
        }
    }
    public static event Func<string, LevelDataTable> Event_LoadLevelDataTable;

    private static LevelDataTable levelData ;

    public static LevelDataTable LevelDataTable
    {
       get
       {
           if (levelData == null)
           {
               if(Event_LoadLevelDataTable != null)
               {
                  levelData = Event_LoadLevelDataTable("LevelDataTable");
               }
           }
           if (levelData == null)
           {
              Debug.LogError("levelData不存在");
              return null;
           }
            return levelData ;
        }
        set
        {
            levelData  = value;
        }
    }
    public static event Func<string, LifePowerDataTable> Event_LoadLifePowerDataTable;

    private static LifePowerDataTable lifePowerData ;

    public static LifePowerDataTable LifePowerDataTable
    {
       get
       {
           if (lifePowerData == null)
           {
               if(Event_LoadLifePowerDataTable != null)
               {
                  lifePowerData = Event_LoadLifePowerDataTable("LifePowerDataTable");
               }
           }
           if (lifePowerData == null)
           {
              Debug.LogError("lifePowerData不存在");
              return null;
           }
            return lifePowerData ;
        }
        set
        {
            lifePowerData  = value;
        }
    }
    public static event Func<string, VariableDataTable> Event_LoadVariableDataTable;

    private static VariableDataTable variableData ;

    public static VariableDataTable VariableDataTable
    {
       get
       {
           if (variableData == null)
           {
               if(Event_LoadVariableDataTable != null)
               {
                  variableData = Event_LoadVariableDataTable("VariableDataTable");
               }
           }
           if (variableData == null)
           {
              Debug.LogError("variableData不存在");
              return null;
           }
            return variableData ;
        }
        set
        {
            variableData  = value;
        }
    }
    public static event Func<string, MonsterPortalDataTable> Event_LoadMonsterPortalDataTable;

    private static MonsterPortalDataTable monsterPortalData ;

    public static MonsterPortalDataTable MonsterPortalDataTable
    {
       get
       {
           if (monsterPortalData == null)
           {
               if(Event_LoadMonsterPortalDataTable != null)
               {
                  monsterPortalData = Event_LoadMonsterPortalDataTable("MonsterPortalDataTable");
               }
           }
           if (monsterPortalData == null)
           {
              Debug.LogError("monsterPortalData不存在");
              return null;
           }
            return monsterPortalData ;
        }
        set
        {
            monsterPortalData  = value;
        }
    }
    public static event Func<string, MergeEventDataTable> Event_LoadMergeEventDataTable;

    private static MergeEventDataTable mergeEventData ;

    public static MergeEventDataTable MergeEventDataTable
    {
       get
       {
           if (mergeEventData == null)
           {
               if(Event_LoadMergeEventDataTable != null)
               {
                  mergeEventData = Event_LoadMergeEventDataTable("MergeEventDataTable");
               }
           }
           if (mergeEventData == null)
           {
              Debug.LogError("mergeEventData不存在");
              return null;
           }
            return mergeEventData ;
        }
        set
        {
            mergeEventData  = value;
        }
    }
    public static event Func<string, ShopDataTable> Event_LoadShopDataTable;

    private static ShopDataTable shopData ;

    public static ShopDataTable ShopDataTable
    {
       get
       {
           if (shopData == null)
           {
               if(Event_LoadShopDataTable != null)
               {
                  shopData = Event_LoadShopDataTable("ShopDataTable");
               }
           }
           if (shopData == null)
           {
              Debug.LogError("shopData不存在");
              return null;
           }
            return shopData ;
        }
        set
        {
            shopData  = value;
        }
    }
    public static event Func<string, CountryDataTable> Event_LoadCountryDataTable;

    private static CountryDataTable countryData ;

    public static CountryDataTable CountryDataTable
    {
       get
       {
           if (countryData == null)
           {
               if(Event_LoadCountryDataTable != null)
               {
                  countryData = Event_LoadCountryDataTable("CountryDataTable");
               }
           }
           if (countryData == null)
           {
              Debug.LogError("countryData不存在");
              return null;
           }
            return countryData ;
        }
        set
        {
            countryData  = value;
        }
    }
    public static event Func<string, ChainTypeDataTable> Event_LoadChainTypeDataTable;

    private static ChainTypeDataTable chainTypeData ;

    public static ChainTypeDataTable ChainTypeDataTable
    {
       get
       {
           if (chainTypeData == null)
           {
               if(Event_LoadChainTypeDataTable != null)
               {
                  chainTypeData = Event_LoadChainTypeDataTable("ChainTypeDataTable");
               }
           }
           if (chainTypeData == null)
           {
              Debug.LogError("chainTypeData不存在");
              return null;
           }
            return chainTypeData ;
        }
        set
        {
            chainTypeData  = value;
        }
    }
    public static event Func<string, ChainGroupDataTable> Event_LoadChainGroupDataTable;

    private static ChainGroupDataTable chainGroupData ;

    public static ChainGroupDataTable ChainGroupDataTable
    {
       get
       {
           if (chainGroupData == null)
           {
               if(Event_LoadChainGroupDataTable != null)
               {
                  chainGroupData = Event_LoadChainGroupDataTable("ChainGroupDataTable");
               }
           }
           if (chainGroupData == null)
           {
              Debug.LogError("chainGroupData不存在");
              return null;
           }
            return chainGroupData ;
        }
        set
        {
            chainGroupData  = value;
        }
    }
    public static event Func<string, MapUnlockDataTable> Event_LoadMapUnlockDataTable;

    private static MapUnlockDataTable mapUnlockData ;

    public static MapUnlockDataTable MapUnlockDataTable
    {
       get
       {
           if (mapUnlockData == null)
           {
               if(Event_LoadMapUnlockDataTable != null)
               {
                  mapUnlockData = Event_LoadMapUnlockDataTable("MapUnlockDataTable");
               }
           }
           if (mapUnlockData == null)
           {
              Debug.LogError("mapUnlockData不存在");
              return null;
           }
            return mapUnlockData ;
        }
        set
        {
            mapUnlockData  = value;
        }
    }
    public static event Func<string, TerrainDataTable> Event_LoadTerrainDataTable;

    private static TerrainDataTable terrainData ;

    public static TerrainDataTable TerrainDataTable
    {
       get
       {
           if (terrainData == null)
           {
               if(Event_LoadTerrainDataTable != null)
               {
                  terrainData = Event_LoadTerrainDataTable("TerrainDataTable");
               }
           }
           if (terrainData == null)
           {
              Debug.LogError("terrainData不存在");
              return null;
           }
            return terrainData ;
        }
        set
        {
            terrainData  = value;
        }
    }
    public static event Func<string, HeadBoxDataTable> Event_LoadHeadBoxDataTable;

    private static HeadBoxDataTable headBoxData ;

    public static HeadBoxDataTable HeadBoxDataTable
    {
       get
       {
           if (headBoxData == null)
           {
               if(Event_LoadHeadBoxDataTable != null)
               {
                  headBoxData = Event_LoadHeadBoxDataTable("HeadBoxDataTable");
               }
           }
           if (headBoxData == null)
           {
              Debug.LogError("headBoxData不存在");
              return null;
           }
            return headBoxData ;
        }
        set
        {
            headBoxData  = value;
        }
    }
    public static event Func<string, HeadIconDataTable> Event_LoadHeadIconDataTable;

    private static HeadIconDataTable headIconData ;

    public static HeadIconDataTable HeadIconDataTable
    {
       get
       {
           if (headIconData == null)
           {
               if(Event_LoadHeadIconDataTable != null)
               {
                  headIconData = Event_LoadHeadIconDataTable("HeadIconDataTable");
               }
           }
           if (headIconData == null)
           {
              Debug.LogError("headIconData不存在");
              return null;
           }
            return headIconData ;
        }
        set
        {
            headIconData  = value;
        }
    }
    public static event Func<string, BulletInfoDataTable> Event_LoadBulletInfoDataTable;

    private static BulletInfoDataTable bulletInfoData ;

    public static BulletInfoDataTable BulletInfoDataTable
    {
       get
       {
           if (bulletInfoData == null)
           {
               if(Event_LoadBulletInfoDataTable != null)
               {
                  bulletInfoData = Event_LoadBulletInfoDataTable("BulletInfoDataTable");
               }
           }
           if (bulletInfoData == null)
           {
              Debug.LogError("bulletInfoData不存在");
              return null;
           }
            return bulletInfoData ;
        }
        set
        {
            bulletInfoData  = value;
        }
    }
    public static event Func<string, BannedResponsTable> Event_LoadBannedResponsTable;

    private static BannedResponsTable bannedRespons ;

    public static BannedResponsTable BannedResponsTable
    {
       get
       {
           if (bannedRespons == null)
           {
               if(Event_LoadBannedResponsTable != null)
               {
                  bannedRespons = Event_LoadBannedResponsTable("BannedResponsTable");
               }
           }
           if (bannedRespons == null)
           {
              Debug.LogError("bannedRespons不存在");
              return null;
           }
            return bannedRespons ;
        }
        set
        {
            bannedRespons  = value;
        }
    }
    public static event Func<string, ConstValueTable> Event_LoadConstValueTable;

    private static ConstValueTable constValue ;

    public static ConstValueTable ConstValueTable
    {
       get
       {
           if (constValue == null)
           {
               if(Event_LoadConstValueTable != null)
               {
                  constValue = Event_LoadConstValueTable("ConstValueTable");
               }
           }
           if (constValue == null)
           {
              Debug.LogError("constValue不存在");
              return null;
           }
            return constValue ;
        }
        set
        {
            constValue  = value;
        }
    }
    public static event Func<string, BuildingAttributesDataTable> Event_LoadBuildingAttributesDataTable;

    private static BuildingAttributesDataTable buildingAttributesData ;

    public static BuildingAttributesDataTable BuildingAttributesDataTable
    {
       get
       {
           if (buildingAttributesData == null)
           {
               if(Event_LoadBuildingAttributesDataTable != null)
               {
                  buildingAttributesData = Event_LoadBuildingAttributesDataTable("BuildingAttributesDataTable");
               }
           }
           if (buildingAttributesData == null)
           {
              Debug.LogError("buildingAttributesData不存在");
              return null;
           }
            return buildingAttributesData ;
        }
        set
        {
            buildingAttributesData  = value;
        }
    }
    public static event Func<string, BuildShopDataTable> Event_LoadBuildShopDataTable;

    private static BuildShopDataTable buildShopData ;

    public static BuildShopDataTable BuildShopDataTable
    {
       get
       {
           if (buildShopData == null)
           {
               if(Event_LoadBuildShopDataTable != null)
               {
                  buildShopData = Event_LoadBuildShopDataTable("BuildShopDataTable");
               }
           }
           if (buildShopData == null)
           {
              Debug.LogError("buildShopData不存在");
              return null;
           }
            return buildShopData ;
        }
        set
        {
            buildShopData  = value;
        }
    }
    public static event Func<string, TutorialTableDataTable> Event_LoadTutorialTableDataTable;

    private static TutorialTableDataTable tutorialTableData ;

    public static TutorialTableDataTable TutorialTableDataTable
    {
       get
       {
           if (tutorialTableData == null)
           {
               if(Event_LoadTutorialTableDataTable != null)
               {
                  tutorialTableData = Event_LoadTutorialTableDataTable("TutorialTableDataTable");
               }
           }
           if (tutorialTableData == null)
           {
              Debug.LogError("tutorialTableData不存在");
              return null;
           }
            return tutorialTableData ;
        }
        set
        {
            tutorialTableData  = value;
        }
    }
    public static event Func<string, MonsterAttributesDataTable> Event_LoadMonsterAttributesDataTable;

    private static MonsterAttributesDataTable monsterAttributesData ;

    public static MonsterAttributesDataTable MonsterAttributesDataTable
    {
       get
       {
           if (monsterAttributesData == null)
           {
               if(Event_LoadMonsterAttributesDataTable != null)
               {
                  monsterAttributesData = Event_LoadMonsterAttributesDataTable("MonsterAttributesDataTable");
               }
           }
           if (monsterAttributesData == null)
           {
              Debug.LogError("monsterAttributesData不存在");
              return null;
           }
            return monsterAttributesData ;
        }
        set
        {
            monsterAttributesData  = value;
        }
    }
    public static event Func<string, ShieldDataTable> Event_LoadShieldDataTable;

    private static ShieldDataTable shieldData ;

    public static ShieldDataTable ShieldDataTable
    {
       get
       {
           if (shieldData == null)
           {
               if(Event_LoadShieldDataTable != null)
               {
                  shieldData = Event_LoadShieldDataTable("ShieldDataTable");
               }
           }
           if (shieldData == null)
           {
              Debug.LogError("shieldData不存在");
              return null;
           }
            return shieldData ;
        }
        set
        {
            shieldData  = value;
        }
    }
    public static event Func<string, RankRewardDataTable> Event_LoadRankRewardDataTable;

    private static RankRewardDataTable rankRewardData ;

    public static RankRewardDataTable RankRewardDataTable
    {
       get
       {
           if (rankRewardData == null)
           {
               if(Event_LoadRankRewardDataTable != null)
               {
                  rankRewardData = Event_LoadRankRewardDataTable("RankRewardDataTable");
               }
           }
           if (rankRewardData == null)
           {
              Debug.LogError("rankRewardData不存在");
              return null;
           }
            return rankRewardData ;
        }
        set
        {
            rankRewardData  = value;
        }
    }
    public static event Func<string, TipsDataTable> Event_LoadTipsDataTable;

    private static TipsDataTable tipsData ;

    public static TipsDataTable TipsDataTable
    {
       get
       {
           if (tipsData == null)
           {
               if(Event_LoadTipsDataTable != null)
               {
                  tipsData = Event_LoadTipsDataTable("TipsDataTable");
               }
           }
           if (tipsData == null)
           {
              Debug.LogError("tipsData不存在");
              return null;
           }
            return tipsData ;
        }
        set
        {
            tipsData  = value;
        }
    }
    public static event Func<string, DestructEventDataTable> Event_LoadDestructEventDataTable;

    private static DestructEventDataTable destructEventData ;

    public static DestructEventDataTable DestructEventDataTable
    {
       get
       {
           if (destructEventData == null)
           {
               if(Event_LoadDestructEventDataTable != null)
               {
                  destructEventData = Event_LoadDestructEventDataTable("DestructEventDataTable");
               }
           }
           if (destructEventData == null)
           {
              Debug.LogError("destructEventData不存在");
              return null;
           }
            return destructEventData ;
        }
        set
        {
            destructEventData  = value;
        }
    }
    public static event Func<string, EnemyDataTable> Event_LoadEnemyDataTable;

    private static EnemyDataTable enemyData ;

    public static EnemyDataTable EnemyDataTable
    {
       get
       {
           if (enemyData == null)
           {
               if(Event_LoadEnemyDataTable != null)
               {
                  enemyData = Event_LoadEnemyDataTable("EnemyDataTable");
               }
           }
           if (enemyData == null)
           {
              Debug.LogError("enemyData不存在");
              return null;
           }
            return enemyData ;
        }
        set
        {
            enemyData  = value;
        }
    }
    public static event Func<string, TextDataTable> Event_LoadTextDataTable;

    private static TextDataTable textData ;

    public static TextDataTable TextDataTable
    {
       get
       {
           if (textData == null)
           {
               if(Event_LoadTextDataTable != null)
               {
                  textData = Event_LoadTextDataTable("TextDataTable");
               }
           }
           if (textData == null)
           {
              Debug.LogError("textData不存在");
              return null;
           }
            return textData ;
        }
        set
        {
            textData  = value;
        }
    }
    public static event Func<string, DeadLandDataTable> Event_LoadDeadLandDataTable;

    private static DeadLandDataTable deadLandData ;

    public static DeadLandDataTable DeadLandDataTable
    {
       get
       {
           if (deadLandData == null)
           {
               if(Event_LoadDeadLandDataTable != null)
               {
                  deadLandData = Event_LoadDeadLandDataTable("DeadLandDataTable");
               }
           }
           if (deadLandData == null)
           {
              Debug.LogError("deadLandData不存在");
              return null;
           }
            return deadLandData ;
        }
        set
        {
            deadLandData  = value;
        }
    }
    public static event Func<string, RankDataTable> Event_LoadRankDataTable;

    private static RankDataTable rankData ;

    public static RankDataTable RankDataTable
    {
       get
       {
           if (rankData == null)
           {
               if(Event_LoadRankDataTable != null)
               {
                  rankData = Event_LoadRankDataTable("RankDataTable");
               }
           }
           if (rankData == null)
           {
              Debug.LogError("rankData不存在");
              return null;
           }
            return rankData ;
        }
        set
        {
            rankData  = value;
        }
    }
    public static event Func<string, DailyTransactionDataTable> Event_LoadDailyTransactionDataTable;

    private static DailyTransactionDataTable dailyTransactionData ;

    public static DailyTransactionDataTable DailyTransactionDataTable
    {
       get
       {
           if (dailyTransactionData == null)
           {
               if(Event_LoadDailyTransactionDataTable != null)
               {
                  dailyTransactionData = Event_LoadDailyTransactionDataTable("DailyTransactionDataTable");
               }
           }
           if (dailyTransactionData == null)
           {
              Debug.LogError("dailyTransactionData不存在");
              return null;
           }
            return dailyTransactionData ;
        }
        set
        {
            dailyTransactionData  = value;
        }
    }
    public static event Func<string, DailyChestsDataTable> Event_LoadDailyChestsDataTable;

    private static DailyChestsDataTable dailyChestsData ;

    public static DailyChestsDataTable DailyChestsDataTable
    {
       get
       {
           if (dailyChestsData == null)
           {
               if(Event_LoadDailyChestsDataTable != null)
               {
                  dailyChestsData = Event_LoadDailyChestsDataTable("DailyChestsDataTable");
               }
           }
           if (dailyChestsData == null)
           {
              Debug.LogError("dailyChestsData不存在");
              return null;
           }
            return dailyChestsData ;
        }
        set
        {
            dailyChestsData  = value;
        }
    }
    public static event Func<string, TapEventDataTable> Event_LoadTapEventDataTable;

    private static TapEventDataTable tapEventData ;

    public static TapEventDataTable TapEventDataTable
    {
       get
       {
           if (tapEventData == null)
           {
               if(Event_LoadTapEventDataTable != null)
               {
                  tapEventData = Event_LoadTapEventDataTable("TapEventDataTable");
               }
           }
           if (tapEventData == null)
           {
              Debug.LogError("tapEventData不存在");
              return null;
           }
            return tapEventData ;
        }
        set
        {
            tapEventData  = value;
        }
    }
    public static event Func<string, FogDataTable> Event_LoadFogDataTable;

    private static FogDataTable fogData ;

    public static FogDataTable FogDataTable
    {
       get
       {
           if (fogData == null)
           {
               if(Event_LoadFogDataTable != null)
               {
                  fogData = Event_LoadFogDataTable("FogDataTable");
               }
           }
           if (fogData == null)
           {
              Debug.LogError("fogData不存在");
              return null;
           }
            return fogData ;
        }
        set
        {
            fogData  = value;
        }
    }
    public static event Func<string, MapObjectDataTable> Event_LoadMapObjectDataTable;

    private static MapObjectDataTable mapObjectData ;

    public static MapObjectDataTable MapObjectDataTable
    {
       get
       {
           if (mapObjectData == null)
           {
               if(Event_LoadMapObjectDataTable != null)
               {
                  mapObjectData = Event_LoadMapObjectDataTable("MapObjectDataTable");
               }
           }
           if (mapObjectData == null)
           {
              Debug.LogError("mapObjectData不存在");
              return null;
           }
            return mapObjectData ;
        }
        set
        {
            mapObjectData  = value;
        }
    }
    public static event Func<string, RoleExpDataTable> Event_LoadRoleExpDataTable;

    private static RoleExpDataTable roleExpData ;

    public static RoleExpDataTable RoleExpDataTable
    {
       get
       {
           if (roleExpData == null)
           {
               if(Event_LoadRoleExpDataTable != null)
               {
                  roleExpData = Event_LoadRoleExpDataTable("RoleExpDataTable");
               }
           }
           if (roleExpData == null)
           {
              Debug.LogError("roleExpData不存在");
              return null;
           }
            return roleExpData ;
        }
        set
        {
            roleExpData  = value;
        }
    }
    public static event Func<string, AllianceScienceDataTable> Event_LoadAllianceScienceDataTable;

    private static AllianceScienceDataTable allianceScienceData ;

    public static AllianceScienceDataTable AllianceScienceDataTable
    {
       get
       {
           if (allianceScienceData == null)
           {
               if(Event_LoadAllianceScienceDataTable != null)
               {
                  allianceScienceData = Event_LoadAllianceScienceDataTable("AllianceScienceDataTable");
               }
           }
           if (allianceScienceData == null)
           {
              Debug.LogError("allianceScienceData不存在");
              return null;
           }
            return allianceScienceData ;
        }
        set
        {
            allianceScienceData  = value;
        }
    }
    public static event Func<string, VegetationDataTable> Event_LoadVegetationDataTable;

    private static VegetationDataTable vegetationData ;

    public static VegetationDataTable VegetationDataTable
    {
       get
       {
           if (vegetationData == null)
           {
               if(Event_LoadVegetationDataTable != null)
               {
                  vegetationData = Event_LoadVegetationDataTable("VegetationDataTable");
               }
           }
           if (vegetationData == null)
           {
              Debug.LogError("vegetationData不存在");
              return null;
           }
            return vegetationData ;
        }
        set
        {
            vegetationData  = value;
        }
    }
    public static event Func<string, SpawnEventDataTable> Event_LoadSpawnEventDataTable;

    private static SpawnEventDataTable spawnEventData ;

    public static SpawnEventDataTable SpawnEventDataTable
    {
       get
       {
           if (spawnEventData == null)
           {
               if(Event_LoadSpawnEventDataTable != null)
               {
                  spawnEventData = Event_LoadSpawnEventDataTable("SpawnEventDataTable");
               }
           }
           if (spawnEventData == null)
           {
              Debug.LogError("spawnEventData不存在");
              return null;
           }
            return spawnEventData ;
        }
        set
        {
            spawnEventData  = value;
        }
    }
    public static event Func<string, EmojiDataTable> Event_LoadEmojiDataTable;

    private static EmojiDataTable emojiData ;

    public static EmojiDataTable EmojiDataTable
    {
       get
       {
           if (emojiData == null)
           {
               if(Event_LoadEmojiDataTable != null)
               {
                  emojiData = Event_LoadEmojiDataTable("EmojiDataTable");
               }
           }
           if (emojiData == null)
           {
              Debug.LogError("emojiData不存在");
              return null;
           }
            return emojiData ;
        }
        set
        {
            emojiData  = value;
        }
    }
    public static event Func<string, HarvestEventDataTable> Event_LoadHarvestEventDataTable;

    private static HarvestEventDataTable harvestEventData ;

    public static HarvestEventDataTable HarvestEventDataTable
    {
       get
       {
           if (harvestEventData == null)
           {
               if(Event_LoadHarvestEventDataTable != null)
               {
                  harvestEventData = Event_LoadHarvestEventDataTable("HarvestEventDataTable");
               }
           }
           if (harvestEventData == null)
           {
              Debug.LogError("harvestEventData不存在");
              return null;
           }
            return harvestEventData ;
        }
        set
        {
            harvestEventData  = value;
        }
    }
    public static event Func<string, TimedShopDataTable> Event_LoadTimedShopDataTable;

    private static TimedShopDataTable timedShopData ;

    public static TimedShopDataTable TimedShopDataTable
    {
       get
       {
           if (timedShopData == null)
           {
               if(Event_LoadTimedShopDataTable != null)
               {
                  timedShopData = Event_LoadTimedShopDataTable("TimedShopDataTable");
               }
           }
           if (timedShopData == null)
           {
              Debug.LogError("timedShopData不存在");
              return null;
           }
            return timedShopData ;
        }
        set
        {
            timedShopData  = value;
        }
    }
    public static event Func<string, RandomNameDataTable> Event_LoadRandomNameDataTable;

    private static RandomNameDataTable randomNameData ;

    public static RandomNameDataTable RandomNameDataTable
    {
       get
       {
           if (randomNameData == null)
           {
               if(Event_LoadRandomNameDataTable != null)
               {
                  randomNameData = Event_LoadRandomNameDataTable("RandomNameDataTable");
               }
           }
           if (randomNameData == null)
           {
              Debug.LogError("randomNameData不存在");
              return null;
           }
            return randomNameData ;
        }
        set
        {
            randomNameData  = value;
        }
    }
    public static event Func<string, RandomShopDataTable> Event_LoadRandomShopDataTable;

    private static RandomShopDataTable randomShopData ;

    public static RandomShopDataTable RandomShopDataTable
    {
       get
       {
           if (randomShopData == null)
           {
               if(Event_LoadRandomShopDataTable != null)
               {
                  randomShopData = Event_LoadRandomShopDataTable("RandomShopDataTable");
               }
           }
           if (randomShopData == null)
           {
              Debug.LogError("randomShopData不存在");
              return null;
           }
            return randomShopData ;
        }
        set
        {
            randomShopData  = value;
        }
    }

    #endregion
    private static Dictionary<TableDataName, ScriptableObject> dataDic = new Dictionary<TableDataName, ScriptableObject>();

    public static Dictionary<TableDataName, ScriptableObject> DataDic
    {
        get
        {
            return dataDic;
        }
        set
        {
            dataDic = value;
        }
    }

    public static void LoadAllData()
    {
        ScriptableObject[] scriptObj = Resources.LoadAll<ScriptableObject>("DataAsset");

        Array nameArray = Enum.GetValues(typeof(TableDataName));
        foreach (ScriptableObject obj in scriptObj)
        {
            bool isContains = false;
            foreach (TableDataName name in nameArray)
            {
                if(obj.ToString().Contains(name.ToString()))
                {
                    isContains= true;
                    DataDic.Add(name, obj);
                }
            }

            if(isContains ==false)
            {
                Debug.LogError("检查策划表和资源是否一致");
            }
        }
    }

    public static List<VIPData> GetAllVIPDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<VIPData>(VIPDataTable.GetAllReadonlyData());
        }
        else
        {
           return VIPDataTable.GetAllData();
        }
    }

    public static VIPData GetSingleVIPData(int id)
    {
        return VIPDataTable.GetData(id);
    }

    public static List<LootData> GetAllLootDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<LootData>(LootDataTable.GetAllReadonlyData());
        }
        else
        {
           return LootDataTable.GetAllData();
        }
    }

    public static LootData GetSingleLootData(int id)
    {
        return LootDataTable.GetData(id);
    }

    public static List<TaskData> GetAllTaskDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<TaskData>(TaskDataTable.GetAllReadonlyData());
        }
        else
        {
           return TaskDataTable.GetAllData();
        }
    }

    public static TaskData GetSingleTaskData(int id)
    {
        return TaskDataTable.GetData(id);
    }

    public static List<ProductIdData> GetAllProductIdDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<ProductIdData>(ProductIdDataTable.GetAllReadonlyData());
        }
        else
        {
           return ProductIdDataTable.GetAllData();
        }
    }

    public static ProductIdData GetSingleProductIdData(int id)
    {
        return ProductIdDataTable.GetData(id);
    }

    public static List<PurchaseData> GetAllPurchaseDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<PurchaseData>(PurchaseDataTable.GetAllReadonlyData());
        }
        else
        {
           return PurchaseDataTable.GetAllData();
        }
    }

    public static PurchaseData GetSinglePurchaseData(int id)
    {
        return PurchaseDataTable.GetData(id);
    }

    public static List<TimedChestData> GetAllTimedChestDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<TimedChestData>(TimedChestDataTable.GetAllReadonlyData());
        }
        else
        {
           return TimedChestDataTable.GetAllData();
        }
    }

    public static TimedChestData GetSingleTimedChestData(int id)
    {
        return TimedChestDataTable.GetData(id);
    }

    public static List<LevelData> GetAllLevelDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<LevelData>(LevelDataTable.GetAllReadonlyData());
        }
        else
        {
           return LevelDataTable.GetAllData();
        }
    }

    public static LevelData GetSingleLevelData(int id)
    {
        return LevelDataTable.GetData(id);
    }

    public static List<LifePowerData> GetAllLifePowerDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<LifePowerData>(LifePowerDataTable.GetAllReadonlyData());
        }
        else
        {
           return LifePowerDataTable.GetAllData();
        }
    }

    public static LifePowerData GetSingleLifePowerData(int id)
    {
        return LifePowerDataTable.GetData(id);
    }

    public static List<VariableData> GetAllVariableDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<VariableData>(VariableDataTable.GetAllReadonlyData());
        }
        else
        {
           return VariableDataTable.GetAllData();
        }
    }

    public static VariableData GetSingleVariableData(int id)
    {
        return VariableDataTable.GetData(id);
    }

    public static List<MonsterPortalData> GetAllMonsterPortalDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<MonsterPortalData>(MonsterPortalDataTable.GetAllReadonlyData());
        }
        else
        {
           return MonsterPortalDataTable.GetAllData();
        }
    }

    public static MonsterPortalData GetSingleMonsterPortalData(int id)
    {
        return MonsterPortalDataTable.GetData(id);
    }

    public static List<MergeEventData> GetAllMergeEventDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<MergeEventData>(MergeEventDataTable.GetAllReadonlyData());
        }
        else
        {
           return MergeEventDataTable.GetAllData();
        }
    }

    public static MergeEventData GetSingleMergeEventData(int id)
    {
        return MergeEventDataTable.GetData(id);
    }

    public static List<ShopData> GetAllShopDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<ShopData>(ShopDataTable.GetAllReadonlyData());
        }
        else
        {
           return ShopDataTable.GetAllData();
        }
    }

    public static ShopData GetSingleShopData(int id)
    {
        return ShopDataTable.GetData(id);
    }

    public static List<CountryData> GetAllCountryDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<CountryData>(CountryDataTable.GetAllReadonlyData());
        }
        else
        {
           return CountryDataTable.GetAllData();
        }
    }

    public static CountryData GetSingleCountryData(int id)
    {
        return CountryDataTable.GetData(id);
    }

    public static List<ChainTypeData> GetAllChainTypeDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<ChainTypeData>(ChainTypeDataTable.GetAllReadonlyData());
        }
        else
        {
           return ChainTypeDataTable.GetAllData();
        }
    }

    public static ChainTypeData GetSingleChainTypeData(int id)
    {
        return ChainTypeDataTable.GetData(id);
    }

    public static List<ChainGroupData> GetAllChainGroupDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<ChainGroupData>(ChainGroupDataTable.GetAllReadonlyData());
        }
        else
        {
           return ChainGroupDataTable.GetAllData();
        }
    }

    public static ChainGroupData GetSingleChainGroupData(int id)
    {
        return ChainGroupDataTable.GetData(id);
    }

    public static List<MapUnlockData> GetAllMapUnlockDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<MapUnlockData>(MapUnlockDataTable.GetAllReadonlyData());
        }
        else
        {
           return MapUnlockDataTable.GetAllData();
        }
    }

    public static MapUnlockData GetSingleMapUnlockData(int id)
    {
        return MapUnlockDataTable.GetData(id);
    }

    public static List<TerrainData> GetAllTerrainDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<TerrainData>(TerrainDataTable.GetAllReadonlyData());
        }
        else
        {
           return TerrainDataTable.GetAllData();
        }
    }

    public static TerrainData GetSingleTerrainData(int id)
    {
        return TerrainDataTable.GetData(id);
    }

    public static List<HeadBoxData> GetAllHeadBoxDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<HeadBoxData>(HeadBoxDataTable.GetAllReadonlyData());
        }
        else
        {
           return HeadBoxDataTable.GetAllData();
        }
    }

    public static HeadBoxData GetSingleHeadBoxData(int id)
    {
        return HeadBoxDataTable.GetData(id);
    }

    public static List<HeadIconData> GetAllHeadIconDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<HeadIconData>(HeadIconDataTable.GetAllReadonlyData());
        }
        else
        {
           return HeadIconDataTable.GetAllData();
        }
    }

    public static HeadIconData GetSingleHeadIconData(int id)
    {
        return HeadIconDataTable.GetData(id);
    }

    public static List<BulletInfoData> GetAllBulletInfoDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<BulletInfoData>(BulletInfoDataTable.GetAllReadonlyData());
        }
        else
        {
           return BulletInfoDataTable.GetAllData();
        }
    }

    public static BulletInfoData GetSingleBulletInfoData(int id)
    {
        return BulletInfoDataTable.GetData(id);
    }

    public static List<BannedRespons> GetAllBannedResponss(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<BannedRespons>(BannedResponsTable.GetAllReadonlyData());
        }
        else
        {
           return BannedResponsTable.GetAllData();
        }
    }

    public static BannedRespons GetSingleBannedRespons(int id)
    {
        return BannedResponsTable.GetData(id);
    }

    public static List<ConstValue> GetAllConstValues(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<ConstValue>(ConstValueTable.GetAllReadonlyData());
        }
        else
        {
           return ConstValueTable.GetAllData();
        }
    }

    public static ConstValue GetSingleConstValue(int id)
    {
        return ConstValueTable.GetData(id);
    }

    public static List<BuildingAttributesData> GetAllBuildingAttributesDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<BuildingAttributesData>(BuildingAttributesDataTable.GetAllReadonlyData());
        }
        else
        {
           return BuildingAttributesDataTable.GetAllData();
        }
    }

    public static BuildingAttributesData GetSingleBuildingAttributesData(int id)
    {
        return BuildingAttributesDataTable.GetData(id);
    }

    public static List<BuildShopData> GetAllBuildShopDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<BuildShopData>(BuildShopDataTable.GetAllReadonlyData());
        }
        else
        {
           return BuildShopDataTable.GetAllData();
        }
    }

    public static BuildShopData GetSingleBuildShopData(int id)
    {
        return BuildShopDataTable.GetData(id);
    }

    public static List<TutorialTableData> GetAllTutorialTableDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<TutorialTableData>(TutorialTableDataTable.GetAllReadonlyData());
        }
        else
        {
           return TutorialTableDataTable.GetAllData();
        }
    }

    public static TutorialTableData GetSingleTutorialTableData(int id)
    {
        return TutorialTableDataTable.GetData(id);
    }

    public static List<MonsterAttributesData> GetAllMonsterAttributesDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<MonsterAttributesData>(MonsterAttributesDataTable.GetAllReadonlyData());
        }
        else
        {
           return MonsterAttributesDataTable.GetAllData();
        }
    }

    public static MonsterAttributesData GetSingleMonsterAttributesData(int id)
    {
        return MonsterAttributesDataTable.GetData(id);
    }

    public static List<ShieldData> GetAllShieldDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<ShieldData>(ShieldDataTable.GetAllReadonlyData());
        }
        else
        {
           return ShieldDataTable.GetAllData();
        }
    }

    public static ShieldData GetSingleShieldData(int id)
    {
        return ShieldDataTable.GetData(id);
    }

    public static List<RankRewardData> GetAllRankRewardDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<RankRewardData>(RankRewardDataTable.GetAllReadonlyData());
        }
        else
        {
           return RankRewardDataTable.GetAllData();
        }
    }

    public static RankRewardData GetSingleRankRewardData(int id)
    {
        return RankRewardDataTable.GetData(id);
    }

    public static List<TipsData> GetAllTipsDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<TipsData>(TipsDataTable.GetAllReadonlyData());
        }
        else
        {
           return TipsDataTable.GetAllData();
        }
    }

    public static TipsData GetSingleTipsData(int id)
    {
        return TipsDataTable.GetData(id);
    }

    public static List<DestructEventData> GetAllDestructEventDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<DestructEventData>(DestructEventDataTable.GetAllReadonlyData());
        }
        else
        {
           return DestructEventDataTable.GetAllData();
        }
    }

    public static DestructEventData GetSingleDestructEventData(int id)
    {
        return DestructEventDataTable.GetData(id);
    }

    public static List<EnemyData> GetAllEnemyDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<EnemyData>(EnemyDataTable.GetAllReadonlyData());
        }
        else
        {
           return EnemyDataTable.GetAllData();
        }
    }

    public static EnemyData GetSingleEnemyData(int id)
    {
        return EnemyDataTable.GetData(id);
    }

    public static List<TextData> GetAllTextDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<TextData>(TextDataTable.GetAllReadonlyData());
        }
        else
        {
           return TextDataTable.GetAllData();
        }
    }

    public static TextData GetSingleTextData(int id)
    {
        return TextDataTable.GetData(id);
    }

    public static List<DeadLandData> GetAllDeadLandDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<DeadLandData>(DeadLandDataTable.GetAllReadonlyData());
        }
        else
        {
           return DeadLandDataTable.GetAllData();
        }
    }

    public static DeadLandData GetSingleDeadLandData(int id)
    {
        return DeadLandDataTable.GetData(id);
    }

    public static List<RankData> GetAllRankDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<RankData>(RankDataTable.GetAllReadonlyData());
        }
        else
        {
           return RankDataTable.GetAllData();
        }
    }

    public static RankData GetSingleRankData(int id)
    {
        return RankDataTable.GetData(id);
    }

    public static List<DailyTransactionData> GetAllDailyTransactionDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<DailyTransactionData>(DailyTransactionDataTable.GetAllReadonlyData());
        }
        else
        {
           return DailyTransactionDataTable.GetAllData();
        }
    }

    public static DailyTransactionData GetSingleDailyTransactionData(int id)
    {
        return DailyTransactionDataTable.GetData(id);
    }

    public static List<DailyChestsData> GetAllDailyChestsDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<DailyChestsData>(DailyChestsDataTable.GetAllReadonlyData());
        }
        else
        {
           return DailyChestsDataTable.GetAllData();
        }
    }

    public static DailyChestsData GetSingleDailyChestsData(int id)
    {
        return DailyChestsDataTable.GetData(id);
    }

    public static List<TapEventData> GetAllTapEventDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<TapEventData>(TapEventDataTable.GetAllReadonlyData());
        }
        else
        {
           return TapEventDataTable.GetAllData();
        }
    }

    public static TapEventData GetSingleTapEventData(int id)
    {
        return TapEventDataTable.GetData(id);
    }

    public static List<FogData> GetAllFogDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<FogData>(FogDataTable.GetAllReadonlyData());
        }
        else
        {
           return FogDataTable.GetAllData();
        }
    }

    public static FogData GetSingleFogData(int id)
    {
        return FogDataTable.GetData(id);
    }

    public static List<MapObjectData> GetAllMapObjectDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<MapObjectData>(MapObjectDataTable.GetAllReadonlyData());
        }
        else
        {
           return MapObjectDataTable.GetAllData();
        }
    }

    public static MapObjectData GetSingleMapObjectData(int id)
    {
        return MapObjectDataTable.GetData(id);
    }

    public static List<RoleExpData> GetAllRoleExpDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<RoleExpData>(RoleExpDataTable.GetAllReadonlyData());
        }
        else
        {
           return RoleExpDataTable.GetAllData();
        }
    }

    public static RoleExpData GetSingleRoleExpData(int id)
    {
        return RoleExpDataTable.GetData(id);
    }

    public static List<AllianceScienceData> GetAllAllianceScienceDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<AllianceScienceData>(AllianceScienceDataTable.GetAllReadonlyData());
        }
        else
        {
           return AllianceScienceDataTable.GetAllData();
        }
    }

    public static AllianceScienceData GetSingleAllianceScienceData(int id)
    {
        return AllianceScienceDataTable.GetData(id);
    }

    public static List<VegetationData> GetAllVegetationDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<VegetationData>(VegetationDataTable.GetAllReadonlyData());
        }
        else
        {
           return VegetationDataTable.GetAllData();
        }
    }

    public static VegetationData GetSingleVegetationData(int id)
    {
        return VegetationDataTable.GetData(id);
    }

    public static List<SpawnEventData> GetAllSpawnEventDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<SpawnEventData>(SpawnEventDataTable.GetAllReadonlyData());
        }
        else
        {
           return SpawnEventDataTable.GetAllData();
        }
    }

    public static SpawnEventData GetSingleSpawnEventData(int id)
    {
        return SpawnEventDataTable.GetData(id);
    }

    public static List<EmojiData> GetAllEmojiDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<EmojiData>(EmojiDataTable.GetAllReadonlyData());
        }
        else
        {
           return EmojiDataTable.GetAllData();
        }
    }

    public static EmojiData GetSingleEmojiData(int id)
    {
        return EmojiDataTable.GetData(id);
    }

    public static List<HarvestEventData> GetAllHarvestEventDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<HarvestEventData>(HarvestEventDataTable.GetAllReadonlyData());
        }
        else
        {
           return HarvestEventDataTable.GetAllData();
        }
    }

    public static HarvestEventData GetSingleHarvestEventData(int id)
    {
        return HarvestEventDataTable.GetData(id);
    }

    public static List<TimedShopData> GetAllTimedShopDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<TimedShopData>(TimedShopDataTable.GetAllReadonlyData());
        }
        else
        {
           return TimedShopDataTable.GetAllData();
        }
    }

    public static TimedShopData GetSingleTimedShopData(int id)
    {
        return TimedShopDataTable.GetData(id);
    }

    public static List<RandomNameData> GetAllRandomNameDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<RandomNameData>(RandomNameDataTable.GetAllReadonlyData());
        }
        else
        {
           return RandomNameDataTable.GetAllData();
        }
    }

    public static RandomNameData GetSingleRandomNameData(int id)
    {
        return RandomNameDataTable.GetData(id);
    }

    public static List<RandomShopData> GetAllRandomShopDatas(bool getReadonly = false)
    {
        if(getReadonly)
        {
           return new List<RandomShopData>(RandomShopDataTable.GetAllReadonlyData());
        }
        else
        {
           return RandomShopDataTable.GetAllData();
        }
    }

    public static RandomShopData GetSingleRandomShopData(int id)
    {
        return RandomShopDataTable.GetData(id);
    }


}

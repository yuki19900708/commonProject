using UnityEngine;
using System.Collections.Generic;
using SimpleJson;
using GOLib.Tool;
/*API说明
* 保存int    SaveIntValue(string key, int finalValue); value为int  
* 读取int    LoadIntValue(string key, int defaultValu); 返回值为int
* 
* 保存bool   SaveBoolValue(string key, bool finalValue);  value为  true 或者 false
* 读取bool   LoadBoolValue(string key, bool defaultValu); 返回为布尔值
* 
* 保存long   SaveLongValue(string key, long finalValue);
* 读取long   LoadLongValue(string key, long defaultValu); 返回值为long
* 
* 保存string SaveStringValue(string key, string finalVlaue);
* 读取string LoadStringValue(string key, string defaultValu); 返回值为string
* 
* 特殊用法: 见下边示例 2
*/

/// <summary>
/// 玩家偏好存储类
/// </summary>
public partial class PlayerProfile
{
    #region 保存偏好 示例  1
    private static string KeySaveIntTest = "save_int_test";
    public static void SaveIntTest(int _test)
    {
        SaveIntValue(KeySaveIntTest, _test);
    }
    public static int LoadIntTest()
    {
        return LoadIntValue(KeySaveIntTest, 0);
    }
    #endregion

    #region 保存偏好 示例 2  希望示例1获取到bool
    public static bool HasIntTest()
    {
        return LoadIntTest() == 1;
    }
    #endregion

    #region Key区域
    private static string KeyChangeMailCooltime = "KeyChangeMailCooltime";
    public static string KeyGuestAccount = "KeyUserAccount";
    public static string KeyGameLanguageType = "KeyGameLanguageType";
    public static string KeyLeagueLanguageType = "KeyLeagueLanguageType";
    public static string KeyLeagueSort = "KeyLeagueSort";
    public static string KeyLeaderboardRefreshStamp = "KeyLeaderboardRefreshStamp";
    public static string KeyClickToCollect = "KeyClickToCollect";
    public static string KeyThirdpartyAccount = "KeyThirdpartyAccount";
    public static string KeyMusicMute = "KeyMusicMute";
    public static string KeyGameLaunchCount = "KeyGameLaunchCount";
    public static string KeyVersionCache = "KeyVersionCache";
    public static string KeyTheDayBeforeTime = "KeyTheDayBeforeTime";
    public static string KeyChapterCanGetDiamondBuyMythicRawaredCount = "KeyChapterCanGetDiamondBuyMythicRawaredCount";
    public static string KeyChapterGetDiamondBuyRawaredCount = "KeyChapterGetDiamondBuyRawaredCount";

    public static string FristGetTrophyBall = "FristGetTrophyBall";

    ///战斗力分布(战斗区段)
    public static string CombatPowerValueSection = "CombatPowerValueSection";

    /// 首次进入_加载界面
    public static string FristLoadInterface = "FristLoadInterface";
    /// 首次进入_注册界面
    public static string FristRegisterInterface = "FristRegisterInterface";
    /// 首次进入_登录界面
    public static string FristEnterLoginIn = "FristEnterLoginIn";
    /// 首次进入_加载大本营数据
    public static string FristLoadBaseCamp = "FristLoadBaseCamp";
    //首次领取每日宝箱
    public static string FristReviceDailyChest = "FristReviceDailyChest";
    /// <summary>
    /// 首次点击任意高级地的等级
    /// </summary>
    public static string FristClickHighArea = "FristClickHighArea";
    public static string FristPlayStartupComics = "FristPlayStartupComics";
    /// 首次进入_引导
    public static string FristTutorial = "FristTutorial";
    /// 首次进入_引导结束
    public static string FristTutorialOver = "FristTutorialOver";
    /// <summary>
    /// 全部古兽陷入沉睡次数
    /// </summary>
    public static string KeyAllMonsterSleepCount = "AllMonsterSleepCount_{0}";

    /// <summary>
    /// 花费钻石跳过体力时间次数
    /// </summary>
    public static string KeyUseDiamondSkipSleepCount = "UseDiamondSkipSleepCount_{0}";

    /// <summary>
    /// 保存玩家的摄像机视角gameCamera.orthographicSize
    /// </summary>
    public static string GameCameraOrthographicSize = "GameCameraOrthographicSize";


    /// <summary>
    /// 第一次上阵龙的数量
    /// </summary>
    /// <param name="version"></param>
    public static string FristBattleNumber = "FristBattleNumber";
    /// <summary>
    /// 第一次主动退出
    /// </summary>
    /// <param name="version"></param>
    public static string FristPlunderExit = "FristPlunderExit";
    /// <summary>
    /// 第一次点击张礼品求
    /// </summary>
    /// <param name="version"></param>
    public static string FristTipBall = "FristTipBall";

    #endregion

    #region 外部调用

    public static void SaveFristBattleNumber(int number)
    {
        //SaveIntValue(FristBattleNumber + PlayerModel.Data.UserID.ToString(), number);
    }

    //public static int LoadFristBattleNumber()
    //{
        //return LoadIntValue(FristBattleNumber + PlayerModel.Data.UserID.ToString(), 0);
    //}

    public static void SaveFristPlunderExit(int index)
    {
        //SaveIntValue(FristPlunderExit + PlayerModel.Data.UserID.ToString(), index);
    }

    //public static int LoadFristPlunderExit()
    //{
    //    return LoadIntValue(FristPlunderExit + PlayerModel.Data.UserID.ToString(), 0);
    //}


    //public static void SaveFristTipBall(int index)
    //{
    //    SaveIntValue(FristTipBall + PlayerModel.Data.UserID.ToString(), index);
    //}

    //public static int LoadFristTipBall()
    //{
    //    //return LoadIntValue(FristTipBall + PlayerModel.Data.UserID.ToString(), 0);
    //}

    //public static void SaveGameCameraOrthographicSize(int version)
    //{
    //    SaveIntValue(GameCameraOrthographicSize+PlayerModel.Data.UserID.ToString(), version);
    //}

    //public static int LoadGameCameraOrthographicSize()
    //{
    //    return LoadIntValue(GameCameraOrthographicSize + PlayerModel.Data.UserID.ToString(), 65);
    //}

    static int[] sectionValue = { 500,1000, 2500, 5000, 7500, 10000, 12500, 15000,
        17500, 20000, 25000, 30000, 40000,50000,75000,100000};
    /// <summary>
    /// 战斗力值更改
    /// </summary>
    /// <param name="changeValue"></param>
    public static int CombatPowerValueSectionChange(int changeValue)
    {
        int newTagertIndex = 0;
        int value = LoadIntValue(CombatPowerValueSection, 0);
        if (value == 0 && changeValue > value)
        {
            newTagertIndex = 0;
            for (int i = 0; i < sectionValue.Length; i++)
            {
                if (changeValue >= sectionValue[i])
                {
                    newTagertIndex = i;
                }
            }
            SaveIntValue(CombatPowerValueSection, sectionValue[newTagertIndex]);
            return sectionValue[newTagertIndex];
        }
        else
        {
            int oldTagertIndex = 0;
            for (int i = 0; i < sectionValue.Length; i++)
            {
                if (value >= sectionValue[i])
                {
                    oldTagertIndex = i;
                    if (i == sectionValue.Length - 1)
                    {
                        return -1;
                    }
                }
            }

            newTagertIndex = 0;
            for (int i = 0; i < sectionValue.Length; i++)
            {
                if (changeValue >= sectionValue[i])
                {
                    newTagertIndex = i;
                }
            }
            if (newTagertIndex > oldTagertIndex)
            {
                SaveIntValue(CombatPowerValueSection, sectionValue[newTagertIndex]);
                return sectionValue[newTagertIndex];
            }
            return -1;
        }
    }

    public static void SaveFristTutorialOver(int version)
    {
        SaveIntValue(FristTutorialOver, version);
    }

    public static int LoadFristTutorialOver()
    {
        return LoadIntValue(FristTutorialOver, 0);
    }
    public static void SaveFristTutorial(int version)
    {
        SaveIntValue(FristTutorial, version);
    }

    public static int LoadFristTutorial()
    {
        return LoadIntValue(FristTutorial, 0);
    }

    public static void SaveFristLoadBaseCamp(int version)
    {
        SaveIntValue(FristLoadBaseCamp, version);
    }

    public static int LoadFristLoadBaseCamp()
    {
        return LoadIntValue(FristLoadBaseCamp, 0);
    }

    public static void SaveFristReviceDailyChest(int version)
    {
        SaveIntValue(FristReviceDailyChest, version);
    }

    public static int LoadFristReviceDailyChest()
    {
        return LoadIntValue(FristReviceDailyChest, 0);
    }

    public static void SaveFristClickHighArea(int version)
    {
        SaveIntValue(FristClickHighArea, version);
    }

    public static int LoadFristClickHighArea()
    {
        return LoadIntValue(FristClickHighArea, 0);
    }

    public static void SaveFristPlayStartupComics(int value)
    {
        SaveIntValue(FristPlayStartupComics, value);
    }

    public static int LoadFristPlayStartupComics()
    {
        return LoadIntValue(FristPlayStartupComics, 0);
    }

    public static void SaveFristLoadInterface(int version)
    {
        SaveIntValue(FristLoadInterface, version);
    }

    public static int LoadFristLoadInterface()
    {
        return LoadIntValue(FristLoadInterface,0);
    }

    public static void SaveFristRegisterInterface(int version)
    {
        SaveIntValue(FristRegisterInterface, version);
    }

    public static int LoadFristRegisterInterface()
    {
        return LoadIntValue(FristRegisterInterface, 0);
    }

    public static void SaveFristGetTrophyBall(int value)
    {
        SaveIntValue(FristGetTrophyBall, value);
    }

    public static int LoadFristGetTrophyBall()
    {
        return LoadIntValue(FristGetTrophyBall, 0);
    }

    public static void SaveFristEnterLoginIn(int version)
    {
        SaveIntValue(FristEnterLoginIn, version);
    }

    public static int LoadFristEnterLoginIn()
    {
        return LoadIntValue(FristEnterLoginIn, 0);
    }

    public static void SaveTheDayBeforeTime(string value)
    {
        SaveStringValue(KeyTheDayBeforeTime, value);
    }

    public static bool CanGetEicpRewards()
    {
        //得到存储的时间戳
        int time = LoadStringValue(KeyTheDayBeforeTime) == "" ? 0 : int.Parse(LoadStringValue(KeyTheDayBeforeTime));
        if (TimerMgr.GetSeconds() - time > 86400 * 2)
        {
            SaveTheDayBeforeTime((TimerMgr.GetSeconds() - 86400).ToString());
            SaveChapterCanGetDiamondBuyMythicRawaredCount(0);
        }

        if (TimerMgr.GetSeconds() - time > 86400 && LoadIntValue(KeyChapterCanGetDiamondBuyMythicRawaredCount, 0) < 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void AddChapterGetDiamondBuyRawaredCount()
    {
        SaveChapterCanGetDiamondBuyMythicRawaredCount(LoadIntValue(KeyChapterCanGetDiamondBuyMythicRawaredCount, 0) + 1);
    }

    public static void SaveChapterCanGetDiamondBuyMythicRawaredCount(int value)
    {
        SaveIntValue(KeyChapterCanGetDiamondBuyMythicRawaredCount, value);
    }

    public static void SaveChapterGetDiamondBuyRewaredCount(int count)
    {
        SaveIntValue(KeyChapterGetDiamondBuyRawaredCount, count);
    }

    public static int LoadChapterGetDiamondBuyRewaredCount()
    {
        return LoadIntValue(KeyChapterGetDiamondBuyRawaredCount, 0);
    }

    public static void SaveVersionCache(string version)
    {
        SaveStringValue(KeyVersionCache, version);
    }

    public static string LoadVersionCache()
    {
        return LoadStringValue(KeyVersionCache);
    }

    public static void SaveMailCooltime(int value)
    {
        SaveIntValue(KeyChangeMailCooltime, value);
    }

    public static int LoadMailCooltime()
    {
        return LoadIntValue(KeySaveIntTest, 60);
    }

    public static void SaveMusicMuteBoolValue(bool finalValue)
    {
        SaveBoolValue(KeyMusicMute, finalValue);
    }

    public static bool LoadMusicMuteBoolValue()
    {
        return LoadBoolValue(KeyMusicMute, false);
    }

    //public static void SaveGameLanguageType(LanguageTypeEnum value)
    //{
    //    SaveIntValue(KeyGameLanguageType, (int)value);
    //}

    //public static int LoadGameLanguageType()
    //{
    //    return LoadIntValue(KeyGameLanguageType, (int)LanguageTypeEnum.English);
    //}

    public static void SaveLeagueLanguageType(string value)
    {
        SaveStringValue(KeyLeagueLanguageType, value);
    }

    public static string LoadLeagueLanguageType()
    {
        return LoadStringValue(KeyLeagueLanguageType);
    }

    public static void SaveLeagueSort(string value)
    {
        SaveStringValue(KeyLeagueSort, value);
    }

    public static string LoadLeagueSort()
    {
        return LoadStringValue(KeyLeagueSort);
    }

    public static void SaveLeaderboardStamp(int value)
    {
        SaveIntValue(KeyLeaderboardRefreshStamp, value);
    }

    public static int LoadLeaderboardStamp()
    {
        return LoadIntValue(KeyLeaderboardRefreshStamp, 0);
    }

    public static void SaveKeyClickToCollect(string value)
    {
        SaveStringValue(KeyClickToCollect, value);
    }

    public static string LoadKeyClickToCollect()
    {
        return LoadStringValue(KeyClickToCollect);
    }

    //public static void SaveGuestAccountData(AccountData value)
    //{
    //    string jsData = SimpleJson.SimpleJson.SerializeObject(value);

    //    PlayerPrefs.SetString(KeyGuestAccount, jsData);
    //}

    //public static AccountData LoadGuestAccountData()
    //{
    //    string value = PlayerPrefs.GetString(KeyGuestAccount);
    //    if (string.IsNullOrEmpty(value))
    //    {
    //        return null;
    //    }
    //    else
    //    {
    //        AccountData jsData = SimpleJson.SimpleJson.DeserializeObject<AccountData>(value);
    //        return jsData;
    //    }
    //}

    //public static void SaveThirdpartyAccount(AccountData value)
    //{
    //    string jsData = SimpleJson.SimpleJson.SerializeObject(value);

    //    SaveStringValue(KeyThirdpartyAccount, jsData);
    //}

    //public static AccountData LoadThirdpartyAccount()
    //{
    //    string value = LoadStringValue(KeyThirdpartyAccount);
    //    if (string.IsNullOrEmpty(value))
    //    {
    //        return null;
    //    }
    //    else
    //    {
    //        AccountData jsData = SimpleJson.SimpleJson.DeserializeObject<AccountData>(value);

    //        return jsData;
    //    }
    //}

    public static void ClearAccountData()
    {
        PlayerPrefs.DeleteKey(KeyGuestAccount);

        PlayerPrefs.Save();
        PlayerProfile.DeleteKey(KeyThirdpartyAccount);
        DeleteKey(CombatPowerValueSection);
        DeleteKey(FristLoadInterface);
        DeleteKey(FristRegisterInterface);
        DeleteKey(FristEnterLoginIn);
        DeleteKey(FristLoadBaseCamp);
        DeleteKey(FristTutorial);
        DeleteKey(FristTutorialOver);
    }

    public static void SaveGameLaunchCount(int value)
    {
        SaveIntValue(KeyGameLaunchCount, value);
    }

    public static int LoadGameLaunchCount()
    {
        return LoadIntValue(KeyGameLaunchCount, 0);
    }

    public static void SaveAllMonsterSleepCount(int value)
    {
        //string key = string.Format(KeyAllMonsterSleepCount, PlayerModel.Data.UserID);
        //SaveIntValue(key, value);
    }

    //public static int LoadAllMonsterSleepCount()
    //{
        //string key = string.Format(KeyAllMonsterSleepCount, PlayerModel.Data.UserID);
        //return LoadIntValue(key, 0);
    //}

    public static void SaveUseDiamondSkipSleepCount(int value)
    {
        //string key = string.Format(KeyUseDiamondSkipSleepCount, PlayerModel.Data.UserID);
        //SaveIntValue(key, value);
    }

    //public static int LoadUseDiamondSkipSleepCount()
    //{
        //string key = string.Format(KeyUseDiamondSkipSleepCount, PlayerModel.Data.UserID);
        //return LoadIntValue(key, 0);
    //}

    #endregion
}


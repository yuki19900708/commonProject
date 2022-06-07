using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameProto;

public static class TDGAModel {

    //public static TDGAAccount MyAccount;

    public static void TaklingDataInit()
    {
        //Debug.Log(string.Format("TalkingDataSDK init begin key:{0} channel:{1}", GameConfig.TalkingDataAppKey, GameConfig.TalkingDataChannelID));
        //TalkingDataGA.OnStart(GameConfig.TalkingDataAppKey, GameConfig.TalkingDataChannelID);
        //Debug.Log("TalkingDataSDK init completed");

        //TDGAAccount.SetAccount(SystemInfo.deviceUniqueIdentifier);
    }
  
    /// <summary> 统计登录 </summary>
    public static void RecordLoadCount()
    {
        //MyAccount = TDGAAccount.SetAccount(PlayerModel.Data.UserID.ToString());

        //if (!string.IsNullOrEmpty(PlayerModel.Data.UserName))
        //{
        //    MyAccount.SetAccountName(PlayerModel.Data.UserName);
        //}

        //MyAccount.SetAccountType(ConverGameAccountTypeToTDGA(PlayerModel.Data.Account.type));
    }

    //public static AccountType ConverGameAccountTypeToTDGA(Platform_type type)
    //{
    //    AccountType accountType = AccountType.REGISTERED;
    //    if (type == Platform_type.GuestAndriod)
    //    {
    //        accountType = AccountType.TYPE1;
    //    }
    //    else if (type == Platform_type.GuestPad)
    //    {
    //        accountType = AccountType.TYPE2;
    //    }
    //    else if (type == Platform_type.GuestIphone)
    //    {
    //        accountType = AccountType.TYPE3;
    //    }
    //    else if (type == Platform_type.GuestIpad)
    //    {
    //        accountType = AccountType.TYPE4;
    //    }
    //    else if (type == Platform_type.Facebook)
    //    {
    //        accountType = AccountType.TYPE5;
    //    }
    //    else if (type == Platform_type.Twitter)
    //    {
    //        accountType = AccountType.TYPE6;
    //    }

    //    else if (type == Platform_type.Wechat)
    //    {
    //        accountType = AccountType.WEIXIN;
    //    }
    //    else if (type == Platform_type.Weibo)
    //    {
    //        accountType = AccountType.SINA_WEIBO;
    //    }
    //    else if (type == Platform_type.Qq)
    //    {
    //        accountType = AccountType.QQ;
    //    }

    //    return accountType;
    //}

    /// <summary> 记录等级 </summary>
    public static void RecordUserLevel(int level)
    {
#if UNITY_EDITOR
#else
        if (MyAccount != null)
        { 
            MyAccount.SetLevel(level);
        }
#endif
    }

    /// <summary> 请求付费 </summary>
    public static void OnChargeRequest(string orderID, string type, double realMoney, int virtualMoney, string paymentType)
    {
        //string currencyCode = "";
        //TDGAVirtualCurrency.OnChargeRequest(orderID, type, realMoney, currencyCode, virtualMoney, paymentType);
    }

    /// <summary> 完成付费 </summary>
    public static void OnChargeSuccess(string orderID)
    {
        //TDGAVirtualCurrency.OnChargeSuccess(orderID);
}

    public static void UserDefineEvent(SelfEvent type, string info = "")
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add(type.ToString(), info);
        //TalkingDataGA.OnEvent(type.ToString(), dic);
    }

    /// <summary>
    /// 产出付费宝箱
    /// </summary>
    /// <param name="info"></param>
    public static void OutPutPayTreasureChest(int treasureTypeId,int id)
    {
        //string tagertName = "";
        //if ((treasureTypeId >= 720 && treasureTypeId <= 724)|| treasureTypeId==733)
        //{
            //tagertName = "宝箱类型：" + treasureTypeId.ToString() + "宝箱ID：" + id.ToString();
            //SelfEvent type = SelfEvent.PURCHASE_CHEST_PRODUCE;
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic.Add(type.ToString(), tagertName);
            //TalkingDataGA.OnEvent(type.ToString(), dic);
        //}
    }

    /// <summary>
    /// 购买付费宝箱
    /// </summary>
    /// <param name="info"></param>
    public static void BuyPayTreasureChest(int treasureTypeId, int id)
    {
        //string tagertName = "";
        //if ((treasureTypeId >= 720 && treasureTypeId <= 724) || treasureTypeId == 733)
        //{
        //    tagertName = "宝箱类型：" + treasureTypeId.ToString() + "宝箱ID：" + id.ToString();
        //    SelfEvent type = SelfEvent.PURCHASE_CHEST_BUY;
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add(type.ToString(), tagertName);
        //    //TalkingDataGA.OnEvent(type.ToString(), dic);
        //}
    }

    static int[] CupRecord = {-2500,-2000,-1500,-1000,-500,-250,-100,100,250,500,1000,1500,2000,2500} ;

    /// <summary>
    /// 掠夺杯数差记录
    /// </summary>
    public static void PlunderMatchCupRecording(int selfMount,int enemyMount)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        int value = selfMount - enemyMount;
        //int TagertValue = 0;
        for (int i = 0; i < CupRecord.Length; i++)
        {
            if (value <= CupRecord[i])
            {
                //TagertValue = CupRecord[i];
            }
        }
        dic.Add(SelfEvent.PLUNDER_MATCH_CUP_RECORDING.ToString(), 
            "Difference:"+ (selfMount- enemyMount).ToString() + "     self cup:" + selfMount.ToString()+ "  enemyMount cup:" + enemyMount.ToString());
        //TalkingDataGA.OnEvent(SelfEvent.PLUNDER_MATCH_CUP_RECORDING.ToString() + TagertValue.ToString(), dic);
    }
}

#region 自定义事件枚举
public enum SelfEvent
{
    VIP_LEVEL,
    CHANGE_AVATAR,
    CHANGE_FRAME,
    MODIFY_NAME,
    SPECIAL_OFFER_PRODUCE,
    SPECIAL_OFFER_BUY,
    PURCHASE_CHEST_PRODUCE,
    PURCHASE_CHEST_BUY,
    //解锁地图迷雾区域；
    MAP_UNLOCK,
    //购买地图迷雾区域
    MAP_BUY,
    /// <summary>
    /// 掠夺次数
    /// </summary>
    PLUNDER_COUNT,
    /// <summary>
    /// 掠夺胜利
    /// </summary>
    PLUNDER_WIN,
    /// <summary>
    /// 掠夺失败
    /// </summary>
    PLUNDER_FAIL,
    /// <summary>
    /// 掠夺复仇
    /// </summary>
    PLUNDER_LOOSE,
    /// <summary>
    /// 掠夺匹配杯数差记录
    /// </summary>
    PLUNDER_MATCH_CUP_RECORDING,
    /// <summary>
    /// 掠夺战斗力分布
    /// </summary>
    PLUNDER_COMBAT_POWER_DISTRIBUTION,
    PLUNDER_TAP_COMBATRECORD,
    PLUNDER_TAP_ARENA,
    PLUNDER_TAP_MAIN,
    PLUNDER_MATCH_PLAYER,
    PLUNDER_MATCH_AI,
    /// <summary>
    /// 掠夺匹配到高战力
    /// </summary>
    PLUNDER_MATCH_HIGH_WARFARE,
    SHEILD_BUY_3,
    SHEILD_BUY_8,
    SHEILD_BUY_16,
    SHEILD_GIVE_UP,
    LEAGUE_TECH_LOW_LEVEL_UPGRADE,
    LEAGUE_TECH_HIGH_LEVEL_UPGRADE,
    //玩家比如A申请联盟的总个数
    LEAGUE_APPLY_TOTAL_COUNT,
    //盟主接受申请的次数
    LEAGUE_APPLY_ACCEPT_COUNT,
    //盟主拒绝申请的次数
    LEAGUE_APPLY_REFUSE_COUNT,
    CHEST_SHOP_BUY_COUNT,
    CHEST_SHOP_BUY_ITEM,
    EGG_SHOP_BUY_COUNT,
    EGG_SHOP_BUY_ITEM,
    BUILDING_SHOP_BUY_COUNT,
    BUILDING_SHOP_BUY_ITEM,
    LEVEL_SHOP_BUY_COUNT,
    VIP_SHOP_REFRESH_COUNT,
    VIP_SHOP_REFRESH_FREE,
    VIP_SHOP_BUY_COUNT,
    VIP_SHOP_OPEN_UI,
    VIP_SHOP_BUY_ITEM,
    LIMIT_SHOP_BUY_COUNT,
    LIMIT_SHOP_GET_FREE_ITEM,
    LIMIT_SHOP_BUY_ITEM,
    //体力购买次数
    AP_BUY_COUNT,
    //购买的体力点数
    AP_BUY_POINT_COUNT,
    //消耗的体力点数
    AP_USE_POINT_COUNT,
    //好友领取体力次数
    AP_RECEIVE_POINT_COUNT,
    LEVEL_FIRST_FINISHED,
    LEVEL_BUY_DOUBLE_REWARD,
    LEVEL_SHOW_BUY_DOUBLE_REWARD,
    //关卡胜利掉落宝箱；
    LEVEL_DROP_CHEST,
    //领取关卡宝箱，即关卡胜利， 宝箱为选中状态，
    LEVEL_RECIEVE_CHEST,
    //钻石购买关卡宝箱
    DIAMOND_BUY_LEVEL_CHEST,
    CHEST_NORMAL_UNLOCK,
    CHEST_UNLOCK_BY_DIAMOND,
    TASK_SKIP_COUNT,
    TASK_FINISH_COUNT,
    SET_CONTINUE_MERGE_ON,
    SET_CONTINUE_MERGE_OFF,
    SET_AUTO_COLLECT_ON,
    SET_AUTO_COLLECT_OFF,
    SET_ENSURE_CLICK_ON,
    SET_ENSURE_CLICK_OFF,
    SET_MUSIC_ON,
    SET_MUSIC_OFF,
    SET_SFX_ON,
    SET_SFX_OFF,
    SET_OVERLAPPING_ON,
    SET_OVERLAPPING_OFF,
    SET_BIND_FACEBOOK,
    SET_BIND_TWITTER,
    SET_CLICK_RATE,
    SET_CLICK_CONTACT_US,
    SET_ACTIVITY_ON,
    SET_ACTIVITY_OFF,
    SET_STAGE_CHESTS_ON,
    SET_STAGE_CHESTS_OFF,
    SET_SHEID_TIME_ON,
    SET_SHEID_TIME_OFF,
    SET_DAILY_CHEST_ON,
    SET_DAILY_CHEST_OFF,
    STARTUP_COMIC,
    LOADING_PANEL,
    LOGIN_PANEL,
    TUTORIAL,
    MAINUI_CLICK_AVATAR,
    MAINUI_CLICK_VIP,
    MAINUI_CLICK_TASK,
    MAINUI_CLICK_LEAGUE,
    MAINUI_CLICK_PLUNDER,
    MAINUI_CLICK_SHIELD,
    MAINUI_CLICK_CHAPTER,
    MAINUI_CLICK_MONSTER,
    MAINUI_CLICK_ACTIVITY,
    MAINUI_CLICK_CHAT,
    MAINUI_CLICK_EXTEND_ON,
    MAINUI_CLICK_EXTEND_OFF,
    MAINUI_CLICK_FRIEND,
    MAINUI_CLICK_MAIL,
    MAINUI_CLICK_HANDBOOK,
    MAINUI_CLICK_SHOP,
    MAINUI_CLICK_PURCHASE,
    SHOP_TAP_LIMIT,
    SHOP_TAP_EGG,
    SHOP_TAP_CHEST,
    SHOP_TAP_BUILDING,
    SHOP_TAP_PURCHASE,
    #region 首次
    /// <summary>
    /// 首次进入_开场动画
    /// </summary>
    FRIST_OPEN_ANIMATION,
    /// <summary>
    /// 结束开场动画
    /// </summary>
    FRIST_END_ANIMATION,
    /// <summary>
    /// 首次进入_资源加载界面
    /// </summary>
    FRIST_LOAD_INTERFACE,
    /// <summary>
    /// 首次退出资源加载页面
    /// </summary>
    FIRST_END_LOAD_INTERFACE,
    /// <summary>
    /// 首次进入大本营加载页面
    /// </summary>
    FIRST_BASECAMP_LOAD_INTERFACE,
    /// <summary>
    /// 首次退出大本营加载页面
    /// </summary>
    FIRST_END_BASECAMP_LOAD_INTERFACE,
    /// <summary>
    /// 首次进入_注册界面
    /// </summary>
    FRIST_REGISTRATIOB_INTERFACE,
    /// <summary>
    /// 首次进入_登录界面
    /// </summary>
    FRIST_ENTER_LOGININ,
    /// <summary>
    /// 首次进入_加载大本营数据
    /// </summary>
    FRIST_LOAD_BASE_CAMP,
    /// <summary>
    /// 首次进入_引导
    /// </summary>
    FRIST_TUTORIAL,
    /// <summary>
    /// 首次进入_引导结束
    /// </summary>
    FRIST_TUTORIAL_OVER,
    #endregion
    #region 登陆界面UI
    /// <summary>
    /// 游客注册
    /// </summary>
    TOURIST_REGISTRATION,
    /// <summary>
    /// Facebook注册
    /// </summary>
    FACKBOOK_REGISTRATION,
    /// <summary>
    /// 推特注册
    /// </summary>
    TT_REGISTRATION,
    /// <summary>
    /// Facebook绑定
    /// </summary>
    FACKBOOK_BIND,
    /// <summary>
    /// 推特绑定
    /// </summary>
    TT_BIND,
    /// <summary>
    /// 进入游戏
    /// </summary>
    ENTER_GAME,
    /// <summary>
    /// 自动修复
    /// </summary>
    AUTO_REPAIR,
    /// <summary>
    /// 轻度修复
    /// </summary>
    MILD_REPAIR,
    /// <summary>
    /// 高级修复
    /// </summary>
    ADVANCE_REPAIR,
    #endregion
    #region
    /// <summary>
    /// 联盟聊天数量
    /// </summary>
    ALLIANCE_CHAT_MOUNT,
    /// <summary>
    /// 世界聊天数量
    /// </summary>
    WORLD_CHAT_MOUNT,
    #endregion
    /// <summary>
    /// 排行按钮被点击
    /// </summary>
    RAND_BUTTON_TAP,

    #region 3月1日新增统计项
    /// <summary>关卡选择页面宝箱按钮 </summary>
    CHAPTER_BOXBUTTON_CLICK,
    /// <summary>关卡选择以及场景内充值按钮 </summary>
    CHAPTER_RECHARGEBUTTON_CLICK,
    /// <summary>关卡选择页面跳转当前进度 </summary>
    CHAPTER_SKIP_CURRENT_RATE,
    /// <summary>关卡购买体力按钮点击 </summary>
    CHAPTER_AP_BUTTON,
    /// <summary>大本营内每日宝箱建筑点击 </summary>
    BASECAMP_DAILY_CHEST_BUTTON_CLICK,
    /// <summary>大本营关卡宝箱建筑点击 </summary>
    BASECAMP_LEVEL_CHEST_BUTTON_CLICK,
    /// <summary>领取第N个每日福利 </summary>
    RECEIVE_DAILY_WELFARE,
    /// <summary>关卡检查星数 </summary>
    CHAPTER_CHECK_TASK_STR,
    /// <summary>领取第N个在线奖励的阶段奖励 </summary>
    RECEIVE_STAGE_REWARED,
    /// <summary>查看高端区按钮 </summary>
    BASECAMP_CHECK_HIGH_AREA,
    /// <summary>第一次进入第一关 </summary>
    FIRST_IN_CHAPTER_1,
    /// <summary>第一次完成第一关 </summary>
    FIRST_END_CHAPTER_1,
    /// <summary>第一次进入第二关 </summary>
    FIRST_IN_CHAPTER_2,
    /// <summary>第一次完成第二关 </summary>
    FIRST_END_CHAPTER_2,
    /// <summary>第一次进入第三关 </summary>
    FIRST_IN_CHAPTER_3,
    /// <summary>第一次完成第三关 </summary>
    FIRST_END_CHAPTER_3,
    /// <summary>第一次进入第四关 </summary>
    FIRST_IN_CHAPTER_4,
    /// <summary>第一次完成第四关 </summary>
    FIRST_END_CHAPTER_4,
    /// <summary>第一次进入第五关 </summary>
    FIRST_IN_CHAPTER_5,
    /// <summary>第一次完成第五关 </summary>
    FIRST_END_CHAPTER_5,
    /// <summary>第一次购买第五关双倍奖励 </summary>
    FRIST_BUY_CHAPTER_5_DOUBLE_REWARED,
    /// <summary>解锁第一块高端区的等级 </summary>
    UNLOCK_FIRST_PIECE_HIGH_AREA_LEVEL,
    /// <summary>第一次领取每日宝箱的等级 </summary>
    FRIST_RECIVE_DIDAY_CHEST_LEVEL,
    /// <summary>第一次打开每日宝箱的等级 </summary>
    FRIST_OPEN_DIDAY_CHEST_LEVEL,
    /// <summary>第一次掠夺出战龙的个数 </summary>
    FRIST_PLUNDER_DRAGON_COUNT,
    /// <summary>第一次掠夺提前退出战斗 </summary>
    FRIST_PLUNDER_EXIT,
    /// <summary>第一次点击战利品球 </summary>
    FRIST_CLICK_TROPHY_BALL,
    /// <summary>第一次古兽全部沉睡的等级 </summary>
    FRIST_DRAGON_ALL_SLEEP_LEVEL,
    /// <summary>第二次古兽全部沉睡的等级 </summary>
    TWO_DRAGON_ALL_SLEEP_LEVEL,
    /// <summary>第一次花费钻石跳过龙休息的等级 </summary>
    FRIST_SKIP_DRAGON_SLEEP_LEVEL,
    /// <summary>第一次点击高端区的等级 </summary>
    FRIST_CLICK_HIGH_AREA_LEVEL,
    #endregion
}
#endregion

using UnityEngine;
using System.Collections;

public static class UIDef 
{
    public const int UI_MAX__DEPTH = 1500;
    #region UI名称定义
    public const string ReConnectUI = "ReConnectUI";
    public const string StartupComicsUI = "StartupComicsUI";
    public const string MapMgrTipCtrl = "MapMgrTipUI";
    public const string MainUI = "MainUI";
    public const string PlunderUI = "PlunderUI";
    public const string PlunderInProgressUI = "PlunderInProgressUI";
    public const string PlunderInitialPromptUI = "PlunderInitialPromptUI";
    public const string ShopUI = "ShopUI";
    public const string CurrencyNotEnoughUI = "CurrencyNotEnoughUI";
    public const string EnsureUI = "EnsureUI";
    public const string LoginUI = "LoginUI";
    public const string ShieldShopUICtrl = "ShieldShopUICtrl";
    public const string SettingUI = "SettingUI";
    public const string SendMailUI = "SendMailUI";
    public const string AnnouncementAndMailUI = "AnnouncementsAndMailUI";
    public const string InGameAnnouncementUI = "InGameAnnouncementUI";
    public const string InformUI = "InformUI";
    public const string ItemInfoUI = "ItemInfoUI";
    public const string DailyTransactionUI = "DailyTransactionUI";
    public const string ChapterSelectUI = "ChapterSelectUI";
    public const string ChapterSceneUI = "ChapterSceneUI";
    public const string ChapterBillingPageUI = "ChapterBillingPageUI";
    public const string AncientGodGuideUI = "AncientGodGuideUI";
    public const string ChestUI = "ChestUI";
    public const string AvatarUI = "AvatarUI";
    public const string TaskStarUI = "TaskStarUI";
    public const string LeaderboardUI = "LeaderboardUI";
    public const string LeagueUI = "LeagueUI";
    public const string ChooseLanguageUI = "ChooseLanguageUI";
    public const string BuySpecialUI = "BuySpecialUI";
    public const string NetErrorUI = "NetErrorUI";
    public const string MainAndChapterSwichUI = "MainAndChapterSwichUI";

    public const string LinkServerUI = "LinkServerUI";
    /// <summary>
    /// 高端区解锁提示UI
    /// </summary>
    public const string High_EndAreaUI = "High_EndAreaUI";
    public const string DragonPowerTipUI = "DragonPowerTipUI";
    /// <summary>
    /// 体力足够界面
    /// </summary>
    public const string APEnoughUI = "APEnoughUI";
    /// <summary>
    /// 体力已满界面
    /// </summary>
    public const string APFullUI = "APFullUI";
    /// <summary>
    /// 体力不足界面
    /// </summary>
    public const string APNotEnoughUI = "APNotEnoughUI";
    /// <summary>
    /// 体力不满界面
    /// </summary>
    public const string APNotFullUI = "APNotFullUI";
    /// <summary>
    /// 体力不足教程UI
    /// </summary>
    public const string APNotEnoughTutorialUI = "APNotEnoughTutorialUI";
    /// <summary>
    /// 任务界面
    /// </summary>
    public const string TaskUI = "TaskUI";
    /// <summary>
    /// 好友界面
    /// </summary>
    public const string FriendUI = "FriendUI";
    /// <summary>
    /// 删除好友确认界面
    /// </summary>
    public const string DeleteFriendConfirmUI = "DeleteFriendConfirmUI";
    /// <summary>
    /// 跳过休息时间界面
    /// </summary>
    public const string SkipTimeUI = "SkipTimeUI";
    /// <summary>
    /// 用户详细信息界面
    /// </summary>
    public const string UserInforUI = "UserInforUI";
    /// <summary>
    /// 玩家详细信息界面
    /// </summary>
    public const string PlayerDetailedInforUI = "PlayerDetailedInforUI";
    /// <summary>
    /// 修改名字介绍界面
    /// </summary>
    public const string ModifyNameIntroduceUI = "ModifyNameIntroduceUI";
    /// <summary>
    /// 修改名字界面
    /// </summary>
    public const string ModifyNameUI = "ModifyNameUI";
    /// <summary>
    /// 点击物体卖出界面
    /// </summary>
    public const string SellGoodWindowUI = "SellGoodWindowUI";
    /// <summary>
    /// 点击护盾问号护盾描述界面
    /// </summary>
    public const string DescriptionWindowUI = "DescriptionWindowUI";
    /// <summary>
    /// 打开场景中宝箱 购买宝箱界面
    /// </summary>
    public const string NotEnoughDiamondBuyTreasureChestUI = "NotEnoughDiamondBuyTreasureChestUI";
    /// <summary>
    /// 物品图鉴UI
    /// </summary>
    public const string HandbookUI = "HandbookUI";
    /// <summary>
    /// 生物图鉴UI
    /// </summary>
    public const string MonsterHandbookUI = "MonsterHandbookUI";
    public const string MonsterDetailUI = "MonsterDetailUI";
    /// <summary>
    /// 聊天界面UI
    /// </summary>
    public const string ChatUI = "ChatUI";
    /// <summary>
    /// VIP界面
    /// </summary>
    public const string VIPUI = "VIPUI";
    /// <summary>
    /// 物品信息界面
    /// </summary>
    public const string MapObjectMsgUI = "MapObjectMsgUI";
    /// <summary>
    /// 活动UI界面
    /// </summary>
    public const string AllActivityUI = "AllActivityUI";
    /// <summary>
    /// 退出游戏界面（仅Android上出现）
    /// </summary>
    public const string QuitGameUI = "QuitGameUI";
    /// <summary>
    /// 玩家消息进入接口UI
    /// </summary>
    public const string PlayerMsgUI = "PlayerMsgUI";
    /// <summary>
    /// 玩家消息进入接口UI
    /// </summary>
    public const string VIPLevelUpUI = "VIPLevelUpUI";
    /// <summary>
    /// 通用的确认对话框，需要自行设定各项内容
    /// </summary>
    public const string ConfirmUI = "ConfirmUI";
    /// <summary>
    /// 通用玩家信息展示界面
    /// </summary>
    public const string LeagueDetailPanelUI = "LeagueDetailPanelUI";
    #endregion

    /// <summary>
    /// 获取界面层级
    /// </summary>
    /// <param name="uiName">UI名称</param>
    /// <returns></returns>
    public static int GetDepth(string uiName)
    {
        switch(uiName)
        {
            case MainUI:
            case ChapterSelectUI:
            case ChapterSceneUI:
                return 1000;
            case AvatarUI:
            case BuySpecialUI:
            case ChapterBillingPageUI:
            case UserInforUI:
                return 1001;
            case PlunderUI:
            case VIPUI:
            case AncientGodGuideUI:
            case AnnouncementAndMailUI:
                return 1001;
            case PlunderInProgressUI:
            case PlunderInitialPromptUI:
                return 1002;
            case HandbookUI:
            case ChestUI:
            case LeaderboardUI:
            case LeagueUI:
            case ItemInfoUI:
                return 1003;
            case ChooseLanguageUI:
            case MapMgrTipCtrl:
            case MonsterHandbookUI:
            case ShieldShopUICtrl:
            case DailyTransactionUI:
            case ChatUI:
            case AllActivityUI:
                return 1004;
            case ShopUI:
            case ModifyNameIntroduceUI:
            case ModifyNameUI:
                return 1015;
            case EnsureUI:
            case PlayerMsgUI:
            case LeagueDetailPanelUI:
                return 1057;
            case PlayerDetailedInforUI:
            case SellGoodWindowUI:
                return 1099;
            case MapObjectMsgUI:
                return 1100;
            case CurrencyNotEnoughUI:
            case DescriptionWindowUI:
            case DeleteFriendConfirmUI:
            case NotEnoughDiamondBuyTreasureChestUI:
                return 1101;
            case LoginUI:
            case MainAndChapterSwichUI:
            case SettingUI:
                return 1205;
            case VIPLevelUpUI:
                return 4010;
            case SendMailUI:
                return 1300;
            case LinkServerUI:
                return 1301;
            case InformUI:
            case NetErrorUI:
            case ReConnectUI:
                return 5900;
            case StartupComicsUI:
                return 6000;
            case ConfirmUI:
                return 6001;
            case QuitGameUI:
                return 6002;
            default:
                return 1001;
        }
    }

    /// <summary>
    /// 获取界面层级
    /// </summary>
    /// <param name="uiName">UI名称</param>
    /// <returns></returns>
    public static void SetOrderLayer(UIBase ui, string uiName)
    {
        int layer = GetDepth(uiName);
        Canvas canvas = ui.GetComponent<Canvas>();
        canvas.worldCamera = CameraGestureMgr.Instance.uiCamera;
        canvas.gameObject.layer = LayerMask.NameToLayer("UI");
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = layer;
    }
}

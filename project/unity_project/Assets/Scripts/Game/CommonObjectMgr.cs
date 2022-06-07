using System;
using System.Collections.Generic;
using UnityEngine;

public class CommonObjectMgr : MonoBehaviour
{
    private static CommonObjectMgr instance;

    public static Transform ClickToCollectParent
    {
        get
        {
            return instance.clickToCollectParent;
        }
    }

    public static Transform UIRoot
    {
        get { return instance.uiRoot; }
    }

    public static Transform PoolParent
    {
        get
        {
            return instance.poolParent;
        }
    }

    public static Transform CollectProgressbarParent
    {
        get
        {
            return instance.collectProgressbarParent;
        }
    }

    public static Transform DerivativeProgressbar
    {
        get
        {
            return instance.derivativeProgressbar;
        }
    }

    public static Transform BuildSliderParent
    {
        get
        {
            return instance.buildSliderParent;
        }
    }

    public static Transform SleepSliderParent
    {
        get
        {
            return instance.sleepSliderParent;
        }
    }

    public static Transform HPSliderParent
    {
        get
        {
            return instance.hpSliderParent;
        }
    }

    public static Transform BulletParent
    {
        get
        {
            return instance.bulletParent;
        }
    }

    public static Transform PolluteSliderParent
    {
        get
        {
            return instance.polluteSliderParent;
        }
    }

    public static Transform MapObjectTipParent
    {
        get
        {
            return instance.mapObjectTipParent;
        }
    }

    public static Transform DragonPowerParent
    {
        get
        {
            return instance.dragonPowerParent;
        }
    }

    public static Transform PurifyProgressParent
    {
        get
        {
            return instance.purifyProgressParent;
        }
    }

    public static Transform FirstLevel_WorldSpace
    {
        get { return instance.firstlevel_WorldSpace; }
    }

    public static Transform FirstLevel_ScreenSpace
    {
        get { return instance.firstlevel_ScreenSpace; }
    }

    //public static MonoBehaviourPool<FirstLevelTip> FirstLevelTipPool
    //{
    //    get { return instance.firstLevelTipPool; }
    //}

    //public static SecondlevelTipUI SecondLevelTip
    //{
    //    get { return instance.secondLevelTip; }
    //}

    //public static ThirdLevelTipUI ThirdLevelTip
    //{
    //    get { return instance.thirdLevelTip; }
    //}

    //public static GetObjectUICtrl GetObjectCtrl
    //{
    //    get { return instance.getObjectUI; }
    //}

    public static Transform GuidanCanvasUI
    {
        get { return instance.guidanCanvasUI; }
    }

    public Transform uiRoot;
    public Transform poolParent;
    public Transform collectProgressbarParent;
    public Transform derivativeProgressbar;
    public Transform buildSliderParent;
    public Transform sleepSliderParent;
    public Transform hpSliderParent;
    public Transform bulletParent;
    public Transform polluteSliderParent;
    public Transform mapObjectTipParent;
    public Transform dragonPowerParent;
    public Transform purifyProgressParent;
    public Transform firstlevel_ScreenSpace;
    public Transform firstlevel_WorldSpace;
    public Transform clickToCollectParent;
    public Transform guidanCanvasUI;

    //private MonoBehaviourPool<FirstLevelTip> firstLevelTipPool;
    //private SecondlevelTipUI secondLevelTip;
    //private ThirdLevelTipUI thirdLevelTip;
    //private GetObjectUICtrl getObjectUI;

    private void Awake()
    {
        instance = this;
        //ResUpdateMgr./*Event_DownloadFinish*/ += LoadPrefab;
    }

    private void LoadPrefab()
    {
        //if (secondLevelTip == null)
        //{
        //    SecondlevelTipUI secondRes = ResMgr.Load<SecondlevelTipUI>("SecondLevelTipUI");
        //    secondLevelTip = Instantiate(secondRes);
        //    secondLevelTip.transform.SetParent(uiRoot);
        //}
        //if (thirdLevelTip == null)
        //{
        //    ThirdLevelTipUI thirdRes = ResMgr.Load<ThirdLevelTipUI>("ThirdLevelTipUI");
        //    thirdLevelTip = Instantiate(thirdRes);
        //    thirdLevelTip.transform.SetParent(uiRoot);
        //}
        //if (getObjectUI == null)
        //{
        //    GetObjectUICtrl getObjectRes = ResMgr.Load<GetObjectUICtrl>("GetObjectUI");
        //    getObjectUI = Instantiate(getObjectRes);
        //    getObjectUI.transform.SetParent(uiRoot);
        //    Canvas canvas = getObjectUI.GetComponent<Canvas>();
        //    canvas.worldCamera = CameraGestureMgr.Instance.uiCamera;
        //    canvas.sortingLayerName = "UI";
        //}
        //if (firstLevelTipPool == null)
        //{
        //    firstLevelTipPool = new MonoBehaviourPool<FirstLevelTip>(ResMgr.Load<FirstLevelTip>("FirstLevelTip"), CommonObjectMgr.PoolParent);
        //}
    }

    public static void ClearTip()
    {
        //    if (instance.thirdLevelTip != null)
        //    {
        //        instance.thirdLevelTip.Clear();
        //    }
        //    if(instance.secondLevelTip != null)
        //    {
        //        instance.secondLevelTip.Clear();
        //    }
        //    if (instance.getObjectUI != null)
        //    {
        //        instance.getObjectUI.Clear();
        //    }
        //    if (instance.firstLevelTipPool != null)
        //    {
        //        List<FirstLevelTip> usingList = instance.firstLevelTipPool.GetOutPoolList();
        //        for (int i = 0; i < usingList.Count; i++)
        //        {
        //            usingList[i].ClearAnimation();
        //        }
        //    }
    }
}

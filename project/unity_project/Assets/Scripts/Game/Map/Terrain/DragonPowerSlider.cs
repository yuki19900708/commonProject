using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;
//using Universal.Tutorial;
using GameProto;
using Google.Protobuf;

public class DragonPowerSlider : MonoBehaviour
{
    public Action<DragonPowerSlider> Event_MapUnlcok;
    enum LockType
    {
        General = 1,
        Recharge,
    }

    public GameObject lockGameObject;
    public GameObject unlockGameObject;

    //public TextMeshProUGUI lockTipText;
    //public TextMeshProUGUI powerProgressText;
    public Button showInfoButton;
    public Image showInfoButtonIcon;
    public Image showInfoButtonMengCengIcon;
    public Slider slider;

    //public TextMeshProUGUI unlockTipText;
    public Button unlockButton;

    public MapUnlockData data;
    private bool isUnLock = false;

    private void Awake()
    {
        showInfoButton.onClick.AddListener(OnShowInfoButtonClick);
        unlockButton.onClick.AddListener(OnUnlockButtonClick);
        //unlockTipText.text = L10NMgr.GetText(22700007);
    }

    public void InitData(Point point, MapUnlockData data)
    {
        isUnLock = false;
        lockGameObject.SetActive(false);
        unlockGameObject.SetActive(false);
        unlockGameObject.transform.localScale = Vector3.one;
        this.data = data;
        if (data.type == 1)
        {
            Sprite sp = UGUISpriteAtlasMgr.LoadSprite("button_monster_power", AtlasDef.BaseCamp);
            showInfoButtonIcon.SetSpritePreserveAspectAndSize(sp, new Vector2(134, 133));
            showInfoButtonMengCengIcon.SetSpritePreserveAspectAndSize(sp, new Vector2(62, 62));
            MapMgr.Event_BiologicalPowerChange += EventBiologicalPowerChange;
            //lockTipText.text = L10NMgr.GetText(22700006);
            if (data.count > MapMgr.biologicalPower)
            {
                slider.value = (float)MapMgr.biologicalPower / (float)data.count;
                //string sr = string.Format(ComUtil.Text_Cusomize_Color, "F2F3E4", MapMgr.biologicalPower + "/");
                //powerProgressText.text = string.Format("{0}{1}", sr, data.count);
                lockGameObject.SetActive(true);
            }
            else
            {
                isUnLock = true;
                unlockGameObject.SetActive(true);
                unlockGameObject.transform.DOScale(1.2f, 1).SetLoops(-1, LoopType.Yoyo).SetDelay(UnityEngine.Random.Range(1.0f, 2.0f));
            }
        }
        else
        {
            MapMgr.Event_CostDiamondTotal += EventCostDiamondTotalChange;
            //lockTipText.text = L10NMgr.GetText(30800001);
            if (data.consume > MapMgr.CostDiamondTotal)
            {
                Sprite sp = UGUISpriteAtlasMgr.LoadSprite("button_cost_diamond_total", AtlasDef.BaseCamp);
                showInfoButtonIcon.SetSpritePreserveAspectAndSize(sp, new Vector2(134, 133));
                showInfoButtonMengCengIcon.SetSpritePreserveAspectAndSize(sp, new Vector2(62, 62));

                slider.value = (float)MapMgr.CostDiamondTotal / (float)data.consume;
                //string sr = string.Format(ComUtil.Text_Cusomize_Color, "F2F3E4", MapMgr.CostDiamondTotal + "/");
                //powerProgressText.text = string.Format("{0}{1}", sr, data.consume);
                lockGameObject.SetActive(true);
            }
            else
            {
                isUnLock = true;
                unlockGameObject.SetActive(true);
                unlockGameObject.transform.DOScale(1.2f, 1).SetLoops(-1, LoopType.Yoyo).SetDelay(UnityEngine.Random.Range(1.0f, 2.0f));
            }
        } 
        transform.position = MapMgr.Instance.GetWorldPosByPoint(point);
    }

    public void ClearPrefab()
    {
        Event_MapUnlcok = null;
        MapMgr.Event_BiologicalPowerChange -= EventBiologicalPowerChange;
        MapMgr.Event_CostDiamondTotal -= EventCostDiamondTotalChange;
        unlockGameObject.transform.DOKill();
    }

    public void RunUnlock()
    {
        unlockGameObject.SetActive(false);
        MapMgr.SendServerUnlockTerrain(this);
        StartCoroutine(PlayAnimation());
    }

    private void OnUnlockButtonClick()
    {
        //if (data.type == (int)LockType.General)
        //{
            RunUnlock();
            //TutorialModel.NextStep(TutorialSection.Evilforest, TutorialMgr.Evilforest_Wait);
            Debug.Log("当龙之力足够 时被点击并且 类型为一般锁定的话 直接进行解锁");
        //}
        //else
        //{
        //    MapMgr.ShowUnlockHighEndArea(this);
        //    Debug.Log("LY", "当龙之力足够 时被点击并且 类型为充值解锁的话 弹出升级页面");
        //}
    }

    private void OnShowInfoButtonClick()
    {
        if (data.type == (int)LockType.General)
        {
            MapMgr.ShowDragonPowerTip(data.count);
        }
        else
        {
            //NetMgr.Send(NetAPIDef.eCTS_GAMESER_COST_DIAMOND_TOTAL, null, NetCallBack);
        }
    }

    private void NetCallBack(int msgID, IMessage response, bool isSucceed)
    {
        Response_cost_diamond_total costDiamond = (Response_cost_diamond_total)response;
        MapMgr.CostDiamondTotal = (int)costDiamond.Total;
        MapMgr.ShowUnlockHighEndArea(this);
    }

    private void EventCostDiamondTotalChange()
    {
        if (isUnLock)
        {
            // Debug.Log("LY","已经触发过解锁了  不再收到龙之力变更的影响");
        }
        else
        {
            if (MapMgr.CostDiamondTotal >= data.consume)
            {
                isUnLock = true;
                lockGameObject.SetActive(false);
                unlockGameObject.SetActive(true);

                unlockGameObject.transform.DOScale(1.2f, 1).SetLoops(-1, LoopType.Yoyo).SetDelay(UnityEngine.Random.Range(1.0f, 2.0f));
            }
            else
            {
                slider.value = (float)MapMgr.CostDiamondTotal / (float)data.consume;
                //powerProgressText.text = string.Format("{0}/{1}", MapMgr.CostDiamondTotal, data.consume);
            }
        }
    }

    private void EventBiologicalPowerChange(int obj)
    {
        if (isUnLock)
        {
           // Debug.Log("LY","已经触发过解锁了  不再收到龙之力变更的影响");
        }
        else
        {
            if (obj >= data.count)
            {
                isUnLock = true;

                if (data.id == 100 && unlockGameObject.activeSelf == false)
                {
                    CameraGestureMgr.Instance.MoveCamera(transform.position);
                }

                lockGameObject.SetActive(false);
                unlockGameObject.SetActive(true);

                unlockGameObject.transform.DOScale(1.2f, 1).SetLoops(-1, LoopType.Yoyo).SetDelay(UnityEngine.Random.Range(1.0f, 2.0f));
            }
            else
            {
                slider.value = (float)MapMgr.biologicalPower / (float)data.count;
                //powerProgressText.text = string.Format("{0}/{1}", MapMgr.biologicalPower, data.count);
            }
        }
    }

    IEnumerator PlayAnimation()
    {
        if (MapMgr.Event_UnLockHighArea != null)
        {
            MapMgr.Event_UnLockHighArea(this.data.id);
        }

        for (int i = 0; i < data.area.Length; i++)
        {
            if (i % 2 == 0)
            {
                int x = data.area[i];
                int y = data.area[i + 1];
                MapMgr.UnlockMapGrid(x, y);
                //SoundMgr.PlayUnsealTree();
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (Event_MapUnlcok != null)
        {
            Event_MapUnlcok(this);
        }
    }
}

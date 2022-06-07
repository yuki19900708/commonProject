using System;
using System.Collections;
using System.Collections.Generic;
using GameProto;
//using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighEndAreaUnlockInterface : MonoBehaviour
{
    //public TextMeshProUGUI titleText;
    //public TextMeshProUGUI desText;

    //public TextMeshProUGUI sliderText;
    public Image sliderValue;

    //public Button buyButton;
    //public TextMeshProUGUI buyButtonText;
    public Button closeButton;
    public DragonPowerSlider item;

    public Transform light1;
    public Image icon;

    private void Awake()
    {
        //AnimMgr.PlayRayRotationAnimation(light1, null, null, null, null, UIDef.High_EndAreaUI);

        //NetAPI.Event_PurchaseStatus += EventPurchaseCallBack;

        //titleText.text = L10NMgr.GetText(30800002);
        //buyButton.onClick.AddListener(OnBuyButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    public void Init(DragonPowerSlider item)
    {
        this.item = item;

        //sliderText.text = string.Format("{0} / {1}", MapMgr.CostDiamondTotal, item.data.consume);
        sliderValue.fillAmount = (float)MapMgr.CostDiamondTotal / (float)item.data.consume;
        icon.sprite = UGUISpriteAtlasMgr.LoadSprite("buy_terrain_" + item.data.id);

        icon.SetNativeSize();

        if (item.data.id == 501)
        {
            icon.transform.localPosition = new Vector3(0, 10, 0);
        }
        else
        { 
            icon.transform.localPosition = new Vector3(0, -3, 0);
        }
        
        //desText.text = string.Format(L10NMgr.GetText(30800003), item.data.consume, item.data.area.Length / 2);
    }

    private void OnCloseButtonClick()
    {
        item = null;
        UIMgr.HideUI(UIDef.High_EndAreaUI);
    }

    private void OnBuyButtonClick()
    {
#if UNITY_IOS && !UNITY_EDITOR
      
        //PurchaseData data = TableDataMgr.GetSinglePurchaseData(item.data.purchaseID);
        //U3DToPlatform.NormalPurchase(data.id.ToString(), data.productId, PlayerModel.Data.Level.ToString(), PlayerModel.Data.UserName);
        //Debug.Log("LY","用户决定购买这块地了！");
#else
        item.RunUnlock();
        OnCloseButtonClick();
#endif
    }

    private void EventPurchaseCallBack(int id, Game_common_status status)
    {
        if (status == Game_common_status.Success)
        {
            item.RunUnlock();
            OnCloseButtonClick();
        }
    }
}

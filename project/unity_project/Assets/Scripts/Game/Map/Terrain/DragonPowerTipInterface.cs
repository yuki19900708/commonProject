using UnityEngine;
using UnityEngine.UI;
//using TMPro;

public class DragonPowerTipInterface : MonoBehaviour
{
    //public TextMeshProUGUI titleText/*;*/
    //public TextMeshProUGUI desText1;
    //public TextMeshProUGUI desText2;
    //public TextMeshProUGUI desText3;
    //public TextMeshProUGUI sliderText;
    public Button closeButton;

    public Image sliderValueImage;

    private void Awake()
    {
        //titleText.text = L10NMgr.GetText(24100001);
        //desText1.text = L10NMgr.GetText(24100002);
        //desText2.text = L10NMgr.GetText(24100003);
        closeButton.onClick.AddListener(OnCloseButton);
    }

    private void OnCloseButton()
    {
        UIMgr.HideUI(UIDef.DragonPowerTipUI);
    }

    public void ShowTip(int total)
    {
        //string sr = string.Format(ComUtil.Text_Cusomize_Color, "F3C027", total);
        //sliderText.text = string.Format("{0}/{1}", MapMgr.biologicalPower, sr);

        sliderValueImage.fillAmount = (float)MapMgr.biologicalPower / (float)total;
    }
}

using UnityEngine.UI;
using System;

public class APMessageBoxError : APMessageBoxBase
{
    public Image backImage;
    public Text titleText;
    public Image contentImage;
    public Text contentText;
    public Button confirmButton;
    public Text confirmText;
    public Image maskImage;

    private event Action Event_ConfirmButtonClick;

    public void ShowUI(string title, string content, string buttonName, Action buttonAction)
    {
        titleText.text = title;
        contentText.text = content;
        confirmText.text = buttonName;
        Event_ConfirmButtonClick = buttonAction;
    }

    private void OnConfirmButtonClick()
    {
        if (Event_ConfirmButtonClick != null)
        {
            Event_ConfirmButtonClick();
        }
        CloseMessageBox();
    }

    #region Override Method

    protected override void InitializeMessageBox()
    {
        maskImage.raycastTarget = isModal;
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    protected override void CloseMessageBox()
    {
        base.CloseMessageBox();
        if (Event_ConfirmButtonClick != null)
        {
            Event_ConfirmButtonClick = null;
        }
    }

    #endregion

}
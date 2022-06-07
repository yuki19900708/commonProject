using System;
using UnityEngine.UI;

public class APMessageBoxConfirm : APMessageBoxBase
{
    public event Action Event_OnConfirmButtonClick;
    public event Action Event_OnCancleButtonClick;
    public event Action Event_OnCloseButtonClick;

    public Image background;
    public Image maskImage;
    public Text titleText;
    public Image contentBg;
    public Text contentText;
    public Button confirmButton;
    public Text confirmButtonText;
    public Button cancleButton;
    public Text cancleButtonText;
    public Button closeButton;

    private void Awake()
    {
        InitializeMessageBox();
    }

    public void ShowUI(string title, string content, string confirm, string cancle, Action confirmAction, Action cancleAction, Action closeAction)
    {
        titleText.text = title;
        contentText.text = content;
        confirmButtonText.text = confirm;
        cancleButtonText.text = cancle;
        Event_OnConfirmButtonClick = confirmAction;
        Event_OnCancleButtonClick = cancleAction;
        Event_OnCloseButtonClick = closeAction;
    }

    private void OnConfirmButtonClick()
    {
        if (Event_OnConfirmButtonClick != null)
        {
            Event_OnConfirmButtonClick();
        }
        CloseMessageBox();
    }

    private void OnCancleButtonClick()
    {
        if (Event_OnCancleButtonClick != null)
        {
            Event_OnCancleButtonClick();
        }
        CloseMessageBox();
    }

    private void OnCloseButtonClick()
    {
        if (Event_OnCloseButtonClick != null)
        {
            Event_OnCloseButtonClick();
        }
        CloseMessageBox();
    }

    #region Override Method

    protected override void InitializeMessageBox()
    {
        maskImage.raycastTarget = isModal;
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        cancleButton.onClick.AddListener(OnCancleButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    protected override void CloseMessageBox()
    {
        base.CloseMessageBox();
        if (Event_OnConfirmButtonClick != null)
        {
            Event_OnConfirmButtonClick = null;
        }
        if (Event_OnCancleButtonClick != null)
        {
            Event_OnCancleButtonClick = null;
        }
        if (Event_OnCloseButtonClick != null)
        {
            Event_OnCloseButtonClick = null;
        }
    }

    #endregion

}
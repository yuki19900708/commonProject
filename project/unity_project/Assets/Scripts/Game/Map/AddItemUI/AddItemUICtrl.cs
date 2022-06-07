using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AddItemUICtrl : MonoBehaviour
{
    public static AddItemUICtrl Instance;
    public APWrapContent content;
    public Button button;
    public Text addText;

    #region 临时添加物体UI
    public InputField findField;
    public Button findButton;
    #endregion

    private List<MapObjectData> selectDataList = new List<MapObjectData>();

    private void Awake()
    {
        Instance = this;
        button.onClick.AddListener(OnCloseButton);        
        findField.onEndEdit.AddListener(OnSearchButtonClick);
        content.Event_OnCreateWrapContentItem += Event_OnCreateWrapContentItem;
        content.Event_OnRefreshWrapContentItem += Event_OnRefreshWrapContentItem;

        content.InitializeWrapContent();
        //if (GameMgr.IsGameAssetsReady)
        //{
        //    InitData();
        //}
        //else
        //{
        //    ResUpdateMgr.Event_DownloadFinish += InitData;
        //}
    }

    // Use this for initialization
    public void InitData()
    {
        selectDataList = EditorTerrainModel.TerrainEditorModel.AllOjbectElements;
        content.RefreshWrapContent(selectDataList.Count);
    }

    private void Event_OnCreateWrapContentItem(GameObject obj)
    {
        obj.GetComponent<AddItem>().Event_AddItem += EventAddDone;
    }

    private void Event_OnRefreshWrapContentItem(GameObject arg1, int arg2)
    {
        arg1.GetComponent<AddItem>().Data = selectDataList[arg2];
    }

    private void OnCloseButton()
    {
        findButton.gameObject.SetActive(!findButton.gameObject.activeSelf);
        findField.gameObject.SetActive(!findField.gameObject.activeSelf);
        content.gameObject.SetActive(!content.gameObject.activeSelf);
        findField.text = "";
        selectDataList = EditorTerrainModel.TerrainEditorModel.AllOjbectElements;
        content.RefreshWrapContent(selectDataList.Count);
    }

    private void EventAddDone(string sr)
    {
        addText.text = sr + "Add Done!!!";
        addText.DOKill();
        addText.color = Color.white;
        addText.DOColor(new Color(1, 1, 1, 0), 1);
    }

    public void OnSearchButtonClick(string text)
    {
        string value = findField.text;
        List<MapObjectData> list = EditorTerrainModel.TerrainEditorModel.AllOjbectElements;
        selectDataList = new List<MapObjectData>();
        if (string.IsNullOrEmpty(value))
        {
            selectDataList = list;
            content.RefreshWrapContent(selectDataList.Count);
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].id.ToString().Contains(value))
            {
                selectDataList.Add(list[i]);
                continue;
            }
            if (list[i].currentName.Contains(value))
            {
                selectDataList.Add(list[i]);
                continue;
            }
            if (list[i].standbyName.Contains(value))
            {
                selectDataList.Add(list[i]);
                continue;
            }
        }
        content.RefreshWrapContent(selectDataList.Count);
        findField.Select();
        //findField.ActivateInputField();
    }
}

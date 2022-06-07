using EditorTerrainModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainEditorContentEditorInterface : MonoBehaviour
{
    public Action<DropDownSelectType, string> Event_BrushStyleChange;
    public Action<DropDownSelectType, TerrainEditorSelectItem> Event_SelectObjectItem;
    public Action<TerrainEditorVegetation> Event_SelectVegetationItem;
    public Action Event_SaveEditor;

    public Dropdown layoutSelectDropDown;
    public Dropdown objectSelectDropDown;
    public Dropdown brushStyleDropDown;

    public GameObject selectGameObject;
    public APWrapContent objectContent;
    public Button searchButton;
    public InputField searchInputField;

    public GameObject selectVegetationGameObject;
    public APWrapContent vegetationContent;

    public Button saveButton;
    public Text tipText;

    private List<MapObjectData> selectDataList = new List<MapObjectData>();
    private List<VegetationData> selectVegetationList = new List<VegetationData>();
    public void InitData(string text)
    {
        tipText.text = text;
        searchButton.onClick.AddListener(OnSearchButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        layoutSelectDropDown.onValueChanged.AddListener(LayoutSelectDropDownValueChange);
        layoutSelectDropDown.options = TerrainEditorModel.GetTerrainEditorDropDownOptionData(TerrainEditorModel.editoryLayouts);
        layoutSelectDropDown.value = 3;

        brushStyleDropDown.onValueChanged.AddListener(BrushStyleDropDownValueChange);
        brushStyleDropDown.options = TerrainEditorModel.GetTerrainEditorDropDownOptionData(TerrainEditorModel.brushStyleElements);
        brushStyleDropDown.value = 0;

//	        objectSelectDropDown.onValueChanged.AddListener(ObjectSelectDropDownValueChange);
        //objectSelectDropDown.options = TerrainEditorModel.GetTerrainEditorDropDownOptionData(TerrainEditorModel.PurificationElements);
        objectSelectDropDown.value = 0;

        selectDataList = EditorTerrainModel.TerrainEditorModel.AllOjbectElements;
        selectVegetationList = EditorTerrainModel.TerrainEditorModel.AllVegetationElements;

        selectVegetationGameObject.SetActive(true);
        vegetationContent.Event_OnCreateWrapContentItem += EventVegetationOnCreateWrapContent;
        vegetationContent.Event_OnRefreshWrapContentItem += EventVegetationOnRefreshWrapContent;
        vegetationContent.RefreshWrapContent(EditorTerrainModel.TerrainEditorModel.AllVegetationElements.Count);
        selectVegetationGameObject.SetActive(false);

        selectGameObject.SetActive(true);
        objectContent.Event_OnRefreshWrapContentItem += EventOnRefreshWrapContentItem;
        objectContent.RefreshWrapContent(EditorTerrainModel.TerrainEditorModel.AllOjbectElements.Count);
        selectGameObject.SetActive(false);
    }

    private void EventVegetationOnRefreshWrapContent(GameObject obj, int index)
    {
        TerrainEditorVegetation item = obj.GetComponent<TerrainEditorVegetation>();
        item.Data = selectVegetationList[index];
        if (item.Data == TerrainEditorUICtrl.Instance.currentSelectVegetationItemData)
        {
            item.IsSelect = true;
        }
        else
        {
            item.IsSelect = false;
        }
    }

    private void EventVegetationOnCreateWrapContent(GameObject obj)
    {
        obj.GetComponent<TerrainEditorVegetation>().Event_SelectObject += EventVegetationSelect;
    }

    private void EventVegetationSelect(TerrainEditorVegetation item)
    {
        Debug.Log("选中了" + item.nameText.text);
        if (Event_SelectObjectItem != null)
        {
            Event_SelectVegetationItem(item);
        }
    }

    private void EventOnRefreshWrapContentItem(GameObject obj, int index)
    {
        TerrainEditorSelectItem item = obj.GetComponent<TerrainEditorSelectItem>();
        item.Data = selectDataList[index];
        if (item.Data == TerrainEditorUICtrl.Instance.currentSelectObjectItemData)
        {
            item.IsSelect = true;
        }
        else
        {
            item.IsSelect = false;
        }
    }


    private void OnSaveButtonClick()
    {
        if (Event_SaveEditor != null)
        {
            Event_SaveEditor();
        }
    }
		

    private void BrushStyleDropDownValueChange(int index)
    {
        if (Event_BrushStyleChange != null)
        {
            Event_BrushStyleChange(DropDownSelectType.BrushStyle, brushStyleDropDown.captionText.text);
        }
    }

    private void LayoutSelectDropDownValueChange(int index)
    {
        selectVegetationGameObject.SetActive(false);
        selectGameObject.gameObject.SetActive(false);
        objectSelectDropDown.gameObject.SetActive(false);
        objectSelectDropDown.value = 0;
        switch (index)
        {
            case 0:
                //objectSelectDropDown.options = TerrainEditorModel.GetTerrainEditorDropDownOptionData(TerrainEditorModel.terrainElements);
                break;
            case 1:
                //objectSelectDropDown.options = TerrainEditorModel.GetTerrainEditorDropDownOptionData(TerrainEditorModel.vegetationElements);
                if (TerrainEditorUICtrl.IsEditor)
                {
                    EnableVegetationSelect();
                }
                break;
            case 2:
                if (TerrainEditorUICtrl.IsEditor)
                {
                    EnableObjectSelect();
                }
                break;
            case 3:
                objectSelectDropDown.options = TerrainEditorModel.GetTerrainEditorDropDownOptionData(TerrainEditorModel.PurificationElements);
                objectSelectDropDown.gameObject.SetActive(true);
                break;
            case 4:
                objectSelectDropDown.options = TerrainEditorModel.GetTerrainEditorDropDownOptionData(TerrainEditorModel.
                    
                    SealLockElements);
                objectSelectDropDown.gameObject.SetActive(true);
                break;
        }

        if (Event_BrushStyleChange != null)
        {
            Event_BrushStyleChange(DropDownSelectType.Layout, layoutSelectDropDown.captionText.text);
        }
    }

    public void EnableObjectSelect()
    {
        selectGameObject.gameObject.SetActive(true);
        searchInputField.text = "";
    }

    public void DisableObjectSelect()
    {
        selectGameObject.gameObject.SetActive(false);
        searchInputField.text = "";
        objectContent.RefreshWrapContent(selectDataList.Count);
    }

    public void EnableVegetationSelect()
    {
        selectVegetationGameObject.SetActive(true);
    }

    public void DisableVegetationSelect()
    {
        selectVegetationGameObject.SetActive(false);
    }

    public void OnSearchButtonClick()
    {
        string value = searchInputField.text;
        List<MapObjectData> list = EditorTerrainModel.TerrainEditorModel.AllOjbectElements;
        selectDataList = new List<MapObjectData>();
        if (string.IsNullOrEmpty(value))
        {
            selectDataList = list;
            objectContent.RefreshWrapContent(selectDataList.Count);
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
        objectContent.RefreshWrapContent(selectDataList.Count);
    }
}

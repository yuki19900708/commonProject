using EditorTerrainModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainEditorContentEditorInterface : MonoBehaviour
{
    public Action<DropDownSelectType, string> Event_BrushStyleChange;
    public Action<TerrainEditorVegetation> Event_SelectVegetationItem;
    public Action Event_SaveEditor;

    public Dropdown brushStyleDropDown;

    public GameObject selectVegetationGameObject;
    public APWrapContent vegetationContent;

    public Button saveButton;
    public Text tipText;

    private List<VegetationData> selectVegetationList = new List<VegetationData>();
    public void InitData(string text)
    {
        tipText.text = text;
        saveButton.onClick.AddListener(OnSaveButtonClick);
  

        brushStyleDropDown.onValueChanged.AddListener(BrushStyleDropDownValueChange);
        brushStyleDropDown.options = TerrainEditorModel.GetTerrainEditorDropDownOptionData(TerrainEditorModel.brushStyleElements);
        brushStyleDropDown.value = 0;

        List<VegetationData> tmpList = EditorTerrainModel.TerrainEditorModel.AllVegetationElements;
        selectVegetationList = new List<VegetationData>();

        if (TerrainEditorModel.MapDrawer())
        {
            selectVegetationList.Add(tmpList[0]);
        }
        else
        {
            selectVegetationList.AddRange(tmpList);
        }
        selectVegetationGameObject.SetActive(true);
        vegetationContent.Event_OnCreateWrapContentItem += EventVegetationOnCreateWrapContent;
        vegetationContent.Event_OnRefreshWrapContentItem += EventVegetationOnRefreshWrapContent;
        vegetationContent.RefreshWrapContent(selectVegetationList.Count);
        selectVegetationGameObject.SetActive(false);

    }

	private void BrushStyleDropDownValueChange(int index)
	{
		if (Event_BrushStyleChange != null)
		{
			Event_BrushStyleChange(DropDownSelectType.BrushStyle, brushStyleDropDown.captionText.text);
		}
	}
    private void EventVegetationOnRefreshWrapContent(GameObject obj, int index)
    {
        TerrainEditorVegetation item = obj.GetComponent<TerrainEditorVegetation>();
        item.Data = selectVegetationList[index];
		if (item == TerrainEditorUICtrl.Instance.CurrentSelectVegetationItem)
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
		if (Event_SelectVegetationItem != null)
        {
            Event_SelectVegetationItem(item);
        }
    }
		
    private void OnSaveButtonClick()
    {
        if (Event_SaveEditor != null)
        {
            Event_SaveEditor();
        }
    }
		

    public void EnableVegetationSelect()
    {
        selectVegetationGameObject.SetActive(true);
    }

    public void DisableVegetationSelect()
    {
        selectVegetationGameObject.SetActive(false);
    }
		
}

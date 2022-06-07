using System;
using UnityEngine;
using UnityEngine.UI;

public class TerrainEditorSelectItem : APWrapContentItem
{
    public Image iconImage;
    public Text nameText;
    public Text initNameText;
    public Text idText;
    public Button button;
    private MapObjectData data;
    private bool isSelect = false;
    public bool IsSelect
    {
        get
        {
            return isSelect;
        }
        set
        {
            isSelect = value;
            
            if (isSelect)
            {
                button.image.color = Color.red;
            }
            else
            {
                button.image.color = Color.white;
            }
        }
    }

    public Action<TerrainEditorSelectItem> Event_SelectObject;

    public MapObjectData Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
            //if (TerrainEditorUICtrl.Instance.atlas.spriteDict.ContainsKey(data.id.ToString()))
            //{
            iconImage.sprite = TerrainEditorUICtrl.Instance.GetSpriteByName(data.id.ToString());
            //}
            //else
            //{
            //    iconImage.sprite = TerrainEditorUICtrl.Instance.tempObjectSprite;
            //}

            if (string.IsNullOrEmpty(data.currentName) || data.currentName == "0")
            {
                nameText.text = "暂无名称!!!";
            }
            else
            {
                nameText.text = data.currentName;
            }

            if (string.IsNullOrEmpty(data.standbyName) || data.standbyName == "0")
            {
                nameText.text = "暂无名称!!!";
            }
            else
            {
                initNameText.text = data.standbyName;
            }

            idText.text = data.id.ToString();
        }
    }

    private void Awake()
    {
        button.onClick.AddListener(OnClickItem);
    }

    private void OnClickItem()
    {
        if (data.detachGrid)
        {
            if (Event_SelectObject != null)
            {
                Event_SelectObject(this);
            }
        }
        else
        {
            if (Event_SelectObject != null)
            {
                Event_SelectObject(this);
            }
        }
    }
}

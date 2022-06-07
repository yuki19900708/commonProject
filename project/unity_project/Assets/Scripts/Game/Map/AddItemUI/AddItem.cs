using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddItem : APWrapContentItem
{
    public Image itemImage;
    public Text itemText;
    public Text idText;
    public Text initName;

    public Button button;
    public Action<string> Event_AddItem;

	private MapObjectData data;

    public MapObjectData Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
            //if (UGUISpriteAtlasMgr.LoadSprite(data.id.ToString()))
            //{
            //    itemImage.sprite = UGUISpriteAtlasMgr.LoadSprite(data.id.ToString());
            //}
            //else
            //{
            //    itemImage.sprite = MapMgr.Instance.tempObjectSprite;
            //}
            itemImage.sprite = UGUISpriteAtlasMgr.LoadSprite(data.id.ToString());
            itemImage.preserveAspect = true;

            //itemText.text = L10NMgr.GetText(data.name);
            idText.text = data.id.ToString();
            initName.text = data.standbyName;
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
            //DeviceCommand.AddBiological(data.id);
            if (Event_AddItem != null)
            {
                Event_AddItem(itemText.text);
            }
        }
        else
        {
            //DeviceCommand.AddItem(data.id);
            if (Event_AddItem != null)
            {
                Event_AddItem(itemText.text);
            }
        }
    }
}
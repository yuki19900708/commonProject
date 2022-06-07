using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainEditorVegetation : APWrapContentItem
{
    public Image iconImage;
    public Text nameText;
    public Text idText;
    public Text infoText;
    public Button button;
    public Sprite[] spriteArray;
    Material mat;
    private VegetationData data;
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

    public Action<TerrainEditorVegetation> Event_SelectObject;

    public VegetationData Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;

            iconImage.sprite = spriteArray[index];
            Material newMat = new Material(mat);
            newMat.SetFloat("_Hue", data.hueValue);
            iconImage.material = newMat;
            nameText.text = data.des.ToString();
            idText.text = data.id.ToString();
            infoText.text = "Hue:" + data.hueValue;
        }
    }

    private void Awake()
    {
        mat = ResMgr.Load<Material>("SpriteHSVMaterial");
        button.onClick.AddListener(OnClickItem);
    }

    private void OnClickItem()
    {
        if (Event_SelectObject != null)
        {
            Event_SelectObject(this);
        }
    }
}

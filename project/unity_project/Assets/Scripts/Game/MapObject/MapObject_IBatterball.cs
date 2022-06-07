using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;

public partial class MapObject
{
    public bool IsBatterBall
    {
        get
        {
            return basicData.objectType == MapObject.OBJECT_TYPE_BATTER_BALL;
        }
    }
    public int batterBallOnlyID;
    private SpriteRenderer entitySpriteRenderer;

    public void Init(int onlyID)
    {
        batterBallOnlyID = onlyID;
        //this.transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
        if(entitySpriteRenderer == null)
        {
            entitySpriteRenderer = transform.Find(Id.ToString()).GetComponent<SpriteRenderer>();
        }
        entitySpriteRenderer.color = ComUtil.Color_A;
        //this.transform.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        this.transform.localScale = Vector3.one;

        OnBatterballClick();
    }

    public void OnBatterballClick()
    {
       //AnimMgr.ClickBatterBall(this.transform, ClickOutPut);
    }

    public void RecycleBatterball()
    {
        if (IsBatterBall)
        {

        }
    }
}

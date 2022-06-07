using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragOutline : MonoBehaviour {
    public static Color CAN_MOVE_CAN_MERGE = new Color(122 / 255f, 255 / 255f, 50 / 255f, 1);
    public static Color CAN_MOVE_NO_MERGE = new Color(41 / 255f, 39 / 255f, 66 / 255f, 1);
    public static Color NO_MOVE_NO_MERGE = new Color(13 / 255f, 13 / 255f, 14 / 255f, 1);
    public static Color FLY = new Color(122 / 255f, 255 / 255f, 50 / 255f, 1);
    public static Color NO_PLACE = new Color(249 / 255f, 50 / 255f, 50 / 255f, 1);
    public static Color TIP = new Color(18 / 255f, 50 / 255f, 166 / 255f, 1);
    public SpriteRenderer spriteRenderer;
    public void Init(MapGridState state)
    {
        switch (state)
        {
            case MapGridState.UnlockAndCured:
                spriteRenderer.sprite = UGUISpriteAtlasMgr.LoadSprite("selected_outline_square");
                spriteRenderer.color = Color.green;
                break;
            case MapGridState.UnlockButDead:
                spriteRenderer.sprite = UGUISpriteAtlasMgr.LoadSprite("selected_outline_square");
                spriteRenderer.color = Color.red;
                break;
        }
        transform.localScale = Vector3.one;
    }

    public void SetSpriteColor(Color color,int orderLayer=0)
    {
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = orderLayer;
    }

    public void InitNoPostion(int state)
    {
        switch (state)
        {
            case 1:
                spriteRenderer.sprite = UGUISpriteAtlasMgr.LoadSprite("selected_outline_circle");
                spriteRenderer.color = Color.green;
                break;
            case 2:
                spriteRenderer.sprite = UGUISpriteAtlasMgr.LoadSprite("selected_outline_circle");
                spriteRenderer.color = Color.red;
                break;
        }
        transform.localScale = Vector3.one;
    }
}

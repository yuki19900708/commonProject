using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGUISpriteLoader : MonoBehaviour
{
    // Use this for initialization
    public bool loadFromLocal;

    void Awake()
    {
        //if (GameMgr.IsGameAssetsReady)
        //{
        //    LoadSprite();
        //}
        //else
        //{
        //    if (loadFromLocal)
        //    {
        //        if (ResMgr.IS_LOCAL_RES_INITIALIZED)
        //        {
        //            LoadSprite();
        //        }
        //        else
        //        {
        //            ResMgr.Event_LocalResInitFinish += ResMgrLocalResInitFinish;
        //        }
        //    }
        //    else
        //    {
        //        //GameMgr.Event_GameResReady += GameMgr_Event_GameResReady;
        //    }
        //}
    }

    private void OnDestroy()
    {
        //GameMgr.Event_GameResReady -= GameMgr_Event_GameResReady;
        ResMgr.Event_LocalResInitFinish -= ResMgrLocalResInitFinish;
    }

    private void ResMgrLocalResInitFinish()
    {
        LoadSprite();
    }

    private void GameMgr_Event_GameResReady()
    {
        LoadSprite();
    }

    private void LoadSprite()
    {
        UGUISpriteTag[] spriteTags = this.GetComponentsInChildren<UGUISpriteTag>(true);
        foreach (UGUISpriteTag tag in spriteTags)
        {
            if (string.IsNullOrEmpty(tag.spriteName))
            {
                continue;
            }

            //如果已经有图了，可能是业务逻辑已经将图更新过了，此时Loader就不应该再将图换回去
            Image image = tag.GetComponent<Image>();
            if (image != null && image.sprite != null)
            {
                continue;
            }
            SpriteRenderer sr = tag.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                continue;
            }

            Sprite sprite = UGUISpriteAtlasMgr.LoadSprite(tag.spriteName, tag.atlasName);

            if (sprite != null)
            {
                if (image != null)
                {
                    image.sprite = sprite;
                    continue;
                }
                
                if (sr != null)
                {
                    sr.sprite = sprite;
                }
            }
        }
    }
}

using System;
using UnityEngine;

public partial class MapObject
{
    public Action Event_DailyChestBackPool;
    public MaterialPropertyBlock mpb;
    public GameObject godTip;
    private bool shouldShowGodTip;

    public bool ShouldShowGodTip
    {
        get
        {
            return shouldShowGodTip;
        }
        set
        {
            shouldShowGodTip = value;
        }
    }
    /// <summary>
    /// 占地面积
    /// </summary>
    public Point Area
    {
        get
        {
            return new Point(basicData.area[0], basicData.area[1]);
        }
    }

    private void SetGodTipPos()
    {
        if (shouldShowGodTip)
        {
            //godTip.transform.position = transform.position + new Vector3(0, 2, 0);
        }
    }

    public void DisplayAsLock(DeadLandData data = null)
    {
        Renderer[] rendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in rendererList)
        {
            if (StaticMapGridList[0].Status == MapGridState.UnlockAndCured || StaticMapGridList[0].deadLandData == null)
            {
                if ((this.IsTerrain || this.IsVegetation) && IsBridge == false)
                {
                    renderer.sortingOrder = 0;
                }

                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", 0);
                mpb.SetFloat("_Saturation", 1);
                mpb.SetFloat("_Value", 1);
                renderer.SetPropertyBlock(mpb);
            }
            else
            {
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", 0);
                mpb.SetFloat("_Saturation", StaticMapGridList[0].deadLandData.saturation);
                mpb.SetFloat("_Value", StaticMapGridList[0].deadLandData.brightness);
                renderer.SetPropertyBlock(mpb);
                if ((this.IsTerrain || this.IsVegetation) && IsBridge == false)
                {
                    renderer.sortingOrder = (int)(StaticMapGridList[0].deadLandData.brightness * 100);
                }
                if (this.IsSealLock)
                {
                    (renderer as SpriteRenderer).color = new Color(171.0f / 255.0f, 231.0f / 255.0f, 255.0f / 255.0f, 1);
                }
            }
        }
        if (IsEntity)
        {
            gameObject.SetActive(false);
            if (gameObject.GetComponent<Collider2D>() != null)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    public void RefreshVegetationMat(int hue, float saturation, float brightness)
    {
        Renderer[] entityRendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in entityRendererList)
        {
            if (this.IsVegetation)
            {
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", hue);
                mpb.SetFloat("_Saturation", saturation);
                mpb.SetFloat("_Value", brightness);
                renderer.SetPropertyBlock(mpb);
            }
            if ((this.IsTerrain || this.IsVegetation) && IsBridge == false)
            {
                renderer.sortingOrder = 0;
            }
        }
    }

    public void RefreshSealLockMat(DeadLandData data, float value)
    {
        Renderer[] entityRendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in entityRendererList)
        {
            if (this.IsSealLock)
            {
                if (data != null)
                {
                    renderer.GetPropertyBlock(mpb);
                    mpb.SetFloat("_Hue", 0);
                    mpb.SetFloat("_Saturation", data.saturation * value);
                    mpb.SetFloat("_Value", data.brightness * value);
                    renderer.SetPropertyBlock(mpb);
                }
                else
                {
                    renderer.GetPropertyBlock(mpb);
                    mpb.SetFloat("_Hue", 0);
                    mpb.SetFloat("_Saturation", 1);
                    mpb.SetFloat("_Value", 1);
                    renderer.SetPropertyBlock(mpb);
                }
            }
            if ((this.IsTerrain || this.IsVegetation) && IsBridge == false)
            {
                renderer.sortingOrder = 0;
            }
        }
    }

    public void RefreshVegetationMat()
    {
        Renderer[] entityRendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in entityRendererList)
        {
            if (this.IsVegetation)
            {
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", this.VegetationData.hueValue);
                mpb.SetFloat("_Saturation", this.VegetationData.saturation);
                mpb.SetFloat("_Value", this.VegetationData.brightness);
                renderer.SetPropertyBlock(mpb);
            }
            if ((this.IsTerrain || this.IsVegetation) && IsBridge == false)
            {
                renderer.sortingOrder = 0;
            }
        }
    }

    /// <summary>
    /// 物体的格子被净化了
    /// </summary>
    public void DisplayAsUnLockAndCured()
    {
        if (IsEntity)
        {
            gameObject.SetActive(true);
            if (gameObject.GetComponent<Collider2D>() != null)
            {
                gameObject.GetComponent<Collider2D>().enabled = true;
            }
            if (IsPurified == false)
            {
                return;
            }
        }
        
        Renderer[] entityRendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in entityRendererList)
        {
            if (this.IsVegetation)
            {
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", this.VegetationData.hueValue);
                mpb.SetFloat("_Saturation", this.VegetationData.saturation);
                mpb.SetFloat("_Value", this.VegetationData.brightness);
                renderer.SetPropertyBlock(mpb);
            }
            else
            {
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", 0);
                mpb.SetFloat("_Saturation", 1);
                mpb.SetFloat("_Value", 1);
                renderer.SetPropertyBlock(mpb);
            }

            if ((this.IsTerrain || this.IsVegetation) && IsBridge == false)
            {
                renderer.sortingOrder = 0;
            }
        }
    }

    public void DisplayAsUnlockButDead(DeadLandData deadLandData, bool keepVegetationColor = false)
    {
        ///被拖拽的物体  不会被怪物的封印锁定
        if (IsEntity && IsDraged)
        {
            return;
        }
        Renderer[] entityRendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in entityRendererList)
        {
            if (keepVegetationColor && this.IsVegetation)
            {
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", this.VegetationData.hueValue);
                mpb.SetFloat("_Saturation", this.VegetationData.saturation * deadLandData.saturation);
                mpb.SetFloat("_Value", this.VegetationData.brightness * deadLandData.brightness);
                renderer.SetPropertyBlock(mpb);
            }
            else if (IsEntity)
            {
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", 0);
                mpb.SetFloat("_Saturation", 0.2f);
                mpb.SetFloat("_Value", 0.7f);
                renderer.SetPropertyBlock(mpb);
            }
            else
            {
                renderer.GetPropertyBlock(mpb);
                mpb.SetFloat("_Hue", 0);
                mpb.SetFloat("_Saturation", deadLandData.saturation);
                mpb.SetFloat("_Value", deadLandData.brightness);
                renderer.SetPropertyBlock(mpb);
            }

            if ((this.IsTerrain || this.IsVegetation) && IsBridge == false)
            {
                renderer.sortingOrder = (int)(deadLandData.brightness * 100);
            }
        }

        if (IsEntity)
        {
            gameObject.SetActive(true);
            if (gameObject.GetComponent<Collider2D>() != null)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    public void ClearObjectAttribute()
    {
        if (gameObject == null)
        {
            //Debug.Log("LY", "错误排查代码出问题的物品id" + basicData.id);
        }

        if (godTip != null)
        {
            godTip.gameObject.SetActive(false);
            //MapMgr.clickToCollectTipPool.RecycleInstance(godTip);
            //godTip = null;
        }

        //if (IsEntity && basicData.id == OBJECT_ID_DAILY_CHEST)
        //{
        //    if (Event_DailyChestBackPool != null)
        //    {
        //        Event_DailyChestBackPool();
        //    }
        //}

        Renderer[] entityRendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in entityRendererList)
        {
            renderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Hue", 0);
            mpb.SetFloat("_Saturation", 1);
            mpb.SetFloat("_Value", 1);
            renderer.SetPropertyBlock(mpb);
        }
        //if (IsEntity && basicData != null && Id != OBJECT_ID_DAILY_CHEST)
        //{
        //    Event_StaticPosRefresh = null;
        //}
        mapObjectSourcesType = MapObejctSourcesType.None;
        ShouldShowGodTip = false;
        IsBridge = false;
        IsTerrain = false;
        IsEntity = false;
        Hue = 0;
        IsVegetation = false;
        IsSealLock = false;
    }
}

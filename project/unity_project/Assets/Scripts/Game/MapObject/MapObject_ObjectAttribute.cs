using System;
using UnityEngine;

public partial class MapObject
{
    public MaterialPropertyBlock mpb;
 
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

    /// <summary>
    /// 物体的格子被净化了
    /// </summary>
    public void DisplayAsUnLockAndCured()
    {
        Renderer[] entityRendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in entityRendererList)
        {
            if (this.VegetationData != null)
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
        }
    }

    public void ClearObjectAttribute()
    {
        Renderer[] entityRendererList = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in entityRendererList)
        {
            renderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_Hue", 0);
            mpb.SetFloat("_Saturation", 1);
            mpb.SetFloat("_Value", 1);
            renderer.SetPropertyBlock(mpb);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;
//using TMPro;

public static class UIUtil
{
    /// <summary>
    /// 将Sprite缩放至符合width, height规定的大小范围内, 但是如果图片已经比范围小了，不进行放大
    /// </summary>
    /// <param name="sprite">sprite对象</param>
    /// <param name="width">宽度像素值</param>
    /// <param name="height">高度像素值</param>
    public static void ScaleSprite(SpriteRenderer sr, int width, int height, int uiHeight, float standardCameraOrthographicSize)
    {
        float standardUIPixelsPerUnit = uiHeight / standardCameraOrthographicSize / 2;
        float widthUnit = width / standardUIPixelsPerUnit;
        float heightUnit = height / standardUIPixelsPerUnit;

        float spriteWidthUnit = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
        float spriteHeightUnit = sr.sprite.rect.height / sr.sprite.pixelsPerUnit;

        float spriteScaleX = widthUnit / spriteWidthUnit;
        float spriteScaleY = heightUnit / spriteHeightUnit;

        float spriteScale = Mathf.Clamp01(Mathf.Min(spriteScaleX, spriteScaleY));

        sr.transform.localScale = Vector3.one * spriteScale;
    }

    public static Vector2 WorldToCanvasPosition(RectTransform canvasTransform, Camera camera, Vector3 position)
    {
        //Vector position (percentage from 0 to 1) considering camera size.
        //For example (0,0) is lower left, middle is (0.5,0.5)
        Vector2 temp = camera.WorldToViewportPoint(position);

        //Calculate position considering our percentage, using our canvas size
        //So if canvas size is (1100,500), and percentage is (0.5,0.5), current value will be (550,250)
        temp.x *= canvasTransform.sizeDelta.x;
        temp.y *= canvasTransform.sizeDelta.y;

        //The result is ready, but, this result is correct if canvas recttransform pivot is 0,0 - left lower corner.
        //But in reality its middle (0.5,0.5) by default, so we remove the amount considering cavnas rectransform pivot.
        //We could multiply with constant 0.5, but we will actually read the value, so if custom rect transform is passed(with custom pivot) , 
        //returned value will still be correct.
        temp.x -= canvasTransform.sizeDelta.x * canvasTransform.pivot.x;
        temp.y -= canvasTransform.sizeDelta.y * canvasTransform.pivot.y;

        return temp;
    }

    public static Vector3 CanvasToWorldPosition(RectTransform canvasTransform, Camera camera, Vector2 anchoredPosition)
    {
        Vector2 screenPoint = anchoredPosition;
        screenPoint.x += canvasTransform.sizeDelta.x * canvasTransform.pivot.x;
        screenPoint.y += canvasTransform.sizeDelta.y * canvasTransform.pivot.y;
        screenPoint.x /= canvasTransform.sizeDelta.x;
        screenPoint.y /= canvasTransform.sizeDelta.y;
        screenPoint.x *= Screen.width;
        screenPoint.y *= Screen.height;

        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasTransform, screenPoint, camera, out worldPosition);
        return worldPosition;
    }

    /// <summary>
    /// 将图片资源的轴点应用到使用该图片的Image的RectTransform组件上
    /// </summary>
    /// <param name="image"></param>
    public static void ApplyTexturePivotToImage(Image image)
    {
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta;
        if (image.sprite != null)
        {
            Vector2 pixelPivot = image.sprite.pivot;
            Vector2 percentPivot = new Vector2(pixelPivot.x / size.x, pixelPivot.y / size.y);
            rectTransform.pivot = percentPivot;
        }
    }

    /// <summary>
    /// 将原图修改为指定大小，如果原图比制定大小还小，那么SetNativeSize()保持原图大小
    /// </summary>
    /// <param name="image"></param>
    /// <param name="sprite"></param>
    /// <param name="size"></param>
    public static void SetSpritePreserveAspectAndSize(this Image image, Sprite sprite, Vector2 size)
    {
        image.preserveAspect = true;
        image.sprite = sprite;
        if (sprite.rect.width < size.x && sprite.rect.height < size.y)
        {
            image.SetNativeSize();
        }
        else
        {
            image.rectTransform.sizeDelta = size;
        }
    }

    /// <summary>
    /// K是kilo千的意思1000=1K
    /// M是million万的意思10000=10K=1M
    /// </summary>
    /// <param name="str"></param>
    /// <param name="mount"></param>
    /// <returns></returns>
    public static void ToText(this Text text, int mount)
    {
        //if (mount > 100000)
        //{
        //    text.text = (mount / 100000f).ToString("f1") + "G";
        //}
        //else if (mount > 10000)
        //{
        //    text.text=(mount / 10000f).ToString("f1")+"M";
        //}
        if (mount > 1000)
        {
            text.text = (mount / 1000f).ToString("f1")+"K";
        }
        else 
        {
            text.text = mount.ToString();
        }

    }


    //public static void ToText(this TextMeshProUGUI text, int mount)
    //{
    //    //if (mount > 100000)
    //    //{
    //    //    text.text = (mount / 100000f).ToString("f1") + "G";
    //    //}
    //    //else if (mount > 10000)
    //    //{
    //    //    text.text = (mount / 10000f).ToString("f1") + "M";
    //    //}
    //    if (mount > 1000)
    //    {
    //        text.text = (mount / 1000f).ToString("f1") + "K";
    //    }
    //    else
    //    {
    //        text.text = mount.ToString();
    //    }

    //}

}

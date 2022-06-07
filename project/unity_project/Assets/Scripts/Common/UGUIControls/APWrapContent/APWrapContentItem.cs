using System;
using UnityEngine;

public class APWrapContentItem : MonoBehaviour
{
    public Action<int, Vector2> Event_OnWrapContentItemSizeChanged;

    [SerializeField]
    protected int index = -1;

    protected Vector2 originSize;
    protected Vector2 size;
    protected RectTransform rectTransform;
    protected bool sizeCacluated = false;
    /// <summary>
    /// GalleryMode下的Item距离视野中心的百分比
    /// -1相当于Item的位置在视野的左边缘
    /// 1相当于Item的位置在视野的右边缘
    /// 0相当于Item在视野中心位置
    /// </summary>
    protected float relativePosToCenter = 0;
    
    public virtual int Index
    {
        get { return index; }
        set
        {
            index = value;
        }
    }

    public float RelativePosToCenter
    {
        get
        {
            return relativePosToCenter;
        }
        set
        {
            relativePosToCenter = value;
        }
    }

    public RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null)
            {
                rectTransform = this.GetComponent<RectTransform>();
            }
            return rectTransform;
        }
    }

    /// <summary>
    /// 当前Size（多行多列的情况下不要使用）
    /// </summary>
    public Vector2 Size
    {
        get
        {
            CheckSize();
            return size;
        }
        set
        {
            size = value;
            if (Event_OnWrapContentItemSizeChanged != null)
            {
                Event_OnWrapContentItemSizeChanged(index, size);
            }
        }
    }

    public Vector2 Position
    {
        get
        {
            if (rectTransform == null)
            {
                rectTransform = this.GetComponent<RectTransform>();
            }
            return rectTransform.anchoredPosition;
        }
    }

    private void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void Scale(float scale)
    {
        CheckSize();
        this.transform.localScale = new Vector3(scale, scale, 1);
        this.size = originSize * scale;
    }

    public void SetSize(Vector2 size)
    {
        CheckSize();
        float scale = size.x / originSize.x;
        this.transform.localScale = new Vector3(scale, scale, 1);
        this.size = size;
    }

    /// <summary>
    /// 通用计算物体大小()
    /// </summary>
    /// <returns></returns>
    protected virtual void ComputeItemSize()
    {
        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform);
        size = bounds.size;
        originSize = bounds.size;
    }

    private void CheckSize()
    {
        if (sizeCacluated == false)
        {
            sizeCacluated = true;
            ComputeItemSize();
        }
    }
}
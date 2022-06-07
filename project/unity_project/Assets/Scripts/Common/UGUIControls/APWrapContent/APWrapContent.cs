using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class APWrapContent : MonoBehaviour, IBeginDragHandler
{
    public enum MoveDirection
    {
        Forward,
        Backward
    }

    /// <summary>
    /// GalleryMode下Item的最小缩放值
    /// </summary>
    public const float SCALE_MIN_SIZE = 0.2f;
    /// <summary>
    /// 创建Item实例事件
    /// </summary>
    public event Action<GameObject> Event_OnCreateWrapContentItem;
    /// <summary>
    /// 刷新Item数据事件
    /// </summary>
    public event Action<GameObject, int> Event_OnRefreshWrapContentItem;
    /// <summary>
    /// 列表内容滚动事件
    /// </summary>
    public event Action Event_OnScrolling;

    /// <summary>
    /// 列表开始拖动事件
    /// </summary>
    public event Action Event_OnBeginDrag;

    /// <summary>
    /// 计算ItemSize事件，初始化就需要生成大小不同的预制时使用
    /// </summary>
    public event Func<int, Vector2> Event_OnCalculateItemSize;

    #region Private Variables
    /// <summary>
    /// 滚动方向
    /// </summary>
    [SerializeField]
    private Slider.Direction direction = Slider.Direction.LeftToRight;
    /// <summary>
    /// 强制刷新
    /// </summary>
    [SerializeField]
    private bool forceRefresh = true;
    /// <summary>
    /// Item间隔
    /// </summary>
    [SerializeField]
    private Vector2 spacing = Vector2.zero;
    /// <summary>
    /// 边界填充
    /// </summary>
    [SerializeField]
    private RectOffset padding;
    /// <summary>
    /// 可以显示的Item的总数
    /// </summary>
    [SerializeField]
    private int dataCount;
    /// <summary>
    /// Item预制体
    /// </summary>
    [SerializeField]
    public GameObject itemPrefab = null;
    /// <summary>
    /// 行数
    /// </summary>
    [SerializeField]
    private int row = 1;
    /// <summary>
    /// 列数
    /// </summary>
    [SerializeField]
    private int column = 1;
    /// <summary>
    /// 是否使用图库模式
    /// </summary>
    [SerializeField]
    private bool useGalleryMode = false;
    /// <summary>
    /// 图库模式下Item尺寸变化曲线
    /// </summary>
    [SerializeField]
    private AnimationCurve galleryItemScaleCurve = AnimationCurve.Linear(0, 1, 0, 1);
    /// <summary>
    /// 图库模式下Item透明度变化曲线
    /// </summary>
    [SerializeField]
    private AnimationCurve galleryItemAlphaCurve = AnimationCurve.Linear(0, 1, 0, 1);
    /// <summary>
    /// 正在使用的Item实例列表
    /// </summary>
    [SerializeField]
    public SortedList<int, APWrapContentItem> usingWrapContentItemList = new SortedList<int, APWrapContentItem>();
    /// <summary>
    /// 列表内容改变时，新增的Item列表，其内容处于未初始化状态
    /// </summary>
    private Queue<APWrapContentItem> willUsingWrapContentItemList = new Queue<APWrapContentItem>();
    /// <summary>
    /// 被回收的Item实例列表
    /// </summary>
    private Queue<APWrapContentItem> recycledWrapContentItemList = new Queue<APWrapContentItem>();
    /// <summary>
    /// 每一个数据索引的Item位置
    /// </summary>
    public Dictionary<int, Vector2> wrapContentItemPositionDic = new Dictionary<int, Vector2>();
    /// <summary>
    /// 每一个数据索引的Item尺寸
    /// </summary>
    public Dictionary<int, Vector2> wrapContentItemSizeDic = new Dictionary<int, Vector2>();
    /// <summary>
    /// ItemPrefab的RectTranform组件
    /// </summary>
    private RectTransform prefabRectTransform;
    /// <summary>
    /// ItemPrefab在当前WrapContent中的AnchoredPosition
    /// </summary>
    private Vector3 prefabAnchoredPosition;
    /// <summary>
    /// Item的默认大小，以预制体为准
    /// </summary>
    private Vector2 itemSize = Vector2.zero;
    /// <summary>
    /// 视野范围
    /// </summary>
    private Vector2 viewPortSize = Vector2.zero;
    /// <summary>
    /// ScrollRect引用
    /// </summary>
    private ScrollRect scrollRect;
    /// <summary>
    /// Content引用
    /// </summary>
    private RectTransform content;
    /// <summary>
    /// 是否支持Item改变大小，多行多列下不支持
    /// </summary>
    private bool supportChangeSize = false;
    /// <summary>
    /// 整个WrapContent是否初始化完成
    /// </summary>
    private bool isInitailized = false;
    /// <summary>
    /// 移动方向，Foward代表向尾部前进，Backward代表向头部前进
    /// </summary>
    private MoveDirection moveDirection = MoveDirection.Forward;
    /// <summary>
    /// 上一帧的ScrollRect Content偏移百分比值
    /// </summary>
    private Vector2 lastScrollRectPercent;

    private string prefabName;
    #endregion

    #region Properties
    public Slider.Direction Direction
    {
        get
        {
            return direction;
        }
    }

    public RectTransform Content
    {
        get
        {
            return content;
        }

        set
        {
            content = value;
        }
    }

    public Vector2 ContentOffset
    {
        get
        {
            return content.anchoredPosition;
        }
    }

    public ScrollRect ScrollRect
    {
        get
        {
            return scrollRect;
        }
    }

    public bool UseGalleryMode
    {
        get
        {
            return useGalleryMode;
        }
    }

    public IList<APWrapContentItem> ItemList
    {
        get
        {
            return usingWrapContentItemList.Values;
        }
    }

    private void Awake()
    {
        InitializeWrapContent();
    }

    #endregion

    #region Public Method
    /// <summary>
    /// 初始化滚动列表，将在Awake时自动调用，但是有很多情况下，业务逻辑代码需要在Awake之前调用
    /// </summary>
    public void InitializeWrapContent()
    {
        if (isInitailized)
        {
            return;
        }
        dataCount = 0;
        isInitailized = true;
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
        prefabName = itemPrefab.name;

        RectTransform viewPort = scrollRect.viewport.GetComponent<RectTransform>();
        viewPortSize = new Vector2(viewPort.rect.width, viewPort.rect.height);

        RectTransform itemRectTransform = itemPrefab.GetComponent<RectTransform>();
        itemSize = new Vector2(itemRectTransform.rect.width, itemRectTransform.rect.height);
        content = scrollRect.content;

        prefabRectTransform = itemPrefab.GetComponent<RectTransform>();
        InitializePrefabAnchoredPosition();

        content.anchoredPosition = Vector2.zero;

        InitializeContentAnchorAndPivot();
        CheckRowAndColumn();
        CheckCanChangeSize();
        if (string.IsNullOrEmpty(itemPrefab.scene.name) == false)
        {
            itemPrefab.SetActive(false);
        }
    }

    /// <summary>
    /// 刷新滚动列表的内容
    /// </summary>
    /// <param name="count">数据的数量</param>
    /// <param name="forceUpdateData">数据整体发生变化为true</param>
    public void RefreshWrapContent(int count, bool forceUpdateData = false)
    {
        if (count < 0)
        {
            return;
        }
        ///数据长度发生变化时，无条件更新全部的缓存信息
        if (dataCount != count || forceUpdateData)
        {
            dataCount = count;
            ResetAllItemSizeDict(count);
            ResetAllItemPosition();
            ResetContentSize();
            ResetContentPosition();
        }
        UpdateItemInViewRange(true);
        if (useGalleryMode)
        {
            for (int i = 0; i < 10; i++)
            {
                UpdateItemInViewRangeForGallery();
                UpdateItemInViewRange(false);
            }
        }
    }

    public APWrapContentItem GetItem(int index)
    {
        if (usingWrapContentItemList.ContainsKey(index))
        {
            return usingWrapContentItemList[index];
        }

        return null;
    }

    /// <summary>
    /// 将Index对应的Item位置调整到视图范围的起始/结束处(瞬间移动）
    /// </summary>
    /// <param name="index">Item索引</param>
    /// <param name="isBeginOrEnd">起始/结束</param>
    /// <returns></returns>
    public Vector2 MoveToTargetPosition(int index, bool isBeginOrEnd = true)
    {
        Vector2 oldPosition = wrapContentItemPositionDic[index];
        Vector2 pos = CalculateContentPositionByIndex(index, isBeginOrEnd);
        if (direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, pos.y);
        }
        else
        {
            content.anchoredPosition = new Vector2(pos.x, content.anchoredPosition.y);
        }
        UpdateItemInViewRange(false);
        return content.anchoredPosition;
    }

    /// <summary>
    /// 将Index对应的Item位置调整到视图范围的起始处(有滚动过程）
    /// </summary>
    /// <param name="index">Item索引</param>
    /// <param name="isBeginOrEnd">起始/结束</param>
    /// <returns></returns>
    public Tweener MoveToTargetPositionAnimated(int index, bool isBeginOrEnd = true)
    {
        Vector2 newPos;
        Vector2 oldPosition = wrapContentItemPositionDic[index];
        Vector2 pos = CalculateContentPositionByIndex(index, isBeginOrEnd);
        float time = 0.2f;
        if (direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom)
        {
            newPos = new Vector2(content.anchoredPosition.x, pos.y);
            time = Mathf.Abs(newPos.y - content.anchoredPosition.y) / viewPortSize.y * 4;
        }
        else
        {
            newPos = new Vector2(pos.x, content.anchoredPosition.y);
            time = Mathf.Abs(newPos.x - content.anchoredPosition.x) / viewPortSize.x * 4;
        }
        time = Mathf.Clamp(time, 0.1f, 0.2f);
        return content.DOAnchorPos(newPos, time).OnUpdate(MovingTweenerUpdate);
    }

    /// <summary>
    /// 将Index对应的Item位置调整到视图范围的中间(瞬间移动）
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector2 MoveTargetToCenter(int index)
    {
        Vector2 oldPosition = wrapContentItemPositionDic[index];
        Vector2 offset = CalculateContentCenterOnPosition(index);
        if (direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, offset.y);
        }
        else
        {
            content.anchoredPosition = new Vector2(offset.x, content.anchoredPosition.y);
        }
        UpdateItemInViewRange(false);
        return content.anchoredPosition;
    }

    /// <summary>
    /// 将Index对应的Item位置调整到视图范围的中间(有滚动过程）
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Tweener MoveTargetToCenterAnimated(int index)
    {
        Vector2 newPos;
        Vector2 oldPosition = wrapContentItemPositionDic[index];
        Vector2 offset = CalculateContentCenterOnPosition(index);
        float time = 1;
        if (direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom)
        {
            newPos = new Vector2(content.anchoredPosition.x, offset.y);
            time = Mathf.Abs(newPos.y - content.anchoredPosition.y) / viewPortSize.y * 4;
        }
        else
        {
            newPos = new Vector2(offset.x, content.anchoredPosition.y);
            time = Mathf.Abs(newPos.x - content.anchoredPosition.x) / viewPortSize.x * 4;
        }
        time = Mathf.Clamp(time, 0.3f, 1);
        return content.DOAnchorPos(newPos, time).OnUpdate(MovingTweenerUpdate);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Event_OnBeginDrag != null)
        {
            Event_OnBeginDrag();
        }
    }
    #endregion

    #region 制作界面时的工具函数，可在Inspector中点击相应的按钮
    /// <summary>
    /// 预览布局效果，用完记得Clear
    /// </summary>
    public void Preview()
    {
        InitializeWrapContent();
        RefreshWrapContent(dataCount, true);
    }

    /// <summary>
    /// 清空预览产生的数据
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < usingWrapContentItemList.Count; i++)
        {
            DestroyImmediate(usingWrapContentItemList[i].gameObject);
            usingWrapContentItemList.RemoveAt(i);
            i--;
        }
        while (recycledWrapContentItemList.Count > 0)
        {
            APWrapContentItem item = recycledWrapContentItemList.Dequeue();
            DestroyImmediate(item.gameObject);
        }
        wrapContentItemSizeDic.Clear();
        wrapContentItemPositionDic.Clear();
    }
    #endregion

    #region Private Method
    /// <summary>
    /// 初始化Content锚点与轴点
    /// </summary>
    private void InitializeContentAnchorAndPivot()
    {
        if (direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom)
        {
            if (direction == Slider.Direction.BottomToTop)
            {
                content.anchorMin = Vector2.zero;
                content.anchorMax = Vector2.right;
                content.pivot = new Vector2(0.5f, 0f);
            }
            else
            {
                content.anchorMin = Vector2.up;
                content.anchorMax = Vector2.one;
                content.pivot = new Vector2(0.5f, 1f);
            }
            scrollRect.vertical = true;
            scrollRect.horizontal = false;
        }
        else
        {
            if (direction == Slider.Direction.LeftToRight)
            {
                content.anchorMin = Vector2.zero;
                content.anchorMax = Vector2.up;
                content.pivot = new Vector2(0, 0.5f);
            }
            else
            {
                content.anchorMin = Vector2.right;
                content.anchorMax = Vector2.one;
                content.pivot = new Vector2(1f, 0.5f);
            }
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
        }
    }

    /// <summary>
    /// 初始化Item预制的锚点与位置
    /// </summary>
    private void InitializePrefabAnchoredPosition()
    {
        if (string.IsNullOrEmpty(prefabRectTransform.gameObject.scene.name))
        {
            RectTransform instanceTransform = Instantiate(prefabRectTransform);
            Vector2 originAnchoredPos = instanceTransform.anchoredPosition;
            instanceTransform.SetParent(this.content.transform);
            instanceTransform.anchoredPosition = originAnchoredPos;
            Vector3 originPos = instanceTransform.position;
            switch (direction)
            {
                case Slider.Direction.LeftToRight:
                case Slider.Direction.TopToBottom:
                    instanceTransform.anchorMin = Vector2.up;
                    instanceTransform.anchorMax = Vector2.up;
                    break;
                case Slider.Direction.RightToLeft:
                    instanceTransform.anchorMin = Vector2.one;
                    instanceTransform.anchorMax = Vector2.one;
                    break;
                case Slider.Direction.BottomToTop:
                    instanceTransform.anchorMin = Vector2.zero;
                    instanceTransform.anchorMax = Vector2.zero;
                    break;
                default:
                    break;
            }
            instanceTransform.position = originPos;
            prefabAnchoredPosition = instanceTransform.anchoredPosition;
            DestroyImmediate(instanceTransform.gameObject);
        }
        else
        {
            Vector3 originPos = prefabRectTransform.position;
            Vector2 originAnchorMin = prefabRectTransform.anchorMin;
            Vector2 originAnchorMax = prefabRectTransform.anchorMax;
            switch (direction)
            {
                case Slider.Direction.LeftToRight:
                case Slider.Direction.TopToBottom:
                    prefabRectTransform.anchorMin = Vector2.up;
                    prefabRectTransform.anchorMax = Vector2.up;
                    break;
                case Slider.Direction.RightToLeft:
                    prefabRectTransform.anchorMin = Vector2.one;
                    prefabRectTransform.anchorMax = Vector2.one;
                    break;
                case Slider.Direction.BottomToTop:
                    prefabRectTransform.anchorMin = Vector2.zero;
                    prefabRectTransform.anchorMax = Vector2.zero;
                    break;
                default:
                    break;
            }
            prefabRectTransform.position = originPos;
            prefabAnchoredPosition = prefabRectTransform.anchoredPosition;
            prefabRectTransform.anchorMin = originAnchorMin;
            prefabRectTransform.anchorMax = originAnchorMax;
            prefabRectTransform.position = originPos;
        }
    }

    /// <summary>
    /// 初始化Item的锚点
    /// </summary>
    private void InitializeItemAnchor(APWrapContentItem item)
    {
        RectTransform itemTransform = item.GetComponent<RectTransform>();
        Vector3 tempPos = itemTransform.position;
        switch (direction)
        {
            case Slider.Direction.LeftToRight:
            case Slider.Direction.TopToBottom:
                itemTransform.anchorMin = Vector2.up;
                itemTransform.anchorMax = Vector2.up;
                break;
            case Slider.Direction.RightToLeft:
                itemTransform.anchorMin = Vector2.one;
                itemTransform.anchorMax = Vector2.one;
                break;
            case Slider.Direction.BottomToTop:
                itemTransform.anchorMin = Vector2.zero;
                itemTransform.anchorMax = Vector2.zero;
                break;
            default:
                break;
        }
        itemTransform.position = tempPos;
    }

    /// <summary>
    /// 重置ItemSize字典
    /// </summary>
    /// <param name="count"></param>
    private void ResetAllItemSizeDict(int count)
    {
        wrapContentItemSizeDic.Clear();
        for (int i = 0; i < count; i++)
        {
            Vector2 size = itemSize;
            if (supportChangeSize)
            {
                //在支持动态改变大小的情况下才会触发计算大小事件
                if (Event_OnCalculateItemSize != null)
                {
                    size = Event_OnCalculateItemSize(i);
                }
            }
            wrapContentItemSizeDic.Add(i, size);
        }
    }

    /// <summary>
    /// 重置ItemPosition字典
    /// </summary>
    private void ResetAllItemPosition()
    {
        wrapContentItemPositionDic.Clear();
        if (row > 1 || column > 1)
        {
            //多行多列暂只支持ItemSize不变的布局
            if (direction == Slider.Direction.TopToBottom)
            {
                for (int i = 0; i < dataCount; i++)
                {
                    float position_x = 0;
                    float position_y = 0;
                    position_x = i % column * (itemSize.x + spacing.x) + padding.left + prefabAnchoredPosition.x;
                    position_y = i / column * (itemSize.y + spacing.y) + padding.top + prefabRectTransform.pivot.y * itemSize.y;
                    wrapContentItemPositionDic.Add(i, new Vector2(position_x, -position_y));
                }
            }
            else if (direction == Slider.Direction.BottomToTop)
            {
                for (int i = 0; i < dataCount; i++)
                {
                    float position_x = 0;
                    float position_y = 0;
                    position_x = i % column * (itemSize.x + spacing.x) + padding.left + prefabAnchoredPosition.x;
                    position_y = i / column * (itemSize.y + spacing.y) + padding.bottom + prefabRectTransform.pivot.y * itemSize.y;
                    wrapContentItemPositionDic.Add(i, new Vector2(position_x, position_y));
                }
            }
            else if (direction == Slider.Direction.LeftToRight)
            {
                for (int i = 0; i < dataCount; i++)
                {
                    float position_x = 0;
                    float position_y = 0;
                    position_x = i / row * (itemSize.x + spacing.x) + padding.left + prefabRectTransform.pivot.x * itemSize.x;
                    position_y = prefabAnchoredPosition.y - (i % row * (itemSize.y + spacing.y) + padding.top);
                    wrapContentItemPositionDic.Add(i, new Vector2(position_x, position_y));
                }
            }
            else if (direction == Slider.Direction.RightToLeft)
            {
                for (int i = 0; i < dataCount; i++)
                {
                    float position_x = 0;
                    float position_y = 0;
                    position_x = i / row * (itemSize.x + spacing.x) + padding.right + prefabRectTransform.pivot.x * itemSize.x;
                    position_y = prefabAnchoredPosition.y - (i % row * (itemSize.y + spacing.y) + padding.top);
                    wrapContentItemPositionDic.Add(i, new Vector2(-position_x, position_y));
                }
            }
        }
        else
        {
            if (direction == Slider.Direction.BottomToTop)
            {
                float size_y = 0;
                for (int i = 0; i < dataCount; i++)
                {
                    if (i == 0)
                    {
                        size_y += padding.bottom + wrapContentItemSizeDic[0].y * prefabRectTransform.pivot.y;
                    }
                    else
                    {
                        size_y += spacing.y + wrapContentItemSizeDic[i - 1].y;
                    }
                    wrapContentItemPositionDic[i] = new Vector2(padding.left + prefabAnchoredPosition.x, size_y);
                }
            }
            else if (direction == Slider.Direction.TopToBottom)
            {
                float size_y = 0;
                for (int i = 0; i < dataCount; i++)
                {
                    if (i == 0)
                    {
                        size_y -= padding.top + wrapContentItemSizeDic[0].y * (1 - prefabRectTransform.pivot.y);
                    }
                    else
                    {
                        size_y -= spacing.y + wrapContentItemSizeDic[i - 1].y;
                    }
                    wrapContentItemPositionDic[i] = new Vector2(padding.left + prefabAnchoredPosition.x, size_y);
                }
            }
            else if (direction == Slider.Direction.LeftToRight)
            {
                float size_x = 0;
                for (int i = 0; i < dataCount; i++)
                {
                    if (i == 0)
                    {
                        size_x += padding.left + wrapContentItemSizeDic[0].x * prefabRectTransform.pivot.x;
                    }
                    else
                    {
                        size_x += spacing.x + wrapContentItemSizeDic[i - 1].x * (1 - prefabRectTransform.pivot.x) + wrapContentItemSizeDic[i].x * prefabRectTransform.pivot.x;
                    }
                    wrapContentItemPositionDic[i] = new Vector2(size_x, prefabAnchoredPosition.y - padding.top);
                }
            }
            else if (direction == Slider.Direction.RightToLeft)
            {
                float size_x = 0;
                for (int i = 0; i < dataCount; i++)
                {
                    if (i == 0)
                    {
                        size_x -= padding.right + wrapContentItemSizeDic[0].x * (1 - prefabRectTransform.pivot.x);
                    }
                    else
                    {
                        size_x -= spacing.x + wrapContentItemSizeDic[i - 1].x;
                    }
                    wrapContentItemPositionDic[i] = new Vector2(size_x, prefabAnchoredPosition.y - padding.top);
                }
            }
        }
    }

    /// <summary>
    /// 重新计算Content的Size
    /// </summary>
    private void ResetContentSize()
    {
        //Vertical
        if (direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom)
        {
            float size_y = 0;
            if (supportChangeSize == false)
            {
                size_y = itemSize.y * Mathf.CeilToInt((float)dataCount / column) + spacing.y * (Mathf.CeilToInt((float)dataCount / column) - 1) + padding.top + padding.bottom;
            }
            else
            {
                foreach (var item in wrapContentItemSizeDic.Keys)
                {
                    size_y += wrapContentItemSizeDic[item].y;
                }
                size_y += (dataCount - wrapContentItemSizeDic.Count) * itemSize.y + spacing.y * (dataCount - 1) + padding.top + padding.bottom;
            }
            content.sizeDelta = new Vector2(0, size_y);
        }
        //Horizontical
        else
        {
            float size_x = 0;
            if (supportChangeSize == false)
            {
                size_x = itemSize.x * Mathf.CeilToInt((float)dataCount / row) + spacing.x * (Mathf.FloorToInt(dataCount / row) - 1) + padding.left + padding.right;
            }
            else
            {
                foreach (var item in wrapContentItemSizeDic.Keys)
                {
                    size_x += wrapContentItemSizeDic[item].x;
                }
                size_x += (dataCount - wrapContentItemSizeDic.Count) * itemSize.x + spacing.x * (dataCount - 1) + padding.left + padding.right;
            }
            content.sizeDelta = new Vector2(size_x, 0);
        }
    }

    /// <summary>
    /// 重置Content位置到起始位置
    /// </summary>
    private void ResetContentPosition()
    {
        float coordinate = 0;
        if (direction == Slider.Direction.TopToBottom)
        {
            if (content.rect.height > viewPortSize.y)
            {
                coordinate = Mathf.Clamp(content.anchoredPosition.y, 0, content.rect.height - viewPortSize.y);
            }
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, coordinate);
        }
        else if (direction == Slider.Direction.BottomToTop)
        {
            if (content.rect.height > viewPortSize.y)
            {
                coordinate = Mathf.Clamp(content.anchoredPosition.y, viewPortSize.y - content.rect.height, 0);
            }
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, coordinate);
        }
        else if (direction == Slider.Direction.LeftToRight)
        {
            if (content.rect.width > viewPortSize.x)
            {
                coordinate = Mathf.Clamp(content.anchoredPosition.x, viewPortSize.x - content.rect.width, 0);
            }
            content.anchoredPosition = new Vector2(coordinate, content.anchoredPosition.y);
        }
        else
        {
            if (content.rect.width > viewPortSize.x)
            {
                coordinate = Mathf.Clamp(content.anchoredPosition.x, 0, content.rect.width - viewPortSize.x);
            }
            content.anchoredPosition = new Vector2(coordinate, content.anchoredPosition.y);
        }
    }

    /// <summary>
    /// 检查是否可以动态的改变大小(在多行多列的情况下不支持Item大小变化)
    /// </summary>
    /// <returns></returns>
    private void CheckCanChangeSize()
    {
        if (direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom)
        {
            supportChangeSize = column <= 1;
        }
        else
        {
            supportChangeSize = row <= 1;
        }
    }

    /// <summary>
    /// 检查行列数，最少为1
    /// </summary>
    private void CheckRowAndColumn()
    {
        if (row < 1)
        {
            row = 1;
        }
        if (column < 1)
        {
            column = 1;
        }
    }

    /// <summary>
    /// 更新Item实例数量
    /// </summary>
    private void UpdateItemInstanceCount(int count)
    {
        if (count < 0)
        {
            return;
        }
        //实例多出的时候将多出的回收
        if (usingWrapContentItemList.Count > count)
        {
            while(usingWrapContentItemList.Count > count)
            {
                //根据当前移动方向从头部或尾部移除
                APWrapContentItem item = null;
                if (moveDirection == MoveDirection.Forward)
                {
                    item = usingWrapContentItemList.Values[0];
                    usingWrapContentItemList.RemoveAt(0);
                }
                else
                {
                    item = usingWrapContentItemList.Values[usingWrapContentItemList.Count - 1];
                    usingWrapContentItemList.RemoveAt(usingWrapContentItemList.Count - 1);
                }
                item.gameObject.SetActive(false);
                recycledWrapContentItemList.Enqueue(item);
            }
        }
        //预制不够用时
        else if (usingWrapContentItemList.Count < count)
        {
            //优先从缓存中获取
            while(recycledWrapContentItemList.Count > 0 && (usingWrapContentItemList.Count + willUsingWrapContentItemList.Count) < count)
            {
                APWrapContentItem item = recycledWrapContentItemList.Dequeue();
                item.gameObject.SetActive(true);
                willUsingWrapContentItemList.Enqueue(item);
            }
            while ((usingWrapContentItemList.Count + willUsingWrapContentItemList.Count) < count)
            {
                GameObject go = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, content);
                go.transform.SetParent(content);
                go.SetActive(true);
                APWrapContentItem item = go.GetComponent<APWrapContentItem>();
                item.Event_OnWrapContentItemSizeChanged += Response_ItemSizeChanged;
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = itemPrefab.transform.localPosition;
                InitializeItemAnchor(item);
                willUsingWrapContentItemList.Enqueue(item);
                if (Event_OnCreateWrapContentItem != null)
                {
                    Event_OnCreateWrapContentItem(item.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 更新ItemSize字典
    /// </summary>
    /// <param name="index"></param>
    /// <param name="newSize"></param>
    private void UpdateItemSize(int index, Vector2 newSize)
    {
        wrapContentItemSizeDic[index] = newSize;
    }

    /// <summary>
    /// 更新视野范围内的所有Item
    /// </summary>
    /// <param name="forceUpdateData">索引相同时是否更新数据</param>
    private void UpdateItemInViewRange(bool forceUpdateData)
    {
        int startIndex = CalculateItemStartIndexInViewRange();
        int endIndex = CalculateItemEndIndexInViewRange();
        int count = Mathf.Clamp(endIndex - startIndex + 1, 0, dataCount);
        UpdateItemInstanceCount(count);
        if (count > 0)
        {
            if (startIndex >= dataCount)
            {
                endIndex = startIndex;
            }
            if (endIndex >= dataCount)
            {
                endIndex = dataCount - 1;
            }
            UpdateItemsWithIndex(startIndex, endIndex, forceUpdateData);
        }
    }

    /// <summary>
    /// 以图库（中间大，两边小，Alpha中间1，两边小）的形式更新视野范围内的Item
    /// </summary>
    private void UpdateItemInViewRangeForGallery()
    {
        Vector2 halfView = viewPortSize / 2;
        Vector2 scrollRectCenter = Vector2.zero;
        if (direction == Slider.Direction.BottomToTop)
        {
            scrollRectCenter = halfView - this.ContentOffset;
        }
        else if (direction == Slider.Direction.TopToBottom)
        {
            scrollRectCenter = this.ContentOffset - halfView;
        }
        else if (direction == Slider.Direction.LeftToRight)
        {
            scrollRectCenter = halfView - this.ContentOffset;
        }
        else if (direction == Slider.Direction.RightToLeft)
        {
            scrollRectCenter = this.ContentOffset - halfView;
        }

        float minScale = galleryItemScaleCurve.Evaluate(1);
        if (minScale < SCALE_MIN_SIZE)
        {
            //避免缩为0无限小导致宽度上可以放无限个物体的死循环
            minScale = SCALE_MIN_SIZE;
        }
        Vector2 minSize = minScale * itemSize;

        //先将所有Item的Size设为最小值
        for (int i = 0; i < dataCount; i++)
        {
            wrapContentItemSizeDic[i] = minSize;
        }
        //然后更新视野范围内的Item的Size
        foreach (APWrapContentItem item in usingWrapContentItemList.Values)
        {
            Vector2 itemCenter = item.Position + new Vector2(item.Size.x / 2 * (prefabRectTransform.pivot.x - 0.5f), item.Size.y / 2 * (prefabRectTransform.pivot.y - 0.5f));
            Vector2 distance = itemCenter - scrollRectCenter;
            float percent = 1;
            if (direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom)
            {
                percent = distance.y / halfView.y;
            }
            else if (direction == Slider.Direction.LeftToRight || direction == Slider.Direction.RightToLeft)
            {
                percent = distance.x / halfView.x;
            }
            item.RelativePosToCenter = percent;
            percent = Mathf.Clamp01(Mathf.Abs(percent));

            float scale = galleryItemScaleCurve.Evaluate(percent);
            if (scale < SCALE_MIN_SIZE)
            {
                //避免缩为0无限小导致宽度上可以放无限个物体的死循环
                scale = SCALE_MIN_SIZE;
            }
            item.Scale(scale);
            UpdateItemSize(item.Index, item.Size);

            float alpha = galleryItemAlphaCurve.Evaluate(percent);
            CanvasGroup cg = item.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = item.gameObject.AddComponent<CanvasGroup>();
            }
            cg.alpha = alpha;
        }
        ResetContentSize();
        ResetAllItemPosition();
    }

    /// <summary>
    /// 根据索引更新Item位置和数据
    /// </summary>
    /// <param name="startIndex">起始索引(包含)</param>
    /// <param name="endIndex">结尾索引(包含)</param>
    /// <param name="forceUpdateData">索引未发生变化的Item是否需要更新数据</param>
    private void UpdateItemsWithIndex(int startIndex, int endIndex, bool forceUpdateData)
    {
        //先将usingWrapContentItemList中和当前要显示的索引范围重合的物体处理掉
        for (int i = 0; i < usingWrapContentItemList.Values.Count; i++)
        {
            APWrapContentItem item = usingWrapContentItemList.Values[i];
            bool inViewRange = item.Index >= startIndex && item.Index <= endIndex;
            
            if (inViewRange)
            {
                item.RectTransform.anchoredPosition = wrapContentItemPositionDic[item.Index];
                if (forceUpdateData || forceRefresh)
                {
                    UpdateOneItemData(item.Index);
                }
            }
            else
            {
                usingWrapContentItemList.Remove(item.Index);
                willUsingWrapContentItemList.Enqueue(item);
                //注意，因为从usingWrapContentItemList中移除了一项，索引要回退1，不然循环就会出现跳过一项的bug
                i--;
            }
        }

        //再处理剩下没有满足的索引物体
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (i >= dataCount)
            {
                //当i超过dataCount时，说明已经没有数据可以展示了
                break;
            }
            if (usingWrapContentItemList.ContainsKey(i) == false)
            {
                APWrapContentItem item = willUsingWrapContentItemList.Dequeue();
                item.RectTransform.anchoredPosition = wrapContentItemPositionDic[i];
                item.name = string.Format("{0}_{1}", prefabName, i);
                usingWrapContentItemList.Add(i, item);
                UpdateOneItemData(i);
            }
        }
    }

    /// <summary>
    /// 更新一个Item的数据
    /// </summary>
    /// <param name="itemKey"></param>
    /// <param name="dataIndex"></param>
    private void UpdateOneItemData(int dataIndex)
    {
        usingWrapContentItemList[dataIndex].Index = dataIndex;
        //理论上来说，只要按列表移动方向来进行实例回收的话，是不需要重新赋值缩放与Alpha的，这里只是为了做个保障
        //Alpha先不重新赋值，留着排查问题（没有按列表移动方向来进行实例回收的问题）
        if (useGalleryMode)
        {
            usingWrapContentItemList[dataIndex].SetSize(wrapContentItemSizeDic[dataIndex]);
        }
        if (Event_OnRefreshWrapContentItem != null)
        {
            Event_OnRefreshWrapContentItem(usingWrapContentItemList[dataIndex].gameObject, dataIndex);
        }
    }

    /// <summary>
    /// 计算出当前视野范围应该显示的Item的起始索引
    /// </summary>
    private int CalculateItemStartIndexInViewRange()
    {
        Vector2 pos = content.anchoredPosition;
        int startIndex = 0;
        //计算content在当前位置的时候应该显示的索引
        if (direction == Slider.Direction.TopToBottom)
        {
            for (int i = 0; i < wrapContentItemPositionDic.Count; i++)
            {
                if (-pos.y >= wrapContentItemPositionDic[i].y - wrapContentItemSizeDic[i].y * prefabRectTransform.pivot.y)
                {
                    startIndex = i;
                    break;
                }
            }
        }
        else if (direction == Slider.Direction.BottomToTop)
        {
            for (int i = 0; i < wrapContentItemPositionDic.Count; i++)
            {
                if (-pos.y <= wrapContentItemPositionDic[i].y + wrapContentItemSizeDic[i].y * prefabRectTransform.pivot.y)
                {
                    startIndex = i;
                    break;
                }
            }
        }
        else if (direction == Slider.Direction.LeftToRight)
        {
            for (int i = 0; i < wrapContentItemPositionDic.Count; i++)
            {
                if (-pos.x <= wrapContentItemPositionDic[i].x + wrapContentItemSizeDic[i].x * prefabRectTransform.pivot.x)
                {
                    startIndex = i;
                    break;
                }
            }
        }
        else if (direction == Slider.Direction.RightToLeft)
        {
            for (int i = 0; i < wrapContentItemPositionDic.Count; i++)
            {
                if (-pos.x >= wrapContentItemPositionDic[i].x - wrapContentItemSizeDic[i].x * prefabRectTransform.pivot.x)
                {
                    startIndex = i;
                    break;
                }
            }
        }
        startIndex = Mathf.Clamp(startIndex, 0, dataCount - 1);
        return startIndex;
    }

    /// <summary>
    /// 计算出当前视野范围应该显示的Item的末尾索引
    /// </summary>
    /// <returns></returns>
    private int CalculateItemEndIndexInViewRange()
    {
        Vector2 pos = content.anchoredPosition;
        int endIndex = wrapContentItemPositionDic.Count;
        if (direction == Slider.Direction.TopToBottom)
        {
            for (int i = wrapContentItemPositionDic.Count - 1; i >= 0; i--)
            {
                if ((-pos.y - viewPortSize.y) <= wrapContentItemPositionDic[i].y + wrapContentItemSizeDic[i].y * prefabRectTransform.pivot.y)
                {
                    endIndex = i;
                    break;
                }
            }
        }
        else if (direction == Slider.Direction.BottomToTop)
        {
            for (int i = wrapContentItemPositionDic.Count - 1; i >= 0; i--)
            {
                if ((-pos.y + viewPortSize.y) >= wrapContentItemPositionDic[i].y - wrapContentItemSizeDic[i].y * prefabRectTransform.pivot.y)
                {
                    endIndex = i;
                    break;
                }
            }
        }
        else if (direction == Slider.Direction.LeftToRight)
        {
            for (int i = wrapContentItemPositionDic.Count - 1; i >= 0 ; i--)
            {
                if ((viewPortSize.x - pos.x) >= wrapContentItemPositionDic[i].x - wrapContentItemSizeDic[i].x * prefabRectTransform.pivot.x)
                {
                    endIndex = i;
                    break;
                }
            }
        }
        else if (direction == Slider.Direction.RightToLeft)
        {
            for (int i = wrapContentItemPositionDic.Count - 1; i >= 0 ; i--)
            {
                if ((-pos.x - viewPortSize.x) <= wrapContentItemPositionDic[i].x + wrapContentItemSizeDic[i].x * prefabRectTransform.pivot.x)
                {
                    endIndex = i;
                    break;
                }
            }
        }
        endIndex = Mathf.Clamp(endIndex, 0, dataCount - 1);
        return endIndex;
    }

    /// <summary>
    /// 计算将Index对应的Item位置调整到视图范围的起始/结束处需要的Content偏移值
    /// </summary>
    /// <param name="index">Item索引</param>
    /// <param name="isBeginOrEnd">起始/结束</param>
    /// <param name="clampResult">是否限制结果在视口范围内</param>
    /// <returns></returns>
    private Vector2 CalculateContentPositionByIndex(int index, bool isBeginOrEnd = true, bool clampResult = true)
    {
        Vector2 pos = Vector2.zero;
        if (isBeginOrEnd)
        {
            if (useGalleryMode)
            {
                float minScale = galleryItemScaleCurve.Evaluate(1);
                if (minScale < SCALE_MIN_SIZE)
                {
                    //避免缩为0无限小导致宽度上可以放无限个物体的死循环
                    minScale = SCALE_MIN_SIZE;
                }
                if (direction == Slider.Direction.BottomToTop)
                {
                    pos.y = -(minScale * itemSize.y * index);
                    if (clampResult)
                    {
                        pos.y = Mathf.Clamp(pos.y, viewPortSize.y - content.rect.height, 0);
                    }
                }
                else if (direction == Slider.Direction.TopToBottom)
                {
                    pos.y = minScale * itemSize.y * index;
                    if (clampResult)
                    {
                        pos.y = Mathf.Clamp(pos.y, 0, content.rect.height - viewPortSize.y);
                    }
                }
                else if (direction == Slider.Direction.LeftToRight)
                {
                    pos.x = -(minScale * itemSize.x * index);
                    if (clampResult)
                    {
                        pos.x = Mathf.Clamp(pos.x, viewPortSize.x - content.rect.width, 0);
                    }
                }
                else if (direction == Slider.Direction.RightToLeft)
                {
                    pos.x = minScale * itemSize.x * index;
                    if (clampResult)
                    {
                        pos.x = Mathf.Clamp(pos.x, 0, content.rect.width - viewPortSize.x);
                    }
                }
            }
            else
            {
                if (direction == Slider.Direction.BottomToTop)
                {
                    pos.y = -wrapContentItemPositionDic[index + column].y + viewPortSize.y;
                    if (clampResult)
                    {
                        pos.y = Mathf.Clamp(pos.y, viewPortSize.y - content.rect.height, 0);
                    }
                }
                else if (direction == Slider.Direction.TopToBottom)
                {
                    pos.y = -wrapContentItemPositionDic[index].y - wrapContentItemSizeDic[index].y * (1 - prefabRectTransform.pivot.y);
                    if (clampResult)
                    {
                        pos.y = Mathf.Clamp(pos.y, 0, content.rect.height - viewPortSize.y);
                    }
                }
                else if (direction == Slider.Direction.LeftToRight)
                {
                    pos.x = -wrapContentItemPositionDic[index].x + wrapContentItemSizeDic[index].x * prefabRectTransform.pivot.x;
                    if (clampResult)
                    {
                        pos.x = Mathf.Clamp(pos.x, viewPortSize.x - content.rect.width, 0);
                    }
                }
                else if (direction == Slider.Direction.RightToLeft)
                {
                    pos.x = -wrapContentItemPositionDic[index + row].x - wrapContentItemSizeDic[index].x * (1- prefabRectTransform.pivot.x) -padding.right;
                    if (clampResult)
                    {
                        pos.x = Mathf.Clamp(pos.x, 0, content.rect.width - viewPortSize.x);
                    }
                }
            }
        }
        else
        {
            if (useGalleryMode)
            {
                Debug.LogError("APWrapContent Gallery 暂不支持此模式，原因：计算过于烦繁");
            }
            else
            {
                if (direction == Slider.Direction.BottomToTop)
                {
                    pos.y = -wrapContentItemPositionDic[index].y;
                    if (clampResult)
                    {
                        pos.y = Mathf.Clamp(pos.y, viewPortSize.y - content.rect.height, 0);
                    }
                }
                else if (direction == Slider.Direction.TopToBottom)
                {
                    pos.y = -wrapContentItemPositionDic[index + column].y - wrapContentItemSizeDic[index].y * prefabRectTransform.pivot.y + padding.bottom;
                    if (clampResult)
                    {
                        pos.y = Mathf.Clamp(pos.y, 0, content.rect.height - viewPortSize.y);
                    }
                }
                else if (direction == Slider.Direction.LeftToRight)
                {
                    pos.x = -wrapContentItemPositionDic[index].x;
                    if (clampResult)
                    {
                        pos.x = Mathf.Clamp(pos.x, viewPortSize.x - content.rect.width, 0);
                    }
                }
                else if (direction == Slider.Direction.RightToLeft)
                {
                    pos.x = -wrapContentItemPositionDic[index + row].x - viewPortSize.x;
                    if (clampResult)
                    {
                        pos.x = Mathf.Clamp(pos.x, 0, content.rect.width - viewPortSize.x);
                    }
                }
            }
        }
        return pos;
    }

    /// <summary>
    /// 计算将Index对应的Item位置调整到视图范围中间所需要的Content偏移值
    /// </summary>
    /// <param name="index">Item索引</param>
    /// <returns></returns>
    private Vector2 CalculateContentCenterOnPosition(int index)
    {
        Vector2 contentOffset = Vector2.zero;
        if (useGalleryMode)
        {
            Vector2 halfView = viewPortSize / 2;
            float minScale = galleryItemScaleCurve.Evaluate(1);
            if (minScale < SCALE_MIN_SIZE)
            {
                //避免缩为0无限小导致宽度上可以放无限个物体的死循环
                minScale = SCALE_MIN_SIZE;
            }

            Vector2 totalItemSize = Vector2.zero;

            //加上最中间的ItemSize
            totalItemSize += itemSize;
            Vector2 preItemSize = itemSize;

            int itemCount = 0;
            if (direction == Slider.Direction.BottomToTop || direction == Slider.Direction.TopToBottom)
            {
                //TODO: 实现竖向的GalleryMode显示
                Debug.LogError("APWrapContent 尚未实现的Feature");
            }
            else if (direction == Slider.Direction.LeftToRight || direction == Slider.Direction.RightToLeft)
            {
                while (totalItemSize.x < viewPortSize.x)
                {
                    itemCount++;
                    //以下公式仅适用于线性变化曲线
                    float nextSize = (halfView.x * 2 - totalItemSize.x) * itemSize.x / (halfView.x * 2 + itemSize.x);
                    //使用逼近法适配任意曲线
                    //先假设一个值，检验这个值合适不，初始为前一Item尺寸的一半
                    float assumeNextSize = preItemSize.x / 2;
                    float assumeNextPos = (totalItemSize.x + assumeNextSize) / 2;
                    float percent = assumeNextPos / halfView.x;
                    float scaleFromCurve = galleryItemScaleCurve.Evaluate(percent);
                    float sizeByScale = itemSize.x * scaleFromCurve;
                    float delta          = sizeByScale - assumeNextSize;
                    while(Mathf.Abs(delta) > 1)
                    {
                        assumeNextSize = (sizeByScale + assumeNextSize) / 2;
                        assumeNextPos = (totalItemSize.x + assumeNextSize) / 2;
                        percent = assumeNextPos / halfView.x;
                        scaleFromCurve = galleryItemScaleCurve.Evaluate(percent);
                        sizeByScale = itemSize.x * scaleFromCurve;
                        delta = sizeByScale - assumeNextSize;
                    }
                    nextSize = assumeNextSize;

                    if (nextSize < itemSize.x * minScale)
                    {
                        nextSize = itemSize.x * minScale;
                    }
                    totalItemSize.x += nextSize * 2;
                }
                int minScaleItemCount = index - itemCount;
                if (minScaleItemCount < 0)
                {
                    minScaleItemCount = 0;
                }
                contentOffset.x = -(totalItemSize.x - viewPortSize.x) / 2 - minScaleItemCount * minScale * itemSize.x;
            }
        }
        else
        {
            if (direction == Slider.Direction.BottomToTop)
            {
                contentOffset = CalculateContentPositionByIndex(index, true,false);
                contentOffset.y = Mathf.Clamp(contentOffset.y -= viewPortSize.y / 2, viewPortSize.y - content.rect.height, 0);
            }
            else if (direction == Slider.Direction.TopToBottom)
            {
                contentOffset = CalculateContentPositionByIndex(index, true, false);
                contentOffset.y = Mathf.Clamp(contentOffset.y -= viewPortSize.y / 2 - wrapContentItemSizeDic[index].y * (1 - prefabRectTransform.pivot.y), 0, content.rect.height - viewPortSize.y);
            }
            else if (direction == Slider.Direction.LeftToRight)
            {
                contentOffset = CalculateContentPositionByIndex(index, true, false);
                contentOffset.x = Mathf.Clamp(contentOffset.x += viewPortSize.x / 2 - wrapContentItemSizeDic[index].x * (1 - prefabRectTransform.pivot.x), viewPortSize.x - content.rect.width, 0);
            }
            else if (direction == Slider.Direction.RightToLeft)
            {
                contentOffset = CalculateContentPositionByIndex(index, true,false);
                contentOffset.x = Mathf.Clamp(contentOffset.x -= viewPortSize.x / 2 + wrapContentItemSizeDic[index].x * prefabRectTransform.pivot.x, viewPortSize.x - content.rect.width, 0);
            }
        }
        return contentOffset;
    }

    /// <summary>
    /// 响应Item的Size变化事件
    /// </summary>
    /// <param name="index"></param>
    /// <param name="newSize"></param>
    private void Response_ItemSizeChanged(int index, Vector2 newSize)
    {
        //支持动态改变大小功能时才会更新
        if (supportChangeSize)
        {
            UpdateItemSize(index, newSize);
            ResetContentSize();
            ResetAllItemPosition();
            UpdateItemInViewRange(false);
        }
    }

    /// <summary>
    /// Tweener动画更新过程中实时刷新Item
    /// </summary>
    private void MovingTweenerUpdate()
    {
        UpdateItemInViewRange(false);
    }

    /// <summary>
    /// 滚动列表滚动响应，自动刷新Item
    /// </summary>
    /// <param name="percent"></param>
    private void OnScrollRectValueChanged(Vector2 percent)
    {
        if (direction == Slider.Direction.BottomToTop)
        {
            moveDirection = lastScrollRectPercent.y > percent.y ? MoveDirection.Backward : MoveDirection.Forward;
        }
        else if (direction == Slider.Direction.TopToBottom)
        {
            moveDirection = lastScrollRectPercent.y > percent.y ? MoveDirection.Forward : MoveDirection.Backward;
        }
        else if (direction == Slider.Direction.LeftToRight)
        {
            moveDirection = lastScrollRectPercent.x > percent.x ? MoveDirection.Backward : MoveDirection.Forward;
        }
        else if (direction == Slider.Direction.RightToLeft)
        {
            moveDirection = lastScrollRectPercent.x > percent.x ? MoveDirection.Forward : MoveDirection.Backward;
        }

        lastScrollRectPercent = percent;
        if (useGalleryMode)
        {
            UpdateItemInViewRangeForGallery();
        }
        UpdateItemInViewRange(false);
        if (Event_OnScrolling != null)
        {
            Event_OnScrolling();
        }
    }
    #endregion

}
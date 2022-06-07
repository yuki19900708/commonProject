using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class APSlider : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public event Action Event_OnAnimationCompleted;
    public event Action<float> Event_OnValueChanged;
    public event Action<float> Event_OnValueReachMax;

    public enum TimeType
    {
        AllAnimationTime,
        OnceAnimationTime
    }

    public EaseType easeType = EaseType.Liner;
    public TimeType timeType = TimeType.AllAnimationTime;

    public RectTransform foreArea;
    public Image foreground;
    public Image handleImage;
    public float totalTime = 1;

    [SerializeField]
    private float value;
    [SerializeField]
    private Slider.Direction direction = Slider.Direction.LeftToRight;
    [SerializeField]
    private bool isInteraction = false;
    [SerializeField]
    private bool isDynamic = false;
    private Canvas canvas;

    public Slider.Direction Direction
    {
        get { return direction; }
        set
        {
            direction = value;
            SetDirectionValue(value);
        }
    }

    public float Value
    {
        get { return value; }
        set
        {
            SetValue(value);
        }
    }

    private float timer;
    public float animationTime;
    private float currentValue;
    private float lastValue;
    private bool isPlaying = false;
    private Image.Type imageType;

    private void Awake()
    {
        InitializeAPSlider();
    }

    private void Update()
    {
        if (isPlaying)
        {
            timer += Time.deltaTime;
            PlaySliderAnimation();
        }
    }

    #region Public Method

    public void SetValueWithOutAnimation(float value)
    {
        this.value = value;
        currentValue = value;
        lastValue = value;
        SetForegroundSize(value);
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void Play()
    {
        isPlaying = true;
    }

    public float GetCurrentValue()
    {
        return currentValue;
    }

    #endregion

    #region Private Method

    private void InitializeAPSlider()
    {
        canvas = GetComponentInParent<Canvas>();
        imageType = foreground.type;
        if (handleImage != null)
        {
            handleImage.transform.SetParent(foreground.transform);
            UnityAction<BaseEventData> drag = new UnityAction<BaseEventData>(Response_OnHandleDrag);
            EventTrigger.Entry dragEntry = new EventTrigger.Entry();
            dragEntry.eventID = EventTriggerType.Drag;
            dragEntry.callback.AddListener(drag);
            EventTrigger handleEvent = handleImage.gameObject.AddComponent<EventTrigger>();
            handleEvent.triggers.Add(dragEntry);

            if (direction == Slider.Direction.LeftToRight)
            {
                handleImage.rectTransform.anchorMax = new Vector2(1, 0.5f);
                handleImage.rectTransform.anchorMin = new Vector2(1, 0.5f);
            }
            else if (direction == Slider.Direction.RightToLeft)
            {
                handleImage.rectTransform.anchorMax = new Vector2(0, 0.5f);
                handleImage.rectTransform.anchorMin = new Vector2(0, 0.5f);
            }
            else if (direction == Slider.Direction.TopToBottom)
            {
                handleImage.rectTransform.anchorMax = new Vector2(0.5f, 0);
                handleImage.rectTransform.anchorMin = new Vector2(0.5f, 0);
            }
            else
            {
                handleImage.rectTransform.anchorMax = new Vector2(0.5f, 1);
                handleImage.rectTransform.anchorMin = new Vector2(0.5f, 1);
            }
            handleImage.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    private void Response_OnHandleDrag(BaseEventData eventData)
    {
        //交互使用时才会响应点击拖拽事件
        if (isInteraction)
        {
            Vector2 pos = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                ((PointerEventData)eventData).position, canvas.worldCamera, out pos))
            {
                CalculatePercentByPosition(pos);
            }
        }
    }

    private void SetDirectionValue(Slider.Direction direction)
    {
        switch (direction)
        {
            case Slider.Direction.LeftToRight:
                foreground.rectTransform.anchorMax = Vector2.up;
                foreground.rectTransform.anchorMin = Vector2.zero;
                foreground.rectTransform.pivot = new Vector2(0, 0.5f);
                break;
            case Slider.Direction.RightToLeft:
                foreground.rectTransform.anchorMax = Vector2.one;
                foreground.rectTransform.anchorMin = Vector2.right;
                foreground.rectTransform.pivot = new Vector2(1, 0.5f);
                break;
            case Slider.Direction.TopToBottom:
                foreground.rectTransform.anchorMax = Vector2.one;
                foreground.rectTransform.anchorMin = Vector2.up;
                foreground.rectTransform.pivot = new Vector2(0.5f, 1);
                break;
            case Slider.Direction.BottomToTop:
                foreground.rectTransform.anchorMax = Vector2.right;
                foreground.rectTransform.anchorMin = Vector2.zero;
                foreground.rectTransform.pivot = new Vector2(0.5f, 0);
                break;
            default:
                break;
        }
    }

    private void SetValue(float value)
    {
        this.value = value;
        if (isInteraction)
        {
            SetForegroundSize(LimitPercent(value));
            if (Event_OnValueChanged != null)
            {
                Event_OnValueChanged(value);
            }
        }
        else
        {
            if (isDynamic)
            {
                CalculateAnimationTime();
            }
            else
            {
                OnAnimationCompleted();
            }
        }
    }

    private void CalculateAnimationTime()
    {
        lastValue = currentValue;
        if (timeType == TimeType.AllAnimationTime)
        {
            animationTime = Mathf.Abs(value - lastValue) * totalTime;
        }
        else
        {
            animationTime = totalTime;
        }
        isPlaying = true;
        timer = 0;
    }

    private void OnAnimationCompleted()
    {
        SetForegroundSize(LimitPercent(value));
        currentValue = value;
    }

    private void PlaySliderAnimation()
    {
        if (timer > animationTime)
        {
            isPlaying = false;
            timer = 0;
            OnAnimationCompleted();
            if (Event_OnAnimationCompleted != null)
            {
                Event_OnAnimationCompleted();
            }
        }
        else
        {
            float result = EaseUtil.EasingMethod(timer, lastValue, value - lastValue, animationTime, easeType);
            SetForegroundSize(LimitPercent(result));
            if (Mathf.Abs((int)result - (int)currentValue) > 0)
            {
                if (Event_OnValueReachMax != null)
                {
                    Event_OnValueReachMax(result);
                }
            }
            currentValue = result;
        }
    }

    /// <summary>
    /// 设置进度条新的大小
    /// </summary>
    /// <param name="percent"></param>
    /// <returns></returns>
    private void SetForegroundSize(float percent)
    {
        if (imageType == Image.Type.Filled)
        {
            foreground.fillAmount = percent;
        }
        else
        {
            if (direction == Slider.Direction.LeftToRight || direction == Slider.Direction.RightToLeft)
            {
                foreground.rectTransform.sizeDelta = new Vector2(foreArea.rect.width * percent, 0);
            }
            else
            {
                foreground.rectTransform.sizeDelta = new Vector2(0, foreArea.rect.height * percent);
            }
        }
    }

    private float LimitPercent(float percent)
    {
        if (percent > 1)
        {
            percent = percent - (int)percent;
        }
        else if (percent < 0)
        {
            percent = 0;
        }
        return percent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        EventSystemMethod(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        EventSystemMethod(eventData);
    }

    private void EventSystemMethod(PointerEventData eventData)
    {
        //填充区域以外不响应点击或者拖拽事件
        if (RectTransformUtility.RectangleContainsScreenPoint(foreArea, eventData.position, Camera.main))
        {
            //交互使用时才会响应点击拖拽事件
            if (isInteraction)
            {
                Vector2 pos = Vector2.zero;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                    eventData.position, canvas.worldCamera, out pos))
                {
                    CalculatePercentByPosition(pos);
                }
            }
        }
    }

    /// <summary>
    /// 根据点击的位置显示slider位置
    /// </summary>
    /// <param name="pos"></param>
    private void CalculatePercentByPosition(Vector2 pos)
    {
        float percent = 0;
        if (direction == Slider.Direction.LeftToRight)
        {
            percent = (pos.x + foreArea.rect.width / 2) / foreArea.rect.width;
        }
        else if (direction == Slider.Direction.RightToLeft)
        {
            percent = 1 - (pos.x + foreArea.rect.width / 2) / foreArea.rect.width;
        }
        else if (direction == Slider.Direction.TopToBottom)
        {
            percent = 1 - (pos.y + foreArea.rect.height / 2) / foreArea.rect.height;
        }
        else
        {
            percent = (pos.y + foreArea.rect.height / 2) / foreArea.rect.height;
        }
        SetValue(LimitPercent(percent));
    }

    #endregion
}
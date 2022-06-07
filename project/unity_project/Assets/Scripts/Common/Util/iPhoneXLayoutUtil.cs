using UnityEngine;

public class iPhoneXLayoutUtil : MonoBehaviour
{
    public const int SCREEN_DESIGN_WIDTH = 1334;
    public const int SCREEN_DESIGN_HEIGHT = 750;
    public const float SCREEN_RATIO_HW = SCREEN_DESIGN_HEIGHT * 1.0f / SCREEN_DESIGN_WIDTH;
    public const int IPHONE_X_SCREEN_WIDTH = 2436;
    public const int IPHONE_X_SCREEN_HEIGHT = 1125;
    public const int IPHONE_X_TOP_DANGER_HEIGHT = 132;
    public const int IPHONE_X_SAFE_WIDTH = IPHONE_X_SCREEN_WIDTH - IPHONE_X_TOP_DANGER_HEIGHT * 2;
    public const int IPHONE_X_BOTTOM_DANGER_HEIGHT = 60;

    public const int IPHONE_X_LEFT_OFFSET = 22;
    public const int IPHONE_X_RIGHT_OFFSET = 44;
    public const int IPHONE_X_BOTTOM_OFFSET = 29;

    public bool onlyAdjustBottom = false;
    public bool doTest = false;
    public ScreenOrientation testOrientation = ScreenOrientation.LandscapeLeft;

    private ScreenOrientation orientation;
#if UNITY_IPHONE && !UNITY_EDITOR
    private RectTransform rectTransform;

    private void Start()
    {
        if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX)
        {
            orientation = doTest ? testOrientation : Screen.orientation;
            rectTransform = this.GetComponent<RectTransform>();
            AdjustUILayoutOnIPhoneX(rectTransform, onlyAdjustBottom, orientation);
            InvokeRepeating("CheckOrientation", 0.5f, 0.5f);
        }
    }

    private void CheckOrientation()
    {
        var targetOrientation = doTest ? testOrientation : Screen.orientation;
        if (orientation != targetOrientation)
        {
            orientation = targetOrientation;
            AdjustUILayoutOnIPhoneX(rectTransform, onlyAdjustBottom, orientation);
        }
    }

    /// <summary>
    /// 适配iPhoneX的UI调整算法
    /// </summary>
    /// <param name="outline">outline是指某个UI界面的根物体的一个子物体，该子物体的RectTransform设定为宽高皆为Stretch,上下左右边缘距离为0，轴点为0.5,0.5</param>
    /// <param name="onlyBottom">是否只调整底部区域，</param>
    public static void AdjustUILayoutOnIPhoneX(RectTransform outline, bool onlyBottom, ScreenOrientation orientation)
    {
        if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX)
        {
            float safeRatio = IPHONE_X_SAFE_WIDTH * 1.0f / IPHONE_X_SCREEN_HEIGHT;
            float safeWidth = SCREEN_DESIGN_HEIGHT * safeRatio;
            float realWidth = SCREEN_DESIGN_HEIGHT * Screen.width * 1.0f / Screen.height;
            float widthDiff = realWidth - safeWidth;
            float heightDiff = IPHONE_X_BOTTOM_DANGER_HEIGHT * SCREEN_DESIGN_HEIGHT * 1.0f / Screen.height;
            heightDiff -= IPHONE_X_BOTTOM_OFFSET;
        
            if (orientation == ScreenOrientation.LandscapeLeft)
            {
                outline.offsetMin = new Vector2(widthDiff / 2 - IPHONE_X_LEFT_OFFSET, heightDiff);
                outline.offsetMax = new Vector2(IPHONE_X_RIGHT_OFFSET - widthDiff / 2, 0);
            }
            else if(orientation == ScreenOrientation.LandscapeRight)
            {
                outline.offsetMin = new Vector2(widthDiff / 2 - IPHONE_X_RIGHT_OFFSET, heightDiff);
                outline.offsetMax = new Vector2(IPHONE_X_LEFT_OFFSET - widthDiff / 2, 0);
            }
        }
    }
#endif

    /// <summary>
    /// 获取iPhoneX上底部避开危险区需要抬升的数值
    /// </summary>
    /// <returns></returns>
    public static float GetAdjustUILayoutOnIPhoneXBottomValue()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX)
        {
            float heightDiff = IPHONE_X_BOTTOM_DANGER_HEIGHT * SCREEN_DESIGN_HEIGHT * 1.0f / Screen.height;
            return heightDiff;
        }
        else
        {
            return 0;
        }
#else
        return 0;
#endif
    }
}

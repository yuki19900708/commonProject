using UnityEngine;
using System;
using DG.Tweening;

enum OperateType
{
    move,
    rotate,
}

[RequireComponent(typeof(CameraGestureConfig))]
public class CameraGestureMgr : MonoBehaviour
{
    public static Func<bool> Event_CheckTutorailOver;


    public const float ENTER_GAME_ORTHOGRAPHIC_SIZE = 6.5f;

    public const float LOCK_PLUNDER_ORTHOGRAPHIC_SIZE = 1.5f;

    /// <summary>摄像机放大orthographicSize的可超越幅度值 </summary>
    public const float MAX_SIZE_EXCEED = 1f;

    /// <summary>摄像机缩小orthographicSize的可超越幅度值</summary>
    public const float MIN_SIZE_EXCEED = 0.5f;

    /// <summary>游戏摄像机</summary>
    public Camera gameCamera;

    public Camera uiCamera;

    /// <summary>拖拽结束后是否还应该继续移动摄像机（逐渐减速到0）</summary>
    private bool shouldMoveAfterDragEnd = false;

    /// <summary>拖拽结束时的残留速度</summary>
    private Vector2 dragEndDeltaMove = new Vector2();

    /// <summary>是否正在播放摄像机视野逐渐缩小的效果</summary>
    private bool isPlayingCameraScaleIn = false;

    /// <summary>摄像机是否在移动</summary>
    private bool isCameraMoving = false;

    /// <summary>摄像机是否在缩放</summary>
    private bool isCameraScaling = false;

    /// <summary>摄像机能达到的最大orthographicSize 需计算或者直接设置值, 比如进入截图模式会比正常游戏的限制size更大</summary>
    private float maxOrthographicSizeLimit = 5;

    /// <summary>摄像机能达到最小orthographicSize的数值/summary>///
    private float minOrthographicSizeLimit = 1f;

    /// <summary>
    /// 摄像机的视野有效范围
    /// </summary>
    private Rect viewRange;

    public Rect ViewRange
    {
        get
        {
            return viewRange;
        }
    }

    /// <summary> 屏幕像素到摄像机正交大小的转换系数 </summary>
    private float PixelToOrthographicSizeRatio
    {
        get
        {
            if (gameCamera.pixelWidth <= gameCamera.pixelHeight)
            {
                return gameCamera.orthographicSize * 2 / gameCamera.pixelWidth;
            }
            else
            {
                return gameCamera.orthographicSize * 2 / gameCamera.pixelHeight;
            }
        }
    }

    private Vector2 angularVelocity = Vector2.zero;

    /// <summary>摄像机正交宽度的一半 </summary>
    private float CameraOrthoHalfWidth
    {
        get
        {
            return gameCamera.orthographicSize * Screen.width / Screen.height;
        }
    }

    /// <summary>摄像机正交高度的一半</summary>
    private float CameraOrthoHalfHeight
    {
        get
        {
            return gameCamera.orthographicSize;
        }
    }

    public static CameraGestureMgr Instance
    {
        get;
        private set;
    }

    /// <summary>摄像机放大缩小改变的速度值</summary>
    public float pinchSpeed = 0.5f;

    /// <summary>校正摄像机视野值到最大或者最小值时的回弹时间 </summary>
    public float correctOrthographicSizeTime = 0.25f;

    /// <summary>校正摄像机到达移动边界时的回弹时间 </summary>
    public float correctPosTime = 0.3f;

    public float moveLerpStrenth = 5;

    public float rotationLerpStrenth = 7.0f;

    public float dragAcceleration = 150.0f;

    public float minPitchAngle = -60.0f;

    public float maxPitchAngle = 60.0f;

    /// <summary>摄像机移动可超越幅度值 </summary>
    public Vector2 positionExceed = new Vector2(1, 1);

    /// <summary> 摄像机最大orthographicSize，可超越</summary>
    public float maxOrthographicSize = 6;

    /// <summary> 摄像机最小orthographicSize</summary>
    public float minOrthographicSize = 2f;

    /// <summary> 摄像机最大的orthographicSize，可超越</summary>
    public float maxFOVSize = 60;

    /// <summary> 摄像机最小的orthographicSize</summary>
    public float minFOVSize = 5f;

    public float shakeTime;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
        }
        gameCamera.transform.position = new Vector3(0, 0, -10);
        //if (GlobalVariable.GameState == GameState.MainSceneMode)
        //{
        //    //gameCamera.orthographicSize = PlayerProfile.LoadGameCameraOrthographicSize() / 10f;
        //}
        //else
        //{
        //    gameCamera.orthographicSize = ENTER_GAME_ORTHOGRAPHIC_SIZE;
        //}
    }

    void Start()
    {
        FingerMgr.Instance.Event_Drag += OnDrag;
        FingerMgr.Instance.Event_Pinch += OnPinch;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            shouldMoveAfterDragEnd = false;
            dragEndDeltaMove = Vector2.zero;
        }

        //if (Input.GetMouseButtonUp(0))
        //{
        //    operation = OperateType.move;
        //}

        #region 移动
        if (shouldMoveAfterDragEnd && gameCamera.orthographic)
        {
            dragEndDeltaMove = Vector2.Lerp(dragEndDeltaMove, Vector2.zero, SpringLerp(moveLerpStrenth, Time.deltaTime));

            Vector3 cameraNewPos = gameCamera.transform.position;
            cameraNewPos.x -= dragEndDeltaMove.x * PixelToOrthographicSizeRatio;
            cameraNewPos.y -= dragEndDeltaMove.y * PixelToOrthographicSizeRatio;
            MoveCameraInstant(cameraNewPos);

            if (dragEndDeltaMove.magnitude < 1)
            {
                shouldMoveAfterDragEnd = false;
                dragEndDeltaMove = Vector2.zero;
            }
        }

        if (shouldMoveAfterDragEnd && !gameCamera.orthographic)
        {
            dragEndDeltaMove = Vector2.Lerp(dragEndDeltaMove, Vector2.zero, SpringLerp(rotationLerpStrenth, Time.deltaTime));
            RotateCamera(dragEndDeltaMove);
            if (dragEndDeltaMove.magnitude < 1)
            {
                shouldMoveAfterDragEnd = false;
                dragEndDeltaMove = Vector2.zero;
            }
        }
        #endregion
    }

    #region 进行手势操作一些计算方法
    private float NormalizePitch(float angle)
    {
        if (angle > 180.0f)
            angle -= 360.0f;

        return angle;
    }

    /// <summary>是否禁用手势插件 </summary>
    private bool DisableGestureFunction(FingerMgrOperation type)
    {
        if (isPlayingCameraScaleIn ||
            isCameraMoving
            || type != FingerMgrOperation.OperationMap)
        {
            return true;
        }
        if (maxOrthographicSize < minOrthographicSize)
        {
            Debug.LogWarningFormat("Orthographic值设置错误");
            return true;
        }

        if (Event_CheckTutorailOver != null)
        {
            if (Event_CheckTutorailOver() == false)
            {
                return true;
            }
        }

        if (maxOrthographicSizeLimit < minOrthographicSizeLimit)
        {
            Debug.LogWarningFormat("Orthographic值设置错误");
            return true;
        }
        return false;
    }

    private float SpringLerp(float strength, float deltaTime)
    {
        if (deltaTime > 1f)
        {
            deltaTime = 1f;
        }
        int ms = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        float cumulative = 0f;
        for (int i = 0; i < ms; ++i)
        {
            cumulative = Mathf.Lerp(cumulative, 1f, deltaTime);
        }
        return cumulative;
    }

    Vector3 CorrectPosition(Vector3 originPosition)
    {
        //计算摄像机的位置有效范围
        //注意。则于实际设备的屏幕宽高比可能比地图更小，所以出现当高度达到极限时，宽度已经超出了要极限，因此需要兼容一下，即摄像机的视野用地图高度限制，宽度可以超过上限，此时不能左右移动了
        float cameraPosYMin = viewRange.yMin + CameraOrthoHalfHeight;
        float cameraPosYMax = viewRange.yMax - CameraOrthoHalfHeight;

        float cameraPosXMin = viewRange.xMin + CameraOrthoHalfWidth;
        float cameraPosXMax = viewRange.xMax - CameraOrthoHalfWidth;

        float yMin;
        float yMax;
        float xMin;
        float xMax;

        //最大值与最小值在极限情况下可能互换，此时要将位置固定为中间值
        if (cameraPosXMin > cameraPosXMax)
        {
            xMin = xMax = (cameraPosXMax + cameraPosXMin) / 2;
        }
        else
        {
            xMin = cameraPosXMin;
            xMax = cameraPosXMax;
        }

        //最大值与最小值在极限情况下可能互换，此时要将位置固定为中间值
        if (cameraPosYMin > cameraPosYMax)
        {
            yMin = yMax = (cameraPosYMax + cameraPosYMin) / 2;
        }
        else
        {
            yMin = cameraPosYMin;
            yMax = cameraPosYMax;
        }

        //将位置约束在有效范围内
        originPosition.x = Mathf.Clamp(originPosition.x, xMin, xMax);
        originPosition.y = Mathf.Clamp(originPosition.y, yMin, yMax);
        originPosition.z = -10;
        return originPosition;
    }

    float CorrectScale(float orthographicSize)
    {
        //先约束在弹性范围内
        orthographicSize = Mathf.Clamp(orthographicSize, minOrthographicSize - MIN_SIZE_EXCEED, maxOrthographicSize + MAX_SIZE_EXCEED);

        //再约束在绝对范围内
        orthographicSize = Mathf.Clamp(orthographicSize, minOrthographicSizeLimit, maxOrthographicSizeLimit);
        return orthographicSize;
    }
    #endregion

    #region 摄像机移动方法
    public void LookAtTarget(Vector3 targetPos, bool instant = false)
    {
        if (instant)
        {
            gameCamera.transform.rotation = Quaternion.LookRotation(targetPos - gameCamera.transform.position);
        }
        else
        {
            gameCamera.transform.DORotateQuaternion(Quaternion.LookRotation(targetPos - gameCamera.transform.position), 3);
        }
    }

    public void RotateCamera(Vector2 deltaMove)
    {
        Vector3 localAngles = gameCamera.transform.localEulerAngles;
        Vector2 idealAngularVelocity = Vector2.zero;

        idealAngularVelocity = dragAcceleration * deltaMove.Centimeters();

        angularVelocity = Vector2.Lerp(angularVelocity, idealAngularVelocity, Time.deltaTime * dragAcceleration);
        Vector2 angularMove = Time.deltaTime * angularVelocity;

        //x旋转变化量为x角度加Y方向偏移量，绕X轴选择上下移动所以是Y方向；
        localAngles.x = Mathf.Clamp(NormalizePitch(localAngles.x + angularMove.y), minPitchAngle, maxPitchAngle);

        localAngles.y = Mathf.Clamp(NormalizePitch(localAngles.y - angularMove.x), minPitchAngle, maxPitchAngle);

        gameCamera.transform.localEulerAngles = localAngles;
    }

    public void ShakeCamera(float shakeTime = 1, Vector2 shakeVector = default(Vector2), int shakeStrenth = 5, float random = 60)
    {
        gameCamera.transform.DOShakePosition(shakeTime, shakeVector, shakeStrenth, random);
    }
    
    public void ReplyPerspective(float time)
    {
        float fl = gameCamera.orthographicSize;
        DOTween.To(() => fl, x => fl = x, 6.5f, time).OnUpdate(()=>
        {
            gameCamera.orthographicSize = fl;
        });
    }

    /// <summary>摄像机的平滑移动，考虑边界限制</summary>
    public void MoveCamera(Vector3 targetPosition, float moveTime = 0.2f, Action endCallback = null, Ease easeType = Ease.OutQuad)
    {
        targetPosition.z = -10;

        //将摄像机移动目标位置约束在有效范围内
        targetPosition = CorrectPosition(targetPosition);

        if (moveTime <= 0)
        {
            MoveCameraInstant(targetPosition);
            return;
        }
        //摄像机位移的时候，要根据实际的距离来动态调整需要的时间，避免摄像机其实已经在目标位置上了，移动还需要那么长的时间
        float distance = Vector3.Distance(gameCamera.transform.position, targetPosition);
        if (distance < gameCamera.orthographicSize)
        {
            moveTime *= (distance / gameCamera.orthographicSize);
            if (moveTime < 0.3f)
            {
                moveTime = 0.3f;
            }
        }
        isCameraMoving = true;
        gameCamera.transform.DOMove(targetPosition, moveTime).OnComplete(() =>
        {
            isCameraMoving = false;
            if (endCallback != null)
            {
                endCallback();
            }
        }).SetEase(easeType);
    }

    /// <summary>立即移动摄像机，考虑边界限制</summary>
    public void MoveCameraInstant(Vector3 targetPosition)
    {
        gameCamera.transform.position = CorrectPosition(targetPosition);
    }
    
    /// <summary>摄像机的平滑缩放，考虑边界限制与大小限制</summary>
    public void ScaleCamera(float newOrthographicSize, float duration = 0.2f, Action endCallback = null)
    {
        if (isCameraScaling)
        {
            return;
        }
        float validSize = CorrectScale(newOrthographicSize);

        isCameraScaling = true;
        DOTween.To(() => gameCamera.orthographicSize, (x) => gameCamera.orthographicSize = x, validSize, duration).OnComplete(()=> {
            isCameraScaling = false;
            if (endCallback != null)
            {
                endCallback();
            }
        });
    }

    public void ScaleCameraInstant(float newOrthographicSize)
    {
        gameCamera.orthographicSize = CorrectScale(newOrthographicSize);
    }

    /// <summary>移动摄像机，使得目标点位于摄像机视野范围内</summary>
    public  void MoveCameraToIncludePosition(GameObject target, float time = 0.2f)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return;
        }
        Bounds maxBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            if (renderers[i].bounds.size.x == 0 || renderers[i].bounds.size.y == 0)
            {
                //如果没有渲染任何东西，直接跳过
                continue;
            }
            Vector2 min = maxBounds.min;
            Vector2 max = maxBounds.max;
            min.x = Mathf.Min(min.x, renderers[i].bounds.min.x);
            min.y = Mathf.Min(min.y, renderers[i].bounds.min.y);
            max.x = Mathf.Max(max.x, renderers[i].bounds.max.x);
            max.y = Mathf.Max(max.y, renderers[i].bounds.max.y);
            maxBounds.min = min;
            maxBounds.max = max;
        }

        float maxXGapBetweenCameraAndTarget = CameraOrthoHalfWidth - maxBounds.size.x / 2;
        float maxYGapBetweenCameraAndTarget = CameraOrthoHalfHeight - maxBounds.size.y / 2;
        Vector3 distance = maxBounds.center - gameCamera.transform.position;
        distance.z = 0;

        float moveXDelta = Mathf.Abs(distance.x) - maxXGapBetweenCameraAndTarget;
        float moveYDelta = Mathf.Abs(distance.y) - maxYGapBetweenCameraAndTarget;
        int xDirection = distance.x > 0 ? 1 : -1;
        int yDirection = distance.y > 0 ? 1 : -1;

        Vector3 moveDelta = Vector3.zero;
        if (moveXDelta > 0)
        {
            moveDelta.x = moveXDelta * xDirection;
        }
        if (moveYDelta > 0)
        {
            moveDelta.y = moveYDelta * yDirection;
        }
        Vector3 cameraNewPos = gameCamera.transform.position + moveDelta;
        MoveCamera(cameraNewPos, time);
    }

    public void MoveCameraToIncludePosition()
    {
        if (Event_CheckTutorailOver != null)
        {
            if (Event_CheckTutorailOver() == false)
            {
                return;
            }
        }

        Vector3 mouseWorldPosition = CameraGestureMgr.Instance.gameCamera.ScreenToWorldPoint(Input.mousePosition);
        float distanceXOfMouseAndCameraCenter = mouseWorldPosition.x - gameCamera.transform.position.x;
        float distanceYOfMouseAndCameraCenter = mouseWorldPosition.y - gameCamera.transform.position.y;

        float gapXToCameraViewBoundary = CameraOrthoHalfWidth - Mathf.Abs(distanceXOfMouseAndCameraCenter);
        float gapYToCameraViewBoundary = CameraOrthoHalfHeight - Mathf.Abs(distanceYOfMouseAndCameraCenter);

        if (CheckCanMoveCamer()||gapXToCameraViewBoundary <= 0.5f || gapYToCameraViewBoundary <= 0.5f)
        {
            if (isCameraMoving == false)
            {
                Vector3 newPosition = gameCamera.transform.position;
                newPosition.x += distanceXOfMouseAndCameraCenter * 0.5f;
                newPosition.y += distanceYOfMouseAndCameraCenter * 0.5f;

                MoveCamera(newPosition, 0.2f, null, Ease.Linear);
            }
        }
    }

    public static bool CheckCanMoveCamer()
    {
        Vector3 postion = Instance.uiCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] data = Physics2D.RaycastAll(postion, Vector3.forward, 1000, 1 << LayerMask.NameToLayer("BoundaryLimit"));
        if (data.Length > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 移动摄像机到指定坐标
    /// </summary>
    /// <param name="postion"></param>
    public void MoveCameraToIncludePosition(Vector3 postion)
    {
        float distanceXOfMouseAndCameraCenter = postion.x - gameCamera.transform.position.x;
        float distanceYOfMouseAndCameraCenter = postion.y - gameCamera.transform.position.y;

        float gapXToCameraViewBoundary = CameraOrthoHalfWidth - Mathf.Abs(distanceXOfMouseAndCameraCenter);
        float gapYToCameraViewBoundary = CameraOrthoHalfHeight - Mathf.Abs(distanceYOfMouseAndCameraCenter);

        if (gapXToCameraViewBoundary <= 0.5f || gapYToCameraViewBoundary <= 0.5f)
        {
            if (isCameraMoving == false)
            {
                Vector3 newPosition = gameCamera.transform.position;
                newPosition.x += distanceXOfMouseAndCameraCenter * 0.5f;
                newPosition.y += distanceYOfMouseAndCameraCenter * 0.5f;

                MoveCamera(newPosition, 0.2f, null, Ease.Linear);
            }
        }
    }
    #endregion

    #region 手势操作
    /// <summary>缩放手势 </summary>
    public void OnPinch(FingerMgrOperation type, PinchGesture gesture)
    {
        if (DisableGestureFunction(type))
        {
            return;
        }
        if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            if (gameCamera.orthographic)
            {
                float scale = 1;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    scale = 20;
                }
                gameCamera.orthographicSize -= gesture.Delta * PixelToOrthographicSizeRatio * pinchSpeed * scale;

                gameCamera.orthographicSize = CorrectScale(gameCamera.orthographicSize);

                //缩放操作也可能产生位移
                Vector3 cameraNewPos = gameCamera.transform.position;
                cameraNewPos.x -= gesture.DeltaMove.x * PixelToOrthographicSizeRatio;
                cameraNewPos.y -= gesture.DeltaMove.y * PixelToOrthographicSizeRatio;

                MoveCameraInstant(cameraNewPos);
            }
            else
            {
                gameCamera.fieldOfView -= gesture.Delta * PixelToOrthographicSizeRatio * pinchSpeed;
                gameCamera.fieldOfView = Mathf.Clamp(gameCamera.fieldOfView, minFOVSize, maxFOVSize);
            }
        }

        if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            if (gameCamera.orthographicSize > maxOrthographicSize)
            {
                DOTween.To(() => gameCamera.orthographicSize, x => gameCamera.orthographicSize = x, maxOrthographicSize, correctOrthographicSizeTime);
            }
            else if (gameCamera.orthographicSize < minOrthographicSize)
            {
                DOTween.To(() => gameCamera.orthographicSize, x => gameCamera.orthographicSize = x, minOrthographicSize, correctOrthographicSizeTime);
            }
            dragEndDeltaMove = gesture.DeltaMove;
        }
    }

    /// <summary>拖拽 </summary>
    public void OnDrag(FingerMgrOperation type, DragGesture gesture)
    {
        if (DisableGestureFunction(type))
        {
            return;
        }
        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
        }
        if (gesture.Phase == ContinuousGesturePhase.Updated)
        {
            if (gameCamera.orthographic)
            {
                Vector3 cameraNewPos = gameCamera.transform.position;

                cameraNewPos.x -= gesture.DeltaMove.x * PixelToOrthographicSizeRatio;
                cameraNewPos.y -= gesture.DeltaMove.y * PixelToOrthographicSizeRatio;

                MoveCameraInstant(cameraNewPos);
            }
            else
            {
                RotateCamera(gesture.DeltaMove);
            }
        }
        else if (gesture.Phase == ContinuousGesturePhase.Ended)
        {
            dragEndDeltaMove = gesture.DeltaMove;
            if (dragEndDeltaMove.sqrMagnitude > 1)
            {
                shouldMoveAfterDragEnd = true;
            }

            if (gameCamera.transform.position.y < viewRange.yMin)
            {
                shouldMoveAfterDragEnd = false;

                gameCamera.transform.DOMoveY(viewRange.yMin, correctPosTime);
            }
            if (gameCamera.transform.position.y > viewRange.yMax)
            {
                shouldMoveAfterDragEnd = false;

                gameCamera.transform.DOMoveY(viewRange.yMax, correctPosTime);
            }
            if (gameCamera.transform.position.x < viewRange.xMin)
            {
                shouldMoveAfterDragEnd = false;

                gameCamera.transform.DOMoveX(viewRange.xMin, correctPosTime);
            }
            if (gameCamera.transform.position.x > viewRange.xMax)
            {
                shouldMoveAfterDragEnd = false;

                gameCamera.transform.DOMoveX(viewRange.xMax, correctPosTime);
            }
        }
    }
    #endregion

    public void Init(float minOrthographicSize, Rect viewRange)
    {
        this.maxOrthographicSize = viewRange.height / 2;
        this.minOrthographicSize = minOrthographicSize;
        if (this.minOrthographicSize > this.maxOrthographicSize)
        {
            this.minOrthographicSize = this.maxOrthographicSize;
        }
        maxOrthographicSizeLimit = maxOrthographicSize + MAX_SIZE_EXCEED;
        minOrthographicSizeLimit = minOrthographicSize - MIN_SIZE_EXCEED;
        this.viewRange = viewRange;
    }

    public Vector3 ReturnLeftUpPos()
    {
        return new Vector3(viewRange.xMin, viewRange.yMax, 0);
    }

    public Vector3 ReturnRightDownPos()
    {
        return new Vector3(viewRange.xMax, viewRange.yMin, 0);
    }
}

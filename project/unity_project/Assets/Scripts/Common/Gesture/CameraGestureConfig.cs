using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraGestureConfig : MonoBehaviour
{
    private float maxOrthographicSize = 5;
    private float minOrthographicSize =1;

    //private bool toggleState = true;
    //private bool cameraProjection = true;
    //private string minAngle = "-45";
    //private string maxAngle = "45";

    void Start()
    {
        //SetOrthographic();
        //CameraGestureMgr.Instance.Init(maxOrthographicSize, minOrthographicSize, new Rect(-3, -5, 10, 10));
    }

    public bool DisableGesture()
    {
        return false;
    }

    //void OnGUI()
    //{
    //    return;
    //    toggleState = GUI.Toggle(new Rect(200, 0, 180, 50), toggleState, toggleState ? "Open" : "close");

    //    //cameraProjection = GUI.Toggle(new Rect(400, 0, 180, 50), cameraProjection, cameraProjection ? "2d" : "3d");
    //    Camera.main.orthographic = cameraProjection;
    //    canvas.SetActive(toggleState);

    //    if (toggleState == false)
    //    {
    //        return;
    //    }

    //    if (GUI.Button(new Rect(600, 0, 180, 50), "ResetPos"))
    //    {
    //        Camera.main.transform.position = new Vector3(0, 0, -10);
    //        Camera.main.transform.eulerAngles = Vector3.zero;
    //        CameraGestureMgr.Instance.ShakeCamera();
    //    }

    //    //if (GUI.Button(new Rect(600, 110, 180, 50), "InitWithValue"))
    //    //{
    //    //    CameraGestureMgr.Instance.Init(maxOrthographicSize, minOrthographicSize, new Rect(-3, -5, 10, 10));
    //    //}

    //    //GUI.Label(new Rect(600, 165, 180, 50), "旋转角度下限");
    //    //minAngle = GUI.TextField(new Rect(600, 190, 180, 20), minAngle, 5);
    //    //float.TryParse(minAngle, out CameraGestureMgr.Instance.minPitchAngle);
    //    ////GUI.Label(new Rect(600, 230, 180, 50), "旋转角度上限");
    //    //maxAngle = GUI.TextField(new Rect(600, 250, 180, 20), maxAngle, 5);
    //    //float.TryParse(maxAngle, out CameraGestureMgr.Instance.maxPitchAngle);

    //    GUILayout.BeginVertical(GUILayout.Height(700));
    //    {
    //        //GUILayout.Label("滑动设置orthographicSize值");
    //        //maxOrthographicSize = GUILayout.HorizontalSlider(maxOrthographicSize, 1, 10, GUILayout.Width(100));
    //        //minOrthographicSize = GUILayout.HorizontalSlider(minOrthographicSize, 1, 5, GUILayout.Width(100));
    //        //GUILayout.Label("最大值：" + maxOrthographicSize.ToString() + "最小值：" + minOrthographicSize);

    //        //GUILayout.Label("设置缩放速度, 越大缩放越快");
    //        //CameraGestureMgr.Instance.pinchSpeed = GUILayout.HorizontalSlider(CameraGestureMgr.Instance.pinchSpeed, 0, 2, GUILayout.Width(100));
    //        //GUILayout.Label(CameraGestureMgr.Instance.pinchSpeed.ToString());

    //        //GUILayout.Label("设置摄像机视野达到极限时回弹的速度， 越大回弹时间越久");
    //        //CameraGestureMgr.Instance.correctOrthographicSizeTime = GUILayout.HorizontalSlider(CameraGestureMgr.Instance.correctOrthographicSizeTime, 0, 1, GUILayout.Width(100));
    //        //GUILayout.Label(CameraGestureMgr.Instance.correctOrthographicSizeTime.ToString());

    //        //GUILayout.Label("设置摄像机到达边界回弹的速度， 越大回弹时间越久");
    //        //CameraGestureMgr.Instance.correctPosTime = GUILayout.HorizontalSlider(CameraGestureMgr.Instance.correctPosTime, 0, 1, GUILayout.Width(100));
    //        //GUILayout.Label(CameraGestureMgr.Instance.correctPosTime.ToString());

    //        //GUILayout.Label("松开手后移动的插值，越大停下的越快");
    //        //CameraGestureMgr.Instance.moveLerpStrenth = GUILayout.HorizontalSlider(CameraGestureMgr.Instance.moveLerpStrenth, 0, 50, GUILayout.Width(100));
    //        //GUILayout.Label(CameraGestureMgr.Instance.moveLerpStrenth.ToString());

    //        //GUILayout.Label("松开手后继续旋转的插值");
    //        //CameraGestureMgr.Instance.rotationLerpStrenth = GUILayout.HorizontalSlider(CameraGestureMgr.Instance.rotationLerpStrenth, 0, 50, GUILayout.Width(100));
    //        //GUILayout.Label(CameraGestureMgr.Instance.rotationLerpStrenth.ToString());
        

    //        //GUILayout.Label("旋转加速度");
    //        //CameraGestureMgr.Instance.dragAcceleration = GUILayout.HorizontalSlider(CameraGestureMgr.Instance.dragAcceleration, 0, 500, GUILayout.Width(100));
    //        //GUILayout.Label(CameraGestureMgr.Instance.dragAcceleration.ToString());

    //        //GUILayout.Label("设置可超越边界值");
    //        //float value = GUILayout.HorizontalSlider(CameraGestureMgr.Instance.positionExceed.x, 0, 2, GUILayout.Width(100));
    //        //GUILayout.Label(value.ToString());

    //        //CameraGestureMgr.Instance.positionExceed = new Vector2(value, value);
    //    }
    //}

    public Slider orSlider;
    public Slider minorSlider;
    public Slider pinchSlider;
    public Slider pinchBackSlider;
    public Slider borderSlider;
    public Slider moveSlider;

    public GameObject canvas;
    public void SetOrthographic()
    {
        Text maxText = orSlider.GetComponentInChildren<Text>();
        maxText.text = "设置摄像机视野最大值" +  orSlider.value.ToString() ;
        maxOrthographicSize = orSlider.value;
        CameraGestureMgr.Instance.maxOrthographicSize = orSlider.value;

        Text minText = minorSlider.GetComponentInChildren<Text>();
        minText.text = "设置摄像机视野最小值" + minorSlider.value.ToString();
        minOrthographicSize = minorSlider.value;
        CameraGestureMgr.Instance.minOrthographicSize = minOrthographicSize;

        Text pinchText = pinchSlider.GetComponentInChildren<Text>();
        pinchText.text = "设置缩放速度, 越大缩放越快" + pinchSlider.value.ToString();
        CameraGestureMgr.Instance.pinchSpeed = pinchSlider.value;

        Text pinchBackText = pinchBackSlider.GetComponentInChildren<Text>();
        pinchBackText.text = "视野达到极限时回弹的速度， 越大回弹时间越久" + pinchBackSlider.value.ToString();
        CameraGestureMgr.Instance.correctOrthographicSizeTime = pinchBackSlider.value;

        Text borderText = borderSlider.GetComponentInChildren<Text>();
        borderText.text = "到达边界回弹的速度， 越大回弹时间越久" + borderSlider.value.ToString();
        CameraGestureMgr.Instance.correctPosTime = borderSlider.value;

        Text moveText = moveSlider.GetComponentInChildren<Text>();
        moveText.text = "松开手后移动的插值，越大停下的越快" + moveSlider.value.ToString();
        CameraGestureMgr.Instance.moveLerpStrenth = moveSlider.value;


        CameraGestureMgr.Instance.Init(maxOrthographicSize, new Rect(-3, -5, 10, 10));
    }
}

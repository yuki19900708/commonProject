using UnityEngine;
using System;
/// <summary>
/// 单击选择
/// </summary>
public class ClickObjectMgr : MonoBehaviour {
    public static ClickObjectMgr Instance;
    public GameObject ClickEventCanvas;
    public Action<Vector2, MapObject> Event_ClickObject;
    public Action<Vector2, MapObject> Event_ClickObjectInPeriodTimeLiftUp;
    public MapObject choiceMapObject;
    public Transform worldCanvas;
    //public CompletelyDeadTip prefab;
    //public CompletelyDeadTip superDeadLandTip;
    //public ClickToCollectTip confirmClickGameObject;
    //private bool isEditorModel = false;
    private MapObject detectionMapObject;
    public void Awake()
    {
        Instance = this;
        //MapMgr.Event_MapObjectRemove += Event_MapObjectRemove;
    }

    public static void LoadSuperDeadLandTip()
    {
        //if (Instance.superDeadLandTip == null)
        //{
        //    Instance.superDeadLandTip = Instantiate(Instance.prefab) as CompletelyDeadTip;
        //    Instance.superDeadLandTip.transform.SetParent(Instance.worldCanvas);
        //    Instance.superDeadLandTip.transform.localScale = Vector3.one;
        //    Instance.SuperDeadLandInit();
        //}
    }

    private void Start()
    {
     
    }

    public void ClickObject(Vector2 worldPostion,MapObject mapObject)
    {
        SceneObjectGestureMgr.RecycleInstanceAllOutLine();
        if (mapObject != null)
        {
            detectionMapObject = mapObject;
            //detectionMapObject.Tap();
        }
        if (mapObject == null)
        {
            choiceMapObject = null;
            mapObject = null;
        }
        else if (mapObject != null && mapObject != choiceMapObject)
        {
            if (mapObject.BasicData.detachGrid)
            {
                DragOutline dragOutline = SceneObjectGestureMgr.DragOutlinePool.GetInstance();
                dragOutline.transform.SetParent(mapObject.transform, true);
                if (mapObject.shadowTransform != null)
                {
                    dragOutline.transform.localPosition = mapObject.shadowTransform.localPosition;
                }
                else
                {
                    dragOutline.transform.localPosition = Vector3.zero;
                }
                dragOutline.InitNoPostion(1);

                if (mapObject.BasicData.canMerge && mapObject.BasicData.canDrag)
                {

                    dragOutline.SetSpriteColor(DragOutline.CAN_MOVE_CAN_MERGE);
                }
                else if (mapObject.BasicData.canMerge == false && mapObject.BasicData.canDrag)
                {

                    dragOutline.SetSpriteColor(DragOutline.CAN_MOVE_NO_MERGE);
                }
                else if (mapObject.BasicData.canMerge == false && mapObject.BasicData.canDrag == false)
                {

                    dragOutline.SetSpriteColor(DragOutline.NO_MOVE_NO_MERGE);
                }
                SceneObjectGestureMgr.mapObjectOutLine.Add(dragOutline);
            }
            else
            {
                for (int i = 0; i < mapObject.StaticMapGridList.Count; i++)
                {
                    DragOutline dragOutline = SceneObjectGestureMgr.DragOutlinePool.GetInstance();
                    dragOutline.transform.position = MapMgr.Instance.GetWorldPosByPointCenter(mapObject.StaticMapGridList[i].point);
                    dragOutline.Init(mapObject.StaticMapGridList[i].Status);
                 
                    if (mapObject.BasicData.canMerge && mapObject.BasicData.canDrag)
                    {

                        dragOutline.SetSpriteColor(DragOutline.CAN_MOVE_CAN_MERGE);
                    }
                    else if (mapObject.BasicData.canMerge == false && mapObject.BasicData.canDrag)
                    {

                        dragOutline.SetSpriteColor(DragOutline.CAN_MOVE_NO_MERGE);
                    }
                    else if (mapObject.BasicData.canMerge == false && mapObject.BasicData.canDrag == false)
                    {

                        dragOutline.SetSpriteColor(DragOutline.NO_MOVE_NO_MERGE);
                    }
                    SceneObjectGestureMgr.mapObjectOutLine.Add(dragOutline);
                }
            }
            choiceMapObject = mapObject;
        }
        else if(mapObject == choiceMapObject)
        {
            choiceMapObject = null;
            mapObject = null;
        }

        if (Event_ClickObject != null)
        {
            Event_ClickObject(worldPostion, mapObject);
        }
    }

    public void HideSuperDeadLandDes()
    {
        //if (isEditorModel == false && superDeadLandTip != null)
        //{
        //    superDeadLandTip.HideTip();
        //}
    }

    public void ClickUnlockButNotCuredObject(Vector2 worldPostion)
    {
        #region 显示为解锁的地形的解锁进度
        MapGrid grid = MapMgr.Instance.GetMapGridData(worldPostion);
        if (grid == null)
        {
            return;
        }

        if (grid!=null && (grid.Status == MapGridState.UnlockButDead))
        {
            if (grid.deadLandData.ignorePure)
            {
                //superDeadLandTip.transform.position = MapMgr.Instance.GetWorldPosByPoint(grid.point) + new Vector3(0, 2,0);
                //superDeadLandTip.ShowTip();
            }
            else
            {
                PurifyProgressbar.Show(grid, grid.CurPurificationValue, grid.CurPurificationValue, grid.deadLandData.pureNeed1);
            }
        }
        #endregion
    }

    /// <summary>
    /// 按下在一定时间内抬起
    /// </summary>
    /// <param name="postion"></param>
    /// <param name="tagertGameObject"></param>
    public void ClickObjectInPeriodTimeLiftUp(Vector2 worldPostion, MapObject tagertMapObject)
    {
        if (detectionMapObject==null||detectionMapObject != tagertMapObject)
        {
            return;
        }
        //TODO 在这里获取点击的物品判断他是否可以点击产出物品 产出物品
        if (Event_ClickObjectInPeriodTimeLiftUp != null)
        {
            Event_ClickObjectInPeriodTimeLiftUp(worldPostion, tagertMapObject);
        }
        detectionMapObject = tagertMapObject;
        if (detectionMapObject != null)
        {
            //TapEventData tapEventData = TableDataMgr.GetSingleTapEventData(detectionMapObject.Id);
            //if (GlobalVariable.GameState == GameState.MainSceneMode &&
            //    tapEventData!=null && GlobalVariable.isConfirmClick && tapEventData.showTip)
            //{
                //if (confirmClickGameObject == null)
                //{
                //    confirmClickGameObject = MapMgr.clickToCollectTipPool.GetInstance();
                //}
                //confirmClickGameObject.gameObject.SetActive(true);
            //    //confirmClickGameObject.ShowClickConfirmButton(detectionMapObject.tipMountPosition.position, L10NMgr.GetText(27800007), detectionMapObject,CallBack);
            //}
            //else
            //{
            //    //if (confirmClickGameObject != null)
            //    //{
            //    //    confirmClickGameObject.gameObject.SetActive(false);
            //    //}
            //    detectionMapObject.ClickInPeriodTimeLiftUp();
            //}
        }
    }

    //private void CallBack(MapObject obj)
    //{
    //    obj.ClickInPeriodTimeLiftUp();
    //}

    private void Event_MapObjectRemove(MapObject obj)
    {
        if (detectionMapObject != null)
        {
            if (obj == detectionMapObject)
            {
                SceneObjectGestureMgr.RecycleInstanceAllOutLine();
            }
        }

    }

    private void SuperDeadLandInit()
    {
        //if (EditorTerrainModel.TerrainEditorModel.IsRunMapEditor == false &&
        //    superDeadLandTip != null)
        //{
        //    isEditorModel = false;
        //    //superDeadLandTip.HideTip();
        //}
        //else
        //{
        //    isEditorModel = true;
        //}
    }

}

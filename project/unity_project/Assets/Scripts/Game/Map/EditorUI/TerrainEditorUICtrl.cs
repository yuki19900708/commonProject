using EditorTerrainModel;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Universal.TileMapping;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using System.Text;
using GameProto;
using Google.Protobuf;

public class MapDataCache
{
    public int width;
    public int height;
    public List<MapGridGameData> playerDataList;
    public MapGridGameData[,] mapDataArray;
}

public class TerrainEditorUICtrl : MonoBehaviour
{
    public static TerrainEditorUICtrl Instance;
    public static string PATH;
    public static string Suffix = ".xlsx";

    public UGUISpriteAtlas[] terrainAtlas;
    public Dropdown editorTypeSelectDropDown;
    public InputField contentInputField;
    public Button loadButton;
    public Button exitButton;

    public TerrainEditorContentEditorInterface editInterface;


//    public TileMap terrainMap;
//    public ScriptableTile terrainTile;
//    public TileGameObjectRenderer terrainRenderer;

    public TileMap vegetationMap;
    public ScriptableTile vegetationTile;
    public TileGameObjectRenderer vegetationRenderer;
   
    public Toggle isEditorToggle;
    public UGUISpriteAtlas[] atlas;

    public MapObjectData currentSelectObjectItemData;
    public Text tipText;
    public Button previewButton;

    public VegetationData currentSelectVegetationItemData;

    public Button settingButton;
    public GameObject settingObject;
    public InputField sizeXInputFiled;
    public InputField sizeYInputFiled;
    public Button settingExitButton;
    public Button settingOkButton;

//    public GameObject showSettingObject;
//    public Toggle showTerrainToggle;
//    public Toggle showVegetionToggle;

    public Material spriteHSVMat;

    #region 界面新增提示
    public Text mousePosText;
    public Text terrainText;
    public Text vegetationText;
    public Text entityText;
    #endregion
    private TerrainEditorVegetation currentSelectVegetationItem;
    private TerrainEditorSelectItem currentSelectObjectItem;
    private TileMap currentSelectTileMap;
    private ScriptableTile currentScriptableTile;
    private EditorType editorType = EditorType.Level;
    private EditoryLayoutType layoutType = EditoryLayoutType.Vegetation;
    private BrushStyle brushStyle = BrushStyle.None;
    private bool isBoxUp = false;
    private List<Point> region = new List<Point>();
    private List<MapGridGameData> mapDataList = new List<MapGridGameData>();
    private bool isShowTip = false;
    private int mapWidth = 100;
	private int mapHeight = 100;
#if UNITY_EDITOR
    private string textPath = "";
    private string checktextPath = "";
#endif
    private Point lastGridPoint;
    private Point startPoint;
    private bool isChangePoint = false;
    private Point lineGridPoint;
    private bool clickIsRmove = true;
    private Dictionary<TileMap, bool> tileMapActiveSelfDict = new Dictionary<TileMap, bool>();
    private bool isBrush = false;

    public static bool IsEditor
    {
        get { return Instance.isEditorToggle.isOn; }
    }

    private void Awake()
    {
        TerrainEditorModel.IsRunMapEditor = true;


        PATH = Application.dataPath + "../../../../design_asset/datatable/";
        Instance = this;
        CameraGestureMgr.Instance.Init(5, new Rect(-60, -10, 120, 70));
        TableDataEventMgr.BindAllEvent();

        foreach (UGUISpriteAtlas atl in terrainAtlas)
        {
            atl.Init();
        }

        editInterface.Event_BrushStyleChange += DropDownSelectChange;
        editInterface.Event_SelectObjectItem += SelectObjectItem;
        editInterface.Event_SaveEditor += SaveEdiotr;

        editInterface.Event_SelectVegetationItem += EventSelectVegetationItem;

        editorTypeSelectDropDown.onValueChanged.AddListener(EditorSelectDropDownValueChange);
        editorTypeSelectDropDown.value = 0;
        editorTypeSelectDropDown.options = TerrainEditorModel.GetTerrainEditorDropDownOptionData(TerrainEditorModel.loadPrefabNames);

        isEditorToggle.onValueChanged.AddListener(IsEditorToggleValueChange);
        isEditorToggle.isOn = false;

        loadButton.onClick.AddListener(OnLoadButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        previewButton.onClick.AddListener(OnPreviewButtonClick);

        settingButton.onClick.AddListener(OnSettingButtonClick);
        settingExitButton.onClick.AddListener(OnSettingExitButtonClick);
        settingOkButton.onClick.AddListener(OnSettingOKButtonClick);

//        showTerrainToggle.onValueChanged.AddListener(ShowTerrainToggleValuechange);
//        showVegetionToggle.onValueChanged.AddListener(ShowVegetionToggleValuechange);

//        terrainRenderer.OnRenderTile += OnTerrainRederTile;
        vegetationRenderer.OnRenderTile += OnVegetationRederTile;

        foreach (UGUISpriteAtlas a in atlas)
        {
            a.Init();
        }
        ExitEditor();
        ShowTipText("来测试一下");

    }

    private void SetAdjacentMat(Point point)
    {
        if (isBrush == false) return;
        //if (layoutType != EditoryLayoutType.Purification)
        {
            List<Point> pointList = new List<Point>();
            pointList.Add(point);
            pointList.Add(point.Up);
            pointList.Add(point.Down);
            pointList.Add(point.Left);
            pointList.Add(point.Right);
            pointList.Add(point.UpLeft);
            pointList.Add(point.UpRight);
            pointList.Add(point.DownLeft);
            pointList.Add(point.DownRight);
            for (int i = 0; i < pointList.Count; i++)
            {
                if (vegetationMap.IsInBounds(pointList[i]))
                {
                    int index = pointList[i].y + pointList[i].x * mapHeight;

                    if (vegetationRenderer.GetTileGameObject(pointList[i].x, pointList[i].y) != null)
                    {
                        //    GameObject gameObject = objectRenderer.GetTileGameObject(pointList[i].x, pointList[i].y);
                        MapObject mapObject;
                        //    if (gameObject != null)
                        //    {
                        //        mapObject = gameObject.GetComponent<MapObject>();
                        //        mapObject.DisplayAsUnLockAndCured();
                        //    }

                        //    gameObject = terrainRenderer.GetTileGameObject(pointList[i].x, pointList[i].y);
                        //    if (gameObject != null)
                        //    {
                        //        mapObject = gameObject.GetComponent<MapObject>();
                        //        mapObject.DisplayAsUnLockAndCured();
                        //    }

                        MapGridGameData data = mapDataList[index];
                        if (data.hasVegetation > 0)
                        {
                            mapObject = vegetationRenderer.GetTileGameObject(pointList[i].x, pointList[i].y).GetComponent<MapObject>();
                            mapObject.DisplayAsUnLockAndCured();
                        }
                    }
                }
            }
        }
    }

    private void OnVegetationRederTile(int x, int y, GameObject go)
    {
        if (go == null)
        {
            return;
        }
        MapObject mapObject = go.GetComponent<MapObject>();
        mapObject.IsVegetation = true;
        int index = y + x * mapHeight;
        MapGridGameData data = mapDataList[index];
        mapObject.VegetationId = data.hasVegetation;
        Renderer[] rendererList = go.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in rendererList)
        {
            renderer.sharedMaterial = spriteHSVMat;
        }
    }

    private void Update()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (brushStyle == BrushStyle.BoxUp)
        {
            isChangePoint = false;
            return;
        }

        Point tmpGridPoint = vegetationMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (lineGridPoint != tmpGridPoint)
        {
			MapUtil.Instance.tran.position = vegetationMap.Coordinate2WorldPosition(tmpGridPoint);
			if (vegetationMap.IsInBounds(tmpGridPoint) && mapDataList.Count > 0)
            {
                MapGridGameData data = mapDataList[tmpGridPoint.y + tmpGridPoint.x * mapHeight];
                mousePosText.text = string.Format("坐标: {0},{1}", tmpGridPoint.x, tmpGridPoint.y);
                if (data.hasTerrain != 0)
                {
                    terrainText.text = string.Format("地形: {0}", data.hasTerrain);
                }
                else
                {
                    terrainText.text = string.Format("地形: {0}", "没有地形");
                }

                if (data.hasVegetation != 0)
                {
                    GameObject go = vegetationRenderer.GetTileGameObject(data.x, data.y);
                    if (go != null)
                    {
                        MapObject obj = go.GetComponent<MapObject>();
                        vegetationText.text = string.Format("草地: {0} 色:{1}饱:{2}亮:{3}", data.hasVegetation,
                            obj.mpb.GetFloat("_Hue"), obj.mpb.GetFloat("_Saturation"), obj.mpb.GetFloat("_Value"));
                    }
                    //ShowVegetationMatInfo(vegetationRenderer.GetTileGameObject(tmpGridPoint.x, tmpGridPoint.y));
                }
                else
                {
                    vegetationText.text = string.Format("草地: {0}", "没有草地");
                }

                if (data.entityId != 0)
                {
                    MapObjectData entityData = TableDataMgr.GetSingleMapObjectData(data.entityId);
                    entityText.text = string.Format("物体: {0}->{1}->{2}", data.entityId, entityData.standbyName, entityData.currentName);
                    if (entityData == null)
                    {
                        Debug.LogError("这里有一个致命错误----格子上的物品不存在表里");
                    }
                }
                else
                {
                    entityText.text = string.Format("物体: {0}", "没有物体");
                }

            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                isChangePoint = true;
                lineGridPoint = tmpGridPoint;
                if (Input.GetMouseButton(0))
                {
                    clickIsRmove = false;
                }
                else if (Input.GetMouseButton(1))
                {
                    clickIsRmove = true;
                }
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                if (isChangePoint && IsEditor)
                {
                    if (currentSelectTileMap && brushStyle == BrushStyle.None)
                    {
                        region = GetRegion(lineGridPoint);
                        isBrush = true;
                        BrushTile();
                        isBrush = false;
                    }
                }
            }
        }
    }


    private void OnSettingOKButtonClick()
    {
        if (sizeXInputFiled.text != null && sizeYInputFiled.text != null)
        {
            int x = 0;
            int y = 0;
            bool result = int.TryParse(sizeXInputFiled.text, out x);
            if (result == false)
            {
                ShowTipText("请输入正确的尺寸！！！");
                return;
            }
            result = int.TryParse(sizeYInputFiled.text, out y);
            if (result == false)
            {
                ShowTipText("请输入正确的尺寸！！！");
                return;
            }
            if (x <= 2 || y <= 2)
            {
                ShowTipText("请输入正确的尺寸！！！");
                return;
            }
            mapWidth = x;
            mapHeight = y;

            Dictionary<Point, MapGridGameData> dict = new Dictionary<Point, MapGridGameData>();

            for (int i = 0; i < mapDataList.Count; i++)
            {
                MapGridGameData da = new MapGridGameData();
                da.x = mapDataList[i].x;
                da.y = mapDataList[i].y;
                da.entityId = mapDataList[i].entityId;
                da.hasTerrain = mapDataList[i].hasTerrain;
                da.hasVegetation = mapDataList[i].hasVegetation;
                da.purificationLevel = mapDataList[i].purificationLevel;
                da.sealLockId = mapDataList[i].sealLockId;
                da.vegetationHue = mapDataList[i].vegetationHue;

                dict.Add(new Point(da.x, da.y), da);
            }

            mapDataList.Clear();
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    MapGridGameData data = new MapGridGameData();
                    data.x = i;
                    data.y = j;
                    Point p = new Point(i, j);
                    if (dict.ContainsKey(p))
                    {
                        data.entityId = dict[p].entityId;
                        data.hasTerrain = dict[p].hasTerrain;
                        data.hasVegetation = dict[p].hasVegetation;
                        data.purificationLevel = dict[p].purificationLevel;
                        data.sealLockId = dict[p].sealLockId;
                        data.vegetationHue = dict[p].vegetationHue;
                    }
                    mapDataList.Add(data);
                }
            }

//            terrainMap.ResizeMap(x, y);
            vegetationMap.ResizeMap(x, y);
            //objectMap.ResizeMap(x, y);
            MapUtil.Instance.DrawMapGridLine(x, y);
        }
        settingObject.gameObject.SetActive(false);
        Timer.AddDelayFunc(0.5f, () =>
        {
            LoadMapObject();
        });
    }

    private void OnSettingExitButtonClick()
    {
        settingObject.gameObject.SetActive(false);
    }

    private void OnSettingButtonClick()
    {
        //if (isCanSetting == false)
        //{
        //    ShowTipText("已经设置过这个关卡的size 或已经开始编辑 无法再继续设置");
        //    return;
        //}
        settingObject.gameObject.SetActive(true);
    }

    private void OnPreviewButtonClick()
    {
        editInterface.gameObject.SetActive(!editInterface.gameObject.activeSelf);
        exitButton.gameObject.SetActive(!exitButton.gameObject.activeSelf);
        isEditorToggle.gameObject.SetActive(!isEditorToggle.gameObject.activeSelf);
        settingButton.gameObject.SetActive(!settingButton.gameObject.activeSelf);
//        showSettingObject.gameObject.SetActive(!showSettingObject.gameObject.activeSelf);

        isEditorToggle.isOn = false;
        if (editInterface.gameObject.activeSelf)
        {
            MapUtil.Instance.EnableDraw();

//           terrainMap.gameObject.SetActive(tileMapActiveSelfDict[terrainMap]);
            vegetationMap.gameObject.SetActive(tileMapActiveSelfDict[vegetationMap]);
            //objectMap.gameObject.SetActive(tileMapActiveSelfDict[objectMap]);
        }
        else
        {
            tileMapActiveSelfDict.Clear();
//            tileMapActiveSelfDict.Add(terrainMap, terrainMap.gameObject.activeSelf);
            tileMapActiveSelfDict.Add(vegetationMap, vegetationMap.gameObject.activeSelf);
            //tileMapActiveSelfDict.Add(objectMap, objectMap.gameObject.activeSelf);

//            terrainMap.gameObject.SetActive(true);
            vegetationMap.gameObject.SetActive(true);
            //objectMap.gameObject.SetActive(true);

            MapUtil.Instance.DisableDraw();
        }
    }

    public List<Point> GetRegion(Point point)
    {
        region = new List<Point>();
        int correctedRadius = 0;
        for (int x = -correctedRadius; x <= correctedRadius; x++)
        {
            for (int y = -correctedRadius; y <= correctedRadius; y++)
            {
                Point offsetPoint = point + new Point(x, y);
                region.Add(offsetPoint);
            }
        }
        return region;
    }

    public List<Point> GetRegion(Point point, Point endPoint)
    {
        region = new List<Point>();
        if (endPoint == startPoint)
            return GetRegion(point);

        int x0 = Mathf.Min(startPoint.x, endPoint.x),
            x1 = Mathf.Max(startPoint.x, endPoint.x),
            y0 = Mathf.Min(startPoint.y, endPoint.y),
            y1 = Mathf.Max(startPoint.y, endPoint.y);

        for (int x = x0; x <= x1; x++)
        {
            for (int y = y0; y <= y1; y++)
            {
                region.Add(new Point(x, y));
            }
        }
        MapUtil.Instance.DrawListLineGrid(x0, x1 + 1, y0, y1 + 1);
        return region;
    }

    private void OnLoadButtonClick()
    {
        switch (editorType)
        {
            case EditorType.Level:
                Debug.Log(string.Format("加载第{0}关地形", int.Parse(contentInputField.text)));
                editInterface.InitData(string.Format("第{0}关", contentInputField.text));
                break;
        }
        LoadEditor();
    }

    private void OnExitButtonClick()
    {
//        terrainMap.gameObject.SetActive(true);
        vegetationMap.gameObject.SetActive(true);

//        terrainMap.CompleteReset();
        vegetationMap.CompleteReset();
        editorTypeSelectDropDown.value = 0;

//        terrainMap.gameObject.SetActive(false);
        vegetationMap.gameObject.SetActive(false);

        ExitEditor();
    }

    public void DragStart()
    {
        Debug.Log("编辑开始");
        isChangePoint = false;
        startPoint = new Point(-1, -1);
        lastGridPoint = new Point(-1, -2);
        lineGridPoint = startPoint;

        if (brushStyle == BrushStyle.BoxUp)
        {
			startPoint = vegetationMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            isBoxUp = Input.GetMouseButton(1) ? true : false;
        }
    }

    public void Drag()
    {
        Point curPoint = currentSelectTileMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (lineGridPoint != curPoint)
        {
            lineGridPoint = curPoint;
			MapUtil.Instance.tran.position = vegetationMap.Coordinate2WorldPosition(curPoint);

            if (currentSelectTileMap && brushStyle == BrushStyle.None)
            {
                Point point = currentSelectTileMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (lastGridPoint == point) return;
                lastGridPoint = point;
                region = GetRegion(point);
                isBrush = true;
                BrushTile();
                isBrush = false;
            }

            if (currentSelectTileMap && brushStyle == BrushStyle.BoxUp)
            {
                Point point = currentSelectTileMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                GetRegion(point, point);
            }
        }
    }

    public void DragEnd()
    {
        Debug.Log("编辑结束");
        if (currentSelectTileMap && brushStyle == BrushStyle.BoxUp)
        {
            Point point = currentSelectTileMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (lastGridPoint == point) return;
            lastGridPoint = point;
            region = GetRegion(point, point);
            isBrush = true;
            BrushTile();
            isBrush = false;
            isChangePoint = false;
            MapUtil.Instance.ClearLineList();
        }
    }

    public void BrushTile()
    {
        ScriptableTile tmpTile = currentScriptableTile;

        if (isBoxUp || Input.GetMouseButton(1) || isChangePoint && clickIsRmove)
        {
            tmpTile = null;
        }

        for (int i = 0; i < region.Count; i++)
        {
            Point offsetPoint = region[i];
			if (vegetationMap.IsInBounds(offsetPoint) == false)
            {
                continue;
            }
         
        
          
            int index = offsetPoint.y + offsetPoint.x * mapHeight;
            switch (layoutType)
            {
                case EditoryLayoutType.Vegetation:
                    if (tmpTile == null)
                    {
                        //objectMap.gameObject.SetActive(true);

                        //objectMap.SetTileAndUpdateNeighbours(offsetPoint.x, offsetPoint.y, null);

                        //objectMap.gameObject.SetActive(true);

                        mapDataList[index].hasTerrain = 0;
                        mapDataList[index].hasVegetation = 0;
                        mapDataList[index].entityId = 0;
                        mapDataList[index].purificationLevel = 0;
                        mapDataList[index].sealLockId = 0;
                    }
                    else
                    {
                        mapDataList[index].hasTerrain = 1;
                        mapDataList[index].hasVegetation = currentSelectVegetationItemData.id;
                        mapDataList[index].vegetationHue = currentSelectVegetationItemData.hueValue;
                    }
                    break;
            }
            currentSelectTileMap.SetTileAndUpdateNeighbours(offsetPoint, tmpTile);


            SetAdjacentMat(offsetPoint);
        }
        isChangePoint = false;
        lineGridPoint = new Point(-1, -1);
    }

    private void EventSelectVegetationItem(TerrainEditorVegetation item)
    {
        if (currentSelectVegetationItem != null)
        {
            currentSelectVegetationItem.IsSelect = false;
            currentSelectVegetationItem = item;
            currentSelectVegetationItemData = null;
        }
        currentSelectVegetationItemData = item.Data;
        currentSelectVegetationItem = item;
        currentSelectVegetationItem.IsSelect = true;
        if (item != null)
        {
            //if (item.Data.type == 1)
            {
                currentScriptableTile = vegetationTile;
            }
            //else
            //{
            //    Debug.Log("LY", "并没有这方面的资源，暂时无法选择这个地形");
            //    currentScriptableTile = sandyLandTile;
            //}
        }
        else
        {
            currentScriptableTile = null;
        }
    }

    private void SelectObjectItem(DropDownSelectType state, TerrainEditorSelectItem item)
    {
        if (currentSelectObjectItem != null)
        {
            currentSelectObjectItem.IsSelect = false;
            currentSelectObjectItem = null;
            currentSelectObjectItemData = null;
        }
        currentSelectObjectItemData = item.Data;
        currentSelectObjectItem = item;
        currentSelectObjectItem.IsSelect = true;
        if (item != null)
        {
            //SimpleTile simpleTile = ResMgr.Load<SimpleTile>("Tile" + item.Data.id.ToString());
            //objectTile = simpleTile;
            //currentScriptableTile = objectTile;
            MapUtil.Instance.DrawGridHightLight(currentSelectObjectItem.Data.area);
        }
        else
        {
            //Debug.Log("LY", "没有物件 请排查数据表!!!");
            currentScriptableTile = null;
        }
    }

    private void DropDownSelectChange(DropDownSelectType type, string name)
    {
        switch (type)
        {
            case DropDownSelectType.BrushStyle:
                if (name == "正常")
                {
                    brushStyle = BrushStyle.None;
                }
                else
                {
                    brushStyle = BrushStyle.BoxUp;
                }
                break;
            case DropDownSelectType.Layout:
                MapUtil.Instance.DrawGridHightLight(new int[] { 1, 1 });
                switch (name)
                {
                    case "草皮":
                        layoutType = EditoryLayoutType.Vegetation;
                        currentSelectTileMap = vegetationMap;
                        currentScriptableTile = vegetationTile;
//                        terrainMap.gameObject.SetActive(true);
//                        showTerrainToggle.isOn = true;
                        vegetationMap.gameObject.SetActive(true);
//                        showVegetionToggle.isOn = true;
                        break;
                }
                break;
        }
    }

    private void EditorSelectDropDownValueChange(int index)
    {
        ExitEditor();
        editorType = (EditorType)index;
        switch (editorType)
        {
            case EditorType.Level:
                contentInputField.gameObject.SetActive(true);
                loadButton.gameObject.SetActive(true);
                (contentInputField.placeholder as Text).text = "输入关卡数";
                break;
        }
    }

    private void IsEditorToggleValueChange(bool arg0)
    {
        if (arg0)
        {
            FingerMgr.Instance.fingerMgrOperation = FingerMgrOperation.OperationObject;
            if (layoutType == EditoryLayoutType.Vegetation)
            {
                editInterface.EnableVegetationSelect();
            }
        }
        else
        {
            FingerMgr.Instance.fingerMgrOperation = FingerMgrOperation.None;
          	if (layoutType == EditoryLayoutType.Vegetation)
            {
                editInterface.DisableVegetationSelect();
            }
        }
    }

    private void ExitEditor()
    {
        isEditorToggle.isOn = false;
        editorTypeSelectDropDown.gameObject.SetActive(true);
        contentInputField.gameObject.SetActive(true);
        loadButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(false);
        isEditorToggle.gameObject.SetActive(false);
        editInterface.gameObject.SetActive(false);
        previewButton.gameObject.SetActive(false);
        settingButton.gameObject.SetActive(false);
//        showSettingObject.gameObject.SetActive(false);

//        terrainMap.gameObject.SetActive(true);
        vegetationMap.gameObject.SetActive(false);
    }

    private void LoadEditor()
    {
        editorTypeSelectDropDown.gameObject.SetActive(false);
        contentInputField.gameObject.SetActive(true);
        loadButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(true);
        isEditorToggle.gameObject.SetActive(true);
        editInterface.gameObject.SetActive(true);
        previewButton.gameObject.SetActive(true);
        settingButton.gameObject.SetActive(true);
//        showSettingObject.gameObject.SetActive(true);

//        terrainMap.gameObject.SetActive(true);
        vegetationMap.gameObject.SetActive(true);
        brushStyle = BrushStyle.None;

        Debug.Log("读取地图数据");
        LoadData();
    }

    private void LoadData()
    {
        isBrush = false;
        string currentPath = "";
        mapDataList.Clear();
        TextAsset ta = null;
        switch (editorType)
        {
            case EditorType.Level:
                currentPath = "MapData/LevelData/" + contentInputField.text;
                ta = Resources.Load(currentPath) as TextAsset;
                break;
        }

        if (ta == null)
        {
            //isCanSetting = true;
            //Debug.Log("LY", "不存在這個关卡");
            mapDataList.Clear();

            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    MapGridGameData data = new MapGridGameData();
                    data.x = i;
                    data.y = j;
                    mapDataList.Add(data);
                }
            }
        }
        else
        {
            //isCanSetting = false;
            MapDataCache saveEditor = SimpleJson.SimpleJson.DeserializeObject<MapDataCache>(ta.text);
            mapDataList = saveEditor.playerDataList;

            mapWidth = saveEditor.width;
            mapHeight = saveEditor.height;
        }

//        terrainMap.ResizeMap(mapWidth, mapHeight);
        vegetationMap.ResizeMap(mapWidth, mapHeight);
        MapUtil.Instance.DrawMapGridLine(mapWidth, mapHeight);

        Timer.AddDelayFunc(0.5f, () =>
         {
             LoadMapObject();
         });

    }

    private void LoadMapObject()
    {
        foreach (MapGridGameData info in mapDataList)
        {
            if (info.hasVegetation == 1)
            {
                Debug.LogError(string.Format("有草皮的值不太对x:{0} y:{1} = {2}", info.x, info.y, info.hasVegetation));
            }
			Debug.LogError (info.hasVegetation);
            if (info.hasVegetation != 0)
            {
                vegetationMap.SetTileAndUpdateNeighbours(info.x, info.y, vegetationTile);
            }
        }
    }

    private void ShowTipText(string txt)
    {
        isShowTip = true;
        tipText.text = txt;
        tipText.DOKill();
        tipText.color = Color.white;
        Timer.AddDelayFunc(0.5f, () =>
         {
             tipText.DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(delegate
                {
                    isShowTip = false;
                    tipText.text = "";
                });
         });
    }

    public bool CheckCanResetSize()
    {
        isBrush = false;
        string currentPath = "";
        mapDataList.Clear();
        TextAsset ta = null;
        switch (editorType)
        {
            case EditorType.Level:
                currentPath = "MapData/LevelData/" + contentInputField.text;
                ta = Resources.Load(currentPath) as TextAsset;
                break;
        }

        if (ta == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void SaveEdiotr()
    {
#if UNITY_EDITOR
        string currentPath = "";
        Debug.Log("将地形保存为文件！！！");
        switch (editorType)
        {
            case EditorType.Level:
                currentPath = PATH + contentInputField.text + Suffix;
                textPath = Application.dataPath + "/Resources/MapData/LevelData/" + contentInputField.text + ".txt";
                checktextPath = Application.dataPath + "/ResourcesRaw/Check/" + contentInputField.text + ".txt";
                CreatExcel(textPath, checktextPath, currentPath, mapDataList);
                break;
        }
#endif
    }

    private void CreatExcel(string textPath1, string checkTextPath1, string outputFilePath, List<MapGridGameData> mapList, int w = -1, int h = -1)
    {
#if UNITY_EDITOR
        MapDataCache saveEditor = new MapDataCache();
        saveEditor.width = mapWidth;
        saveEditor.height = mapHeight;
        if (w > 0)
        {
            saveEditor.width = w;
            saveEditor.height = h;
        }
        FileInfo newFile = new FileInfo(outputFilePath);

        if (newFile.Exists)
        {
            newFile.Delete();
            newFile = new FileInfo(outputFilePath);
            Debug.Log("创建文件 = " + outputFilePath);
        }
        saveEditor.playerDataList = mapList;

        Debug.Log("查看格子列表的总长度" + saveEditor.playerDataList.Count);

        //生成服务端数据（Protobuf二进制）
        Camp_data camp_data = new Camp_data();

        StringBuilder sb = new StringBuilder();
        foreach (MapGridGameData data in saveEditor.playerDataList)
        {
            Grid_data grid_data = new Grid_data();

            if (data.hasTerrain != 0 && data.hasVegetation == 0)
            {
                data.hasTerrain = 2;
            }

            string s = string.Format("x:{0} y:{1} hasTerrain:{2} hasVegetation:{3} vegetationHue:{4} entityId:{5} purificationLevel:{6} sealLockId{7},",
                data.x, data.y, data.hasTerrain, data.hasVegetation, data.vegetationHue, data.entityId, data.purificationLevel, data.sealLockId);
            sb.AppendLine(s);

            grid_data.Coord = new Coord();
            grid_data.Coord.X = (uint)data.x;
            grid_data.Coord.Y = (uint)data.y;
            grid_data.SeallockId = (uint)data.sealLockId;
            grid_data.DeadLevel = (uint)data.purificationLevel;
            if (data.sealLockId > 0)
            {
                grid_data.State = Grid_state.Locked;
            }
            else
            {
                if (data.purificationLevel > 0)
                {
                    grid_data.State = Grid_state.UnlockAndDead;
                }
                else
                {
                    grid_data.State = Grid_state.UnlockAndCured;
                }
            }

            if (data.hasTerrain > 0)
            {
                grid_data.Terrain = new Map_object_data();
                grid_data.Terrain.Id = 1;
                //此处为桥
                if (data.hasVegetation == 0)
                {
                    grid_data.Terrain.Id = 2;
                }
            }
            else
            {
                grid_data.State = Grid_state.Locked;
            }

            if (data.entityId > 0)
            {
                grid_data.Entity = new Map_object_data();
                grid_data.Entity.Id = (uint)data.entityId;

                Resting_building_data buildingData = new Resting_building_data();
                buildingData.Timestamp = -1;
                grid_data.Entity.BuildingData = buildingData;

                MapObjectData mapObjectData = TableDataMgr.GetSingleMapObjectData(data.entityId);

                if (mapObjectData.initLock)
                {
                    grid_data.Entity.Status = MAP_OBJECT_STATUS.Lock;
                }

                if (mapObjectData.destructType > 0)
                {
                    DestructEventData destructEventData = TableDataMgr.GetSingleDestructEventData(data.entityId);
                    grid_data.Entity.Hp = (uint)destructEventData.hp;
                }
                if (mapObjectData.canClick && data.entityId != 70022)
                {
                    TapEventData tapEventData = TableDataMgr.GetSingleTapEventData(data.entityId);
                    int max = UnityEngine.Random.Range(tapEventData.clickTimes[0], tapEventData.clickTimes[1]);
                    grid_data.Entity.LeftTapCount = max;
                    grid_data.Entity.TapMaxMount = max;
                }
                if (mapObjectData.canHarvest)
                {
                    HarvestEventData harvestEventData = TableDataMgr.GetSingleHarvestEventData(data.entityId);
                    grid_data.Entity.LeftCollectCount = harvestEventData.count;
                    grid_data.Entity.CollectMaxCount = harvestEventData.count;
                }
                if (mapObjectData.canSpawn)
                {
                    SpawnEventData spawnEventData = TableDataMgr.GetSingleSpawnEventData(data.entityId);
                    grid_data.Entity.LeftSpawnCount = spawnEventData.cooldown;
                    grid_data.Entity.SpawnMaxCount = spawnEventData.cooldown;
                }
            }
            camp_data.Grid.Add(grid_data);
        }
        Map_size size = new Map_size();
        size.Width = (uint)vegetationMap.MapWidth;
		size.Height = (uint)vegetationMap.MapHeight;
        camp_data.MapSize = size;

        byte[] protobufData = camp_data.ToByteArray();

        if (textPath1.Contains("BaseCamp"))
        {
            File.WriteAllBytes(textPath1 + "b", protobufData);
        }

        File.WriteAllText(textPath1, SimpleJson.SimpleJson.SerializeObject(saveEditor));
        File.WriteAllText(checkTextPath1, sb.ToString());
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public static Sprite GetSprite(string spriteName, Sprite sp)
    {
        foreach (UGUISpriteAtlas atl in Instance.terrainAtlas)
        {
            if (atl.spriteDict.ContainsKey(spriteName))
            {
                return atl.spriteDict[spriteName];
            }
        }
        return sp;
    }


    public Sprite GetSpriteByName(string name)
    {
        foreach (UGUISpriteAtlas atl in atlas)
        {
            if (atl.spriteDict.ContainsKey(name))
            {
                return atl.spriteDict[name];
            }
        }
        return null;
    }
}
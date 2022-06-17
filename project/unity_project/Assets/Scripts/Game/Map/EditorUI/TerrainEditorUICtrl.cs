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
using UnityEngine.SceneManagement;


public class TerrainEditorUICtrl : MonoBehaviour
{
    public static TerrainEditorUICtrl Instance;

    public Button exitButton;

    public TerrainEditorContentEditorInterface editInterface;

    public TileMap drawerMap;
    public TileGameObjectRenderer drawerRenderer;

    public TileMap editMap;
    public TileGameObjectRenderer editRender;
    #region 鼠标指向格子信息
    public Text mousePosText;
    public Text vegetationText;
    #endregion

    public InputField gridIndexInput;
    public Button searchBtn;
    public Button tipPanel;
    public Button helpBtn;


    public GameObject ensurePanel;
    public Button exitBtn;
    public Button cancelBtn;

    public Button publishBtn;

    public GameObject savePanel;

    public Camera uiCamera;
    private TerrainEditorVegetation currentSelectVegetationItem;
    public TerrainEditorVegetation CurrentSelectVegetationItem
    {
        get
        {
            return currentSelectVegetationItem;
        }
    }
    private ScriptableTile currentScriptableTile;
    private EditoryLayoutType layoutType = EditoryLayoutType.Vegetation;
    private BrushStyle brushStyle = BrushStyle.None;
    
    private List<Point> region = new List<Point>();
    private int mapWidth = 100;
	private int mapHeight = 100;
	private int mapPhase = 1;

    private Point lastGridPoint;
    private Point startPoint;
    private Point lineGridPoint;

    private bool clickIsRmove = true;

    private static bool isEditor = false;

    public static bool IsEditor
    {
        get { return isEditor; }
    }

    private MapDataCache mapData;

    private bool isInit = false;

    public bool IsInit
    {
        get
        {
            return isInit;
        }
    }

    private int[] gridArray = new int[] { 1, 1 };
    void Awake()
	{
		TerrainEditorModel.IsRunMapEditor = true;

        Instance = this;
	}
			
	public void Init()
    {
        isInit = false;
        mapData = new MapDataCache();
        isEditor = false;
        drawerRenderer.SetChangeColor(true);
        editRender.SetChangeColor(false);
        CameraGestureMgr.Instance.Init(5, new Rect(-60, -60, 120, 120));
        Camera.main.fieldOfView = 35;
        TableDataEventMgr.BindAllEvent();

        editInterface.Event_BrushStyleChange += DropDownSelectChange;
        editInterface.Event_SaveEditor += () =>
        {
            savePanel.SetActive(true);
            SaveEdiotr();
        };
        editInterface.Event_SelectVegetationItem += EventSelectVegetationItem;
        tipPanel.gameObject.SetActive(false);
        ensurePanel.gameObject.SetActive(false);
        savePanel.gameObject.SetActive(false);
        exitButton.onClick.AddListener(() =>
        {
            OnExitButtonClick();
        });
        helpBtn.onClick.AddListener(() =>
        {
            tipPanel.gameObject.SetActive(true);
        });
        tipPanel.onClick.AddListener(() =>
        {
            tipPanel.gameObject.SetActive(false);
        });

        publishBtn.onClick.AddListener(() =>
        {
            ensurePanel.SetActive(true);
        });
        exitBtn.onClick.AddListener(() =>
        {
            SavePublishData();
            ensurePanel.SetActive(false);
        });

        cancelBtn.onClick.AddListener(() =>
        {
            ensurePanel.SetActive(false);
        });

        searchBtn.onClick.AddListener(() =>
        {
            string str = gridIndexInput.text;
            if(string.IsNullOrEmpty(str) == false)
            {
                int index = int.Parse(str);
                if(index>=1 && index <=10000)
                {
                    index = index - 1;
                    Vector3 point = drawerMap.Coordinate2WorldPosition(new Point(index % drawerMap.MapWidth, index / drawerMap.MapWidth));
                    CameraGestureMgr.Instance.MoveCameraInstant(point);
                }
            }
        });
        LoadEditor ();
	}

    private void Update()
    {
        if(tipPanel.gameObject.activeSelf || ensurePanel.activeSelf)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            layoutType = EditoryLayoutType.Vegetation;
            isEditor = !isEditor;
            IsEditorToggleValueChange(isEditor);
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnPreviewButtonClick();
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            OnPreviewButtonClick();
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
    

        Point tmpGridPoint = drawerMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));

		MapUtil.Instance.tran.position = drawerMap.Coordinate2WorldPosition(tmpGridPoint);
		if (drawerMap.IsInBounds(tmpGridPoint))
        {
            mousePosText.text = string.Format("坐标: {0},{1}\n格子索引{2}", tmpGridPoint.x, tmpGridPoint.y, tmpGridPoint.x + tmpGridPoint.y * drawerMap.MapHeight);
            MapObject obj = drawerRenderer.GetMapObject(tmpGridPoint.x, tmpGridPoint.y);
            if (obj != null)
            {
                vegetationText.text = string.Format("地块id: {0}", obj.VegetationId);
                vegetationText.text += string.Format("\n所属格子索引{0}\n起始格子坐标x:{1},y:{2}:\n{3}", obj.gridIndex, obj.xPos, obj.yPos, mapData.playerDataList[tmpGridPoint.x + tmpGridPoint.y * drawerMap.MapHeight].state.ToString());
            }
            else
            {
                vegetationText.text = "空地块";
            }
        }
    }

  
    private void OnPreviewButtonClick()
    {
        editInterface.gameObject.SetActive(!editInterface.gameObject.activeSelf);
        exitButton.gameObject.SetActive(!exitButton.gameObject.activeSelf);
        searchBtn.gameObject.SetActive(!searchBtn.gameObject.activeSelf);
        gridIndexInput.gameObject.SetActive(!gridIndexInput.gameObject.activeSelf);
        helpBtn.gameObject.SetActive(!helpBtn.gameObject.activeSelf);
        publishBtn.gameObject.SetActive(!publishBtn.gameObject.activeSelf);

        if (editInterface.gameObject.activeSelf == false)
        {
            IsEditorToggleValueChange(false);
        }
        else
        {
            IsEditorToggleValueChange(isEditor);
        }

        if (editInterface.gameObject.activeSelf)
        {
            MapUtil.Instance.EnableDraw();
        }
        else
        {
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
		
    private void OnExitButtonClick()
    {
        SceneManager.LoadScene(0);
    }

    public void DragStart()
    {
        Debug.Log("编辑开始");

        startPoint = new Point(-1, -1);
        lastGridPoint = new Point(-1, -2);
        lineGridPoint = startPoint;
        clickIsRmove = Input.GetMouseButton(1) ? true : false;

        MapUtil.Instance.DrawGridHightLight(new int[] { 1, 1 });



        if (brushStyle == BrushStyle.BoxUp)
        {
			startPoint = drawerMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else
        {
            if (currentSelectVegetationItem != null && currentSelectVegetationItem.Data != null)
            {
                if (clickIsRmove == false)
                {
                    MapUtil.Instance.DrawGridHightLight(currentSelectVegetationItem.Data.area);
                }
            }
        }
    }

    public void Drag()
    {
        Point curPoint = drawerMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (lineGridPoint != curPoint)
        {
            lineGridPoint = curPoint;
			MapUtil.Instance.tran.position = drawerMap.Coordinate2WorldPosition(curPoint);

            if ( brushStyle == BrushStyle.None)
            {
                Point point = drawerMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (lastGridPoint == point) return;
                lastGridPoint = point;
                region = GetRegion(point);
                BrushTile();
            }
            else if (brushStyle == BrushStyle.BoxUp)
            {
                Point point = drawerMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                GetRegion(point, point);
            }
        }
    }

    public void DragEnd()
    {
        Debug.Log("编辑结束");
        if (brushStyle == BrushStyle.BoxUp)
        {
            Point point = drawerMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (lastGridPoint == point) return;
            lastGridPoint = point;
            region = GetRegion(point, point);
            BrushTile();
            MapUtil.Instance.ClearLineList();

        }

        if(isEditor)
        {
            SaveEdiotr();
        }
    }

    public void BrushTile()
    {
        if(DisableDraw())
        {
            return;
        }
        if (clickIsRmove == false)
        {
            if (currentSelectVegetationItem == null || currentSelectVegetationItem.Data == null)
            {
                return;
            }
        }
       
        ScriptableTile tmpTile = currentScriptableTile;

        if (clickIsRmove)
        {
            tmpTile = null;
        }

        for (int i = 0; i < region.Count; i++)
        {
            Point offsetPoint = region[i];
			if (drawerMap.IsInBounds(offsetPoint) == false)
            {
                continue;
            }

            bool canBrush = true;
            if (tmpTile != null)
            {
                int[] area = gridArray;
                if(clickIsRmove == false && currentSelectVegetationItem != null)
                {
                    area = currentSelectVegetationItem.Data.area;
                }
                for (int x = 0; x < area[0]; x++)
                {
                    for (int j = 0; j < area[1]; j++)
                    {
                        int index = (offsetPoint.x + x) + (offsetPoint.y + j) * mapWidth;

                        if (index < 0 || index > mapData.playerDataList.Count)
                        {
                            canBrush = false;
                            continue;
                        }


                        if (drawerMap.IsInBounds(offsetPoint.x + x, offsetPoint.y + j) == false)
                        {
                            canBrush = false;
                            continue;
                        }


                        if (TerrainEditorModel.MapDrawer())
                        {
                            if (drawerRenderer.GetMapObject(offsetPoint.x + x, offsetPoint.y + j) != null)
                            {
                                canBrush = false;
                            }
                        }
                        else
                        {
                            if (clickIsRmove)
                            {
                                if (mapData.playerDataList[index].state != TerrainState.Opened)
                                {
                                    canBrush = false;
                                }
                            }
                            else
                            {
                                if (mapData.playerDataList[index].state != TerrainState.Locked)
                                {
                                    canBrush = false;
                                }


                                MapObject o = editRender.GetMapObject(offsetPoint.x + x, offsetPoint.y + j);
                                if (o != null)
                                {
                                    if(o.gridIndex  != index)
                                    {
                                        canBrush = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(!canBrush)
            {
                continue;
            }
            
            if(TerrainEditorModel.MapDrawer())
            {
                drawerMap.SetTileAndUpdateNeighbours(offsetPoint, tmpTile);
            }
            else
            {
                editMap.SetTileAndUpdateNeighbours(offsetPoint, tmpTile);
            }
        }
        lineGridPoint = new Point(-1, -1);
    }

    public void ChangeMapData(int id, int index, bool isAdd)
    {
        if(isEditor == false)
        {
            return;
        }
        if(TerrainEditorModel.MapDrawer())
        {
            if(isAdd)
            {
                mapData.playerDataList[index].state = TerrainState.Locked;
            }
            else
            {
                mapData.playerDataList[index].state = TerrainState.Blank;
            }
            mapData.playerDataList[index].entityId = id;

        }
        else
        {
            if (isAdd)
            {
                if(clickIsRmove)
                {
                    mapData.playerDataList[index].state = TerrainState.Locked;
                }
                else
                {
                    mapData.playerDataList[index].state = TerrainState.Opened;
                }
                mapData.playerDataList[index].entityId = id;
            }
            else
            {
                mapData.playerDataList[index].state = TerrainState.Locked;
            }
        }
    }

    public TerrainState GetTerrainState(int index)
    {
        return mapData.playerDataList[index].state;
    }


    private void EventSelectVegetationItem(TerrainEditorVegetation item)
    {
        if (currentSelectVegetationItem != null)
        {
            currentSelectVegetationItem.IsSelect = false;
            currentSelectVegetationItem = item;
        }
        currentSelectVegetationItem = item;
        currentSelectVegetationItem.IsSelect = true;

        if (item != null)
        {
            currentScriptableTile = GetTile(item.Data.id);
            if (brushStyle == BrushStyle.BoxUp)
            {
                MapUtil.Instance.DrawGridHightLight(new int[] { 1, 1 });
            }
            else
            {
                if(currentSelectVegetationItem.Data != null)
                MapUtil.Instance.DrawGridHightLight(currentSelectVegetationItem.Data.area);
            }
        }
    }
	
    private SimpleTile GetTile(int id)
    {
        return Resources.Load<SimpleTile>("Prefabs/" + id);
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
        }
    }
		
    private void IsEditorToggleValueChange(bool arg0)
    {
		layoutType = EditoryLayoutType.Vegetation;
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

    private void LoadEditor()
    {
		TerrainEditorModel.LoadMapSize (ref mapWidth, ref mapHeight, ref mapPhase);

        brushStyle = BrushStyle.None;

        Debug.Log("读取地图数据");
        LoadData();

    }

    private void LoadData()
    {
        string path = mapPhase.ToString();
      
        drawerMap.ResizeMap(mapWidth, mapHeight);
        editMap.ResizeMap(mapWidth, mapHeight);
        MapUtil.Instance.DrawMapGridLine(mapWidth, mapHeight);
        uiCamera.orthographicSize = mapHeight * 0.5f;
        mapData.playerDataList = new List<MapGridGameData>();

        for(int i = 0; i < mapHeight;i ++)
        {
            for(int j = 0; j< mapWidth;j++)
            {
                int index = j + i * mapWidth;
                MapGridGameData data = new MapGridGameData();
                data.x = j;
                data.y = i;
                data.gridIndex = index;
                data.state = TerrainState.Blank;
                data.entityId = 0;
                mapData.playerDataList.Add(data);
            }
        }
     
        StartCoroutine(DataDownloader.LoadData(path, (ta) => {


            if (string.IsNullOrEmpty(ta) == false)
            {
                MapDataCache saveEditor = SimpleJson.SimpleJson.DeserializeObject<MapDataCache>(ta);
                List<MapGridGameData> savedData = saveEditor.playerDataList;
                mapData = saveEditor;
                for (int j = 0; j < mapData.playerDataList.Count; j++)
                {
                    for (int i = 0; i < savedData.Count; i++)
                    {
                        if (savedData[i].gridIndex == mapData.playerDataList[j].gridIndex)
                        {
                            mapData.playerDataList[j] = savedData[i];
                        }
                    }

                }
            }

            if(mapData.playerDataList != null)
            {
                foreach (MapGridGameData info in mapData.playerDataList)
                {
                    if (info.entityId > 0)
                    {
                        VegetationData data = TableDataMgr.GetSingleVegetationData(info.entityId);
                        int[] area = new int[2] { 1, 1 };

                        if (data != null)
                        {
                            area = data.area;
                        }
                      
                        SimpleTile simpleTile = GetTile(1001);
                        for (int i = 0; i < area[0]; i++)
                        {
                            for (int j = 0; j < area[1]; j++)
                            {
                                drawerMap.SetTileAt(info.x + i, info.y + j, simpleTile, false);
                            }
                        }
                    }
                }

                foreach (MapGridGameData info in mapData.playerDataList)
                {
                    if (info.entityId > 0 && info.state  == TerrainState.Opened)
                    {
                        SimpleTile simpleTile = GetTile(info.entityId);
                        editMap.SetTileAt(info.x, info.y, simpleTile, false);
                    }
                }
            }
            isInit = true;

            string str = "";
            if (TerrainEditorModel.MapDrawer())
            {
                if (mapData.state != EditState.Drawer)
                {
                    str = "--该地图产品已发布，美术无法进行编辑";
                }
            }
            else
            {
                if (mapData.state == EditState.Drawer)
                {
                    str = "--该地图美术正在编辑，尚未发布，产品无法进行编辑";
                }
            }
            editInterface.InitData(string.Format("第{0}期地图", mapPhase) + str);
            exitButton.gameObject.SetActive(true);
            drawerMap.gameObject.SetActive(true);

            if (TerrainEditorModel.MapDrawer() == false)
            {
                editMap.gameObject.SetActive(true);
            }
        }));


    }

    private void SaveEdiotr(bool returnEnterScene = false)
    {
        string savePath = mapPhase.ToString();

        string result = SimpleJson.SimpleJson.SerializeObject(mapData);
        StartCoroutine(DataDownloader.SaveData(savePath, result, ()=> {
            savePanel.SetActive(false);
            if(returnEnterScene)
            {
                SceneManager.LoadScene(0);
            }
        }));

    }

    void SavePublishData()
    {
        string savePath;
        mapData.state = EditState.Publish;

        savePath = mapPhase.ToString();

        string result = SimpleJson.SimpleJson.SerializeObject(mapData);

        StartCoroutine(DataDownloader.SaveData(savePath, result, () => {
            ensurePanel.SetActive(false);
        }, false));
    }

    bool DisableDraw()
    {
        if(IsEditor == false)
        {
            return true;
        }

        if(TerrainEditorModel.MapDrawer())
        {
            if(mapData.state != EditState.Drawer)
            {
                return true;
            }
        }
        else
        {
            if(mapData.state == EditState.Drawer)
            {
                return true;
            }
        }
        return false;
    }
}
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

    public TileMap vegetationMap;
    public TileGameObjectRenderer vegetationRenderer;
   
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
            savePublishData();
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
                    Vector3 point = vegetationMap.Coordinate2WorldPosition(new Point(index % vegetationMap.MapWidth, index / vegetationMap.MapWidth));
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
    

        Point tmpGridPoint = vegetationMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));

		MapUtil.Instance.tran.position = vegetationMap.Coordinate2WorldPosition(tmpGridPoint);
		if (vegetationMap.IsInBounds(tmpGridPoint))
        {
            mousePosText.text = string.Format("坐标: {0},{1}\n格子索引{2}", tmpGridPoint.x, tmpGridPoint.y, tmpGridPoint.x + tmpGridPoint.y * vegetationMap.MapHeight);
            MapObject obj = vegetationRenderer.GetMapObject(tmpGridPoint.x, tmpGridPoint.y);
            if (obj != null)
            {
                vegetationText.text = string.Format("地块id: {0}", obj.VegetationId);
                vegetationText.text += string.Format("\n所属格子索引{0}\n起始格子坐标x:{1},y:{2}:", obj.gridIndex, obj.xPos, obj.yPos);
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
			startPoint = vegetationMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
        Point curPoint = vegetationMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (lineGridPoint != curPoint)
        {
            lineGridPoint = curPoint;
			MapUtil.Instance.tran.position = vegetationMap.Coordinate2WorldPosition(curPoint);

            if ( brushStyle == BrushStyle.None)
            {
                Point point = vegetationMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (lastGridPoint == point) return;
                lastGridPoint = point;
                region = GetRegion(point);
                BrushTile();
            }
            else if (brushStyle == BrushStyle.BoxUp)
            {
                Point point = vegetationMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                GetRegion(point, point);
            }
        }
    }

    public void DragEnd()
    {
        Debug.Log("编辑结束");
        if (brushStyle == BrushStyle.BoxUp)
        {
            Point point = vegetationMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
			if (vegetationMap.IsInBounds(offsetPoint) == false)
            {
                continue;
            }

            bool canBrush = true;
            if (tmpTile != null)
            {
                int[] area = currentSelectVegetationItem.Data.area;
                for (int x = 0; x < area[0]; x++)
                {
                    for (int j = 0; j < area[1]; j++)
                    {
                        int index = (offsetPoint.x + x) + (offsetPoint.y + j) * mapWidth;

                        if (TerrainEditorModel.MapDrawer())
                        {
                            if (vegetationRenderer.GetMapObject(offsetPoint.x + x, offsetPoint.y + j) != null)
                            {
                                canBrush = false;
                            }
                        }
                        else
                        {
                            if (index >= 0 && index < mapData.playerDataList.Count)
                            {
                                if (mapData.playerDataList[index].state != TerrainState.Locked)
                                {
                                    canBrush = false;
                                }
                            }
                            else
                            {
                                canBrush = false;
                            }
                                
                        }
                        
                        if(vegetationMap.IsInBounds(offsetPoint.x + x, offsetPoint.y + j) == false)
                        {
                            canBrush = false;
                        }
                        else
                        {
                            if (TerrainEditorModel.MapDrawer() == false)
                            {
                                if (mapData.playerDataList[index].state != TerrainState.Locked)
                                {
                                    canBrush = false;
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
            
            vegetationMap.SetTileAndUpdateNeighbours(offsetPoint, tmpTile);
        }
        lineGridPoint = new Point(-1, -1);
    }

    public TerrainState ChangeMapData(int id, int index, bool isAdd)
    {
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
        }
        else
        {
            if (isAdd)
            {
                mapData.playerDataList[index].state = TerrainState.Opened;
            }
            else
            {
                mapData.playerDataList[index].state = TerrainState.Locked;
            }
        }
        mapData.playerDataList[index].entityId = id;
        return mapData.playerDataList[index].state;
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

		editInterface.InitData(string.Format("第{0}期地图", mapPhase));
        exitButton.gameObject.SetActive(true);
        editInterface.gameObject.SetActive(true);
        vegetationMap.gameObject.SetActive(true);
        brushStyle = BrushStyle.None;

        Debug.Log("读取地图数据");
        LoadData();
    }

    private void LoadData()
    {
        string path = mapPhase.ToString();
      
        vegetationMap.ResizeMap(mapWidth, mapHeight);
        MapUtil.Instance.DrawMapGridLine(mapWidth, mapHeight);

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
                        SimpleTile simpleTile = GetTile(info.entityId);
                        vegetationMap.SetTileAt(info.x, info.y, simpleTile, false);
                    }
                }
            }
            isInit = true;
        }));


    }

    private void SaveEdiotr(bool returnEnterScene = false)
    {
        string savePath = mapPhase.ToString();
        mapData.state = EditState.Drawer;


        string result = SimpleJson.SimpleJson.SerializeObject(mapData);
        StartCoroutine(DataDownloader.SaveData(savePath, result, ()=> {
            savePanel.SetActive(false);
            if(returnEnterScene)
            {
                SceneManager.LoadScene(0);
            }
        }));

    }

    void savePublishData()
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

        return false;
    }
}
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
public class MapDataCache
{
    public int width;
    public int height;
    public List<MapGridGameData> playerDataList;
}

public class TerrainEditorUICtrl : MonoBehaviour
{
    public static TerrainEditorUICtrl Instance;

    public Button exitButton;

    public TerrainEditorContentEditorInterface editInterface;

    public TileMap vegetationMap;
    public TileGameObjectRenderer vegetationRenderer;
   
    public Toggle isEditorToggle;

    public Text tipText;
    public Button previewButton;

	[SerializeField]
	private VegetationData currentSelectVegetationItemData = null;

	public VegetationData CurrentSelectVegetationItemData
	{
		get {
			return currentSelectVegetationItemData;
		}
	}

    #region 界面新增提示
    public Text mousePosText;
    public Text vegetationText;   
    #endregion
    private TerrainEditorVegetation currentSelectVegetationItem;
    private ScriptableTile currentScriptableTile;
    private EditorType editorType = EditorType.Level;
    private EditoryLayoutType layoutType = EditoryLayoutType.Vegetation;
    private BrushStyle brushStyle = BrushStyle.None;
    
    private List<Point> region = new List<Point>();
    private List<MapGridGameData> mapDataList = new List<MapGridGameData>();
    private int mapWidth = 100;
	private int mapHeight = 100;
	private int mapPhase = 1;
#if UNITY_EDITOR
    private string textPath = "";
#endif
    private Point lastGridPoint;
    private Point startPoint;
    private Point lineGridPoint;

    private bool clickIsRmove = true;

    public static bool IsEditor
    {
        get { return Instance.isEditorToggle.isOn; }
    }

	void Awake()
	{
		TerrainEditorModel.IsRunMapEditor = true;

        Instance = this;
	}
			
	public void Init()
    {        
        CameraGestureMgr.Instance.Init(5, new Rect(-5000, -5000, 10000, 10000));
        TableDataEventMgr.BindAllEvent();
        Camera.main.fieldOfView = 35;
        editInterface.Event_BrushStyleChange += DropDownSelectChange;
        editInterface.Event_SaveEditor += SaveEdiotr;

        editInterface.Event_SelectVegetationItem += EventSelectVegetationItem;

        isEditorToggle.onValueChanged.AddListener(IsEditorToggleValueChange);
        isEditorToggle.isOn = false;

        exitButton.onClick.AddListener(OnExitButtonClick);
        previewButton.onClick.AddListener(OnPreviewButtonClick);

        ExitEditor();
        ShowTipText("来测试一下");
		LoadEditor ();
	}

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
     
        if (brushStyle == BrushStyle.BoxUp)
        {
            return;
        }

        Point tmpGridPoint = vegetationMap.WorldPosition2Coordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition));

		MapUtil.Instance.tran.position = vegetationMap.Coordinate2WorldPosition(tmpGridPoint);
		if (vegetationMap.IsInBounds(tmpGridPoint) && mapDataList.Count > 0)
        {
            MapGridGameData data = mapDataList[tmpGridPoint.y + tmpGridPoint.x * mapHeight];
            mousePosText.text = string.Format("坐标: {0},{1}\n格子索引{2}", tmpGridPoint.x, tmpGridPoint.y, tmpGridPoint.x + tmpGridPoint.y * vegetationMap.MapHeight);
            MapObject obj = vegetationRenderer.GetMapObject(data.x, data.y);
            if (obj != null)
            {
                vegetationText.text = string.Format("地块id: {0}", obj.VegetationId);
                vegetationText.text += string.Format("\n所属格子索引{0}\n起始格子坐标x:{1},y:{2}:", obj.gridIndex, obj.gridIndex % vegetationMap.MapWidth, obj.gridIndex / vegetationMap.MapWidth);
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
        isEditorToggle.gameObject.SetActive(!isEditorToggle.gameObject.activeSelf);

        isEditorToggle.isOn = false;
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
        vegetationMap.gameObject.SetActive(true);

        vegetationMap.CompleteReset();

        ExitEditor();
		SceneManager.LoadScene (0);
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
            if (currentSelectVegetationItemData != null)
            {
                if (clickIsRmove == false)
                {
                    MapUtil.Instance.DrawGridHightLight(currentSelectVegetationItemData.area);
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
    }

    public void BrushTile()
    {
        if (clickIsRmove == false)
        {
            if (CurrentSelectVegetationItemData == null)
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
                int[] area = currentSelectVegetationItemData.area;
                for (int x = 0; x < area[0]; x++)
                {
                    for (int j = 0; j < area[1]; j++)
                    {
                        if (vegetationRenderer.GetMapObject(offsetPoint.x + x, offsetPoint.y + j) != null)
                        {
                            canBrush = false;
                        }
                        else if(vegetationMap.IsInBounds(offsetPoint.x + x, offsetPoint.y + j) == false)
                        {
                            canBrush = false;
                        }
                    }
                }
            }

            if(!canBrush)
            {
                continue;
            }

            int index = offsetPoint.x + offsetPoint.y * mapHeight;

            switch (layoutType)
            {
                case EditoryLayoutType.Vegetation:
                    if (tmpTile == null)
                    {
                        mapDataList[index].entityId = 0;
                    }
                    else
                    {
                        mapDataList[index].entityId = currentSelectVegetationItemData.id;
                    }
                    break;
            }
            vegetationMap.SetTileAndUpdateNeighbours(offsetPoint, tmpTile);
        }
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
            currentScriptableTile = GetTile(item.Data.id);
            if (brushStyle == BrushStyle.BoxUp)
            {
                MapUtil.Instance.DrawGridHightLight(new int[] { 1, 1 });
            }
            else
            {
                MapUtil.Instance.DrawGridHightLight(currentSelectVegetationItemData.area);
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

    private void ExitEditor()
    {
        isEditorToggle.isOn = false;
        exitButton.gameObject.SetActive(false);
        isEditorToggle.gameObject.SetActive(false);
        editInterface.gameObject.SetActive(false);
        previewButton.gameObject.SetActive(false);

        vegetationMap.gameObject.SetActive(false);
		currentSelectVegetationItemData = null;
    }

    private void LoadEditor()
    {
		TerrainEditorModel.LoadMapSize (ref mapWidth, ref mapHeight, ref mapPhase);

		editInterface.InitData(string.Format("第{0}期地图", mapPhase));
        exitButton.gameObject.SetActive(true);
        isEditorToggle.gameObject.SetActive(true);
        editInterface.gameObject.SetActive(true);
        previewButton.gameObject.SetActive(true);
        vegetationMap.gameObject.SetActive(true);
        brushStyle = BrushStyle.None;

        Debug.Log("读取地图数据");
        LoadData();
    }

    private void LoadData()
    {
        string currentPath = "";
        mapDataList.Clear();
        TextAsset ta = null;
        switch (editorType)
        {
            case EditorType.Level:
			currentPath = "MapData/LevelData/" + mapPhase;
                ta = Resources.Load(currentPath) as TextAsset;
                break;
        }

        mapDataList.Clear();
        vegetationMap.ResizeMap(mapWidth, mapHeight);

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                MapGridGameData data = new MapGridGameData();
                data.x = j;
                data.y = i;
                data.gridIndex = j + i * mapHeight;
                mapDataList.Add(data);
            }
        }
        if (ta != null)
        {
            MapDataCache saveEditor = SimpleJson.SimpleJson.DeserializeObject<MapDataCache>(ta.text);
            List<MapGridGameData> savedData = saveEditor.playerDataList;
            
            for (int j = 0; j < mapDataList.Count; j++)
            {
                for (int i = 0; i < savedData.Count; i++)
                {
                    if (savedData[i].x == mapDataList[j].x && savedData[i].y == mapDataList[j].y)
                    {
                        mapDataList[j] = savedData[i];
                    }
                }
            
            }
        }

        MapUtil.Instance.DrawMapGridLine(mapWidth, mapHeight);

        LoadMapObject();
    }

    private void LoadMapObject()
    {
        foreach (MapGridGameData info in mapDataList)
        {
            if (info.entityId > 0)
            {
                SimpleTile simpleTile = GetTile(info.entityId);
                vegetationMap.SetTileAt(info.x, info.y, simpleTile, false);
            }
        }
    }

    private void ShowTipText(string txt)
    {       
		tipText.text = txt;
        tipText.DOKill();
        tipText.color = Color.white;
        tipText.DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(delegate
        {
            tipText.text = "";
        }).SetDelay(0.5f);
    }
		
    private void SaveEdiotr()
    {
#if UNITY_EDITOR
        Debug.Log("将地形保存为文件！！！");
        switch (editorType)
        {
            case EditorType.Level:
			textPath = Application.dataPath + "/Resources/MapData/LevelData/" + mapPhase + ".txt";
                CreatExcel(textPath, mapDataList);
                break;
        }
#endif
    }

    private void CreatExcel(string textPath1, List<MapGridGameData> mapList, int w = -1, int h = -1)
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
   
        saveEditor.playerDataList = mapList;

        File.WriteAllText(textPath1, SimpleJson.SimpleJson.SerializeObject(saveEditor));
        UnityEditor.AssetDatabase.Refresh();
#endif
    }


}
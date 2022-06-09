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
using UnityEngine.SceneManagement;
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

    public Material spriteHSVMat;

    #region 界面新增提示
    public Text mousePosText;
    public Text terrainText;
    public Text vegetationText;
    public Text entityText;
    #endregion
    private TerrainEditorVegetation currentSelectVegetationItem;
    private TileMap currentSelectTileMap;
    private ScriptableTile currentScriptableTile;
    private EditorType editorType = EditorType.Level;
    private EditoryLayoutType layoutType = EditoryLayoutType.Vegetation;
    private BrushStyle brushStyle = BrushStyle.None;
    private bool isBoxUp = false;
    private List<Point> region = new List<Point>();
    private List<MapGridGameData> mapDataList = new List<MapGridGameData>();
    private int mapWidth = 100;
	private int mapHeight = 100;
	private int mapPhase = 1;
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

	void Awake()
	{
		TerrainEditorModel.IsRunMapEditor = true;

		PATH = Application.dataPath + "../../../../design_asset/datatable/";
		Instance = this;
	}
			
	public void Init()
    {
        
        CameraGestureMgr.Instance.Init(5, new Rect(-60, -10, 120, 70));
        TableDataEventMgr.BindAllEvent();

        foreach (UGUISpriteAtlas atl in terrainAtlas)
        {
            atl.Init();
        }

        editInterface.Event_BrushStyleChange += DropDownSelectChange;
        editInterface.Event_SaveEditor += SaveEdiotr;

        editInterface.Event_SelectVegetationItem += EventSelectVegetationItem;

        isEditorToggle.onValueChanged.AddListener(IsEditorToggleValueChange);
        isEditorToggle.isOn = false;

        exitButton.onClick.AddListener(OnExitButtonClick);
        previewButton.onClick.AddListener(OnPreviewButtonClick);

        vegetationRenderer.OnRenderTile += OnVegetationRederTile;

        foreach (UGUISpriteAtlas a in atlas)
        {
            a.Init();
        }
        ExitEditor();
        ShowTipText("来测试一下");

		LoadEditor ();
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

//        if (lineGridPoint != tmpGridPoint)
//        {
			MapUtil.Instance.tran.position = vegetationMap.Coordinate2WorldPosition(tmpGridPoint);
			if (vegetationMap.IsInBounds(tmpGridPoint) && mapDataList.Count > 0)
            {
                MapGridGameData data = mapDataList[tmpGridPoint.y + tmpGridPoint.x * mapHeight];
              

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

              
//            }	

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

  
    private void OnPreviewButtonClick()
    {
        editInterface.gameObject.SetActive(!editInterface.gameObject.activeSelf);
        exitButton.gameObject.SetActive(!exitButton.gameObject.activeSelf);
        isEditorToggle.gameObject.SetActive(!isEditorToggle.gameObject.activeSelf);

        isEditorToggle.isOn = false;
        if (editInterface.gameObject.activeSelf)
        {
            MapUtil.Instance.EnableDraw();
            vegetationMap.gameObject.SetActive(tileMapActiveSelfDict[vegetationMap]);
        }
        else
        {
            tileMapActiveSelfDict.Clear();
            tileMapActiveSelfDict.Add(vegetationMap, vegetationMap.gameObject.activeSelf);
            vegetationMap.gameObject.SetActive(true);
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

        vegetationMap.gameObject.SetActive(false);

        ExitEditor();
		SceneManager.LoadScene (0);
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

		if (currentSelectTileMap == null || CurrentSelectVegetationItemData == null) {
			return;
		}
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
                        mapDataList[index].hasVegetation = 1001;
                    }
                    else
                    {
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
            currentScriptableTile = vegetationTile;
        }
        else
        {
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

				currentSelectTileMap = vegetationMap;
				currentScriptableTile = vegetationTile;
                editInterface.EnableVegetationSelect();
            }
        }
        else
        {
            FingerMgr.Instance.fingerMgrOperation = FingerMgrOperation.None;
          	if (layoutType == EditoryLayoutType.Vegetation)
            {
				currentSelectTileMap = null;
				currentScriptableTile = null;
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
        isBrush = false;
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

        if (ta == null)
        {
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
            MapDataCache saveEditor = SimpleJson.SimpleJson.DeserializeObject<MapDataCache>(ta.text);
            mapDataList = saveEditor.playerDataList;

            mapWidth = saveEditor.width;
            mapHeight = saveEditor.height;
        }

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

			if (info.hasVegetation != 0)
            {
                vegetationMap.SetTileAndUpdateNeighbours(info.x, info.y, vegetationTile);
            }
        }
//		vegetationMap.SetTileAndUpdateNeighbours(0, 0, vegetationTile);
		Timer.AddDelayFunc(1, () =>
			{
				foreach (MapGridGameData info in mapDataList)
				{
					MapObject mapObject;

					if (info.hasVegetation > 0)
					{
						mapObject = vegetationRenderer.GetTileGameObject(info.x, info.y).GetComponent<MapObject>();
						mapObject.DisplayAsUnLockAndCured();

					}

				}
			});
    }

    private void ShowTipText(string txt)
    {       
		tipText.text = txt;
        tipText.DOKill();
        tipText.color = Color.white;
        Timer.AddDelayFunc(0.5f, () =>
         {
             tipText.DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(delegate
                {
                    tipText.text = "";
                });
         });
    }
		
    private void SaveEdiotr()
    {
#if UNITY_EDITOR
        string currentPath = "";
        Debug.Log("将地形保存为文件！！！");
        switch (editorType)
        {
            case EditorType.Level:
			currentPath = PATH + mapPhase + Suffix;
			textPath = Application.dataPath + "/Resources/MapData/LevelData/" + mapPhase + ".txt";
			checktextPath = Application.dataPath + "/ResourcesRaw/Check/" + mapPhase + ".txt";
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

//            if (data.hasTerrain != 0 && data.hasVegetation == 0)
//            {
//                data.hasTerrain = 2;
//            }
//
            string s = string.Format("x:{0} y:{1} hasVegetation:{2} vegetationHue:{3},",
               						data.x, data.y, data.hasVegetation, data.vegetationHue);
            sb.AppendLine(s);

            grid_data.Coord = new Coord();
            grid_data.Coord.X = (uint)data.x;
            grid_data.Coord.Y = (uint)data.y;
        

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
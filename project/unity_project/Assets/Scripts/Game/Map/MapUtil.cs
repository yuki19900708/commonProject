using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using EditorTerrainModel;
namespace Universal.TileMapping
{
    public class MapUtil : MonoBehaviour
    {
        public static MapUtil Instance;
        public TileMap tileMap;
        public VectorLine line;
        public VectorLine newLine;
        public VectorLine listLine;
        public Transform tran;
        Vector3 pos;

        private void Start()
        {
            Instance = this;
            if (TerrainEditorUICtrl.Instance == null)
                return;
            SetLine();

            int mapWidth = tileMap.MapWidth;
            int mapHeight = tileMap.MapHeight;

			int mapPhase = 1;

			TerrainEditorModel.LoadMapSize(ref mapWidth, ref mapHeight, ref mapPhase);

            for (int i = 0; i <= mapHeight; i++)
            {
                Vector3 start = tileMap.Coordinate2WorldPosition(0, i);
                Vector3 end = tileMap.Coordinate2WorldPosition(mapWidth, i);
                DrawGrid(start, end);
            }
            //Draw vertical grid lines
            for (int i = 0; i <= mapWidth; i++)
            {
                Vector3 start = tileMap.Coordinate2WorldPosition(i, 0);
                Vector3 end = tileMap.Coordinate2WorldPosition(i, mapHeight);
                DrawGrid(start, end);
            }
            line.Draw();
            transform.RotateAround(new Vector2(Screen.width / 2, Screen.height / 2), Vector3.forward, Time.deltaTime * 90.0f * Input.GetAxis("Horizontal"));
            Canvas canvas = line.rectTransform.parent.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = -2;

            if (tran != null)
            {
                SetNewLine();
              
				DrawGridHightLight (new int[]{1, 1});

                canvas = line.rectTransform.parent.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.sortingLayerName = "UI";
                canvas.sortingOrder = -2;

                SetListLine();

                listLine.Draw();
                canvas = listLine.rectTransform.parent.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.sortingLayerName = "UI";
                canvas.sortingOrder = -2;
            }

			TerrainEditorUICtrl.Instance.Init();
		}

        public void DisableDraw()
        {
            if (TerrainEditorUICtrl.Instance == null)
                return;
            line.rectTransform.parent.gameObject.SetActive(false);
        }
        public void EnableDraw()
        {
            if (TerrainEditorUICtrl.Instance == null)
                return;
            line.rectTransform.parent.gameObject.SetActive(true);
        }

        public void DrawListLineGrid(int x0,int x1,int y0,int y1)
        {
            if (TerrainEditorUICtrl.Instance == null)
                return;
            int mapHeight = y1;
            int mapWidth = x1;
            listLine.points2.Clear();

            for (int i = y0; i <= mapHeight; i++)
            {
                Vector3 start = tileMap.Coordinate2WorldPosition(x0, i);
                Vector3 end = tileMap.Coordinate2WorldPosition(mapWidth, i);
                listLine.points2.Add(start);
                listLine.points2.Add(end);
            }
            //Draw vertical grid lines
            for (int i = x0; i <= mapWidth; i++)
            {
                Vector3 start = tileMap.Coordinate2WorldPosition(i, y0);
                Vector3 end = tileMap.Coordinate2WorldPosition(i, mapHeight);
                listLine.points2.Add(start);
                listLine.points2.Add(end);
            }
            listLine.Draw();
        }

        public void ClearLineList()
        {
            if (TerrainEditorUICtrl.Instance == null)
                return;
            listLine.points2.Clear();
            listLine.Draw();
        }

        public void DrawMapGridLine(int width, int height)
        {
            if (TerrainEditorUICtrl.Instance == null)
                return;
            line.points2.Clear();
            int mapWidth = width;
            int mapHeight = height;

            for (int i = 0; i <= mapHeight; i++)
            {
                Vector3 start = tileMap.Coordinate2WorldPosition(0, i);
                Vector3 end = tileMap.Coordinate2WorldPosition(mapWidth, i);
                DrawGrid(start, end);
            }
            //Draw vertical grid lines
            for (int i = 0; i <= mapWidth; i++)
            {
                Vector3 start = tileMap.Coordinate2WorldPosition(i, 0);
                Vector3 end = tileMap.Coordinate2WorldPosition(i, mapHeight);
                DrawGrid(start, end);
            }
            line.Draw();
        }

        public void DrawGrid(Vector2 startPos, Vector2 endPos)
        {
            if (TerrainEditorUICtrl.Instance == null)
                return;
            line.points2.Add(startPos);
            line.points2.Add(endPos);
            Color color = Color.gray;
            color.a = 0.5f;
            line.color = color;
        }

        public void DrawGridHightLight(int[] area)
        {
            if (TerrainEditorUICtrl.Instance == null)
                return;
            newLine.points2.Clear();
			Vector3 tmp = Vector3.zero;
            tran.position = tmp;
            for (int i = 0; i <= area[1]; i++)
            {
				Vector3 start = tran.transform.position + Vector3.right * i;
				Vector3 end = start  + Vector3.up;

                DrawGridHightLight(start, end );
            }
            //Draw vertical grid lines
            for (int i = 0; i <= area[0]; i++)
            {
				Vector3 start =  tran.transform.position + Vector3.up * i;
				Vector3 end = start  + Vector3.right;
                DrawGridHightLight(start, end);
            }
            newLine.Draw();
        }

        public void DrawGridHightLight(Vector2 startPos, Vector2 endPos)
        {
            if (TerrainEditorUICtrl.Instance == null)
                return;
            newLine.points2.Add(startPos);
            newLine.points2.Add(endPos);
            Color color = Color.gray;
            color.a = 0.5f;
            line.color = color;
        }

        void SetLine()
        {
            VectorLine.Destroy(ref line);

            var lineType = LineType.Discrete;//(true ? LineType.Continuous : LineType.Discrete);
            var joins = Joins.None;//(true ? Joins.Fill : Joins.None);
            var lineWidth = 0.1f;// (false ? 24 : 2);

            line = new VectorLine("Line", new List<Vector2>(), lineWidth, lineType, joins);
            line.drawTransform = transform;
        }

        void SetNewLine()
        {
            VectorLine.Destroy(ref newLine);

            var lineType = LineType.Discrete;//(true ? LineType.Continuous : LineType.Discrete);
            var joins = Joins.None;//(true ? Joins.Fill : Joins.None);
            var lineWidth = 0.1f;// (false ? 24 : 2);

            newLine = new VectorLine("NewLine", new List<Vector2>(), lineWidth, lineType, joins);
            newLine.drawTransform = tran;
        }

        void SetListLine()
        {
            VectorLine.Destroy(ref listLine);

            var lineType = LineType.Discrete;//(true ? LineType.Continuous : LineType.Discrete);
            var joins = Joins.None;//(true ? Joins.Fill : Joins.None);
            var lineWidth = 0.1f;// (false ? 24 : 2);

            listLine = new VectorLine("listLine", new List<Vector2>(), lineWidth, lineType, joins);
            listLine.drawTransform = transform;
        }
        private void Update()
        {
            if (tran != null)
            {
                newLine.rectTransform.transform.position = tran.position;
            }
        }
    }
}

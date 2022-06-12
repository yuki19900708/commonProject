using System;
using System.Collections.Generic;
using UnityEngine;

namespace Universal.TileMapping
{

    [ExecuteInEditMode, AddComponentMenu("2D/TileMap"), DisallowMultipleComponent]
    public class TileMap : MonoBehaviour
    {
#if UNITY_EDITOR
        public static ScriptableTile pickedTile;
#endif
        public enum Layout
        {
            CartesianCoordinate,
            IsometricDiamond,
            IsometricDiamondFreestyle,
            IsometricStaggered,
            Hexagonal
        }

        public enum HexOrientation
        {
            PointySideUp,
            FlatSideUp
        }

        [SerializeField]
        private int mapWidth = 25, mapHeight = 25; // The Whole map width(horizontal grid count) and height(vertical grid count)
        [SerializeField]
        private float gridSize = 1; // Grid size when layout is CartesianCoordinate
        [SerializeField]
        private float xRotation = 30; // x axis rotation(to unity x axis counterclockwise) when layout is IsometricDiamondFreestyle
        private float sinX, cosX;
        [SerializeField]
        private float yRotation = 60; // y axis rotation(to unity y axis counterclockwise) when layout is IsometricDiamondFreestyle
        private float sinY, cosY;
        [SerializeField]
        private float isoWidth = 0, isoHeight = 0; // Single isometric grid width and height when layout is Isometric*, but the value means length along the unity world x y asix when layout is IsometricDiamond and IsometricStaggered, means the length along the tile shape axis when layout is IsometricDiamondFreestyle
        private float isoHalfWidth, isoHalfHeight;
        private Vector2 freestylePivot;
        private float freestyleWidth, freestyleHeight;
        [SerializeField]
        private float outerRadius = 0.5f;
        [SerializeField]
        private HexOrientation orientation = HexOrientation.PointySideUp;
        private float innerRadius = 0;
        private Vector2[] hexCornersOffset = new Vector2[6];
        [SerializeField]
        private ScriptableTile2DArray map = new ScriptableTile2DArray(0,0);
        [SerializeField]
        private Layout mapLayout = Layout.IsometricDiamond;

        private bool CurrentOperation = false;
        private List<ChangeElement> CurrentEdit;
        private Timeline timeline;

        public Action<int, int> OnUpdateTileAt = (x, y) => { };
        public Action OnUpdateTileMap = () => { };
        public Action<int, int> OnResize = (width, height) => { };
        public Action OnCompleteReset;

        public ScriptableTile2DArray Map { get { return map; } } 
        public int MapWidth { get { return mapWidth; } }
        public int MapHeight { get { return mapHeight; } }

        public float GridSize { get { return gridSize; } }

        public float IsoWidth { get { return isoWidth; } }
        public float IsoHeight { get { return isoHeight; } }

        public float XRotation { get { return xRotation; } }
        public float YRotation { get { return yRotation; } }

        public float OuterRadius { get { return outerRadius; } }
        public HexOrientation Orientation { get { return orientation; } }
        public float InnerRadius { get { return innerRadius; } }
        public Vector2[] HexCornersOffset { get { return hexCornersOffset; } }

        public Layout MapLayout { get { return mapLayout; } }

        public Vector2 FreestylePivot { get { return freestylePivot; } }
        public float FreestyleWidth { get { return freestyleWidth; } }
        public float FreestyleHeight { get { return freestyleHeight; } }

        private void Awake()
        {
            UpdateIsoHalfValue();
            UpdateFreestyle();
            UpdateHexInnerRadius();
            UpdateHexCornerOffset();
        }

        public void CompleteReset()
        {
            //OnCompleteReset();
            Reset();
        }

        public void Reset()
        {
            map = new ScriptableTile2DArray(mapWidth, mapHeight);
            timeline = new Timeline();
            CurrentEdit = new List<ChangeElement>();

            UpdateTileMap();
        }

        public void ResizeMap(int mapWidth, int mapHeight, bool isAttachedObject = true)
        {
            if ((mapWidth <= 0 || mapHeight <= 0) || (this.mapWidth == mapWidth && this.mapHeight == mapHeight))
                return;

            ScriptableTile2DArray oldMap = map;

            map = new ScriptableTile2DArray(mapWidth, mapHeight);
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            OnResize(mapWidth, mapHeight);
            if (isAttachedObject)
            {
                for (int x = 0; x < oldMap.ColumnCount; x++)
                {
                    for (int y = 0; y < oldMap.Row; y++)
                    {
                        ScriptableTile tile = oldMap[x, y];
                        if (tile && IsInBounds(x, y))
                            SetTileAt(x, y, tile);
                    }
                }
            }
        }

        public void ResizeGrid(float gridSize)
        {
            this.gridSize = gridSize;
            UpdateTileMap();
        }

        public void ResizeHexGrid(float outerRadius)
        {
            this.outerRadius = outerRadius;
            UpdateHexInnerRadius();
            UpdateHexCornerOffset();
            UpdateTileMap();
        }

        public void ResizeHexOrientation(HexOrientation orientation)
        {
            this.orientation = orientation;
            UpdateHexCornerOffset();
            UpdateTileMap();
        }

        private void UpdateHexInnerRadius()
        {
            this.innerRadius = outerRadius * 0.866025404f;
        }

        private void UpdateHexCornerOffset()
        {
            /* 
              PointyUp point position, value is offset to center
                  4
                5 ︿ 3
                |    |
                0 ﹀ 2
                   1

              Flat up point position, value is offset to center
               4 ___3
               5/   \2
                \___/
                0   1

            */
            if (orientation == HexOrientation.PointySideUp)
            {
                hexCornersOffset[0] = new Vector2(-innerRadius, -outerRadius / 2);
                hexCornersOffset[1] = new Vector2(0, -outerRadius);
                hexCornersOffset[2] = new Vector2(innerRadius, -outerRadius / 2);
                hexCornersOffset[3] = new Vector2(innerRadius, outerRadius / 2);
                hexCornersOffset[4] = new Vector2(0, outerRadius);
                hexCornersOffset[5] = new Vector2(-innerRadius, outerRadius / 2);
            }
            else
            {
                hexCornersOffset[0] = new Vector2(-outerRadius / 2, -innerRadius);
                hexCornersOffset[1] = new Vector2(outerRadius / 2, -innerRadius);
                hexCornersOffset[2] = new Vector2(outerRadius, 0);
                hexCornersOffset[3] = new Vector2(outerRadius / 2, innerRadius);
                hexCornersOffset[4] = new Vector2(-outerRadius / 2, innerRadius);
                hexCornersOffset[5] = new Vector2(-outerRadius, 0);
            }
        }

        public void ChangeFreestyleRotation(float xRotation, float yRotation)
        {
            this.xRotation = xRotation;
            this.yRotation = yRotation;
            UpdateFreestyle();
            UpdateTileMap();
        }

        private void UpdateFreestyle()
        {
            /*          y
                        ↑
                        │    /
             ╲yRotation│   /
                ╲      │  /
                   ╲ ☇ │ /  xRotation
                      ╲│/ ↶ 
             —————————————➝ x
            */
            sinX = Mathf.Sin(xRotation * Mathf.Deg2Rad);
            cosX = Mathf.Cos(xRotation * Mathf.Deg2Rad);
            sinY = Mathf.Sin(yRotation * Mathf.Deg2Rad);
            cosY = Mathf.Cos(yRotation * Mathf.Deg2Rad);

            freestyleWidth = cosX * isoWidth + sinY * isoHeight;
            freestyleHeight = sinX * isoWidth + cosY * isoHeight;
            freestylePivot = new Vector2(sinY * isoHeight / freestyleWidth, 0);
        }

        public void ResizeIso(float isoWidth, float isoHeight)
        {
            this.isoWidth = isoWidth;
            this.isoHeight = isoHeight;
            UpdateIsoHalfValue();
            UpdateFreestyle();
            UpdateTileMap();
        }

        private void UpdateIsoHalfValue()
        {
            this.isoHalfWidth = isoWidth / 2;
            this.isoHalfHeight = isoHeight / 2;
        }

        public void ChangeLayout(Layout layout)
        {
            this.mapLayout = layout;
            UpdateTileMap();
        }

        public bool IsInBounds(Point point)
        {
            return IsInBounds(point.x, point.y);
        }
        public bool IsInBounds(int x, int y)
        {
            return (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight);
        }

        public bool IsEdge(Point point)
        {
            return IsEdge(point.x, point.y);
        }

        public bool IsEdge(int x, int y)
        {
            bool isEdge = false;
            for (int i = 0; i < Point.neighbour.Length; i++)
            {
                Point p = Point.neighbour[i];
                bool exists = GetTileAt(p.x + x, p.y + y) != null;
                if (!exists)
                {
                    isEdge = true;
                    break;
                }
            }
            return isEdge;
        }

        public ScriptableTile GetTileAt(Vector2 worldPosition)
        {
            return GetTileAt(WorldPosition2Coordinate(worldPosition));
        }
        public ScriptableTile GetTileAt(Point point)
        {
            return GetTileAt(point.x, point.y);
        }
        public ScriptableTile GetTileAt(int x, int y)
        {
            if (!IsInBounds(x, y))
                return null;

            return map[x, y];
        }

        public bool SetTileAt(Vector2 worldPosition, ScriptableTile to)
        {
            return SetTileAt(WorldPosition2Coordinate(worldPosition), to);
        }
        public bool SetTileAt(Point point, ScriptableTile to)
        {
            return SetTileAt(point.x, point.y, to);
        }
        public bool SetTileAt(int x, int y, ScriptableTile to, bool updateNeighbour = true)
        {
            ScriptableTile from = GetTileAt(x, y);
            //Conditions for returning
            if (IsInBounds(x, y) /*&&
                !(from == null && to == null) &&
                (((from == null || to == null) && (from != null || to != null)) ||
                from.ID != to.ID)*/)
            {
                map[x, y] = to;

                if (CurrentEdit == null)
                    CurrentEdit = new List<ChangeElement>();
                CurrentEdit.Add(new ChangeElement(x, y, from, to));

                UpdateTileAt(x, y);
                if (updateNeighbour)
                {
//                    UpdateNeighbours(x, y, true);
                }
                return true;
            }
            return false;
        }
        public void SetTileAndUpdateNeighbours(Point point, ScriptableTile to)
        {
            SetTileAt(point.x, point.y, to, true);
        }
        public void SetTileAndUpdateNeighbours(int x, int y, ScriptableTile to)
        {
            SetTileAt(x, y, to, true);
        }

        public void UpdateTileAt(Point point)
        {
            UpdateTileAt(point.x, point.y);
        }
        public void UpdateTileAt(int x, int y)
        {
            OnUpdateTileAt(x, y);
        }
        public void UpdateTilesAt(Point[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                UpdateTileAt(points[i]);
            }
        }
        public void UpdateNeighbours(int x, int y, bool incudeCorners = false)
        {
            ScriptableTile centerTile = map[x, y];
            for (int xx = -1; xx <= 1; xx++)
            {
                for (int yy = -1; yy <= 1; yy++)
                {
                    if (xx == 0 && yy == 0)
                        continue;

                    if (!incudeCorners && !(xx == 0 || yy == 0))
                        continue;

                    if (IsInBounds(x + xx, y + yy))
                    {
                        ScriptableTile tile = map[x + xx, y + yy];
                        if (tile != null && tile is AutoTile)
                        {
                            bool isSame = centerTile != null && tile.ID == centerTile.ID;
                            if (isSame || (tile as AutoTile).mode == AutoTile.AutoTileMode.Everything)
                            {
                                UpdateTileAt(x + xx, y + yy);
                            }
                        }
                    }
                }
            }
        }
        public void UpdateType(ScriptableTile type)
        {
            for (int x = 0; x <= MapWidth; x++)
            {
                for (int y = 0; y <= MapHeight; y++)
                {
                    if(GetTileAt(x, y) == type) {
                        UpdateTileAt(x, y);
                    }
                }
            }        
        }
        public List<ScriptableTile> GetAllTileTypes()
        {
            List<ScriptableTile> result = new List<ScriptableTile>();
            for (int x = 0; x <= MapWidth; x++)
            {
                for (int y = 0; y <= MapHeight; y++)
                {
                    ScriptableTile tile = GetTileAt(x, y);
                    if(!result.Contains(tile) && tile != null)
                    {
                        result.Add(tile);
                    }
                }
            }
            return result;
        }
        public void UpdateTileMap()
        {
            OnUpdateTileMap();
        }

        public Vector3 Coordinate2WorldPosition(Point point)
        {
            return Coordinate2WorldPosition(point.x, point.y);
        }

        public Vector3 Coordinate2WorldPosition(float x, float y)
        {
			return this.transform.position + Coordinate2LocalPosition (x, y);
//			+new Vector3(0, 0, (x + y) * 0.01f);
        }

        public Vector3 Coordinate2LocalPosition(float x, float y)
        {
            if (mapLayout == Layout.CartesianCoordinate)
            {
                return new Vector3((x - mapWidth * 0.5f) * gridSize, (y - mapHeight * 0.5f) * gridSize);
                //return new Vector3((x/*)*/ * gridSize, (y) * gridSize);

            }
            return Vector3.zero;
        }

        public Vector2 World2CoordinateFloat(Vector3 worldPos)
        {
            Vector3 posInTileMap = worldPos - this.transform.position;
            if (mapLayout == Layout.CartesianCoordinate)
            {
//				return Vector2.zero;
//				Vector2 re = new Vector2((posInTileMap.x +  mapWidth * 0.5f) / gridSize , (posInTileMap.y)/ gridSize + 0.5f);
//				Debug.LogError (posInTileMap + "***" + re);

				return new Vector2( mapWidth * 0.5f + (posInTileMap.x) / gridSize , mapHeight * 0.5f + (posInTileMap.y)/ gridSize);
            }
            
            return Vector2.zero;
        }

        public Point WorldPosition2Coordinate(Vector3 worldPos)
        {
            return (Point)World2CoordinateFloat(worldPos);
        }

        public bool CanUndo
        {
            get { return (timeline != null && timeline.CanUndo); }
        }
        public bool CanRedo
        {
            get { return (timeline != null && timeline.CanRedo); }
        }

        public void Undo()
        {
            if (timeline == null)
                return;
            List<ChangeElement> changesToRevert = timeline.Undo();

            foreach (var c in changesToRevert)
            {
                map[c.x, c.y] = c.from;
                UpdateTileAt(c.x, c.y);
                UpdateNeighbours(c.x, c.y, true);
            }
        }

        public void Redo()
        {
            if (timeline == null)
                return;
            List<ChangeElement> changesToRevert = timeline.Redo();

            foreach (var c in changesToRevert)
            {
                map[c.x, c.y] = c.to;
                UpdateTileAt(c.x, c.y);
                UpdateNeighbours(c.x, c.y, true);
            }
        }

        public void BeginOperation()
        {
            CurrentOperation = true;
            CurrentEdit = new List<ChangeElement>();
        }

        public void FinishOperation()
        {
            CurrentOperation = false;
            if (timeline == null)
                timeline = new Timeline();
            timeline.PushChanges(CurrentEdit);
        }

        public bool OperationInProgress()
        {
            return CurrentOperation;
        }
    }
}


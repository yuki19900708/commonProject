using System.Collections.Generic;
using System;
using UnityEngine;

namespace Universal.TileMapping
{
    /// <summary>
    /// 针对本游戏特别制作的优化后的渲染器，可以将整张Map的渲染DrawCall降为1，不具备通用性，且需要其他手段配合
    /// 使用要求：1. 本Map所有Tile使用的图片来自同一张图集
    /// </summary>
    [AddComponentMenu("2D/Renderer/TileMeshRenderer")]
    public class TileMeshRenderer : TileRenderer
    {
        public Material material;
        public enum TextureAtlasMode { Normal, Dynamic, SameFile }
        [SerializeField]
        private Texture2D atlas;
        [SerializeField]
        private MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        private bool isBatching = false;

        public Texture2D Atlas
        {
            get { return atlas; }
        }

        public override void Start()
        {
            base.Start();
            meshRenderer.sharedMaterial = material;
            meshRenderer.sharedMaterial.color = color;
            meshRenderer.sortingLayerID = sortingLayer;
            meshRenderer.sortingOrder = orderInLayer;
        }

        public override void Resize(int width, int height)
        {
        }

        public override void Reset()
        {
            material = new Material(Shader.Find("Unlit/Transparent"));
            SetUpMesh();
            base.Reset();
        }

        private void SetUpMesh()
        {
            if (meshRenderer == null || meshFilter == null)
            {
                ClearChildren();
                meshFilter = new GameObject("_MESH").AddComponent<MeshFilter>();
                meshFilter.transform.SetParent(parent);
                meshFilter.transform.localPosition = Vector3.zero;
                meshFilter.transform.localScale = Vector3.one;

                meshRenderer = meshFilter.gameObject.AddComponent<MeshRenderer>();
                meshRenderer.transform.SetParent(parent);
                meshRenderer.transform.localPosition = Vector3.zero;
                meshRenderer.transform.localScale = Vector3.one;
            }
        }

        public override void UpdateTileAt(int x, int y)
        {
            atlas = null;
            if (isBatching == false)
            {
                GenerateMesh();
            }
        }

        public void BeginBatch()
        {
            isBatching = true;
        }

        public void EndBatch()
        {
            isBatching = false;
            GenerateMesh();
        }

        private void GenerateMesh()
        {
            List<Vector3> vertices = new List<Vector3>(0);
            List<int> triangles = new List<int>(0);
            List<Vector3> normals = new List<Vector3>(0);
            List<Vector2> uv = new List<Vector2>(0);
            int verticesCountOffset = 0;
            for (int x = 0; x < tileMap.MapWidth; x++)
            {
                for (int y = 0; y < tileMap.MapHeight; y++)
                {
                    ScriptableTile tile = tileMap.GetTileAt(x, y);
                    if (tile == null)
                    {
                        continue;
                    }
                    GameObject prefab = tile.GetTile(tileMap, new Point(x, y));
                    if (prefab == null)
                    {
                        continue;
                    }
                    SpriteRenderer sr = prefab.GetComponentInChildren<SpriteRenderer>();
                    if (sr == null)
                    {
                        Debug.LogError(string.Format("TileMeshRenderer Error: tile objects doesn't have SpriteRendererComponent at x:{0} y:{1}", x, y));
                        continue;
                    }
                    if (sr.sprite == null)
                    {
                        Debug.LogError(string.Format("TileMeshRenderer Error: sprite is null at x:{0} y:{1}", x, y));
                        continue;
                    }
                    if (atlas == null)
                    {
                        atlas = sr.sprite.texture;
                        meshRenderer.sharedMaterial.SetTexture("_MainTex", atlas);
                    }
                    else if (atlas != sr.sprite.texture)
                    {
                        Debug.LogError(string.Format("TileMeshRenderer Error: There are different atals at x:{0} y:{1}", x, y));
                        continue;
                    }
                    Vector3 basePos = tileMap.Coordinate2LocalPosition(x, y);

                    if (autoPivot)
                    {
                        //pivot change will affect the sprite transform position, because tile map system is base on 
                        //fixed pivot, so the keypoint is calculate the offset between actual pivot and bottomCenter
                        Vector2 pivotTileMap;
                        if (tileMap.MapLayout == TileMap.Layout.CartesianCoordinate)
                        {
                            pivotTileMap = new Vector2(0f, 0f);
                        }
                        else if (tileMap.MapLayout == TileMap.Layout.Hexagonal)
                        {
                            pivotTileMap = new Vector2(0.5f, 0.5f);
                        }
                        else if (tileMap.MapLayout == TileMap.Layout.IsometricDiamondFreestyle)
                        {
                            pivotTileMap = tileMap.FreestylePivot;
                        }
                        else
                        {
                            pivotTileMap = new Vector2(0.5f, 0);
                        }
                        Vector2 pivotActual = new Vector2(sr.sprite.pivot.x / sr.sprite.rect.width, sr.sprite.pivot.y / sr.sprite.rect.height);
                        Vector2 pivotOffset = pivotActual - pivotTileMap;
                        //Then translate the pivotOffset's world position to tilemap coordinate
                        Vector3 offset = Vector3.zero;
                        if (tileMap.MapLayout == TileMap.Layout.CartesianCoordinate)
                        {
                            offset = new Vector3(pivotOffset.x * tileMap.GridSize, pivotOffset.y * tileMap.GridSize);
                        }
                        else if (tileMap.MapLayout == TileMap.Layout.Hexagonal)
                        {
                            if (tileMap.Orientation == TileMap.HexOrientation.PointySideUp)
                            {
                                offset = new Vector3(pivotOffset.x * tileMap.InnerRadius * 2, pivotOffset.y * tileMap.OuterRadius * 2);
                            }
                            else
                            {
                                offset = new Vector3(pivotOffset.x * tileMap.OuterRadius * 2, pivotOffset.y * tileMap.InnerRadius * 2);
                            }
                        }
                        else if (tileMap.MapLayout == TileMap.Layout.IsometricDiamondFreestyle)
                        {
                            offset = new Vector3(pivotOffset.x * tileMap.FreestyleWidth, pivotOffset.y * tileMap.FreestyleHeight);
                        }
                        else
                        {
                            offset = new Vector3(pivotOffset.x * tileMap.IsoWidth, pivotOffset.y * tileMap.IsoHeight);
                        }
                        basePos += offset;
                    }
                    for (int i = 0; i < sr.sprite.vertices.Length; i++)
                    {
                        Vector3 vertPos = sr.sprite.vertices[i];
                        if (sr.flipX)
                        {
                            vertPos.x = -vertPos.x;
                        }
                        if (sr.flipY)
                        {
                            vertPos.y = -vertPos.y;
                        }
                        vertices.Add(vertPos + basePos);
                        normals.Add(Vector3.forward);
                        uv.Add(sr.sprite.uv[i]);
                    }
                    if (sr.flipX ^ sr.flipY)
                    {
                        for (int i = sr.sprite.triangles.Length - 1; i >= 0; i--)
                        {
                            triangles.Add(sr.sprite.triangles[i] + verticesCountOffset);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < sr.sprite.triangles.Length; i++)
                        {
                            triangles.Add(sr.sprite.triangles[i] + verticesCountOffset);
                        }
                    }
                    verticesCountOffset += sr.sprite.vertices.Length;
                }
            }
            
            var mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                normals = normals.ToArray(),
                uv = uv.ToArray(),
                name = "TileMesh"
            };
            meshFilter.sharedMesh = mesh;
        }
    }
}

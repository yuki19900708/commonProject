using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/PolyImage", 51)]
    [DisallowMultipleComponent]
    public class PolyImage : Image
    {
        #region Image.cs
        /// Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
        private Vector4 _GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = overrideSprite == null ? Vector4.zero : Sprites.DataUtility.GetPadding(overrideSprite);
            var size = overrideSprite == null ? Vector2.zero : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

            Rect r = GetPixelAdjustedRect();
            // Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                    padding.x / spriteW,
                    padding.y / spriteH,
                    (spriteW - padding.z) / spriteW,
                    (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                var spriteRatio = size.x / size.y;
                var rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
            }

            v = new Vector4(
                    r.x + r.width * v.x,
                    r.y + r.height * v.y,
                    r.x + r.width * v.z,
                    r.y + r.height * v.w
                    );

            return v;
        }

        Vector4 _GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int axis = 0; axis <= 1; axis++)
            {
                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                if (rect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    float borderScaleRatio = rect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }
        #endregion

#if UNITY_5_0 || UNITY_5_1
        static VertexHelper toFill = new VertexHelper();
        protected override void OnFillVBO(List<UIVertex> vbo)
#elif UNITY_5_2
        protected override void OnPopulateMesh(Mesh mesh)
#else
        protected override void OnPopulateMesh(VertexHelper toFill)
#endif
        {
            // Tiled & Filled not supported yet
            if (overrideSprite == null || type == Type.Tiled || type == Type.Filled)
            {
#if UNITY_5_0 || UNITY_5_1
                base.OnFillVBO(vbo);
#elif UNITY_5_2
                base.OnPopulateMesh(mesh);
#else
                base.OnPopulateMesh(toFill);
#endif
                return;
            }
            
            toFill.Clear();

            switch (type)
            {
                case Type.Simple:
                    GenerateSimplePolySprite(toFill, preserveAspect);
                    break;
                case Type.Sliced:
                    GenerateSlicedPolySprite(toFill);
                    break;
            }

#if UNITY_5_0 || UNITY_5_1
            toFill.FillVBO(vbo);
#elif UNITY_5_2
            toFill.FillMesh(mesh);
#endif
        }

        void GenerateSimplePolySprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 v = _GetDrawingDimensions(lPreserveAspect);
            Vector4 uv = Sprites.DataUtility.GetOuterUV(overrideSprite);

            Color color32 = color;

            Vector2[] uvs = overrideSprite.uv;
            float invU = 1 / (uv.z - uv.x);
            float invV = 1 / (uv.w - uv.y);
            for (int i = 0; i < uvs.Length; i++)
            {
                float u2 = invU * (uvs[i].x - uv.x);
                float v2 = invV * (uvs[i].y - uv.y);
                float x = u2 * (v.z - v.x) + v.x;
                float y = v2 * (v.w - v.y) + v.y;

                vh.AddVert(new Vector2(x, y), color32, uvs[i]);
            }

            ushort[] triangles = overrideSprite.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                vh.AddTriangle(triangles[i], triangles[i + 1], triangles[i + 2]);
            }
        }

        public static float Cross(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.y - lhs.y * rhs.x;
        }

        // idea comes from RadialCut
        private static List<ushort> LineCut(
            List<Vector2> uvs, List<ushort> triangles,
            Vector2 start, float angle, Func<Vector2, bool> predict = null)
        {
            List<ushort> splitTriangles = new List<ushort>();
            List<ushort> splitTriCache1 = new List<ushort>();
            List<ushort> splitTriCache2 = new List<ushort>();
            var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            
            for (int i = 0; i < triangles.Count; i += 3)
            {
                splitTriCache1.Clear();
                splitTriCache2.Clear();

                for (int j = 0; j < 3; j++)
                {
                    ushort tri1 = triangles[i + j], tri2 = triangles[i + (j + 1) % 3];
                    Vector2 uv1 = uvs[tri1], uv2 = uvs[tri2];

                    float sign1 = Cross(offset, uv1 - start), sign2 = Cross(offset, uv2 - start);

                    if (sign1 <= 0)
                        splitTriCache1.Add(tri1);
                    if (sign1 >= 0)
                        splitTriCache2.Add(tri1);

                    if (sign1 * sign2 < 0)
                    {
                        // Line Intersects!
                        var diff = uv2 - uv1;
                        float t1 = -sign1 / Cross(offset, uv2 - uv1);
                        var p = uv1 + diff * t1;

                        ushort idx = (ushort)uvs.Count;
                        uvs.Add(p);

                        splitTriCache1.Add(idx);
                        splitTriCache2.Add(idx);
                    }
                }

                if (splitTriCache1.Count >= 3)
                {
                    for (int j = 2; j < splitTriCache1.Count; j++)
                    {
                        if(predict != null)
                        {
                            Vector2 center = (uvs[splitTriCache1[0]] + uvs[splitTriCache1[j - 1]] + uvs[splitTriCache1[j]]) / 3;
                            if (!predict(center)) continue;
                        }
                        splitTriangles.Add(splitTriCache1[0]);
                        splitTriangles.Add(splitTriCache1[j - 1]);
                        splitTriangles.Add(splitTriCache1[j]);
                    }
                }
                if (splitTriCache2.Count >= 3)
                {
                    for (int j = 2; j < splitTriCache2.Count; j++)
                    {
                        if (predict != null)
                        {
                            Vector2 center = (uvs[splitTriCache2[0]] + uvs[splitTriCache2[j - 1]] + uvs[splitTriCache2[j]]) / 3;
                            if (!predict(center)) continue;
                        }
                        splitTriangles.Add(splitTriCache2[0]);
                        splitTriangles.Add(splitTriCache2[j - 1]);
                        splitTriangles.Add(splitTriCache2[j]);
                    }
                }
            }

            return splitTriangles;
        }

        static readonly Vector2[] s_VertScratch = new Vector2[4];
        static readonly Vector2[] s_UVScratch = new Vector2[4];
        
        private static int XSlot(float x)
        {
            for (int idx = 0; idx < 3; idx++)
            {
                if (s_UVScratch[idx].x < s_UVScratch[idx + 1].x && x <= s_UVScratch[idx + 1].x)
                    return idx;
            }
            return 2;
        }

        private static int YSlot(float y)
        {
            for (int idx = 0; idx < 3; idx++)
            {
                if (s_UVScratch[idx].y < s_UVScratch[idx + 1].y && y <= s_UVScratch[idx + 1].y)
                    return idx;
            }            
            return 2;
        }
        
        private void GenerateSlicedPolySprite(VertexHelper toFill)
        {
            if (!hasBorder)
            {
                GenerateSimplePolySprite(toFill, false);
                return;
            }

            Vector4 outer = Sprites.DataUtility.GetOuterUV(overrideSprite);
            Vector4 inner = Sprites.DataUtility.GetInnerUV(overrideSprite);
            Vector4 padding = Sprites.DataUtility.GetPadding(overrideSprite);
            Vector4 border = overrideSprite.border;

            Rect rect = GetPixelAdjustedRect();
            border = _GetAdjustedBorders(border / pixelsPerUnit, rect);
            padding = padding / pixelsPerUnit;

            s_VertScratch[0] = new Vector2(padding.x, padding.y);
            s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

            s_VertScratch[1].x = border.x;
            s_VertScratch[1].y = border.y;
            s_VertScratch[2].x = rect.width - border.z;
            s_VertScratch[2].y = rect.height - border.w;

            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);
            
            List<Vector2> uvs = new List<Vector2>(overrideSprite.uv);
            List<ushort> triangles = new List<ushort>(overrideSprite.triangles);

            var splitedTriangles = LineCut(uvs, triangles, new Vector2(inner.x, inner.y), Mathf.PI / 2);
            splitedTriangles = LineCut(uvs, triangles, new Vector2(inner.z, inner.w), 0);
            splitedTriangles = LineCut(uvs, triangles, new Vector2(inner.x, inner.y), Mathf.PI / 2);
            splitedTriangles = LineCut(uvs, triangles, new Vector2(inner.z, inner.w), 0);
            splitedTriangles = LineCut(uvs, triangles, new Vector2(inner.x, inner.y), Mathf.PI / 2);
            splitedTriangles = LineCut(uvs, triangles, new Vector2(inner.z, inner.w), 0);
            splitedTriangles = LineCut(uvs, triangles, new Vector2(inner.x, inner.y), Mathf.PI / 2);
            splitedTriangles = LineCut(uvs, triangles, new Vector2(inner.z, inner.w), 0);

            for (int i = 0; i < uvs.Count; i++)
            {
                int x = XSlot(uvs[i].x);
                int y = YSlot(uvs[i].y);

                float kX = (uvs[i].x - s_UVScratch[x].x) / (s_UVScratch[x + 1].x - s_UVScratch[x].x);
                float kY = (uvs[i].y - s_UVScratch[y].y) / (s_UVScratch[y + 1].y - s_UVScratch[y].y);

                Vector2 pos = new Vector2(kX * (s_VertScratch[x + 1].x - s_VertScratch[x].x) + s_VertScratch[x].x,
                    kY * (s_VertScratch[y + 1].y - s_VertScratch[y].y) + s_VertScratch[y].y);

                toFill.AddVert(pos, color, uvs[i]);
            }

            for (int i = 0; i < splitedTriangles.Count; i += 3)
            {
                int x = XSlot(uvs[splitedTriangles[i + 0]].x);
                int y = YSlot(uvs[splitedTriangles[i + 0]].y);
                if (x == 1 && y == 1 && !fillCenter) continue;
                toFill.AddTriangle(splitedTriangles[i + 0], splitedTriangles[i + 1], splitedTriangles[i + 2]);
            }
        }
    }
}
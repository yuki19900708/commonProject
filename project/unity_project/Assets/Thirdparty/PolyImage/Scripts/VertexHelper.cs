#if UNITY_5_0 || UNITY_5_1
using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class VertexHelper
    {
        private List<Vector3> m_Positions = new List<Vector3>();
        private List<Color32> m_Colors = new List<Color32>();
        private List<Vector2> m_Uv0S = new List<Vector2>();
        private List<Vector2> m_Uv1S = new List<Vector2>();
        private List<Vector3> m_Normals = new List<Vector3>();
        private List<Vector4> m_Tangents = new List<Vector4>();
        private List<int> m_Indices = new List<int>();

        private static readonly Vector4 s_DefaultTangent = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);
        private static readonly Vector3 s_DefaultNormal = Vector3.back;

        public VertexHelper()
        {}
        
        public void Clear()
        {
            m_Positions.Clear();
            m_Colors.Clear();
            m_Uv0S.Clear();
            m_Uv1S.Clear();
            m_Normals.Clear();
            m_Tangents.Clear();
            m_Indices.Clear();
        }

        public int currentVertCount
        {
            get { return m_Positions.Count; }
        }
        public int currentIndexCount
        {
            get { return m_Indices.Count; }
        }
        
        public void FillVBO(List<UIVertex> vbo)
        {
            vbo.Clear();

            UIVertex vertex;
            for (int i = 0; i < m_Indices.Count; i++)
            {
                int idx = m_Indices[i];
                vertex.position = m_Positions[idx];
                vertex.color = m_Colors[idx];
                vertex.uv0 = m_Uv0S[idx];
                vertex.uv1 = m_Uv1S[idx];
                vertex.normal = m_Normals[idx];
                vertex.tangent = m_Tangents[idx];
                vbo.Add(vertex);
                // quad act as triangle, low performance :(
                if (i % 3 == 0)
                    vbo.Add(vertex);
            }

            if (vbo.Count >= 65000)
                throw new ArgumentException("Mesh can not have more than 65000 vertices");
        }

        public void AddVert(Vector3 position, Color32 color, Vector2 uv0, Vector2 uv1, Vector3 normal, Vector4 tangent)
        {
            m_Positions.Add(position);
            m_Colors.Add(color);
            m_Uv0S.Add(uv0);
            m_Uv1S.Add(uv1);
            m_Normals.Add(normal);
            m_Tangents.Add(tangent);
        }

        public void AddVert(Vector3 position, Color32 color, Vector2 uv0)
        {
            AddVert(position, color, uv0, Vector2.zero, s_DefaultNormal, s_DefaultTangent);
        }

        public void AddVert(UIVertex v)
        {
            AddVert(v.position, v.color, v.uv0, v.uv1, v.normal, v.tangent);
        }

        public void AddTriangle(int idx0, int idx1, int idx2)
        {
            m_Indices.Add(idx0);
            m_Indices.Add(idx1);
            m_Indices.Add(idx2);
        }

        public void AddUIVertexQuad(UIVertex[] verts)
        {
            int startIndex = currentVertCount;

            for (int i = 0; i < 4; i++)
                AddVert(verts[i].position, verts[i].color, verts[i].uv0, verts[i].uv1, verts[i].normal, verts[i].tangent);

            AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }
        
    }
}
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PolygonCollider2DCreator : MonoBehaviour
{
    [MenuItem("Appcpi/资源编辑/Create PolygonCollider2D #&C")]
    static void CreatePolygonCollider2D()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        if (Selection.activeGameObject == null || gameObjects == null || gameObjects.Length == 0)
        {
            return;
        }
        foreach (GameObject gameObject in gameObjects)
        {
            int[] triangles = null;
            Vector2[] vertices = null;
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                vertices = sr.sprite.vertices;
                triangles = sr.sprite.triangles.ToInt32Array();
            }
            else
            {
                MeshFilter mf = gameObject.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    vertices = mf.sharedMesh.vertices.ToVector2Array();
                    triangles = mf.sharedMesh.triangles;
                }
            }

            if (vertices == null || triangles == null)
            {
                return;
            }

            Vector2 offset = gameObject.transform.localPosition;

            // Get just the outer edges from the mesh's triangles (ignore or remove any shared edges)
            Dictionary<string, KeyValuePair<int, int>> edges = new Dictionary<string, KeyValuePair<int, int>>();
            for (int i = 0; i < triangles.Length; i += 3)
            {
                for (int e = 0; e < 3; e++)
                {
                    int vert1 = triangles[i + e];
                    int vert2 = triangles[i + e + 1 > i + 2 ? i : i + e + 1];
                    string edge = Mathf.Min(vert1, vert2) + ":" + Mathf.Max(vert1, vert2);
                    if (edges.ContainsKey(edge))
                    {
                        edges.Remove(edge);
                    }
                    else
                    {
                        edges.Add(edge, new KeyValuePair<int, int>(vert1, vert2));
                    }
                }
            }

            // Create edge lookup (Key is first vertex, Value is second vertex, of each edge)
            Dictionary<int, int> lookup = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> edge in edges.Values)
            {
                if (lookup.ContainsKey(edge.Key) == false)
                {
                    lookup.Add(edge.Key, edge.Value);
                }
            }

            // Create empty polygon collider
            PolygonCollider2D polygonCollider = gameObject.transform.root.GetComponent<PolygonCollider2D>();
            if (polygonCollider == null)
            {
                polygonCollider = gameObject.transform.root.gameObject.AddComponent<PolygonCollider2D>();
            }
            polygonCollider.pathCount = 0;

            // Loop through edge vertices in order
            int startVert = 0;
            int nextVert = startVert;
            int highestVert = startVert;
            List<Vector2> colliderPath = new List<Vector2>();
            while (true)
            {

                // Add vertex to collider path
                colliderPath.Add(vertices[nextVert] + offset);

                // Get next vertex
                nextVert = lookup[nextVert];

                // Store highest vertex (to know what shape to move to next)
                if (nextVert > highestVert)
                {
                    highestVert = nextVert;
                }

                // Shape complete
                if (nextVert == startVert)
                {

                    // Add path to polygon collider
                    polygonCollider.pathCount++;
                    polygonCollider.SetPath(polygonCollider.pathCount - 1, colliderPath.ToArray());
                    colliderPath.Clear();

                    // Go to next shape if one exists
                    if (lookup.ContainsKey(highestVert + 1))
                    {

                        // Set starting and next vertices
                        startVert = highestVert + 1;
                        nextVert = startVert;

                        // Continue to next loop
                        continue;
                    }

                    // No more verts
                    break;
                }
                //polygonCollider.isTrigger = true;
            }
        }
    }
}
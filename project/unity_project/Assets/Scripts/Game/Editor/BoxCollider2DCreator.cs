using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoxCollider2DCreator : MonoBehaviour
{
    [MenuItem("Appcpi/资源编辑/Create BoxCollider2D #&D")]
    static void CreateBoxCollider2D()
    {
        GameObject[] gameObjects = Selection.gameObjects;
        if (Selection.activeGameObject == null || gameObjects == null || gameObjects.Length == 0)
        {
            return;
        }
        foreach (GameObject gameObject in gameObjects)
        {
            Vector2[] vertices = null;
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                vertices = sr.sprite.vertices;
            }
            else
            {
                MeshFilter mf = gameObject.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    vertices = mf.sharedMesh.vertices.ToVector2Array();
                }
            }

            if (vertices == null)
            {
                return;
            }

            Vector2 offset = gameObject.transform.localPosition;

            // Create empty polygon collider
            BoxCollider2D boxCollider = gameObject.transform.root.GetComponent<BoxCollider2D>();
            if (boxCollider == null)
            {
                boxCollider = gameObject.transform.root.gameObject.AddComponent<BoxCollider2D>();
            }

            // Loop through edge vertices in order
            float left = 0;
            float right = 0;
            float top = 0;
            float bottom = 0;
            foreach (Vector2 vertice in vertices)
            {
                if (vertice.x > right)
                {
                    right = vertice.x;
                }
                if (vertice.x < left)
                {
                    left = vertice.x;
                }
                if (vertice.y > top)
                {
                    top = vertice.y;
                }
                if (vertice.y < bottom)
                {
                    bottom = vertice.y;
                }
            }
            boxCollider.size = new Vector2(right - left, top - bottom);
            boxCollider.offset = offset;
            boxCollider.isTrigger = true;
        }
    }
}
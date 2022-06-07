using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtend
{
    public static Vector2[] ToVector2Array(this Vector3[] v3)
    {
        return System.Array.ConvertAll<Vector3, Vector2>(v3, Vector3ToVector2);
    }

    public static Vector2 Vector3ToVector2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }

    public static Vector3[] ToVector3Array(this Vector2[] v3)
    {
        return System.Array.ConvertAll<Vector2, Vector3>(v3, Vector2ToVector3);
    }

    public static Vector3 Vector2ToVector3(Vector2 v2)
    {
        return new Vector3(v2.x, v2.y, 0);
    }
}

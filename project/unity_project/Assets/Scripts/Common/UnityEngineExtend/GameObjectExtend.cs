using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtend
{
    public static void SetLayerRecursively(this GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetLayerRecursively(layer);
        }
    }

    public static void SetLayerRecursively(this GameObject obj, string layerName)
    {
        int layerMask = LayerMask.NameToLayer(layerName);
        obj.layer = layerMask;

        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetLayerRecursively(layerMask);
        }
    }
}

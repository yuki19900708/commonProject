using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class UGUISpriteAtlasMaker
{
    [MenuItem("Assets/Make UGUI Sprite Atlas")]
    public static void MakeUGUISpriteAtlas()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
        MakeUGUISpriteAtlas(path);
    }

    public static void MakeUGUISpriteAtlas(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer != null && importer.textureType == TextureImporterType.Sprite)
        {
            GameObject atlasObj = new GameObject();
            UGUISpriteAtlas atlas = atlasObj.AddComponent<UGUISpriteAtlas>();
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            if (atlas.spriteList == null)
            {
                atlas.spriteList = new List<Sprite>(assets.Length);
            }
            foreach (Object asset in assets)
            {
                if (asset is Sprite)
                {
                    atlas.spriteList.Add(asset as Sprite);
                }
            }
            string prefabPath = path.Substring(0, path.LastIndexOf('.')) + ".prefab";
            if (File.Exists(prefabPath))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                PrefabUtility.ReplacePrefab(atlasObj, go, ReplacePrefabOptions.ReplaceNameBased);
            }
            else
            {
                PrefabUtility.CreatePrefab(prefabPath, atlasObj);
            }
            AssetImporter.GetAtPath(prefabPath).assetBundleName = System.IO.Path.GetFileNameWithoutExtension(path);
            UnityEngine.MonoBehaviour.DestroyImmediate(atlasObj);
        }
        else
        {
            Debug.LogWarning("选中的资源不是Sprite模式的图片!");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class SpriteAtlasPipeline : EditorWindow
{
    private static string atlasFolderPath = "/ResourcesRaw/Atlas";

    [MenuItem("Appcpi/数据生成/1.更新图集", false, 1)]
    public static void Init()
    {
        SpriteAtlasPipeline sap = EditorWindow.CreateInstance<SpriteAtlasPipeline>();
        sap.titleContent = new GUIContent("SpriteAtlasPipeline");
        sap.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Atlas目录：Assets");
                atlasFolderPath = EditorGUILayout.TextField(atlasFolderPath);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("生成spriteAtlasIni"))
            {
                GeneratSpriteAtlasIni();
            }
            if (GUILayout.Button("还原UGUISpriteTag"))
            {
                RestoreAllUGUISpriteTag();
            }
        }
        GUILayout.EndVertical();
    }

    public static void GeneratSpriteAtlasIni()
    {
        //生成sprite -> atlas name 的对应数据
        Dictionary<string, object> spriteAtlasDict = new Dictionary<string, object>();
        string[] atlasFiles = Directory.GetFiles(Application.dataPath + atlasFolderPath, "*.png");
        foreach (string atlasFile in atlasFiles)
        {
            string fileName = Path.GetFileName(atlasFile);
            string assetPath = string.Format("Assets{0}/{1}", atlasFolderPath, fileName);
            UGUISpriteAtlasMaker.MakeUGUISpriteAtlas(assetPath);
            Sprite[] sprites = (Sprite[])AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath).OfType<Sprite>().ToArray(); ;
            foreach (Sprite sprite in sprites)
            {
                if (spriteAtlasDict.ContainsKey(sprite.name))
                {
                    ((List<string>)spriteAtlasDict[sprite.name]).Add(sprite.texture.name);
                }
                else
                {
                    spriteAtlasDict.Add(sprite.name, new List<string>() { sprite.texture.name });
                }
            }
        }

        string spriteAtlasInfo = SimpleJson.SimpleJson.SerializeObject(spriteAtlasDict, new SimpleJson.PocoJsonSerializerStrategy());
        File.WriteAllText(Application.dataPath + "/Resources/spriteAtlasIni.txt", spriteAtlasInfo);
        AssetDatabase.Refresh();
    }

    public static void RestoreAllUGUISpriteTag()
    {
        GameObject[] assets = Resources.LoadAll<GameObject>("Prefabs");
        foreach (GameObject go in assets)
        {
            UGUISpriteLoader loader = go.GetComponent<UGUISpriteLoader>();
            if (loader != null)
            {
                DestroyImmediate(loader, true);
            }
            var images = go.GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                UGUISpriteTag tag = image.gameObject.GetComponent<UGUISpriteTag>();
                if (tag != null)
                {
                    tag.Restore();
                }
                DestroyImmediate(tag, true);
            }
            var srs = go.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in srs)
            {
                UGUISpriteTag tag = sr.gameObject.GetComponent<UGUISpriteTag>();
                if (tag != null)
                {
                    tag.Restore();
                }
                DestroyImmediate(tag, true);
            }
        }
    }
}

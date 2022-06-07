using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class UGUISpriteAtlasMgr : MonoBehaviour
{
    public static UGUISpriteAtlasMgr Instance
    {
        private set;
        get;
    }

    private static Dictionary<string, UGUISpriteAtlas> atlasDict = new Dictionary<string, UGUISpriteAtlas>();
    private static Dictionary<string, List<string>> spriteAtlasDict = null;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PreLoad();
    }

    private static void PreLoad()
    {
        string[] preLoadAtlas = new string[] {};

        foreach(string atlasName in preLoadAtlas)
        {
            GameObject atlasObj = ResMgr.Load<GameObject>(atlasName);
            UGUISpriteAtlas atlas = atlasObj.GetComponent<UGUISpriteAtlas>();
            atlasDict.Add(atlasName, atlas);
        }
    }

    /// <summary>
    /// 重新加载spriteAtlasIni,因为热更有可能更新了spriteAtlasIni
    /// </summary>
    public static void ReloadSpriteAtlasIni()
    {
        ResMgr.UnloadAssetBundle("spriteatlasini");
        TextAsset ta = ResMgr.Load<TextAsset>("spriteAtlasIni");
        spriteAtlasDict = SimpleJson.SimpleJson.DeserializeObject<Dictionary<string, List<string>>>(ta.text);
    }

    public static Sprite LoadSprite(string spriteName, string atlasName = "")
    {
        if (spriteAtlasDict == null)
        {
            TextAsset ta = ResMgr.Load<TextAsset>("spriteAtlasIni");
            spriteAtlasDict = SimpleJson.SimpleJson.DeserializeObject<Dictionary<string, List<string>>>(ta.text);
        }
        if (string.IsNullOrEmpty(atlasName))
        {
            if (spriteAtlasDict.ContainsKey(spriteName))
            {
                atlasName = spriteAtlasDict[spriteName][0];
            }
            else
            {
                Debug.LogWarning(string.Format("未找到Sprite:{0}对应的Atlas（忘了生成SpriteAtlasIni ?)", spriteName));
                return null;
            }
        }
        if (atlasDict.ContainsKey(atlasName) == false)
        {
//            Debug.Log(spriteName + "加载Atlas: " + atlasName);
            GameObject atlasObj = ResMgr.Load<GameObject>(atlasName);
            if (atlasObj != null)
            {
                UGUISpriteAtlas atlas = atlasObj.GetComponent<UGUISpriteAtlas>();
                atlas.Init();
                atlasDict.Add(atlasName, atlas);
            }
            else
            {
                Debug.LogWarning("加载Atlas: " + atlasName + "失败");
                return null;
            }
        }

        if (atlasDict[atlasName].spriteDict.ContainsKey(spriteName))
        {
            return atlasDict[atlasName].spriteDict[spriteName];
        }
        else
        {
            Debug.Log("无法在Atlas: " + atlasName + "中找到 Sprite: " + spriteName);
            return null;
        }
    }
}

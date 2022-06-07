using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UGUISpriteTag : MonoBehaviour
{
    /// <summary>
    /// UGUI的默认资源图集，需要在添加Tag时避开
    /// </summary>
    public static List<string> UGUI_EMBED_ATLAS = new List<string>()
    {
        "UISprite",
        "InputFieldBackground",
        "Background",
        "UIMask",
        "DropdownArrow",
        "Knob",
        "Checkmark"
    };

    public string atlasName;
    public string spriteName;

    [HideInInspector]
    public bool isSetValue = false;

    /// <summary>
    /// 当脚本被Reset或添加到GameObject上时，会触发Reset方法调用，用来初始化Sprite信息
    /// </summary>
    void Reset()
    {
        if (isSetValue == false)
        {
            isSetValue = true;
            Image image = this.GetComponent<Image>();
            if (image != null && image.sprite != null)
            {
                atlasName = image.sprite.texture.name;
                if (UGUI_EMBED_ATLAS.Contains(atlasName))
                {
                    atlasName = string.Empty;
                    return;
                }
                spriteName = image.sprite.name;
                image.sprite = null;
                return;
            }

            SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                atlasName = sr.sprite.texture.name;
                if (UGUI_EMBED_ATLAS.Contains(atlasName))
                {
                    atlasName = string.Empty;
                    return;
                }
                spriteName = sr.sprite.name;
                sr.sprite = null;
            }
        }
    }

	#if UNITY_EDITOR
	[ContextMenu("Restore")]
	public void Restore()
	{
		if (string.IsNullOrEmpty(spriteName) == false && string.IsNullOrEmpty(atlasName) == false)
		{
			Image image = this.GetComponent<Image>();
			if (image != null)
			{
				image.sprite = LoadSprite(spriteName, atlasName);
			}

			SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
			if (sr != null)
			{
				sr.sprite = LoadSprite(spriteName, atlasName);
			}
		}
	}

	Sprite LoadSprite(string spriteName, string atlasName)
	{
		GameObject atlasObj = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/ResourcesRaw/Atlas/{0}.prefab", atlasName));
		UGUISpriteAtlas atlas = atlasObj.GetComponent<UGUISpriteAtlas>();
		atlas.Init();
		return atlas.spriteDict[spriteName];
	}
	#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGUISpriteAtlas : MonoBehaviour
{
    public List<Sprite> spriteList;
    public Dictionary<string, Sprite> spriteDict;

    public void Init()
    {
        if (spriteDict == null)
        {
            spriteDict = new Dictionary<string, Sprite>(spriteList.Count);
            foreach (Sprite sprite in spriteList)
            {
                spriteDict.Add(sprite.name, sprite);
            }
        }
    }
}

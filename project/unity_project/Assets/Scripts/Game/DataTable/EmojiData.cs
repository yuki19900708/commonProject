using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class EmojiData
{
    /// <summary>编号</summary>
    public int id;
    /// <summary>含义</summary>
    public string des;


    public EmojiData Clone()
    {
        return (EmojiData)this.MemberwiseClone();
    }
}
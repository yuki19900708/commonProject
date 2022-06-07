using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class TextData
{
    /// <summary>编号</summary>
    public int id;
    /// <summary>中文</summary>
    public string CN;
    /// <summary>英文</summary>
    public string EN;


    public TextData Clone()
    {
        return (TextData)this.MemberwiseClone();
    }
}
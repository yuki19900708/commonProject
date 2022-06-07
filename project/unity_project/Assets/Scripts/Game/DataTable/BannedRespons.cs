using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class BannedRespons
{
    /// <summary>编号</summary>
    public int id;
    /// <summary>反馈文本</summary>
    public int content;


    public BannedRespons Clone()
    {
        return (BannedRespons)this.MemberwiseClone();
    }
}
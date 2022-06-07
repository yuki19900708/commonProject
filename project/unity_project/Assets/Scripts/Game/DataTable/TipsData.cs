using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class TipsData
{
    /// <summary>关卡编号</summary>
    public int id;
    /// <summary>标题</summary>
    public int title;
    /// <summary>ICON</summary>
    public string[] icon;
    /// <summary>文本列表</summary>
    public int[] desList;


    public TipsData Clone()
    {
        return (TipsData)this.MemberwiseClone();
    }
}
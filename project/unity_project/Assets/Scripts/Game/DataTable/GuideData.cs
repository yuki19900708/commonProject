using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class GuideData
{
    /// <summary>ID</summary>
    public int key;
    /// <summary>ID</summary>
    public int Index;
    /// <summary>教程分区间</summary>
    public int Section;
    /// <summary>是否强制</summary>
    public int Force;
    /// <summary>全屏黑幕</summary>
    public int ScreenMask;
    /// <summary>描述信息</summary>
    public int Msg;
    /// <summary>标题</summary>
    public int Title;
    /// <summary>进度</summary>
    public int Schedule;
    /// <summary>图片</summary>
    public string Icon;


    public GuideData Clone()
    {
        return (GuideData)this.MemberwiseClone();
    }
}
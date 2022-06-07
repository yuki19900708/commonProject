using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class TutorialTableData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>初始化或者重连时应该跳到的教程步骤</summary>
    public int skipIndex;
    /// <summary>该步引导触发时继续进行的步数</summary>
    public int nextStepCount;
    /// <summary>该步引导触发时回滚的步数</summary>
    public int previousStepCount;
    /// <summary>教程分区间</summary>
    public int section;
    /// <summary>是否强制</summary>
    public bool force;
    /// <summary>全屏黑幕</summary>
    public int screenMask;
    /// <summary>描述信息</summary>
    public int speakIndex;
    /// <summary>标题</summary>
    public int title;
    /// <summary>进度</summary>
    public string Schedule;
    /// <summary>图片</summary>
    public string icon;


    public TutorialTableData Clone()
    {
        return (TutorialTableData)this.MemberwiseClone();
    }
}
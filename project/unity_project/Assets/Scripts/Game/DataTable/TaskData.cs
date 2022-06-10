using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class TaskData
{
    /// <summary>任务编号</summary>
    public int id;
    /// <summary>任务名称</summary>
    public int name;
    /// <summary>对应关卡</summary>
    public int level;
    /// <summary>投放口</summary>
    public int showPos;
    /// <summary>序号</summary>
    public int index;
    /// <summary>后置任务</summary>
    public int nextTask;
    /// <summary>目标类型</summary>
    public int targetType;
    /// <summary>任务对象</summary>
    public int tragetIndex;
    /// <summary>对象标签</summary>
    public int targetTag;
    /// <summary>对象文本</summary>
    public int tagText;
    /// <summary>任务计数</summary>
    public int count;
    /// <summary>跳过成本</summary>
    public int skipNeed;
    /// <summary>任务奖励</summary>
    /// <summary>任务ICON</summary>
    public string taskIcon;
    /// <summary>描述文本</summary>
    public int describe;
    /// <summary>帮助文本</summary>
    public int helpText;


    public TaskData Clone()
    {
        return (TaskData)this.MemberwiseClone();
    }
}
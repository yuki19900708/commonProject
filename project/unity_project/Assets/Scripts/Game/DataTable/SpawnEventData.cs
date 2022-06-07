using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class SpawnEventData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>产出列表</summary>
    public OutputData outputItems;
    /// <summary>产出数量</summary>
    public int[] count;
    /// <summary>衍生循环充能次数</summary>
    public int cooldown;
    /// <summary>衍生间隔时间区间</summary>
    public int[] interval;
    /// <summary>衍生半径</summary>
    public int radius;
    /// <summary>衍生显示进度条</summary>
    public bool showProgressBar;


    public SpawnEventData Clone()
    {
        return (SpawnEventData)this.MemberwiseClone();
    }
}
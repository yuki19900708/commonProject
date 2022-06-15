using UnityEngine;
using System.Collections.Generic; 
[System.Serializable]
public class MapGridGameData
{
    /// <summary>X轴位置</summary>
    public int x;
    /// <summary>Y轴位置</summary>
    public int y;

    /// <summary>各自索引 x + y * width</summary>
    public int gridIndex;

    /// <summary>物件块编号 </summary>
    public int entityId;

    /// <summary>土地状态 </summary>
    public TerrainState state;

}

public enum TerrainState
{
    Locked,
    Opened
}

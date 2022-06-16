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
    Blank = 0, //空格子，即格子上什么也没有，无法进行任何操作；
    Locked,//锁定的格子，有地块，但无法进行操作，后续根据版本该格子可能开放
    Opened//开放的格子，有地块，可以进行确权等一系列操作。
}

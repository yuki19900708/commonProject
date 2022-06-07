using UnityEngine;
using System.Collections.Generic; 
[System.Serializable]
public class MapGridGameData
{
    /// <summary>X轴位置</summary>
    public int x;
    /// <summary>Y轴位置</summary>
    public int y;
    /// <summary>是否有地形块</summary>
    public int hasTerrain;
    /// <summary>是否有草地块号</summary>
    public int hasVegetation;
    /// <summary>草地的Hue值</summary>
    public int vegetationHue;
    /// <summary>物件块编号</summary>
    public int entityId;
    /// <summary>净化块编号</summary>
    public int purificationLevel;
    /// <summary>是否是被封锁</summary>
    public int sealLockId;

}

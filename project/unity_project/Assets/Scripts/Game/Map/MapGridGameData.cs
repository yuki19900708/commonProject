using UnityEngine;
using System.Collections.Generic; 
[System.Serializable]
public class MapGridGameData
{
    /// <summary>X轴位置</summary>
    public int x;
    /// <summary>Y轴位置</summary>
    public int y;
    /// <summary>是否有草地块号</summary>
    public int hasVegetation;
    /// <summary>草地的Hue值</summary>
    public int vegetationHue;


}

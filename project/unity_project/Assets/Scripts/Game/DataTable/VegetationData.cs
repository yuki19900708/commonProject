using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class VegetationData
{
    /// <summary>草皮编号</summary>
    public int id;
    /// <summary>草皮类型</summary>
    public int type;
    /// <summary>类型备注</summary>
    public string des;
    /// <summary>占地</summary>
    public int[] area;

    public VegetationData Clone()
    {
        return (VegetationData)this.MemberwiseClone();
    }
}
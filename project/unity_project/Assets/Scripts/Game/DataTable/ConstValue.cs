using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class ConstValue
{
    /// <summary>常量编号</summary>
    public int id;
    /// <summary>数值</summary>
    public int value;


    public ConstValue Clone()
    {
        return (ConstValue)this.MemberwiseClone();
    }
}
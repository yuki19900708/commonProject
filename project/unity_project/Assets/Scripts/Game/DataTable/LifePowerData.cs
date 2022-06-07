using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class LifePowerData
{
    /// <summary>ID</summary>
    public int id;
    /// <summary>净化之力</summary>
    public int value;


    public LifePowerData Clone()
    {
        return (LifePowerData)this.MemberwiseClone();
    }
}
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class VariableData
{
    /// <summary>变量ID</summary>
    public int id;
    /// <summary>变量名称</summary>
    public string name;


    public VariableData Clone()
    {
        return (VariableData)this.MemberwiseClone();
    }
}
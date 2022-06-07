using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class RandomNameData
{
    /// <summary>编号</summary>
    public int id;
    /// <summary>类型</summary>
    public int type;
    /// <summary>前缀</summary>
    public string firstName;
    /// <summary>名称</summary>
    public string lastName;


    public RandomNameData Clone()
    {
        return (RandomNameData)this.MemberwiseClone();
    }
}
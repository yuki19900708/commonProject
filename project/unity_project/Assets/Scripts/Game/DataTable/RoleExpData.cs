using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class RoleExpData
{
    /// <summary>等级</summary>
    public int id;
    /// <summary>所需经验</summary>
    public int exp;


    public RoleExpData Clone()
    {
        return (RoleExpData)this.MemberwiseClone();
    }
}
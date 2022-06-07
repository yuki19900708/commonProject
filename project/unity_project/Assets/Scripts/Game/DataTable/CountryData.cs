using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class CountryData
{
    /// <summary>序号</summary>
    public int id;
    /// <summary>国家名</summary>
    public string country;


    public CountryData Clone()
    {
        return (CountryData)this.MemberwiseClone();
    }
}
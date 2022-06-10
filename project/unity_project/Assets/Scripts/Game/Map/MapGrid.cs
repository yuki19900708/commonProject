using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MapGrid
{
    public Point point;


    public int entityId;
    public int vegetationHue = 0;
    public int vegetationId = 0;


    private MapObject vegetation;
    public MapObject Vegetation
    {
        get
        {
            return vegetation;
        }
        set
        {
            vegetation = value;
            if (vegetation != null)
            {
                vegetation.VegetationId = vegetationId;
            }
        }
    }

}

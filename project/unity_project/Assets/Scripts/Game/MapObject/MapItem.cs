using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour, ITileIndexSetter
{
    const string GRASS = "grass";

    private int vegetationId;
    private VegetationData vegetationData;

    private List<MapGrid> staticMapGridList = new List<MapGrid>();

    private Point staticPos;

    public Point StaticPos
    {
        get
        {
            return staticPos;
        }
        set
        {
            staticPos = value;
        }
    }

    public List<MapGrid> StaticMapGridList
    {
        get
        {
            return staticMapGridList;
        }
    }
  
    public int VegetationId
    {
        get
        {
            return vegetationId;
        }
        set
        {
            vegetationId = value;
            VegetationData = TableDataMgr.GetSingleVegetationData(vegetationId);
        }
    }

    public VegetationData VegetationData
    {
        get
        {
            return vegetationData;
        }
        set
        {
            vegetationData = value;
            if (vegetationData == null)
            {
                Debug.LogError("致命错误 === 没有这种草皮！！!" + vegetationId);
                return;
            }
            string prefix = "";
            SpriteRenderer[] renderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer re in renderers)
            {
                string spName = re.sprite.name;
                if (spName.Contains(prefix))
                {
                    continue;
                }

                if (spName.Contains(GRASS))
                {
                    spName = spName.Replace(GRASS, prefix);
                    if (EditorTerrainModel.TerrainEditorModel.IsRunMapEditor)
                    {
                        re.sprite = TerrainEditorUICtrl.GetSprite(spName, re.sprite);
                    }
                    else
                    {
                        re.sprite = UGUISpriteAtlasMgr.LoadSprite(spName);
                    }
                    continue;
                }


            }
        }
    }

    
    public virtual void SetTileIndex(int tileIndex)
    {

    }
}

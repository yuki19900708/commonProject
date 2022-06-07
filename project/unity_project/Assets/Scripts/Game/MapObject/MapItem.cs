using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour, ITileIndexSetter
{
    const string GRASS = "grass";
    const string DRYGRASS = "dry";
    const string SAND = "sand";
    const string SONW = "sonw";
    const string EVIL = "evil";
    const string PURCHASE = "purchase";
    const string REMAINS = "remains";
    const string JUNGLE = "jungle";

    public Action Event_StaticPosRefresh;

    private int hue = 0;

    /// <summary>
    /// 正常地块
    /// </summary>
    private bool isTerrain = false;

    /// <summary>
    /// 桥属于地的一种 但是和地的区别是 
    /// 1.桥不能被作为龙的封印目标
    /// 2.桥上不能摆放任何物体
    /// 3.桥所在的格子始终处于Lock状态 不可以被解锁
    /// </summary>
    private bool isBridge = false;

    private bool isVegetation = false;
    private int vegetationId;
    private VegetationData vegetationData;

    private List<MapGrid> staticMapGridList = new List<MapGrid>();

    private bool isEntity = false;

    private bool isSealLock = false;

    private Point staticPos;

    public int Hue
    {
        get { return hue; }
        set { hue = value; }
    }

    public Point StaticPos
    {
        get
        {
            return staticPos;
        }
        set
        {
            staticPos = value;
            if (Event_StaticPosRefresh != null)
            {
                Event_StaticPosRefresh();
            }
        }
    }

    public List<MapGrid> StaticMapGridList
    {
        get
        {
            return staticMapGridList;
        }
    }

    public Point InstantPos
    {
        get
        {
            return MapMgr.Instance.terrainMap.WorldPosition2Coordinate(transform.position);
        }
    }

    public bool IsSealLock
    {
        get { return isSealLock; }
        set { isSealLock = value; }
    }

    public bool IsEntity
    {
        get
        {
            return isEntity;
        }
        set
        {
            isEntity = value;
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
            switch (vegetationData.type)
            {
                case 1:
                    prefix = GRASS;
                    break;
                case 2:
                    prefix = DRYGRASS;
                    break;
                case 3:
                    prefix = SAND;
                    break;
                case 4:
                    prefix = SONW;
                    break;
                case 5:
                    prefix = PURCHASE;
                    break;
                case 6:
                    prefix = REMAINS;
                    break;
                case 7:
                    prefix = JUNGLE;
                    break;
                case 8:
                    prefix = EVIL;
                    break;
            }
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

                if (spName.Contains(DRYGRASS))
                {
                    spName = spName.Replace(DRYGRASS, prefix);
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

                if (spName.Contains(SAND))
                {
                    spName = spName.Replace(SAND, prefix);
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

                if (spName.Contains(SONW))
                {
                    spName = spName.Replace(SONW, prefix);
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

                if (spName.Contains(EVIL))
                {
                    spName = spName.Replace(EVIL, prefix);
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
                if (spName.Contains(PURCHASE))
                {
                    spName = spName.Replace(PURCHASE, prefix);
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
                if (spName.Contains(REMAINS))
                {
                    spName = spName.Replace(REMAINS, prefix);
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
                if (spName.Contains(JUNGLE))
                {
                    spName = spName.Replace(JUNGLE, prefix);
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

    public bool IsVegetation
    {
        get
        {
            return isVegetation;
        }
        set
        {
            isVegetation = value;
        }
    }

    public bool IsBridge
    {
        get { return isBridge; }
        set { isBridge = value; }
    }

    public bool IsTerrain
    {
        get
        {
            return isTerrain;
        }
        set
        {
            isTerrain = value;
        }
    }

    /// <summary>
    /// 物品所包含的格子是否全部是净化过的
    /// </summary>
    public bool IsPurified
    {
        get
        {
            if (StaticMapGridList != null && StaticMapGridList.Count > 0)
            {
                foreach (MapGrid value in StaticMapGridList)
                {
                    if (value.Status != MapGridState.UnlockAndCured)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 战利品球是否全部是净化过的
    /// </summary>
    public bool IsTrophyBallPurified
    {
        get
        {
            if (StaticMapGridList != null && StaticMapGridList.Count > 0)
            {
                foreach (MapGrid value in StaticMapGridList)
                {
                    if (value.Status != MapGridState.UnlockAndCured)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        }
    }

    public virtual void SetTileIndex(int tileIndex)
    {

    }
}

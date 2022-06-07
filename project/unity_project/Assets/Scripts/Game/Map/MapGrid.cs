using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MapGridState
{
    Locked,
    UnlockButDead,
    UnlockAndCured,
}

[Serializable]
public class MapGrid
{
    /// <summary>
    /// 游戏运行过程中 MapGrid 的状态变更事件
    /// </summary>
    public Action<MapGrid, MapGridState, MapGridState> Event_GameRuningMapGridStatusChanged;

    public Point point;

    private MapGridState status;

    public DeadLandData deadLandData;

    public int entityId;
    public int vegetationHue = 0;
    public int vegetationId = 0;

    private int curPurificationValue;

    private MapObject wall;

    private MapObject terrain;

    private MapObject vegetation;

    private MapObject entity;

    public MapObject sealLock;

    public MapGridState Status
    {
        get
        {
            return status;
        }
        set
        {
            MapGridState lastState = status;
            status = value;
            if (Event_GameRuningMapGridStatusChanged != null)
            {
                Event_GameRuningMapGridStatusChanged(this, lastState, value);
            }
        }
    }


    public int CurPurificationValue
    {
        get
        {
            return curPurificationValue;
        }
        set
        {
            curPurificationValue = value;
            if (deadLandData != null && curPurificationValue >= deadLandData.pureNeed1)
            {
                //VFXMgr.PlayDeadLandCuredVFX(this);
                SetStatus(MapGridState.UnlockAndCured);
            }
        }
    }

    public MapObject Wall
    {
        get
        {
            return wall;
        }
        set
        {
            wall = value;
        }
    }

    public MapObject Terrain
    {
        get
        {
            return terrain;
        }
        set
        {
            terrain = value;
        }
    }

    public MapObject Entity
    {
        get
        {
            return entity;
        }
        set
        {
            entity = value;
        }
    }

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
                vegetation.Hue = vegetationHue;
                vegetation.VegetationId = vegetationId;
            }
        }
    }

    public void SetStatus(MapGridState status)
    {
        Status = status;
        switch (this.Status)
        {
            case MapGridState.Locked:
                if (wall != null)
                {
                    wall.DisplayAsLock();
                }

                if (terrain != null)
                {
                    terrain.DisplayAsLock();
                }

                if (vegetation != null)
                {
                    vegetation.DisplayAsLock();
                }

                if (entity != null && entity.StaticMapGridList[0] == this)
                {
                    entity.DisplayAsLock();
                }

                if (sealLock != null)
                {
                    sealLock.DisplayAsLock();
                }
                break;
            case MapGridState.UnlockAndCured:
                if (wall != null)
                {
                    wall.DisplayAsUnLockAndCured();
                }

                if (terrain != null)
                {
                    terrain.DisplayAsUnLockAndCured();
                }

                if (vegetation != null)
                {
                    vegetation.DisplayAsUnLockAndCured();
                }

                if (entity != null)
                {
                    entity.DisplayAsUnLockAndCured();
                }

                if (sealLock != null)
                {
                    sealLock.DisplayAsUnLockAndCured();
                }
                deadLandData = null;
                break;
            case MapGridState.UnlockButDead:
                if (wall != null)
                {
                    wall.DisplayAsUnlockButDead(deadLandData);
                }

                if (terrain != null)
                {
                    terrain.DisplayAsUnlockButDead(deadLandData);
                }

                if (vegetation != null)
                {
                    vegetation.DisplayAsUnlockButDead(deadLandData);
                }

                if (entity != null)
                {
                    entity.DisplayAsUnlockButDead(deadLandData);
                }

                if (sealLock != null)
                {
                    sealLock.DisplayAsUnlockButDead(deadLandData);
                }
                break;
        }
    }
}

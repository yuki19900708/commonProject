using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneObjectTestItem : MonoBehaviour {
    public int x;
    public int y;
    private int terrainState = 0;
    private int checkeState = 0;
    public int TerrainState
    {
        get
        {
            return terrainState;
        }

        set
        {
            terrainState = value;
            Init();
        }
    }

    public int CheckeState
    {
        get
        {
            return checkeState;
        }

        set
        {
            checkeState = value;
            Init();
        }
    }

    public void Init()
    {
        if (TerrainState == 0)
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }
        else if (TerrainState == 1)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (TerrainState == 2)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        if (CheckeState == 1)
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }
        else if (CheckeState == 2)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}

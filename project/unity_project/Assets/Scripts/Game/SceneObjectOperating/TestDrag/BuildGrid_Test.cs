using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildGrid_Test : MonoBehaviour {
    public InputField inputOne;
    public InputField inputTwo;
    public static BuildGrid_Test Instance;
    public GameObject Predef;
    public SceneObjectTestItem[,] testGameObjectArray = new SceneObjectTestItem[20, 20];
    public void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                GameObject a = Instantiate<GameObject>(Predef);
                int value = Random.Range(0, 100);
                if (value < 65)
                {
                    a.GetComponent<SceneObjectTestItem>().TerrainState = 2;
                }
                else if (value < 80)
                {
                    a.GetComponent<SceneObjectTestItem>().TerrainState = 1;
                }
                else
                {
                    a.GetComponent<SceneObjectTestItem>().TerrainState = 0;
                }
                a.GetComponent<SceneObjectTestItem>().x = i;
                a.GetComponent<SceneObjectTestItem>().y = j;
                a.transform.SetParent(this.transform);
                a.transform.localPosition = new Vector3(i * 0.84f, j * 0.84f, 0);
                a.transform.localRotation = Quaternion.Euler(Vector3.zero);
                testGameObjectArray[i, j] = a.GetComponent<SceneObjectTestItem>();
            }
        }

        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (Random.Range(0, 100) > 70)
                {
                    mgsArray[i, j] = 1;
                }
                else
                {
                    mgsArray[i, j] = 0;
                }
            }
        }
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                CArray[i, j] = 0;
            }
        }
    }

    /// <summary>
    /// 获取这个地形是否可以放入物体
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CheckObjectPostion(int x,int y)
    {
        if (testGameObjectArray[x, y].TerrainState == 2 || testGameObjectArray[x, y].TerrainState == 1)
        {
            return true;
        }
        return false;
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    testGameObjectArray[i, j].CheckeState = mgsArray[i, j];
                }
            }
        }
    }
    public void CheckButtonClick()
    {
        string[] a = inputOne.text.Split(',');
        Type = (FindObejctShapeEnum)int.Parse(inputTwo.text);
        List<Vector2> ceshilst = new List<Vector2>();
         ceshilst.Add(new Vector2(int.Parse(a[0]), int.Parse(a[1])));
        FindIt = false;
        FindAllPostion(ceshilst);
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                 CArray[i, j] = 0;
            }
        }
    }
    #region 寻找可放入的位置
    #region 测试数组
    /// <summary>
    /// 地形数据
    /// </summary>
    public int[,] mgsArray = new int[20, 20];
    /// <summary>
    /// 检查数据
    /// </summary>
    public int[,] CArray = new int[20, 20];
    #endregion
    #region 检查区域
    Vector2[] checkArray= new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(1, 0) }; 
    #endregion
    public enum FindObejctShapeEnum
    {
        Shape1_1 = 0,
        Shape1_2,
        Shape2_1,
        Shape2_2,
    }
    public static Vector2 FindVector2;
    /// <summary>
    /// 是否找到
    /// </summary>
    public static bool FindIt=false;
    /// <summary>
    /// 查找类型
    /// </summary>
    public static FindObejctShapeEnum Type = FindObejctShapeEnum.Shape1_1;
    /// <summary>
    /// 获取到的结果
    /// </summary>
    public static List<Vector2> allFindPostion = new List<Vector2>();

    /// <summary>
    /// 开始一次查找
    /// </summary>
    /// <param name="startPoint"></param>
    public void FindAllPostion(List<Vector2> startPoint)
    {
        List<Vector2> allList = new List<Vector2>();
        for (int i=0;i< startPoint.Count;i++)
        {
            List<Vector2> onelist = Extrusion(startPoint[i]);
            for (int j = 0; j < onelist.Count; j++)
            {
                allList.Add(onelist[j]);
            }
        }
        if (allList.Count != 0)
        {
            FindAllPostion(allList);
        }
    }


    public List<Vector2> Extrusion(Vector2 Postion)
    {
        List<Vector2> findlist = new List<Vector2>();
        if (FindIt)
        {
            return findlist;
        }
        ////TODO  判断所有在该位置的物品
        ////TODO  获取 物品占用的位置信息
        ////TODO 逐步遍历寻找位置
        int pos_x = (int)Postion.x;
        int pos_y = (int)Postion.y;

        int tagertPos_x = 0;
        int tagertPos_y = 0;
        for (int i = 0; i < checkArray.Length; i++)
        {
            tagertPos_x = (int)(checkArray[i].x + pos_x);
            tagertPos_y = (int)(checkArray[i].y + pos_y);
            if (tagertPos_x >= 0 && tagertPos_x < 20 && tagertPos_y >= 0 && tagertPos_y < 20)
            {
                //如果此位置被占用那么返回
                if (CArray[tagertPos_x, tagertPos_y] == 3)
                {
                    continue;
                }
                if (FindPostion(tagertPos_x, tagertPos_y, Type))
                {
                    FindVector2 = new Vector2(tagertPos_x, tagertPos_y);
                    FindIt = true;
                }
                else
                {
                    if (CArray[tagertPos_x, tagertPos_y] == 0)
                    {
                        CArray[tagertPos_x, tagertPos_y] = 3;
                    }
                    findlist.Add(new Vector2(tagertPos_x, tagertPos_y));
                }
            }
        }
        return findlist;
    }

    bool FindPostion(int x, int y, FindObejctShapeEnum type)
    {
        switch (type)
        {
            case FindObejctShapeEnum.Shape1_1:
                return FindPostion1_1(x, y);
            case FindObejctShapeEnum.Shape1_2:
                return FindPostion1_2(x, y);
            case FindObejctShapeEnum.Shape2_1:
                return FindPostion2_1(x, y);
            case FindObejctShapeEnum.Shape2_2:
                return FindPostion2_2(x, y);
        }
        return false;
    }

    /// <summary>
    /// 查找1*1
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="type"></param>
    bool FindPostion1_1(int x, int y)
    {
        if (JugeRand(x, y))
        {
            mgsArray[x, y] = 2;
            allFindPostion.Clear();
            allFindPostion.Add(new Vector2(x, y));
            return true;
        }
        return false;
    }

    /// <summary>
    /// 查找1*2 x
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="type"></param>
    bool FindPostion1_2(int x, int y)
    {
        if (JugeRand(x, y) && JugeRand(x+1, y))
        {
            mgsArray[x, y] = 2;
            mgsArray[x + 1, y] = 2;
            allFindPostion.Clear();
            allFindPostion.Add(new Vector2(x, y));
            allFindPostion.Add(new Vector2(x + 1, y));
            return true;
        }
        else if (JugeRand(x, y) && JugeRand(x-1, y))
        {
            mgsArray[x, y] = 2;
            mgsArray[x-1, y] = 2;
            allFindPostion.Clear();
            allFindPostion.Add(new Vector2(x, y));
            allFindPostion.Add(new Vector2(x - 1, y));
            return true;
        }
        return false;
    }

    /// <summary>
    /// 查找2*1 x
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="type"></param>
    bool FindPostion2_1(int x, int y)
    {
        if (JugeRand(x, y) && JugeRand(x, y-1))
        {
            mgsArray[x, y] = 2;
            mgsArray[x, y - 1] = 2;
            allFindPostion.Clear();
            allFindPostion.Add(new Vector2(x, y));
            allFindPostion.Add(new Vector2(x, y - 1));
            return true;
        }
        else if (JugeRand(x, y) && JugeRand(x, y+1))
        {
            mgsArray[x, y] = 2;
            mgsArray[x, y + 1] = 2;
            allFindPostion.Clear();
            allFindPostion.Add(new Vector2(x, y));
            allFindPostion.Add(new Vector2(x, y + 1));
            return true;
        }
        return false;
    }

    /// <summary>
    /// 查找2*2 x
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="type"></param>
    bool FindPostion2_2(int x, int y)
    {
        if (JugeRand(x,y) && JugeRand(x, y + 1)&& JugeRand(x + 1, y + 1) && JugeRand(x + 1, y) )
        {
            mgsArray[x, y] = 2;
            mgsArray[x, y + 1] = 2;
            mgsArray[x + 1, y + 1] = 2;
            mgsArray[x + 1, y] = 2;
            allFindPostion.Clear();
            allFindPostion.Add(new Vector2(x, y));
            allFindPostion.Add(new Vector2(x, y + 1));
            allFindPostion.Add(new Vector2(x + 1, y + 1));
            allFindPostion.Add(new Vector2(x + 1, y));
            return true;
        }
        else if (JugeRand(x, y)&& JugeRand(x - 1, y) && JugeRand(x - 1, y + 1) && JugeRand(x, y + 1))
        {
            mgsArray[x, y] = 2;
            mgsArray[x - 1, y] = 2;
            mgsArray[x - 1, y + 1] = 2;
            mgsArray[x, y + 1] = 2;
            allFindPostion.Clear();
            allFindPostion.Add(new Vector2(x, y));
            allFindPostion.Add(new Vector2(x - 1, y));
            allFindPostion.Add(new Vector2(x - 1, y + 1));
            allFindPostion.Add(new Vector2(x, y + 1));
            return true;
        }
        else if (JugeRand(x, y) && JugeRand(x + 1, y)  && JugeRand(x + 1, y - 1) && JugeRand(x, y - 1))
        {
            mgsArray[x, y] = 2;
            mgsArray[x + 1, y] = 2;
            mgsArray[x + 1, y - 1] =2;
            mgsArray[x, y - 1] = 2;
            allFindPostion.Clear();
            allFindPostion.Add(new Vector2(x, y));
            allFindPostion.Add(new Vector2(x + 1, y));
            allFindPostion.Add(new Vector2(x + 1, y - 1));
            allFindPostion.Add(new Vector2(x, y - 1));
            return true;
        }
        else if (JugeRand(x, y) && JugeRand(x - 1, y)  && JugeRand(x - 1, y - 1) && JugeRand(x, y - 1))
        {
            mgsArray[x, y] = 2;
            mgsArray[x - 1, y] = 2;
            mgsArray[x - 1, y - 1] =2;
            mgsArray[x, y - 1] =2;
            allFindPostion.Clear();
            allFindPostion.Add(new Vector2(x, y));
            allFindPostion.Add(new Vector2(x - 1, y));
            allFindPostion.Add(new Vector2(x - 1, y - 1));
            allFindPostion.Add(new Vector2(x, y - 1));
            return true;
        }
        return false;
    }

    public bool JugeRand(int x,int y)
    {
        if(x>=0&&x<20&& y>=0&&y<20)
        {
            if (mgsArray[x, y] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    #endregion
}

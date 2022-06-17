
using UnityEngine;

public class MapObject : MonoBehaviour
{
    [SerializeField]
    private int vegetationId;

    public int VegetationId
    {
        get
        {
            return vegetationId;
        }
    }

    public int xPos;
    public int yPos;

    private SpriteRenderer sprite;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (this.transform.childCount > 0)
            {
                this.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public int gridIndex;

    private VegetationData vegetationData = null;

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
            }
        }
    }

    public int[] GetArea()
    {
        VegetationData = TableDataMgr.GetSingleVegetationData(vegetationId);
        if (VegetationData != null)
        {
            return VegetationData.area;
        }
        int[] a = new int[2] { 1, 1 };
        return a;
    }

    public void ChangeColor()
    {
        if(sprite == null)
        {
            sprite = this.GetComponent<SpriteRenderer>();
        }
        sprite.color = Color.black;

    }
}

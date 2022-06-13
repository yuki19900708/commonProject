
using UnityEngine;

public class MapObject : MonoBehaviour, ITileIndexSetter
{
    [SerializeField]
    private int vegetationId;
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


    public virtual void SetTileIndex(int tileIndex)
    {

    }

    public int[] GetArea()
    {
        VegetationData = TableDataMgr.GetSingleVegetationData(vegetationId);
        if (VegetationData != null)
        {
            return VegetationData.area;
        }
        int[] a = new int[2] {1,1 };
        return a;
    }
}

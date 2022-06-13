
using UnityEngine;

public class MapObject : MonoBehaviour, ITileIndexSetter
{
    private int vegetationId;
    private VegetationData vegetationData;
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
            }
        }
    }


    public virtual void SetTileIndex(int tileIndex)
    {

    }
}

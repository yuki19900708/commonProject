using UnityEngine;

public class SortingOrderTag : MonoBehaviour
{
    public int sortingOrder;
    public float zOrder;
    public string objectName;
    public string parentObjectName; //为了优化指定游戏GC Alloc而做的特定代码，不具有通用性

    private void Awake()
    {
        objectName = this.name;
        if(this.transform.parent != null)
        {
            parentObjectName = this.transform.parent.name;
        }
    }
}

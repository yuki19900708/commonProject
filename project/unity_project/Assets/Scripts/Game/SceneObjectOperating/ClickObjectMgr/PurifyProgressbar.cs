using UnityEngine;
using System.Collections.Generic;
//using TMPro;

public class PurifyProgressbar : MonoBehaviour
{
    private static MonoBehaviourPool<PurifyProgressbar> pool;
    private static Dictionary<Point, PurifyProgressbar> displayInfoDict = new Dictionary<Point, PurifyProgressbar>();
    public static MonoBehaviourPool<PurifyProgressbar> Pool
    {
        get { return pool; }
    }

    public static Dictionary<Point, PurifyProgressbar> DisplayInfoDict
    {
        get { return displayInfoDict; }
    }

    public APSlider slider;
    //public TextMeshProUGUI tipText;
    //public TextMeshProUGUI progressText;

    private void Awake()
    {
        //tipText.text = L10NMgr.GetText(22900013);
    }

    /// <summary>
    /// 当前出现的位置
    /// </summary>
    private Point showPoint;
    /// <summary>
    /// 显示计时器
    /// </summary>
    private int timerID;

    public static void Show(MapGrid grid, int from, int to, int max)
    {
        if (pool == null)
        {
            pool = new MonoBehaviourPool<PurifyProgressbar>(ResMgr.Load<PurifyProgressbar>("PurifyProgressbar"), CommonObjectMgr.PoolParent);
        }
        PurifyProgressbar instance = null;
        if (displayInfoDict.ContainsKey(grid.point))
        {
            instance = displayInfoDict[grid.point];
            TimerMgr.Reset(instance.timerID);
        }
        else
        {
            Vector3 worldPos = MapMgr.Instance.entityMap.Coordinate2WorldPosition(grid.point);
            worldPos.y += 1.7f;

            instance = pool.GetInstance();
            instance.transform.SetParent(CommonObjectMgr.PurifyProgressParent, false);
            instance.transform.position = worldPos;
            instance.showPoint = grid.point;
            instance.timerID = TimerMgr.OpenOneTimerWithSecond(1, instance.Hide);
            displayInfoDict.Add(grid.point, instance);
        }

        if (from == to)
        {
            instance.SetValue(to, max, false);
        }
        else
        {
            instance.SetValue(from, max, false);
            instance.SetValue(to, max, true);
        }
    }

    public void TimerRemove()
    {
        TimerMgr.CloseOneTimer(timerID);
        pool.RecycleInstance(this);
    }

    private void Hide(object param)
    {
        displayInfoDict.Remove(showPoint);
        pool.RecycleInstance(this);
    }

    public void SetValue(int current, int max, bool hasAnimatin = false)
    {
        //progressText.text = current.ToString() + "/" + max.ToString();
        if (hasAnimatin == false)
        {
            slider.SetValueWithOutAnimation(current / (float)max);
        }
        else
        {
            slider.Value = current / (float)max;
        }
    }
}

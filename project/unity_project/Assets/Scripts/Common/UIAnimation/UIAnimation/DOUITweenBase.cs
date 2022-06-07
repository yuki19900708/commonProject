using DG.Tweening;
using UnityEngine;

public abstract class DOUITweenBase : MonoBehaviour
{
    public float duration = 1;
    public Ease ease = Ease.Linear;
    public bool userCruveEase = false;
    public AnimationCurve curveEase;
    public float delay = 0;
    public bool randomDelay;
    public Vector2 randomDelayRange;
    public int loopCount = -1;
    public LoopType loopType = LoopType.Yoyo;
}

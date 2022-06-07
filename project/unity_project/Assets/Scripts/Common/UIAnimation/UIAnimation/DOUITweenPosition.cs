using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DOUITweenPosition : DOUITweenBase
{
    public Vector3 start;
    public Vector3 end;

    public bool isLocal = false;
    
    private void OnEnable()
    {
        float delayTime = delay;
        if (randomDelay)
        {
            delayTime = Random.Range(randomDelayRange.x, randomDelayRange.y);
        }
        if (isLocal)
        {
            this.transform.localPosition = start;
            if (userCruveEase)
            {
                this.transform.DOLocalMove(end, duration).SetEase(curveEase).SetDelay(delayTime).SetLoops(loopCount, loopType);
            }
            else
            {
                this.transform.DOLocalMove(end, duration).SetEase(ease).SetDelay(delayTime).SetLoops(loopCount, loopType);
            }
        }
        else
        {
            this.transform.position = start;
            if (userCruveEase)
            {
                this.transform.DOMove(end, duration).SetEase(curveEase).SetDelay(delayTime).SetLoops(loopCount, loopType);
            }
            else
            {
                this.transform.DOMove(end, duration).SetEase(ease).SetDelay(delayTime).SetLoops(loopCount, loopType);
            }
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(this.transform);
    }
}

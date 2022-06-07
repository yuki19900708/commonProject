using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DOUITweenAlpha : DOUITweenBase
{
    public float start;
    public float end;

    public bool includeChildren = true;
    
    private void OnEnable()
    {
        float delayTime = delay;
        if (randomDelay)
        {
            delayTime = Random.Range(randomDelayRange.x, randomDelayRange.y);
        }
        Image[] imageTargets;
        if (includeChildren)
        {
            imageTargets = this.GetComponentsInChildren<Image>(true);
        }
        else
        {
            imageTargets = this.GetComponents<Image>();
        }
        foreach (Image target in imageTargets)
        {
            Color color = target.color;
            color.a = start;
            target.color = color;
            if (userCruveEase)
            {
                target.DOFade(end, duration).SetEase(curveEase).SetDelay(delayTime).SetLoops(loopCount, loopType);
            }
            else
            {
                target.DOFade(end, duration).SetEase(ease).SetDelay(delayTime).SetLoops(loopCount, loopType);
            }
        }

        SpriteRenderer[] srTargets;
        if (includeChildren)
        {
            srTargets = this.GetComponentsInChildren<SpriteRenderer>(true);
        }
        else
        {
            srTargets = this.GetComponents<SpriteRenderer>();
        }
        foreach (SpriteRenderer target in srTargets)
        {
            Color color = target.color;
            color.a = start;
            target.color = color;
            if (userCruveEase)
            {
                target.DOFade(end, duration).SetEase(curveEase).SetDelay(delayTime).SetLoops(loopCount, loopType);
            }
            else
            {
                target.DOFade(end, duration).SetEase(ease).SetDelay(delayTime).SetLoops(loopCount, loopType);
            }
        }
    }

    private void OnDisable()
    {
        Image[] imageTargets;
        if (includeChildren)
        {
            imageTargets = this.GetComponentsInChildren<Image>(true);
        }
        else
        {
            imageTargets = this.GetComponents<Image>();
        }
        foreach (Image target in imageTargets)
        {
            DOTween.Kill(target);
        }

        SpriteRenderer[] srTargets;
        if (includeChildren)
        {
            srTargets = this.GetComponentsInChildren<SpriteRenderer>(true);
        }
        else
        {
            srTargets = this.GetComponents<SpriteRenderer>();
        }
        foreach (SpriteRenderer target in srTargets)
        {
            DOTween.Kill(target);
        }
    }
}

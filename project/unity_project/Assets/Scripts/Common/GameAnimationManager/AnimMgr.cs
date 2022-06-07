using UnityEngine;
using DG.Tweening;
using System;
//using TMPro;

/// <summary>
/// 动画管理类
/// </summary>
public partial class AnimMgr : MonoBehaviour
{
    public static AnimMgr Instance;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 播放解锁关卡动画
    /// </summary>
    /// <param name="chapterItem"></param>
    /// <param name="endCallback"></param>
    //public static void PlayUnlockChapterAnimation(ChapterItem chapterItem, TweenCallback endCallback = null)
    //{
    //    Timer.AddDelayFunc(0.2f, () =>
    //    {
    //        chapterItem.lockedLogo.transform.localRotation = Quaternion.Euler(0, 0, -9);
    //        Sequence sequence = DOTween.Sequence();

    //        //缩放抖动阶段
    //        Tweener tweener1 = chapterItem.lockedLogo.transform.DORotate(Vector3.zero, 0.029f);
    //        Tweener tweener2 = chapterItem.lockedLogo.transform.DORotate(new Vector3(0, 0, -5), 0.029f);
    //        Tweener tweener3 = chapterItem.lockedLogo.transform.DORotate(new Vector3(0, 0, 4), 0.058f);
    //        Tweener tweener4 = chapterItem.lockedLogo.transform.DORotate(Vector3.zero, 0.056f);
    //        Tweener tweener5 = chapterItem.lockedLogo.transform.DOScale(Vector3.one * 1.2f, 0.258f).SetEase(Ease.OutBack);
    //        Tweener tweener6 = chapterItem.lockedLogo.transform.DOScale(Vector3.one * 0.8f, 0.029f);
    //        Tweener tweener7 = chapterItem.itemButton.transform.DOScale(Vector3.one * 1.07f, 0.029f);
    //        Tweener tweener8 = chapterItem.lockedLogo.transform.DOScale(Vector3.one, 0.058f);
    //        Tweener tweener9 = chapterItem.itemButton.transform.DOScale(Vector3.one * 1.05f, 0.058f);
    //        sequence.Append(tweener1);
    //        sequence.AppendInterval(0.287f);
    //        sequence.Append(tweener2);
    //        sequence.Append(tweener3);
    //        sequence.Append(tweener4);
    //        sequence.Append(tweener5);
    //        sequence.Append(tweener6);
    //        sequence.Join(tweener7);
    //        sequence.Append(tweener8);
    //        sequence.Join(tweener9);

    //        sequence.AppendCallback(()=> 
    //        {
    //            SoundMgr.PlayNormalCollection();
    //        });

    //        //翻转阶段
    //        Tweener tweener10 = chapterItem.itemButton.transform.DOScaleY(0.98f, 0.2f);
    //        Tweener tweener11 = chapterItem.itemButton.transform.DOScaleX(0.05f, 0.2f);
    //        Tweener tweener12 = chapterItem.lockedLogo.transform.DOScaleX(0.05f, 0.2f);
    //        sequence.Append(tweener10);
    //        sequence.Join(tweener11);
    //        sequence.Join(tweener12);
    //        sequence.AppendCallback(()=> {
    //            chapterItem.itemButton.image.sprite = UGUISpriteAtlasMgr.LoadSprite("chapter_item_bg");
    //            chapterItem.itemButton.transform.localScale = new Vector3(0.98f, 0.98f, 1);
    //            Color color = chapterItem.itemButton.image.color;
    //            color.a = 0.5f;
    //            chapterItem.itemButton.image.color = color;
    //            chapterItem.nameText.enabled = true;
    //            chapterItem.nameText.alpha = 0.5f;
    //            chapterItem.lockedLogo.enabled = false;

    //            if (chapterItem.TableData.challengeLevel)
    //            {
    //                chapterItem.starGameObject.SetActive(false);
    //                chapterItem.sliderGameObject.SetActive(true);
    //            }
    //            else
    //            {
    //                chapterItem.starGameObject.SetActive(chapterItem.TableData.taskList.Length > 0);
    //            }
    //            if (chapterItem.starGameObject.activeSelf)
    //            {
    //                for (int i = 0; i < chapterItem.starImages.Length; i++)
    //                {
    //                    chapterItem.starImages[i].transform.localScale = Vector3.zero;
    //                }
    //            }
    //        });

    //        //翻过来后阶段
    //        Tweener tweener16 = chapterItem.itemButton.transform.DOScale(Vector3.one, 0.116f);
    //        Tweener tweener17 = chapterItem.itemButton.image.DOFade(1, 0.116f);
    //        Tweener tweener18 = chapterItem.nameText.DOFade(1, 0.116f);

    //        sequence.Append(tweener16);
    //        sequence.Join(tweener17);
    //        sequence.Join(tweener18);

    //        bool canShowStar = false;

    //        if (chapterItem.TableData.challengeLevel)
    //        {
    //            canShowStar = false;
    //        }
    //        else
    //        {
    //            canShowStar = chapterItem.TableData.taskList.Length > 0;
    //        }

    //        if (canShowStar)
    //        {
    //            //星星动画阶段
    //            for (int i = 0; i < chapterItem.starImages.Length; i++)
    //            {
    //                Tweener tweener19 = chapterItem.starImages[i].transform.DOScale(Vector3.one, 0.116f);
    //                sequence.Append(tweener19);
    //            }
    //        }
    //        sequence.OnComplete(endCallback);
    //    });
    //}

    /// <summary>
    /// 播放抖动动画
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="endCallback"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static Tweener PlayShakeAnimation(Transform transform, float duration = 0.3f, float strength = 0.1f, int vibrato = 20)
    {
        if (DOTween.IsTweening(transform))
        {
            return null;
        }
        else
        {
            return transform.DOShakePosition(duration, strength, vibrato, 0);
        }
    }

    /// <summary>
    /// 播放呼吸动画
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="strength"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static Tweener PlayBreathAnimation(Transform transform, float strength = 1.15f,  float duration = 1f)
    {
        return transform.DOScale(Vector3.one * strength, duration).ChangeStartValue(Vector3.one).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// 播放悬浮动画
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="startValue"></param>
    /// <param name="distance"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static Tweener PlayFloatingAnimation(Transform transform, float startValue = 0, float distance = 0.1f, float duration = 3)
    {
        if (DOTween.IsTweening(transform))
        {
            return null;
        }
        else
        {
            Vector3 pos = transform.localPosition;
            pos.y = startValue;
            transform.localPosition = pos;
            return transform.DOLocalMoveY(startValue + distance, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }
    }

    /// <summary>
    /// 播放消失动画
    /// </summary>
    /// <returns></returns>
    public static Tweener PlayDisappearAnimation(Transform transform, float duration = 0.5f, bool forcePlay = false)
    {
        DOTween.Kill(transform);
        if (DOTween.IsTweening(transform) && forcePlay == false)
        {
            return null;
        }
        else
        {
            if (forcePlay)
            {
                DOTween.Kill(transform);
            }
            return transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack);
        }
    }

    /// <summary>
    /// 播放旋转动画
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="endCallback"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static Tweener PlayRotateAnimation(Transform transform, Vector3 rotatevalue, float duration = 0.1f,int loop = 1, RotateMode mode = RotateMode.Fast)
    {
        if (DOTween.IsTweening(transform))
        {
            return null;
        }
        else
        {
            return transform.DORotate(rotatevalue, duration, mode).SetLoops(loop);
        }
    }
}

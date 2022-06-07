using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ImageExpansion {

    public static void PlayImageFrameAnimation(this Image image,List<Sprite> spriteList,int frames,Action callback = null)
    {
        FrameAnimation frameAnimation = image.GetComponent<FrameAnimation>();
        if (frameAnimation == null)
        {
            frameAnimation = image.gameObject.AddComponent<FrameAnimation>();
        }
        frameAnimation.PlayFrameAnimation(image, spriteList, frames, callback);
    }

    public static void PlayImageFrameAnimationKeepSize(this Image image, List<Sprite> spriteList, int frames, Action callback = null)
    {
        FrameAnimation frameAnimation = image.GetComponent<FrameAnimation>();
        if (frameAnimation == null)
        {
            frameAnimation = image.gameObject.AddComponent<FrameAnimation>();
        }
        frameAnimation.PlayFrameAnimationKeepSize(image, spriteList, frames, callback);
    }
}

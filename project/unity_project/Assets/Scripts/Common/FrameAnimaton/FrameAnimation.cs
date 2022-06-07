using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameAnimation : MonoBehaviour {

    public event Action Event_AnimationOver; 
    public List<Sprite> spriteList = new List<Sprite>();
    public int frames;
    private Image animImage;
    private float animTime;
    private float timer;
    private float perSpriteTime;
    private int currentIndex;
    private bool isPlaying;
    private bool keepSize=false;
    public bool IsPlaying
    {
        get { return isPlaying; }
    }

    public void PlayFrameAnimation(Image animImage, List<Sprite> spriteList,int frames,Action callBack)
    {
        ResetMembers();
        Event_AnimationOver = callBack;
        this.animImage = animImage;
        this.frames = frames;
        this.spriteList.AddRange(spriteList);
        animTime = (float)frames / Application.targetFrameRate;
        perSpriteTime = animTime / spriteList.Count;
        animImage.sprite = spriteList[0];
        animImage.SetNativeSize();
        isPlaying = true;
    }

    public void PlayFrameAnimationKeepSize(Image animImage, List<Sprite> spriteList, int frames, Action callBack)
    {
        ResetMembers();
        Event_AnimationOver = callBack;
        this.animImage = animImage;
        this.frames = frames;
        this.spriteList.AddRange(spriteList);
        animTime = (float)frames / Application.targetFrameRate;
        perSpriteTime = animTime / spriteList.Count;
        animImage.sprite = spriteList[0];
        isPlaying = true;
        keepSize = true;
    }

    private void Update()
    {
        if (isPlaying)
        {
            timer += Time.deltaTime;
            if (timer > perSpriteTime && currentIndex < spriteList.Count - 1)
            {
                timer = 0;
                currentIndex++;
                animImage.sprite = spriteList[currentIndex];
                if (keepSize == false)
                {
                    animImage.SetNativeSize();
                }
                if (currentIndex >= spriteList.Count - 1)
                {
                    ResetMembers();
                    if (Event_AnimationOver != null)
                    {
                        Event_AnimationOver();
                    }
                }
            }
        }
    }

    private void ResetMembers()
    {
        timer = 0;
        spriteList.Clear();
        isPlaying = false;
        currentIndex = 0;
    }
}

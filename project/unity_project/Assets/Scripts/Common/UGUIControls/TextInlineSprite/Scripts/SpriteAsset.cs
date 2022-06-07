﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteAsset : ScriptableObject
{
    /// <summary>
    /// 图集ID
    /// </summary>
    public int Id;
    /// <summary>
    /// 静态表情
    /// </summary>
    public bool IsStatic;
    /// <summary>
    /// 图片资源
    /// </summary>
    public Texture TexSource;
    /// <summary>
    /// 所有sprite信息 SpriteAssetInfor类为具体的信息类
    /// </summary>
    public List<SpriteInforGroup> ListSpriteGroup;
}

[System.Serializable]
public class SpriteInfor
{
    /// <summary>
    /// ID
    /// </summary>
    public int  Id;
    /// <summary>
    /// 名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 中心点
    /// </summary>
    public Vector2 Pivot;
    /// <summary>
    ///坐标&宽高
    /// </summary>
    public Rect Rect;
    /// <summary>
    /// 精灵
    /// </summary>
    public Sprite Sprite;
    /// <summary>
    /// 标签
    /// </summary>
    public string Tag;
    /// <summary>
    /// uv
    /// </summary>
    public Vector2[] Uv;
}

[System.Serializable]
public class SpriteInforGroup
{
    public string Tag="";
    public List<SpriteInfor> ListSpriteInfor=new List<SpriteInfor>();
    public float Width=1.0f;
    public float Size=24.0f;
}
using System;
using UnityEngine;

public class SetWaveValue : MonoBehaviour {

    private MaterialPropertyBlock mpb;

    // Use this for initialization
    void Start () {
        mpb = new MaterialPropertyBlock();
        string name = this.transform.parent.name.Substring(0, this.transform.parent.name.IndexOf('_') + 1);
        string[] coords = name.Split(new char[] { '[',']',',' }, StringSplitOptions.RemoveEmptyEntries);
        int x = Convert.ToInt32(coords[0]);
        int y = Convert.ToInt32(coords[1]);
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        Rect rect = sr.sprite.rect;
        int textureWidth = sr.sprite.texture.width;
        int textureHeight = sr.sprite.texture.height;
        Vector4 rectForShader = new Vector4(rect.x / textureWidth, rect.y / textureHeight, rect.width / textureWidth, rect.height / textureHeight);
        sr.GetPropertyBlock(mpb);
        mpb.SetFloat("_WaveDelay", x + y);
        mpb.SetVector("_Rect", rectForShader);
        sr.SetPropertyBlock(mpb);
    }
}

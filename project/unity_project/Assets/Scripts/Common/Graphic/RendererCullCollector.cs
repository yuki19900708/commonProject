using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererCullCollector : MonoBehaviour
{
    void Start()
    {
        Renderer[] rs = this.GetComponentsInChildren<Renderer>(true);
        foreach(Renderer r in rs)
        {
            if (r is ParticleSystemRenderer)
            {
                //ParticleSystemRenderer的bounds在Start中获取时会是0，因为此时还没有开始渲染，因此延迟添加
                StartCoroutine(RegisterRenderer(r));
            }
            else
            {
                RendererCull.RegisterRenderer(r);
            }
        }
    }

    IEnumerator RegisterRenderer(Renderer r)
    {
        yield return new WaitForEndOfFrame();
        RendererCull.RegisterRenderer(r);
    }
}

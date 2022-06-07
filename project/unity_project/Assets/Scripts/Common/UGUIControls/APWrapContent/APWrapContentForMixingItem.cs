using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class APWrapContentForMixingItem : APWrapContentItem
{
    public List<InlineText> MixingText = new List<InlineText>();

    public override int Index
    {
        get { return index; }
        set
        {
            index = value;
            Refresh();
        }
    }
    /// <summary>
    /// 刷新文本
    /// </summary>
    public void Refresh()
    {
        //for (int i = 0; i < MixingText.Count; i++)
        //{
        //    MixingText[i].ForUpdateGeometry();
        //}
    }

}

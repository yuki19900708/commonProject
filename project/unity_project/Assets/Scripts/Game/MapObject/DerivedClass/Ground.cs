using UnityEngine;
using System.Collections.Generic;

public class Ground : MapObject
{
    public static int[] neightbourIndex = new int[]{ 1, 2, 4, 8 };
    public static string[] bottomBorderImageNameArray = { "ground_bottom_border_1", "ground_bottom_border_2", "ground_bottom_border_3" };
    public static string[] topBorderImageNameArray = { "ground_top_border_1", "ground_top_border_2", "ground_top_border_3" };
    public SpriteRenderer[] borders;
    
    public override void SetTileIndex(int tileIndex)
    {
        //地面是四邻模式
        for (int i = 0; i < borders.Length; i++)
        {
            int borderValue = neightbourIndex[i];
            borders[i].enabled = (tileIndex & borderValue) != borderValue;
            int borderStyle = Random.Range(0, 3);
            if (i == 0 || i == 3)
            {
                borders[i].sprite = UGUISpriteAtlasMgr.LoadSprite(bottomBorderImageNameArray[borderStyle]);
            }
            else
            {
                borders[i].sprite = UGUISpriteAtlasMgr.LoadSprite(topBorderImageNameArray[borderStyle]);
            }
        }
    }
}

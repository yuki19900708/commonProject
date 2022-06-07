using System;
using System.Text;
//using TMPro;
using UnityEngine;

public class ComUtil
{
    public static string Text_Cusomize_Color = "<color=#{0}FF>{1}</color>";
    public static Vector2 Camera_InitPos = new Vector3(3.5f, 11.3f, -10);
    public static Color Currency_Enough_Color = ColorExtend.ParseHexString("F2F3E4FF");
    public static Color Currency_Insufficient_Color = ColorExtend.ParseHexString("ED4671FF");
    public static Color Color_A = new Color(1, 1, 1, 0);
    public static Color Color_ChapterStartCountdown_A = new Color(1, 210.0f / 255.0f, 71.0f / 255.0f, 0);
    public static Color Color_ChapterStartCountdown = new Color(1, 210.0f / 255.0f, 71.0f / 255.0f, 1);

    public static Color Color_ChapterRewaredButtonColor = new Color(136.0f / 255.0f, 161.0f / 255.0f, 88.0f / 255.0f, 1);

    public static int Item_Type_LifeEffect = 701;
    public static string Expression_AP = "<sprite name=icon_ap>";
    public static string Expression_Gem = "<sprite name=icon_diamond>";
    public static string Expression_Gold = "<sprite name=icon_coin>";
    public static string Expression_Stone = "<sprite name=icon_stone>";
    public static string Expression_Warning_1 = "<sprite name=icon_warning_1>";
    public static string Expression_Warning = "<sprite name=tip_icon>";
    public static string Expression_Horn = "<sprite name=announcements_icon>";
    public static string Expression_Change = "<sprite name=icon_change>";
    public static string Expression_Flash = "<sprite name=flash_star>";
    public static string Expression_Star = "<sprite name=icon_star_2_filled>";
    public static string Expression_Refresh = "<sprite name=icon_refresh>";

    public static float Camera_InitSize = 6.5f;
    public static float Camera_Dragon_Dead_Size = 3f;
    public static float Camera_Plunder_Enter_Battle_Size = 5.5f;
    public static string TextContext = "No Text";
    public static string TextNull = "";
    public static string Text_x = "x";
    public static string Text_M = "M";
    public static string Text_m = "m";
    public static string Text_S = "S";
    public static string Text_s = "s";
    public static string Text_H = "H";
    public static string Text_h = "h";
    public static string Text_Space = " ";
    public static string Text_Colon = ":";
    public static string Text_Zone_Zone = "00";
    public static string SortingLayerObjectEffect = "ObjectEffect";

    public static string GetStringAdd(int p1, string p2, int p3)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, string p2, int p3)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, string p2, string p3)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3);
        return sb.ToString();
    }

    public static string GetStringAdd(int p1, int p2)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2);
        return sb.ToString();
    }

    public static string GetStringAdd(int p1, string p2)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, int p2)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, int p2, string p3)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, string p2, string p3, string p4)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3); sb.Append(p4);
        return sb.ToString();
    }

    public static string GetStringAdd(int p1, string p2, int p3, string p4)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3); sb.Append(p4);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, string p2, string p3, string p4, string p5)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3); sb.Append(p4); sb.Append(p5);
        return sb.ToString();
    }

    public static string GetStringAdd(int p1, string p2, int p3, string p4, int p5)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3); sb.Append(p4); sb.Append(p5);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, string p2, string p3, string p4, int p5)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3); sb.Append(p4); sb.Append(p5);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, string p2, int p3, string p4, int p5)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3); sb.Append(p4); sb.Append(p5);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, string p2, string p3, string p4, string p5, string p6)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3); sb.Append(p4); sb.Append(p5); sb.Append(p6);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, int p2, string p3, int p4, string p5, int p6, string p7)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3); sb.Append(p4); sb.Append(p5); sb.Append(p6); sb.Append(p7);
        return sb.ToString();
    }

    public static string GetStringAdd(string p1, int p2, string p3, int p4, string p5)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1); sb.Append(p2); sb.Append(p3); sb.Append(p4); sb.Append(p5);
        return sb.ToString();
    }

    //public static void ShowTimeText(TextMeshProUGUI timeText, int time)
    //{
    //    int second = TimerMgr.GetSeconds() - time;
    //    TimeSpan ts = new TimeSpan(0, 0, 0, second);

    //    if (ts.Days > 30)
    //    {
    //        int mon = ts.Days % 30;
    //        timeText.text = string.Format(L10NMgr.GetText(28300007), mon);
    //    }
    //    else if (ts.Days > 0)
    //    {
    //        timeText.text = string.Format(L10NMgr.GetText(28300006), ts.Days);
    //    }
    //    else if (ts.Hours > 0)
    //    {
    //        timeText.text = string.Format(L10NMgr.GetText(28300005), ts.Hours);
    //    }
    //    else if (ts.Minutes > 0)
    //    {
    //        if (ts.Minutes > 30)
    //        {
    //            timeText.text = L10NMgr.GetText(28300004);
    //        }
    //        else
    //        {
    //            timeText.text = string.Format(L10NMgr.GetText(28300003), ts.Minutes);
    //        }
    //    }
    //    else
    //    {
    //        timeText.text = L10NMgr.GetText(28300002);
    //    }
    //}

    /// <summary>
    /// 世界坐标转Canvas坐标
    /// </summary>
    /// <param name="canvasTransform"></param>
    /// <param name="camera"></param>
    /// <param name="position">世界物体坐标，CanvasOverLay模式下的世界坐标不适用</param>
    /// <returns></returns>
    public static Vector2 WorldToCanvasPosition(RectTransform canvasTransform, Camera camera, Vector3 position)
    {
        //Vector position (percentage from 0 to 1) considering camera size.
        //For example (0,0) is lower left, middle is (0.5,0.5)
        Vector2 temp = camera.WorldToViewportPoint(position);
        //Calculate position considering our percentage, using our canvas size
        //So if canvas size is (1100,500), and percentage is (0.5,0.5), current value will be (550,250)
        temp.x *= canvasTransform.sizeDelta.x;
        temp.y *= canvasTransform.sizeDelta.y;

        //The result is ready, but, this result is correct if canvas recttransform pivot is 0,0 - left lower corner.
        //But in reality its middle (0.5,0.5) by default, so we remove the amount considering cavnas rectransform pivot.
        //We could multiply with constant 0.5, but we will actually read the value, so if custom rect transform is passed(with custom pivot) , 
        //returned value will still be correct.
        temp.x -= canvasTransform.sizeDelta.x * canvasTransform.pivot.x;
        temp.y -= canvasTransform.sizeDelta.y * canvasTransform.pivot.y;

        return temp;
    }

    public static Vector3 CanvasToWorldPosition(RectTransform canvasTransform, Camera camera, Vector2 anchoredPosition)
    {
        Vector2 screenPoint = anchoredPosition;
        screenPoint.x += canvasTransform.sizeDelta.x * canvasTransform.pivot.x;
        screenPoint.y += canvasTransform.sizeDelta.y * canvasTransform.pivot.y;
        screenPoint.x /= canvasTransform.sizeDelta.x;
        screenPoint.y /= canvasTransform.sizeDelta.y;
        screenPoint.x *= Screen.width;
        screenPoint.y *= Screen.height;

        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasTransform, screenPoint, camera, out worldPosition);
        return worldPosition;
    }

    //public static string GetCurrencyExpression(int type)
    //{
    //    if (type == (int)PlayerDataType.CurrentGold)
    //    {
    //        return Expression_Gold;
    //    }
    //    else if (type == (int)PlayerDataType.CurrentStone)
    //    {
    //        return Expression_Stone;
    //    }
    //    else if (type == (int)PlayerDataType.CurrentGem)
    //    {
    //        return Expression_Gem;
    //    }
    //    return null;
    //}

    //public static void SetCurrencyCount(PlayerDataType type, int showCount, TextMeshProUGUI text)
    //{
    //    int totalCount = 0;
    //    if (type == PlayerDataType.CurrentGold)
    //    {
    //        totalCount = PlayerModel.Data.CurrentGold;
    //    }
    //    else if (type == PlayerDataType.CurrentStone)
    //    {
    //        totalCount = PlayerModel.Data.CurrentStone;
    //    }
    //    else if (type == PlayerDataType.CurrentGem)
    //    {
    //        totalCount = PlayerModel.Data.CurrentGem;
    //    }
    //    text.color = showCount > totalCount ? Currency_Insufficient_Color : Currency_Enough_Color;
    //}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Globalization;

public static class ColorExtend
{
    /// <summary>
    /// 将形如 #ABCDEF 或 #ABCDEFFF 格式的字符串转为Unity Color对象
    /// </summary>
    /// <param name="hexString">Hex颜色字符串</param>
    /// <returns></returns>
    public static Color ParseHexString(string hexString)
    {
        Regex regex = new Regex("#?([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})");
        Match match = regex.Match(hexString);
        if (match.Success)
        {
            float r = int.Parse(match.Groups[1].Value, NumberStyles.HexNumber) / 255f;
            float g = int.Parse(match.Groups[2].Value, NumberStyles.HexNumber) / 255f;
            float b = int.Parse(match.Groups[3].Value, NumberStyles.HexNumber) / 255f;
            float a = 1;
            if (match.Groups.Count == 5)
            {
                a = int.Parse(match.Groups[4].Value, NumberStyles.HexNumber) / 255f;
            }
            return new Color(r, g, b, a);
        }
        else
        {
            Debug.LogError(string.Format("Input: {0} is not valid hex color string", hexString));
        }
        return Color.black;
    }
}

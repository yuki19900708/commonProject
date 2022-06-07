using System;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate
{
    public int x;
    public int y;
}

public class ItemRate
{
    public int type;
    public int index;
    public int rate;
}

public class Item
{
    public int id;
    public int count;
}

public static class ResolveTypeUtil
{
    private static char[] SPLIT_CHAR = new char[] { '|', '-', ':',',' };
    #region 类型转换
    //供客户端使用
    public static object GetType(string t, object obj, string data)
    {
        try
        {
            if (string.IsNullOrEmpty(obj.ToString()) == false)
            {
                obj = obj.ToString().Replace("[", "");
                obj = obj.ToString().Replace("]", "");
                obj = obj.ToString().Trim(new char[] {' '});
            }

            if (t == "int")
            {
                if (string.IsNullOrEmpty(obj.ToString()))
                {
                    obj = "0";
                }
                obj = int.Parse(obj.ToString());
            }
            else if (t == "double")
            {
                if (string.IsNullOrEmpty(obj.ToString()))
                {
                    obj = "0";
                }
                obj = double.Parse(obj.ToString());
            }
            else if (t == "float")
            {
                if (string.IsNullOrEmpty(obj.ToString()))
                {
                    obj = "0";
                }
                obj = float.Parse(obj.ToString());
            }
            else if (t == "string")
            {
                if (obj.ToString().Contains("\\n"))
                {
                    obj = obj.ToString().Replace("\\n", "\n");
                }
                obj = obj.ToString();
            }
            else if (t == "bool" || t == "boolean")
            {
                if (string.IsNullOrEmpty(obj.ToString()))
                {
                    obj = "0";
                }
                if (int.Parse(obj.ToString()) >= 1)
                {
                    obj = true;
                }
                else
                {
                    obj = false;
                }
            }
            else if (t == "int[]" || t == "Item")
            {
                string[] strs = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                List<System.String> listS = new List<System.String>(strs);
                obj = Array.ConvertAll(listS.ToArray(), s => int.Parse(s));
            }
            else if (t == "double[]")
            {
                string[] strs = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                List<System.String> listS = new List<System.String>(strs);
                obj = Array.ConvertAll(listS.ToArray(), s => double.Parse(s));
            }
            else if (t == "float[]")
            {
                string[] strs = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                List<System.String> listS = new List<System.String>(strs);
                obj = Array.ConvertAll(listS.ToArray(), s => float.Parse(s));
            }
            else if (t == "string[]")
            {
                obj = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (t == "Vector2")
            {
                obj = Vector2Parse(obj.ToString());
            }
            else if (t == "Vector3")
            {
                obj = Vector3Parse(obj.ToString());
            }
            else if (t == "OutputData" || t == "ItemRate")
            {
                OutputData newData = new OutputData();
                List<ObjectInfo> infoList = new List<ObjectInfo>();
                string[] strs = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                List<string> listStr = new List<string>(strs);

                if (listStr.Count > 2)
                {
                    if ((listStr.Count - 2) % 3 != 0)
                    {
                        Debug.LogError("表" + data + "**数值" + obj.ToString() + "出错");
                    }
                }
                if (listStr.Count >= 2)
                {
                    for (int i = 0; i < 2; i += 2)
                    {
                        ObjectInfo info = new ObjectInfo();
                        info.itemType = int.Parse(listStr[i]);
                        info.itemIndex = int.Parse(listStr[i + 1]);
                        newData.defaultLoot = info;
                    }

                    for (int i = 2; i < listStr.Count; i += 3)
                    {
                        ObjectInfo info = new ObjectInfo();
                        info.itemType = int.Parse(listStr[i]);
                        info.itemIndex = int.Parse(listStr[i + 1]);
                        info.rate = int.Parse(listStr[i + 2]);
                        infoList.Add(info);
                    }
                    newData.rateLootList = infoList;
                }
            
                obj = newData;
            }
            else
            {
                Debug.LogError("表" + data + "**数值" + obj.ToString() + "该类型不存在" + t);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("表" + data + "**数值" + obj.ToString() + "该类型不存在" + t + "**" + ex);
        }
        return obj;
    }

    //供服务器端使用
    public static object GetServerType(string t, object obj, string data, int row)
    {
        try
        {
            if (string.IsNullOrEmpty(obj.ToString()))
            {
                return null;
            }
            else
            {
                if (t != "Point")
                {
                    obj = obj.ToString().Replace("[", "");
                    obj = obj.ToString().Replace("]", "");
                    obj = obj.ToString().Replace(" ", "");
                }
            }
            if (t == "int")
            {
                if (string.IsNullOrEmpty(obj.ToString()))
                {
                    obj = "0";
                }
                obj = int.Parse(obj.ToString());
            }
            else if (t == "Point")
            {
                if (string.IsNullOrEmpty(obj.ToString()) == false)
                {
                    string[] strs = obj.ToString().Split(new string[] { "]" }, StringSplitOptions.RemoveEmptyEntries);

                    List<int[]> arrList = new List<int[]>();
                    List<Coordinate> pointList = new List<Coordinate>();

                    for (int i = 0; i < strs.Length; i++)
                    {
                        string tempStr = strs[i];
                        tempStr = tempStr.Replace("[", "");
                        string[] newStr = tempStr.Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                        List<System.String> listS = new List<System.String>(newStr);
                        arrList.Add(Array.ConvertAll(listS.ToArray(), s => int.Parse(s)));
                    }
                    for (int i = 0; i < arrList.Count;i++ )
                    {
                        Coordinate t1 = new Coordinate();
                        t1.x = arrList[i][0];
                        t1.y = arrList[i][1];
                        pointList.Add(t1);
                    }
                    obj = pointList;
                }
                else
                {
                    obj = null;
                }
            }
            else if (t == "double")
            {
                obj = double.Parse(obj.ToString());
            }
            else if (t == "float")
            {
                obj = float.Parse(obj.ToString());
            }
            else if (t == "string")
            {
                obj = obj.ToString();
            }
            else if (t == "bool" || t == "boolean")
            {
                if (int.Parse(obj.ToString()) >= 1)
                {
                    obj = true;
                }
                else
                {
                    obj = false;
                }
            }
            else if (t == "int[]")
            {
                string[] strs = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                List<System.String> listS = new List<System.String>(strs);
                obj = Array.ConvertAll(listS.ToArray(), s => int.Parse(s));
                
            }
            else if(t == "Item")
            {
                string[] strs = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                List<System.String> listS = new List<System.String>(strs);
                obj = Array.ConvertAll(listS.ToArray(), s => int.Parse(s));
                int [] value = (int[])obj;
                List<Item> array = new List<Item>();
                if (value.Length % 2 == 0)
                {
                    for (int i = 0; i < value.Length; i+=2)
                    {
                        Item item = new Item();
                        item.id = value[i];
                        item.count = value[i+1];
                        array.Add(item);
                    }
                }
                obj = array;
            }
            else if (t == "double[]")
            {
                string[] strs = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                List<System.String> listS = new List<System.String>(strs);
                obj = Array.ConvertAll(listS.ToArray(), s => double.Parse(s));
            }
            else if (t == "float[]")
            {
                string[] strs = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                List<System.String> listS = new List<System.String>(strs);
                obj = Array.ConvertAll(listS.ToArray(), s => float.Parse(s));
            }
            else if (t == "string[]")
            {
                obj = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (t == "Vector2")
            {
                obj = Vector2Parse(obj.ToString());
            }
            else if (t == "Vector3")
            {
                obj = Vector3Parse(obj.ToString());
            }
            else if (t == "OutputData" || t == "ItemRate")
            {
                OutputData newData = new OutputData();
                List<ObjectInfo> infoList = new List<ObjectInfo>();
                string[] strs = obj.ToString().Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
                List<string> listStr = new List<string>(strs);

                if (listStr.Count >= 2)
                {
                    for (int i = 0; i < 2; i += 2)
                    {
                        ObjectInfo info = new ObjectInfo();
                        info.itemType = int.Parse(listStr[i]);
                        info.itemIndex = int.Parse(listStr[i + 1]);
                        newData.defaultLoot = info;
                    }

                    for (int i = 2; i < listStr.Count; i += 3)
                    {
                        ObjectInfo info = new ObjectInfo();
                        info.itemType = int.Parse(listStr[i]);
                        info.itemIndex = int.Parse(listStr[i + 1]);
                        info.rate = int.Parse(listStr[i + 2]);
                        infoList.Add(info);
                    }
                    newData.rateLootList = infoList;
                }

                List<ItemRate> itemRate = new List<ItemRate>();
                if (newData.defaultLoot != null)
                {
                    ItemRate item = new ItemRate();
                    item.type = newData.defaultLoot.itemType;
                    item.index = newData.defaultLoot.itemIndex;
                    item.rate = newData.defaultLoot.rate;
                    itemRate.Add(item);
                }
                if (newData.rateLootList != null)
                {
                    for (int i = 0; i < newData.rateLootList.Count; i++)
                    {
                        ItemRate item = new ItemRate();
                        item.type = newData.defaultLoot.itemType;
                        item.index = newData.defaultLoot.itemIndex;
                        item.rate = newData.defaultLoot.rate;
                        itemRate.Add(item);
                    }
                }
                obj = itemRate;
            }
            else
            {
                Debug.LogError("表" + data + "**数值" + obj.ToString() + "该类型不存在" + t);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("表" + data + "**数值" + obj.ToString() + "行" + row + "该类型不存在" + t + "**" + ex);
        }
        return obj;
    }

    public static Vector3 Vector3Parse(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Vector3.zero;
        }
        else
        {
            string[] s = value.Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
            return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
        }
    }

    public static Vector2 Vector2Parse(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Vector2.zero;
        }
        else
        {
            string[] s = value.Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
            return new Vector2(float.Parse(s[0]), float.Parse(s[1]));
        }
    }

    public static List<int> ListParsestring(string value)
    {
        List<int> dataList = new List<int>();

        string[] s = value.Split(SPLIT_CHAR, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < s.Length; i++)
        {
            dataList.Add(int.Parse(s[i]));
        }
        return dataList;
    }
    #endregion

}

using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using GOLib.Tool;

public partial class PlayerProfile
{
    public static int ProfileVersion = 1;

    public static string KeyProfileVersion = "profile_version";
    public static string KeyDeviceUniqueID = "device_unique_id";

    public static void ClearProfile()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    #region Public Methods
    /// <summary>
    /// 检查设备Uid 
    /// </summary>
    /// <returns></returns>
    public static bool CheckDeviceUniqueID()
    {
        string deviceUniqueId = LoadStringValue(KeyDeviceUniqueID);
        if (string.IsNullOrEmpty(deviceUniqueId) == false)
        {
            return deviceUniqueId.Equals(CommonTools.GetDeviceUID());
        }
        else
        {
            SaveStringValue(KeyDeviceUniqueID, CommonTools.GetDeviceUID());
            return true;
        }
    }

    public static bool HasKey(string key)
    {
        string encrypedKey = SecurityTool.EncryptString(key);
        return PlayerPrefs.HasKey(encrypedKey);
    }

    public static void DeleteKey(string key)
    {
        string encrypedKey = SecurityTool.EncryptString(key);
        PlayerPrefs.DeleteKey(encrypedKey);
        PlayerPrefs.Save();
    }
    #endregion

    #region Private Methods
    private static void SaveProfileVersion(int version)
    {
        PlayerPrefs.SetInt(KeyProfileVersion, version);
        PlayerPrefs.Save();
    }

    private static void SaveIntValue(string key, int finalValue)
    {
        SaveStringValue(key, finalValue.ToString());
    }

    private static void SaveLongValue(string key, long finalValue)
    {
        SaveStringValue(key, finalValue.ToString());
    }

    private static void SaveStringValue(string key, string finalValue)
    {
        string encrypedKey = SecurityTool.EncryptString(key);
        string encryptedValue = CombineEncryptKeyAndValue(key, finalValue);
        PlayerPrefs.SetString(encrypedKey, encryptedValue);
        PlayerPrefs.Save();
    }

    private static void SaveBoolValue(string key, bool finalValue)
    {
        SaveIntValue(key, finalValue ? 1 : 0);
    }

    private static bool LoadBoolValue(string key, bool defaultValue)
    {
        string savedString = LoadStringValue(key);
        if (string.IsNullOrEmpty(savedString) == false)
        {
            int realValue = int.Parse(savedString);
            return (realValue == 1);
        }
        else
        {
            SaveBoolValue(key, defaultValue);
            return defaultValue;
        }
    }

    private static int LoadIntValue(string key, int defaultValue)
    {
        string savedString = LoadStringValue(key);
        if (string.IsNullOrEmpty(savedString) == false)
        {
            int realValue = int.Parse(savedString);
            return realValue;
        }
        else
        {
            SaveIntValue(key, defaultValue);
            return defaultValue;
        }
    }

    public static long LoadLongValue(string key, long defaultValue)
    {
        string savedString = LoadStringValue(key);
        if (string.IsNullOrEmpty(savedString) == false)
        {
            long realValue = long.Parse(savedString);
            return realValue;
        }
        else
        {
            SaveLongValue(key, defaultValue);
            return defaultValue;
        }
    }

    private static string LoadStringValue(string key)
    {
        string encrypedKey = SecurityTool.EncryptString(key);
        if (PlayerPrefs.HasKey(encrypedKey))
        {
            string savedString = PlayerPrefs.GetString(encrypedKey);
            string[] decryptStrings = SplitDecryptKeyAndValue(savedString);
            if (decryptStrings != null && decryptStrings.Length == 2)
            {
                if (decryptStrings[0].Equals(key))
                {
                    return decryptStrings[1];
                }
                else
                {
                    return String.Empty;
                }
            }
            else
            {
                return String.Empty;
            }
        }
        else
        {
            return String.Empty;
        }
    }

    private static string CombineEncryptKeyAndValue(string key, string val)
    {
        return SecurityTool.EncryptString(key + "@" + val);
    }

    private static string[] SplitDecryptKeyAndValue(string encryptString)
    {
        if (string.IsNullOrEmpty(encryptString) == false)
        {
            string decryptedString = SecurityTool.DecryptString(encryptString);
            return decryptedString.Split(new char[] { '@' });
        }
        else
        {
            return null;
        }
    }
    #endregion
}
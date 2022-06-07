using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.Internal;
using System.Reflection;
using UnityEditor;

/// <summary>
/// 日志颜色配置项
/// </summary>
[System.Serializable]
public class LogColorConfigItem
{
    /// <summary>
    /// 输出者的名字
    /// </summary>
    [SerializeField]
    [Header("开发者名称")]
    public string developerName;
    /// <summary>
    ///Log输出者需要输出的模块数组必须和色值组一一对应 
    /// </summary>
    [SerializeField]
    [Header("模块颜色列表")]
    public List<ModuleColor> moduleColorList;
}

/// <summary>
/// 模块颜色
/// </summary>
[System.Serializable]
public struct ModuleColor
{
    /// <summary>
    /// 模块名称
    /// </summary>
    [SerializeField]
    [Header("模块名称")]
    public string moduleName;
    /// <summary>
    /// 文字颜色
    /// </summary>
    [SerializeField]
    [Header("文字名称")]
    public Color color;
}

/// <summary>
/// 字体大小选择的枚举
/// </summary>
public enum LogFontSize
{
    /// <summary>一般字体样式 默认样式</summary>
    General = 10,
    /// <summary>小型字体</summary>
    Small = 5,
    /// <summary>中型字体</summary>
    Big = 20,
    /// <summary>大型字体</summary>
    OverBig = 30,
}

/// <summary>
/// 日志输出类
/// </summary>
public class Debug
{
    #region 扩展的Log方法
    static Dictionary<string, Dictionary<string, Color>> developerNameModuleColorDict = new Dictionary<string, Dictionary<string, Color>>();

    static StringBuilder stringBuilder = new StringBuilder(65535);

    static Debug()
    {
        Initialize();
    }

    /// <summary>
    /// 读取DebugSetting的中内容初始化开发者，模块，颜色数据
    /// </summary>
    public static void Initialize()
    {
        //获取到DebugSetting的设置内容
        List<LogColorConfigItem> itemList = DebugSetting.Instance.logColorConfigItemList;
        
        //对设置缓存进行赋值
        foreach (LogColorConfigItem item in itemList)
        {
            developerNameModuleColorDict.Add(item.developerName, new Dictionary<string, Color>());
            for (int i = 0; i < item.moduleColorList.Count; i++)
            {
                developerNameModuleColorDict[item.developerName].Add(item.moduleColorList[i].moduleName, item.moduleColorList[i].color);
            }
        }
    }

    /// <summary>
    /// 可自定义 文字颜色、字体大小 的Log方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="fontSize"></param>
    /// <param name="color"></param>
    [Conditional("UNITY_LOG")]
    public static void Log(object message, LogFontSize fontSize, string color)
    {
        DoLog(message, null, string.Empty, string.Empty, color, fontSize);
    }

    /// <summary>
    /// 可自定义 文字颜色、字体大小 的Log方法，带Context
    /// </summary>
    /// <param name="message"></param>
    /// <param name="fontSize"></param>
    /// <param name="color"></param>
    /// <param name="context">输出内容关联的Object</param>
    [Conditional("UNITY_LOG")]
    public static void Log(object message, LogFontSize fontSize, string color, UnityEngine.Object context)
    {
        DoLog(message, context, string.Empty, string.Empty, color, fontSize);
    }

    /// <summary>
    /// 可自定义 开发者 的Log方法
    /// </summary>
    /// <param name="developerName">输出者名字</param>
    /// <param name="message">输出内容</param>
    [Conditional("UNITY_LOG")]
    public static void Log(string developerName, string message)
    {
        DoLog(message, null, developerName, string.Empty, string.Empty, LogFontSize.General);
    }

    /// <summary>
    /// 可自定义 开发者 的Log方法，带Context
    /// </summary>
    /// <param name="developerName">输出者名字</param>
    /// <param name="message">输出内容</param>
    /// <param name="context">输出内容关联的Object</param>
    [Conditional("UNITY_LOG")]
    public static void Log(string developerName, string message, UnityEngine.Object context)
    {
        DoLog(message, context, developerName, string.Empty, string.Empty, LogFontSize.General);
    }

    /// <summary>
    /// 可自定义 开发者、模块名 的Log方法
    /// </summary>
    /// <param name="developerName">输出者名字</param>
    /// <param name="moduleName">模块名字</param>
    /// <param name="message">消息内容</param>
    [Conditional("UNITY_LOG")]
    public static void Log(string developerName, string moduleName, object message)
    {
        DoLog(message, null, developerName, moduleName, string.Empty, LogFontSize.General);
    }

    /// <summary>
    /// 可自定义 开发者、模块名 的Log方法，带Context
    /// </summary>
    /// <param name="developerName">输出者名字</param>
    /// <param name="moduleName">模块名字</param>
    /// <param name="message">消息内容</param>
    /// <param name="context">输出内容关联的Object</param>
    [Conditional("UNITY_LOG")]
    public static void Log(string developerName, string moduleName, object message, UnityEngine.Object context)
    {
        DoLog(message, context, developerName, moduleName, string.Empty, LogFontSize.General);
    }

    /// <summary>
    /// 可自定义 开发者、模块名、字体大小 的Log方法
    /// </summary>
    /// <param name="message">输出内容</param>
    /// <param name="developerName">输出者的名字</param>
    /// <param name="moduleName">输出内容所在模块</param>
    /// <param name="fontSize">字体大小</param>
    [Conditional("UNITY_LOG")]
    public static void Log(string developerName, string moduleName, object message, LogFontSize fontSize)
    {
        string color = string.Empty;
        if (developerNameModuleColorDict.ContainsKey(developerName))
        {
            if (developerNameModuleColorDict[developerName].ContainsKey(moduleName))
            {
                color = FormatColor32(developerNameModuleColorDict[developerName][moduleName]);
            }
        }
        DoLog(message, null, developerName, moduleName, color, fontSize);
    }

    /// <summary>
    /// 可自定义 开发者、模块名、字体大小 的Log方法，带context
    /// </summary>
    /// <param name="message">输出内容</param>
    /// <param name="context">输出内容关联的Object</param>
    /// <param name="developerName">输出者的名字</param>
    /// <param name="moduleName">输出内容所在模块</param>
    /// <param name="fontSize">字体大小</param>
    [Conditional("UNITY_LOG")]
    public static void Log(string developerName, string moduleName, object message, UnityEngine.Object context, LogFontSize fontSize)
    {
        string color = string.Empty;
        if (developerNameModuleColorDict.ContainsKey(developerName))
        {
            if (developerNameModuleColorDict[developerName].ContainsKey(moduleName))
            {
                color = FormatColor32(developerNameModuleColorDict[developerName][moduleName]);
            }
        }
        DoLog(message, context, developerName, moduleName, color, fontSize);
    }

    /// <summary>
    /// 自定义类消息的打印
    /// </summary>
    /// <param name="message">String or object to be converted to string representation for display.</param>
    /// <param name="context">Object to which the message applies.</param>
    /// <param name="developerName"></param>
    /// <param name="moduleName"></param>
    /// <param name="color"></param>
    /// <param name="fontSize"></param>
    static void DoLog(object message, UnityEngine.Object context, string developerName, string moduleName, string color, LogFontSize fontSize)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");

        if (string.IsNullOrEmpty(developerName) == false)
        {
            stringBuilder.Append(developerName);
            if (string.IsNullOrEmpty(moduleName) == false)
            {
                stringBuilder.Append(" --> ");
                stringBuilder.Append(moduleName);
            }
            stringBuilder.Append(" : ");
        }
        else
        {
            if (string.IsNullOrEmpty(moduleName) == false)
            {
                stringBuilder.Append(moduleName);
                stringBuilder.Append(" : ");
            }
        }
        if (fontSize != LogFontSize.General)
        {
            stringBuilder.AppendFormat("<size={0}>", (int)fontSize);
        }
        if (string.IsNullOrEmpty(color) == false)
        {
            stringBuilder.AppendFormat("<color={0}>", color);
        }

        stringBuilder.Append(message);

        if (string.IsNullOrEmpty(color) == false)
        {
            stringBuilder.Append("</color>");
        }
        if (fontSize != LogFontSize.General)
        {
            stringBuilder.Append("</size>");
        }

        if (context == null)
        {
            UnityEngine.Debug.Log(stringBuilder.ToString());
        }
        else
        {
            UnityEngine.Debug.Log(stringBuilder.ToString(), context);
        }
    }

    /// <summary>
    /// 将Color32转换为#FFFFFFFF格式
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    static private string FormatColor32(Color32 color)
    {
        stringBuilder.Remove(0, stringBuilder.Length);
        stringBuilder.Append("#");
        stringBuilder.Append(color.r.ToString("x2"));
        stringBuilder.Append(color.g.ToString("x2"));
        stringBuilder.Append(color.b.ToString("x2"));
        return stringBuilder.ToString();
    }
    #endregion

    #region UnityEngine Debug 原生成员与方法
    public static bool developerConsoleVisible
    {
        get
        {
            return UnityEngine.Debug.developerConsoleVisible;
        }
        set
        {
            UnityEngine.Debug.developerConsoleVisible = value;
        }
    }
    
    public static bool isDebugBuild
    {
        get
        {
            return UnityEngine.Debug.isDebugBuild;
        }
    }
    
    public static ILogger logger
    {
        get
        {
            return UnityEngine.Debug.logger;
        }
    }

    #region 覆盖 UnityEngine.Debug 原生Log方法，添加Conditional特性，方便发布时去除日志
    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition)
    {
        UnityEngine.Debug.Assert(condition);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, string message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, object message)
    {
        UnityEngine.Debug.Assert(condition, message);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, UnityEngine.Object context)
    {
        UnityEngine.Debug.Assert(condition, context);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, string message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Assert(condition, message, context);
    }
    
    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Assert(condition, message, context);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void AssertFormat(bool condition, string format, params object[] args)
    {
        UnityEngine.Debug.AssertFormat(condition, format, args);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void AssertFormat(bool condition, UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.AssertFormat(condition, context, format, args);
    }
    
    public static void DrawLine(Vector3 start, Vector3 end)
    {
        UnityEngine.Debug.DrawLine(start, end);
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        UnityEngine.Debug.DrawLine(start, end, color);
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration);
    }

    public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    public static void DrawRay(Vector3 start, Vector3 dir)
    {
        UnityEngine.Debug.DrawLine(start, dir);
    }

    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
        UnityEngine.Debug.DrawLine(start, dir, color);
    }

    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
    {
        UnityEngine.Debug.DrawLine(start, dir, color, duration);
    }

    public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
    {
        UnityEngine.Debug.DrawLine(start, dir, color, duration, depthTest);
    }

    [Conditional("UNITY_LOG")]
    public static void Log(object message)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(message);
        UnityEngine.Debug.Log(stringBuilder.ToString());
    }

    [Conditional("UNITY_LOG")]
    public static void Log(object message, UnityEngine.Object context)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(message);
        UnityEngine.Debug.Log(stringBuilder.ToString(), context);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertion(object message)
    {
        UnityEngine.Debug.LogAssertion(message);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertion(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogAssertion(message, context);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertionFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogAssertionFormat(format, args);
    }

    [Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertionFormat(UnityEngine.Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogAssertionFormat(context, format, args);
    }

    [Conditional("UNITY_LOG")]
    public static void LogError(object message)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(message);
        UnityEngine.Debug.LogError(stringBuilder.ToString());
    }

    [Conditional("UNITY_LOG")]
    public static void LogError(object message, UnityEngine.Object context)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(message);
        UnityEngine.Debug.LogError(stringBuilder.ToString(), context);
    }

    [Conditional("UNITY_LOG")]
    public static void LogErrorFormat(string format, params object[] args)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(format);
        UnityEngine.Debug.LogErrorFormat(stringBuilder.ToString(), args);
    }

    [Conditional("UNITY_LOG")]
    public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(format);
        UnityEngine.Debug.LogErrorFormat(stringBuilder.ToString(), format, args);
    }

    [Conditional("UNITY_LOG")]
    public static void LogException(Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

    [Conditional("UNITY_LOG")]
    public static void LogException(Exception exception, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogException(exception, context);
    }

    [Conditional("UNITY_LOG")]
    public static void LogFormat(string format, params object[] args)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(format);
        UnityEngine.Debug.LogFormat(stringBuilder.ToString(), args);
    }

    [Conditional("UNITY_LOG")]
    public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(format);
        UnityEngine.Debug.LogFormat(context, stringBuilder.ToString(), args);
    }

    [Conditional("UNITY_LOG")]
    public static void LogWarning(object message)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(message);
        UnityEngine.Debug.LogWarning(stringBuilder.ToString());
    }

    [Conditional("UNITY_LOG")]
    public static void LogWarning(object message, UnityEngine.Object context)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(message);
        UnityEngine.Debug.LogWarning(stringBuilder.ToString(), context);
    }

    [Conditional("UNITY_LOG")]
    public static void LogWarningFormat(string format, params object[] args)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(format);
        UnityEngine.Debug.LogWarningFormat(stringBuilder.ToString(), args);
    }

    [Conditional("UNITY_LOG")]
    public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
    {
        stringBuilder.Remove(0, stringBuilder.Length);

        stringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        stringBuilder.Append(" ");
        stringBuilder.Append(format);
        UnityEngine.Debug.LogWarningFormat(context, stringBuilder.ToString(), args);
    }
    #endregion
    #endregion
}
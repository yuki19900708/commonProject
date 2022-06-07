using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class LogUtil : MonoBehaviour {
    
    private void OnApplicationQuit()
    {
        SaveEditorLog();
    }

    private void SaveEditorLog()
    {
#if UNITY_EDITOR_WIN
        string editorLogFile = Environment.GetEnvironmentVariable("LocalAppData") + "/Unity/Editor/Editor.log";
        string dateString = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss", CultureInfo.InvariantCulture);
        string newFile = editorLogFile.Replace("Editor.log", string.Format("Editor_{0}.log", dateString));

        File.Copy(editorLogFile, newFile);
#endif
    }

    public static void LogToFile(string message)
    {
        File.AppendAllText("../../output/FileLog.txt", message + "\n");
    }
}

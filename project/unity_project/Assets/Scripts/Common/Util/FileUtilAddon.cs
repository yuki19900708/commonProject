using System.IO;
using UnityEngine;

public static class FileUtilAddon
{
    public static string GetStreamingAssetsPathForWWW(string relativePath)
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        string protcol = "file://";
#elif UNITY_ANDROID
        string protcol = string.Empty;
#endif
        return string.Format("{0}{1}/{2}", protcol, Application.streamingAssetsPath, relativePath);
    }

    public static string GetPersistentDataPathForWWW(string relativePath)
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        string protcol = "file://";
#elif UNITY_ANDROID
        string protcol = string.Empty;
#endif
        return string.Format("{0}{1}/{2}", protcol, Application.persistentDataPath, relativePath);
    }

    public static void CopyFolder(string sourceFolder, string destFolder)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourceFolder, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourceFolder, destFolder));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourceFolder, destFolder), true);
        }
    }

    public static readonly string[] FILE_SIZE_SUFFIX = new string[] { "Bytes", "KB", "MB", "GB", "TB"};
    /// <summary>
    /// 获得以Bytes, KB, MB, GB, TB形式显示的文件尺寸
    /// </summary>
    /// <returns></returns>
    public static string GetFileSizeString(long fileSize)
    {
        int loopCount = 0;
        while(loopCount < FILE_SIZE_SUFFIX.Length && fileSize > 1024)
        {
            loopCount++;
            fileSize /= 1024;
        }
        return string.Format("{0}{1}", fileSize, FILE_SIZE_SUFFIX[loopCount]); 
    }
}

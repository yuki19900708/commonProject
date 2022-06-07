using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using SimpleJson;
using System.Collections.Generic;

public class CommandBuild : EditorWindow
{
    private static string   XCODE_PROJECT_FOLDER 	= "unity_xcode_project";
	
	private static string   STRING_WIN     			= "win";
	private static string   STRING_ANDROID 			= "android";
	private static string   STRING_IOS     			= "ios";
	private static string   STRING_EXE     			= "exe";
	private static string   STRING_APK     			= "apk";
	private static string   STRING_APP     			= "app";
	
    private static string   KEYSTORE_NAME           = "appcpi";
    private static string   KEYSTORE_ALIAS          = "appcpi_alias";
	private static string   KEYSTORE_PASS	        = "appcpi123";
	private static string   KEYSTORE_ALIAS_PASS    	= "appcpi123";
	
    private static EditorBuildSettingsScene[] scenes = null;

    #region Build Version
    [MenuItem("Appcpi/打包出版/Build Windows APP")]
    public static void BuildWinApp()
    {
        CheckSceneList();
        string winAppPath = CreateAppOutputDir(STRING_WIN, STRING_EXE);
        BuildPipeline.BuildPlayer(scenes, winAppPath, BuildTarget.StandaloneWindows, BuildOptions.None); 
    }
	
	[MenuItem("Appcpi/打包出版/Build Android Unity APP")]
	public static void BuildAndroidUnityApp()
	{
		CheckSceneList();
        string androidAppPath = CreateAppOutputDir(STRING_ANDROID, STRING_APK);
        string keystoreFile = Path.Combine(Path.GetFullPath("."), string.Format("../android_project/{0}.keystore", KEYSTORE_NAME));
        PlayerSettings.Android.keystoreName = keystoreFile;
        PlayerSettings.Android.keystorePass = KEYSTORE_PASS;
        PlayerSettings.Android.keyaliasName = KEYSTORE_ALIAS;
        PlayerSettings.Android.keyaliasPass = KEYSTORE_ALIAS_PASS;

        BuildPipeline.BuildPlayer(scenes, androidAppPath, BuildTarget.Android, BuildOptions.None);
	}
	
	[MenuItem("Appcpi/打包出版/Build Android Studio Project")]
	public static void BuildAndroidStudioProject()
	{
		CheckSceneList();
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        string androidProjectDir = CreateProjectOutputDir(STRING_ANDROID);
		BuildPipeline.BuildPlayer(scenes, androidProjectDir, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
    }
	
	[MenuItem("Appcpi/打包出版/Build iOS Xcode Project")]
	public static void BuildiOSXcodeProject()
	{
		CheckSceneList();
        string xCodeProjectDir = CreateProjectOutputDir(STRING_IOS);
		BuildPipeline.BuildPlayer(scenes, xCodeProjectDir, BuildTarget.iOS, BuildOptions.None); 
    }

    public static void BuildPVRTCAssetBundle()
    {
        AssetBundlePipeline.BuildAssetBundleFromExternal();
    }

    public static void UpdateBuildTimestamp()
    {
        string configFile = Path.Combine(Path.GetFullPath("."), "Assets/Resources/config.txt");
        string configFileContent = File.ReadAllText(configFile);

        JsonObject configJson = SimpleJson.SimpleJson.DeserializeObject<JsonObject>(configFileContent);
        configJson["BuildTimestamp"] = DateTime.Now.ToString("yyyyMMddhhmm");

        configFileContent = configJson.ToString();
        File.WriteAllText(configFile, configFileContent);
    }

    /// <summary>
    /// 获取命令行参数
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string GetCommandLineArgs(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            Debug.Log("Command Args: " + i + " " + args[i]);
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }

    private static void CheckSceneList()
    {
        scenes = EditorBuildSettings.scenes;
    }

	private static string CreateProjectOutputDir(string platform)
	{
        string outputFolder = Path.Combine(Path.GetFullPath("."), string.Format("../../output/{0}", platform));
		if (platform.Equals(STRING_IOS))
		{
            outputFolder = Path.Combine(outputFolder, XCODE_PROJECT_FOLDER);
		}

		try
		{
            string finalDir = Path.Combine(outputFolder, PlayerSettings.productName);
			if (Directory.Exists(finalDir))
			{
				Directory.Delete(finalDir, true);
			}
			if (Directory.Exists(outputFolder) == false)
			{
				Directory.CreateDirectory(outputFolder);
			}
		}
		catch(IOException e)
		{
            DebugCommandLine(e.Message);
		}
		return outputFolder;
	}
	
	private static string CreateAppOutputDir(string platform, string extension)
    {
        string outputFolder = Path.Combine(Path.GetFullPath("."), string.Format("../../output/{0}", platform));
        string outputAppFolder = Path.Combine(outputFolder, STRING_APP);
        string outputAppPath = null;
        if(extension == STRING_APK)
        {
            outputAppPath = Path.Combine(outputAppFolder, string.Format("{0}_v{1}_{2}.{3}",
                PlayerSettings.productName, 
                PlayerSettings.bundleVersion,
                System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss"),
                extension));
        }
        else
        {
            outputAppPath = Path.Combine(outputAppFolder, string.Format("{0}.{1}", PlayerSettings.productName, extension));
        }
		try
		{
			if (Directory.Exists(outputAppFolder))
			{
				Directory.Delete(outputAppFolder, true);
			}
			Directory.CreateDirectory(outputAppFolder);
		}
		catch(IOException e)
		{
            DebugCommandLine(e.Message);
		}
		return outputAppPath;
    }

    private static void DebugCommandLine(string message, bool isError = false)
    {
        if (isError)
        {
            Debug.LogError(message);
        }
        else
        {
            Debug.Log(message);
        }
        //File.AppendAllText("../../output/DebugCommandLine.txt", message + "\n");
    }
    #endregion
}
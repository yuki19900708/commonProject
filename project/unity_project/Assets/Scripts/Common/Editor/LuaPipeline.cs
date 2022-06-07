using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using System.Net;

public enum UploadSever
{
    RYH_Server,
    HK_Server
}
/// <summary>
/// 基于xLua的Lua生产线
/// </summary>
public class LuaPipeline : EditorWindow
{
    private static readonly string[] uploadServerURLs = {
        "http://192.168.196.28/park-of-monster/upload_zip.php",
        "http://hdqj.aktgo.com/park-of-monster/upload_zip.php"
    };

    /// <summary>
    /// Lua脚本目录
    /// </summary>
    private string luaSourcePath = "Assets/Scripts/Game/Lua";
    private string epLuaSourcePath = "LuaPipeline_luaSourcePath";
    /// <summary>
    /// Lua脚本签名后输出目录，相对于Application.persistentDataPath
    /// </summary>
    private string outputPath = "Lua";
    private string epOutputPath = "LuaPipeline_outputPath";
    /// <summary>
    /// 完整的输出路径，包含Application.persistentDataPath
    /// </summary>
    private string finalOutputPath = string.Empty;
    /// <summary>
    /// 生成完加密Lua后是否上传到服务器上
    /// </summary>
    private bool uploadToServer = true;
    private string epUploadToServer = "LuaPipeline_uploadToServer";
    /// <summary>
    /// 上传地址
    /// </summary>
    private static UploadSever uploadSever = UploadSever.RYH_Server;
    private static string epUploadServer = "AssetBundlePipeline_uploadServer";
    /// <summary>
    /// 生成完加密Lua后，是否在客户端本地persistentDataPath中保留一份
    /// 因为客户端是从persistentDataPath中来读取Lua的，通常应该走热更流程，从服务端下载新的AssetBundle
    /// 把该值改为true用于不经过服务器下载，直接在客户端测试AssetBundle内容的正确性
    /// </summary>
    private bool keepLocal = false;
    private string epKeepLocal = "LuaPipeline_keepLocal";

    //[MenuItem("Appcpi/数据生成/BuildLua")]
    public static void Init()
    {
        LuaPipeline luap = EditorWindow.CreateInstance<LuaPipeline>();
        luap.titleContent = new GUIContent("LuaPipline");
        luap.luaSourcePath = EditorPrefs.GetString(luap.epLuaSourcePath, luap.luaSourcePath);
        luap.outputPath = EditorPrefs.GetString(luap.epOutputPath, luap.outputPath);
        luap.uploadToServer = EditorPrefs.GetBool(luap.epUploadToServer, luap.uploadToServer);
        uploadSever = (UploadSever)EditorPrefs.GetInt(epUploadServer, (int)uploadSever);
        luap.keepLocal = EditorPrefs.GetBool(luap.epKeepLocal, luap.keepLocal);
        luap.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Lua源文件目录：");
                EditorGUI.BeginChangeCheck();
                luaSourcePath = EditorGUILayout.TextField(luaSourcePath);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(epLuaSourcePath, luaSourcePath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                GUILayout.Label(string.Format("输出目录：{0}/", Application.persistentDataPath));
                outputPath = EditorGUILayout.TextField(outputPath);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetString(epOutputPath, outputPath);
                }

                if (GUILayout.Button("打开目录"))
                {
                    finalOutputPath = Path.Combine(Application.persistentDataPath, outputPath);
                    if (Directory.Exists(finalOutputPath) == false)
                    {
                        Directory.CreateDirectory(finalOutputPath);
                    }
                    Application.OpenURL(finalOutputPath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("是否上传服务器(若不上传，则一定在persistentDataPath保留)：");
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                uploadToServer = EditorGUILayout.Toggle(uploadToServer);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(epUploadToServer, uploadToServer);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (uploadToServer)
                {
                    GUILayout.Label("上传地址：");
                    EditorGUI.BeginChangeCheck();
                    uploadSever = (UploadSever)EditorGUILayout.EnumPopup(uploadSever, GUILayout.Width(100));
                    EditorGUILayout.LabelField(uploadServerURLs[(int)uploadSever]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetInt(epUploadServer, (int)uploadSever);
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("是否在persistentDataPath保留一份：");
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                if (uploadToServer == false)
                {
                    keepLocal = EditorGUILayout.Toggle(true);
                }
                else
                {
                    keepLocal = EditorGUILayout.Toggle(keepLocal);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(epKeepLocal, keepLocal);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Build Signed Lua"))
                {
                    GenerateSingedLua();
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    public void GenerateSingedLua()
    {
        string encryptExe = Path.Combine(Path.GetFullPath("."), "Tools/FilesSignature.exe");
        string encryptExeDir = Path.GetDirectoryName(encryptExe);
        string finalSourcePath = Path.Combine(Path.GetFullPath("."), luaSourcePath);
        finalOutputPath = Path.Combine(Application.persistentDataPath, outputPath).Replace("\\", "/");
        if (Directory.Exists(finalOutputPath))
        {
            Directory.Delete(finalOutputPath, true);
        }

        Process process = new Process();
        process.StartInfo.FileName = encryptExe;
        process.StartInfo.Arguments = string.Format("{0} {1}", finalSourcePath, finalOutputPath);
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.WorkingDirectory = encryptExeDir;
        process.Start();
        process.WaitForExit();
        UnityEngine.Debug.Log("Build Signed Lua Finish : " + process.StandardOutput.ReadToEnd());
        
        HashUtil.ComputeVersionAndHash(finalOutputPath, "versionInfo.txt");

        if (uploadToServer)
        {
            UploadToServer();
            if (keepLocal == false)
            {
                Directory.Delete(finalOutputPath, true);
            }
        }
        EditorUtility.DisplayDialog("", "Build Signed Lua完成", "OK");
    }

    /// <summary>
    /// 将签名后的Lua脚本上传到服务器(Lua脚本量小，压缩到上传不会太久，就不改造后台上传了）
    /// </summary>
    private void UploadToServer()
    {
        FastZip fastZip = new FastZip();
        string zipFile = Path.Combine(Application.persistentDataPath, "Lua.zip");
        fastZip.CreateZip(zipFile, Application.persistentDataPath, true, @"+\.lua$;+\.txt$", "Lua");

        WebClient Client = new WebClient();
        Client.Headers.Add("Content-Type", "binary/octet-stream");

        string platform = "unknow";
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.WindowsEditor)
        {
            platform = "Android";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXEditor)
        {
            platform = "iOS";
        }
        byte[] result = Client.UploadFile(uploadServerURLs[(int)uploadSever] + "?platform=" + platform, "POST", zipFile);

        string s = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
        UnityEngine.Debug.Log("文件上传结果：" + s);

        File.Delete(zipFile);
    }
}

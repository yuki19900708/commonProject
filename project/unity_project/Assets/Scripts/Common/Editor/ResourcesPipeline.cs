using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

public class ResourcesPipeline : EditorWindow
{
    [MenuItem("Appcpi/数据生成/更新Resources资源（自动更新）")]
    public static void Init()
    {
        ResourcesPipeline resp = EditorWindow.CreateInstance<ResourcesPipeline>();
        resp.titleContent = new GUIContent("ResourcesPipline");
        resp.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("生成resIni"))
                {
                    GenerateResourcesIni();
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    public static void GenerateResourcesIni()
    {
        string resourcesPath = Application.dataPath + "/Resources";
        DirectoryInfo resourcesDir = new DirectoryInfo(resourcesPath);
        if (!resourcesDir.Exists)
        {
            return;
        }
        string resourcesIni = Path.Combine(resourcesPath, "resIni.txt");

        FileInfo[] files = resourcesDir.GetFiles("*", SearchOption.AllDirectories);
        StringBuilder sb = new StringBuilder(1000);
        for (var i = 0; i < files.Length; ++i)
        {
            FileInfo fileInfo = files[i];
            if (fileInfo.Name.EndsWith(".meta") || fileInfo.Name.Equals("resIni.txt"))
            {
                continue;
            }

            string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            string filePathWithoutName = fileInfo.DirectoryName.Replace("\\", "/").Replace(resourcesPath, "").Trim('/');
            sb.Append(fileName);
            sb.Append("|");
            sb.Append(filePathWithoutName);
            if (i < files.Length -1)
            {
                sb.AppendLine();
            }
        }
        File.WriteAllText(resourcesIni, sb.ToString());
        AssetDatabase.Refresh();
    }
}

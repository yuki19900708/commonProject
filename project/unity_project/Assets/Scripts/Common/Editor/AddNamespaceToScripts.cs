using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;

/// <summary>
/// 给使用Unity菜单生成的cs文件添加名空间
/// </summary>
public class AddNamespaceToScripts : UnityEditor.AssetModificationProcessor
{
    /// <summary>
    /// 请将YourCompanyName改为你的公司名，YourProjectName改为你的项目名
    /// </summary>
    public static readonly string DefaultNameSpace = "Appcpi.NewGame";
    /// <summary>
    /// 限制自动添加名空间的路径范围
    /// </summary>
    public static readonly string RestrictScriptPath = "Assets/Scripts/Game/Change_This_Name";

    public static void OnWillCreateAsset(string path)
    {
        //检查是否将模版名字改掉（这里的YourCompanyName和YourProjectName不要改）
        if (DefaultNameSpace == "YourCompanyName.YourProjectName")
        {
            Debug.LogWarning("请将变量DefaultNameSpace中的YourCompanyName改为你的公司名，YourProjectName改为你的项目名");
        }

        bool isMetaFile = path.EndsWith(".meta");
        path = path.Replace("\\", "/");
        bool isInEditor = (from folder in path.Split('/') where folder.Equals("Editor") select folder).Count() > 0;
        bool isInDataTable = (from folder in path.Split('/') where folder.Equals("DataTable") select folder).Count() > 0;

        bool isInRestrictPath = path.Contains(RestrictScriptPath);
        path = path.Replace(".meta", "");
        string fileName = Path.GetFileName(path);
        bool isCSFile = fileName.EndsWith(".cs");

        //排除编辑器脚本，正在导入的数据表脚本,排除meta，限制范围，排除非cs脚本
        if (isInEditor || isInDataTable  || isMetaFile == false || isInRestrictPath == false || isCSFile == false)
        {
            return;
        }

        int index = Application.dataPath.LastIndexOf("Assets");
        path = Application.dataPath.Substring(0, index) + path;
        string fileContent = File.ReadAllText(path);
        string[] fileContents = fileContent.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

        //排除已经加了名空间或者行数不为16行的（16行是Unity自带的C#脚本模版的行数）
        //这些判断并不完善，请根据实际情况来添加各种判断条件
        if (fileContent.Contains("namespace") || fileContents.Length != 16)
        {
            return;
        }

        StringBuilder sb = new StringBuilder(200);
        int classStartLine = 4;
        for (int i = 0; i < fileContents.Length; i++)
        {
            string lineContent = fileContents[i];
            if (i < classStartLine)
            {
                sb.AppendLine(lineContent);
            }
            else if (i == classStartLine)
            {
                sb.AppendLine(string.Format("namespace {0}", DefaultNameSpace));
                sb.AppendLine("{");
                sb.AppendLine("    " + lineContent);
            }
            else if (i > classStartLine)
            {
                if (i == fileContents.Length - 1)
                {
                    sb.AppendLine("}");
                }
                else
                {
                    sb.AppendLine("    " + lineContent);
                }
            }
        }

        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
    }
}

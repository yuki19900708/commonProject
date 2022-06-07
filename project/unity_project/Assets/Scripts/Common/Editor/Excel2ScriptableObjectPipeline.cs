using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Text;
//using System.Data;
using System.Reflection;
//using Excel;
using System.Text.RegularExpressions;
using SimpleJson;
using ICSharpCode.SharpZipLib.Zip;

public class SaveAssetInfo
{
    public FieldInfo info;
    public object obj;
}

public class FieldFormat
{
    public string variableName;
    public string type;
    public string description;
}

public class Excel2ScriptableObjectPipeline : EditorWindow
{
    private static string[] lineNumberNames = new string[7] { "1", "2", "3", "4", "5", "6", "7" };

    //excel表中行号
    private static int[] excelLineNumber = new int[7] { 1, 2, 3, 4, 5, 6, 7 };

    //程序实际使用的行号
    private static int[] lineNumber = new int[7] { 0, 1, 2, 3, 4, 5, 6 };

    private const string SAVE_ASSET_PATH = "/ResourcesRaw/DataAsset/";

    private const string ASSETBUNDLE_WORKING_PATH = "Assets/ResourcesRaw/DataAsset/";

    private const string CLIENT_CACHE_PATH = "../../output/buildDataCache_{0}.txt";

    private const string SERVER_CACHE_PATH = "../../output/serverDataCache_{0}.txt";

    private const string EXCEL_PATH = "../../design_asset/datatable/";

    private const string ASSET_MODULE_PATH = "Assets/Scripts/Common/ScriptModule/AssetModule.txt";

    private const string CLASS_MODULE_PATH = "Assets/Scripts/Common/ScriptModule/ClassModule.txt";

    private const string CODE_MODULE_PATH = "Assets/Scripts/Common/ScriptModule/CodeModule.txt";

    private const string DATA_MGR_PATH = "Assets/Scripts/Common/ScriptModule/TableDataMgrModule.txt";

    private const string VARIABLE_PATH = "Assets/Scripts/Common/ScriptModule/VariableModule.txt";

    private const string FUNCTION_PATH = "Assets/Scripts/Common/ScriptModule/FunctionModule.txt";

    private const string EVENT_MGR_PATH = "Assets/Scripts/Common/ScriptModule/TableDataEventMgr.txt";

    private const int IMPORTABLE_VALUE = 1;

    private const int SERVER_VARIABLE_TYPE = 4;

    private const int SERVER_CONFIG_LINE_NO = 5;
    //数据表名称的格式
    private const string SHEET_NAME_PATERN = @"^[0-9_a-zA-Z]*$";

    private const string KEY_DESCRIPT = "KEY_DESCRIPT";

    private const string KEY_VARIABLE = "KEY_VARIABLE";

    private const string KEY_TYPE = "KEY_TYPE";

    private const string KEY_EXCEL_PATH = "KEY_EXCEL_PATH";

    private const string KEY_SCRIPT_PATH = "KEY_SCRIPT_PATH";
    /// <summary> 表头需要的行数;</summary>
    private const string KEY_ROW_COUNT = "KEY_ROW_COUNT";

    /// <summary> 策划数据表文件路径 </summary>
    private static string dataSourceFolderPath;

    private static string saveScriptsPath;

    /// <summary>当前这次build的数据文件的Hash</summary>
    private static List<string> dataHashList = new List<string>();

    //客户端数据文件的Hash缓存
    private static string[] clientDataHashCache = null;

    /// <summary>当前build的目标服务器</summary>
    private static string targetServer = "default";

    private static int descriptionLine = 1;

    private static int variableLine = 2;

    private static int typeLine = 3;

    private static int tableHeadNeedRows = 6;

    [MenuItem("Appcpi/数据生成/生成服务端用的数据表json", false, 5)]
    public static void BuildServerJson()
    {
        //List<string> allTablePaths = GetAllTablePaths(Path.Combine(Path.GetFullPath("."), EXCEL_PATH));
        //List<DataTable> allTableDatas = GetTableData(allTablePaths);
        //BuildServerData(allTableDatas);
    }

    [MenuItem("Appcpi/数据生成/2.更新数据表", false, 2)]
    public static void BuildTableData()
    {
        #region 设置初始值
        descriptionLine = EditorPrefs.GetInt(KEY_DESCRIPT, 1);
        variableLine = EditorPrefs.GetInt(KEY_VARIABLE, 2);
        typeLine = EditorPrefs.GetInt(KEY_TYPE, 3);
        tableHeadNeedRows = EditorPrefs.GetInt(KEY_ROW_COUNT, 6);
        dataSourceFolderPath = EditorPrefs.GetString(KEY_EXCEL_PATH);
        saveScriptsPath = EditorPrefs.GetString(KEY_SCRIPT_PATH);
        #endregion

        Excel2ScriptableObjectPipeline window = EditorWindow.CreateInstance<Excel2ScriptableObjectPipeline>();
        window.titleContent = new GUIContent("数据表导入");
        window.Show();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        {
            if (SetFieldValueValid() == false)
            {
                return;
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("excel的路径：" + dataSourceFolderPath);
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("选择数据表路径"))
            {
                if (string.IsNullOrEmpty(dataSourceFolderPath) == false)
                {
                    dataSourceFolderPath = EditorUtility.OpenFolderPanel("选择excel所在路径", dataSourceFolderPath, "");
                }
                else
                {
                    dataSourceFolderPath = EditorUtility.OpenFolderPanel("选择excel所在路径", Path.Combine(Path.GetFullPath("."), EXCEL_PATH), "");
                }
            }

            if (string.IsNullOrEmpty(dataSourceFolderPath))
            {
                GUILayout.Label(new GUIContent("选择excel表路径", EditorGUIUtility.FindTexture("console.erroricon")), EditorStyles.helpBox);
                return;
            }

            GUILayout.Space(30);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("脚本保存路径：" + saveScriptsPath);
            }
            GUILayout.EndHorizontal();

            Texture2D tex = EditorGUIUtility.FindTexture("console.warnicon");
            GUILayout.Label(new GUIContent(string.Format("保存路径必须{0}子目录", Application.dataPath + "/Scripts"), tex), EditorStyles.helpBox);
            if (GUILayout.Button("选择保存路径"))
            {
                saveScriptsPath = EditorUtility.OpenFolderPanel("选择保存路径", Application.dataPath + "/Scripts", "");
                if (string.IsNullOrEmpty(saveScriptsPath) == false)
                {
                    saveScriptsPath = saveScriptsPath.Replace("\\", "/") + "/";
                }
            }

            GUILayout.BeginHorizontal();
            {
                if (!string.IsNullOrEmpty(saveScriptsPath) && saveScriptsPath.Contains("Assets"))
                {
                    if (GUILayout.Button("导出所有表"))
                    {
                        BuildAllData();
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                if (!string.IsNullOrEmpty(saveScriptsPath) && saveScriptsPath.Contains("Assets"))
                {
                    if (GUILayout.Button("导出文本表,只导出scriptobject, 不导出脚本"))
                    {
                        BuildTextData();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private static bool SetFieldValueValid()
    {
        #region
        tableHeadNeedRows = EditorGUILayout.IntPopup(string.Format("表头需要几行:", "默认" + 6), tableHeadNeedRows, lineNumberNames, excelLineNumber);
        if (tableHeadNeedRows < 6)
        {
            GUILayout.Label(new GUIContent("不能小于6", EditorGUIUtility.FindTexture("console.erroricon")), EditorStyles.helpBox);
            return false;
        }

        descriptionLine = EditorGUILayout.IntPopup(string.Format("中文说明行号({0}):", "默认" + 2), descriptionLine, lineNumberNames, lineNumber);

        variableLine = (int)EditorGUILayout.IntPopup(string.Format("变量行号({0}):", "默认" + 3), variableLine, lineNumberNames, lineNumber);

        typeLine = (int)EditorGUILayout.IntPopup(string.Format("变量类型行号({0}):", "默认" + 4), typeLine, lineNumberNames, lineNumber);

        if (descriptionLine > tableHeadNeedRows || variableLine > tableHeadNeedRows || typeLine > tableHeadNeedRows)
        {
            GUILayout.Label(new GUIContent("配置填写错误, 表头行数不能比变量行数小", EditorGUIUtility.FindTexture("console.erroricon")), EditorStyles.helpBox);
            return false;
        }

        if (descriptionLine == variableLine || descriptionLine == typeLine || variableLine == typeLine)
        {
            GUILayout.Label(new GUIContent("三个变量行数不能两两相等", EditorGUIUtility.FindTexture("console.erroricon")), EditorStyles.helpBox);
            return false;
        }
        #endregion
        return true;
    }

    private static void BuildAllData()
    {
        if (string.IsNullOrEmpty(dataSourceFolderPath) == false)
        {
            try
            {
                bool importAll = false;
                EditorPrefs.SetInt(KEY_ROW_COUNT, tableHeadNeedRows);
                EditorPrefs.SetInt(KEY_DESCRIPT, descriptionLine);
                EditorPrefs.SetInt(KEY_VARIABLE, variableLine);
                EditorPrefs.SetInt(KEY_TYPE, typeLine);
                EditorPrefs.SetString(KEY_EXCEL_PATH, dataSourceFolderPath);
                EditorPrefs.SetString(KEY_SCRIPT_PATH, saveScriptsPath);

                List<string> allTablePaths = GetAllTablePaths(dataSourceFolderPath);

                //List<DataTable> allTableDatas = GetTableData(allTablePaths);

                #region 判断需要更新的数据
                List<string> updateTablePaths = new List<string>();
                clientDataHashCache = GetClientDataHashCache();
                dataHashList.Clear();

                //获取该表中Code不同的表进行更新
                foreach (string tablePath in allTablePaths)
                {
                    bool isNeedUpdate = false;
                    isNeedUpdate |= IsTableDataChanged(tablePath, clientDataHashCache, importAll);
                    if (isNeedUpdate)
                    {
                        updateTablePaths.Add(tablePath);
                    }
                }
                #endregion

                //BuildMgrScript(allTableDatas);

                #region 生成asset
                //如果不导表，直接生成asset;否则缓存变量等编译结束在生成asset
                if (updateTablePaths.Count != 0)
                {
                    //List<DataTable> updateTabs = GetTableData(updateTablePaths);
                    //BuildScript(updateTabs);

                    //int maxProgress = updateTabs.Count;
                    //int curProgress = 0;

                    //foreach (DataTable table in updateTabs)
                    //{
                    //    curProgress++;
                    //    ShowProgressBar(curProgress, maxProgress);
                    //    CreateAsset(table);
                    //}
                    UpdateBuildDataCache();
                    //BuildServerData(updateTabs);
                }
                AssetBundlePipeline.SetAssetBundleName(ASSETBUNDLE_WORKING_PATH);
                #endregion
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        else
        {
            Debug.LogError(string.Format("输入参数有误：dataTableSource：{0}", dataSourceFolderPath));
        }
    }

    private static void BuildTextData()
    {
        if (string.IsNullOrEmpty(dataSourceFolderPath) == false)
        {
            try
            {
                List<string> allTablePaths = GetAllTablePaths(dataSourceFolderPath);
                string textTablePath = "";
                int sameNameCount = 0;
                foreach (string name in allTablePaths)
                {
                    if(name.Contains("文本表.xlsm"))
                    {
                        textTablePath = name;
                        sameNameCount++;
                    }
                }
                if(string.IsNullOrEmpty(textTablePath) || sameNameCount!=1)
                {
                    Debug.LogError("检查表名是否为文本表.xlsm");
                    Debug.LogError("检查其他表名是否包含文本表.xlsm字符串");
                    return;
                }
                #region 生成asset
                //List<DataTable> updateTabs = GetTableData(new List<string> { textTablePath });
                //foreach (DataTable table in updateTabs)
                //{
                //    CreateAsset(table);
                //}
                AssetBundlePipeline.SetAssetBundleName(ASSETBUNDLE_WORKING_PATH);
                #endregion
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        else
        {
            Debug.LogError(string.Format("输入参数有误：dataTableSource：{0}", dataSourceFolderPath));
        }
    }


    //private static void BuildMgrScript(List<DataTable> tables)
    //{
    //    TextAsset codeText = AssetDatabase.LoadAssetAtPath(CODE_MODULE_PATH, typeof(TextAsset)) as TextAsset;

    //    TextAsset variableText = AssetDatabase.LoadAssetAtPath(VARIABLE_PATH, typeof(TextAsset)) as TextAsset;

    //    string enumName = null;
    //    string allVariableStr = null;
    //    string contentStr = null;
    //    string eventContent = ScriptStringModule.eventFormat;
    //    string allEvent = null;
    //    foreach (DataTable data in tables)
    //    {
    //        string scriptName = char.ToUpper(data.TableName[0]) + data.TableName.Substring(1);
    //        string varialName = char.ToLower(data.TableName[0])+ data.TableName.Substring(1);
    //        enumName += scriptName + "Table,\r\n    ";

    //        allEvent += eventContent.Replace("#Content#", scriptName + "Table");
    //        allEvent += "\r\n    ";
    //        allVariableStr += variableText.text.Replace("#EnumName#", scriptName).Replace("#VarialName#", varialName);
    //        string[] sArray=scriptName.Split('_');// 一定是单引 

    //        string dataName = sArray[0];

    //        contentStr += codeText.text.Replace("#TypeName#", scriptName).Replace("#DataName#", dataName) + "\r\n";
    //    }

    //    string filePath = saveScriptsPath + "TableDataMgr" + ".cs";

    //    string dir = Path.GetDirectoryName(filePath);
    //    if (!Directory.Exists(dir))
    //    {
    //        Directory.CreateDirectory(dir);
    //    }

    //    TextAsset text = AssetDatabase.LoadAssetAtPath(DATA_MGR_PATH, typeof(TextAsset)) as TextAsset;
    //    string content = text.text.Replace("#EnumName#", enumName);
    //    content = content.Replace("#Variable#", allVariableStr);
    //    content = content.Replace("#Content#", contentStr);
    //    File.WriteAllText(filePath, content, new UTF8Encoding(true));

    //    string eventFilePath = saveScriptsPath + "TableDataEventMgr" + ".cs";

    //    if (!Directory.Exists(Path.GetDirectoryName(eventFilePath)))
    //    {
    //        Directory.CreateDirectory(Path.GetDirectoryName(eventFilePath));
    //    }
    //    TextAsset eventText = AssetDatabase.LoadAssetAtPath(EVENT_MGR_PATH, typeof(TextAsset)) as TextAsset;
    //    string contentEvent = eventText.text.Replace("#Content#", allEvent);

    //    File.WriteAllText(eventFilePath, contentEvent, new UTF8Encoding(true));

    //}

    //private static void BuildScript(List<DataTable> tables)
    //{
    //    foreach (DataTable data in tables)
    //    {
    //        CreateScript(data);
    //    }
    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh();
    //}

    //private static void CreateScript(DataTable data)
    //{
    //    DataRow descriptionRow = data.Rows[descriptionLine];
    //    DataRow nameRow = data.Rows[variableLine];
    //    DataRow typeRow = data.Rows[typeLine];

    //    List<FieldFormat> list = new List<FieldFormat>();
    //    bool containsID = false;

    //    foreach (DataColumn column in data.Columns)
    //    {
    //        if (IsColumnValueValid(descriptionRow[column].ToString(), ""))
    //        {
    //            FieldFormat def = new FieldFormat();
    //            def.description = descriptionRow[column].ToString();
    //            def.variableName = nameRow[column].ToString();
    //            def.type = typeRow[column].ToString();
    //            if (def.type == "boolean")
    //            {
    //                def.type = "bool";
    //            }
    //            if (def.type == "ItemRate")
    //            {
    //                def.type = "OutputData";
    //            }
    //            if (def.type == "Item")
    //            {
    //                def.type = "int[]";
    //            }
    //            list.Add(def); 
    //            if (string.Equals(def.variableName, "id"))
    //            {
    //                containsID = true;
    //            }
    //        }
    //    }

    //    if (list == null)
    //    {
    //        Debug.LogErrorFormat("检查{0}表", data.TableName);
    //        return;
    //    }

    //    string scriptName = char.ToUpper(data.TableName[0]) + data.TableName.Substring(1);

    //    string filePath = saveScriptsPath + scriptName + ".cs";

    //    SaveScript(list, filePath, scriptName, containsID);
    //}

    private static void SaveScript(List<FieldFormat> list, string filePath, string scriptName, bool containsId)
    {
        string listName = char.ToLower(scriptName[0]) + scriptName.Substring(1);

        //创建代码//
        StringBuilder sb = new StringBuilder();
        foreach (FieldFormat filed in list)
        {
            sb.AppendFormat(ScriptStringModule.fieldFormat, filed.description, filed.type, filed.variableName);
        }

        string dataName = scriptName;
        if (scriptName.Contains("_"))
        {
            string[] sArray = scriptName.Split('_');
            dataName = sArray[0];
        }
        filePath = saveScriptsPath + dataName + ".cs";
        //生成数据类
        TextAsset classText = AssetDatabase.LoadAssetAtPath(CLASS_MODULE_PATH, typeof(TextAsset)) as TextAsset;

        string classContent = classText.text.Replace("#ScriptName#", dataName);
            //.ScriptStringModule.classModule.Replace("#ScriptName#", dataName);
        classContent = classContent.Replace("#Content#", sb.ToString());
        File.WriteAllText(filePath, classContent, new UTF8Encoding(true));

        filePath = saveScriptsPath + scriptName + ".cs";

        TextAsset assetText = AssetDatabase.LoadAssetAtPath(ASSET_MODULE_PATH, typeof(TextAsset)) as TextAsset;
        TextAsset functionText = AssetDatabase.LoadAssetAtPath(FUNCTION_PATH, typeof(TextAsset)) as TextAsset;

        string assetContent = assetText.text.Replace("#ScriptName#", scriptName);
        assetContent = assetContent.Replace("#ListName#", listName);
        assetContent = assetContent.Replace("#DataName#", dataName);

        if (containsId)
        {
            string function = functionText.text.Replace("#ScriptName#", scriptName);
            function = function.Replace("#ListName#", listName);
            function = function.Replace("#DataName#", dataName);
            assetContent = assetContent.Replace("#Content#", function);
        }
        else
        {
            assetContent = assetContent.Replace("#Content#", "return null;");
        }
        File.WriteAllText(filePath.Replace(scriptName, scriptName + "Table"), assetContent, new UTF8Encoding(true));
    }

    private static void BuildAsset(string excelFilePath)
    {
        //List<string> fileNames = GetAllTablePaths(excelFilePath);
        //List<DataTable> allTableDatas = GetTableData(fileNames);
        //int maxProgress = fileNames.Count;
        //int curProgress = 0;
        //bool isBuilding = true;

        //foreach (DataTable data in allTableDatas)
        //{
        //    CreateAsset(data);
        //    curProgress++;
        //    if (isBuilding)
        //    {
        //        ShowProgressBar(curProgress, maxProgress);
        //    }
        //}
        //isBuilding = false;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //private static void CreateAsset(DataTable data)
    //{
    //    string assetName = char.ToUpper(data.TableName[0]) + data.TableName.Substring(1);
    //    string dataName = assetName;
    //    if (assetName.Contains("_"))
    //    {
    //        string[] sArray = assetName.Split('_');
    //        dataName = sArray[0];
    //    }
    //    string filePath = Application.dataPath + SAVE_ASSET_PATH + assetName + "Table.asset";
    //    List<List<SaveAssetInfo>> assetData = new List<List<SaveAssetInfo>>();
    //    Assembly assem = Assembly.Load("Assembly-CSharp");
    //    DataRow variableRow = data.Rows[variableLine];
    //    DataRow typeRow = data.Rows[typeLine];
    //    DataRow descriptionRow = data.Rows[descriptionLine];

    //    Type typeName = assem.GetType(dataName==null? assetName:dataName, true);

    //    FieldInfo[] infos = typeName.GetFields();
    //    for (int i = tableHeadNeedRows; i < data.Rows.Count; i++)
    //    {
    //        List<SaveAssetInfo> list = new List<SaveAssetInfo>();
    //        var row = data.Rows[i];

    //        foreach (DataColumn column in data.Columns)
    //        {
    //            if (IsColumnValueValid(descriptionRow[column].ToString(), ""))
    //            {
    //                object value = row[column];
    //                string name = variableRow[column].ToString();
    //                string type = typeRow[column].ToString();
    //                if (type == "boolean")
    //                {
    //                    type = "bool";
    //                }
    //                if (type == "ItemRate")
    //                {
    //                    type = "OutputData";
    //                }
    //                if (type == "Item")
    //                {
    //                    type = "int[]";
    //                }
    //                value = ResolveTypeUtil.GetType(type, value, data.TableName);

    //                SaveAssetInfo info = new SaveAssetInfo();
    //                for (int j = 0; j < infos.Length; j++)
    //                {
    //                    if (infos[j].Name == name)
    //                    {
    //                        info.info = infos[j];
    //                    }
    //                }
    //                info.obj = value;
    //                list.Add(info);
    //            }
    //        }
    //        assetData.Add(list);
    //    }
    //    SaveAsset(assem, typeName, assetData, filePath);
    //}

    private static void SaveAsset(Assembly assem, Type scriptType, List<List<SaveAssetInfo>> data, string filePath)
    {
        if (data == null)
        {
            return;
        }
        //文件名 也是类名//
        string defName = Path.GetFileNameWithoutExtension(filePath);

        defName.Replace("Table", "");
        //要保存的路径//
        string dir = Path.GetDirectoryName(filePath);
        ///当路径不存在就创建这个路径//
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        List<object> list = new List<object>();
        foreach (List<SaveAssetInfo> dic in data)
        {
            //通过反射创建一个单个保存的对象//
            object obj = Activator.CreateInstance(scriptType);

            foreach (SaveAssetInfo info in dic)
            {
                //通过反射 设置这个对象的值//
                info.info.SetValue(obj, info.obj);
            }
            //把单个数据保存到 list中//
            list.Add(obj);
        }

        //通过反射拿到要生成asset的类//
        Type Serialize = assem.GetType(defName);
        //根据类名创建一个ScriptableObject 实例//
        object serializeObj = ScriptableObject.CreateInstance(Serialize);
        //从要生成asset的类中反射获取一个叫 SetDatas的函数//
        MethodInfo mi = Serialize.GetMethod("SetDatas");

        //调用 asset对象中的这个函数 传入我们上一步保存的数据//
        mi.Invoke(serializeObj, new object[] { list.ToArray() });
        AssetDatabase.CreateAsset(serializeObj as UnityEngine.Object, FileUtil.GetProjectRelativePath(filePath));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //private static void BuildServerData(List<DataTable> tableDatas)
    //{
    //    string jsonFolder = "../../output/ServerJson/";
    //    if (Directory.Exists(jsonFolder) == false)
    //    {
    //        Directory.CreateDirectory(jsonFolder);
    //    }
    //    foreach (DataTable data in tableDatas)
    //    {
    //        string serveDataCacheFile = Path.Combine(Path.GetFullPath("."), string.Format("../../output/ServerJson/{0}.json", data.TableName));
    //        if (File.Exists(serveDataCacheFile) == false)
    //        {
    //            File.Create(serveDataCacheFile).Dispose();
    //        }

    //        JsonObject jsonObj = new JsonObject();
    //        jsonObj.Add("list", CreateServerData(data));

    //        File.WriteAllText(serveDataCacheFile, SimpleJson.SimpleJson.SerializeObject(jsonObj));
    //    }

    //    FastZip fastZip = new FastZip();
    //    string filePath = Path.Combine(Path.GetFullPath("."), "../../output/ServerJson/");
    //    string zipFile = Path.Combine(Path.GetFullPath("."), "../../output/output.zip");
    //    fastZip.CreateZip(zipFile, filePath, true, @"\w+");
    //}

    //private static JsonArray CreateServerData(DataTable data)
    //{
    //    DataRow typeRow = data.Rows[typeLine];
    //    DataRow descriptionRow = data.Rows[descriptionLine];
    //    DataRow serverLimit = data.Rows[SERVER_CONFIG_LINE_NO];
    //    DataRow serverVariableType = data.Rows[SERVER_VARIABLE_TYPE];

    //    JsonArray jsonArray= new JsonArray();
    //    for (int i = tableHeadNeedRows; i < data.Rows.Count; i++)
    //    {
    //        JsonObject jsonObj = new JsonObject();
    //        foreach (DataColumn column in data.Columns)
    //        {
    //            if (descriptionRow[column].ToString().Contains("#") == false)
    //            {
    //                if (string.IsNullOrEmpty(serverLimit[column].ToString()) == false)
    //                {
    //                    object obj = ResolveTypeUtil.GetServerType(string.IsNullOrEmpty(serverVariableType[column].ToString()) == true ? typeRow[column].ToString() : serverVariableType[column].ToString(), data.Rows[i][column], serverLimit[column].ToString()+ "*"+ data.TableName, i);
    //                    if (obj != null)
    //                    {
    //                        jsonObj.Add(serverLimit[column].ToString(), obj);
    //                    }
    //                }
    //            }
    //        }
    //        jsonArray.Add(jsonObj);
    //    }

    //    return jsonArray;

    //}

    //private static List<DataTable> GetTableData(List<string> excelFilePath)
    //{
    //    List<DataTable> list = new List<DataTable>();
    //    int index = 0;
    //    try
    //    {
    //        foreach (string fileName in excelFilePath)
    //        {
    //            FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);
    //            //IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
    //            //DataSet result = excelReader.AsDataSet();

    //            //foreach (DataTable data in result.Tables)
    //            //{
    //            //    if (data != null && data.Rows.Count >= tableHeadNeedRows && data.Columns.Count > 0)
    //            //    {
    //            //        if (Regex.IsMatch(data.TableName, SHEET_NAME_PATERN))
    //            //        {
    //            //            list.Add(data);
    //            //        }
    //            //    }
    //            //    else
    //            //    {
    //            //        if (data.TableName.Contains("!") == false)
    //            //        {
    //            //            Debug.LogErrorFormat("{0}表有问题", fileName);
    //            //        }
    //            //    }
    //            //}
    //            Debug.LogFormat("{0}表成功获取", fileName);
    //            stream.Close();
    //            index++;
    //        }
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Debug.LogErrorFormat("{0}表有问题", excelFilePath[index]);
    //        Debug.Log(ex.ToString());
    //    }
    //    return list;
    //}

    private static List<string> GetAllTablePaths(string path)
    {
        List<string> list = new List<string>();
        string[] files = Directory.GetFiles(path);
        foreach (string str in files)
        {
            string fileName = str.Replace("\\", "/");

            if ((fileName.EndsWith(".xls") || fileName.EndsWith(".xlsx") || fileName.EndsWith(".xlsm")) && !Path.GetFileName(fileName).StartsWith("~$"))
            {
                list.Add(fileName);
            }
            else
            {
                if(fileName.EndsWith(".gitkeep") == false)
                {
                    Debug.LogError("检查文件后缀是否正确, 如果正确让程序添加新的文件后缀名称" + fileName);
                }
            }
        }
        return list;
    }

    private static string[] GetClientDataHashCache()
    {
        string cachePath = Path.Combine(Path.GetFullPath("."), string.Format(CLIENT_CACHE_PATH, targetServer));
        if (File.Exists(cachePath))
        {
            return File.ReadAllLines(cachePath);
        }
        else
        {
            return null;
        }
    }

    private static bool IsTableDataChanged(string tableFullPath, string[] cacheData, bool importAll)
    {
        HashAlgorithm sha1 = HashAlgorithm.Create();
        string fileHash = null;

        using (FileStream stream = new FileStream(tableFullPath, FileMode.Open, FileAccess.Read))
        {
            fileHash = BitConverter.ToString(sha1.ComputeHash(stream));
            fileHash += Path.GetFileName(tableFullPath);
            if (dataHashList.Contains(fileHash) == false)
            {
                dataHashList.Add(fileHash);
            }
        }

        if (importAll)
        {
            return true;
        }
        else
        {
            //缓存与当前数据不匹配返回-1；
            if (cacheData == null)
            {
                return true;
            }
            else
            {
                //返回数组中第一个找到该元素的位置如果小于零那么没找到
                int fileIndex = Array.IndexOf(cacheData, fileHash);
                return fileIndex < 0;
            }
        }

    }

    //private static bool CheckTableValid(DataTable data)
    //{
    //    if (Regex.IsMatch(data.TableName, SHEET_NAME_PATERN))
    //    {
    //        return true;
    //    }
    //    DataRow serverLimit = data.Rows[SERVER_CONFIG_LINE_NO];
    //    //foreach (DataColumn column in data.Columns)
    //    //{
    //    //    if (string.IsNullOrEmpty(serverLimit[column].ToString()) || int.Parse(serverLimit[column].ToString()) != IMPORTABLE_VALUE)
    //    //    {
    //    //        return true;

    //    //    }
    //    //}
    //    return false;
    //}
	
    private static bool IsColumnValueValid(string decriptColumn, string columnValue)
    {
        if (decriptColumn.Contains("#") == false)
        {
            if (string.IsNullOrEmpty(columnValue) || int.Parse(columnValue) != IMPORTABLE_VALUE)
            {
                return true;
            }
        }
        return false;
    }

    private static void UpdateBuildDataCache()
    {
        string buildDataCacheFile = Path.Combine(Path.GetFullPath("."), string.Format(CLIENT_CACHE_PATH, targetServer));
        if (File.Exists(buildDataCacheFile) == false)
        {
            File.Create(buildDataCacheFile).Dispose();
        }
        File.WriteAllLines(buildDataCacheFile, dataHashList.ToArray(), Encoding.UTF8);
    }

    private static void ShowProgressBar(int curProgress, int maxProgress)
    {
        if (curProgress < maxProgress)
        {
            EditorUtility.DisplayProgressBar("生成Asset文件", "正在将数据表生成asset文件 请稍后......", (float)curProgress / maxProgress);
        }
        else
        {
            EditorUtility.ClearProgressBar();
        }
    }
}

using UnityEditor;

public class ResourcesPostprocessor : AssetPostprocessor
{
    public static bool enabled = true;

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (enabled == false)
        {
            return;
        }
        bool isResourcesDirty = false;
        foreach (string str in importedAssets)
        {
            if (str.StartsWith("Assets/Resources/"))
            {
                if (str.EndsWith("resIni.txt"))
                {
                    continue;
                }
                isResourcesDirty = true;
                break;
            }
        }

        if (isResourcesDirty == false)
        {
            foreach (string str in deletedAssets)
            {
                if (str.EndsWith("resIni.txt"))
                {
                    continue;
                }
                if (str.StartsWith("Assets/Resources/"))
                {
                    isResourcesDirty = true;
                    break;
                }
            }
        }

        if (isResourcesDirty == false)
        {
            foreach (string str in movedAssets)
            {
                if (str.EndsWith("resIni.txt"))
                {
                    continue;
                }
                if (str.StartsWith("Assets/Resources/"))
                {
                    isResourcesDirty = true;
                    break;
                }
            }
        }

        if (isResourcesDirty)
        {
            UnityEngine.Debug.Log("检测到Resources目录变化，更新resIni！");
            ResourcesPipeline.GenerateResourcesIni();
        }
    }
}

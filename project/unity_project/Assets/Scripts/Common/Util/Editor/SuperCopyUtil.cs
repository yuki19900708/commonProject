using UnityEngine;

public static class SuperCopyUtil
{
    private static Component[] copyComponents;
    private static Component[] pasteComponents;

    /// <summary>
    /// 复制与粘贴
    /// </summary>
    /// <param name="copy">需要复制的物体</param>
    /// <param name="paste">需要粘贴的物体</param>
    public static void CopyAndPaste(GameObject copy, GameObject paste)
    {
        bool canCopyAndPaste = CheckCanCopyAndPaste(copy, paste, copyComponents);
        if (canCopyAndPaste)
        {
            for (int i = 0; i < copyComponents.Length; i++)
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(copyComponents[i]);
                UnityEditorInternal.ComponentUtility.PasteComponentValues(pasteComponents[i]);
            }
            Debug.Log("Paste Success！");
        }
        else
        {
            copyComponents = null;
            pasteComponents = null;
        }
    }

    private static bool CheckCanCopyAndPaste(GameObject copy, GameObject paste, Component[] copyComponents)
    {
        if (copy == null)
        {
            Debug.LogError("Copy GameObject is null！");
            return false;
        }
        if (paste == null)
        {
            Debug.LogError("Paste GameObject is null！");
            return false;
        }
        copyComponents = copy.GetComponentsInChildren<Component>();
        if (copyComponents != null && copyComponents.Length > 0)
        {
            Debug.Log("Copy Success !");
        }
        else
        {
            Debug.LogError("无法获取复制的组件列表，复制失败！");
            return false;
        }
        Component[] pasteComponents = paste.GetComponentsInChildren<Component>();
        if (pasteComponents != null && pasteComponents.Length > 0)
        {
            if (pasteComponents.Length == copyComponents.Length)
            {
                for (int i = 0; i < copyComponents.Length; i++)
                {
                    if (copyComponents[i].GetType() != pasteComponents[i].GetType())
                    {
                        string log = string.Format("存在不同的组件类型，复制：{0} ；粘贴：{1}",
                            copyComponents[i], pasteComponents[i]);
                        Debug.LogError(log);
                        return false;
                    }
                }
                return true;
            }
            else
            {
                Debug.LogError("复制与粘贴物体组件数量不同，粘贴失败!");
                return false;
            }
        }
        else
        {
            Debug.LogError("无法获取粘贴的组件列表，粘贴失败");
            return false;
        }
    }

}

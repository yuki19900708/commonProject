using UnityEditor;
using UnityEngine;

public class SuperCopyEditor
{
    private static Component[] copyComponents;

    [MenuItem("CONTEXT/Component/Copy AllComponents")]
    private static void CopyAllComponent(MenuCommand command)
    {
        copyComponents = GetChildrenComponents(command);
        if (copyComponents != null && copyComponents.Length > 0)
        {
            Debug.Log("Copy Success!");
        }
    }

    [MenuItem("CONTEXT/Component/Copy AllComponents", true)]
    private static bool CheckCanCopy()
    {
        GameObject[] selectedObject = Selection.GetFiltered<GameObject>(SelectionMode.Unfiltered);
        return selectedObject.Length == 1;
    }

    [MenuItem("CONTEXT/Component/Paste AllComponents")]
    private static void PasteAllComponent(MenuCommand command)
    {
        Component[] pasteComponents = GetChildrenComponents(command);
        for (int i = 0; i < pasteComponents.Length; i++)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(copyComponents[i]);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(pasteComponents[i]);
        }
        Debug.Log("Paste Success!");
    }

    [MenuItem("CONTEXT/Component/Paste AllComponents",true)]
    private static bool CheckCanPaste(MenuCommand command)
    {
        GameObject[] selectedObject = Selection.GetFiltered<GameObject>(SelectionMode.Unfiltered);
        Component[] currentComponent = GetChildrenComponents(command);
        if (selectedObject.Length == 1)
        {
            if (copyComponents != null && copyComponents.Length > 0)
            {
                if (currentComponent.Length == copyComponents.Length)
                {
                    for (int i = 0; i < currentComponent.Length; i++)
                    {
                        if (currentComponent[i].GetType() != copyComponents[i].GetType())
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private static Component[] GetChildrenComponents(MenuCommand command)
    {
        Component currentSelect = (Component)command.context;
        return currentSelect.GetComponentsInChildren<Component>();
    }

}

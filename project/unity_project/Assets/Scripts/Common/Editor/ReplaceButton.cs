using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class ReplaceButton : Editor
{
    static int count = 0;
    [MenuItem("GameObject/ReplaceButton", priority = 0)]
    public static void ReplaceItemButton()
    {
        GameObject[] gos = Selection.gameObjects;
        count++;
        Debug.Log("马上开始" + count);
        if (count == gos.Length)
        {
            Debug.Log(gos.Length + "马上开始" + count);
            RunReplace(gos);
        }

    }

    static void RunReplace(GameObject[] gos)
    {
        count = 0;
        foreach (GameObject go in gos)
        {
            RectTransform rectTran = go.GetComponent<RectTransform>();
            GameObject newGo = new GameObject();
            Image image = go.GetComponent<Image>();
            Image newGoImage = newGo.AddComponent<Image>();
            if (image == null)
            {
                image = go.transform.GetChild(0).GetComponent<Image>();
            }
            newGoImage.overrideSprite = image.overrideSprite;
            newGoImage.sprite = image.sprite;
            newGoImage.color = image.color;
            newGoImage.raycastTarget = false;
            newGoImage.preserveAspect = true;
            newGoImage.type = image.type;
            newGo.GetComponent<RectTransform>().sizeDelta = rectTran.sizeDelta;
            //newGoImage.SetNativeSize();
            newGo.name = "ButtonAnimation";

            newGo.transform.SetParent(go.transform);
            newGo.transform.localPosition = Vector3.zero;
            newGo.transform.localScale = Vector3.one;

            Transform[] trans = go.GetComponentsInChildren<Transform>();
            foreach (Transform tran in trans)
            {
                if (tran != go.transform && tran != newGo.transform)
                {
                    Debug.Log(tran.name);
                    tran.SetParent(newGo.transform, false);
                }
            }
            GameObject mask = new GameObject();
            Image maskImage = mask.AddComponent<Image>();
            maskImage.overrideSprite = image.overrideSprite;
            maskImage.sprite = image.sprite;
            maskImage.raycastTarget = false;
            maskImage.GetComponent<RectTransform>().sizeDelta = rectTran.sizeDelta;
            maskImage.name = "ImageMenCeng";
            maskImage.transform.SetParent(newGo.transform);
            maskImage.transform.localPosition = Vector3.zero;
            maskImage.transform.localScale = Vector3.one;
            maskImage.color = new Color(0, 0, 0, 0);
            DestroyImmediate(image, true);
            go.AddComponent<Empty4Raycast>();

            Animator anim = go.AddComponent<Animator>();
            string path = "Assets/ResourcesRaw/Animator/ButtonAnimator/ButtonTip.controller";
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
            anim.runtimeAnimatorController = controller;

            go.GetComponent<Button>().transition = Selectable.Transition.Animation;
        }
    }

    [MenuItem("GameObject/ReplaceTapButton", priority = 0)]
    public static void ReplaceTapButton()
    {
        GameObject[] gos = Selection.gameObjects;
        count++;
        Debug.Log("马上开始" + count);
        if (count == gos.Length)
        {
            Debug.Log(gos.Length + "马上开始" + count);
            RunTapReplace(gos);
        }

    }

    static void RunTapReplace(GameObject[] gos)
    {
        count = 0;
        foreach (GameObject go in gos)
        {
            RectTransform rectTran = go.GetComponent<RectTransform>();
            GameObject newGo = new GameObject();
            Image image = go.GetComponent<Image>();
            Image newGoImage = newGo.AddComponent<Image>();
            if (image == null)
            {
                image = go.transform.GetChild(0).GetComponent<Image>();
            }
            newGoImage.overrideSprite = image.overrideSprite;
            newGoImage.sprite = image.sprite;
            newGoImage.color = image.color;
            newGoImage.raycastTarget = false;
            newGoImage.preserveAspect = true;
            newGoImage.type = image.type;
            newGo.GetComponent<RectTransform>().sizeDelta = rectTran.sizeDelta;
            //newGoImage.SetNativeSize();
            newGo.name = "ButtonAnimation";

            newGo.transform.SetParent(go.transform);
            newGo.transform.localPosition = Vector3.zero;
            newGo.transform.localScale = Vector3.one;

            Transform[] trans = go.GetComponentsInChildren<Transform>();
            foreach (Transform tran in trans)
            {
                if (tran != go.transform && tran != newGo.transform)
                {
                    Debug.Log(tran.name);
                    tran.SetParent(newGo.transform, false);
                }
            }
            DestroyImmediate(image, true);
            go.AddComponent<Empty4Raycast>();

            Animator anim = go.AddComponent<Animator>();
            string path = "Assets/ResourcesRaw/Animator/ButtonAnimator/ButtonTapTip.controller";
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
            anim.runtimeAnimatorController = controller;

            go.GetComponent<Button>().transition = Selectable.Transition.Animation;
        }
    }

}

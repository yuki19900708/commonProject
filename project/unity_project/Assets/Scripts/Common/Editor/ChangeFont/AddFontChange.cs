using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AddFontChange : MonoBehaviour {



    [MenuItem("Assets/Tool/AddFontText")]
    static void AddFontText()
    {
        string[] files = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            Debug.Log(files[i]);
            string source = files[i].Replace(Application.dataPath, "Assets");

            Debug.Log(source);
            GameObject a = AssetDatabase.LoadAssetAtPath(source, typeof(GameObject)) as GameObject;
            if (a != null)
            {
                Text[] BothText = a.GetComponentsInChildren<Text>(true);
                if (BothText.Length > 0)
                {
                    for (int j = 0; j < BothText.Length; j++)
                    {
                        if (BothText[j].transform.GetComponent<FontChangeScript>() == null)
                        {
                            BothText[j].gameObject.AddComponent<FontChangeScript>();
                        }
                    }

                }
            }
        }
        AssetDatabase.SaveAssets();
    }
    [MenuItem("Assets/Tool/DelFontText")]
    static void DelFontText()
    {

        string[] files = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);

        //string[] scene = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            Debug.Log(files[i]);
            string source = files[i].Replace(Application.dataPath, "Assets");
            //string[] source = AssetDatabase.GetDependencies(new string[] { files[i].Replace(Application.dataPath, "Assets") });
            Debug.Log(source);
            GameObject a = AssetDatabase.LoadAssetAtPath(source, typeof(GameObject)) as GameObject;
            if (a != null)
            {
                Text[] BothText = a.GetComponentsInChildren<Text>(true);
                if (BothText.Length > 0)
                {
                    for (int j = 0; j < BothText.Length; j++)
                    {
                        if (BothText[j].transform.GetComponent<FontChangeScript>() != null)
                        {
                            DestroyImmediate(BothText[j].gameObject.GetComponent<FontChangeScript>(), true);//删除绑定脚本  
                        }
                    }

                }
            }
        }
        AssetDatabase.SaveAssets();
    }
}

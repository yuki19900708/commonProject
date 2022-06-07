using EditorTerrainModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Enter : MonoBehaviour {
	public Button enterBtn;
	public InputField x;
	public InputField y;
	// Use this for initialization
	void Start () {
		x.text = "100";
		y.text = "100";
		enterBtn.onClick.AddListener(() =>
		{
			if(string.IsNullOrEmpty(x.text) || string.IsNullOrEmpty(y.text))
            {
				return;
            }
			int xValue = int.Parse(x.text);
			int yValue = int.Parse(y.text);
			if (xValue <= 2 || yValue <= 2)
			{
				Debug.LogError(string.Format("输入x:{0},y:{1}不合法，请检查出入内容", xValue, yValue));
				return;
			}

			TerrainEditorModel.SetMapSize(xValue, yValue);
			SceneManager.LoadScene(1);
		});
	}
	
}

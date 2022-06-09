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
	public InputField phase;

	public Button close;
	public Text tipText;
	public GameObject tipPanel;

	// Use this for initialization
	void Start () {
		x.text = "100";
		y.text = "100";
		phase.text = "1";

		ShowTipPanel(false);
		close.onClick.AddListener(() =>
		{
				ShowTipPanel(false);
		});
		enterBtn.onClick.AddListener(() =>
		{
			if(string.IsNullOrEmpty(x.text) || string.IsNullOrEmpty(y.text))
            {
				return;
            }
			int xValue = int.Parse(x.text);
			int yValue = int.Parse(y.text);
			if (xValue < 2 || yValue < 2)
			{
					ShowTipPanel(true, "输入不合法，必须为数字且大于等于2");
				return;
			}

			int  phaseV = int.Parse(phase.text);
			if(phaseV <= 0)
			{
				ShowTipPanel(true, "输入土地期数不合法, 必须为数字且大于等于0");
				return;
			}
			
			TerrainEditorModel.SetMapSize(xValue, yValue, phaseV);
			SceneManager.LoadScene(1);
		});
	}


	void ShowTipPanel(bool show, string tip = "")
	{
		tipPanel.SetActive (show);
		tipText.text = tip;
	}
}

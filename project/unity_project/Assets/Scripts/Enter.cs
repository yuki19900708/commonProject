using EditorTerrainModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Enter : MonoBehaviour {
	public Text mapInfoTxt;

	public Button enterBtn;

	public InputField phaseInput;

	public Button closeBtn;

	public Text tipText;
	public GameObject tipPanel;

	public Dropdown dropPanel;

	[SerializeField]
	private string widthStr = "100";

	[SerializeField]
	private string heightStr = "100";

	private string phaseStr = "1";
	// Use this for initialization
	void Start () {
		enterBtn.gameObject.SetActive(true);
		ShowTipPanel(false);
		phaseInput.text = phaseStr;
		dropPanel.onValueChanged.AddListener(BrushStyleDropDownValueChange);
		dropPanel.options = TerrainEditorModel.GetTerrainEditorDropDownOptionData(new string[]
		{
			"绘制地图",
			"编辑地图",
		});
		dropPanel.value = TerrainEditorModel.LoadCharacter();

		closeBtn.onClick.AddListener(() =>
		{
				ShowTipPanel(false);
		});
		enterBtn.onClick.AddListener(() =>
		{
			if(string.IsNullOrEmpty(widthStr) || string.IsNullOrEmpty(heightStr))
            {
				return;
            }
			int xValue = int.Parse(widthStr);
			int yValue = int.Parse(heightStr);
			if (xValue < 2 || yValue < 2)
			{
					ShowTipPanel(true, "输入不合法，必须为数字且大于等于2");
				return;
			}

			int  phaseV = int.Parse(phaseInput.text);
			if(phaseV <= 0)
			{
				ShowTipPanel(true, "输入土地期数不合法, 必须为数字且大于等于0");
				return;
			}
			
			TerrainEditorModel.SetMapSize(xValue, yValue, phaseV);
			SceneManager.LoadScene(1);
		});
	}

    private void Update()
    {
		mapInfoTxt.text = string.Format("地图宽高：{0} * {1}", widthStr, heightStr) ;
	}

    private void BrushStyleDropDownValueChange(int index)
	{
		TerrainEditorModel.SetCharacter(index);
	}

	void ShowTipPanel(bool show, string tip = "")
	{
		tipPanel.SetActive (show);
		tipText.text = tip;
	}
}

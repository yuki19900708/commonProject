using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace EditorTerrainModel
{
    public enum BrushStyle
    {
        None,
        BoxUp,
    }

    public enum DropDownSelectType
    {
        Layout,
        BrushStyle,
    }

    public enum EditorType
    {
        Level,
    }

    public enum EditoryLayoutType
    {
        Vegetation,
    }

    public class TerrainEditorModel
    {
        private static string WIDTH_KEY = "MAP_WIDTH_KEY";

        private static string HEIGHT_KEY = "MAP_HEIGHT_KEY";

		private static string PHASE_KEY = "MAP_PHASE_KEY";
        private static string CHARACTER_KEY = "MAP_CHARACTER_KEY";

        public static void SetMapSize(int width , int height, int phase)
        {
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
			PlayerPrefs.SetInt(PHASE_KEY, phase);
        }

		public static void LoadMapSize(ref int width, ref int height, ref int phase)
        {
            width = PlayerPrefs.GetInt(WIDTH_KEY, 100);
            height = PlayerPrefs.GetInt(HEIGHT_KEY, 100);
			phase= PlayerPrefs.GetInt(PHASE_KEY, 1);
        }

        public static void SetCharacter(int character)
        {
            PlayerPrefs.SetInt(CHARACTER_KEY, character);
        }

        public static int LoadCharacter()
        {
            return PlayerPrefs.GetInt(CHARACTER_KEY, 0);
        }

        public static bool MapDrawer()
        {
            return LoadCharacter() == 0;

        }
        private static bool isRunMapEditor = false;

        public static bool IsRunMapEditor
        {
            get
            {
                return isRunMapEditor;
            }
            set
            {
                isRunMapEditor = value;
            }
        }
	
        private static List<VegetationData> allVegetationElements;

        public static string[] brushStyleElements = new string[]
        {
            "正常",
            "拉涂",
        };
       
        public static List<VegetationData> AllVegetationElements
        {
            get
            {
                if (allVegetationElements == null)
                {
                    allVegetationElements = TableDataMgr.GetAllVegetationDatas();
                }
                return allVegetationElements;
            }
        }

        public static List<Dropdown.OptionData> GetTerrainEditorDropDownOptionData(string[] list)
        {
            List<Dropdown.OptionData> optionData = new List<Dropdown.OptionData>();
            for (int i = 0; i < list.Length; i++)
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = list[i];
                optionData.Add(data);
            }
            return optionData;
        }

    }
}
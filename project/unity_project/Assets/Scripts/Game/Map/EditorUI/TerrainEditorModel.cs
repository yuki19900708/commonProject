using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace EditorTerrainModel
{
    public class SelectItemData
    {
        public bool isSelect;
        public MapObjectData data;
    }

    [SerializeField]
    public class terrainInfoTableData
    {
        public int xPos;
        public int yPos;
        public int terrainIndex;
        public int vegetationIndex;
        public int objectIndex;
        public int purificationIndex;
        public int sealLockIndex;
    }

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

        public static void SetMapSize(int width , int height)
        {
            PlayerPrefs.SetInt(WIDTH_KEY, width);
            PlayerPrefs.SetInt(HEIGHT_KEY, height);
        }

        public static void LoadMapSize(ref int width, ref int height)
        {
            width = PlayerPrefs.GetInt(WIDTH_KEY, width);
            height = PlayerPrefs.GetInt(HEIGHT_KEY, height);
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

        public static string[] loadPrefabNames = new string[]
        {
            "关卡选择",
        };

        public static string[] editoryLayouts = new string[]
        {
            "草皮",
        };

        private static List<VegetationData> allVegetationElements;

        private static List<MapObjectData> allObjectElements;

        private static string[] purificationElements;

        private static string[] sealLockElements;

        public static string[] SealLockElements
        {
            get
            {
                if (sealLockElements == null)
                {
                    List<string> list = new List<string>();
                    List<MapUnlockData> data = TableDataMgr.GetAllMapUnlockDatas();
                    for (int i = 0; i < data.Count; i++)
                    {
                        list.Add(data[i].id.ToString());
                    }
                    sealLockElements = list.ToArray();
                }
                return sealLockElements;
            }
        }

        public static string[] brushStyleElements = new string[]
        {
            "正常",
            "拉涂",
        };

        public static string[] PurificationElements
        {
            get
            {
                if (purificationElements == null)
                {
                    List<string> list = new List<string>();
                    List<DeadLandData> data = TableDataMgr.GetAllDeadLandDatas();
                    for (int i = 0; i < data.Count; i++)
                    {
                        list.Add(data[i].id.ToString());
                    }
                    purificationElements = list.ToArray();
                }
                return purificationElements;
            }
        }

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

        public static List<MapObjectData> AllOjbectElements
        {
            get
            {
                if (allObjectElements == null)
                {
                    allObjectElements = TableDataMgr.GetAllMapObjectDatas();
                }
                return allObjectElements;
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

        public static List<Dropdown.OptionData> GetObjectElementsDropDownOptionData(int index)
        {
         
            List<string> list = new List<string>();
            for (int i = 0; i < AllOjbectElements.Count; i++)
            {
                if (AllOjbectElements[i].illustration == index)
                {
                    string s = "";
                     if (TableDataMgr.TextDataTable == null)
                    {
                        s = "";
                    }
                    if (TableDataMgr.GetSingleTextData(AllOjbectElements[i].describe) == null)
                    {
                        s = "";
                    }
                   
                    s = TableDataMgr.GetSingleTextData(AllOjbectElements[i].describe).CN;

                    list.Add(s);
                }
            }
            return GetTerrainEditorDropDownOptionData(list.ToArray());
        }

        public static MapObjectData GetObjectInfoTabelByDescribe(string describe)
        {
            for (int i = 0; i < AllOjbectElements.Count; i++)
            {
                if (AllOjbectElements[i].describe == GetTextTableDataByDes(describe).id)
                {
                    return AllOjbectElements[i];
                }
            }
            return null;
        }

        public static TextData GetTextTableDataByDes(string des)
        {
            for (int i = 0; i < TableDataMgr.GetAllTextDatas().Count; i++)
            {
                if (TableDataMgr.GetAllTextDatas()[i].CN == des)
                {
                    return TableDataMgr.GetAllTextDatas()[i];
                }
            }
            return null;
        }
    }
}
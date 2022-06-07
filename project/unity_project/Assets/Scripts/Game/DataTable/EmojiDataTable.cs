using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class EmojiDataTable : ScriptableObject
{
    public List<EmojiData> emojiDataTable = new List<EmojiData>();
    public Dictionary<int, EmojiData> emojiDataDic = new Dictionary<int, EmojiData>();

    public void SetDatas(object[] obj)
    {
        emojiDataTable.Clear();
        foreach (object o in obj)
        {
            emojiDataTable.Add(o as EmojiData);
        }
    }

    public List<EmojiData> GetAllData()
    {
        if (emojiDataTable == null || emojiDataTable.Count == 0)
        {
            Debug.LogError("EmojiDataTable未导入asset");
        }
        return emojiDataTable;
    }

    public IList<EmojiData> GetAllReadonlyData()
    {
        if (emojiDataTable == null || emojiDataTable.Count == 0)
        {
            Debug.LogError("EmojiDataTable未导入asset");
        }

        ReadOnlyCollection<EmojiData> readOnlyEmojiData = new ReadOnlyCollection<EmojiData>(emojiDataTable);

        return readOnlyEmojiData ;
    }

    public EmojiData GetData(int index)
    {
        if (emojiDataTable == null || emojiDataTable.Count == 0)
        {
            Debug.LogError("EmojiDataTable未导入asset");
        }
        if (emojiDataDic.Count == 0)
        {
            ReadOnlyCollection<EmojiData> readOnlyEmojiData = new ReadOnlyCollection<EmojiData>(emojiDataTable);
            foreach (EmojiData value in readOnlyEmojiData)
            {
                if (emojiDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //emojiDataDic.Add(value.id, value.Clone());
                emojiDataDic.Add(value.id, value);
            }
        }
        if (emojiDataDic.ContainsKey(index))
        {
            return emojiDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}


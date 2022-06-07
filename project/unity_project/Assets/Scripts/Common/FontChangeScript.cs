using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FontChangeScript : MonoBehaviour {
    Text mText = null;
    void Start()
    {
        SetFont();
    }
    public void SetFont()
    {
        if (mText == null)
        {
            mText = gameObject.GetComponent<Text>();
        }
        //if (GlobalVariable.font != null)
        //{
        //    mText.font = GlobalVariable.font;
        //}
    }
    private void OnEnable()
    {
        //if (GameDef.currentlagfont != null)
        //{
         //   mText.font = GameDef.currentlagfont;
        //}
    }
}

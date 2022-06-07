using UnityEngine;
using UnityEngine.UI;

public class SealockItem : MonoBehaviour
{
    public SpriteRenderer background;
    public Text indexText;

    public void SetTextIndex(int id)
    {
        indexText.text = id.ToString();
    }

    public void SetInitColor()
    {
        background.color = Color.grey;
    }

    public void SetSelectColor()
    {
        background.color = Color.red;
    }
}

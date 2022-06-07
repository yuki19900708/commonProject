using UnityEngine;

public class Wall : MapObject
{
    public static int[] neightbourIndex = new int[] { 1, 2, 4, 8 };
    public SpriteRenderer[] walls;
    public SpriteRenderer[] waveSides;

    public override void SetTileIndex(int tileIndex)
    {
        for (int i = 0; i < walls.Length; i++)
        {
            string objectName = walls[i].name;
            int maxIndex = 1;
            if (objectName == "wall_corner_left" || objectName == "wall_corner_right" || objectName == "wall_corner")
            {
                maxIndex = 3;
            }
            else if (objectName == "wall_dark_left" || objectName == "wall_light_right")
            {
                maxIndex = 5;
            }
            int randomIndex = Random.Range(1, maxIndex + 1);
            string finalName = string.Format("{0}-{1}", objectName, randomIndex);
            walls[i].sprite = UGUISpriteAtlasMgr.LoadSprite(finalName);
        }
        for (int i = 0; i < waveSides.Length; i++)
        {
            int randomIndex = Random.Range(1, 7);
            string finalName = "wave_side_" + randomIndex.ToString();
            waveSides[i].sprite = UGUISpriteAtlasMgr.LoadSprite(finalName);
        }
    }
}

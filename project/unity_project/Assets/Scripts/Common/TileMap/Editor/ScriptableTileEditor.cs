using UnityEngine;
using UnityEditor;

namespace Universal.TileMapping
{
    public class ScriptableTileEditor : Editor
    {
        protected ScriptableTile scriptableTile;

        protected override void OnHeaderGUI()
        {
            scriptableTile = target as ScriptableTile;

            Rect headerRect = new Rect(0, 0, Screen.width, 50);
            GUILayout.BeginArea(headerRect, new GUIStyle("IN ThumbnailShadow"));
            Rect contentRect = new Rect(10, 10, Screen.width - 20, 30);
            GUI.DrawTexture(new Rect(contentRect.x, contentRect.y, contentRect.height, contentRect.height), scriptableTile.GetPreview(), ScaleMode.ScaleAndCrop);
            contentRect.x += contentRect.height + 10;
            contentRect.width -= contentRect.x;
            GUI.Label(new Rect(contentRect.x, contentRect.y, contentRect.width, contentRect.height / 2), scriptableTile.name, TileMapGUIStyles.leftBoldLabel);
            GUI.Label(new Rect(contentRect.x, contentRect.y + contentRect.height / 2, contentRect.width, contentRect.height / 2), "Tile Type: " + scriptableTile.GetType().Name, TileMapGUIStyles.leftMiniLabel);

            GUILayout.EndArea();
            GUILayout.Space(headerRect.height + 10);
        }
    }
}
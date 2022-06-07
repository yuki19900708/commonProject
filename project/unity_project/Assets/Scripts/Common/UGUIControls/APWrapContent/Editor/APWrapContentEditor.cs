using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(APWrapContent))]
public class APWrapContentEditor : Editor
{
    SerializedProperty spDirection;
    SerializedProperty spSpacing;
    SerializedProperty spPadding;
    SerializedProperty spDataCount;
    SerializedProperty spItemPrefab;
    SerializedProperty spRow;
    SerializedProperty spColumn;
    SerializedProperty spUseGalleryMode;
    SerializedProperty spGalleryItemScaleCurve;
    SerializedProperty spGalleryItemAlphaCurve;
    SerializedProperty spForceRefresh;

    private void OnEnable()
    {
        spDirection                = serializedObject.FindProperty("direction");
        spSpacing                  = serializedObject.FindProperty("spacing");
        spPadding                  = serializedObject.FindProperty("padding");
        spDataCount                = serializedObject.FindProperty("dataCount");
        spItemPrefab               = serializedObject.FindProperty("itemPrefab");
        spRow                      = serializedObject.FindProperty("row");
        spColumn                   = serializedObject.FindProperty("column");
        spUseGalleryMode           = serializedObject.FindProperty("useGalleryMode");
        spGalleryItemScaleCurve    = serializedObject.FindProperty("galleryItemScaleCurve");
        spGalleryItemAlphaCurve    = serializedObject.FindProperty("galleryItemAlphaCurve");
        spForceRefresh             = serializedObject.FindProperty("forceRefresh");
    }

    public override void OnInspectorGUI()
    {
        APWrapContent wrapContent = (APWrapContent)target;
        EditorGUILayout.PropertyField(spDirection);
        EditorGUILayout.PropertyField(spSpacing);
        EditorGUILayout.PropertyField(spPadding, true);
        EditorGUILayout.PropertyField(spDataCount);
        EditorGUILayout.PropertyField(spForceRefresh);
        if (wrapContent.Direction == Slider.Direction.LeftToRight || wrapContent.Direction == Slider.Direction.RightToLeft)
        {
            EditorGUILayout.PropertyField(spRow);
        }
        else if (wrapContent.Direction == Slider.Direction.TopToBottom || wrapContent.Direction == Slider.Direction.BottomToTop)
        {
            EditorGUILayout.PropertyField(spColumn);
        }
        EditorGUILayout.PropertyField(spItemPrefab);
        EditorGUILayout.PropertyField(spUseGalleryMode);
        if (wrapContent.UseGalleryMode)
        {
            EditorGUILayout.PropertyField(spGalleryItemScaleCurve);
            EditorGUILayout.PropertyField(spGalleryItemAlphaCurve);
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Preview"))
        {
            wrapContent.Preview();
        }
        if (GUILayout.Button("Clear"))
        {
            wrapContent.Clear();
        }
        GUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }

    [MenuItem("GameObject/UI/APWrapContent", priority = 0)]
    public static void CreateSliderComponent()
    {
        object sliderObject = Resources.Load("Prefabs/APWrapContent/APWrapContent", typeof(GameObject));
        GameObject go = Instantiate(sliderObject as GameObject);
        APWrapContent wrapContent = go.GetComponent<APWrapContent>();
        if (Selection.activeGameObject != null)
        {
            wrapContent.transform.SetParent(Selection.activeGameObject.transform);
        }
        wrapContent.transform.localScale = Vector3.one;
        wrapContent.transform.localPosition = Vector3.zero;
        wrapContent.name = "APWrapContent";
    }
}
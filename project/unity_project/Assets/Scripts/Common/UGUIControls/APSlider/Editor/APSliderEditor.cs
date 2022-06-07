using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(APSlider))]
public class APSliderEditor : Editor
{
    private APSlider slider;
    private SerializedObject sliderObject;
    private SerializedProperty foreground, handleImage, foreArea,
        isInteraction, easeType, isDynamic, timeType, totalTime;

    private void OnEnable()
    {
        sliderObject = new SerializedObject(target);
        slider = target as APSlider;
        foreground = sliderObject.FindProperty("foreground");
        foreArea = sliderObject.FindProperty("foreArea");
        handleImage = sliderObject.FindProperty("handleImage");
        isInteraction = sliderObject.FindProperty("isInteraction");
        isDynamic = sliderObject.FindProperty("isDynamic");
        timeType = sliderObject.FindProperty("timeType");
        totalTime = sliderObject.FindProperty("totalTime");
        easeType = sliderObject.FindProperty("easeType");
    }

    public override void OnInspectorGUI()
    {
        sliderObject.Update();
        EditorGUILayout.PropertyField(foreground);
        EditorGUILayout.PropertyField(handleImage);
        EditorGUILayout.PropertyField(foreArea);
        
        Slider.Direction direction = (Slider.Direction)EditorGUILayout.EnumPopup("Direction", slider.Direction);
        if (direction != slider.Direction)
        {
            slider.Direction = direction;
        }
        EditorGUILayout.PropertyField(isInteraction);
        if (isInteraction.boolValue == false)
        {
            float currentValue = EditorGUILayout.FloatField("Value", slider.Value);
            if (currentValue != slider.Value)
            {
                slider.Value = currentValue;
            }
            EditorGUILayout.PropertyField(isDynamic);
           
            if (isDynamic.boolValue)
            {
                EditorGUILayout.PropertyField(easeType);
                EditorGUILayout.PropertyField(timeType);
                EditorGUILayout.PropertyField(totalTime);
            }
        }
        else
        {
            float currentValue = EditorGUILayout.Slider("Value", slider.Value, 0, 1);
            if (currentValue != slider.Value)
            {
                slider.Value = currentValue;
            }
        }
        sliderObject.ApplyModifiedProperties();
    }

    [MenuItem("GameObject/UI/APSlider", priority = 0)]
    public static void CreateSliderComponent()
    {
        object sliderObject = Resources.Load("Prefabs/APSlider/APSlider", typeof(GameObject));
        GameObject go = Instantiate(sliderObject as GameObject);
        APSlider slider = go.GetComponent<APSlider>();
        if (Selection.activeGameObject != null)
        {
            slider.transform.SetParent(Selection.activeGameObject.transform);
        }
        slider.transform.localScale = Vector3.one;
        slider.transform.localPosition = Vector3.zero;
        slider.name = "APSlider";
    }
}

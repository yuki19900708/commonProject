using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Universal.TileMapping
{
    [CustomEditor(typeof(TileRenderer))]
    public class TileRendererEditor : Editor
    {
        SerializedProperty spColor;
        SerializedProperty spSortingMethod;
        SerializedProperty spSortingLayer;
        SerializedProperty spOrderInLayer;
        SerializedProperty spOrderDelta;
        SerializedProperty spZDepthDelta;
        SerializedProperty spAutoPivot;
        SerializedProperty spParentOffset;

        public virtual void OnEnable()
        {
            spColor = serializedObject.FindProperty("color");
            spSortingMethod = serializedObject.FindProperty("sortingMethod");
            spSortingLayer = serializedObject.FindProperty("sortingLayer");
            spOrderInLayer = serializedObject.FindProperty("orderInLayer");
            spOrderDelta = serializedObject.FindProperty("orderDelta");
            spZDepthDelta = serializedObject.FindProperty("zDepthDelta");
            spAutoPivot = serializedObject.FindProperty("autoPivot");
            spParentOffset = serializedObject.FindProperty("parentOffset");
        }

        public virtual void OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(spColor);

            TileRenderer.SortingMethod method = (TileRenderer.SortingMethod)spSortingMethod.enumValueIndex;
            method = (TileRenderer.SortingMethod)EditorGUILayout.EnumPopup(spSortingMethod.displayName, method);
            spSortingMethod.enumValueIndex = (int)method;
            if (method == TileRenderer.SortingMethod.SortingLayer)
            {
                var sortingLayerNames = SortingLayer.layers.Select(l => l.name).ToArray();

                if (sortingLayerNames != null)
                {

                    // Look up the layer name using the current layer ID
                    string oldName = SortingLayer.IDToName(spSortingLayer.intValue);

                    // Use the name to look up our array index into the names list
                    int oldLayerIndex = Array.IndexOf(sortingLayerNames, oldName);

                    // Show the popup for the names
                    int newLayerIndex = EditorGUILayout.Popup(spSortingLayer.displayName, oldLayerIndex, sortingLayerNames);

                    // If the index changes, look up the ID for the new index to store as the new ID
                    if (newLayerIndex != oldLayerIndex)
                    {
                        spSortingLayer.intValue = SortingLayer.NameToID(sortingLayerNames[newLayerIndex]);
                    }
                }
                else {
                    int newValue = EditorGUILayout.IntField(spSortingLayer.displayName, spSortingLayer.intValue);
                    if (newValue != spSortingLayer.intValue)
                    {
                        spSortingLayer.intValue = newValue;
                    }
                    EditorGUI.EndProperty();
                }

                EditorGUILayout.PropertyField(spOrderInLayer);
                EditorGUILayout.PropertyField(spOrderDelta);
            }
            else if (method == TileRenderer.SortingMethod.ZDepth)
            {
                EditorGUILayout.PropertyField(spZDepthDelta);
            }
            bool autoPivot = EditorGUILayout.Toggle(spAutoPivot.displayName, spAutoPivot.boolValue);
            spAutoPivot.boolValue = autoPivot;

            Vector3 offset = EditorGUILayout.Vector3Field(spParentOffset.displayName, spParentOffset.vector3Value);
            spParentOffset.vector3Value = offset;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
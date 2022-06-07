using UnityEditor;
using UnityEngine;

namespace Universal.TileMapping
{
    [CustomEditor(typeof(TileMeshRenderer))]
    public class TileMeshRendererEditor : TileRendererEditor
    {
        private TileMeshRenderer renderer;
        private SerializedProperty spMaterial;

        public override void OnEnable()
        {
            base.OnEnable();

            spMaterial = serializedObject.FindProperty("material");
            renderer = (target as TileMeshRenderer);
            EditorUtility.SetSelectedRenderState(renderer.meshRenderer, EditorSelectedRenderState.Highlight);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(spMaterial);
            if (renderer.Atlas != null)
            {
                GUILayout.BeginVertical();
                float width = EditorGUIUtility.currentViewWidth - 70;
                float height = width * renderer.Atlas.height / renderer.Atlas.width;
                GUILayout.Box(renderer.Atlas, GUILayout.Width(width), GUILayout.Height(height));
                GUILayout.EndVertical();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

namespace UnityEngine.UI
{
    public static class PolyImageExtenstion
    {
        [MenuItem("GameObject/UI/PolyImage", false, 2001)]
        static public void AddPolyImage(MenuCommand menuCommand)
        {
            GameObject go = CreatePolyImage();
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("CONTEXT/Image/Convert to PolyImage")]
        static void Image2SGImage(MenuCommand menuCommand)
        {
            Image image = menuCommand.context as Image;
            GameObject go = image.gameObject;
            var color = image.color;
            var sprite = image.sprite;
#if UNITY_5_2_OR_NEWER
            var raycastTarget = image.raycastTarget;
#endif
            var type = image.type;
            GameObject.DestroyImmediate(image);

            PolyImage image2 = go.AddComponent<PolyImage>();
            image2.color = color;
            image2.sprite = sprite;
#if UNITY_5_2_OR_NEWER
            image2.raycastTarget = raycastTarget;
#endif
            image2.type = type;
        }

#region DefaultControls.cs
        private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        public static GameObject CreatePolyImage()
        {
            GameObject go = CreateUIElementRoot("Image", s_ImageElementSize);
            go.AddComponent<PolyImage>();
            return go;
        }
#endregion

#region MenuOptions.cs
        private const string kUILayerName = "UI";

        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                parent = GetOrCreateCanvasGameObject();
            }

            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
            element.name = uniqueName;
            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
            GameObjectUtility.SetParentAndAlign(element, parent);
            //if (parent != menuCommand.context) // not a context click, so center in sceneview
            //    SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

            Selection.activeGameObject = element;
        }

        // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
        static public GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use just any canvas..
            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in the scene at all? Then create a new one.
            return CreateNewUI();
        }

        static public GameObject CreateNewUI()
        {
            // Root for the UI
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer(kUILayerName);
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            // if there is no event system add one...
            CreateEventSystem(false);
            return root;
        }

        private static void CreateEventSystem(bool select)
        {
            CreateEventSystem(select, null);
        }

        private static void CreateEventSystem(bool select, GameObject parent)
        {
            var esys = Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }
#endregion
    }
}

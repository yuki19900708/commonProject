using System.Collections.Generic;
using UnityEngine;

public class RendererCull : MonoBehaviour
{
    public Camera rendererCamera;
    public int capacity = 100;
    private static RendererCull instance;

    private Renderer[] rendererList;

    private Vector3 cameraPosCache;
    private float cameraOrthCache;
    private Quaternion cameraRotation;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        cameraPosCache = rendererCamera.transform.position;
        cameraOrthCache = rendererCamera.orthographicSize;
        cameraRotation = rendererCamera.transform.rotation;
        rendererList = new Renderer[capacity];
    }

    // Update is called once per frame
    void Update()
    {
        if (rendererList.Length == 0)
        {
            return;
        }

        bool isCameraStatic = Vector3.Distance(cameraPosCache, rendererCamera.transform.position) <= 0.01f;
        isCameraStatic &= Mathf.Abs(cameraOrthCache - rendererCamera.orthographicSize) <= 0.01f;
        isCameraStatic &= cameraRotation.Equals(rendererCamera.transform.rotation);
        
        cameraPosCache = rendererCamera.transform.position;
        cameraOrthCache = rendererCamera.orthographicSize;
        cameraRotation = rendererCamera.transform.rotation;

        if (isCameraStatic)
        {
            return;
        }

        Plane[] cameraPlant = GeometryUtility.CalculateFrustumPlanes(rendererCamera);
        for (int i = 0; i < rendererList.Length; i++)
        {
            Renderer r = rendererList[i];
            if (r == null)
            {
                continue;
            }
            else
            {
                r.gameObject.SetActive(GeometryUtility.TestPlanesAABB(cameraPlant, r.bounds));
            }
        }
    }

    public static void RegisterRenderer(Renderer r)
    {
        bool added = false;
        for (int i = 0; i < instance.rendererList.Length; i++)
        {
            if (instance.rendererList[i] == null)
            {
                instance.rendererList[i] = r;
                added = true;
                break;
            }
        }

        if (added == false)
        {
            int addIndex = instance.capacity;
            instance.capacity *= 2;
            Renderer[] newList = new Renderer[instance.capacity];
            instance.rendererList.CopyTo(newList, 0);
            instance.rendererList = newList;
            instance.rendererList[addIndex] = r;
        }
    }
}

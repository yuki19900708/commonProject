using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class WaterShadowEffect : MonoBehaviour
{
    private static WaterShadowEffect instance;

    public bool useMutipleInstanceMaterial = false; 
    public RawImage waterImage;
    public Shader objectShapeFillShader;
    public Shader waterShadowShader;
    public Texture2D noiseTexture;
    public Texture2D waterTexture;

    public Color shadowColor = Color.white;
    public float shadowOffsetX = 0;
    public float shadowOffsetY = 0;
    [Range(0.0f, 100f)]
    public float shadowLength = 50;
    [Range(0.0f, 1.0f)]
    public float maxShadowness = 0.6f;

    public List<GameObject> targetObjects = new List<GameObject>();

    private RenderTexture renderTexture = null;
    private RenderTexture shadowRendereTexture = null;
    private CommandBuffer commandBuffer = null;

    private Material waterShadowMatrial;
    private Queue<Material> shapeFillMaterialQueue = new Queue<Material>();
    private List<Material> shapeFillMaterialUsingList = new List<Material>();
    private Camera mainCamera;
    
    private Material finalMaterial;

    private List<Renderer> rendererList = new List<Renderer>();

    private Vector3 cameraPosCache;
    private float cameraOrthCache;
    private Quaternion cameraRotation;
    private bool forceUpdate = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mainCamera = this.GetComponent<Camera>();
        commandBuffer = new CommandBuffer();
        if (renderTexture == null)
        {
            renderTexture = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2, 0);
            shadowRendereTexture = RenderTexture.GetTemporary(Screen.width / 2, Screen.height / 2, 0);
            waterImage.texture = shadowRendereTexture;
            waterImage.gameObject.SetActive(true);
        }

        if (waterShadowShader != null)
        {
            waterShadowMatrial = new Material(waterShadowShader);

        }
        finalMaterial = GetShapeFillMaterial();

        foreach(GameObject go in targetObjects)
        {
            RegisterRenderer(go);
        }
        targetObjects.Clear();

        mainCamera.AddCommandBuffer(CameraEvent.AfterSkybox, commandBuffer);
    }

    void Update()
    {
        if (rendererList.Count == 0)
        {
            return;
        }

        bool isCameraStatic = Vector3.Distance(cameraPosCache, mainCamera.transform.position) <= 0.01f;
        isCameraStatic &= Mathf.Abs(cameraOrthCache - mainCamera.orthographicSize) <= 0.01f;
        isCameraStatic &= cameraRotation.Equals(mainCamera.transform.rotation);

        cameraPosCache = mainCamera.transform.position;
        cameraOrthCache = mainCamera.orthographicSize;
        cameraRotation = mainCamera.transform.rotation;

        if (isCameraStatic && forceUpdate == false)
        {
            return;
        }

        forceUpdate = false;

        Plane[] cameraPlant = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        commandBuffer.Clear();
        commandBuffer.SetRenderTarget(renderTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.black);
        foreach (Material mat in shapeFillMaterialUsingList)
        {
            shapeFillMaterialQueue.Enqueue(mat);
        }
        shapeFillMaterialUsingList.Clear();

        bool visible = false;
        for (int i = 0; i < rendererList.Count; i++)
        {
            Renderer r = rendererList[i];
            if (r == null)
            {
                continue;
            }
            else
            {
                visible = GeometryUtility.TestPlanesAABB(cameraPlant, r.bounds);
                r.gameObject.SetActive(visible);
            }

            if (visible)
            {
                DrawRenderer(r);
            }
        }
        Vector3 cameraBottomLeftPoint = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector2 waterUVOffset = new Vector2((cameraBottomLeftPoint.x - CameraGestureMgr.Instance.ViewRange.xMin) / CameraGestureMgr.Instance.ViewRange.width, (cameraBottomLeftPoint.y - CameraGestureMgr.Instance.ViewRange.yMin) / CameraGestureMgr.Instance.ViewRange.height);
        Vector2 waterUVScale = new Vector2(mainCamera.orthographicSize * 2 * Screen.width / Screen.height / CameraGestureMgr.Instance.ViewRange.width, mainCamera.orthographicSize * 2 / CameraGestureMgr.Instance.ViewRange.height);
        waterShadowMatrial.SetColor("_WaterColor", shadowColor);
        waterShadowMatrial.SetFloat("_ShadowOffsetX", shadowOffsetX);
        waterShadowMatrial.SetFloat("_ShadowOffsetY", shadowOffsetY * (1 - CameraGestureMgr.ENTER_GAME_ORTHOGRAPHIC_SIZE / mainCamera.orthographicSize));
        waterShadowMatrial.SetFloat("_ShadowLength", shadowLength * CameraGestureMgr.ENTER_GAME_ORTHOGRAPHIC_SIZE / mainCamera.orthographicSize);
        waterShadowMatrial.SetFloat("_XRotation", 26);
        waterShadowMatrial.SetFloat("_YRotation", 65);
        waterShadowMatrial.SetFloat("_MaxShadowness", maxShadowness);
        waterShadowMatrial.SetVector("_waterUVOffset", waterUVOffset);
        waterShadowMatrial.SetVector("_waterUVScale", waterUVScale);
        waterShadowMatrial.SetTexture("_NoiseTex", noiseTexture);
        waterShadowMatrial.SetTexture("_WaterTex", waterTexture);
        commandBuffer.Blit(renderTexture, shadowRendereTexture, waterShadowMatrial);
    }

    void OnDestroy()
    {
        RenderTexture.ReleaseTemporary(renderTexture);
        foreach (Material mat in shapeFillMaterialQueue)
        {
            DestroyImmediate(mat);
        }
        foreach (Material mat in shapeFillMaterialUsingList)
        {
            DestroyImmediate(mat);
        }
        if (commandBuffer != null)
        {
            commandBuffer.Release();
            commandBuffer = null;
        }
    }

    public static void RegisterRenderer(Renderer renderer)
    {
        instance.rendererList.Add(renderer);
    }

    public static void RegisterRenderer(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            instance.rendererList.Add(r);
        }
    }

    public static void Clear()
    {
        instance.rendererList.Clear();
        instance.commandBuffer.Clear();
        instance.forceUpdate = true;
    }

    private void DrawRenderer(Renderer r)
    {
        if (finalMaterial == null || useMutipleInstanceMaterial)
        {
            finalMaterial = GetShapeFillMaterial();
        }
        finalMaterial.SetTexture("_MainTex", r.sharedMaterial.mainTexture);
        commandBuffer.DrawRenderer(r, finalMaterial);
    }

    private Material GetShapeFillMaterial()
    {
        Material mat = null;
        if (shapeFillMaterialQueue.Count > 0)
        {
            mat = shapeFillMaterialQueue.Dequeue();
        }
        else
        {
            mat = new Material(objectShapeFillShader);
        }
        shapeFillMaterialUsingList.Add(mat);
        return mat;
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    if (pause == false)
    //    {
    //        if (commandBuffer != null)
    //        {
    //            RefreshCommandBuffer();
    //        }
    //    }
    //}
}

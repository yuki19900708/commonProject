using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Universal.TileMapping;
//using Spine.Unity.Editor;
//using Spine.Unity;

public class CreateObjectPrefab : MonoBehaviour
{
    static string FilePath = Application.dataPath + "/Resources/Prefabs/TilePrefabs/object/";
    static string PrefabPath = "Assets/Resources/Prefabs/TilePrefabs/object/";
    static string TileFilePath = Application.dataPath + "/Resources/Tiles/objectTile/";
    static string TileFrefabPath = "Assets/Resources/Tiles/objectTile/";
    static UGUISpriteAtlas[] atlasArray;

    [MenuItem("Appcpi/资源编辑/Build MapObject Prefabs")]
    public static void CreatePrefabs()
    {
        Dictionary<int, List<MapObjectData>> objInfoDict = new Dictionary<int, List<MapObjectData>>();

        ResourcesPostprocessor.enabled = false;

        MapObjectDataTable allObjectElements = AssetDatabase.LoadAssetAtPath<MapObjectDataTable>("Assets/ResourcesRaw/DataAsset/MapObjectDataTable.asset");
        string atlasFolder = Application.dataPath + "/ResourcesRaw/Atlas/";
        string[] atlasFiles = Directory.GetFiles(atlasFolder, "*.prefab");
        atlasArray = new UGUISpriteAtlas[atlasFiles.Length];
        for (int i = 0; i < atlasFiles.Length; i++)
        {
            string atlasFile = atlasFiles[i];
            string assetPath = "Assets" + atlasFile.Remove(0, Application.dataPath.Length);
            GameObject assetObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            UGUISpriteAtlas atlas = assetObject.GetComponent<UGUISpriteAtlas>();
            atlas.Init();
            atlasArray[i] = atlas;
        }

        foreach (MapObjectData tab in allObjectElements.GetAllData())
        {
            if (objInfoDict.ContainsKey(tab.objectType))
            {
                objInfoDict[tab.objectType].Add(tab);
            }
            else
            {
                objInfoDict.Add(tab.objectType, new List<MapObjectData>() { tab });
            }
        }
        Sprite shadowSprite = GetSprite("healing_ball_shadow");
        float curProgress = 0;
        float maxProgress = 1;

        //基础物体
        GameObject basicPrefab = new GameObject("basicPrefab");
        GameObject item = new GameObject("item");
        SpriteRenderer itemSpriteRenderer = item.AddComponent<SpriteRenderer>();
        itemSpriteRenderer.sortingLayerName = "Object";
        item.transform.SetParent(basicPrefab.transform, false);
        basicPrefab.AddComponent<MapObject>();

        //带阴影物体
        GameObject withShadowObjectPrefab = Instantiate(basicPrefab);
        withShadowObjectPrefab.name = "withShadowObjectPrefab";
        GameObject shadow = new GameObject("Shadow");
        SpriteRenderer shadowRenderer = shadow.AddComponent<SpriteRenderer>();
        shadowRenderer.sortingLayerName = "Object";
        shadowRenderer.sortingOrder = -1;
        shadowRenderer.sprite = shadowSprite;
        shadow.transform.SetParent(withShadowObjectPrefab.transform, false);
        shadow.transform.localPosition = new Vector3(0, 0.6f, 0);

        //可摧毁型预制+阴影
        GameObject canDestroyObjectWithShadowPrefab = Instantiate(withShadowObjectPrefab);
        canDestroyObjectWithShadowPrefab.name = "canDestroyObjectWithShadowPrefab";
        //MapObject mapObject = canDestroyObjectWithShadowPrefab.GetComponent<MapObject>();
        GameObject hitPosition = new GameObject("HitPosition");
        hitPosition.transform.SetParent(canDestroyObjectWithShadowPrefab.transform, false);
        //mapObject.hitPosition = hitPosition.transform;
        GameObject hpPosition = new GameObject("HPPosition");
        hpPosition.transform.SetParent(canDestroyObjectWithShadowPrefab.transform, false);
        //mapObject.hpPosition = hpPosition.transform;

        //可摧毁型预制无阴影
        GameObject canDestroyObjectNoShadowPrefab = Instantiate(basicPrefab);
        canDestroyObjectNoShadowPrefab.name = "canDestroyObjectNoShadowPrefab";
        //mapObject = canDestroyObjectNoShadowPrefab.GetComponent<MapObject>();
        hitPosition = new GameObject("HitPosition");
        hitPosition.transform.SetParent(canDestroyObjectNoShadowPrefab.transform, false);
        //mapObject.hitPosition = hitPosition.transform;
        hpPosition = new GameObject("HPPosition");
        hpPosition.transform.SetParent(canDestroyObjectNoShadowPrefab.transform, false);
        //mapObject.hpPosition = hpPosition.transform;

        //敌对怪物预制
        GameObject enemyMonsterPrefab = Instantiate(canDestroyObjectWithShadowPrefab);
        enemyMonsterPrefab.name = "enemyMonsterPrefab";
        //mapObject = enemyMonsterPrefab.GetComponent<MapObject>();
        GameObject bulletPosition = new GameObject("BulletPosition");
        bulletPosition.transform.SetParent(enemyMonsterPrefab.transform, false);
        //mapObject.bulletPosition = bulletPosition.transform;

        //我方怪物预制
        GameObject allyMonsterPrefab = Instantiate(enemyMonsterPrefab);
        allyMonsterPrefab.name = "allyMonsterPrefab";
        //mapObject = allyMonsterPrefab.GetComponent<MapObject>();
        GameObject collectProgressbarMountTransform = new GameObject("CollectProgressbarMountTransform");
        collectProgressbarMountTransform.transform.SetParent(allyMonsterPrefab.transform, false);
        //mapObject.collectProgressbarMountTransform = collectProgressbarMountTransform.transform;
        GameObject collectArticleMountTransform = new GameObject("CollectArticleMountTransform");
        collectArticleMountTransform.transform.SetParent(allyMonsterPrefab.transform, false);
        //mapObject.collectArticleMountTransform = collectArticleMountTransform.transform;

        foreach (KeyValuePair<int, List<MapObjectData>> kv in objInfoDict)
        {
            string path = FilePath + kv.Key + "/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
    
            curProgress = 0;
            maxProgress = kv.Value.Count;
            for (int i = 0; i < kv.Value.Count; i++)
            {
                curProgress++;
                //EditorUtility.DisplayProgressBar("生成MapObject Prefabs中", string.Format("正在生成{0}的Prefab 请稍后......", kv.Key), curProgress / maxProgress);
                string objectIDString = kv.Value[i].id.ToString();
                path = PrefabPath + kv.Key + "/" + kv.Value[i].id + ".prefab";

                MapObjectData mapObjectData = kv.Value[i];

                bool canDestory = mapObjectData.destructType > 0;
                bool isEnemyMonster = mapObjectData.objectType == 611;
                bool isAllyMonster = mapObjectData.illustration == 5 && mapObjectData.detachGrid;
                bool isEgg = mapObjectData.illustration == 5 && mapObjectData.detachGrid == false;
                bool withShadow = mapObjectData.objectType == 101 ||
                                mapObjectData.objectType == 711 ||
                                mapObjectData.objectType == 729 ||
                                mapObjectData.objectType == 730 ||
                                mapObjectData.objectType == 731 ||
                                mapObjectData.id == 70001 ||
                                mapObjectData.id == 70002 ||
                                mapObjectData.id == 70005 ||
                                mapObjectData.id == 70006 ||
                                mapObjectData.id == 70010 ||
                                mapObjectData.id == 70011 ||
                                mapObjectData.id == 70012 ||
                                mapObjectData.id == 70013 ||
                                mapObjectData.id == 70014 ||
                                mapObjectData.id == 70015 ||
                                isEgg;

                GameObject instance;
                bool isNotExist = File.Exists(path) == false;
                if (isNotExist)
                {
                    if (isEnemyMonster)
                    {
                        instance = PrefabUtility.CreatePrefab(path, enemyMonsterPrefab);
                    }
                    else if (isAllyMonster)
                    {
                        instance = PrefabUtility.CreatePrefab(path, allyMonsterPrefab);
                    }
                    else if (canDestory)
                    {
                        if (withShadow)
                        {
                            instance = PrefabUtility.CreatePrefab(path, canDestroyObjectWithShadowPrefab);
                        }
                        else
                        {
                            instance = PrefabUtility.CreatePrefab(path, canDestroyObjectNoShadowPrefab);
                        }
                    }
                    else
                    {
                        if (withShadow)
                        {
                            instance = PrefabUtility.CreatePrefab(path, withShadowObjectPrefab);
                        }
                        else
                        {
                            instance = PrefabUtility.CreatePrefab(path, basicPrefab);
                        }
                    }
                    //首次创建物体预制的同时还要创建对应的Tile
                    CreatObjectTile(instance, kv.Key.ToString());
                    //更新物体名称为ID
                    Transform itemTransform = instance.transform.Find("item");
                    itemTransform.name = objectIDString;

                    //确保引用了正确的Sprite
                    itemSpriteRenderer = itemTransform.GetComponent<SpriteRenderer>();
                    itemSpriteRenderer.sprite = GetSprite(objectIDString);

                    //飘浮物体的sortingLayerName为Floating
                    if (mapObjectData.detachGrid)
                    {
                        for (int k = 0; k < instance.transform.childCount; k++)
                        {
                            SpriteRenderer renderer = instance.transform.GetChild(k).GetComponent<SpriteRenderer>();
                            if (renderer != null)
                            {
                                renderer.sortingLayerName = "Floating";
                            }
                        }
                    }
                }
                else
                {
                    //预制已经存在的话，从已经存在预制上复制属性，然后更换预制
                    instance = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                    bool needReplace = false;

                    if (instance.transform.Find(objectIDString) == null)
                    {
                        needReplace = true;
                    }
                    if (isEnemyMonster)
                    {
                        if (instance.transform.Find("BulletPosition") == null)
                        {
                            needReplace = true;
                        }
                    }
                    else if (isAllyMonster)
                    {
                        if (instance.transform.Find("CollectProgressbarMountTransform") == null)
                        {
                            needReplace = true;
                        }
                        if (instance.transform.Find("CollectArticleMountTransform") == null)
                        {
                            needReplace = true;
                        }
                    }
                    else if (canDestory)
                    {
                        if (instance.transform.Find("HitPosition") == null)
                        {
                            needReplace = true;
                        }
                        if (instance.transform.Find("HPPosition") == null)
                        {
                            needReplace = true;
                        }
                        if (withShadow)
                        {
                            if (instance.transform.Find("Shadow") == null)
                            {
                                needReplace = true;
                            }
                        }
                        else
                        {
                            if (instance.transform.Find("Shadow"))
                            {
                                needReplace = true;
                            }
                        }
                    }
                    else
                    {
                        if (withShadow)
                        {
                            if (instance.transform.Find("Shadow") == null)
                            {
                                needReplace = true;
                            }
                        }
                        else
                        {
                            if (instance.transform.Find("Shadow"))
                            {
                                needReplace = true;
                            }
                        }
                    }

                    if (needReplace == false || mapObjectData.objectType == 718 || mapObjectData.objectType == 719)
                    {
                        CreatObjectTile(instance, kv.Key.ToString());
                        continue;
                    }

                    Debug.Log(string.Format("更新 {0} 的预制体", mapObjectData.id));

                    GameObject newInstance;
                    if (isEnemyMonster)
                    {
                        newInstance = Instantiate(enemyMonsterPrefab);
                    }
                    else if (isAllyMonster)
                    {
                        newInstance = Instantiate(allyMonsterPrefab);
                    }
                    else if (canDestory)
                    {
                        if (withShadow)
                        {
                            newInstance = Instantiate(canDestroyObjectWithShadowPrefab);
                        }
                        else
                        {
                            newInstance = Instantiate(canDestroyObjectNoShadowPrefab);
                        }
                    }
                    else
                    {
                        if (withShadow)
                        {
                            newInstance = Instantiate(withShadowObjectPrefab);
                        }
                        else
                        {
                            newInstance = Instantiate(basicPrefab);
                        }
                    }

                    Transform itemTransform = instance.transform.Find(objectIDString);
                    Transform newItemTransform = newInstance.transform.Find("item");
                    newItemTransform.name = objectIDString;

                    //确保引用了正确的Sprite
                    SpriteRenderer sr = newItemTransform.GetComponent<SpriteRenderer>();
                    sr.sprite = GetSprite(objectIDString);
                    float spriteHeight = sr.sprite.rect.height / sr.sprite.pixelsPerUnit;

                    //复制Item位置
                    UnityEditorInternal.ComponentUtility.CopyComponent(itemTransform);
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(newItemTransform);

                    //复制Collider
                    PolygonCollider2D oldCollider = instance.GetComponent<PolygonCollider2D>();
                    UnityEditorInternal.ComponentUtility.CopyComponent(oldCollider);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newInstance);

                    if (canDestory)
                    {
                        //复制HitPosition与HPPosition Transfrom
                        Transform copyFrom = instance.transform.Find("HitPosition");
                        Transform copyTo = newInstance.transform.Find("HitPosition");
                        UnityEditorInternal.ComponentUtility.CopyComponent(copyFrom);
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(copyTo);
                        if (copyFrom == null)
                        {
                            copyTo.localPosition = newItemTransform.localPosition;
                        }

                        copyFrom = instance.transform.Find("HPPosition");
                        copyTo = newInstance.transform.Find("HPPosition");
                        UnityEditorInternal.ComponentUtility.CopyComponent(copyFrom);
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(copyTo);
                        if (copyFrom == null)
                        {
                            copyTo.localPosition = newItemTransform.localPosition + new Vector3(0, spriteHeight / 2 + 0.3f, 0);
                        }
                    }
                    if (isEnemyMonster)
                    {
                        Transform copyFrom = instance.transform.Find("BulletPosition");
                        Transform copyTo = newInstance.transform.Find("BulletPosition");
                        UnityEditorInternal.ComponentUtility.CopyComponent(copyFrom);
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(copyTo);
                    }
                    else if (isAllyMonster)
                    {
                        Transform copyFrom = instance.transform.Find("CollectProgressbarMountTransform");
                        Transform copyTo = newInstance.transform.Find("CollectProgressbarMountTransform");
                        UnityEditorInternal.ComponentUtility.CopyComponent(copyFrom);
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(copyTo);
                        if (copyFrom == null)
                        {
                            copyTo.localPosition = newItemTransform.localPosition + new Vector3(0, spriteHeight / 2 + 0.3f, 0);
                        }

                        copyFrom = instance.transform.Find("CollectArticleMountTransform");
                        copyTo = newInstance.transform.Find("CollectArticleMountTransform");
                        UnityEditorInternal.ComponentUtility.CopyComponent(copyFrom);
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(copyTo);
                        if (copyFrom == null)
                        {
                            copyTo.localPosition = newItemTransform.localPosition;
                        }
                    }

                    //飘浮物体的sortingLayerName为Floating
                    if (mapObjectData.detachGrid)
                    {
                        for (int k = 0; k < newInstance.transform.childCount; k++)
                        {
                            SpriteRenderer renderer = newInstance.transform.GetChild(k).GetComponent<SpriteRenderer>();
                            if (renderer != null)
                            {
                                renderer.sortingLayerName = "Floating";
                            }
                        }
                    }
                    PrefabUtility.CreatePrefab(path, newInstance, ReplacePrefabOptions.Default);
                    DestroyImmediate(newInstance);
                }
            }
        }

        DestroyImmediate(basicPrefab);
        DestroyImmediate(withShadowObjectPrefab);
        DestroyImmediate(canDestroyObjectWithShadowPrefab);
        DestroyImmediate(canDestroyObjectNoShadowPrefab);
        DestroyImmediate(enemyMonsterPrefab);
        DestroyImmediate(allyMonsterPrefab);

        //EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        ResourcesPostprocessor.enabled = true;
    }

    [MenuItem("Appcpi/资源编辑/Assign Spine Assets")]
    public static void AssignSpineAssets()
    {
        GameObject[] monsterPrefabs = Resources.LoadAll<GameObject>("Prefabs/TilePrefabs/object/");
        foreach (GameObject monsterPrefab in monsterPrefabs)
        {
            string id = monsterPrefab.name;
            string id3 = id.Substring(0, 3);
            if ((id[0] != '5' || id[id.Length - 1] == '1' || id[id.Length - 1] == '6') && id3 != "611")
            {
                continue;
            }
            //SkeletonAnimation spineAnim = monsterPrefab.GetComponentInChildren<SkeletonAnimation>(true);
            //MapObject mapObject = monsterPrefab.GetComponent<MapObject>();
            //if (spineAnim != null && mapObject.skeletonAnimation != null && spineAnim)
            //{
            //    continue;
            //}
            //SkeletonDataAsset spineAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(string.Format("Assets/ResourcesRaw/Spine/{0}_SkeletonData.asset", id));
            //if (spineAsset == null)
            //{
            //    Debug.LogWarning(string.Format("{0} Spine Skeleton Assets Missing", id), monsterPrefab);
            //    continue;
            //}

            //EditorUtility.DisplayProgressBar("AssignSpineAssets", string.Format("正在生成{0}的Prefab 请稍后......", id), 1);
            //GameObject instance = Instantiate(monsterPrefab);
            //mapObject = instance.GetComponent<MapObject>();
            //Transform oldObject = instance.transform.Find(id);
            //spineAnim = SpineEditorUtilities.InstantiateSkeletonAnimation(spineAsset);
            //spineAnim.transform.SetParent(instance.transform);
            //spineAnim.transform.localPosition = oldObject.localPosition;
            //spineAnim.name = id;
            //spineAnim.transform.SetSiblingIndex(0);
            //mapObject.skeletonAnimation = spineAnim;
            //DestroyImmediate(oldObject.gameObject, true);
            //PrefabUtility.ReplacePrefab(instance, monsterPrefab);
        }
        EditorUtility.ClearProgressBar();
    }

    private static Sprite GetSprite(string name)
    {
        foreach (UGUISpriteAtlas atlas in atlasArray)
        {
            if (atlas.spriteDict.ContainsKey(name))
            {
                return atlas.spriteDict[name];
            }
        }
        return null;
    }

    public static void CreatObjectTile(GameObject go,string fileName)
    {
        string path = TileFilePath + fileName + "/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        SimpleTile tile = ScriptableObject.CreateInstance<SimpleTile>();
        tile.prefab = go;
        string outputPath = string.Format(TileFrefabPath + fileName + "/Tile{0}.prefab", go.name);
        AssetDatabase.DeleteAsset(outputPath);
        AssetDatabase.CreateAsset(tile, outputPath);
    }

    [MenuItem("Appcpi/资源编辑/Build MapObject Prefabs One GameObject")]
    public static void Find()
    {
        GameObject[] monsterPrefabs = Resources.LoadAll<GameObject>("Prefabs/TilePrefabs/object/");
        foreach (GameObject monsterPrefab in monsterPrefabs)
        {
            string id = monsterPrefab.name;
            EditorUtility.DisplayProgressBar("AssignSpineAssets", string.Format("正在生成{0}的Prefab 请稍后......", id), 1);
            GameObject instance = Instantiate(monsterPrefab);
            MapObject mapObject = instance.GetComponent<MapObject>();
            GameObject TipMountPosition = new GameObject("TipMountPosition");
            TipMountPosition.transform.SetParent(instance.transform);
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(instance.transform);
            mapObject.tipMountPosition = TipMountPosition.transform;
            mapObject.tipMountPosition.position = bounds.center+new Vector3(0, 0.75f) ;
            PrefabUtility.ReplacePrefab(instance, monsterPrefab, ReplacePrefabOptions.ReplaceNameBased);
            DestroyImmediate(instance, true);
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/Garss/Set Sprite")]
    public static void GarssSetSprite()
    {
        //string atlasFolder = Application.dataPath + "/ResourcesRaw/Atlas/";
        //string[] atlasFiles = Directory.GetFiles(atlasFolder, "*.prefab");
        //atlasArray = new UGUISpriteAtlas[atlasFiles.Length];
        //for (int i = 0; i < atlasFiles.Length; i++)
        //{
        //    string atlasFile = atlasFiles[i];
        //    string assetPath = "Assets" + atlasFile.Remove(0, Application.dataPath.Length);
        //    GameObject assetObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        //    UGUISpriteAtlas atlas = assetObject.GetComponent<UGUISpriteAtlas>();
        //    atlas.Init();
        //    atlasArray[i] = atlas;
        //}

        //GameObject[] gos = Selection.gameObjects;
        //foreach (GameObject go in gos)
        //{
        //    SpriteRenderer[] renderers = go.GetComponentsInChildren<SpriteRenderer>();
        //    foreach (SpriteRenderer ren in renderers)
        //    {
        //       ren.sprite = 
        //    }

        //    Debug.Log(go.name);
        //}
    }
}

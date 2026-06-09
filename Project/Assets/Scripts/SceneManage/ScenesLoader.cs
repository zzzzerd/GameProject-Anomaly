using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ScenesLoader : MonoBehaviour,ISaveService
{


    [Header("事件")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEventSO;
    public VoidEventSO newGameEventSO;
    public SceneLoadEventSO unloadedSceneEvent;


    [Header("传递的参数")]
    public Transform playerTransform;
    public Vector3 playerSpawnPoint; //��ҳ������һ������

    [Header("场景请求加载事件监听")]
    public SceneLoadEventSO loadEventSO;


    [Header("游戏场景")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO currentLoadedScene;
    public GameSceneSO mapScene;
    public GameSceneSO mainMenuScene;

    //һ
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;

    private bool isLoading;

    private bool fadeScreen;
    public float fadeDuration;

    /// <summary>
    /// 防止 LoadData 触发加载时与 OnloadComplete 的 Load() 形成死循环
    /// </summary>
    private bool isLoadingFromSaveData;

    /// <summary>
    /// 非读档触发的切场景（如异世界返回）结束后，是否需要补一次读档恢复场景物体
    /// </summary>
    private bool shouldRestoreAfterNextSceneLoad;

    /// <summary>
    /// 标记当前是否在异世界中，异世界中保存时不应覆盖存档场景
    /// </summary>
    [HideInInspector]
    public bool isInOtherWorld;



    private void Start()
    {
        // 第一次启动加载主菜单
        loadEventSO.RaiseLoadRequestEvent(mainMenuScene, playerSpawnPoint, true);
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEventSO.OnEventRaised += NewGame;

        // 注册存档（OnEnable 确保每次启用都注册，不遗漏 Save 调用）
        ISaveService saveService = this;
        saveService.TurnToSaveble();
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEventSO.OnEventRaised -= NewGame;
        
        ISaveService saveService=this;
        saveService.TurnToUnsaveble();
    }

/// <summary>
/// 
/// </summary>
/// <param name="locationToLoad"></param>
/// <param name="positionToGo"></param>
/// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 positionToGo, bool fadeScreen)
    {
        if (isLoading)
            return;

        isLoading = true;


        sceneToLoad = locationToLoad;
        this.positionToGo = positionToGo;
        this.fadeScreen = fadeScreen;
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
        Debug.Log("Load scene: " + sceneToLoad.sceneAssetReference.SubObjectName + " to position: " + positionToGo + " with fade screen: " + fadeScreen);

    }

    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            fadeEventSO.FadeIn(fadeDuration);

        }
        yield return new WaitForSeconds(fadeDuration);
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);   //��ʵ������ʲôֵ����ν��ֻ�ǽ�������¼�ȥ����uimananger
        yield return currentLoadedScene.sceneAssetReference.UnLoadScene();

        playerTransform.gameObject.SetActive(false);
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneAssetReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnloadComplete;

    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="handle"></param>
    private void OnloadComplete(AsyncOperationHandle<SceneInstance> handle)
    {
        currentLoadedScene = sceneToLoad;

        playerTransform.position = positionToGo;

        playerTransform.gameObject.SetActive(true);

        if (fadeScreen)
        {
            fadeEventSO.FadeOut(fadeDuration);
        }

        isLoading = false;


        if (currentLoadedScene.sceneType == SceneType.Location)
        {
            afterSceneLoadedEvent.RaiseEvent();

            // 只有在加载存档场景时，才延迟一帧后读档（恢复场景物体状态）
            // 非存档加载（正常场景切换）不需要读档，避免死循环
            if (isLoadingFromSaveData || shouldRestoreAfterNextSceneLoad)
            {
                isLoadingFromSaveData = false;
                shouldRestoreAfterNextSceneLoad = false;
                StartCoroutine(DelayedLoadAfterSceneLoaded());
            }
        }
    }

    private IEnumerator DelayedLoadAfterSceneLoaded()
    {
        yield return null; // 等一帧，让新场景的 SavePointBase 都 OnEnable 完成
        GameDataManager.Instance.Load();
    }




    private void NewGame()
    {
        Debug.Log("[ScenesLoader] NewGame 被触发");
        Debug.Log(
    $"[ScenesLoader] 出生点 = {playerSpawnPoint}"
);

        if (isLoading)
        {
            Debug.Log("[ScenesLoader] 当前正在加载，直接返回");
            return;
        }

        sceneToLoad = firstLoadScene;

        Debug.Log("[ScenesLoader] 准备加载：" + sceneToLoad.name);

        loadEventSO.RaiseLoadRequestEvent(
            sceneToLoad,
            playerSpawnPoint,
            true
        );
    }


    public UniqueId GetUniqueId()
    {
        //throw new NotImplementedException();
        return GetComponent<UniqueId>();
    }

    public void RequestRestoreAfterNextSceneLoad()
    {
        shouldRestoreAfterNextSceneLoad = true;
    }

    public void ReadSaveData(GameData data)
    {
        // 只有在非异世界时才保存当前场景到存档
        // 否则会把异世界场景覆盖掉真正的存档场景
        if (isInOtherWorld)
            return;

        data.SaveGameScene(currentLoadedScene);
    }

    public void LoadData(GameData data)
    {
        Debug.Log("ScemesLoader开始读档");
        var playerID = playerTransform.GetComponent<UniqueId>().Id;
        if (data.characterData.ContainsKey(playerID))
        {
            positionToGo = data.characterData[playerID].position;
            sceneToLoad = data.GetSavedScene();//会返回一个场景






            // 判断存档场景与当前场景是否相同
            bool sameScene = currentLoadedScene != null &&
                             sceneToLoad != null &&
                             currentLoadedScene.sceneAssetReference.AssetGUID == sceneToLoad.sceneAssetReference.AssetGUID;

            if (sameScene)
            {
                // 同一场景：只移动玩家位置，不重新加载
                playerTransform.position = positionToGo;
                return;
            }

            // 不同场景（例如按 L 从其它场景读档）：触发一次场景加载
            isLoadingFromSaveData = true;
            StartCoroutine(DelayedSceneReload());
        }
    }

    private IEnumerator DelayedSceneReload()
    {
        yield return null;
        // isLoadingFromSaveData 会在 OnloadComplete 中使用后自动重置，不要在这里清除
        OnLoadRequestEvent(sceneToLoad, positionToGo, false);
    }
}



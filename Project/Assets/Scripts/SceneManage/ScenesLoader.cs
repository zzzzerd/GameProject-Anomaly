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

    [Header("事件")]
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



    private void Start()
    {
        // 注册存档（Start 在 Awake 之后，确保 GameDataManager.Instance 已初始化）
        ISaveService saveService = this;
        saveService.TurnToSaveble();

        // 第一次启动加载主菜单
        loadEventSO.RaiseLoadRequestEvent(mainMenuScene, playerSpawnPoint, true);
    }

    private void Awake()
    {

    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEventSO.OnEventRaised += NewGame;
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
        //throw new NotImplementedException();
        currentLoadedScene = sceneToLoad;

        playerTransform.position = positionToGo;  //�ƶ�player������

        //playerTransform.position.Set
        playerTransform.gameObject.SetActive(true);

        if (fadeScreen)
        {
            fadeEventSO.FadeOut(fadeDuration);
        }

        isLoading = false;

     
        if (currentLoadedScene.sceneType == SceneType.Location)
            afterSceneLoadedEvent.RaiseEvent();
    }




    private void NewGame()
    {
        if (isLoading)
            return;

        sceneToLoad = firstLoadScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, playerSpawnPoint, true);
    }

    public UniqueId GetUniqueId()
    {
        //throw new NotImplementedException();
        return GetComponent<UniqueId>();
    }

    public void ReadSaveData(GameData data)
    {
        //保存这个scene文件
        data.SaveGameScene(currentLoadedScene);
        //throw new NotImplementedException();
    }

    public void LoadData(GameData data)
    {
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
                // 同一场景：只移动玩家位置，不重新加载（避免敌人重生）
                playerTransform.position = positionToGo;
            }
            else
            {
                // 不同场景（如从 OtherWorld 返回，Level1 已卸载）：重新加载存档场景
                loadEventSO.RaiseLoadRequestEvent(sceneToLoad, positionToGo, false);
            }
        }
    }
}



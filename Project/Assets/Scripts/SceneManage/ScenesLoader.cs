using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ScenesLoader : MonoBehaviour
{


    [Header("事件广播")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEventSO;
    public VoidEventSO newGameEventSO;
    public SceneLoadEventSO unloadedSceneEvent;


    [Header("玩家参数")]
    public Transform playerTransform;
    public Vector3 playerSpawnPoint; //玩家出生点第一次坐标

    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;


    [Header("场景参数")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO currentLoadedScene;
    public GameSceneSO mapScene;
    //public GameSceneSO mainMenuScene;

    //一些临时的变量，后续可能会用到
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;

    private bool isLoading;

    private bool fadeScreen;
    public float fadeDuration;



    private void Start()
    {
        //一开始要去的场景，不过map其实无所谓
        //后面要吧第一个场景改成mainmeu，然后在mainmenu里面有个开始探索的按钮会带到map里面去
        loadEventSO.RaiseLoadRequestEvent(mapScene, playerSpawnPoint, true);
        //currentLoadedScene = firstLoadScene;
    }
    private void Awake()
    {
        
        //currentLoadedScene.sceneAssetReference.LoadSceneAsync(LoadSceneMode.Additive);
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

    }

    /// <summary>
    /// 这是一个去往新场景的请求事件，参数分别是要去往的场景、在新场景中的位置、是否要淡入淡出
    /// </summary>
    /// <param name="locationToLoad"></param>
    /// <param name="positionToGo"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 positionToGo, bool fadeScreen)
    {
        if(isLoading)
            return;

        isLoading = true;
        
        
        //先赋值
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
            //只是测试
            Debug.Log("Load scene: " + sceneToLoad.sceneAssetReference.SubObjectName + " to position: " + positionToGo + " with fade screen: " + fadeScreen);

    }

    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            //逐渐变黑，然后卸载场景
            fadeEventSO.FadeIn(fadeDuration);

        }
        yield return new WaitForSeconds(fadeDuration);
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad,positionToGo, true);   //其实这里是什么值无所谓，只是借用这个事件去启动uimananger
        yield return currentLoadedScene.sceneAssetReference.UnLoadScene();

        playerTransform.gameObject.SetActive(false);
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneAssetReference.LoadSceneAsync(LoadSceneMode.Additive,true);
        loadingOption.Completed+=OnloadComplete;

    }

    /// <summary>
    /// 新场景都加载好了
    /// </summary>
    /// <param name="handle"></param>
    private void OnloadComplete(AsyncOperationHandle<SceneInstance> handle)
    {
        //throw new NotImplementedException();
        currentLoadedScene = sceneToLoad;

        playerTransform.position = positionToGo;  //移动player的坐标

        //playerTransform.position.Set
        playerTransform.gameObject.SetActive(true);

        if (fadeScreen)
        {
            fadeEventSO.FadeOut(fadeDuration);
        }

        isLoading = false;

        //场景加载完成的事件广播
        if(currentLoadedScene.sceneType ==SceneType.Location)
            afterSceneLoadedEvent.RaiseEvent();
    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        //OnLoadRequestEvent(sceneToLoad, playerSpawnPoint, true);
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, playerSpawnPoint, true);
    }
}




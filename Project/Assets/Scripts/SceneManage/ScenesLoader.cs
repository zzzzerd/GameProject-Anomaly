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


    [Header("玩家参数")]
    public Transform playerTransform;

    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO firstLoadScene;
    public GameSceneSO currentLoadedScene;

    //一些临时的变量，后续可能会用到
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;

    private bool isLoading;

    private bool fadeScreen;
    public float fadeDuration;




    private void Awake()
    {
        //Addressables.LoadSceneAsync(firstLoadScene.sceneAssetReference, LoadSceneMode.Additive);
        currentLoadedScene = firstLoadScene;
        currentLoadedScene.sceneAssetReference.LoadSceneAsync(LoadSceneMode.Additive);
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;

    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;

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
        //只是测试
        Debug.Log("Load scene: " + sceneToLoad.sceneAssetReference.SubObjectName + " to position: " + positionToGo + " with fade screen: " + fadeScreen);

    }

    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            //实现渐入渐出

        }
        yield return new WaitForSeconds(fadeDuration);
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
            //减出
        }

        isLoading = false;

        //场景加载完成的事件广播
        afterSceneLoadedEvent.RaiseEvent();
    }
}




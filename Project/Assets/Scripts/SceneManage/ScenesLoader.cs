using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ScenesLoader : MonoBehaviour, ISaveService
{


    [Header("场景加载完成后事件广播-通知照相机获取边界")]
    public VoidEventSO afterSceneLoadedEvent;

    [Header("场景渐变时间广播-通知fading Controller执行渐变")]
    public FadeEventSO fadeEventSO;

    [Header("场景事件监听-执行创建新游戏逻辑")]
    public VoidEventSO newGameEventSO;

    [Header("场景事件监听-执行OnLoadRequestEvent加载事件")]
    public SceneLoadEventSO loadEventSO;

    [Header("暂时好像没有用得到")]
    public SceneLoadEventSO unloadedSceneEvent;


    [Header("参数:player组件")]
    public Transform playerTransform;

    [Header("参数:场景渐变时间")]
    public float fadeDuration;


    [Header("参数:游戏level1出生点")]
    public Vector3 playerSpawnPoint; //出生点




    [Header("游戏场景")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO currentLoadedScene;
    public GameSceneSO mapScene;
    public GameSceneSO mainMenuScene;

    //
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool isLoading;
    private bool fadeScreen;


    private bool isLoadingFromSaveData;  // 防止 LoadData 触发加载时与 OnloadComplete 的 Load() 形成死循环
    private bool shouldRestoreAfterNextSceneLoad;// 非读档触发的切场景（如异世界返回）结束后，是否需要补一次读档恢复场景物体
    [HideInInspector]
    public bool isInOtherWorld;    /// 标记当前是否在异世界中，异世界中保存时不应覆盖存档场景



    private void Start()
    {
        //启动游戏加载的第一个场景
        LoadInitialScene();
    }


    /// <summary>
    /// 加载游戏第一个场景
    /// </summary>
    private void LoadInitialScene()
    {
        loadEventSO.RaiseLoadRequestEvent(mainMenuScene, playerSpawnPoint, true);
    }









    //两个监听函数
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


        //取消注册存档注册存档
        ISaveService saveService = this;
        saveService.TurnToUnsaveble();
    }







    /// <summary>
    /// 监听的具体实现
    /// </summary>




    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 positionToGo, bool fadeScreen)
    {
        //测试
        Debug.Log("[ScenesLoader] 监听到OnLoadRequestEvent 被触发,开始执行OnLoadRequestEvent");
        if (isLoading)
            return;

        isLoading = true;


        sceneToLoad = locationToLoad;
        this.positionToGo = positionToGo;
        this.fadeScreen = fadeScreen;
        if (currentLoadedScene != null)//当前有场景要先卸载掉然后加载新场景
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();//直接加载新场景
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
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);   //卸载事件
        yield return currentLoadedScene.sceneAssetReference.UnLoadScene();

        playerTransform.gameObject.SetActive(false);
        LoadNewScene();
    }



    //加载要去的场景
    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneAssetReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnloadComplete;

    }




    /// <summary>
    ///结束load的时候要做的事情
    /// </summary>
    /// <param name="handle"></param>
    private void OnloadComplete(AsyncOperationHandle<SceneInstance> handle)
    {
        currentLoadedScene = sceneToLoad;
        Debug.Log($"【不显示bug】OnloadComplete | 场景={currentLoadedScene.name} | sceneType={currentLoadedScene.sceneType}");
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







    /// <summary>
    /// 开始新游戏逻辑
    /// </summary>


    private void NewGame()
    {
        Debug.Log("[ScenesLoader] 监听到NewGame信号 开始执行newGame");

        if (isLoading)
        {
            Debug.Log("NewGame-[ScenesLoader] 当前正在加载新场景，就不重复了，直接返回");
            return;
        }

        // 开始新游戏前清除旧存档，防止"继续旅程"读到脏数据
        if (GameDataManager.Instance.HasSaveData())
        {
            GameDataManager.Instance.DeleteSaveData();
            Debug.Log("[ScenesLoader] 新游戏开始，旧存档已清除");
        }

        sceneToLoad = firstLoadScene;
        Debug.Log("NewGame-[ScenesLoader] 准备加载：" + sceneToLoad.name);

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
            positionToGo = data.characterData[playerID].position.ToVector3();
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
        // 通过 SO 广播，确保 UIManager 等所有监听者都能收到场景切换通知
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, positionToGo, false);
    }


    ///// <summary>
    ///// 加载游戏第一个场景
    ///// </summary>
    //private void LoadInitialScene()
    //{
    //    loadEventSO.RaiseLoadRequestEvent(mainMenuScene, playerSpawnPoint, true);
    //}
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.ResourceManagement.ResourceProviders;
//using UnityEngine.SceneManagement;

//public class OtherWorldManager : MonoBehaviour
//{
//    [Header("场景引用")]
//    public ScenesLoader scenesLoader;
//    public GameSceneSO otherWorldScene;

//    [Header("事件广播")]
//    public VoidEventSO enterOtherWorldEvent;
//    public VoidEventSO exitOtherWorldEvent;

//    [Header("参数")]
//    public float otherWorldDuration = 30f;

//    private GameSceneSO returnScene;
//    private Vector3 returnPosition;
//    private bool inOtherWorld;
//    private float remainingTime;
//    private AsyncOperationHandle<SceneInstance> otherWorldHandle;

//    // 缓存的 Level1 根物体列表
//    private List<GameObject> hiddenRootObjects = new List<GameObject>();

//    private void Update()
//    {
//        if (Keyboard.current.pKey.wasPressedThisFrame)
//        {
//            EnterOtherWorld();
//        }

//        // 倒计时
//        if (inOtherWorld)
//        {
//            remainingTime -= Time.deltaTime;
//            if (remainingTime <= 0)
//            {
//                remainingTime = 0;
//                ExitOtherWorld();
//            }
//        }
//    }

//    public float GetRemainingTime() => remainingTime;
//    public bool IsInOtherWorld() => inOtherWorld;

//    public void EnterOtherWorld()
//    {
//        if (inOtherWorld) return;
//        if (otherWorldScene == null) return;

//        inOtherWorld = true;
//        remainingTime = otherWorldDuration;

//        // 记录返回信息
//        returnScene = scenesLoader.currentLoadedScene;
//        returnPosition = scenesLoader.playerTransform.position;

//        // 隐藏玩家
//        scenesLoader.playerTransform.gameObject.SetActive(false);

//        // 隐藏 Level1 场景的所有根物体
//        HideCurrentSceneRoots();

//        // 广播：进入异世界
//        enterOtherWorldEvent?.RaiseEvent();

//        // Additive 加载异世界场景
//        var handle = otherWorldScene.sceneAssetReference.LoadSceneAsync(LoadSceneMode.Additive, true);
//        handle.Completed += OnOtherWorldLoaded;
//        otherWorldHandle = handle;
//    }

//    private void HideCurrentSceneRoots()
//    {
//        hiddenRootObjects.Clear();
//        if (returnScene != null && returnScene.sceneAssetReference.IsValid())
//        {
//            // 找到已加载的场景实例
//            for (int i = 0; i < SceneManager.sceneCount; i++)
//            {
//                var scene = SceneManager.GetSceneAt(i);
//                if (scene.name == returnScene.sceneAssetReference.SubObjectName)
//                {
//                    foreach (var rootGO in scene.GetRootGameObjects())
//                    {
//                        if (rootGO.activeSelf)
//                        {
//                            hiddenRootObjects.Add(rootGO);
//                            rootGO.SetActive(false);
//                        }
//                    }
//                    break;
//                }
//            }
//        }
//    }

//    private void ShowCurrentSceneRoots()
//    {
//        foreach (var go in hiddenRootObjects)
//        {
//            if (go != null)
//                go.SetActive(true);
//        }
//        hiddenRootObjects.Clear();
//    }

//    private void OnOtherWorldLoaded(AsyncOperationHandle<SceneInstance> handle)
//    {
//        scenesLoader.playerTransform.position = Vector3.zero;
//        scenesLoader.playerTransform.gameObject.SetActive(true);
//    }

//    public void ExitOtherWorld()
//    {
//        if (!inOtherWorld) return;

//        inOtherWorld = false;

//        // 广播：离开异世界
//        exitOtherWorldEvent?.RaiseEvent();

//        scenesLoader.playerTransform.gameObject.SetActive(false);

//        // 卸载异世界
//        if (otherWorldHandle.IsValid())
//            SceneManager.UnloadSceneAsync(otherWorldHandle.Result.Scene);

//        // 恢复 Level1
//        ShowCurrentSceneRoots();

//        scenesLoader.playerTransform.position = returnPosition;
//        scenesLoader.playerTransform.gameObject.SetActive(true);
//    }
//}


using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class OtherWorldManager : MonoBehaviour
{
    public ScenesLoader scenesLoader;
    [Header("事件")]
    public SceneLoadEventSO loadEventSO;

    [Header("异世界持续时间")]
    public float otherWorldDuration = 30f;
    private float remainingTime;



    [Header("异世界场景")]
    public GameSceneSO otherWorldScene;

    private GameSceneSO returnScene;
    private Vector3 returnPosition;


    [Header("异世界参数")]
    public bool inOtherWorld;

    [Header("其他饮用")]
    public PlayerStatBar playerStatBar;


    public bool IsInOtherWorld()
    {
        return inOtherWorld;
    }

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            EnterOtherWorld();
        }

        if (inOtherWorld)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                ExitOtherWorld();
            }
        }
    }

    public void EnterOtherWorld()
    {
        if (inOtherWorld)
            return;
        Debug.Log("进入异世界");

        inOtherWorld = true;
        Debug.Log("inOtherWorld = " + inOtherWorld);
        playerStatBar.SwitchToOtherWorld();
        remainingTime = otherWorldDuration;

        returnScene = scenesLoader.currentLoadedScene;
        returnPosition = scenesLoader.playerTransform.position;

        loadEventSO.RaiseLoadRequestEvent(
            otherWorldScene,
            Vector3.zero,
            true
        );
    }

    private void ExitOtherWorld()
    {
        playerStatBar.SwitchToNormalWorld();
        loadEventSO.RaiseLoadRequestEvent(
            returnScene,
            returnPosition,
            true
        );

        inOtherWorld = false;
    }

    //private IEnumerator ReturnAfter30Seconds()
    //{
    //    yield return new WaitForSeconds(30f);

    //    // 返回原场景
    //    loadEventSO.RaiseLoadRequestEvent(
    //        returnScene,
    //        returnPosition,
    //        true
    //    );

    //    inOtherWorld = false;
    //}

    public float GetRemainingTime()
    {
        return remainingTime;
    }
}



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
    [Header("进入异世界的出生点")]
    public Vector3 otherWorldSpawnPoint;


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

                //离开
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
        scenesLoader.isInOtherWorld = true;
        Debug.Log("inOtherWorld = " + inOtherWorld);
        playerStatBar.SwitchToOtherWorld();
        remainingTime = otherWorldDuration;

        returnScene = scenesLoader.currentLoadedScene;
        returnPosition = scenesLoader.playerTransform.position;

        loadEventSO.RaiseLoadRequestEvent(
            otherWorldScene,
            otherWorldSpawnPoint,
            true
        );
    }

    //private void ExitOtherWorld()
    //{
    //    playerStatBar.SwitchToNormalWorld();
    //    inOtherWorld = false;
    //    scenesLoader.isInOtherWorld = false;

    //    // 直接从存档数据加载回存档点场景
    //    // 不调 Save()，因为 Save() 会把异世界场景覆盖掉存档场景
    //    GameDataManager.Instance.Load();
    //}

    private void ExitOtherWorld()
    {
        playerStatBar.SwitchToNormalWorld();

        inOtherWorld = false;
        scenesLoader.isInOtherWorld = false;

        // 保持原有切场景逻辑不变，只在切回后补一次读档恢复状态
        scenesLoader.RequestRestoreAfterNextSceneLoad();

        loadEventSO.RaiseLoadRequestEvent(
            returnScene,
            returnPosition,
            true
        );
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
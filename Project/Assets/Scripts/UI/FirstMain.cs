using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMain : MonoBehaviour
{
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;

    [Header("一些事件")]
    public GameSceneSO mapScene;

    [Header("其他参数")]
    public Vector3 playerSpawnPoint;

    public void StartNewExplore()
    {
        loadEventSO.RaiseLoadRequestEvent(mapScene, playerSpawnPoint,true);
        Debug.Log("开始新的旅程");
    }
    public void ContinueExplore()
    {
        //loadEventSO.RaiseLoadRequestEvent(mapScene, playerSpawnPoint, true);
        Debug.Log("继续旅程");
    }
    public void ExistGame()
    {
        Application.Quit();
        Debug.Log("离开游戏");
    }

    public void ShowTutorial()
    {
        //Application.Quit();
        Debug.Log("离开游戏");
    }
}

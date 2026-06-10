using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMain : MonoBehaviour
{
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;

    [Header("场景")]
    public GameSceneSO mapScene;

    [Header("继续旅程按钮（有存档时显示）")]
    public GameObject continueButton;

    [Header("结局记录面板")]
    public EndingRecordPanel endingRecordPanel;

    private void Start()
    {
        // 根据是否有存档，决定继续旅程按钮的显隐
        if (continueButton != null)
            continueButton.SetActive(GameDataManager.Instance != null && GameDataManager.Instance.HasSaveData());
    }

    /// <summary>
    /// 开始新旅程：进入地图界面，在地图点 Level1 是新游戏
    /// </summary>
    public void StartNewExplore()
    {
        Map.IsContinueMode = false;
        loadEventSO.RaiseLoadRequestEvent(mapScene, Vector3.zero, true);
        Debug.Log("开始新的旅程");
    }

    /// <summary>
    /// 继续旅程：进入地图界面，在地图点 Level1 是读档继续
    /// </summary>
    public void ContinueExplore()
    {
        Map.IsContinueMode = true;
        loadEventSO.RaiseLoadRequestEvent(mapScene, Vector3.zero, true);
        Debug.Log("继续旅程 → 进入地图");
    }

    public void ExistGame()
    {
        Application.Quit();
        Debug.Log("离开游戏");
    }

    public void ShowTutorial()
    {
        Debug.Log("显示教程");
    }

    public void ShowEndingRecords()
    {
        endingRecordPanel?.Open();
    }
}

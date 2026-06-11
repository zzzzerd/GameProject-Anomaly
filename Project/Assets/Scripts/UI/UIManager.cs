using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    public PlayerStatBar playerStatBar;
    [Header("迎来结局的时候展示的面板")]
    public GameObject gameOverPanel;

    [Header("暂停面板")]
    public GameObject pausePanel;
    public GameObject gearButton;  // 右上角齿轮按钮

    [Header("事件监听")]
    public CharacterEventSO healthEvent;
    public SceneLoadEventSO loadEvent;



    [Header("事件监听-获得了结局结果（EndingManager 广播）")]
    public EndingResultEventSO endingResultEventSO;

    [Header("事件广播-死亡结局（发给 EndingManager 判断）")]
    public VoidEventSO deathEndingEventSO;

    [Header("死亡面板按钮参数")]
    public GameSceneSO mainMenuScene;
    public PlayerController playerController;
    [Header("事件广播-开始新游戏")]
    public VoidEventSO newGameEventSO;

    [Header("一些状态")]
    private bool isDying;   // 防重入：死亡流程进行中，忽略后续 hp==0 的重复事件

    /// <summary>
    /// 注册事件（固定写法）
    /// </summary>
    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        loadEvent.LoadRequestEvent += OnLoadEvent;
        endingResultEventSO.OnEventRaised += OnEndingResult;
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        loadEvent.LoadRequestEvent -= OnLoadEvent;
        endingResultEventSO.OnEventRaised -= OnEndingResult;
    }


    //加载了新场景
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        var isMenu = arg0.sceneType == SceneType.Menu;
        Debug.Log($"【不显示bug】UIManager.OnLoadEvent | 场景={arg0.name} | sceneType={arg0.sceneType} | isMenu={isMenu} → bar设为{!isMenu}");
        playerStatBar.gameObject.SetActive(!isMenu);
        if (gearButton != null) gearButton.SetActive(!isMenu);
        HideGameOverPanel();
    }


    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChange(percentage);
        playerStatBar.OnPowerChange(character);


        //这里是判断死亡的最终
        if (character.currentHealth <= 0 && !isDying)
        {
            isDying = true;
            StartCoroutine(DeathRoutine());
        }
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(2f);
        // 通知 EndingManager 处理死亡结局
        deathEndingEventSO.RaiseEvent();
    }





    /// <summary>
    /// 收到 EndingManager 广播的结局结果，显示面板
    /// </summary>
    private void OnEndingResult(EndingType endingType)
    {
        Debug.Log($"[UIManager] 收到结局结果: {endingType}");
        isDying = false;  // 结局流程走完，重置防重入标志
        ShowGameOverPanel();
    }

    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("[UIManager] GameOverPanel 面板未绑定，无法显示死亡面板，也不会暂停时间。");
        }

        //LogPanelInteractableState("ShowGameOverPanel");
    }


    //关闭面板
    private void HideGameOverPanel()
    {
        isDying = false;
        Time.timeScale = 1f;  // 先恢复时间，再做其他操作

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }





    /// <summary>
    /// 结束面板按钮功能1--返回主菜单
    /// </summary>
    public void ReturnToMain()
    {
        Debug.Log($"[UIManager] 点击 ReturnToMain | timeScale={Time.timeScale} | mainMenuScene={mainMenuScene} | loadEvent={loadEvent}");
        HideGameOverPanel();
        Debug.Log($"[UIManager] HideGameOverPanel 执行完 | timeScale={Time.timeScale}");

        playerController?.ReviveAfterLoad();
        Debug.Log($"[UIManager] ReviveAfterLoad 执行完 | playerController={playerController}");

        if (mainMenuScene != null)
        {
            Debug.Log("[UIManager] 触发 loadEvent.RaiseLoadRequestEvent → 主菜单");
            loadEvent.RaiseLoadRequestEvent(mainMenuScene, Vector3.zero, true);
        }
        else
        {
            Debug.LogError("[UIManager] mainMenuScene 未绑定，无法返回主菜单");
        }
    }


    //结束面板功能2--开始新游戏(其实)
    public void RestartGame()
    {
        Debug.Log("[UIManager] 点击 RestartGame 按钮");
        HideGameOverPanel();
        newGameEventSO.RaiseEvent();
    }


    // ==================== 暂停面板 ====================

    /// <summary>
    /// 齿轮按钮 OnClick 连这个（替代直连 SetActive）
    /// </summary>
    public void ShowPausePanel()
    {
        if (pausePanel == null) return;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 暂停面板关闭按钮 OnClick 连这个
    /// </summary>
    public void HidePausePanel()
    {
        if (pausePanel == null) 
            return;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 暂停面板按钮继续游戏
    /// </summary>
    public void PauseContinue()
    {
       
        HidePausePanel();
    }

    /// <summary>
    /// 暂停面板按钮：保存并返回主菜单（不删存档，让玩家下次可以继续）
    /// </summary>
    public void PauseSaveAndReturnToMain()
    {
        HidePausePanel();


        playerController?.ReviveAfterLoad();//不用new就要重新设置一下
        if (mainMenuScene != null)
            loadEvent.RaiseLoadRequestEvent(mainMenuScene, Vector3.zero, true);
        else
            Debug.LogError("[UIManager] mainMenuScene 未绑定");
    }

    /// <summary>
    /// 暂停面板按钮：放弃本局，开始新游戏（删除进度存档）
    /// </summary>
    public void PauseStartNewGame()
    {
        HidePausePanel();


        // 删除进度存档
        GameDataManager.Instance.DeleteSaveData();
        newGameEventSO.RaiseEvent();
    }
}

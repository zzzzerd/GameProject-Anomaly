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
    public GameObject gameOverPanel;

    [Header("事件监听")]
    public CharacterEventSO healthEvent;//监听事件
    public SceneLoadEventSO loadEvent;

    [Header("死亡面板按钮参数")]
    public GameSceneSO mainMenuScene;
    public PlayerController playerController;
    public VoidEventSO newGameEventSO;

    [Header("一些状态")]
    private bool gameOverShowing;

    /// <summary>
    /// 注册事件（固定写法）
    /// </summary>
    private void OnEnable()
    {
        //Debug.Log($"[UIManager] OnEnable | healthEvent={(healthEvent != null)} | loadEvent={(loadEvent != null)} | gameOverPanel={(gameOverPanel != null)}");
        healthEvent.OnEventRaised += OnHealthEvent; //一个事件可以注册多个函数
        loadEvent.LoadRequestEvent += OnLoadEvent;
    }

    /// <summary>
    /// 取消注册，所以就不会接收到事件的消息了
    /// </summary>
    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        loadEvent.LoadRequestEvent -= OnLoadEvent;

    }

    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        var isMenu = arg0.sceneType == SceneType.Menu;
        playerStatBar.gameObject.SetActive(!isMenu);
        HideGameOverPanel();
    }

    //private void OnHealthEvent(Character character)
    //{
    //    var persentage = character.currentHealth / character.maxHealth;
    //    playerStatBar.OnHealthChange(persentage);
    //    playerStatBar.OnPowerChange(character);

    //    //if (character.currentHealth <= 0)
    //    //{
    //    //    Debug.Log($"[UIManager] 检测到死亡血量，准备打开死亡面板。character={character.name}, hp={character.currentHealth}");
    //    //    ShowGameOverPanel();
    //    //}
    //    if (character.currentHealth <= 0 && !gameOverShowing)
    //    {
    //        gameOverShowing = true;
    //        ShowGameOverPanel();
    //    }
    //}

    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChange(percentage);
        playerStatBar.OnPowerChange(character);

        if (character.currentHealth <= 0)
        {
            StartCoroutine(DeathRoutine());
        }
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(2f);
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
            Debug.LogWarning("[UIManager] gameOverPanel 未绑定，无法显示死亡面板，也不会暂停时间。");
        }

        LogPanelInteractableState("ShowGameOverPanel");
    }

    private void HideGameOverPanel()
    {
        gameOverShowing = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        LogPanelInteractableState("HideGameOverPanel");
    }

    public void ReturnToMain()
    {
        Debug.Log("[UIManager] 点击 ReturnToMain 按钮");
        HideGameOverPanel();
        playerController?.ReviveAfterLoad();

        if (mainMenuScene != null)
        {
            Debug.Log("[UIManager] 开始加载主菜单场景");
            loadEvent.RaiseLoadRequestEvent(mainMenuScene, Vector3.zero, true);
        }
        else
        {
            Debug.LogError("[UIManager] mainMenuScene 未绑定，无法返回主菜单");
        }
    }

    public void RestartGame()
    {
        Debug.Log("[UIManager] 点击 RestartGame 按钮");

        HideGameOverPanel();

        newGameEventSO.RaiseEvent();
    }

    private void LogPanelInteractableState(string from)
    {
        var panelActiveSelf = gameOverPanel != null && gameOverPanel.activeSelf;
        var panelActiveInHierarchy = gameOverPanel != null && gameOverPanel.activeInHierarchy;
        Debug.Log($"[UIManager] {from} | panelRef={(gameOverPanel != null)} | activeSelf={panelActiveSelf} | activeInHierarchy={panelActiveInHierarchy} | timeScale={Time.timeScale}");

        if (EventSystem.current == null)
        {
            Debug.LogError("[UIManager] 当前场景没有 EventSystem，UI 按钮无法点击");
        }
        else
        {
            Debug.Log($"[UIManager] EventSystem 存在：{EventSystem.current.name}");
        }

        if (gameOverPanel == null)
            return;

        var buttons = gameOverPanel.GetComponentsInChildren<Button>(true);
        Debug.Log($"[UIManager] 死亡面板按钮数量: {buttons.Length}");
        foreach (var btn in buttons)
        {
            Debug.Log($"[UIManager] Button={btn.name}, activeInHierarchy={btn.gameObject.activeInHierarchy}, interactable={btn.interactable}");
        }

        var groups = gameOverPanel.GetComponentsInParent<CanvasGroup>(true);
        foreach (var group in groups)
        {
            Debug.Log($"[UIManager] CanvasGroup={group.name}, alpha={group.alpha}, interactable={group.interactable}, blocksRaycasts={group.blocksRaycasts}");
        }
    }
}

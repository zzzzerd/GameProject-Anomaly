using UnityEngine;
using TMPro;

/// <summary>
/// 任务面板（始终显示在游戏中，切主菜单时由 UIManager 隐藏）
/// 监听 taskUpdatedEventSO，刷新显示
/// </summary>
public class TaskPanel : MonoBehaviour
{
    [Header("UI 引用")]
    public TextMeshProUGUI statueProgressText;   // 显示 "Statues: 1 / 3"
    public TextMeshProUGUI bossStatusText;        // 显示 "Sorcerer: Defeated / Not Defeated"

    [Header("监听 - 任务状态更新")]
    public VoidEventSO taskUpdatedEventSO;

    private void OnEnable()
    {
        if (taskUpdatedEventSO != null)
            taskUpdatedEventSO.OnEventRaised += RefreshUI;
    }

    private void OnDisable()
    {
        if (taskUpdatedEventSO != null)
            taskUpdatedEventSO.OnEventRaised -= RefreshUI;
    }

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        var tm = TaskManager.Instance;
        if (tm == null) return;

        if (statueProgressText != null)
            statueProgressText.text = $"Statues: {tm.StatuesActivated} / {tm.statuesRequired}";

        if (bossStatusText != null)
            bossStatusText.text = $"Sorcerer: {(tm.BossDefeated ? "Defeated" : "Not Defeated")}";
    }
}

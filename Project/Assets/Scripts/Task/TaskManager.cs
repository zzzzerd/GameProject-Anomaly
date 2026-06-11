using UnityEngine;

/// <summary>
/// 任务管理器（挂在 Persistent 场景）
/// 监听雕像激活和 Boss 死亡，维护任务状态，完成后广播
/// </summary>
public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [Header("任务参数")]
    public int statuesRequired = 3;

    [Header("监听:新游戏（重置任务状态）")]
    public VoidEventSO newGameEventSO;

    [Header("监听:好雕像激活（StatueSavePoint 广播）")]
    public VoidEventSO statueActivatedEventSO;

    [Header("监听:Boss 死亡")]
    public VoidEventSO bossDefeatedEventSO;

    [Header("广播:任务状态更新（TaskPanel 监听）")]
    public VoidEventSO taskUpdatedEventSO;

    [Header("广播:全部任务完成")]
    public VoidEventSO allTasksCompletedEventSO;



    // 任务状态
    public int StatuesActivated { get; private set; } = 0;
    public bool BossDefeated { get; private set; } = false;

    private bool completed = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (newGameEventSO != null)
            newGameEventSO.OnEventRaised += ResetTasks;
        if (statueActivatedEventSO != null)
            statueActivatedEventSO.OnEventRaised += OnStatueActivated;
        if (bossDefeatedEventSO != null)
            bossDefeatedEventSO.OnEventRaised += OnBossDefeated;
    }

    private void OnDisable()
    {
        if (newGameEventSO != null)
            newGameEventSO.OnEventRaised -= ResetTasks;
        if (statueActivatedEventSO != null)
            statueActivatedEventSO.OnEventRaised -= OnStatueActivated;
        if (bossDefeatedEventSO != null)
            bossDefeatedEventSO.OnEventRaised -= OnBossDefeated;
    }

    private void OnStatueActivated()
    {
        if (StatuesActivated >= statuesRequired) return;
        StatuesActivated++;
        Debug.Log($"[TaskManager] 雕像激活 {StatuesActivated}/{statuesRequired}");
        taskUpdatedEventSO?.RaiseEvent();
        CheckAllCompleted();
    }

    private void OnBossDefeated()
    {
        if (BossDefeated) return;
        BossDefeated = true;
        Debug.Log("[TaskManager] Boss 已击败");
        taskUpdatedEventSO?.RaiseEvent();
        CheckAllCompleted();
    }

    private void CheckAllCompleted()
    {
        if (completed) return;
        if (StatuesActivated >= statuesRequired && BossDefeated)
        {
            completed = true;
            Debug.Log("[TaskManager] 已经完成了");
            allTasksCompletedEventSO?.RaiseEvent();
        }
    }

    /// <summary>
    /// 新游戏/重新开始时重置任务状态
    /// </summary>
    public void ResetTasks()
    {
        StatuesActivated = 0;
        BossDefeated = false;
        completed = false;
        taskUpdatedEventSO?.RaiseEvent();
    }
}

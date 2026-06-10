using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 结局管理器（挂在 Persistent 场景）
/// 职责：判断结局类型、写入结局记录、删除进度存档、广播结局结果事件
/// </summary>
public class EndingManager : MonoBehaviour
{
    [Header("广播-结局判断结果（UIManager 监听此事件显示面板）")]
    public EndingResultEventSO endingResultEventSO;

    [Header("监听-死亡事件（玩家死亡时触发）")]
    public VoidEventSO deathEndingEventSO;

    private void OnEnable()
    {
        deathEndingEventSO.OnEventRaised += OnDeathEnding;
    }

    private void OnDisable()
    {
        deathEndingEventSO.OnEventRaised -= OnDeathEnding;
    }

    private void Update()
    {
        // 测试用：按 O 键手动触发结局判断（模拟 Boss 死亡）
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            Debug.Log("[EndingManager] 按下 O 键，触发结局判断");
            TriggerEndingByStats();
        }
    }


    /// <summary>
    /// 死亡结局入口（由 deathEndingEventSO 触发）
    /// </summary>
    private void OnDeathEnding()
    {
        TriggerEnding(EndingType.Death);
    }

    /// <summary>
    /// 根据玩家统计数据判断结局
    /// </summary>
    public void TriggerEndingByStats()
    {
        //获取player的统计值
        var stats = GameDataManager.Instance.Data.playerStats;

        //计算结局公式
        int total = stats.killedEnemies + stats.litCampfires + stats.activatedStars
                  + stats.enteredOtherWorld + stats.openedChests;
        //根据数值获取结局
        EndingType ending = (total == 0) ? EndingType.GoodEnding : EndingType.BadEnding;


        //测试输出
        Debug.Log($"[EndingManager] 统计总值={total}，判定结局={ending}");

        //激发结局
        TriggerEnding(ending);
    }

    /// <summary>
    /// 
    /// 统一处理结局：写记录 → 删进度档 → 广播结果
    /// </summary>
    private void TriggerEnding(EndingType endingType)
    {
        //迎来结局

        GameDataManager.Instance.SaveEndingRecord(endingType);
        GameDataManager.Instance.DeleteSaveData();
        endingResultEventSO.RaiseEvent(endingType);
        Debug.Log($"[EndingManager] 结局触发完成: {endingType}，");
    }
}

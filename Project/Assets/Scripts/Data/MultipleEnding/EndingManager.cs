using UnityEngine;

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

    [Header("监听-任务全部完成（TaskManager 广播）")]
    public VoidEventSO allTasksCompletedEventSO;

    private void OnEnable()
    {
        deathEndingEventSO.OnEventRaised += OnDeathEnding;
        if (allTasksCompletedEventSO != null)
            allTasksCompletedEventSO.OnEventRaised += TriggerEndingByStats;
    }

    private void OnDisable()
    {
        deathEndingEventSO.OnEventRaised -= OnDeathEnding;
        if (allTasksCompletedEventSO != null)
            allTasksCompletedEventSO.OnEventRaised -= TriggerEndingByStats;
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
    /// 哪个属性最高就走对应结局；侵蚀最高时再看力量高低
    /// </summary>
    public void TriggerEndingByStats()
    {
        var stats = GameDataManager.Instance.Data.playerStats;


        //计算三个方向的值
        int strength   = stats.killedEnemies + stats.killedBosses * 5;
        int hope       = stats.activatedStars * 3 + stats.litCampfires * 2;
        int corruption = stats.enteredOtherWorld * 4;

        Debug.Log($"[EndingManager] strength={strength} hope={hope} corruption={corruption}");

        EndingType ending;

        if (strength >= hope && strength >= corruption)
        {
            ending = EndingType.Warrior;       
        }
        else if (hope >= strength && hope >= corruption)
        {
            ending = EndingType.Saint;// 希望最高
        }
        else

        {
            // 侵蚀最高
            ending = (strength >= 10) ? EndingType.AnomalySage : EndingType.LostSoul;
        }

        // 全都是 0 → 归乡者
        if (strength == 0 && hope == 0 && corruption == 0)
            ending = EndingType.Farmer;

        Debug.Log($"[EndingManager] 判定结局={ending}");
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

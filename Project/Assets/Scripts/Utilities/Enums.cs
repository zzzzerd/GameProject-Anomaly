public enum NPCState
{
    Patrol,Chase,Skill,
    // Boss 专用
    BossIdle, BossChase, BossAttack
}
public enum SceneType {
    Location , Menu
}


public enum EnemyType
{
    Normal, Boss
}

public enum EndingType
{
    Death,       // 死亡结局
    GoodEnding,  // 好结局（统计值之和 == 0）
    BadEnding    // 坏结局（统计值之和 > 0）
}

/// <summary>
/// 雕塑存档点类型
/// </summary>
public enum StatueType
{
    Good,       // 好的雕塑：加血 + 统计
    Anomaly     // 异常雕塑：切换到 OtherWorld
}
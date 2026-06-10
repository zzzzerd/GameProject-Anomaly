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
    Death,          // 死亡结局
    Warrior,        // 勇士：力量最高
    Saint,          // 圣者：希望最高
    AnomalySage,    // 异界贤者：侵蚀最高 + 力量高
    LostSoul,       // 失魂者：侵蚀最高 + 力量低
    Farmer          // 归乡者：其余情况
}

/// <summary>
/// 雕塑存档点类型
/// </summary>
public enum StatueType
{
    Good,       // 好的雕塑：加血 + 统计
    Anomaly     // 异常雕塑：切换到 OtherWorld
}
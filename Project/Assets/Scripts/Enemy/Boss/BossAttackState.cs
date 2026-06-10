/// <summary>
/// 暂未使用，Boss 攻击逻辑已集成在 BossChaseState 中
/// </summary>
public class BossAttackState : BaseState
{
    public override void OnEnter(Enemy enemy) { currentEnemy = enemy; }
    public override void LogicUpdate() { }
    public override void PhysicsUpdate() { }
    public override void OnExit() { }
}

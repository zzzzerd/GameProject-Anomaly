using UnityEngine;

/// <summary>
/// Boss 静止状态：原地不动，播放 Idle 动画，等待发现玩家
/// </summary>
public class BossIdleState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.anim.SetBool("chase", false);
        currentEnemy.rb.velocity = Vector2.zero;
    }

    public override void LogicUpdate()
    {
        // 发现玩家 → 追逐
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.BossChase);
        }
    }

    public override void PhysicsUpdate()
    {
        // 原地静止
        if (!currentEnemy.isHurt && !currentEnemy.isDead)
        {
            currentEnemy.rb.velocity = Vector2.zero;
        }
    }

    public override void OnExit() { }
}

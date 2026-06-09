using UnityEngine;

/// <summary>
/// Boss 追逐状态：朝玩家飞行追击
/// </summary>
public class BossChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("chase", true);
        attack = enemy.GetComponent<Attack>();
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
    }

    public override void LogicUpdate()
    {
        // 丢失目标太久 → 回到 Idle
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.BossIdle);
            return;
        }

        // 有 attacker 才能追击
        if (currentEnemy.attacker == null) return;

        // 目标点：玩家头顶上方（Boss 漂浮攻击）
        target = currentEnemy.attacker.position + Vector3.up * 1.5f;

        // 进入攻击范围 → 切换攻击状态
        float dist = Vector3.Distance(currentEnemy.transform.position, target);
        if (attack != null && dist <= attack.attackRange)
        {
            currentEnemy.SwitchState(NPCState.BossAttack);
            return;
        }

        // 朝目标移动
        moveDir = (target - currentEnemy.transform.position).normalized;

        // 面向
        if (moveDir.x > 0)
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        else if (moveDir.x < 0)
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isHurt && !currentEnemy.isDead)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("chase", false);
    }
}

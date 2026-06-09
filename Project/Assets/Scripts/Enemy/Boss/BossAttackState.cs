using UnityEngine;

/// <summary>
/// Boss 攻击状态：在玩家附近发动攻击，攻击完追不上就回到追逐
/// </summary>
public class BossAttackState : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;
    private float attackTimer;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = 0; // 攻击时减速/停下
        attack = enemy.GetComponent<Attack>();
        attackTimer = attack != null ? attack.attackRate : 2f;
        currentEnemy.anim.SetBool("chase", false);
    }

    public override void LogicUpdate()
    {
        // 丢失目标太久 → Idle
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.BossIdle);
            return;
        }

        if (currentEnemy.attacker == null) return;

        target = currentEnemy.attacker.position + Vector3.up * 1.5f;
        float dist = Vector3.Distance(currentEnemy.transform.position, target);

        // 超出攻击范围 → 回到追逐
        if (attack != null && dist > attack.attackRange * 1.5f)
        {
            currentEnemy.SwitchState(NPCState.BossChase);
            return;
        }

        // 攻击计时器
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            currentEnemy.anim.SetTrigger("attack");
            attackTimer = attack != null ? attack.attackRate : 2f;
        }

        // 朝玩家微调位置
        moveDir = (target - currentEnemy.transform.position).normalized;
        if (moveDir.x > 0)
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        else if (moveDir.x < 0)
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isHurt && !currentEnemy.isDead)
        {
            // 攻击时慢速靠近玩家
            currentEnemy.rb.velocity = moveDir * currentEnemy.normalSpeed * 0.3f * Time.deltaTime;
        }
    }

    public override void OnExit()
    {
        attackTimer = 0;
    }
}

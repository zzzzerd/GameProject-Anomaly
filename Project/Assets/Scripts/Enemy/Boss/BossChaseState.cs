using UnityEngine;

/// <summary>
/// Boss 追逐状态：朝玩家飞行追击（仿 BatChaseState）
/// </summary>
public class BossChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;
    private float attackRateCounter;
    private bool isAttack;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("chase", true);
        attack = enemy.GetComponentInChildren<Attack>();
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
    }

    public override void LogicUpdate()
    {
        // 丢失目标太久 → 回 Idle
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.BossIdle);
            return;
        }

        // 每帧刷新 attacker
        currentEnemy.FoundPlayer();

        if (currentEnemy.attacker == null) return;

        // 目标点：玩家位置抬高一点
        target = new Vector3(
            currentEnemy.attacker.position.x,
            currentEnemy.attacker.position.y + 1f,
            0
        );

        float stopDist = (currentEnemy is BossEnemy boss) ? boss.attackStopDistance : 2f;

        // 进入攻击停止距离 → 停下播攻击动画
        if (Vector3.Distance(currentEnemy.transform.position, target) <= stopDist)
        {
            isAttack = true;

            if (currentEnemy.isHurt)
                currentEnemy.rb.velocity = Vector2.zero;

            attackRateCounter -= Time.deltaTime;
            if (attackRateCounter <= 0)
            {
                currentEnemy.anim.SetTrigger("attack");
                if (attack != null)
                    attackRateCounter = attack.attackRate;
                else
                    attackRateCounter = 1.5f;
            }
        }
        else
        {
            isAttack = false;
        }

        moveDir = (target - currentEnemy.transform.position).normalized;

        // 朝向
        if (moveDir.x > 0)
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        if (moveDir.x < 0)
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !isAttack)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }
        else if (isAttack)
        {
            currentEnemy.rb.velocity = Vector2.zero;
        }
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("chase", false);
        isAttack = false;
        attackRateCounter = 0;
    }
}

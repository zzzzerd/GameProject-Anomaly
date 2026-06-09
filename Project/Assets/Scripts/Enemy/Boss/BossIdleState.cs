using UnityEngine;

/// <summary>
/// Boss 静止/巡逻状态：漂浮在原位附近，等待发现玩家
/// </summary>
public class BossIdleState : BaseState
{
    private float idleTimer;
    private Vector3 floatTarget;
    private Vector3 moveDir;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        currentEnemy.anim.SetBool("chase", false);

        // 随机一个漂浮目标点（在出生点附近）
        floatTarget = currentEnemy.spwanPoint + new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(-1f, 1f),
            0
        );
    }

    public override void LogicUpdate()
    {
        // 发现玩家 → 追逐
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.BossChase);
            return;
        }

        // 到达漂浮目标 → 换一个新目标
        if (Vector3.Distance(currentEnemy.transform.position, floatTarget) < 0.3f)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > 1.5f)
            {
                idleTimer = 0;
                floatTarget = currentEnemy.spwanPoint + new Vector3(
                    Random.Range(-2f, 2f),
                    Random.Range(-1f, 1f),
                    0
                );
            }
        }

        // 面向移动方向
        moveDir = (floatTarget - currentEnemy.transform.position).normalized;
        if (moveDir.x > 0)
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        else if (moveDir.x < 0)
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.wait && !currentEnemy.isHurt && !currentEnemy.isDead)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }
        else
        {
            currentEnemy.rb.velocity = Vector2.zero;
        }
    }

    public override void OnExit()
    {
        idleTimer = 0;
    }
}

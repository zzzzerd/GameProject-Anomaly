using UnityEngine;

public class GhostChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;
    private float attackRateCounter;
    private bool isAttacking;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        attack = enemy.GetComponent<Attack>();
        currentEnemy.anim.SetBool("chase", true);
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
            return;
        }

        target = new Vector3(currentEnemy.attacker.position.x, currentEnemy.attacker.position.y + 1.5f, 0);

        float dx = Mathf.Abs(target.x - currentEnemy.transform.position.x);
        float dy = Mathf.Abs(target.y - currentEnemy.transform.position.y);

        if (dx <= attack.attackRange && dy <= attack.attackRange)
        {
            isAttacking = true;
            if (currentEnemy.isHurt) currentEnemy.rb.velocity = Vector2.zero;

            attackRateCounter -= Time.deltaTime;
            if (attackRateCounter <= 0)
            {
                currentEnemy.anim.SetTrigger("attack");
                attackRateCounter = attack.attackRate;
            }
        }
        else
        {
            isAttacking = false;
        }

        moveDir = (target - currentEnemy.transform.position).normalized;
        if (moveDir.x > 0) currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        if (moveDir.x < 0) currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !isAttacking)
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("chase", false);
    }
}

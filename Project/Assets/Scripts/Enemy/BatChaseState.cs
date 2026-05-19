using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;
    private float attackRateCounter;

    private bool isAttack;
    public override void OnEnter(Enemy enmey)
    {
        currentEnemy = enmey;
        //Debug.Log("切换到Chase");
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;//切换逻辑
        //currentEnemy.anim.SetBool("run", true);//播放动画
        attack = enmey.GetComponent<Attack>();

        //防止一直反反复复转换状态
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;

        currentEnemy.anim.SetBool("chase",true);
    
    }
    public override void LogicUpdate()
    {
        //丢失目标时间太长了就不追击了
        if (currentEnemy.lostTimeCounter <= 0)
            currentEnemy.SwitchState(NPCState.Patrol);

        //新的目标点要变成player，由于现在那个位置是人物脚底的位置，所以抬高1.5就比较真实,攻击人物头顶
        target =  new Vector3(currentEnemy.attacker.position.x , currentEnemy.attacker.position.y+1.5f,0);

        //判断攻击距离:如果x,y都小于attack.cs里面设置的攻击范围，就可以执行攻击
        if(Mathf.Abs(target.x - currentEnemy.transform.position.x)<=attack.attackRange && Mathf.Abs(target.y - currentEnemy.transform.position.y) <= attack.attackRange)
        {
            //攻击逻辑
            //1.停下来
            isAttack = true;
            //如果受伤了才执行这行代码
            if(currentEnemy.isHurt)
                currentEnemy.rb.velocity = Vector2.zero;

            //计时器
            attackRateCounter -= Time.deltaTime;
            if (attackRateCounter <= 0)
            {
                //一旦小于0就采取攻击
                currentEnemy.anim.SetTrigger("attack");
                attackRateCounter = attack.attackRate;//重置时间

            }



        }
        else//超出
        {
            isAttack=false;
        }

            moveDir = (target - currentEnemy.transform.position).normalized;

        if (moveDir.x > 0)
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);//我怀疑这里要改
        if (moveDir.x < 0)
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);

    }



    public override void PhysicsUpdate()
    {
        //移动逻辑
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !isAttack)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }



    }
    public override void OnExit()
    {
        //currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        //currentEnemy.anim.SetBool("run", false);
        Debug.Log("离开蝙蝠追踪状态");
        currentEnemy.anim.SetBool("chase", false);
    }
}

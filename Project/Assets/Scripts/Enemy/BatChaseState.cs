using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;

    public override void OnEnter(Enemy enmey)
    {
        currentEnemy = enmey;
        //Debug.Log("切换到Chase");
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;//切换逻辑
        //currentEnemy.anim.SetBool("run", true);//播放动画
        attack = enmey.GetComponent<Attack>();
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
            currentEnemy.rb.velocity = Vector2.zero;

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
        if (!currentEnemy.isHurt && !currentEnemy.isDead)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
        }



    }
    public override void OnExit()
    {
        //currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        //currentEnemy.anim.SetBool("run", false);
    }
}

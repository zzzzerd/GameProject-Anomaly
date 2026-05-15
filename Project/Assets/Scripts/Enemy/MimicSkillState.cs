using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicSkillState : BaseState
{
    public override void OnEnter(Enemy enmey)
    {

        currentEnemy = enmey;
        currentEnemy.currentSpeed = 0;
        //播放动画:进入这个状态就变成箱子
        currentEnemy.anim.SetBool("walk", false);
        currentEnemy.anim.SetBool("hide", true);
        currentEnemy.anim.SetTrigger("skill");

        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
            
    }    
    
    
    public override void LogicUpdate()
    {
        //如果人不在了就切换
        if (currentEnemy.lostTimeCounter <= 0)

        {
            Debug.Log("人不在了");
            currentEnemy.SwitchState(NPCState.Patrol);
        }
    }

    public override void PhysicsUpdate()
    {
       
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("hide", false);
        //currentEnemy.anim.SetBool("walk", true);
    }


}

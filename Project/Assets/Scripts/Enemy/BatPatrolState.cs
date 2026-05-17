using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatPatrolState : BaseState
{

    private Vector3 target;
    private Vector3 moveDir;


     public override void OnEnter(Enemy enmey)
     {
        currentEnemy = enmey;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        target = enmey.GetNewPoint();//获得一个新的点
     }   
    
    public override void LogicUpdate()
    {

        //如果发现player
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }
        
        //是否到达指定position，根据和目标坐标的差值来判断
        if (Mathf.Abs(target.x - currentEnemy.transform.position.x)<0.1f && Mathf.Abs(target.y - currentEnemy.transform.position.y) < 0.1f)
        {
            //到达目标位置，便进入等待时间
            currentEnemy.wait = true;
            //然后获取新的随机目标
            target = currentEnemy.GetNewPoint();
        }


        moveDir = (target -  currentEnemy.transform.position).normalized;

        if(moveDir.x > 0)
            currentEnemy.transform.localScale = new Vector3(1,1,1);//我怀疑这里要改
        if (moveDir.x < 0)
            currentEnemy.transform.localScale = new Vector3(-1,1,1);
    }

    public override void PhysicsUpdate()
    {
        if(!currentEnemy.wait && !currentEnemy.isHurt && !currentEnemy.isDead)
        {
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed* Time.deltaTime;
        }
        else
        {
            currentEnemy.rb.velocity = Vector2.zero;
        }
    }

    public override void OnExit()
    {
        //throw new System.NotImplementedException();
    }


}

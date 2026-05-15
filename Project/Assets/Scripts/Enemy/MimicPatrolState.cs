using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicPatrolState : BaseState
{


    public override void OnEnter(Enemy enmey)
    {
        //throw new System.NotImplementedException();
        currentEnemy = enmey;//来自父类的，也就是当前的这个物体
        //Debug.Log("physicsCheck = " + currentEnemy.physicsCheck);
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;

    }
    public override void LogicUpdate()
    {
        //如果发现player就切换状态
        if (currentEnemy.FoundPlayer())
        {
            //调用这个方法就是换state
            currentEnemy.SwitchState(NPCState.Skill);
        }

        //是不是撞墙,是的就停下并变换方向
        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0))
        {
            //transform.localScale = new Vector3(1, 1, 1); // 朝右
            if (!currentEnemy.physicsCheck.isGround)
            {
                Debug.Log("检测不是地面");
            }
            else if (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0)
            {
                Debug.Log("撞墙左边");

            }
            else
            {
                Debug.Log("撞墙右边边");

            }
            currentEnemy.wait = true; //控制这个变量
            currentEnemy.anim.SetBool("walk", false);

        }
        else
        {
            currentEnemy.anim.SetBool("walk", true);
        }

    }
       public override void PhysicsUpdate()
    {
       
    } 
    
    public override void OnExit()
    {
       
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这是巡逻的状态
/// </summary>
public class SkeletonPatrolState : BaseState
{

    public override void OnEnter(Enemy enmey)
    {
        //throw new System.NotImplementedException();
        currentEnemy = enmey;//来自父类的，也就是当前的这个物体
        Debug.Log("physicsCheck = " + currentEnemy.physicsCheck);
    }



    public override void LogicUpdate()
    {
        //如果碰到墙
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
        //巡逻结束就结束走路状态
        currentEnemy.anim.SetBool("walk", false);
    }
}

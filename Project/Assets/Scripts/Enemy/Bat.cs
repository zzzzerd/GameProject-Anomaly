using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

//继承enmey类 
public class Bat : Enemy
{
    [Header("移动范围")]
    public float patrolRadius;

    [Header("空中死亡状态")]
    private bool airDeath;

    //重写但是不覆盖父类
    protected override void Awake()
    {
        base.Awake();
        ////就是先创建一下这两个状态
        patrolState = new BatPatrolState();
        chaseState = new BatChaseState();
        //skillState = new MimicSkillState();
    }




    //public override void OnDie()
    //{
    //    if (isDead) return;

    //    isDead = true;

    //    if (!physicsCheck.isGround)
    //    {
    //        airDeath = true;
    //        anim.SetBool("dead", true);
    //        anim.SetBool("isGround", false);
    //    }
    //    else
    //    {
    //        anim.SetBool("dead", true);
    //        anim.SetBool("isGround", true);
    //    }
    //}

    public override void OnDie()
    {
        if (isDead) return;

        isDead = true;
        //currentState = null;   // ⭐关键：停止状态机
        //rb.velocity = Vector2.zero;

        if (!physicsCheck.isGround)
        {
            airDeath = true;
            anim.SetBool("dead", true);
            anim.SetTrigger("FallTrigger");   // ⭐关键
        }
        else
        {
            anim.SetBool("dead", true);
            anim.SetBool("isGround", true);
        }
    }

    //// 子类重写
    //protected override void Update()
    //{
    //    // 1. 先执行父类的 Update 逻辑（必须写！）
    //    base.Update();

    //    // 2. 再追加你自己的逻辑
    //    if (isDead && airDeath && physicsCheck.isGround)
    //    {
    //        airDeath = false;
    //        anim.SetBool("isGround", true);
    //    }
    //}

    protected override void Update()
    {
        base.Update();

        if (isDead && airDeath && physicsCheck.isGround)
        {
            airDeath = false;

            anim.SetBool("isGround", true);
            anim.SetBool("dead", true); // 或触发 Death
        }
    }


    /// <summary>
    /// 重写找到player的方法
    /// </summary>
    /// <returns></returns>
    public override bool FoundPlayer()
    {
        //这个是关于攻击的
        var obj = Physics2D.OverlapCircle(transform.position,checkDistance,attackLayer);//检测的原点，距离，以及图层
        if (obj)
        {
            attacker = obj.transform;
        }

        return obj;
    }

   /// <summary>
   /// 这个是移动的点随机生成的
   /// </summary>
   /// <returns></returns>
    public override Vector3 GetNewPoint()
    {
        //半径范围里面的随机点
        var targatX = Random.Range(-patrolRadius,patrolRadius);
        var targatY = Random.Range(-patrolRadius,patrolRadius);

        return spwanPoint + new Vector3(targatX, targatY);//生成点之上的随机范围的点

    }

    public override void Move()
    {
        //base.Move();
    }

    public override void OnDrawGizmosSelected()
    {
        //白色是随机检测
        Gizmos.DrawWireSphere(transform.position,checkDistance);//中心点+边境
        //绿色的是随机移动的范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,patrolRadius);//中心点+边境
    }

}

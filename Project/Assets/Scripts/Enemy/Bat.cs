using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

//继承enmey类 
public class Bat : Enemy
{
    [Header("移动范围")]
    public float patrolRadius;

    //重写但是不覆盖父类
    protected override void Awake()
    {
        base.Awake();
        ////就是先创建一下这两个状态
        patrolState = new BatPatrolState();
        //skillState = new MimicSkillState();
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

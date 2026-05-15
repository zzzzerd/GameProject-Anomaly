using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//继承enmey类 
public class Mimic : Enemy
{

    //private void Start()
    //{
    //    canMove = false;
    //}

    //重写但是不覆盖父类
    protected override void Awake()
    {
        base.Awake();
        //就是先创建一下这两个状态
        patrolState = new MimicPatrolState();
        skillState = new MimicSkillState();
    }
}

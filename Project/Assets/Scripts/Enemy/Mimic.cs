using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//继承enmey类 
public class Mimic : Enemy
{

    private void Start()
    {
        canMove = false;
    }

    //重写但是不覆盖父类
    public override void Move()
    {
        base.Move();
        anim.SetBool("walk", true);
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

////ºÃ≥–enmey¿‡ 
//public class Skeleton : Enemy
//{
//    public override void Move()
//    {
//        base.Move();
//        anim.SetBool("walk", true);
//    }
//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ºÃ≥–enmey¿‡ 
public class Skeleton : Enemy
{

    protected override void Awake()
    {
        base.Awake();
        patrolState = new SkeletonPatrolState();
        chaseState = new SkeletonChaseState();
    }
}
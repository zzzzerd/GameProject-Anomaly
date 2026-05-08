using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ºÃ≥–enmey¿‡ 
public class Skeleton : Enemy
{
    public override void Move()
    {
        base.Move();
        anim.SetBool("walk", true);
    }
}

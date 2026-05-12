using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //获取敌人身上的组件的准备
    protected Rigidbody2D rb;
    protected Animator anim;
    PhysicsCheck physicsCheck;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    public float currentSpeed;
    [Header("状态")]
    public bool canMove = true;
    //面朝的方向
    public Vector3 faceDir;

    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        currentSpeed = normalSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {   //获取面朝方向
        faceDir = new Vector3(transform.localScale.x, 0, 0);//夹负数会让会让 Sprite左右翻转
        //Debug.Log(transform.localScale.x);
        
        //如果碰到墙
        if ((physicsCheck.touchLeftWall&&faceDir.x<0) || (physicsCheck.touchRightWall&& faceDir.x>0))
        {
            //transform.localScale = new Vector3(1, 1, 1); // 朝右
            anim.SetBool("walk", false);
            wait = true;
        }
        //else if (physicsCheck.touchRightWall)
        //{
        //    //transform.localScale = new Vector3(-1, 1, 1); // 朝左
        //    wait = true;
        //}

        TimeCounter();



    }


    //固定时间执行，和物理系统同步
    private void FixedUpdate()
    {
        //如果模拟怪物是箱子就先不动
        if (canMove)
        {
            Move();
        }
    }

    //可以被子类重写
    public virtual void Move()
    {
        //移动：让敌人产生位移
        rb.velocity = new Vector2( currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(-faceDir.x, 1, 1);
            }
        }
    }
}

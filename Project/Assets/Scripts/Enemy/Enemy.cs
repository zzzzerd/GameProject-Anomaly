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
    public float hurtForce;
    public Vector3 faceDir;   //面朝的方向

    [Header("状态")]
    public bool canMove = true;
    public bool isHurt;
    public bool isDead;

   


    public Transform attacker;


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

        TimeCounter();



    }


    //固定时间执行，和物理系统同步
    private void FixedUpdate()
    {
        //如果模拟怪物是箱子就先不动
        if (canMove)
        {
            if (!isHurt & !isDead)//受伤就不可以执行这个代码了
            {
                Move();
            }
        }
    }

    //可以被子类重写
    public virtual void Move()
    {
        //移动：让敌人产生位移
        rb.velocity = new Vector2( currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    /// <summary>
    /// 计时器
    /// </summary>
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

    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        //转身
        if (attackTrans.position.x - transform.position.x > 0)//站在右侧
        {
            //Debug.Log("转身1");
            transform.localScale = new Vector3(1, 1, 1);
        }
        if ( attackTrans.position.x - transform.position.x < 0 )//站在左边
        {
            //Debug.Log("转身2");
            transform.localScale = new Vector3( -1, 1, 1);
        }

        //受伤被击退
        isHurt = true;
        anim.SetTrigger("hurt");//播放受伤的动画
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;

        StartCoroutine(OnHurt(dir));
    }

    //协成，按照一定的顺序逐一执行，还可以等待,返回一个迭代器
    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);//等待一忽儿
        isHurt = false;
    }

    public void OnDie()
    {
        //播放死亡动画
        anim.SetBool("dead", true);
        //改变状态
        isDead = true;//已经死了
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void DestroyAfterAnimation()
    {
        //销毁当前物体
        Destroy(this.gameObject);
    }
}

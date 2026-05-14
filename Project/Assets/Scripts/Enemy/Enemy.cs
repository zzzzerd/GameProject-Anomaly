//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Enemy : MonoBehaviour
//{
//    //获取敌人身上的组件的准备
//    protected Rigidbody2D rb;
//    protected Animator anim;
//    PhysicsCheck physicsCheck;

//    [Header("基本参数")]
//    public float normalSpeed;
//    public float chaseSpeed;
//    public float currentSpeed;
//    public float hurtForce;
//    public Vector3 faceDir;   //面朝的方向

//    [Header("状态")]
//    public bool canMove = true;
//    public bool isHurt;
//    public bool isDead;




//    public Transform attacker;


//    [Header("计时器")]
//    public float waitTime;
//    public float waitTimeCounter;
//    public bool wait;



//    private void Awake()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        anim = GetComponent<Animator>();
//        physicsCheck = GetComponent<PhysicsCheck>();
//        currentSpeed = normalSpeed;

//    }

//    // Update is called once per frame
//    void Update()
//    {   //获取面朝方向
//        faceDir = new Vector3(transform.localScale.x, 0, 0);//夹负数会让会让 Sprite左右翻转
//        //Debug.Log(transform.localScale.x);

//        //如果碰到墙
//        if ((physicsCheck.touchLeftWall&&faceDir.x<0) || (physicsCheck.touchRightWall&& faceDir.x>0))
//        {
//            //transform.localScale = new Vector3(1, 1, 1); // 朝右
//            anim.SetBool("walk", false);
//            wait = true;
//        }

//        TimeCounter();



//    }


//    //固定时间执行，和物理系统同步
//    private void FixedUpdate()
//    {
//        //如果模拟怪物是箱子就先不动
//        if (canMove)
//        {
//            if (!isHurt & !isDead)//受伤就不可以执行这个代码了
//            {
//                Move();
//            }
//        }
//    }

//    //可以被子类重写
//    public virtual void Move()
//    {
//        //移动：让敌人产生位移
//        rb.velocity = new Vector2( currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
//    }

//    /// <summary>
//    /// 计时器
//    /// </summary>
//    public void TimeCounter()
//    {
//        if (wait)
//        {
//            waitTimeCounter -= Time.deltaTime;
//            if (waitTimeCounter <= 0)
//            {
//                wait = false;
//                waitTimeCounter = waitTime;
//                transform.localScale = new Vector3(-faceDir.x, 1, 1);
//            }
//        }
//    }

//    public void OnTakeDamage(Transform attackTrans)
//    {
//        attacker = attackTrans;
//        //转身
//        if (attackTrans.position.x - transform.position.x > 0)//站在右侧
//        {
//            //Debug.Log("转身1");
//            transform.localScale = new Vector3(1, 1, 1);
//        }
//        if ( attackTrans.position.x - transform.position.x < 0 )//站在左边
//        {
//            //Debug.Log("转身2");
//            transform.localScale = new Vector3( -1, 1, 1);
//        }

//        //受伤被击退
//        isHurt = true;
//        anim.SetTrigger("hurt");//播放受伤的动画
//        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;

//        StartCoroutine(OnHurt(dir));
//    }

//    //协成，按照一定的顺序逐一执行，还可以等待,返回一个迭代器
//    private IEnumerator OnHurt(Vector2 dir)
//    {
//        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
//        yield return new WaitForSeconds(0.45f);//等待一忽儿
//        isHurt = false;
//    }

//    public void OnDie()
//    {
//        //播放死亡动画
//        anim.SetBool("dead", true);
//        //改变状态
//        isDead = true;//已经死了
//    }

//    /// <summary>
//    /// 销毁
//    /// </summary>
//    public void DestroyAfterAnimation()
//    {
//        //销毁当前物体
//        Destroy(this.gameObject);
//    }
//}


/////今天上午错误版本：


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Enemy : MonoBehaviour
{
    //获取敌人身上的组件的准备
    [HideInInspector] protected Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck;
    public Transform attacker;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    public float hurtForce;
    public Vector3 faceDir;   //面朝的方向

    [Header("状态")]
    public bool canMove = true; //暂时用不上
    public bool isHurt;//被打了
    public bool isDead;//已死

    protected BaseState currentState;//当前状态
    protected BaseState patrolState; //巡逻状态
    protected BaseState chaseState; //追赶状态


    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;//这个也是状态实则，如果wait只要设置wait的true，就会停下来两秒然后转身



    protected virtual void Awake()
    {
        //Debug.Log($"[Awake] {name} ID={GetInstanceID()}");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();

        currentSpeed = normalSpeed;
        waitTimeCounter = waitTime;

    }

    /// <summary>
    /// 物体被激活的时候
    /// </summary>
    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);//把当前这个类型传入

        ////测试
        //Debug.Log("currentState = " + currentState);
        //Debug.Log("patrolState = " + patrolState);

        //if (patrolState == null)
        //{
        //    Debug.LogError("在OnEnable里面 patrolState is NULL");
        //}
        //else
        //{
        //    Debug.Log("在OnEnable里面patrolState不是空的，patrolState = " + patrolState);
        //}

        //if (currentState == null)
        //{
        //    Debug.LogError("在OnEnable里面 currentState is NULL");
        //}
        //else
        //{
        //    Debug.Log("在OnEnable里面currentState不是空的，currentState = " + currentState);
        //}


    }

    // Update is called once per frame,实时获取
    void Update()
    {   //获取面朝方向
        //Debug.Log($"[Update] {name} ID={GetInstanceID()}");
        faceDir = new Vector3(transform.localScale.x, 0, 0);//因为只需要x的值(左右方向),然后在update里面会实时更新


        ////测试：
        //if (patrolState == null)
        //    Debug.LogError("patrolState is NULL");

        //if (currentState == null)
        //    Debug.LogError("currentState is NULL");

        //if (physicsCheck == null)
        //    Debug.LogError("physicsCheck is NULL");

        currentState?.LogicUpdate();
        TimeCounter();//每一帧都会调用，但是只有wai==true的时候才会进行内部倒计时的逻辑



    }


    //固定时间执行，和物理系统同步
    private void FixedUpdate()
    {
        //如果模拟怪物是箱子就先不动
        if (canMove)
        {
            if (!isHurt && !isDead && !wait)//受伤就不可以执行这个代码了
            {
                Move();
            }
        }

        //状态机
        currentState.PhysicsUpdate();
    }

    /// <summary>
    /// 退出执行，人物消失执行
    /// </summary>
    private void OnDisable()
    {
        //Debug.Log("Enemy OnDisable");
        currentState.OnExit();
    }

    //移动的行为函数，不加别的逻辑判断，单纯移动
    public virtual void Move()
    {
        //移动：让敌人产生位移
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }



    /// <summary>
    /// 计时器
    /// </summary>
    public void TimeCounter()
    {
        if (wait)//wait==true的时候
        {
            //Debug.Log("开始倒计时");
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)//倒计时结束了
            {
                wait = false;//修改状态(在update那里可以调用move函数了)，这里会改回状态
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(-faceDir.x, 1, 1);//转身
            }
        }
    }

    /// <summary>
    /// 被攻击的行为
    /// </summary>
    /// <param name="attackTrans"></param>
    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;

        //受伤就转身
        if (attackTrans.position.x - transform.position.x > 0)//站在右侧
        {
            Debug.Log("受伤转身1");
            transform.localScale = new Vector3(1, 1, 1);//就朝着右边
        }
        if (attackTrans.position.x - transform.position.x < 0)//站在左边
        {
            Debug.Log("受伤转身2");
            transform.localScale = new Vector3(-1, 1, 1);//朝向左边
        }

        //受伤被击退
        isHurt = true;//影响update里面的move
        anim.SetTrigger("hurt");//播放受伤的动画,由于是trigger，所以打一拳就触发一次

        //被击飞
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        StartCoroutine(OnHurt(dir));
    }

    //协成，按照一定的顺序逐一执行，还可以等待,返回一个迭代器
    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);//受伤被击退
        yield return new WaitForSeconds(0.45f);//等待一忽儿
        isHurt = false;//可以恢复行动了
    }

    public void OnDie()
    {
        //把这个敌人的图层改成不和player发生碰撞的图层
        gameObject.layer = 2; //inspector里面去看具体layer和对应的编号
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

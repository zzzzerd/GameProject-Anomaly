using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator),typeof(PhysicsCheck))]//一定需要的组件

public class Enemy : MonoBehaviour
{
    //获取敌人身上的组件的准备
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck;
    public Transform attacker;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    public float hurtForce;
    public Vector3 faceDir;   //面朝的方向
    public Vector3 spwanPoint;

    [Header("检测")]
    public Vector2 centerOffet;
    public Vector2 checkSize;//盒子大小
    public float checkDistance;//检测距离
    public LayerMask attackLayer;//要检测的人属于的图层？？


    [Header("检测-追踪时间")]
    public float lostTime;
    public float lostTimeCounter;


    [Header("状态")]
    public bool canMove = true; //暂时用不上
    public bool isHurt;//被打了
    public bool isDead;//已死

    protected BaseState currentState;//当前状态
    protected BaseState patrolState; //巡逻状态
    protected BaseState chaseState; //追赶状态
    protected BaseState skillState; //特殊技能状态


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
        spwanPoint =  transform.position;

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
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("MimicTransformToBox")&& !anim.GetCurrentAnimatorStateInfo(0).IsName("MimicTransformToMonster"))
            //移动：让敌人产生位移
            rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }


    /// <summary>
    /// 留给飞行动物的获取新的position目标点的函数
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 GetNewPoint()
    {

        return transform.position;
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

        if (!FoundPlayer()&& lostTimeCounter>0)
        {
            lostTimeCounter-=Time.deltaTime;
        }
        //else//如果在这个过程中又发现了敌人
        //{
        //    lostTimeCounter = lostTime;
        //}
    }

    /// <summary>
    /// 发射一个东西，然后看看能不能找到对应物体，返回bool
    /// </summary>
    /// <returns></returns>
    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffet,checkSize,0,faceDir,checkDistance,attackLayer);
    }


    /// <summary>
    /// 状态切换
    /// </summary>
    /// <param name="state"></param>
    public void SwitchState(NPCState state)
    {
        
        var newState = state switch //检查枚举类型state的value
        {
            NPCState.Patrol => patrolState,//如果是patrol就返回patrolState
            NPCState.Chase => chaseState,//同理
            NPCState.Skill => skillState,
            _ => null
        };

        currentState.OnExit();//先退出上一个
        currentState = newState;//换成新的状态
        currentState.OnEnter(this);//进入新状态

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
        rb.velocity = new Vector2(0,rb.velocity.y);
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

    public virtual void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(transform.position+(Vector3)centerOffet+ new Vector3(checkDistance*faceDir.x,0),0.2f);
        Gizmos.DrawWireSphere(transform.position+(Vector3)centerOffet+ new Vector3( checkDistance * transform.localScale.x, 0 ),0.2f);
    }
}

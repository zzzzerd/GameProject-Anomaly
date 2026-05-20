using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;

    //Rigidbody组件，后面可以直接拖动获得引用
    private Rigidbody2D rb;

    private PlayerAnimation playerAnimation;

    [Header("基本参数")]
    //左右移动-速度
    public float speed;

    //跳跃-力
    public float jumpForce;
    public float wallJumpForce;//登墙力

    //my own script
    private PhysicsCheck physicsCheck;

    private float runSpeed;
    private float walkSpeed => speed / 2.5f;

    //public int combo;


    //获取碰撞体组件
    private CapsuleCollider2D coll;
    //原始尺寸
    private Vector2 originalSize;
    private Vector2 originalOffset;


    //受伤被弹开
    public float hurtForce;



    [Header("状态")]
    public bool isDead;    //角色死亡
    public bool isAttack;
    public bool isHurt;
    public bool isCrouch;    //下蹲
    public bool wallJump;


    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();

        //获取组件面板上这两个参数（下蹲）
        originalOffset = coll.offset;
        originalSize = coll.size;


        //跳跃：把这个函数添加到按钮按下的一刻执行
        inputControl.GamePlay.Jump.started += Jump;

        #region 强制走路
        runSpeed = speed;//在这里设置，不会因为speed被更改而更改

        //按下按键就是走路
        inputControl.GamePlay.Walk.performed +=  ctx =>
        {
            if (physicsCheck.isGround)
                speed = walkSpeed;
        };

        inputControl.GamePlay.Walk.canceled +=  ctx =>
        {
            if (physicsCheck.isGround)
                speed = runSpeed;
        };
        #endregion

        //攻击
        inputControl.GamePlay.Attack.started += PlayerAttack;
    }


    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }


    //只有下面这两个update函数才会一直执行在代码中
    private void Update()
    {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
        CheckState();
    }

    private void FixedUpdate()
    {
        if(!isHurt && !isAttack)
            Move();
        
    }

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    //Debug.Log(other.name);
    //}

    public void Move()
    {
        //主要的移动方法
        if(!isCrouch && !wallJump) //不是下蹲才可以移动
         rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        
        //初始值
        int faceDir = (int)transform.localScale.x;

        if(inputDirection.x >0)
            faceDir = 1;
        if (inputDirection.x < 0)
            faceDir = -1;

        //人物翻转
        transform.localScale = new Vector3(faceDir, 1, 1);

        //下蹲
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        //保证下蹲的时候能胶囊碰撞体也会变小
        if (isCrouch)
        {
            //修改碰撞体大小
            coll.offset = new Vector2(-0.05f,0.85f);
            coll.size = new Vector2(0.7f,1.7f);

        }
        else
        {
            //还原之前的大小
            coll.size = originalSize;
            coll.offset = originalOffset;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2.1f) * wallJumpForce, ForceMode2D.Impulse);//这里改蹬墙跳的纵向高度
            wallJump = true;//蹬墙跳状态
        }

    }

    //攻击函数
    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        playerAnimation.PlayAttack();
        isAttack  = true;
        //combo++;
        //if (combo >= 3)
        //    combo = 0;
    }



    //这个可以绑定到事件上面去，人物受伤弹开
    public void GetHurt(Transform attacker)
    {
        isHurt = true; //防止人物进行其他移动
        rb.velocity = Vector2.zero;
        //获得单位方向
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x),0).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);

    }

    public void PlayerDead()
    {
        isDead = true;
        //把这些游戏操作都关闭
        inputControl.GamePlay.Disable();
    }

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;


        if (physicsCheck.onWall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);


        if(wallJump&& rb.velocity.y < 0f)
        {
            wallJump = false;
        }
       }
     
    
}
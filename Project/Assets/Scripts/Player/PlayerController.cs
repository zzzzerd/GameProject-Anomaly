using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("监听")]
    public SceneLoadEventSO loadEventSO; 
    public VoidEventSO afterSceneLoad;
    public VoidEventSO newGameEventSO;




    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    public Character character;

    //Rigidbody组件，后面可以直接拖动获得引用
    private Rigidbody2D rb;

    private PlayerAnimation playerAnimation;

    [Header("基本参数")]
    //左右移动-速度
    public float speed;

    //跳跃-力
    public float jumpForce;
    public float wallJumpForce;//登墙力
    public float slideDistance;     //滑铲距离
    public float slideSpeed;    //滑铲速度
    public int slidePowerCost;

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
    private PlayerCampfireReceiver campfireReceiver;


    //受伤被弹开
    public float hurtForce;



    [Header("状态")]
    public bool isDead;    //角色死亡
    public bool isAttack;
    public bool isHurt;
    public bool isCrouch;    //下蹲
    public bool wallJump;
    public bool isSlide;
    public bool isFire;    //烧火


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
        character = GetComponent<Character>();
        campfireReceiver = GetComponent<PlayerCampfireReceiver>();

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

        //滑铲
        inputControl.GamePlay.Slide.started += Slide;
    }


    private void OnEnable()
    {
        inputControl.Enable();
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        afterSceneLoad.OnEventRaised += OnAfterSceneLoad;
        //if (newGameEventSO != null)
        //    newGameEventSO.OnEventRaised += OnNewGame;
    }
    
    private void OnDisable()
    {
        inputControl.Disable();
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        afterSceneLoad.OnEventRaised -= OnAfterSceneLoad;
        //if (newGameEventSO != null)
        //    newGameEventSO.OnEventRaised -= OnNewGame;
    }

    private void OnAfterSceneLoad()
    {
        if (isDead)
        {
            Debug.Log("场景加载完成，执行复活");
            ReviveAfterLoad();
        }

        // 切场景后清除残留状态，防止受伤/烧火动画被打断后状态卡住
        isHurt = false;
        isFire = false;
        ColorUtility.TryParseHtmlString("#BCFAFF", out Color defaultColor);
        GetComponent<SpriteRenderer>().color = defaultColor;
        var anim = GetComponent<Animator>();
        if (anim != null) anim.ResetTrigger("hurt");
    }

    //private void OnNewGame()
    //{
    //    ReviveAfterLoad();
    //}

    private void OnLoadRequestEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        // 场景切换时禁用输入，防止玩家在加载过程中移动
        // OnDisable 会自动调用 inputControl.Disable()
    }


    //只有下面这两个update函数才会一直执行在代码中
    private void Update()
    {
        if (isDead)
        {
            inputDirection = Vector2.zero;
            CheckState();
            return;
        }

        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();
        CheckState();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        if (!isHurt && !isFire)
            Move();
    }

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    //Debug.Log(other.name);
    //}

    public void Move()
    {
        //主要的移动方法
        if (!wallJump) //蹬墙跳时不能移动
        {
            float currentSpeed = isCrouch ? speed * 0.5f : speed; // 下蹲时速度减半
            rb.velocity = new Vector2(inputDirection.x * currentSpeed * Time.deltaTime, rb.velocity.y);
        }
        
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

            //打断这个滑铲的协程
            isSlide = false;
            StopAllCoroutines();
        }
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2.1f) * wallJumpForce, ForceMode2D.Impulse);//这里改蹬墙跳的纵向高度
            wallJump = true;//蹬墙跳状态
        }

    }

    private void Slide(InputAction.CallbackContext obj)
    {
        //不能滑铲的场景: 在空中不能滑铲
        if (!isSlide && physicsCheck.isGround && character.currentPower >=slidePowerCost)
        {
            isSlide = true;

            var targetPos = new Vector3(transform.position.x+slideDistance * transform.localScale.x, transform.position.y);
            //slide的时候把玩家的layer改成Enemy
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            //打开协成：
            StartCoroutine(TriggerSlide(targetPos));
            character.OnSlide(slidePowerCost);
            
        }
    }



    /// <summary>
    /// 这个就是有bug
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {

            Debug.Log(
                "isGround: " + physicsCheck.isGround +
                " | leftWall: " + physicsCheck.touchLeftWall +
                " | rightWall: " + physicsCheck.touchRightWall +
                " | onWall: " + physicsCheck.onWall +
                " | posX: " + transform.position.x +
                " | velocity: " + rb.velocity
            );

            if (!physicsCheck.isGround)
            {
                Debug.Log("协成结束");
                Debug.Log("离开地面");
                break;
            }

            if (physicsCheck.touchLeftWall && transform.localScale.x<0f|| physicsCheck.touchRightWall && transform.localScale.x > 0f)
            {
                Debug.Log("撞墙停止");

                isSlide = false;
                break;
            }
            yield return null;

            //第一个版本，不可以会穿墙
            //rb.MovePosition(
            //    new Vector2(
            //        transform.position.x + transform.localScale.x * slideSpeed,
            //        transform.position.y
            //    )
            //);

            //不会穿墙的版本
            rb.MovePosition(rb.position + new Vector2(transform.localScale.x * slideSpeed * Time.fixedDeltaTime, 0));

        } while (MathF.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
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
        rb.velocity = Vector2.zero;
        //把这些游戏操作都关闭
        inputControl.GamePlay.Disable();
    }

    public void ReviveAfterLoad()
    {
        Debug.Log("爸爸妈妈我复活了");
        isDead = false;
        isHurt = false;
        isAttack = false;
        isFire = false;
        isSlide = false;
        wallJump = false;
        character.currentHealth = character.maxHealth;
        character.currentPower = character.maxPower;
        rb.velocity = Vector2.zero;
        gameObject.layer = LayerMask.NameToLayer("Player");
        inputControl.GamePlay.Enable();
    }

    private void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;


        if (physicsCheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
            // 贴墙时朝向墙壁方向（左墙朝左，右墙朝右）
            if (physicsCheck.touchLeftWall)
                transform.localScale = new Vector3(1, 1, 1);
            else if (physicsCheck.touchRightWall)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);


        if(wallJump&& rb.velocity.y < 0f)
        {
            wallJump = false;
        }
       }
     
    
}
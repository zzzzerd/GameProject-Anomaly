using System.Collections;
using UnityEngine;

/// <summary>
/// Boss 敌人：漂浮型，拥有完整的 Idle/Chase/Attack/Hurt/Dead 状态
/// 需要挂载：Rigidbody2D, Animator, PhysicsCheck, Attack, Character
/// </summary>
//[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck), typeof(Character))]


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PhysicsCheck))]
[RequireComponent(typeof(Character))]

public class BossEnemy : Enemy
{
    [Header("Boss 漂浮参数")]
    public float floatAmplitude = 0.3f;   // 上下浮动幅度
    public float floatFrequency = 1.5f;   // 浮动频率
    private float floatOffset;            // 初始 y 偏移（让浮动看起来自然）

    private Character bossCharacter;      // Boss 自己的血量组件

    // Boss 状态
    private BaseState bossIdleState;
    private BaseState bossChaseState;
    private BaseState bossAttackState;

    [Header("Boss 攻击技能")]
    public GameObject projectilePrefab;   // 弹幕预制体（可选）
    public float projectileSpeed = 5f;

    protected override void Awake()
    {
        base.Awake();

        // 创建 Boss 专用状态
        bossIdleState = new BossIdleState();
        bossChaseState = new BossChaseState();
        bossAttackState = new BossAttackState();

        // 映射到父类的状态槽位（复用 SwitchState）
        patrolState = bossIdleState;
        chaseState = bossChaseState;
        skillState = bossAttackState;

        // 获取血量组件
        bossCharacter = GetComponent<Character>();

        // 记录初始 y 做浮动偏移
        floatOffset = Random.Range(0f, Mathf.PI * 2f);

        // 禁用重力（Boss 漂浮）
        rb.gravityScale = 0;
    }

    private void OnEnable()
    {
        // 初始进入 Idle 状态
        currentState = bossIdleState;
        currentState.OnEnter(this);

        // 订阅 Character 的受伤和死亡事件
        if (bossCharacter != null)
        {
            bossCharacter.OnTakeDamage.AddListener(OnBossTakeDamage);
            bossCharacter.OnDie.AddListener(OnBossDie);
        }
    }

    private void OnDisable()
    {
        if (bossCharacter != null)
        {
            bossCharacter.OnTakeDamage.RemoveListener(OnBossTakeDamage);
            bossCharacter.OnDie.RemoveListener(OnBossDie);
        }
        currentState?.OnExit();
    }

    protected override void Update()
    {
        base.Update();

        // Boss 漂浮效果
        if (!isDead)
        {
            FloatEffect();
        }
    }

    /// <summary>
    /// 上下漂浮效果
    /// </summary>
    private void FloatEffect()
    {
        float newY = spwanPoint.y + Mathf.Sin(Time.time * floatFrequency + floatOffset) * floatAmplitude;
        // 只在非物理移动时微调 y（避免和 velocity 冲突）
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, newY, Time.deltaTime * 2f);
        transform.position = pos;
    }

    // ==================== 受伤 ====================

    private void OnBossTakeDamage(Transform attackerTrans)
    {
        if (isDead) return;

        // 记录攻击者
        attacker = attackerTrans;

        // 受伤转身
        if (attackerTrans.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        // 播放受伤动画
        isHurt = true;
        anim.SetTrigger("hurt");

        // 击退
        Vector2 dir = new Vector2(transform.position.x - attackerTrans.position.x, 0).normalized;
        rb.velocity = Vector2.zero;
        StartCoroutine(OnHurtCoroutine(dir));
    }

    private IEnumerator OnHurtCoroutine(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;

        // 受伤后回到追逐状态（如果已发现玩家）
        if (!isDead && attacker != null && FoundPlayer())
        {
            SwitchState(NPCState.BossChase);
        }
    }

    // ==================== 死亡 ====================

    private void OnBossDie()
    {
        if (isDead) return;
        OnDie();
    }

    public override void OnDie()
    {
        if (isDead) return;

        isDead = true;
        gameObject.layer = 2;   // IgnoreRaycast
        anim.SetBool("dead", true);
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;    // 死亡后掉落

        // 统计
        GameDataManager.Instance?.AddKilledEnemy();
        Debug.Log("[BossEnemy] Boss 已死亡");
    }

    // ==================== 攻击技能（动画事件调用） ====================

    /// <summary>
    /// 发射弹幕（由攻击动画事件调用）
    /// </summary>
    public void FireProjectile()
    {
        if (projectilePrefab == null || attacker == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
        {
            Vector2 dir = (attacker.position - transform.position).normalized;
            projRb.velocity = dir * projectileSpeed;
        }
    }

    // ==================== 检测玩家 ====================

    public override bool FoundPlayer()
    {
        // 圆形范围检测（适合漂浮 Boss）
        var obj = Physics2D.OverlapCircle(transform.position, checkDistance, attackLayer);
        if (obj != null)
        {
            attacker = obj.transform;
            lostTimeCounter = lostTime;  // 重置丢失计时器
        }
        return obj != null;
    }

    // ==================== Boss 不使用地面移动 ====================

    public override void Move()
    {
        // Boss 移动由各状态的 PhysicsUpdate 控制
    }

    public override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkDistance);
    }
}

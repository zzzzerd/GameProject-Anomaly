using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PhysicsCheck))]
[RequireComponent(typeof(Character))]

public class BossEnemy : Enemy
{
    private Character bossCharacter;
    private bool bossDead;  // 本地死亡标记，防止重复计数

    private BaseState bossIdleState;
    private BaseState bossChaseState;
    private BaseState bossAttackState;

    [Header("广播 - Boss 死亡任务事件（连 TaskManager 的 bossDefeatedEventSO）")]
    public VoidEventSO bossDefeatedEventSO;

    [Header("Boss 攻击技能")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;

    [Header("音效")]
    public AudioDefination deadAudio;  // Boss 死亡音效

    [Header("攻击点")]
    public Collider2D attackCollider;
    public float attackStopDistance = 2f;

    protected override void Awake()
    {
        base.Awake();

        bossIdleState   = new BossIdleState();
        bossChaseState  = new BossChaseState();
        bossAttackState = new BossAttackState();

        patrolState = bossIdleState;
        chaseState  = bossChaseState;
        skillState  = bossAttackState;

        bossCharacter = GetComponent<Character>();
        rb.gravityScale = 0;
    }

    private void OnEnable()
    {
        bossDead = false;

        currentState = bossIdleState;
        currentState.OnEnter(this);

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
    }

    // ==================== 受伤 ====================

    private void OnBossTakeDamage(Transform attackerTrans)
    {
        if (isDead) return;
        if (isHurt) return;

        attacker = attackerTrans;

        if (attackerTrans.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        isHurt = true;
        anim.SetTrigger("hurt");

        Vector2 dir = new Vector2(transform.position.x - attackerTrans.position.x, 0).normalized;
        rb.velocity = Vector2.zero;
        StartCoroutine(OnHurtCoroutine(dir));
    }

    private IEnumerator OnHurtCoroutine(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;

        if (!isDead && attacker != null && FoundPlayer())
            SwitchState(NPCState.BossChase);
    }

    // ==================== 死亡 ====================

    private void OnBossDie()
    {
        OnDie();
    }

    public override void OnDie()
    {
        if (bossDead) return;  // 只执行一次
        bossDead = true;
        isDead = true;

        gameObject.layer = 2;
        anim.SetBool("dead", true);
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;

        deadAudio?.PlayAudioCLip();  // 播放死亡音效
        GameDataManager.Instance?.AddKilledBoss();
        bossDefeatedEventSO?.RaiseEvent();  // 通知 TaskManager
        Debug.Log("[BossEnemy] Boss 已死亡");
    }

    // ==================== 攻击碰撞体开关（动画事件调用） ====================

    public void EnableAttackCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = true;
    }

    public void DisableAttackCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    // ==================== 检测玩家 ====================

    public override bool FoundPlayer()
    {
        var obj = Physics2D.OverlapCircle(transform.position, checkDistance, attackLayer);
        if (obj != null)
        {
            attacker = obj.transform;
            lostTimeCounter = lostTime;
        }
        return obj != null;
    }

    public override void Move() { }

    public override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkDistance);
    }
}

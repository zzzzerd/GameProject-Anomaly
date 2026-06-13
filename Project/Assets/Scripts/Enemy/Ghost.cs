using System.Collections;
using UnityEngine;

public class Ghost : Enemy
{
    [Header("巡逻范围半径")]
    public float patrolRadius = 3f;

    [Header("音效")]
    public AudioDefination deathAudio;   // 死亡音效
    public AudioDefination ambientAudio; // 靠近时持续音效（挂 AudioSource loop）

    private SpriteRenderer sr;

    protected override void Awake()
    {
        base.Awake();
        patrolState = new GhostPatrolState();
        chaseState = new GhostChaseState();
        sr = GetComponent<SpriteRenderer>();
    }

    // 受伤时闪红（基类会自动触发 hurt trigger 播动画）
    public void OnHurtFlash()
    {
        if (sr != null) StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }

    public override bool FoundPlayer()
    {
        var obj = Physics2D.OverlapCircle(transform.position, checkDistance, attackLayer);
        if (obj) attacker = obj.transform;
        return obj;
    }

    public override Vector3 GetNewPoint()
    {
        var x = Random.Range(-patrolRadius, patrolRadius);
        var y = Random.Range(-patrolRadius, patrolRadius);
        return spwanPoint + new Vector3(x, y);
    }

    // 不用地面移动逻辑
    public override void Move() { }

    public override void OnDie()
    {
        base.OnDie();
        if (deathAudio != null) deathAudio.PlayAudioCLip();
        // 停止环境音
        if (ambientAudio != null)
        {
            var src = ambientAudio.GetComponent<AudioSource>();
            if (src != null) src.Stop();
        }
    }

    // Animation Event：攻击帧调用，开启攻击碰撞体
    public void EnableAttackCollider()
    {
        var col = GetComponentInChildren<Attack>()?.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
    }

    // Animation Event：攻击结束帧调用，关闭攻击碰撞体
    public void DisableAttackCollider()
    {
        var col = GetComponentInChildren<Attack>()?.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    public override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, checkDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}

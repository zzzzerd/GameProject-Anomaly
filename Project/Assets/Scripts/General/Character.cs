using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;  //当前力量滑铲值
    public float powerRecoverSpeed; //力量回复速度


    [Header("受伤无敌")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;

    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    //接受伤害逻辑和行为
    public void TakeDamage(Attack attacker)
    {
        //Debug.Log(attacker.damage);
        //如果是在无敌状态就不受伤
        if (invulnerable)
        {
            Debug.Log("进入TakeDamage函数：无敌状态");
            return;
        }

        
        //非无敌状态-剩余血量-受到伤害
        if (currentHealth - attacker.damage > 0)
        {
            Debug.Log($"[TakeDamage] 进入受伤逻辑 | 当前血量: {currentHealth} | 伤害: {attacker.damage} | 攻击者: {attacker.name}");

            //血量减少
            currentHealth -= attacker.damage;

            Debug.Log($"[TakeDamage] 受伤完成 | 剩余血量: {currentHealth}");
            //触发无敌
            TriggerInvulnerable();
            //执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
       

        }
        //非无敌状态-没有血量-死亡
        else
        {
            currentHealth = 0;
            //触发死亡
            OnDie?.Invoke();
        }

        Debug.Log($"[OnHealthChange] 调用对象: {gameObject.name}");
        //OnHealthChange?.Invoke(this);

        OnHealthChange?.Invoke(this);
    }

    //触发无敌
    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;

        }
    }


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (invulnerable)
        {
            //计时器开始运行，递减
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        if (currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
        
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            //死亡
            OnDie.Invoke();//死亡的一系列事件
            //跟新血量
            //播放音乐
        }
    }

    /// <summary>
    /// 更新power数值，要在playerController里面滑铲的时候调用
    /// </summary>
    /// <param name="cost">这里是每次调用减去的power数值</param>
    public void OnSlide(int cost)
    {
        currentPower -= cost;
        //这里
        OnHealthChange?.Invoke(this);
    }


}

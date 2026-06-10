using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour,ISaveService
{
    [Header("事件监听")]
    public VoidEventSO newGameEventSO;


    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;  //当前力量滑铲值
    public float powerRecoverSpeed; //力量回复速度


    [Header("受伤无敌")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;

    [Header("音效")]
    public AudioDefination healAudio;  // 加血音效

    public UnityEvent<Character> OnHealthChange;

    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    /// <summary>
    /// 加血的逻辑，不超过 maxHealth
    /// </summary>
    /// <param name="amount">回复量</param>
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healAudio?.PlayAudioCLip();//播放加血音效
        OnHealthChange?.Invoke(this);
    }

    //玩家被伤害
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
            //Debug.Log($"[TakeDamage] 进入受伤逻辑 | 当前血量: {currentHealth} | 伤害: {attacker.damage} | 攻击者: {attacker.name}");

            //血量减少
            currentHealth -= attacker.damage;

            //Debug.Log($"[TakeDamage] 受伤完成 | 剩余血量: {currentHealth}");
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

        //Debug.Log($"[OnHealthChange] 调用对象: {gameObject.name}");
        //OnHealthChange?.Invoke(this);

        OnHealthChange?.Invoke(this);
    }


    private void OnEnable()
    {
        newGameEventSO.OnEventRaised += NewGame;

        //保存系统注册
        ISaveService saveble = this;
        saveble.TurnToSaveble();
    }

    private void OnDisable()
    {
        newGameEventSO.OnEventRaised -= NewGame;
        //保存系统注销
        ISaveService saveble = this;
        saveble.TurnToUnsaveble();
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
    //就是会重置一下血量和能量值
    private void Awake()
    {
        // 如果 currentHealth 未在 Inspector 设置，自动用 maxHealth 初始化
        if (currentHealth <= 0)
            currentHealth = maxHealth;
        if (currentPower <= 0)
            currentPower = maxPower;
    }

    void NewGame()
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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Water"))
            return;


        Debug.Log("Water Trigger | isDead={GetComponent<PlayerController>().isDead}");

        Debug.Log( $"[Death] 掉进水里 | pos={transform.position}");


        var playerController = GetComponent<PlayerController>();

        if (playerController != null && playerController.isDead)
            return;

        currentHealth = 0;

        OnHealthChange?.Invoke(this);

        OnDie?.Invoke();
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


    ///关于这个character的数据保存(血量能量以及位置)

    /// <summary>
    /// 获取当前的id
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public UniqueId GetUniqueId()
    {
        //throw new System.NotImplementedException();
        return GetComponent<UniqueId>();
    }


    /// <summary>
    /// 添加数据到"数据库"里面
    /// </summary>
    public void ReadSaveData(GameData data)
    {
        //创建这个对象的数据结构
        string id = GetUniqueId().Id;
        var charData = new CharacterData
        {
            position = new SerializeVector3(transform.position),
            currentHealth = currentHealth,
            currentPower = currentPower
        };

        //如果存在就修改，不存在直接添加
        if (data.characterData.ContainsKey(id))
        {
            data.characterData[id] = charData;
        }
        else
        {
            data.characterData.Add(id, charData);
        }
    }

    /// <summary>
    /// 加载当前这个对象的数据把数据加载
    /// </summary>
    /// <param name="data"></param>
    public void LoadData(GameData data)
    {
        //根据唯一id找到对应的数据
        string id = GetUniqueId().Id;
        if (data.characterData.TryGetValue(id, out CharacterData charData))
        {
            transform.position = charData.position.ToVector3();
            currentHealth = charData.currentHealth;
            currentPower = charData.currentPower;
            OnHealthChange?.Invoke(this);
        }
    }
}

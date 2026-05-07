using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;//伤害值
    public float attackRange; //攻击范围
    public float damageRate; //攻击频率

    private void OnTriggerStay2D(Collider2D other)
    {
        //测试
        Debug.Log("--------触发攻击------------");
        string attackerName = gameObject.name;
        string targetName = other.gameObject.name;
        Debug.Log($"[Attack] {attackerName} → {targetName} | Damage: {damage}");

        //被攻击的人
        //other.GetComponent<Character>().currentHealth
        other.GetComponent<Character>()?.TakeDamage(this);//把自己传递进去

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

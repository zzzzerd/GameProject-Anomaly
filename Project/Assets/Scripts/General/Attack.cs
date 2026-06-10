using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;        // 伤害值
    public float attackRange; // 攻击范围
    public float attackRate;  // 攻击频率

    private void OnTriggerStay2D(Collider2D other)
    {
        // 向上找父物体，兼容碰到玩家子物体（如Bounds）的情况
        var character = other.GetComponentInParent<Character>();
        if (character != null)
        {
            character.TakeDamage(this);
        }
    }
}

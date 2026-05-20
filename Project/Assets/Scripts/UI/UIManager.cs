using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerStatBar playerStatBar;
    [Header("事件监听")]
    public CharacterEventSO healthEvent;//监听事件

    /// <summary>
    /// 注册事件（固定写法）
    /// </summary>
    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent; //一个事件可以注册多个函数

    }

    /// <summary>
    /// 取消注册，所以就不会接收到事件的消息了
    /// </summary>
    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;

    }

    private void OnHealthEvent(Character character)
    {
        var persentage = character.currentHealth / character.maxHealth;
        playerStatBar.OnHealthChange(persentage);

    }
}

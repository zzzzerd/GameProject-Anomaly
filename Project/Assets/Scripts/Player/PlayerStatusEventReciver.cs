using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerStatusEventReciver : MonoBehaviour
{
    public StatusEventSO badEvent;
    public StatusGoodEventSO goodEvent;

    private Character character;
    private OtherWorldManager otherWorldManager;

    private void Awake()
    {
        character = GetComponent<Character>();
        otherWorldManager = FindAnyObjectByType<OtherWorldManager>();
    }

    private void OnEnable()
    {
        badEvent.OnEventRaised += BadEventExecute;
        goodEvent.OnEventRaised += GoodEventExecute;
    }

    private void OnDisable()
    {
        badEvent.OnEventRaised -= BadEventExecute;
        goodEvent.OnEventRaised -= GoodEventExecute;
    }

    /// <summary>
    /// 坏事件：进入异世界（和按P键逻辑一样）
    /// </summary>
    private void BadEventExecute()
    {
        Debug.Log("|玩家接受到雕塑坏事件，开始进入异世界");
        if (otherWorldManager != null)
        {
            otherWorldManager.EnterOtherWorld();
        }
        else
        {
            Debug.LogWarning("PlayerStatusEventReciver|玩家OtherWorldManager 未找到，无法进入异世界");
        }
    }

    /// <summary>
    /// 好事件：加血量和power
    /// </summary>
    private void GoodEventExecute(float addHealth, float addPower)
    {
        Debug.Log($"PlayerStatusEventReciver|玩家接受到雕塑好事件，开始加血: {addHealth}, 加Power: {addPower}");
        if (character != null)
        {
            character.Heal(addHealth);
            character.currentPower = Mathf.Min(character.currentPower + addPower, character.maxPower);
            character.OnHealthChange?.Invoke(character); // 刷新UI
        }
    }
}

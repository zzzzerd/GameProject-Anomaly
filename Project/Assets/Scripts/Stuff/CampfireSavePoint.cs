using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireSavePoint : SavePointBase
{
    public Animator animator;
    public CampfireEventSO campfireEvent;
    public float healAmount = 20f;



    protected override void OnActivatedVisual()
    {
        Debug.Log("生火");

        animator.SetBool("fire", true);

    }

    /// <summary>
    /// 通知玩家 + 统计
    /// </summary>
    protected override void OnFirstActivated()
    {
        Debug.Log("事件");
        campfireEvent.RaiseEvent(healAmount);

        // 统计：篝火 +1
        GameDataManager.Instance?.AddLitCampfire();
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatueSavePoint : SavePointBase
{
    private SpriteRenderer spriteRenderer;

    //暂时还没有想好这个雕塑具体干什么

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }



    protected override void OnActivatedVisual()
    {
        Debug.Log("雕塑激活|开始形态变化");
        spriteRenderer.color = Color.yellow;
        //animator.SetBool("fire", true);

    }

    /// <summary>
    /// 通知玩家
    /// </summary>
    protected override void OnFirstActivated()
    {
        Debug.Log("雕塑激活|开始事件逻辑的广播");
        //激活雕塑事件记录+1
        //存档
        //获得能力
        //campfireEvent.RaiseEvent(healAmount);
    }
}

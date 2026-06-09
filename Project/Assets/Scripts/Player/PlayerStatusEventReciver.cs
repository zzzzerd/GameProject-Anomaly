using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerStatusEventReciver : MonoBehaviour
{
    public StatusEventSO badEvent;
    public StatusGoodEventSO goodEvent;
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


    //玩家执行
    private void BadEventExecute()
    {
        Debug.Log($"|玩家这边接受到雕塑坏事件，开始执行转换世界逻辑逻辑");

    }


    private void GoodEventExecute(float arg0, float arg1)
    {
        Debug.Log($"|玩家这边接受到雕塑好事件，开始执行加血量等逻辑");

    }
}

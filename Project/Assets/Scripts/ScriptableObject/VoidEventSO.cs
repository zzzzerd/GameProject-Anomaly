using UnityEngine;
using UnityEngine.Events;

//创建菜单里面的选项
[CreateAssetMenu(menuName = "Event/VoidEventSO")]
//继承ScriptableObject就是一个独立事件资源,可以跨场景使用
public class VoidEventSO : ScriptableObject
{
    //一个可以用来存储很多事情的广播表，谁订阅了这个事件，就会在触发的时候被通知
    public UnityAction OnEventRaised;
     
    /// <summary>
    /// 
    /// 调用raiseEvent就相当于广播发声
    /// </summary>
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }

}

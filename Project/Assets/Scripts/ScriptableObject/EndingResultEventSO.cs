using System;
using UnityEngine;
/// <summary>
/// 结局事件
/// </summary>
[CreateAssetMenu(menuName = "Event/EndingResultEventSO")]
public class EndingResultEventSO : ScriptableObject
{
    public event Action<EndingType> OnEventRaised;

    public void RaiseEvent(EndingType endingType)
    {
        OnEventRaised?.Invoke(endingType);
    }
}

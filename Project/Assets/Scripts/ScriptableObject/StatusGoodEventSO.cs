using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/StatusGood Event")]
public class StatusGoodEventSO : ScriptableObject
{
    public UnityAction<float, float> OnEventRaised;

    public void RaiseEvent(float health, float power)
    {
        OnEventRaised?.Invoke(health, power);
    }
}
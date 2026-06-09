using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Campfire Event")]
public class CampfireEventSO : ScriptableObject
{
    public UnityAction<float> OnEventRaised;

    public void RaiseEvent(float healAmount)
    {
        OnEventRaised?.Invoke(healAmount);
    }
}
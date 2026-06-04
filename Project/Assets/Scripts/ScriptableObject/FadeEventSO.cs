using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/FadeEvent", order = 1)]
public class FadeEventSO : ScriptableObject
{

    public UnityAction<Color, float, bool> OnEventRaised;


    /// <summary>
    /// bian black
    /// </summary>
    /// <param name="duration"></param>
    public void FadeIn(float duration)
    {
        RaiseEvent(Color.black, duration, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="duration"></param>
    public void FadeOut(float duration)
    {
        RaiseEvent(Color.clear, duration, false);
    }

    public void RaiseEvent(Color color, float duration, bool isFadeIn)
    {

         OnEventRaised?.Invoke(color, duration, isFadeIn);
        
    }

}

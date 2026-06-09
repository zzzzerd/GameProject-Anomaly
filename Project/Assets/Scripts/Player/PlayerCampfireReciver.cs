using UnityEngine;

public class PlayerCampfireReceiver : MonoBehaviour
{
    //篝火事件发生了
    public CampfireEventSO campfireEvent;

    private Character character;
    private Animator animator;

    private void Awake()
    {
        character = GetComponent<Character>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        campfireEvent.OnEventRaised += OnCampfire;
    }

    private void OnDisable()
    {
        campfireEvent.OnEventRaised -= OnCampfire;
    }


    /// <summary>
    /// 发生时执行的事
    /// </summary>
    /// <param name="healAmount"></param>
    private void OnCampfire(float healAmount)
    {
        Debug.Log($"|玩家这边接受到篝火信号，开始执行篝火逻辑");
        animator.SetTrigger("fire");

        character.Heal(healAmount);

        Debug.Log($"|玩家这边:回血 {healAmount}");
    }
}
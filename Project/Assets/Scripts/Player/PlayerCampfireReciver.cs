using System.Collections;
using UnityEngine;

public class PlayerCampfireReceiver : MonoBehaviour
{
    //篝火事件发生了
    public CampfireEventSO campfireEvent;

    [Header("烧火禁止移动时间")]
    public float freezeDuration = 2f;

    private Character character;
    private Animator animator;
    private PlayerController playerController;

    private void Awake()
    {
        character = GetComponent<Character>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
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

        // 禁止移动
        StartCoroutine(FreezePlayer());

        Debug.Log($"|玩家这边:回血 {healAmount}");
    }

    private IEnumerator FreezePlayer()
    {
        if (playerController != null)
        {
            playerController.isFire = true;
            yield return new WaitForSeconds(freezeDuration);
            playerController.isFire = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class FadingController : MonoBehaviour
{
    [Header("事件监听")]
    public FadeEventSO fadeEventSO;
    public Image fadeImage;

    private void OnEnable()
    {
        fadeEventSO.OnEventRaised += OnFadeEvent;
    }

    private void OnDisable()
    {
        fadeEventSO.OnEventRaised -= OnFadeEvent;
    }


    /// <summary>
    /// 要变成什么颜色，第二个参数是持续时间，单位是秒
    /// </summary>
    private void OnFadeEvent(Color target, float duration, bool isfadeIn)
    {
        // 确保 fadeImage 的 GameObject 是激活的
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOColor(target, duration);
    }


}

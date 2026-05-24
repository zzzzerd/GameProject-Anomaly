using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioManager : MonoBehaviour
{
    [Header("事件监听")]
    public PlayAudioEventSO FXevent; //音效
    public PlayAudioEventSO BGMevent; //bgm
    public AudioSource BGMSource;
    public AudioSource FXSource;

    public void OnEnable()
    {
        FXevent.OnEventRaised += OnFXEvent;
        BGMevent.OnEventRaised += OnBGMEvent;
    }


    public void OnDisable()
    {
        FXevent.OnEventRaised-= OnFXEvent;
        BGMevent.OnEventRaised -= OnBGMEvent;
    }


    //音效
    private void OnFXEvent(AudioClip clip)
    {
        FXSource.clip = clip;
        FXSource.Play();//单次播放
       // throw new NotImplementedException();

    }

    //每个场景准备一个空物体告诉这个bgm是啥
    private void OnBGMEvent(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();//单次播放
    }
}

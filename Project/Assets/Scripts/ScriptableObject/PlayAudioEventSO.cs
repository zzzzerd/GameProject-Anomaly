using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用这个来传递每个挂载在物体身上的audioSource，传递给manager来播放
/// </summary>

[CreateAssetMenu(menuName ="Event/PlayAudioEventSO")]
public class PlayAudioEventSO : ScriptableObject
{
    //要传递的音乐片段(这是一个喇叭)
    public UnityAction<AudioClip> OnEventRaised;

    //别的地方的人调用这的函数，就相当于触发这个事件了
    public void RaisedEvent(AudioClip ac)
    {
        OnEventRaised?.Invoke(ac);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

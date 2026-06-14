using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    
    public PlayAudioEventSO pEvent; //就是创建了一个原型模板啊，
    public AudioClip audioClip;
    public bool playOnEnable;   //是不是挂载的物体一激活就会播放音乐


    //当物体激活的时候
    private void OnEnable()
    {
        Debug.Log($"[AudioDefination] OnEnable 触发，物体：{gameObject.name}，playOnEnable：{playOnEnable}");
        if (playOnEnable)
            PlayAudioCLip();
    }

    //调用这个函数就相当于激活这个播放喇叭
    public void PlayAudioCLip()
    {
        Debug.Log($"[AudioDefination] PlayAudioCLip 调用，pEvent：{pEvent}，audioClip：{audioClip}");
        if (pEvent == null) { Debug.LogError("[AudioDefination] pEvent 为空！"); return; }
        if (audioClip == null) { Debug.LogWarning("[AudioDefination] audioClip 未赋值！"); return; }
        Debug.Log($"[AudioDefination] RaisedEvent 执行，订阅者数量：{pEvent.OnEventRaised?.GetInvocationList().Length}");
        pEvent.RaisedEvent(audioClip);
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

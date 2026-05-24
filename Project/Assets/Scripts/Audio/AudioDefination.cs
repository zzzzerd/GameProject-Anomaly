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
        if (playOnEnable)
            PlayAudioCLip();
            
        
    }

    //调用这个函数就相当于激活这个播放喇叭
    public void PlayAudioCLip()
    {
        //Debug.Log($"执行播放，所属物体：{gameObject.name}", gameObject);
        //if (pEvent == null)
        //{
        //    Debug.LogError($"报错！物体【{gameObject.name}】上的pEvent为空", gameObject);
        //    return;
        //}
        //if (audioClip == null)
        //{
        //    Debug.LogWarning($"物体【{gameObject.name}】音频片段未赋值", gameObject);
        //    return;
        //}
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

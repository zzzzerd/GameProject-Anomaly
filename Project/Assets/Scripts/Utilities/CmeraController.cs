using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
/// <summary>
/// 把bounds赋值给相机，控制移动的范围
/// </summary>
public class CmeraController : MonoBehaviour
{

    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource iSource;
    public VoidEventSO cameraShakeEvent;


  

    // Start is called before the first frame update
    void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
        //impulseSource = GetComponent<CinemachineImpulseSource>();
    }


    /// <summary>
    /// 
    /// </summary>
    private void OnCameraShakeEvent()
    {
        iSource.GenerateImpulse();
    }
    private void OnEnable()
    {
        //cameraShakeEvent被触发的时候,执行OnCameraShakeEvent的方法

        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
    }
    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
    }

    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null)
            return;
        //获得新图形身上的组件Bounds
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        //清空缓存
        confiner2D.InvalidateCache();
    }
    // Update is called once per frame
    void Start()
    {
        GetNewCameraBounds();
    }
}

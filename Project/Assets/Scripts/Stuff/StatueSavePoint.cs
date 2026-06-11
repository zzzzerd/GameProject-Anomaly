using UnityEngine;


public class StatueSavePoint : SavePointBase
{

    [Header("雕塑类型")]
    public StatueType statueType;
    private SpriteRenderer spriteRenderer;

    [Header("雕塑广播的事件-anomaly")]
    public StatusEventSO BadEvent;

    [Header("雕塑广播事件-好事情")]
    public StatusGoodEventSO goodEvent;

    [Header("广播 - 好雕像激活任务事件（连 TaskManager 的 statueActivatedEventSO）")]
    public VoidEventSO statueActivatedEventSO;
    public float addHealth;
    public float addPower;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 第一次激活的
    /// 
    /// </summary>
    protected override void OnFirstActivated()
    {
        Debug.Log("雕塑激活发生事件");
        //这里应该是两种，一种是增加一次激活数量，一种是去往异世界的异常雕塑,用枚举类,然后可以挂载雕塑上面选择的
        //好的激活事件就加血吧

        if (statueType == StatueType.Good)
        {
            Debug.Log("【雕塑:获得好的事件】");
            GameDataManager.Instance.AddActivatedStar();
            goodEvent.RaiseEvent(addHealth, addPower);
            statueActivatedEventSO?.RaiseEvent();  // 通知TaskManager
        }
        else if (statueType == StatueType.Anomaly)
        {
            Debug.Log("【雕塑:去往异世界事件】");
            GameDataManager.Instance.AddEnteredOtherWorld();//事件增加
            BadEvent.RaiseEvent();
        }
    }


    /// <summary>
    /// 这个就是本身激活后状态
    /// </summary>
    protected override void OnActivatedVisual()
    {
        Debug.Log("变成雕塑激活了后的状态");
        //改变雕塑的样子
        ChangeTheStatus();
    }


    /// <summary>
    /// 
    /// </summary>
    private void ChangeTheStatus()
    {

        Debug.Log("雕塑变色");

        if (statueType == StatueType.Good)
        {
            spriteRenderer.color = Color.yellow;
        }
        else
        {
            spriteRenderer.color = Color.red;
        }

    }

}

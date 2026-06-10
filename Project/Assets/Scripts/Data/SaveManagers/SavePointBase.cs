using UnityEngine;

public abstract class SavePointBase : MonoBehaviour, interInteractable, ISaveService
{
    [Header("存档广播")]
    public VoidEventSO saveGameEvent;
    public bool isDone;//当前完成状态

    public void TriggerAction()
    {
        if (isDone)
            return;

        // 先改状态 + 首次激活逻辑 + 视觉
        OnFirstActivated(); //player变化或者一次性的东西

        isDone = true;
        gameObject.tag = "Untagged";

        OnActivatedVisual(); // 播放视觉效果物体本身变化

        // 最后存档，这样 isDone 已经是 true 了
        saveGameEvent.RaiseEvent(); //发出广播,有人要求存档了
    }

    protected abstract void OnFirstActivated();
    protected abstract void OnActivatedVisual();




    // 存档相关接口:

    public UniqueId GetUniqueId()
    {
        return GetComponent<UniqueId>();
    }


    /// <summary>
    /// 写入
    /// </summary>
    /// <param name="data"></param>
    public void ReadSaveData(GameData data)
    {
        string id = GetUniqueId().Id;
        //Debug.Log($"保存 {name}  ID={id}  isDone={isDone}");
        if (data.sceneObjectData.ContainsKey(id))
        {
            data.sceneObjectData[id] = new SceneObjectData { isDone = isDone };
        }
        else
        {
            data.sceneObjectData.Add(id, new SceneObjectData { isDone = isDone });
        }
    }


    /// <summary>
    /// 读取数据
    /// </summary>
    /// <param name="data"></param>
    public void LoadData(GameData data)
    {

        //测试
        //Debug.Log(
        //    name
        //    + " 正在读取存档"
        //);
        string id = GetUniqueId().Id;

        if (data.sceneObjectData.TryGetValue(id, out SceneObjectData objData))
        {
            //Debug.Log(
            //    $"找到存档 {name}  isDone={objData.isDone}"
            //);

            isDone = objData.isDone;

            //已经done了的话要把那个视觉上还有接触上都改了
            //Debug.Log(name + " 读取到 isDone=" + isDone);
            if (isDone)
            {
                gameObject.tag = "Untagged";
                OnActivatedVisual(); 
            }
        }
        //else
        //{
        //    Debug.LogWarning(
        //        $"没找到存档数据 {name}"
        //    );

        //}
    }

    //注册/注销

    private void OnEnable()
    {
        ISaveService saveble = this;
        
        saveble.TurnToSaveble();
        //Debug.Log(name + " 注册存档");
    }

    private void OnDisable()
    {
        ISaveService saveble = this;
        saveble.TurnToUnsaveble();
        //Debug.Log(name + " 注销存档");
    }
}
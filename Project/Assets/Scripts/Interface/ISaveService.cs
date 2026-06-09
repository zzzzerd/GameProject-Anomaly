using UnityEngine;

public interface ISaveService
{
    /// <summary>
    /// 返回物体上挂载的 UniqueId 组件，用于存档系统按 ID 存取数据。
    /// 实现了 ISaveService 的物体必须同时挂载 UniqueId 组件。
    /// </summary>
    UniqueId GetUniqueId();

    /// <summary>
    /// 注册到存档系统
    /// </summary>
    void TurnToSaveble()
    {
        if (GameDataManager.Instance != null)
            GameDataManager.Instance.AddToSavableList(this);
    }

    /// <summary>
    /// 从存档系统注销
    /// </summary>
    void TurnToUnsaveble()
    {
        if (GameDataManager.Instance != null)
            GameDataManager.Instance.RemoveFromSavableList(this);
    }

    void ReadSaveData(GameData data);
    void LoadData(GameData data);
}

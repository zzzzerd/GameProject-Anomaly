using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameDataManager : MonoBehaviour
{
    [Header("监听广播数据-保存")]
    public VoidEventSO saveGameEvent;

    [Header("监听广播数据-加载存档")]
    public VoidEventSO loadGameEvent;
    public static GameDataManager Instance;
    private List<ISaveService> savableList = new List<ISaveService>();
    private GameData data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        data = new GameData();
    }

    public void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Debug.Log("按下了L键，触发加载存档事件");
            Load();
        }
    }

    private void OnEnable()
    {
        saveGameEvent.OnEventRaised += Save;
        loadGameEvent.OnEventRaised += Load;
    }

    private void OnDisable()
    {
        saveGameEvent.OnEventRaised -= Save;
        loadGameEvent.OnEventRaised -= Load;

    }

    public void AddToSavableList(ISaveService saveService)
    {
        if (!savableList.Contains(saveService)) savableList.Add(saveService);
    }

    public void RemoveFromSavableList(ISaveService saveService)
    {
        // MonoBehaviour 已被销毁时，直接移除（Unity 的 == null 会返回 true）
        if (saveService is MonoBehaviour mb && mb == null) return;
        savableList.Remove(saveService);
    }

    /// <summary>
    /// saveGameEvent 事件广播时运行，更新存档数据
    /// </summary>
    private void Save()
    {
        // 每个要存储的对象，把最新数据更新到 data 中
        foreach (var saveable in savableList)
        {
            saveable.ReadSaveData(data);
        }

        // 打印角色数据
        foreach (var kvp in data.characterData)
        {
            Debug.Log($"角色存档: ID={kvp.Key}, Position={kvp.Value.position}, Health={kvp.Value.currentHealth}");
        }

        //打印场景物体数据
        foreach (var kvp in data.sceneObjectData)
        {
            Debug.Log($"场景物体存档: ID={kvp.Key}, isDone={kvp.Value.isDone}");
        }

        //打印玩家统计
        Debug.Log($"玩家统计: 开箱={data.playerStats.openedChests}, 篝火={data.playerStats.litCampfires}, " +
                  $"掉星={data.playerStats.activatedStars}, 杀敌={data.playerStats.killedEnemies}, " +
                  $"异世界={data.playerStats.enteredOtherWorld}");

        // TODO: 序列化 data 为 JSON 写入文件

    }

    public bool HasSaveData()
    {
        return data != null && data.characterData != null && data.characterData.Count > 0;
    }

    public bool TryLoad()
    {
        Debug.Log($"[GameDataManager] TryLoad | hasSave={HasSaveData()} | characterDataCount={(data?.characterData != null ? data.characterData.Count : 0)}");

        if (!HasSaveData())
        {
            Debug.LogWarning("[GameDataManager] 当前没有可用存档，跳过读档。");
            return false;
        }

        Load();
        return true;
    }

    public void Load()
    {
        if (!HasSaveData())
        {
            Debug.LogWarning("[GameDataManager] 当前没有可用存档，Load 不执行。");
            return;
        }

        // TODO: 从文件读取 JSON 反序列化为 data
        // var json = File.ReadAllText(path);
        // data = JsonUtility.FromJson<GameData>(json);
        Debug.Log("[GameDataManager] =====开始读档=====");
        Debug.Log("[GameDataManager] 存档对象数量：" + savableList.Count);
        foreach (var saveable in savableList)
        {
            Debug.Log("[GameDataManager] LoadData -> " + saveable);
            saveable.LoadData(data);
        }
        Debug.Log("[GameDataManager] =====读档结束=====");
    }

    /// <summary>
    /// 供 OtherWorldManager 调用，退出异世界时先保存再读档回到存档点
    /// </summary>
    public void SaveAndLoad()
    {
        // 先保存当前数据（确保统计数据等不丢失）
        Save();
        // 然后从存档数据加载回存档点
        Load();
    }




    // ==================== 玩家统计接口 ====================

    /// <summary>
    /// 增加打开箱子数
    /// </summary>
    public void AddOpenedChest()
    {
        data.playerStats.openedChests++;
    }

    /// <summary>
    /// 增加升起篝火数
    /// </summary>
    public void AddLitCampfire()
    {
        data.playerStats.litCampfires++;
    }

    /// <summary>
    /// 增加激活掉星数
    /// </summary>
    public void AddActivatedStar()
    {
        data.playerStats.activatedStars++;
    }

    /// <summary>
    /// 增加杀死敌人数
    /// </summary>
    public void AddKilledEnemy()
    {
        data.playerStats.killedEnemies++;
        Debug.Log(
    "杀敌增加后 = "
    + data.playerStats.killedEnemies
);
    }

    /// <summary>
    /// 增加去往异世界次数
    /// </summary>
    public void AddEnteredOtherWorld()
    {
        data.playerStats.enteredOtherWorld++;
    }
}

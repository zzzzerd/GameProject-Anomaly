using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameDataManager : MonoBehaviour
{
    [Header("监听广播数据")]
    public VoidEventSO saveGameEvent;
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
    }

    private void OnDisable()
    {
        saveGameEvent.OnEventRaised -= Save;
    }

    public void AddToSavableList(ISaveService saveService)
    {
        if (!savableList.Contains(saveService)) savableList.Add(saveService);
    }

    public void RemoveFromSavableList(ISaveService saveService)
    {
        savableList.Remove(saveService);
    }

    /// <summary>
    /// saveGameEvent 事件广播时运行，这是一次整体的储存
    /// </summary>
    private void Save()
    {
        // 创建存档数据，但保留运行时的玩家统计（统计是累积的，不应随 new 重置）
        GameData saveData = new GameData();
        saveData.playerStats = data.playerStats;

        //每个要存储的数据，把数据写到saveData里面
        foreach (var saveable in savableList)
        {
            saveable.ReadSaveData(saveData);
        }

        // 测试：打印角色数据
        foreach (var kvp in saveData.characterData)
        {
            Debug.Log($"角色存档: ID={kvp.Key}, Position={kvp.Value.position}, Health={kvp.Value.currentHealth}");
        }

        // 测试：打印场景物体数据
        foreach (var kvp in saveData.sceneObjectData)
        {
            Debug.Log($"场景物体存档: ID={kvp.Key}, isDone={kvp.Value.isDone}");
        }

        // 测试：打印玩家统计
        Debug.Log($"玩家统计: 开箱={saveData.playerStats.openedChests}, 篝火={saveData.playerStats.litCampfires}, " +
                  $"掉星={saveData.playerStats.activatedStars}, 杀敌={saveData.playerStats.killedEnemies}, " +
                  $"异世界={saveData.playerStats.enteredOtherWorld}");

        // TODO: 序列化 saveData 为 JSON 写入文件

    }

    private void Load()
    {
        // TODO: 从文件读取 JSON 反序列化为 data
        // var json = File.ReadAllText(path);
        // data = JsonUtility.FromJson<GameData>(json);

        foreach (var saveable in savableList)
        {
            saveable.LoadData(data);
        }
    }

    /// <summary>
    /// 供 OtherWorldManager 调用，退出异世界时读档回到存档点
    /// </summary>
    public void LoadFromSave()
    {
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

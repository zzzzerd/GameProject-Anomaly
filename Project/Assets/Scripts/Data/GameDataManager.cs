using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
//using UnityEngine.InputSystem;
using Newtonsoft.Json;

//数字越小越早执行
[DefaultExecutionOrder(-200)]

public class GameDataManager : MonoBehaviour
{
    [Header("监听广播数据-保存")]
    public VoidEventSO saveGameEvent;

    [Header("监听广播数据-加载存档")]
    public VoidEventSO loadGameEvent;
    public static GameDataManager Instance;
   
    
    private List<ISaveService> savableList = new List<ISaveService>();
    private GameData data;
    public GameData Data => data;

    [Header("关于磁盘存储")]
    private string jasonPath;
    


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

        jasonPath = Application.persistentDataPath + "/AnomalyGmaeSaveData/";
        ReadSavedData();//游戏一开始就能读到过往的数据(如果有的话)
    
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
        Debug.Log("---【Save-开始写入数据】---");
        // 每个要存储的对象，把最新数据更新到 data 中
        //把每个要保存的对象数据保存在data数据结构里面,由于每个数据实现的具体接口不一样，所以保存方式多态
        foreach (var saveable in savableList)
        {
            saveable.ReadSaveData(data);
        }

        // 打印角色数据
        //foreach (var kvp in data.characterData)
        //{
        //    Debug.Log($"【Save-1】角色存档: ID={kvp.Key}, Position={kvp.Value.position}, Health={kvp.Value.currentHealth}");
        //}

        ////打印场景物体数据
        //foreach (var kvp in data.sceneObjectData)
        //{
        //    Debug.Log($"【Save-2】场景物体存档: ID={kvp.Key}, isDone={kvp.Value.isDone}");
        //}

        //打印玩家统计
        Debug.Log($"【Save-3】玩家统计: 开箱={data.playerStats.openedChests}, 篝火={data.playerStats.litCampfires}, " +
                  $"掉星={data.playerStats.activatedStars}, 杀敌={data.playerStats.killedEnemies}, " +
                  $"异世界={data.playerStats.enteredOtherWorld}");

        // TODO: 序列化 data 为 JSON 写入文件
        var resultPath = jasonPath + "GameData.zsr";
        var jasonData = JsonConvert.SerializeObject(data);
        if (!File.Exists(resultPath))
        {
            Debug.Log("【SAVE】写入文件时，不存在文件，所以要创建");
            Directory.CreateDirectory(jasonPath);
        }
        File.WriteAllText(resultPath, jasonData);
        Debug.Log("存档路径：" + resultPath);//测试
    }

    public bool HasSaveData()
    {
        return data != null && data.characterData != null && data.characterData.Count > 0;
    }

    public bool TryLoad()
    {
        Debug.Log($"[GameDataManager] TryLoad 执行| hasSave={HasSaveData()} | characterDataCount={(data?.characterData != null ? data.characterData.Count : 0)}");

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

        //测试
        Debug.Log("[GameDataManager] =====Load开始读档=====");
        Debug.Log("[GameDataManager/Load] 存档对象数量：" + savableList.Count);
        foreach (var saveable in savableList)
        {
            saveable.LoadData(data);
        }
        Debug.Log("[GameDataManager/Load] =====读档结束=====");
   
    }







    /// <summary>
    /// OtherWorldManager 调用，退出异世界时先保存再读档回到存档点
    /// </summary>
    public void SaveAndLoad()
    {
        // 先保存当前数据（确保统计数据等不丢失）
        Save();
        // 然后从存档数据加载回存档点
        Load();
    }

    /// <summary>
    /// 阅读并翻译文件里面的数据
    /// </summary>
    private void ReadSavedData()
    {
        var resultPath = jasonPath + "GameData.zsr";
        //var jasonData = JsonConvert.SerializeObject(data);
        if (File.Exists(resultPath))
        {
            var stringData = File.ReadAllText(resultPath);
            //Debug.Log("【SAVE】写入文件时，不存在文件，所以要创建");
            var jsonData = JsonConvert.DeserializeObject<GameData>(stringData);//把stringdata读成data类型
            data = jsonData;
        }
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
        Debug.Log("[GameDataManager] killedEnemies=" + data.playerStats.killedEnemies);
    }

    /// <summary>
    /// 增加击杀 Boss 次数
    /// </summary>
    public void AddKilledBoss()
    {
        data.playerStats.killedBosses++;
        Debug.Log("[GameDataManager] killedBosses=" + data.playerStats.killedBosses);
    }

    /// <summary>
    /// 增加去往异世界次数
    /// </summary>
    public void AddEnteredOtherWorld()
    {
        data.playerStats.enteredOtherWorld++;
    }

    // ==================== 结局存档接口 ====================

    /// <summary>
    /// 删除进度存档（游戏结束时调用，防止死局存档被"继续游戏"读取）
    /// </summary>
    public void DeleteSaveData()
    {
        var resultPath = jasonPath + "GameData.zsr";
        if (File.Exists(resultPath))
        {
            File.Delete(resultPath);
            Debug.Log("[GameDataManager] 进度存档已删除");
        }
        data = new GameData();
    }

    /// <summary>
    /// 追加一条结局记录到 EndingRecord.zsr（不覆盖，只追加）
    /// </summary>
    public void SaveEndingRecord(EndingType endingType)
    {
        var recordPath = jasonPath + "EndingRecord.zsr";
        EndingRecordData recordData;

        if (File.Exists(recordPath))
        {
            var json = File.ReadAllText(recordPath);
            recordData = JsonConvert.DeserializeObject<EndingRecordData>(json) ?? new EndingRecordData();
        }
        else
        {
            Directory.CreateDirectory(jasonPath);
            recordData = new EndingRecordData();
        }

        var stats = data.playerStats;
        int strength   = stats.killedEnemies + stats.killedBosses * 5;
        int hope       = stats.activatedStars * 3 + stats.litCampfires * 2;
        int corruption = stats.enteredOtherWorld * 4;
        recordData.records.Add(new EndingRecord
        {
            endingType        = endingType,
            dateTime          = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            killedEnemies     = stats.killedEnemies,
            killedBosses      = stats.killedBosses,
            litCampfires      = stats.litCampfires,
            activatedStars    = stats.activatedStars,
            enteredOtherWorld = stats.enteredOtherWorld,
            openedChests      = stats.openedChests,
            strength          = strength,
            hope              = hope,
            corruption        = corruption
        });

        File.WriteAllText(recordPath, JsonConvert.SerializeObject(recordData, Formatting.Indented));
        Debug.Log($"[GameDataManager] 结局记录已写入: {endingType} @ {System.DateTime.Now}");
    }

    /// <summary>
    /// 读取所有历史结局记录
    /// </summary>
    public System.Collections.Generic.List<EndingRecord> GetEndingRecords()
    {
        var recordPath = jasonPath + "EndingRecord.zsr";
        if (!File.Exists(recordPath)) return new System.Collections.Generic.List<EndingRecord>();
        var json = File.ReadAllText(recordPath);
        var recordData = JsonConvert.DeserializeObject<EndingRecordData>(json);
        return recordData?.records ?? new System.Collections.Generic.List<EndingRecord>();
    }
}

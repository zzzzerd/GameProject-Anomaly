using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
public class SerializeVector3
{
    public float x; public float y; public float z;
    public SerializeVector3(Vector3 position)
    {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x,y,z);
    }

}
/// <summary>
/// 每个角色的存档数据
/// </summary>
[System.Serializable]
public class CharacterData
{
    public SerializeVector3 position;
    public float currentHealth;
    public float currentPower;
    public bool isDead;
}

/// <summary>
/// 玩家统计（用于结局判定）
/// </summary>
[System.Serializable]
public class PlayerStatsData
{
    public int openedChests;        //打开箱子个数
    public int litCampfires;        //升起篝火个数
    public int activatedStars;      //激活掉星个数
    public int killedEnemies;       //杀死敌人个数
    public int enteredOtherWorld;   //去往异世界次数
}

/// <summary>
/// 场景物体的状态数据（箱子、篝火等）
/// </summary>
[System.Serializable]
public class SceneObjectData
{
    public bool isDone;             // 是否已被触发
    //public Vector3 position;        // 位置（可选）
}

/// <summary>
/// 单条结局记录
/// </summary>
[System.Serializable]
public class EndingRecord
{
    public EndingType endingType;
    public string dateTime;
    public int killedEnemies;
    public int litCampfires;
    public int activatedStars;
    public int enteredOtherWorld;
    public int openedChests;
}

/// <summary>
/// 结局记录列表容器（用于序列化到 EndingRecord.zsr）
/// </summary>
[System.Serializable]
public class EndingRecordData
{
    public System.Collections.Generic.List<EndingRecord> records = new System.Collections.Generic.List<EndingRecord>();
}






public class GameData
{
    /// <summary>
    /// 所有角色数据玩家和敌人，Key = UniqueId
    /// </summary>
    public Dictionary<string, CharacterData> characterData = new Dictionary<string, CharacterData>();
    public PlayerStatsData playerStats = new PlayerStatsData();    //玩家统计数据全局唯一
    public Dictionary<string, SceneObjectData> sceneObjectData = new Dictionary<string, SceneObjectData>(); //场景物体数据（箱子、篝火等），Key = UniqueId
    public string sceneToSave;//要保存的场景



    public void SaveGameScene(GameSceneSO savedScene)
    {
        //把object类型变成一个jason文件
        sceneToSave = JsonUtility.ToJson(savedScene);
        //测试
        Debug.Log(sceneToSave);
    }


    /// <summary>
    /// 把jason改回SO文件
    /// </summary>
    /// <returns></returns>
    public GameSceneSO GetSavedScene()
    {
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();
        //把反序列化后的scene覆盖给new scene
        JsonUtility.FromJsonOverwrite(sceneToSave, newScene);
        return newScene;
    }



    ///// <summary>
    ///// 兼容旧存档：旧的 playerData 暂保留，迁移完成后可删除
    ///// </summary>
    //[System.Obsolete("请使用 characterData 替代")]
    //public Dictionary<string, Vector3> playerData = new Dictionary<string, Vector3>();
}

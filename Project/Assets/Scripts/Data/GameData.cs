using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 每个角色的存档数据
/// </summary>
[System.Serializable]
public class CharacterData
{
    public Vector3 position;
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

public class GameData
{
    /// <summary>
    /// 所有角色数据玩家 + 敌人，Key = UniqueId
    /// </summary>
    public Dictionary<string, CharacterData> characterData = new Dictionary<string, CharacterData>();

    public PlayerStatsData playerStats = new PlayerStatsData();    //玩家统计数据全局唯一
    public Dictionary<string, SceneObjectData> sceneObjectData = new Dictionary<string, SceneObjectData>(); //场景物体数据（箱子、篝火等），Key = UniqueId
    public string sceneToSave;//要保存的场景


    /// <summary>
    /// 
    /// </summary>
    /// <param name="savedScene"></param>
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



    /// <summary>
    /// 兼容旧存档：旧的 playerData 暂保留，迁移完成后可删除
    /// </summary>
    [System.Obsolete("请使用 characterData 替代")]
    public Dictionary<string, Vector3> playerData = new Dictionary<string, Vector3>();
}

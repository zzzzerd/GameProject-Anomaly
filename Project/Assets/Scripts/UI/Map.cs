using UnityEngine;
using UnityEngine.SceneManagement;


public class Map : MonoBehaviour
{
    /// <summary>
    /// 进入地图前由主菜单设置：true=继续旅程，false=开始新旅程
    /// </summary>
    public static bool IsContinueMode = false;

    [Header("关卡场景")]
    public GameSceneSO level1Scene;

    [Header("主菜单场景")]
    public GameSceneSO mainMenuScene;

    [Header("事件")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEventSO;

    /// <summary>
    /// Level1 按钮 OnClick：根据入口模式决定新游戏还是读档
    /// </summary>
    public void LoadLevel1()
    {
        Debug.Log($"[Map] LoadLevel1 被调用 | IsContinueMode={IsContinueMode} | level1Scene={level1Scene}");
        if (level1Scene == null) { Debug.LogError("[Map] level1Scene 未绑定！"); return; }

        if (IsContinueMode)
        {
            // 继续旅程：读存档，由 ScenesLoader 跳转到存档场景
            GameDataManager.Instance.TryLoad();
        }
        else
        {
            // 开始新旅程：触发 newGameEventSO，由 ScenesLoader.NewGame() 处理出生点
            newGameEventSO.RaiseEvent();
        }
    }

    public void ReturnToMainMenu()
    {
        // 使用 Addressable 系统加载场景，保持 Persistent Scene 不被卸载
        if (mainMenuScene == null)
        {
            Debug.LogError("[Map] mainMenuScene 未绑定！");
            return;
        }
        
        if (loadEventSO == null)
        {
            Debug.LogError("[Map] loadEventSO 未绑定！");
            return;
        }
        
        loadEventSO.RaiseLoadRequestEvent(mainMenuScene, Vector3.zero, true);
    }
}

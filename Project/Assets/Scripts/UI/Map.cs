using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class Map : MonoBehaviour
{
    [Header("关卡场景")]
    public GameSceneSO level1Scene;
    public Vector3 level1SpawnPoint;

    [Header("UI")]
    public GameObject newGameButton;

    [Header("事件")]
    public SceneLoadEventSO loadEventSO;

    private void OnEnable()
    {
        // 延迟一帧设置，等 EventSystem 初始化
        StartCoroutine(SetDefaultButton());
    }

    private IEnumerator SetDefaultButton()
    {
        yield return null;
        if (EventSystem.current != null && newGameButton != null)
            EventSystem.current.SetSelectedGameObject(newGameButton);
    }

    public void LoadLevel1()
    {
        if (level1Scene != null)
        {
            loadEventSO.RaiseLoadRequestEvent(level1Scene, level1SpawnPoint, true);
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}

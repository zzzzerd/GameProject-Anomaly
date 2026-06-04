using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, interInteractable
{

    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;
    public void TriggerAction()
    {
        Debug.Log("´«ËÍ " + positionToGo);
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}

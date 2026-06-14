using UnityEngine;

public class BGMTrigger : MonoBehaviour
{
    public AudioDefination enterBGM;
    public AudioDefination exitBGM;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 先打印所有进入的物体
        Debug.Log($"[Enter] 碰到物体：{other.name} | Tag：{other.tag}");

        if (!other.CompareTag("Player"))
        {
            Debug.Log("[Enter] 不是玩家，跳过");
            return;
        }

        Debug.Log("[Enter] 是玩家，开始播放进入BGM");
        enterBGM.PlayAudioCLip();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"[Exit] 离开物体：{other.name} | Tag：{other.tag}");
        if (!other.CompareTag("Player")) return;

        Debug.Log("[Exit] 玩家离开，恢复默认BGM");
        exitBGM.PlayAudioCLip();
    }
}
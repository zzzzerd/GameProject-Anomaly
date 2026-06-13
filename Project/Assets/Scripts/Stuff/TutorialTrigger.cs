using UnityEngine;

/// <summary>
/// 玩家进入范围显示漂浮提示，离开隐藏
/// </summary>
public class TutorialTrigger : MonoBehaviour
{
    [Header("漂浮提示根物体（包含 SpriteRenderer + TextMeshPro）")]
    public GameObject tipRoot;

    private void Start()
    {
        if (tipRoot != null)
            tipRoot.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("触发: " + other.name + " tag: " + other.tag);
        if (!other.CompareTag("Player")) return;
        if (tipRoot != null) tipRoot.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (tipRoot != null) tipRoot.SetActive(false);
    }
}

using UnityEngine;
using TMPro;

/// <summary>
/// 挂在 tipRoot 上，自动设置漂浮文字内容
/// </summary>
public class TutorialPanel : MonoBehaviour
{
    [Header("文字内容")]
    [TextArea(3, 10)]
    public string tutorialText = "按 J 攻击\n有两条路径可探索\n篝火处生火可回血";

    private void Start()
    {
        var tmp = GetComponentInChildren<TMP_Text>();
        if (tmp != null)
            tmp.text = tutorialText;
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 挂在单条结局记录 Prefab 上，负责填充图片和文字
/// </summary>
public class EndingRecordItem : MonoBehaviour
{
    [Header("一些参数")]
    public Image endingImage;      
    public TextMeshProUGUI nameText;  // 结局名称
    public TextMeshProUGUI timeText;  // 时间
    public TextMeshProUGUI dateText;  

    public void Setup(EndingRecord record, Sprite sprite)
    {
        if (endingImage != null)
            endingImage.sprite = sprite;

        if (nameText != null)
            nameText.text = GetEndingName(record.endingType);

        if (timeText != null)
            timeText.text = record.dateTime;
    }

    //获取结局的名字
    private string GetEndingName(EndingType type)
    {
        switch (type)
        {
            case EndingType.Death:
                return "死亡结局";

            case EndingType.Warrior:
                return "勇士结局";

            case EndingType.Saint:
                return "圣者结局";

            case EndingType.AnomalySage:
                return "异界贤者";

            case EndingType.LostSoul:
                return "失魂者";

            case EndingType.Farmer:
                return "归乡者";

            default:
                return "未知结局";
        }
    }
}

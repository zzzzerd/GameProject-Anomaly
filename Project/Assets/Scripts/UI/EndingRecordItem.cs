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
    //public TextMeshProUGUI dateText;  

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
                return "Death";

            case EndingType.Warrior:
                return "Warrior";

            case EndingType.Saint:
                return "Sain";

            case EndingType.AnomalySage:
                return "Anomaly Sage";

            case EndingType.LostSoul:
                return "Lost Soul";

            case EndingType.Farmer:
                return "Farmer";

            default:
                return "Unknown";
        }
    }
}

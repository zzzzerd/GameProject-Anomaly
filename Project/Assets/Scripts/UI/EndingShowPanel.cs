using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingShowPanel : MonoBehaviour
{
    [Header("UI 引用")]
    public Image endingImage;
    public TextMeshProUGUI endingTitleText;
    public TextMeshProUGUI endingDescText;
    public Button continueButton;

    [Header("结局内容（按 EndingType 枚举顺序）")]
    // 顺序：Death=0, Warrior=1, Saint=2, AnomalySage=3, LostSoul=4, Farmer=5
    public Sprite[] endingSprites;
    public string[] endingTitles;
    [TextArea(3, 6)]
    public string[] endingDescs;

    private void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClick);
    }

    public void Show(EndingType type)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        int index = (int)type;

        if (endingImage != null)
        {
            bool hasSprite = endingSprites != null && index < endingSprites.Length && endingSprites[index] != null;
            endingImage.gameObject.SetActive(hasSprite);
            if (hasSprite) endingImage.sprite = endingSprites[index];
        }

        if (endingTitleText != null)
            endingTitleText.text = (endingTitles != null && index < endingTitles.Length) ? endingTitles[index] : type.ToString();

        if (endingDescText != null)
            endingDescText.text = (endingDescs != null && index < endingDescs.Length) ? endingDescs[index] : "";
    }

    private void OnContinueClick()
    {
        gameObject.SetActive(false);
        if (onContinue != null) onContinue.Invoke();
    }

    public System.Action onContinue;
}

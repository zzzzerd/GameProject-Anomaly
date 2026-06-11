using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 分页故事面板：每页一张图 + 一段文字，点 Next/Prev 翻页
/// 挂在 StoryPanel 上，FirstMain 调 Open() 打开
/// </summary>
public class StoryPanel : MonoBehaviour
{
    [Header("UI 引用")]
    public Image storyImage;
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI pageIndicator;   // 显示 "1 / 5"
    public Button prevButton;
    public Button nextButton;
    public TextMeshProUGUI nextButtonText;  // Next 最后一页改成 Close

    [Header("故事内容（顺序填入）")]
    public Sprite[] pageSprites;            // 每页图片，可以为 null
    [TextArea(3, 8)]
    public string[] pageTexts;             // 每页文字

    private int currentPage = 0;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        currentPage = 0;
        gameObject.SetActive(true);
        RefreshPage();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void NextPage()
    {
        Debug.Log($"[StoryPanel] NextPage clicked | currentPage={currentPage} | PageCount={PageCount}");
        if (currentPage >= PageCount - 1)
        {
            Close();
            return;
        }
        currentPage++;
        RefreshPage();
    }

    public void PrevPage()
    {
        if (currentPage <= 0) return;
        currentPage--;
        RefreshPage();
    }

    private int PageCount => Mathf.Max(
        pageSprites != null ? pageSprites.Length : 0,
        pageTexts   != null ? pageTexts.Length   : 0
    );

    private void RefreshPage()
    {
        int total = PageCount;

        // 图片
        if (storyImage != null)
        {
            bool hasSprite = pageSprites != null && currentPage < pageSprites.Length && pageSprites[currentPage] != null;
            storyImage.gameObject.SetActive(hasSprite);
            if (hasSprite) storyImage.sprite = pageSprites[currentPage];
        }

        // 文字
        if (storyText != null)
        {
            string text = (pageTexts != null && currentPage < pageTexts.Length) ? pageTexts[currentPage] : "";
            storyText.text = text;
        }

        // 页码
        if (pageIndicator != null)
            pageIndicator.text = $"{currentPage + 1} / {total}";

        // Prev 按钮
        if (prevButton != null)
            prevButton.interactable = currentPage > 0;

        // Next 按钮：最后一页显示 Close
        if (nextButtonText != null)
            nextButtonText.text = (currentPage >= total - 1) ? "Close" : "Next";
    }
}

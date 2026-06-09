using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class OtherWorldTimerUI : MonoBehaviour
{
    [Header("引用")]
    //public PlayerStatBar playerStatBar;
    public OtherWorldManager otherWorldManager;
    public GameObject timerPanel;
    //public Text timerText;
    public TMP_Text timerText;

    private void Start()
    {
        if (timerPanel != null)
            timerPanel.SetActive(false);
    }

    private void Update()
    {
        if (otherWorldManager == null) return;

        if (otherWorldManager.IsInOtherWorld())
        {
            if (timerPanel != null && !timerPanel.activeSelf)
                timerPanel.SetActive(true);

            float remaining = otherWorldManager.GetRemainingTime();
            int seconds = Mathf.CeilToInt(remaining);

            if (timerText != null)
                timerText.text = $"Time Until Return: {seconds}s";
        }
        else
        {
            if (timerPanel != null && timerPanel.activeSelf)
                timerPanel.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatBar : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;

    private void Update()
    {
        //if (healthDelayImage.fillAmount > healthImage.fillAmount)
        //{
        //    healthDelayImage.fillAmount-=Time.deltaTime*2;//延迟
        //}

        healthDelayImage.fillAmount = Mathf.MoveTowards(
            healthDelayImage.fillAmount,
            healthImage.fillAmount,
            Time.deltaTime * 0.5f
        );
    }

    /// <summary>
    /// 这个就是把组件的数值设为一个传入的参数，shhealth 变化的时候的百分比
    /// </summary>
    /// <param name="persentage">current/Maxhealth</param>
    public void OnHealthChange(float persentage)
    {
        healthImage.fillAmount = persentage;
    }

}

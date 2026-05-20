using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatBar : MonoBehaviour
{
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;


    /// <summary>
    /// health 变化的时候的百分比
    /// </summary>
    /// <param name="persentage">current/Maxhealth</param>
    public void OnHealthChange(float persentage)
    {
        healthImage.fillAmount = persentage;
    }

}

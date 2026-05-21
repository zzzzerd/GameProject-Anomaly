using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatBar : MonoBehaviour
{
    private Character currentCharacter;
    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;
    private bool isRecovering;

    private void Update()
    {
        ////这一个版本没有那么好
        //if (healthDelayImage.fillAmount > healthImage.fillAmount)
        //{
        //    healthDelayImage.fillAmount-=Time.deltaTime*2;//延迟
        //}

        //ai给的反应更明显
        healthDelayImage.fillAmount = Mathf.MoveTowards(
            healthDelayImage.fillAmount,
            healthImage.fillAmount,
            Time.deltaTime * 0.5f
        );

        if (isRecovering)
        {
            //在恢复期间把power的数值持续赋予ui进度条
            float persentage = currentCharacter.currentPower / currentCharacter.maxPower;
            powerImage.fillAmount = persentage;

            //恢复完成以后就结束
            if (persentage >= 1)
            {
                isRecovering = false;
                return;
            }
        }
    }

    /// <summary>
    /// 这个就是把组件的数值设为一个传入的参数，shhealth 变化的时候的百分比
    /// </summary>
    /// <param name="persentage">current/Maxhealth</param>
    public void OnHealthChange(float persentage)
    {
        healthImage.fillAmount = persentage;
    }

    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;
    }
}

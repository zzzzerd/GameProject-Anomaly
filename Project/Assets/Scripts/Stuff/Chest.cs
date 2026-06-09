using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour,interInteractable
{
    //achieve the interface
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    public bool isDone;

    [Header("奖励")]
    public float healAmount = 15f;  // 开箱回复血量，0 表示不加血
    public Character playerCharacter;  // 拖入玩家 Character

    public void TriggerAction()
    {
        //throw new System.NotImplementedException();
        Debug.Log("You opened the chest!");
        if(!isDone)
        {

            OpenChest();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone? openSprite : closeSprite;   
    }

    private void OpenChest()
    {
        StartCoroutine(OpenChestCoroutine());
    }

    private IEnumerator OpenChestCoroutine()
    {
        GetComponent<AudioDefination>()?.PlayAudioCLip();

        // 等待0.5秒
        yield return new WaitForSeconds(0.5f);

        spriteRenderer.sprite = openSprite;

        // 奖励：加血
        if (playerCharacter != null && healAmount > 0)
        {
            playerCharacter.Heal(healAmount);
        }

        // 后续可扩展：二段跳等其他奖励

        isDone = true;
        gameObject.tag = "Untagged";

        // 统计：开箱 +1
        GameDataManager.Instance?.AddOpenedChest();
    }
}

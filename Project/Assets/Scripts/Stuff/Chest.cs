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

        // 可能获得任意技能：加血量、二段跳

        isDone = true;
        gameObject.tag = "Untagged";
    }
}

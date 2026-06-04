using System;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tip : MonoBehaviour
{
    private PlayerInputControl pControl;
    private Animator anim;
    public Transform playerTrans;
    public GameObject tipContents;

    //获得互动的物体
    private interInteractable targetItem;

    private bool canPress;

    private void Awake()
    {
        pControl = new PlayerInputControl();
        pControl.Enable();
        anim = tipContents.GetComponent<Animator>();

        
    }



    private void OnActionChange(object arg1, InputActionChange change)
    {
        if (change ==InputActionChange.ActionStarted)
        {
            var device1 = ((InputAction)arg1).activeControl.device;
            switch (device1.device)
            {
                case Keyboard:
                    anim.Play("tip_e"); //直接播放动画
                    break;
            }
        }
    }
    

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
        pControl.GamePlay.Confirm.started += OnConfirmStarted;
    }



    private void OnDisable()
    {
        //InputSystem.onActionChange += OnActionChange;
        //pControl.GamePlay.Confirm.started += OnConfirmStarted;
        canPress = false;
    }


    private void OnConfirmStarted(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
        if (canPress)
        {
            targetItem.TriggerAction();        
        }

    }

    private void Update()
    {
        //tipContents.SetActive(canPress);
        tipContents.GetComponent<SpriteRenderer>().enabled = canPress;
        tipContents.transform.localScale = playerTrans.localScale;

    }

    public void OnTriggerStay2D(Collider2D other)
  
    {
        if ((other.CompareTag("Interactable")))
        {
            canPress = true;
            targetItem = other.GetComponent<interInteractable>();
        }

    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        canPress = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    //获取组件
    public CapsuleCollider2D coll;
    private PlayerController playerController;
    private Rigidbody2D rb;


    [Header("检测参数")]
    public bool manual;
    public bool isPlayer;
    public Vector2 bottomOffset;
    public Vector2 rightOffset;
    public Vector2 leftOffset;
    public float checkRaduis;//范围，一般0.2就行
    public LayerMask groundLayer;//要检测的ground，后面可以下拉选择图层

    [Header("状态")]
    public bool isGround;//看object是否在ground上面
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;


    private void Awake()
    {
        coll  = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        if (!manual)
        {
            rightOffset  = new Vector2 ((coll.bounds.size.x+coll.offset.x)/2, coll.bounds.size.y / 2 );//整体尺寸(碰撞体宽度)+碰撞体相对物体中心的偏移(位移  的一半
            leftOffset = new Vector2 (-rightOffset.x, rightOffset.y);//高度一样，只是左右两边位置不一样
        }

        if(isPlayer)
            playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Check();
    }


    //通过碰撞体检测来判断
    private void Check()
    {
        if (onWall)
        {
            //检测地面,以transform.position+-checkRaduis为检测范围，看是否碰撞groundLayer
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRaduis, groundLayer);

        }
        else
        {
            //检测地面,以transform.position+-checkRaduis为检测范围，看是否碰撞groundLayer
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, 0), checkRaduis, groundLayer);

        }

        //墙体判断
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset.x,leftOffset.y), checkRaduis, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRaduis, groundLayer);
         
        //在墙上且不再地上
        if(isPlayer)
             onWall = (touchLeftWall && playerController.inputDirection.x<0f||touchRightWall && playerController.inputDirection.x > 0f) && rb.velocity.y<0f;
    }

        


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRaduis);
    }
}

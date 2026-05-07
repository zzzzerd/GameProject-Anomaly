using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    [Header("检测参数")]
    public Vector2 bottomOffset;
    public float checkRaduis;//范围，一般0.2就行
    public LayerMask groundLayer;//要检测的ground，后面可以下拉选择图层

    [Header("状态")]
    public bool isGround;//看object是否在ground上面


    // Update is called once per frame
    void Update()
    {
        Check();
    }


    //通过碰撞体检测来判断
    private void Check()
    {
        //检测地面,以transform.position+-checkRaduis为检测范围，看是否碰撞groundLayer
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
    }
}

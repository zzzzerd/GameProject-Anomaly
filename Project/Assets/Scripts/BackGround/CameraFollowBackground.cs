using UnityEngine;

/// <summary>
/// 始终跟随相机，保持固定偏移，适用于月亮等不动的背景元素
/// </summary>
public class CameraFollowBackground : MonoBehaviour
{
    [Header("相对相机的偏移")]
    public Vector3 offset;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("[CameraFollowBackground] 找不到 Camera.main！请确认相机有 MainCamera 标签");
            return;
        }
        // offset 直接作为相对相机的固定偏移，不依赖初始世界位置
        // 在 Inspector 设置 offset 来控制月亮在视野中的位置
        transform.SetPositionAndRotation(mainCamera.transform.position + offset, transform.rotation);
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;
        // 用 SetParent 解除父物体影响，直接设世界坐标
        transform.SetPositionAndRotation(mainCamera.transform.position + offset, transform.rotation);
    }
}

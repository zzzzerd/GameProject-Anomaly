using System.Collections;
using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [Header("相机引用")]
    public Transform cameraTransform;

    [Header("视差系数 (0:跟随相机不动, 1:完全静止在世界坐标)")]
    [Range(0f, 1f)]
    public float parallaxEffect;

    private float startPosX;
    private float textureSizeX;
    private bool isReady;

    void Start()
    {
        StartCoroutine(InitRoutine());
    }

    private IEnumerator InitRoutine()
    {
        yield return null;

        if (cameraTransform == null)
        {
            // 优先找 VirtualCamera（Cinemachine），因为它才是实际跟随玩家的相机
            Camera[] cams = GameObject.FindObjectsOfType<Camera>();
            foreach (var c in cams)
            {
                if (c.gameObject.CompareTag("VirtualCamera") && c.isActiveAndEnabled)
                {
                    cameraTransform = c.transform;
                    break;
                }
            }

            // 回退：找 MainCamera
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        if (cameraTransform == null)
        {
            Debug.LogError("[InfiniteBackground] 没有找到可用 Camera！");
            yield break;
        }

        Debug.Log($"[InfiniteBackground] 使用相机: {cameraTransform.name}, Tag={cameraTransform.tag}, orthoSize={(cameraTransform.GetComponent<Camera>()?.orthographicSize)}");

        startPosX = transform.position.x;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            textureSizeX = spriteRenderer.bounds.size.x;
            Debug.Log($"[InfiniteBackground] 背景贴图宽度: {textureSizeX}, 当前对象: {gameObject.name}");
        }

        isReady = true;
    }

    void LateUpdate()
    {
        if (!isReady || cameraTransform == null) return;

        float distanceToMove = cameraTransform.position.x * parallaxEffect;
        transform.position = new Vector3(startPosX + distanceToMove, transform.position.y, transform.position.z);

        float tempPosition = cameraTransform.position.x * (1 - parallaxEffect);

        if (tempPosition > startPosX + textureSizeX)
        {
            startPosX += textureSizeX;
        }
        else if (tempPosition < startPosX - textureSizeX)
        {
            startPosX -= textureSizeX;
        }
    }
}

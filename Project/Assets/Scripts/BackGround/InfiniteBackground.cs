using System.Collections;
using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [Header("视差系数 (0:完全跟随玩家, 1:完全静止)")]
    [Range(0f, 1f)]
    public float parallaxEffect;

    [Header("无限滚动触发距离 (默认为贴图宽度，可调小提前衔接)")]
    public float scrollThreshold = 0f;

    [Header("Y轴跟随")]
    public bool followY = false;
    [Range(0f, 1f)]
    public float parallaxEffectY = 0.5f;

    private Transform playerTransform;
    private float textureSizeX;
    private bool isReady;

    private float lastTargetX;
    private float lastTargetY;

    void Start()
    {
        StartCoroutine(InitRoutine());
    }

    private IEnumerator InitRoutine()
    {
        // 确保等场景加载器完全准备好
        yield return null;
        yield return null;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            textureSizeX = sr.bounds.size.x;
            if (scrollThreshold <= 0f)
                scrollThreshold = textureSizeX; // 默认用贴图宽度
        }

        // 优先通过 ScenesLoader 获取玩家
        ScenesLoader loader = FindObjectOfType<ScenesLoader>();
        if (loader != null && loader.playerTransform != null)
        {
            playerTransform = loader.playerTransform;
        }
        else
        {
            // 回退方式
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
            else if (Camera.main != null)
                playerTransform = Camera.main.transform; // 最后回退找主相机
        }

        if (playerTransform == null)
        {
            Debug.LogError("[InfiniteBackground] 依然找不到参考目标(Player或相机)！");
            yield break;
        }

        // 初始对齐到目标
        transform.position = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);
        lastTargetX = playerTransform.position.x;
        lastTargetY = playerTransform.position.y;

        isReady = true;
    }

    void LateUpdate()
    {
        if (!isReady || playerTransform == null) return;

        float deltaX = playerTransform.position.x - lastTargetX;
        lastTargetX = playerTransform.position.x;

        float deltaY = playerTransform.position.y - lastTargetY;
        lastTargetY = playerTransform.position.y;

        float moveY = followY ? deltaY * (1f - parallaxEffectY) : 0f;
        transform.position += new Vector3(deltaX * (1f - parallaxEffect), moveY, 0);

        float dist = transform.position.x - playerTransform.position.x;
        if (dist > scrollThreshold)
            transform.position -= new Vector3(textureSizeX, 0, 0);
        else if (dist < -scrollThreshold)
            transform.position += new Vector3(textureSizeX, 0, 0);
    }
}

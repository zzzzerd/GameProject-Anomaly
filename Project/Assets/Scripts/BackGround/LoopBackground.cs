using System.Collections;
using UnityEngine;

/// <summary>
/// 无限循环背景：自动生成左右两份复制，形成三张连续
/// </summary>
public class LoopBackground : MonoBehaviour
{
    [Header("视差系数 (0:完全跟随玩家, 1:完全静止)")]
    [Range(0f, 1f)]
    public float parallaxEffect;

    [Header("偏移调整 (微调背景位置)")]
    public float offsetAdjust = 0f;

    [Header("左复制偏移 (调小往左移，调大往右移)")]
    public float leftOffset = 0f;

    [Header("右复制偏移 (调小往左移，调大往右移)")]
    public float rightOffset = 0f;

    private Transform playerTransform;
    private float textureSizeX;
    private bool isReady;

    private float lastPlayerX;
    private GameObject leftCopy;
    private GameObject rightCopy;

    void Start()
    {
        StartCoroutine(InitRoutine());
    }

    void OnDestroy()
    {
        if (leftCopy != null) Destroy(leftCopy);
        if (rightCopy != null) Destroy(rightCopy);
    }

    private IEnumerator InitRoutine()
    {
        yield return null;
        yield return null;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            textureSizeX = sr.bounds.size.x;
            Debug.Log($"[LoopBackground] 贴图实际宽度: {textureSizeX}, Sprite: {sr.sprite?.name}");
        }

        ScenesLoader loader = FindObjectOfType<ScenesLoader>();
        if (loader != null && loader.playerTransform != null)
        {
            playerTransform = loader.playerTransform;
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTransform = playerObj.transform;
            else if (Camera.main != null)
                playerTransform = Camera.main.transform;
        }

        if (playerTransform == null)
        {
            Debug.LogError("[LoopBackground] 找不到参考目标！");
            yield break;
        }

        // 创建左右复制
        CreateCopies();

        // 初始对齐到玩家
        float initX = playerTransform.position.x;
        transform.position = new Vector3(initX + offsetAdjust, transform.position.y, transform.position.z);
        lastPlayerX = playerTransform.position.x;

        isReady = true;
    }

    void CreateCopies()
    {
        // 左边复制
        leftCopy = Instantiate(gameObject, transform.parent);
        leftCopy.transform.position = new Vector3(transform.position.x - textureSizeX + leftOffset, transform.position.y, transform.position.z);
        Destroy(leftCopy.GetComponent<LoopBackground>()); // 移除脚本避免递归

        // 右边复制
        rightCopy = Instantiate(gameObject, transform.parent);
        rightCopy.transform.position = new Vector3(transform.position.x + textureSizeX + rightOffset, transform.position.y, transform.position.z);
        Destroy(rightCopy.GetComponent<LoopBackground>());

        Debug.Log($"[LoopBackground] 中间X: {transform.position.x:F2}, 左X: {leftCopy.transform.position.x:F2}, 右X: {rightCopy.transform.position.x:F2}");
        Debug.Log($"[LoopBackground] 左边距: {transform.position.x - leftCopy.transform.position.x:F2}, 右边距: {rightCopy.transform.position.x - transform.position.x:F2}");
    }

    void LateUpdate()
    {
        if (!isReady || playerTransform == null) return;

        float delta = playerTransform.position.x - lastPlayerX;
        lastPlayerX = playerTransform.position.x;

        // 三张一起移动
        float moveX = delta * (1f - parallaxEffect);
        transform.position += new Vector3(moveX, 0, 0);
        leftCopy.transform.position += new Vector3(moveX, 0, 0);
        rightCopy.transform.position += new Vector3(moveX, 0, 0);

        // 当中心背景离开玩家太远，整体平移一个贴图宽度
        float dist = transform.position.x - playerTransform.position.x;
        if (dist > textureSizeX)
        {
            transform.position -= new Vector3(textureSizeX, 0, 0);
            leftCopy.transform.position -= new Vector3(textureSizeX, 0, 0);
            rightCopy.transform.position -= new Vector3(textureSizeX, 0, 0);
        }
        else if (dist < -textureSizeX)
        {
            transform.position += new Vector3(textureSizeX, 0, 0);
            leftCopy.transform.position += new Vector3(textureSizeX, 0, 0);
            rightCopy.transform.position += new Vector3(textureSizeX, 0, 0);
        }
    }
}

using UnityEngine;

/// <summary>
/// 视差背景系统 - 根据相机移动产生不同速度的水平偏移
/// 支持无限循环（Infinite Scrolling），适合横版长地图
/// 
/// 架构：Main Camera 在持久化场景中，背景在 Additive 动态加载场景中。
/// 背景所在的场景被加载激活后，Start 中自动获取相机并初始化。
/// 场景被卸载时背景随之销毁，无需手动管理。
/// 
/// 使用方法：
/// 1. 挂载到每层背景的父 GameObject 上
/// 2. 设置 parallaxFactor（0=固定, 1=与相机同步移动）
/// 3. 将背景 SpriteRenderer 设为该 GameObject 的子物体
/// </summary>
[DisallowMultipleComponent]
public class ParallaxBackground : MonoBehaviour
{
    [Header("视差参数")]
    [Tooltip("视差因子: 0=完全静止, 1=与相机同步移动\n推荐: Sky=0.05, FarMountains=0.2, NearMountains=0.4, Trees=0.6")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxFactor = 0.5f;

    [Header("无限循环（可选）")]
    [Tooltip("是否启用无限滚动。启用后背景会循环出现，适合长地图")]
    [SerializeField] private bool infiniteScrolling = false;

    [Tooltip("单个背景块的宽度（世界单位）。必须与实际 Sprite 宽度一致")]
    [SerializeField] private float spriteWidth = 20f;

    [Tooltip("左右各额外生成几块背景。推荐值为 1~2")]
    [SerializeField] private int extraCopies = 1;

    [Header("调试")]
    [SerializeField] private bool showDebugLog = false;

    // 私有变量
    private Transform _cameraTransform;
    private Vector3 _initialPosition;   // 背景初始世界位置
    private Vector3 _cameraStartPos;    // 相机初始世界位置

    private Transform[] _clones;        // 无限循环的克隆体
    private int _totalCopies;           // 总块数（1 + 左右各 extraCopies）
    private bool _isInitialized = false;

    // 诊断用
    private int _frameCount = 0;

    private void Awake()
    {
        // 记录背景在编辑器中的初始位置（不依赖相机）
        _initialPosition = transform.position;
    }

    private void Start()
    {
        // 场景被加载激活后，延迟几帧等 CinemachineBrain 把 Virtual Camera 位置同步到 Main Camera
        // 之后记录相机当前位置作为视差基准，开始跟随
        StartCoroutine(InitAfterCameraReady());
    }

    private System.Collections.IEnumerator InitAfterCameraReady()
    {
        // 等待 2 帧：让场景完全激活 + CinemachineBrain 完成 LateUpdate 同步
        yield return null;
        yield return null;

        // 获取主相机
        _cameraTransform = Camera.main?.transform;
        if (_cameraTransform == null)
        {
            Debug.LogError($"[ParallaxBackground] {gameObject.name} 找不到 Main Camera（Tag 需为 MainCamera）");
            yield break;
        }

        // 记录相机当前位置作为视差基准
        _cameraStartPos = _cameraTransform.position;

        // 无限循环初始化
        if (infiniteScrolling)
        {
            SetupInfiniteScrolling();
        }

        _isInitialized = true;
        _frameCount = Time.frameCount;

        Debug.Log($"[ParallaxBackground] {gameObject.name} 初始化完成 | Frame={_frameCount} | Factor={parallaxFactor} | CameraStartPos=({_cameraStartPos.x:F2}, {_cameraStartPos.y:F2}) | InitialPos=({_initialPosition.x:F2}, {_initialPosition.y:F2})");
    }

    private void LateUpdate()
    {
        if (!_isInitialized || _cameraTransform == null) return;

        ApplyParallax();
    }

    /// <summary>
    /// 核心视差计算
    /// 所有背景块（原物体 + 克隆体）统一按视差因子偏移
    /// 公式: 背景新位置 = 初始位置 + 相机累计位移 * parallaxFactor
    /// </summary>
    private void ApplyParallax()
    {
        float parallaxOffsetX = (_cameraTransform.position.x - _cameraStartPos.x) * parallaxFactor;

        // 更新原物体位置
        Vector3 newPos = _initialPosition;
        newPos.x += parallaxOffsetX;
        transform.position = newPos;

        // 同步更新所有克隆体位置（统一偏移）
        if (_clones != null)
        {
            for (int i = 0; i < _clones.Length; i++)
            {
                if (_clones[i] == null) continue;
                int offsetIndex = (i < extraCopies) ? -(extraCopies - i) : (i - extraCopies + 1);
                Vector3 clonePos = _initialPosition;
                clonePos.x += parallaxOffsetX + offsetIndex * spriteWidth;
                _clones[i].position = clonePos;
            }
        }
    }

    #region 无限循环 (Infinite Scrolling)

    /// <summary>
    /// 初始化无限循环：在左右两侧创建克隆体
    /// </summary>
    private void SetupInfiniteScrolling()
    {
        if (spriteWidth <= 0f)
        {
            Debug.LogError($"[ParallaxBackground] spriteWidth 必须大于 0，已禁用无限循环");
            infiniteScrolling = false;
            return;
        }

        _totalCopies = 1 + extraCopies * 2;
        _clones = new Transform[_totalCopies - 1];

        // 获取子物体的 SpriteRenderer
        SpriteRenderer originalRenderer = GetComponentInChildren<SpriteRenderer>();

        int cloneIndex = 0;
        for (int i = -extraCopies; i <= extraCopies; i++)
        {
            if (i == 0) continue;

            GameObject clone;
            if (originalRenderer != null)
            {
                clone = Instantiate(originalRenderer.gameObject, transform);
            }
            else
            {
                clone = new GameObject($"{gameObject.name}_Clone_{(i > 0 ? "R" : "L")}{Mathf.Abs(i)}");
                clone.transform.SetParent(transform);
            }

            clone.name = $"{gameObject.name}_Clone_{(i > 0 ? "R" : "L")}{Mathf.Abs(i)}";
            clone.transform.localPosition = new Vector3(i * spriteWidth, 0f, 0f);
            clone.transform.localScale = Vector3.one;

            _clones[cloneIndex] = clone.transform;
            cloneIndex++;
        }
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 运行时动态修改视差因子
    /// </summary>
    public void SetParallaxFactor(float factor)
    {
        parallaxFactor = Mathf.Clamp01(factor);
    }

    /// <summary>
    /// 重置背景到初始位置（场景切换时调用）。
    /// 更新相机基准位置和背景位置，使视差在新场景中正常工作。
    /// </summary>
    public void ResetPosition()
    {
        if (_cameraTransform == null && Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }

        if (_cameraTransform != null)
        {
            _cameraStartPos = _cameraTransform.position;
        }

        transform.position = _initialPosition;

        Debug.Log($"[ParallaxBackground] {gameObject.name} ResetPosition | CameraStartPos=({_cameraStartPos.x:F2}, {_cameraStartPos.y:F2}) | Pos=({transform.position.x:F2}, {transform.position.y:F2})");
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!infiniteScrolling || spriteWidth <= 0f) return;

        Gizmos.color = Color.cyan;
        Vector3 pos = Application.isPlaying ? _initialPosition : transform.position;

        // 绘制原始块范围
        Gizmos.DrawWireCube(pos, new Vector3(spriteWidth, 2f, 0f));

        // 绘制克隆体范围
        Gizmos.color = Color.yellow;
        for (int i = -extraCopies; i <= extraCopies; i++)
        {
            if (i == 0) continue;
            Vector3 clonePos = new Vector3(pos.x + i * spriteWidth, pos.y, pos.z);
            Gizmos.DrawWireCube(clonePos, new Vector3(spriteWidth, 2f, 0f));
        }
    }
#endif
}

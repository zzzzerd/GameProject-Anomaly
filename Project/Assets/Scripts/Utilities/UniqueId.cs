using UnityEngine;

/// <summary>
/// 为挂载的 GameObject 生成全局唯一 ID，保证不重复。
/// 
/// ID 生成规则：
/// - 编辑器下挂载时自动生成，保存到场景中
/// - 运行时 Instantiate 克隆体时自动生成新 ID
/// - 使用 GUID 保证全局唯一
/// </summary>
[DisallowMultipleComponent]
public class UniqueId : MonoBehaviour
{
    [Header("唯一 ID")]
    [SerializeField]
    [Tooltip("全局唯一标识符，由编辑器自动生成，请勿手动修改")]
    private string _uniqueId;

    /// <summary>
    /// 获取唯一 ID
    /// </summary>
    public string Id => _uniqueId;

    /// <summary>
    /// 已生成的 ID 集合，用于运行时去重
    /// </summary>
    private static System.Collections.Generic.HashSet<string> _usedIds = new System.Collections.Generic.HashSet<string>();

#if UNITY_EDITOR
    private void Reset()
    {
        GenerateNewId();
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_uniqueId))
        {
            GenerateNewId();
            return;
        }

        // 检查场景中是否有重复 ID
        var allIds = FindObjectsByType<UniqueId>(FindObjectsSortMode.None);
        foreach (var other in allIds)
        {
            if (other != this && other._uniqueId == _uniqueId)
            {
                Debug.LogWarning($"[UniqueId] 检测到重复 ID: {_uniqueId}，位于 {gameObject.name} 和 {other.gameObject.name}，正在重新生成...");
                GenerateNewId();
                break;
            }
        }
    }

    private void GenerateNewId()
    {
        string newId;
        do
        {
            newId = System.Guid.NewGuid().ToString();
        }
        while (_usedIds.Contains(newId));

        _uniqueId = newId;
        _usedIds.Add(newId);

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    private void Awake()
    {
        // 克隆体：生成新 ID
        if (string.IsNullOrEmpty(_uniqueId))
        {
            _uniqueId = System.Guid.NewGuid().ToString();
        }

        // 检查运行时重复
        if (_usedIds.Contains(_uniqueId))
        {
            string newId;
            do
            {
                newId = System.Guid.NewGuid().ToString();
            }
            while (_usedIds.Contains(newId));

            Debug.LogWarning($"[UniqueId] 运行时检测到重复 ID: {_uniqueId} → 已重新生成: {newId}，物体: {gameObject.name}");
            _uniqueId = newId;
        }

        _usedIds.Add(_uniqueId);
    }

    private void OnDestroy()
    {
        if (_usedIds.Contains(_uniqueId))
        {
            _usedIds.Remove(_uniqueId);
        }
    }

    /// <summary>
    /// 手动刷新生成新 ID（可通过右键菜单调用）
    /// </summary>
    [ContextMenu("刷新 UniqueId")]
    public void RegenerateId()
    {
#if UNITY_EDITOR
        if (_usedIds.Contains(_uniqueId))
            _usedIds.Remove(_uniqueId);

        GenerateNewId();
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[UniqueId] 已刷新 ID: {_uniqueId}，物体: {gameObject.name}");
#endif
    }

    /// <summary>
    /// 通过 ID 查找场景中的 GameObject
    /// </summary>
    public static GameObject FindById(string id)
    {
        var all = FindObjectsByType<UniqueId>(FindObjectsSortMode.None);
        foreach (var uid in all)
        {
            if (uid._uniqueId == id)
                return uid.gameObject;
        }
        return null;
    }

    public override string ToString()
    {
        return $"UniqueId: {_uniqueId}, GameObject: {gameObject.name}";
    }
}

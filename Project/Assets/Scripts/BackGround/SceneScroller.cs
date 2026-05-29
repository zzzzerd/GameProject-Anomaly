using UnityEngine;

public class SceneScroller : MonoBehaviour
{
    [SerializeField] SpriteRenderer tilemapRenderer;
    [SerializeField] Vector2 v2;
    [SerializeField] float speed;

    private void Awake()
    {
        tilemapRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() => tilemapRenderer.material.mainTextureOffset += v2 * speed * Time.deltaTime;
}
using UnityEngine;

public class Collectable : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    [Header("Tile Sprites")]
    public Sprite eaten;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateSprite()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = eaten;
        
    }
}

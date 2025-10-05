using UnityEngine;

public class GoalObject : MonoBehaviour
{
    [Header("Goal Sprites")]
    public Sprite defaultSprite;
    public Sprite alternateSprite;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the starting sprite if available
        if (defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
    }

    public void ChangeSprite()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = alternateSprite;
    }
}
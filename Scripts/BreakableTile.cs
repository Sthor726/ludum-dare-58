using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BreakableTile : MonoBehaviour
{
    private enum TileState { Light, Heavy, Broken }
    private TileState state = TileState.Light;

    private SpriteRenderer spriteRenderer;

    [Header("Tile Sprites")]
    public Sprite lightSprite;
    public Sprite heavySprite;
    public Sprite brokenSprite;

    [Header("Settings")]
    public bool disableCollidersOnBroken = true;
    public AudioClip crumbleSfx;

    private Collider2D[] myColliders;
    private SFXManager sfx;
    private LevelLoader levelLoader;
    private GridManager grid;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myColliders = GetComponents<Collider2D>();
        sfx = SFXManager.Instance;
        levelLoader = LevelLoader.Instance;
        grid = GridManager.Instance;

        UpdateSprite();
    }

    public void ChangeState(GameObject source)
    {
        Debug.Log($"[BreakableTile] ChangeState triggered by {source.name}. Current state={state}");

        switch (state)
        {
            case TileState.Light:
                state = TileState.Heavy;
                UpdateSprite();
                break;

            case TileState.Heavy:
                state = TileState.Broken;
                UpdateSprite();
                HandleBrokenTile(source);
                break;

            case TileState.Broken:
                HandleBrokenTile(source);
                break;
        }
    }

    private void HandleBrokenTile(GameObject source)
    {

        if (crumbleSfx && sfx)
            sfx.PlaySFX(crumbleSfx, 0.5f);
        else

        if (disableCollidersOnBroken)
        {
            foreach (var c in myColliders)
                if (c != null) c.enabled = false;

        }

        if (source.TryGetComponent(out PlayerController player))
        {
            if (sfx) sfx.PlayLoseSound();

            if (levelLoader)
            {
                Debug.Log("[BreakableTile] Reloading level...");
                StartCoroutine(levelLoader.LoadLevel(SceneManager.GetActiveScene().buildIndex, 1f));
            }

            if (grid) grid.isGameOver = true;
            player.SendMessage("OnFell", SendMessageOptions.DontRequireReceiver);
        }
        else if (source.TryGetComponent(out Enemy enemy))
        {
            Debug.Log("[BreakableTile] Enemy triggered break — destroying enemy.");
            Destroy(enemy.gameObject);
        }
        else
        {
            Debug.Log($"[BreakableTile] {source.name} is not Player or Enemy — no action taken.");
        }
    }

    private void UpdateSprite()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        switch (state)
        {
            case TileState.Light:
                spriteRenderer.sprite = lightSprite;
                break;
            case TileState.Heavy:
                spriteRenderer.sprite = heavySprite;
                break;
            case TileState.Broken:
                spriteRenderer.sprite = brokenSprite;
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.9f);
    }
}

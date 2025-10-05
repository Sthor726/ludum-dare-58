using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveDelay = 0.15f;
    private bool canMove = true;
    public AudioClip moveAudio;

    private Vector2Int gridPos;
    private GridManager grid;
    private SFXManager sfxManager;
    private LevelLoader levelLoader;

    private void Start()
    {
        grid = GridManager.Instance;
        sfxManager = SFXManager.Instance;
        levelLoader = LevelLoader.Instance;
        gridPos = grid.WorldToGrid(transform.position);
    }

    private void Update()
    {
        if (!canMove || grid.isGameOver) return;

        Vector2Int input = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W)) input = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) input = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) input = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) input = Vector2Int.right;
        else if (Input.GetKeyDown(KeyCode.R)) {
            StartCoroutine(levelLoader.LoadLevel(SceneManager.GetActiveScene().buildIndex, 0f));
        }

        if (input != Vector2Int.zero)
        {
            bool moved = TryMove(input);
            if (moved)
            {
                sfxManager.PlaySFX(moveAudio, 0.25f);
                // Move all enemies
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    enemy.MoveEnemy();
                }
            }
        }
    }

    private bool TryMove(Vector2Int dir)
    {
        Vector2Int targetPos = gridPos + dir;
        if (!grid.IsInBounds(targetPos)) return false;

        Vector2Int oldPos = gridPos;

        // What's currently at the target?
        Collider2D hit = Physics2D.OverlapPoint(grid.GridToWorld(targetPos));

        // Helpful debug if something is unexpected:
        // Debug.Log($"TryMove to {targetPos} hit: {(hit? hit.gameObject.name : "null")}");

        // Treat breakable tiles specially: allow stepping on them even if they have a collider.
        BreakableTile breakable = null;
        if (hit != null)
        {
            // Try direct component on hit, then parent (covers tilemap child cases)
            hit.TryGetComponent(out breakable);
            if (breakable == null)
                breakable = hit.GetComponentInParent<BreakableTile>();
        }

        // Don't allow movement into grid obstacles unless it's a breakable tile we permit stepping on
        if (grid.IsObstacle(targetPos) && breakable == null)
            return false;

        // If there's a pushable object, try to push
        if (hit != null && hit.TryGetComponent(out PushableObject pushable))
        {
            Vector2Int pushTarget = targetPos + dir;
            if (!grid.IsInBounds(pushTarget) || grid.IsObstacle(pushTarget)) return false;

            Collider2D pushHit = Physics2D.OverlapPoint(grid.GridToWorld(pushTarget));
            if (pushHit == null)
            {
                pushable.MoveTo(pushTarget);
                MoveTo(targetPos);
                TryPull(oldPos, dir);
                return true;
            }

            return false;
        }

        // If it's a breakable tile (or a child of one), step onto it and trigger state change
        if (breakable != null)
        {
            MoveTo(targetPos);
            breakable.ChangeState(gameObject);
            TryPull(oldPos, dir);
            return true;
        }

        // If nothing blocking, just move
        if (hit == null)
        {
            MoveTo(targetPos);
            TryPull(oldPos, dir);
            return true;
        }

        // Otherwise some other non-pushable thing is there — block movement
        return false;
    }

    private void TryPull(Vector2Int oldPos, Vector2Int dir)
    {
        if (!Input.GetKey(KeyCode.K)) return;

        Vector2Int behindPos = oldPos - dir; // opposite direction
        Collider2D behindHit = Physics2D.OverlapPoint(grid.GridToWorld(behindPos));
        if (behindHit != null && behindHit.TryGetComponent(out PushableObject pullable))
        {
            // Make sure oldPos is free (ignore player collider)
            Collider2D[] colliders = Physics2D.OverlapPointAll(grid.GridToWorld(oldPos));
            bool tileFree = true;
            foreach (var c in colliders)
            {
                if (c.gameObject != gameObject) // ignore the player itself
                {
                    tileFree = false;
                    break;
                }
            }

            if (tileFree)
            {
                pullable.MoveTo(oldPos);
                Debug.Log($"Pulled {pullable.name} into {oldPos}");
            }
            else
            {
                Debug.Log($"Cannot pull {pullable.name}: player tile occupied by something else");
            }
        }
    }

    private void MoveTo(Vector2Int newPos)
    {
        gridPos = newPos;
        transform.position = grid.GridToWorld(gridPos);

        if (grid.IsGoal(gridPos))
            Debug.Log("Reached the goal!");

        StartCoroutine(MoveCooldown());
    }

    private IEnumerator MoveCooldown()
    {
        canMove = false;
        yield return new WaitForSeconds(moveDelay);
        canMove = true;
    }
}

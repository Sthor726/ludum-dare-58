using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    private Vector2Int gridPos;
    private GridManager grid;
    public Transform collectable;
    private SFXManager sfx;
    private LevelLoader levelLoader;
    public AudioClip gameOverSound;
    public AudioClip gameOverJingle;
    public float timeAfterDeath = 1f;

    private void Start()
    {
        grid = GridManager.Instance;
        sfx = SFXManager.Instance;
        gridPos = grid.WorldToGrid(transform.position);
        levelLoader = LevelLoader.Instance;
    }

    public void MoveEnemy()
    {
        if (collectable == null) {
            return;
        }

        Vector2Int target = grid.WorldToGrid(collectable.position);
    
        // Try BFS pathfinding first
        Vector2Int? nextStep = GetNextStepTowards(gridPos, target);

        if (!nextStep.HasValue)
        {
            // No path? fallback to greedy move toward collectable
            nextStep = GetGreedyStep(gridPos, target);
        }

        if (nextStep.HasValue)
        {
            gridPos = nextStep.Value;
            transform.position = grid.GridToWorld(gridPos);

            Collider2D tileHit = Physics2D.OverlapPoint(transform.position);
            if (tileHit != null && tileHit.TryGetComponent(out BreakableTile breakableTile))
            {
                breakableTile.ChangeState(gameObject);
            }

            // Game over if adjacent
            if (Vector2Int.Distance(gridPos, target) == 1)
            {
                sfx.PlaySFX(gameOverSound, 0.25f);
                sfx.PlaySFX(gameOverJingle, 1f);
                grid.isGameOver = true;
                collectable.GetComponent<GoalObject>().ChangeSprite();

                StartCoroutine(levelLoader.LoadLevel(SceneManager.GetActiveScene().buildIndex, 1f));

            }
        }
    }

    private Vector2Int? GetNextStepTowards(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        frontier.Enqueue(start);
        cameFrom[start] = start;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == goal) break;

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                if (!grid.IsInBounds(next) || grid.IsObstacle(next))
                    continue;

                // Avoid pushable objects
                Collider2D hit = Physics2D.OverlapPoint(grid.GridToWorld(next));
                if (hit != null && hit.TryGetComponent(out PushableObject _))
                    continue;

                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
            return null; // No path found

        // Backtrack to find next step
        Vector2Int step = goal;
        while (cameFrom[step] != start)
            step = cameFrom[step];

        return step;
    }

    private Vector2Int? GetGreedyStep(Vector2Int start, Vector2Int goal)
    {
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        Vector2Int bestMove = start;
        float bestDist = Vector2Int.Distance(start, goal);

        foreach (var dir in directions)
        {
            Vector2Int next = start + dir;

            if (!grid.IsInBounds(next) || grid.IsObstacle(next))
                continue;

            Collider2D hit = Physics2D.OverlapPoint(grid.GridToWorld(next));
            if (hit != null && hit.TryGetComponent(out PushableObject _))
                continue;

            float dist = Vector2Int.Distance(next, goal);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestMove = next;
            }
        }

        if (bestMove != start)
            return bestMove;

        return null; // Can't move closer
    }

}


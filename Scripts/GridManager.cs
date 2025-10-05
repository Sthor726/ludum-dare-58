using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public Vector2 origin = Vector2.zero;
    public LayerMask obstacleLayer;
    public Vector2Int goalPosition = new Vector2Int(5, 5);
    public bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - origin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y - origin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector2 GridToWorld(Vector2Int gridPos)
    {
        float x = origin.x + gridPos.x * cellSize;
        float y = origin.y + gridPos.y * cellSize;
        return new Vector2(x, y);
    }

    public bool IsInBounds(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < width && gridPos.y >= 0 && gridPos.y < height;
    }

    public bool IsGoal(Vector2Int gridPos)
    {
        return gridPos == goalPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                Vector3 pos = new Vector3(origin.x + x * cellSize, origin.y + y * cellSize, 0);
                Gizmos.DrawWireCube(pos, Vector3.one * cellSize);
            }
        }

        // Draw goal
        Gizmos.color = Color.green;
        Vector2 goalWorld = GridToWorld(goalPosition);
        Gizmos.DrawCube(goalWorld, Vector3.one * cellSize * 0.9f);
    }

    public bool IsObstacle(Vector2Int gridPos)
    {
        Vector2 worldPos = GridToWorld(gridPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos, obstacleLayer);
        return hit != null;
    }

}

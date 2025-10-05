using UnityEngine;
using UnityEngine.SceneManagement;

public class PushableObject : MonoBehaviour
{
    private GridManager grid;
    private Vector2Int gridPos;
    private LevelLoader levelLoader;
    private SFXManager sfx;

    private void Start()
    {
        grid = GridManager.Instance;
        levelLoader = LevelLoader.Instance;
        sfx = SFXManager.Instance;
        gridPos = grid.WorldToGrid(transform.position);
    }

    public void MoveTo(Vector2Int newPos)
    {
        gridPos = newPos;
        transform.position = grid.GridToWorld(gridPos);

        if (grid.IsGoal(gridPos))
        {
            if (gameObject.tag == "Collectable") {
                Debug.Log($"{name} reached goal! Collectable cleared!");
                sfx.PlayWinSound();
                StartCoroutine(levelLoader.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1, 1f));
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }

        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public static LevelLoader Instance;
    
    private void Awake() { Instance = this; }

    public void LoadNextLevel() {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1, 0f));
    }
    

    public IEnumerator LoadLevel(int levelIndex, float timeBefore) {
        yield return new WaitForSeconds(timeBefore);

        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        if (IsValidBuildIndex(levelIndex)) {
            SceneManager.LoadScene(levelIndex);
        } else {
            SceneManager.LoadScene(0);

        }
    }

    bool IsValidBuildIndex(int index)
    {
        return !string.IsNullOrEmpty(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(index));
    }

}

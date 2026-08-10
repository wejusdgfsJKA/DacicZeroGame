using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour {
    public int counter = 3;
    public void Interaction() {
        counter--;
        if (counter <= 0) {
            Debug.Log("Level end");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1;
        }
    }
}

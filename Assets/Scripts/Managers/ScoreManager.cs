using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static int score;
    Text text;
    bool levelChanged = false;

    void Awake()
    {
        text = GetComponent<Text>();
        score = 0;
    }

    void Update()
    {
        text.text = "Score: " + score;

        if (score >= 500 && !levelChanged)
        {
            levelChanged = true;
            LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Cargar siguiente escena según la actual
        if (currentScene == "Level 01")
        {
            SceneManager.LoadScene("Level 02");
        }
        else if (currentScene == "Level 02")
        {
            SceneManager.LoadScene("Level 03");
        }
        else
        {
            Debug.Log("Último nivel alcanzado o nombre desconocido.");
        }
    }
}

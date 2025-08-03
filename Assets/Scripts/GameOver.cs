using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI highScoreText; // Arraste o TextMeshProUGUI no Inspector
    private UIManager uiManager;

    void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();

        float highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        highScoreText.text = "Highscore: " + Mathf.RoundToInt(highScore);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Restaura o tempo normal
        Time.fixedDeltaTime = 0.02f; // Valor padrão do Unity para física
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // Carrega a cena do menu principal
        Time.timeScale = 1f; // Também importante garantir que o tempo volte ao normal
        Time.fixedDeltaTime = 0.02f;
        SceneManager.LoadScene("MainMenu");
    }

}

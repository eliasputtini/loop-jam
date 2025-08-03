using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for managing the UI elements of the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    public Image fuelImageProgressBar;
    public Gradient fuelGradient;
    public Image lowFuelWarning;

    public Text distanceCounter;
    public Text boostCounter;
    public Text rpmCounter;
    public Text fuelCounter;

    public CollectiblesManager collectiblesManager;
    private Vector3 _startPosition;
    public Button sfxButton;
    public Sprite sfxButtonOn;
    public Sprite sfxButtonOff;
    public AudioManager audioManager;
    public Image coinSprite;
    private bool _isPaused = false;
    private bool _gameOver = false;

    public Text timerText;

    // New UI element for score
    public Text scoreCounter;

    // Parameters for score text effect
    public Color normalScoreColor = Color.white;
    public Color highVelocityColor = Color.yellow;
    public float normalFontSize = 24f;
    public float highVelocityFontSize = 36f;

    // Added private field to accumulate score
    private float score = 0f;
    private Vector3 _lastPosition;

    public void UpdateTimerUI(float timeLeft)
    {
        timerText.text = Mathf.CeilToInt(timeLeft).ToString(); // round up
    }

    private void Start()
    {
        _startPosition = collectiblesManager.carBody.transform.position;
        _lastPosition = _startPosition;
        score = 0f;
    }

    /// <summary>
    /// This method is responsible for updating the all the UI elements.
    /// </summary>
    /// <returns>
    /// void
    /// </returns>
    private void Update()
    {
        // Detecta se a tecla ESC foi pressionada
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

        CheckLowFuelWarning();

        UpdateFuelGUI();

        UpdateRpmCounter();

        UpdateScore();
    }

    private void UpdateRpmCounter()
    {
        var distance = collectiblesManager.carBody.transform.position - _startPosition;

        distanceCounter.text = distance.magnitude.ToString("F0") + "m";

        // Convert velocity from m/s to km/h by multiplying by 3.6
        float velocityKmh = collectiblesManager.carBody.linearVelocity.magnitude * 3.6f;
        rpmCounter.text = velocityKmh.ToString("F0") + " km/h";
    }

    private void CheckLowFuelWarning()
    {
        if (collectiblesManager.fuel < 20)
        {
            lowFuelWarning.enabled = true;
            lowFuelWarning.color = new Color(lowFuelWarning.color.r,
                lowFuelWarning.color.g, lowFuelWarning.color.b, Mathf.PingPong(Time.time, 0.5f));
        }
        else
        {
            lowFuelWarning.enabled = false;
        }
    }

    // --- Updated to accumulate score over time ---
    private void UpdateScore()
    {
        Vector3 currentPos = collectiblesManager.carBody.transform.position;
        Vector3 frameDistanceVector = currentPos - _lastPosition;
        float frameDistance = frameDistanceVector.magnitude;

        float velocity = collectiblesManager.carBody.linearVelocity.magnitude;

        // Add only the incremental distance traveled this frame times velocity
        score += frameDistance * velocity;

        scoreCounter.text = "SCORE: " + Mathf.RoundToInt(score).ToString();

        // Convert velocity to km/h for the high velocity check
        float velocityKmh = velocity * 3.6f;
        if (velocityKmh > 36f)  // 36 km/h (equivalent to 10 m/s)
        {
            scoreCounter.color = highVelocityColor;
            scoreCounter.fontSize = (int)highVelocityFontSize;
        }
        else
        {
            scoreCounter.color = normalScoreColor;
            scoreCounter.fontSize = (int)normalFontSize;
        }

        _lastPosition = currentPos;

        if (score > PlayerPrefs.GetFloat("HighScore", 0f))
        {
            PlayerPrefs.SetFloat("HighScore", score);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// This method is responsible for restarting the level. Called when the player runs out of fuel.
    /// </summary>
    /// <returns>
    /// void
    /// </returns>
    public static void RestartLevel()
    {
        Time.timeScale = 1f; // Restaura o tempo normal
        Time.fixedDeltaTime = 0.02f; // Valor padrão do Unity para física
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// This method is responsible for pausing the game. It toggles the Time.timeScale between 0 and 1. Called when ESC key is pressed.
    /// Only works if the game is not over.
    /// </summary>
    /// <returns>
    /// void
    /// </returns>
    public void PauseGame()
    {
        // Só permite pausar se o jogo não acabou
        if (_gameOver) return;

        if (!_isPaused)
        {
            Time.timeScale = 0;
            _isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            _isPaused = false;
        }
    }

    /// <summary>
    /// Call this method when the game ends (time runs out, fuel runs out, etc.)
    /// </summary>
    public void SetGameOver()
    {
        _gameOver = true;
        _isPaused = false; // Garante que não está pausado quando o jogo acaba
    }

    private void UpdateFuelGUI()
    {
        fuelImageProgressBar.fillAmount = collectiblesManager.fuel / 100f;

        fuelImageProgressBar.color = fuelGradient.Evaluate(collectiblesManager.fuel / 100f);

        fuelCounter.text = collectiblesManager.fuel.ToString("F0") + "%";

        fuelCounter.color = fuelGradient.Evaluate(collectiblesManager.fuel / 100f);
    }

    public void ToggleSfx()
    {
        sfxButton.image.sprite = sfxButton.image.sprite == sfxButtonOn ? sfxButtonOff : sfxButtonOn;
        audioManager.ToggleAllSfx();
    }

    public float GetHighScore()
    {
        return PlayerPrefs.GetFloat("HighScore", 0f);
    }
}